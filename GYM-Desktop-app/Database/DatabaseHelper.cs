using GYM_Desktop_app.Helpers;
using GYM_Desktop_app.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace GYM_Desktop_app.Database
{
    // ============================================================
    //  UFA data layer  -  SQLite (embedded, no server needed)
    // ============================================================
    public static class DatabaseHelper
    {
        private static string _dbPath;

        // Called once at startup (Program.cs) with the writable DB file path.
        public static void SetDatabasePath(string path) => _dbPath = path;

        private static string ConnString =>
            $"Data Source={_dbPath};Version=3;Foreign Keys=True;";

        public static SQLiteConnection GetConnection() => new SQLiteConnection(ConnString);

        private static void Exec(SQLiteConnection c, string sql)
        {
            using (var cmd = new SQLiteCommand(sql, c)) cmd.ExecuteNonQuery();
        }

        // ===== SCHEMA + SEED =====
        public static void EnsureSchema()
        {
            if (string.IsNullOrEmpty(_dbPath))
                throw new InvalidOperationException("Database path not set.");

            using (var c = GetConnection())
            {
                c.Open();
                Exec(c, @"CREATE TABLE IF NOT EXISTS Users(
                            UserID   INTEGER PRIMARY KEY AUTOINCREMENT,
                            Username TEXT UNIQUE,
                            Password TEXT,
                            Role     TEXT);");
                Exec(c, @"CREATE TABLE IF NOT EXISTS Trainers(
                            TrainerID INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name      TEXT,
                            Specialty TEXT,
                            Phone     TEXT);");
                Exec(c, @"CREATE TABLE IF NOT EXISTS Plans(
                            PlanID    INTEGER PRIMARY KEY AUTOINCREMENT,
                            TrainerID INTEGER,
                            PlanName  TEXT,
                            Sessions  INTEGER,
                            Price     REAL);");
                Exec(c, @"CREATE TABLE IF NOT EXISTS Members(
                            MemberID          INTEGER PRIMARY KEY AUTOINCREMENT,
                            UserID            INTEGER,
                            Name              TEXT,
                            Phone             TEXT,
                            Age               INTEGER,
                            Address           TEXT,
                            JoinDate          TEXT,
                            TrainerID         INTEGER,
                            PlanID            INTEGER,
                            SessionsTotal     INTEGER,
                            SessionsRemaining INTEGER);");
                Exec(c, @"CREATE TABLE IF NOT EXISTS Payments(
                            PaymentID INTEGER PRIMARY KEY AUTOINCREMENT,
                            MemberID  INTEGER,
                            PlanID    INTEGER,
                            Amount    REAL,
                            Date      TEXT,
                            Method    TEXT);");
                // migrate older DBs that predate the PlanID column
                try { Exec(c, "ALTER TABLE Payments ADD COLUMN PlanID INTEGER"); } catch { }
                Exec(c, @"CREATE TABLE IF NOT EXISTS SessionLog(
                            SessionID INTEGER PRIMARY KEY AUTOINCREMENT,
                            MemberID  INTEGER,
                            UsedAt    TEXT);");
            }

            SeedAdmin();
            SeedCoachesAndPlans();
        }

        private static string Now() => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // ===== PASSWORD HASHING =====
        public static string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        public static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(hash) || !hash.StartsWith("$2")) return false;
            try { return BCrypt.Net.BCrypt.Verify(password, hash); }
            catch { return false; }
        }

        // ===== SEED =====
        public static void SeedAdmin()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Users WHERE Role='Admin'", conn))
                {
                    if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                    {
                        using (var ins = new SQLiteCommand(
                            "INSERT INTO Users (Username, Password, Role) VALUES ('admin', @p, 'Admin')", conn))
                        {
                            ins.Parameters.AddWithValue("@p", HashPassword("admin123"));
                            ins.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        // Seed example coaches + their session plans on first run
        public static void SeedCoachesAndPlans()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Trainers", conn))
                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0) return;

                void AddCoach(string name, string spec, string phone, (string, int, decimal)[] plans)
                {
                    long coachId;
                    using (var cmd = new SQLiteCommand(
                        "INSERT INTO Trainers (Name, Specialty, Phone) VALUES (@n,@s,@p); SELECT last_insert_rowid();", conn))
                    {
                        cmd.Parameters.AddWithValue("@n", name);
                        cmd.Parameters.AddWithValue("@s", spec);
                        cmd.Parameters.AddWithValue("@p", phone);
                        coachId = (long)cmd.ExecuteScalar();
                    }
                    foreach (var (pn, sess, price) in plans)
                        using (var cmd = new SQLiteCommand(
                            "INSERT INTO Plans (TrainerID, PlanName, Sessions, Price) VALUES (@t,@n,@se,@pr)", conn))
                        {
                            cmd.Parameters.AddWithValue("@t", coachId);
                            cmd.Parameters.AddWithValue("@n", pn);
                            cmd.Parameters.AddWithValue("@se", sess);
                            cmd.Parameters.AddWithValue("@pr", price);
                            cmd.ExecuteNonQuery();
                        }
                }

                AddCoach("Coach Bilal", "MMA / Striking", "",
                    new[] { ("Plan 1", 8, 1000m), ("Plan 2", 10, 1200m) });
                AddCoach("Coach Osama", "Boxing", "",
                    new[] { ("Starter", 6, 800m), ("Standard", 10, 1200m), ("Pro", 16, 1800m) });
            }
        }

        // ===== USERS / AUTH =====
        public static User ValidateUser(string username, string password)
        {
            int userId = 0; string storedPassword = null; string role = null;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT * FROM Users WHERE Username=@u", conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    using (var r = cmd.ExecuteReader())
                        if (r.Read())
                        {
                            userId = Convert.ToInt32(r["UserID"]);
                            storedPassword = r["Password"].ToString();
                            role = r["Role"].ToString();
                        }
                }
            }

            if (storedPassword == null) return null;

            bool ok = false;
            if (VerifyPassword(password, storedPassword)) ok = true;
            else if (storedPassword == password) { UpdateUserPassword(userId, HashPassword(password)); ok = true; }
            if (!ok) return null;

            // A member login is only valid while its member record still exists
            if (string.Equals(role, "Member", StringComparison.OrdinalIgnoreCase) && !MemberExistsForUser(userId))
                return null;

            return new User { UserID = userId, Username = username, Role = role };
        }

        private static bool MemberExistsForUser(int userId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Members WHERE UserID=@u", conn))
                {
                    cmd.Parameters.AddWithValue("@u", userId);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public static void UpdateUserPassword(int userId, string newHashedPassword)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("UPDATE Users SET Password=@p WHERE UserID=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@p", newHashedPassword);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static bool VerifyUserPassword(string username, string password)
            => ValidateUser(username, password) != null;

        // ===== COACHES (Trainers) =====
        public static List<Trainer> GetAllTrainers()
        {
            var list = new List<Trainer>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT * FROM Trainers ORDER BY Name", conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add(new Trainer
                        {
                            TrainerID = Convert.ToInt32(r["TrainerID"]),
                            Name = r["Name"].ToString(),
                            Specialty = r["Specialty"]?.ToString(),
                            Phone = r["Phone"]?.ToString()
                        });
            }
            return list;
        }

        public static void AddTrainer(Trainer t)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    "INSERT INTO Trainers (Name, Specialty, Phone) VALUES (@n,@s,@p)", conn))
                {
                    cmd.Parameters.AddWithValue("@n", t.Name);
                    cmd.Parameters.AddWithValue("@s", t.Specialty ?? "");
                    cmd.Parameters.AddWithValue("@p", t.Phone ?? "");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateTrainer(Trainer t)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    "UPDATE Trainers SET Name=@n, Specialty=@s, Phone=@p WHERE TrainerID=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@n", t.Name);
                    cmd.Parameters.AddWithValue("@s", t.Specialty ?? "");
                    cmd.Parameters.AddWithValue("@p", t.Phone ?? "");
                    cmd.Parameters.AddWithValue("@id", t.TrainerID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteTrainer(int trainerID)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    // remove the coach's plans first
                    using (var cmd = new SQLiteCommand("DELETE FROM Plans WHERE TrainerID=@id", conn, tx))
                    { cmd.Parameters.AddWithValue("@id", trainerID); cmd.ExecuteNonQuery(); }
                    using (var cmd = new SQLiteCommand("DELETE FROM Trainers WHERE TrainerID=@id", conn, tx))
                    { cmd.Parameters.AddWithValue("@id", trainerID); cmd.ExecuteNonQuery(); }
                    tx.Commit();
                }
            }
        }

        // ===== PLANS (per coach) =====
        public static List<MembershipPlan> GetAllPlans()
        {
            var list = new List<MembershipPlan>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = @"SELECT p.*, t.Name AS CoachName
                               FROM Plans p LEFT JOIN Trainers t ON t.TrainerID=p.TrainerID
                               ORDER BY t.Name, p.PlanName";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) list.Add(ReadPlan(r));
            }
            return list;
        }

        public static List<MembershipPlan> GetPlansByCoach(int coachID)
        {
            var list = new List<MembershipPlan>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = @"SELECT p.*, t.Name AS CoachName
                               FROM Plans p LEFT JOIN Trainers t ON t.TrainerID=p.TrainerID
                               WHERE p.TrainerID=@c ORDER BY p.Price";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@c", coachID);
                    using (var r = cmd.ExecuteReader())
                        while (r.Read()) list.Add(ReadPlan(r));
                }
            }
            return list;
        }

        private static MembershipPlan ReadPlan(IDataRecord r) => new MembershipPlan
        {
            PlanID = Convert.ToInt32(r["PlanID"]),
            CoachID = r["TrainerID"] != DBNull.Value ? Convert.ToInt32(r["TrainerID"]) : 0,
            PlanName = r["PlanName"].ToString(),
            Sessions = r["Sessions"] != DBNull.Value ? Convert.ToInt32(r["Sessions"]) : 0,
            Price = r["Price"] != DBNull.Value ? Convert.ToDecimal(r["Price"]) : 0m,
            CoachName = SafeGet(r, "CoachName")
        };

        public static void AddPlan(MembershipPlan p)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    "INSERT INTO Plans (TrainerID, PlanName, Sessions, Price) VALUES (@t,@n,@s,@pr)", conn))
                {
                    cmd.Parameters.AddWithValue("@t", p.CoachID);
                    cmd.Parameters.AddWithValue("@n", p.PlanName);
                    cmd.Parameters.AddWithValue("@s", p.Sessions);
                    cmd.Parameters.AddWithValue("@pr", p.Price);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdatePlan(MembershipPlan p)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    "UPDATE Plans SET TrainerID=@t, PlanName=@n, Sessions=@s, Price=@pr WHERE PlanID=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@t", p.CoachID);
                    cmd.Parameters.AddWithValue("@n", p.PlanName);
                    cmd.Parameters.AddWithValue("@s", p.Sessions);
                    cmd.Parameters.AddWithValue("@pr", p.Price);
                    cmd.Parameters.AddWithValue("@id", p.PlanID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeletePlan(int planID)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM Plans WHERE PlanID=@id", conn))
                { cmd.Parameters.AddWithValue("@id", planID); cmd.ExecuteNonQuery(); }
            }
        }

        public static MembershipPlan GetPlanById(int planID)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT * FROM Plans WHERE PlanID=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", planID);
                    using (var r = cmd.ExecuteReader())
                        if (r.Read()) return ReadPlan(r);
                }
            }
            return null;
        }

        public static string GetPlanNameByID(int planID)
        {
            var p = GetPlanById(planID);
            return p != null ? p.PlanName : "-";
        }

        // ===== MEMBERS =====
        public static bool MemberExists(string name, string phone)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    "SELECT COUNT(*) FROM Members WHERE TRIM(Name)=@n AND IFNULL(TRIM(Phone),'')=@p", conn))
                {
                    cmd.Parameters.AddWithValue("@n", (name ?? "").Trim());
                    cmd.Parameters.AddWithValue("@p", (phone ?? "").Trim());
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public static List<Member> GetAllMembers()
        {
            var list = new List<Member>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = @"SELECT m.*, t.Name AS CoachName, p.PlanName AS PlanName
                               FROM Members m
                               LEFT JOIN Trainers t ON t.TrainerID=m.TrainerID
                               LEFT JOIN Plans    p ON p.PlanID=m.PlanID
                               ORDER BY m.Name";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) list.Add(ReadMember(r));
            }
            return list;
        }

        public static void AddMember(Member m, string username, string password)
        {
            if (MemberExists(m.Name, m.Phone))
                throw new InvalidOperationException(
                    $"A member named \"{m.Name}\" with phone \"{m.Phone}\" already exists.");

            password = HashPassword(password);
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    long userID;
                    using (var cmd = new SQLiteCommand(
                        "INSERT INTO Users (Username, Password, Role) VALUES (@u,@p,'Member'); SELECT last_insert_rowid();", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", password);
                        userID = (long)cmd.ExecuteScalar();
                    }

                    using (var cmd = new SQLiteCommand(
                        @"INSERT INTO Members (UserID, Name, Phone, Age, Address, JoinDate, TrainerID, PlanID, SessionsTotal, SessionsRemaining)
                          VALUES (@uid,@name,@phone,@age,@addr,@join,@coach,@plan,@stot,@srem)", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@uid", userID);
                        cmd.Parameters.AddWithValue("@name", m.Name);
                        cmd.Parameters.AddWithValue("@phone", m.Phone ?? "");
                        cmd.Parameters.AddWithValue("@age", m.Age);
                        cmd.Parameters.AddWithValue("@addr", m.Address ?? "");
                        cmd.Parameters.AddWithValue("@join", m.JoinDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@coach", m.CoachID);
                        cmd.Parameters.AddWithValue("@plan", m.PlanID);
                        cmd.Parameters.AddWithValue("@stot", m.SessionsTotal);
                        cmd.Parameters.AddWithValue("@srem", m.SessionsRemaining);
                        cmd.ExecuteNonQuery();
                    }
                    tx.Commit();
                }
            }
        }

        public static void UpdateMember(Member m)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    @"UPDATE Members SET Name=@name, Phone=@phone, Age=@age, Address=@addr,
                        TrainerID=@coach, PlanID=@plan, SessionsTotal=@stot, SessionsRemaining=@srem
                      WHERE MemberID=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@name", m.Name);
                    cmd.Parameters.AddWithValue("@phone", m.Phone ?? "");
                    cmd.Parameters.AddWithValue("@age", m.Age);
                    cmd.Parameters.AddWithValue("@addr", m.Address ?? "");
                    cmd.Parameters.AddWithValue("@coach", m.CoachID);
                    cmd.Parameters.AddWithValue("@plan", m.PlanID);
                    cmd.Parameters.AddWithValue("@stot", m.SessionsTotal);
                    cmd.Parameters.AddWithValue("@srem", m.SessionsRemaining);
                    cmd.Parameters.AddWithValue("@id", m.MemberID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteMember(int memberID)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    int userID = 0;
                    using (var cmd = new SQLiteCommand("SELECT UserID FROM Members WHERE MemberID=@id", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@id", memberID);
                        var r = cmd.ExecuteScalar();
                        if (r != null && r != DBNull.Value) userID = Convert.ToInt32(r);
                    }
                    foreach (var sql in new[]
                    {
                        "DELETE FROM SessionLog WHERE MemberID=@id",
                        "DELETE FROM Payments WHERE MemberID=@id",
                        "DELETE FROM Members WHERE MemberID=@id"
                    })
                        using (var cmd = new SQLiteCommand(sql, conn, tx))
                        { cmd.Parameters.AddWithValue("@id", memberID); cmd.ExecuteNonQuery(); }

                    if (userID > 0)
                        using (var cmd = new SQLiteCommand("DELETE FROM Users WHERE UserID=@u", conn, tx))
                        { cmd.Parameters.AddWithValue("@u", userID); cmd.ExecuteNonQuery(); }

                    tx.Commit();
                }
            }
        }

        public static Member FindMember(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            input = QRHelper.ParseQRContent(input.Trim());
            if (!int.TryParse(input, out int memberID) || memberID <= 0) return null;

            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = @"SELECT m.*, t.Name AS CoachName, p.PlanName AS PlanName
                               FROM Members m
                               LEFT JOIN Trainers t ON t.TrainerID=m.TrainerID
                               LEFT JOIN Plans    p ON p.PlanID=m.PlanID
                               WHERE m.MemberID=@id";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", memberID);
                    using (var r = cmd.ExecuteReader())
                        if (r.Read()) return ReadMember(r);
                }
            }
            return null;
        }

        public static List<Member> FindMembers(string search)
        {
            var list = new List<Member>();
            if (string.IsNullOrWhiteSpace(search)) return list;

            string parsed = QRHelper.ParseQRContent(search.Trim());
            if (int.TryParse(parsed, out int memberID) && memberID > 0)
            {
                var m = FindMember(search);
                if (m != null) list.Add(m);
                return list;
            }

            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = @"SELECT m.*, t.Name AS CoachName, p.PlanName AS PlanName
                               FROM Members m
                               LEFT JOIN Trainers t ON t.TrainerID=m.TrainerID
                               LEFT JOIN Plans    p ON p.PlanID=m.PlanID
                               WHERE m.Name LIKE @s OR m.Phone LIKE @s";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@s", "%" + search + "%");
                    using (var r = cmd.ExecuteReader())
                        while (r.Read()) list.Add(ReadMember(r));
                }
            }
            return list;
        }

        public static Member GetMemberByUserID(int userID)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = @"SELECT m.*, t.Name AS CoachName, p.PlanName AS PlanName
                               FROM Members m
                               LEFT JOIN Trainers t ON t.TrainerID=m.TrainerID
                               LEFT JOIN Plans    p ON p.PlanID=m.PlanID
                               WHERE m.UserID=@uid";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userID);
                    using (var r = cmd.ExecuteReader())
                        if (r.Read()) return ReadMember(r);
                }
            }
            return null;
        }

        private static Member ReadMember(IDataRecord r) => new Member
        {
            MemberID = Convert.ToInt32(r["MemberID"]),
            UserID = r["UserID"] != DBNull.Value ? Convert.ToInt32(r["UserID"]) : 0,
            Name = r["Name"].ToString(),
            Phone = r["Phone"]?.ToString(),
            Age = r["Age"] != DBNull.Value ? Convert.ToInt32(r["Age"]) : 0,
            Address = r["Address"]?.ToString(),
            JoinDate = ParseDate(r["JoinDate"]),
            CoachID = r["TrainerID"] != DBNull.Value ? Convert.ToInt32(r["TrainerID"]) : 0,
            PlanID = r["PlanID"] != DBNull.Value ? Convert.ToInt32(r["PlanID"]) : 0,
            SessionsTotal = r["SessionsTotal"] != DBNull.Value ? Convert.ToInt32(r["SessionsTotal"]) : 0,
            SessionsRemaining = r["SessionsRemaining"] != DBNull.Value ? Convert.ToInt32(r["SessionsRemaining"]) : 0,
            CoachName = SafeGet(r, "CoachName"),
            PlanName = SafeGet(r, "PlanName")
        };

        // ===== SESSIONS =====
        public static bool HasSessionsLeft(int memberID)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT SessionsRemaining FROM Members WHERE MemberID=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", memberID);
                    var v = cmd.ExecuteScalar();
                    return v != null && v != DBNull.Value && Convert.ToInt32(v) > 0;
                }
            }
        }

        // Use one session; returns remaining count, or -1 if none were left.
        public static int UseSession(int memberID)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    int remaining;
                    using (var cmd = new SQLiteCommand("SELECT SessionsRemaining FROM Members WHERE MemberID=@id", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@id", memberID);
                        var v = cmd.ExecuteScalar();
                        remaining = (v == null || v == DBNull.Value) ? 0 : Convert.ToInt32(v);
                    }
                    if (remaining <= 0) { tx.Rollback(); return -1; }

                    remaining--;
                    using (var cmd = new SQLiteCommand("UPDATE Members SET SessionsRemaining=@r WHERE MemberID=@id", conn, tx))
                    { cmd.Parameters.AddWithValue("@r", remaining); cmd.Parameters.AddWithValue("@id", memberID); cmd.ExecuteNonQuery(); }
                    using (var cmd = new SQLiteCommand("INSERT INTO SessionLog (MemberID, UsedAt) VALUES (@id,@t)", conn, tx))
                    { cmd.Parameters.AddWithValue("@id", memberID); cmd.Parameters.AddWithValue("@t", Now()); cmd.ExecuteNonQuery(); }

                    tx.Commit();
                    return remaining;
                }
            }
        }

        // Add sessions (renewal / top-up)
        public static void AddSessions(int memberID, int planID)
        {
            var plan = GetPlanById(planID);
            if (plan == null) return;
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    @"UPDATE Members SET PlanID=@p, TrainerID=@t,
                        SessionsTotal=SessionsTotal+@s, SessionsRemaining=SessionsRemaining+@s
                      WHERE MemberID=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@p", plan.PlanID);
                    cmd.Parameters.AddWithValue("@t", plan.CoachID);
                    cmd.Parameters.AddWithValue("@s", plan.Sessions);
                    cmd.Parameters.AddWithValue("@id", memberID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ===== PAYMENTS =====
        public static void AddPayment(Payment p)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    "INSERT INTO Payments (MemberID, PlanID, Amount, Date, Method) VALUES (@m,@plan,@a,@d,@meth)", conn))
                {
                    cmd.Parameters.AddWithValue("@m", p.MemberID);
                    cmd.Parameters.AddWithValue("@plan", p.PlanID);
                    cmd.Parameters.AddWithValue("@a", p.Amount);
                    cmd.Parameters.AddWithValue("@d", (p.Date == default(DateTime) ? DateTime.Now : p.Date).ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@meth", p.Method ?? "Cash");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static DataTable GetPaymentsReport()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = @"SELECT p.PaymentID, m.Name AS MemberName,
                                      COALESCE(t.Name || ' - ' || pl.PlanName, '-') AS Plan,
                                      p.Amount, p.Date, p.Method
                               FROM Payments p
                               JOIN Members m ON p.MemberID=m.MemberID
                               LEFT JOIN Plans pl ON pl.PlanID=p.PlanID
                               LEFT JOIN Trainers t ON t.TrainerID=pl.TrainerID
                               ORDER BY p.Date DESC";
                using (var adapter = new SQLiteDataAdapter(sql, conn))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        // ===== ANALYTICS =====
        public static (int totalMembers, int activeMembers, decimal monthRevenue,
                        int weekSessions, decimal yearRevenue, int newThisMonth) GetDashboardStats()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string monthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd HH:mm:ss");
                string yearStart = new DateTime(DateTime.Now.Year, 1, 1).ToString("yyyy-MM-dd HH:mm:ss");
                string weekAgo = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss");

                int total = ScalarInt(conn, "SELECT COUNT(*) FROM Members");
                int active = ScalarInt(conn, "SELECT COUNT(*) FROM Members WHERE SessionsRemaining>0");
                decimal monthRev = ScalarDec(conn, "SELECT IFNULL(SUM(Amount),0) FROM Payments WHERE Date>=@d", ("@d", monthStart));
                int weekSess = ScalarInt(conn, "SELECT COUNT(*) FROM SessionLog WHERE UsedAt>=@d", ("@d", weekAgo));
                decimal yearRev = ScalarDec(conn, "SELECT IFNULL(SUM(Amount),0) FROM Payments WHERE Date>=@d", ("@d", yearStart));
                int newMonth = ScalarInt(conn, "SELECT COUNT(*) FROM Members WHERE JoinDate>=@d", ("@d", monthStart));
                return (total, active, monthRev, weekSess, yearRev, newMonth);
            }
        }

        public static List<(string Month, decimal Total)> GetMonthlyRevenue(int monthsBack = 12)
        {
            var result = new List<(string, decimal)>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
                                   .AddMonths(-(monthsBack - 1)).ToString("yyyy-MM-dd HH:mm:ss");
                string sql = @"SELECT strftime('%Y-%m', Date) AS M, SUM(Amount) AS Total
                               FROM Payments WHERE Date>=@s GROUP BY M ORDER BY M";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@s", start);
                    using (var r = cmd.ExecuteReader())
                        while (r.Read())
                        {
                            var ym = r["M"].ToString();
                            var label = DateTime.TryParse(ym + "-01", out var d) ? d.ToString("MMM yyyy") : ym;
                            result.Add((label, Convert.ToDecimal(r["Total"])));
                        }
                }
            }
            return result;
        }

        public static List<(string Month, int Count)> GetMemberGrowth(int monthsBack = 12)
        {
            var perMonth = new Dictionary<string, int>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    "SELECT strftime('%Y-%m', JoinDate) AS M, COUNT(*) AS C FROM Members WHERE JoinDate IS NOT NULL GROUP BY M", conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) perMonth[r["M"].ToString()] = Convert.ToInt32(r["C"]);
            }

            var result = new List<(string, int)>();
            var start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-(monthsBack - 1));
            int cumulative = 0;
            foreach (var kv in perMonth)
                if (DateTime.TryParse(kv.Key + "-01", out var d) && d < start) cumulative += kv.Value;
            for (int i = 0; i < monthsBack; i++)
            {
                var month = start.AddMonths(i);
                if (perMonth.TryGetValue(month.ToString("yyyy-MM"), out int n)) cumulative += n;
                result.Add((month.ToString("MMM yyyy"), cumulative));
            }
            return result;
        }

        public static List<(string PlanName, int Count)> GetPlanDistribution()
        {
            var result = new List<(string, int)>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = @"SELECT (t.Name || ' - ' || p.PlanName) AS Label, COUNT(m.MemberID) AS C
                               FROM Plans p
                               LEFT JOIN Trainers t ON t.TrainerID=p.TrainerID
                               LEFT JOIN Members m ON m.PlanID=p.PlanID
                               GROUP BY p.PlanID HAVING COUNT(m.MemberID)>0 ORDER BY C DESC";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) result.Add((r["Label"].ToString(), Convert.ToInt32(r["C"])));
            }
            return result;
        }

        public static List<(string Method, decimal Total)> GetPaymentMethodBreakdown()
        {
            var result = new List<(string, decimal)>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = @"SELECT IFNULL(Method,'Unknown') AS Method, SUM(Amount) AS Total
                               FROM Payments GROUP BY Method ORDER BY Total DESC";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) result.Add((r["Method"].ToString(), Convert.ToDecimal(r["Total"])));
            }
            return result;
        }

        public static List<(string Day, int Count)> GetSessionsPerDay(int days = 7)
        {
            var map = new Dictionary<string, int>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string start = DateTime.Now.Date.AddDays(-(days - 1)).ToString("yyyy-MM-dd HH:mm:ss");
                using (var cmd = new SQLiteCommand(
                    "SELECT strftime('%Y-%m-%d', UsedAt) AS D, COUNT(*) AS C FROM SessionLog WHERE UsedAt>=@s GROUP BY D", conn))
                {
                    cmd.Parameters.AddWithValue("@s", start);
                    using (var r = cmd.ExecuteReader())
                        while (r.Read()) map[r["D"].ToString()] = Convert.ToInt32(r["C"]);
                }
            }
            var result = new List<(string, int)>();
            for (int i = days - 1; i >= 0; i--)
            {
                var d = DateTime.Now.Date.AddDays(-i);
                map.TryGetValue(d.ToString("yyyy-MM-dd"), out int c);
                result.Add((d.ToString("ddd"), c));
            }
            return result;
        }

        // ===== helpers =====
        private static int ScalarInt(SQLiteConnection conn, string sql, params (string, object)[] ps)
        {
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                foreach (var (k, v) in ps) cmd.Parameters.AddWithValue(k, v);
                var r = cmd.ExecuteScalar();
                return (r == null || r == DBNull.Value) ? 0 : Convert.ToInt32(r);
            }
        }

        private static decimal ScalarDec(SQLiteConnection conn, string sql, params (string, object)[] ps)
        {
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                foreach (var (k, v) in ps) cmd.Parameters.AddWithValue(k, v);
                var r = cmd.ExecuteScalar();
                return (r == null || r == DBNull.Value) ? 0m : Convert.ToDecimal(r);
            }
        }

        private static string SafeGet(IDataRecord r, string col)
        {
            for (int i = 0; i < r.FieldCount; i++)
                if (string.Equals(r.GetName(i), col, StringComparison.OrdinalIgnoreCase))
                    return r.IsDBNull(i) ? null : r.GetValue(i).ToString();
            return null;
        }

        private static DateTime ParseDate(object o)
        {
            if (o == null || o == DBNull.Value) return DateTime.Now;
            return DateTime.TryParse(o.ToString(), out var d) ? d : DateTime.Now;
        }
    }
}
