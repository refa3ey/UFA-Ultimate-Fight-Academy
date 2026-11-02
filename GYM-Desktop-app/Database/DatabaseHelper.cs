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

        // End of the window a plan opens on `start`. Each plan carries its own
        // length (1 week ... 2 months), chosen by the manager when the plan is
        // created. A missing plan falls back to 1 month.
        public static DateTime ComputeExpiry(DateTime start, MembershipPlan plan) =>
            (plan?.Duration ?? PlanDuration.Default).AddTo(start);

        public static DateTime ComputeExpiry(DateTime start, int planID) =>
            ComputeExpiry(start, GetPlanById(planID));

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
                            PlanID        INTEGER PRIMARY KEY AUTOINCREMENT,
                            TrainerID     INTEGER,
                            PlanName      TEXT,
                            Sessions      INTEGER,
                            Price         REAL,
                            DurationValue INTEGER,
                            DurationUnit  TEXT);");
                // migrate plans created before durations were per-plan
                try { Exec(c, "ALTER TABLE Plans ADD COLUMN DurationValue INTEGER"); } catch { }
                try { Exec(c, "ALTER TABLE Plans ADD COLUMN DurationUnit  TEXT"); } catch { }
                Exec(c, "UPDATE Plans SET DurationValue = 1       WHERE IFNULL(DurationValue,0) < 1");
                Exec(c, "UPDATE Plans SET DurationUnit  = 'Month' WHERE IFNULL(DurationUnit,'') = ''");
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
                            SessionsRemaining INTEGER,
                            PlanStart         TEXT,
                            PlanExpiry        TEXT);");
                // migrate older DBs that predate the plan-window columns
                try { Exec(c, "ALTER TABLE Members ADD COLUMN PlanStart  TEXT"); } catch { }
                try { Exec(c, "ALTER TABLE Members ADD COLUMN PlanExpiry TEXT"); } catch { }
                // backfill: existing members' window runs from the day they joined
                Exec(c, "UPDATE Members SET PlanStart = JoinDate WHERE IFNULL(PlanStart,'') = ''");
                // Members who predate plan windows get one month from their join date.
                // Their plan's own length only applies from their next renewal on.
                Exec(c, @"UPDATE Members
                             SET PlanExpiry = date(PlanStart, '+1 month')
                           WHERE IFNULL(PlanExpiry,'') = '' AND IFNULL(PlanID,0) > 0");
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
                // Deleted members and coaches are kept here so they can be restored.
                // They leave the live tables, which frees their IDs for reuse.
                Exec(c, @"CREATE TABLE IF NOT EXISTS ArchivedTrainers(
                            ArchiveID  INTEGER PRIMARY KEY AUTOINCREMENT,
                            OriginalID INTEGER,
                            Name       TEXT,
                            Specialty  TEXT,
                            Phone      TEXT,
                            DeletedAt  TEXT);");
                Exec(c, @"CREATE TABLE IF NOT EXISTS ArchivedMembers(
                            ArchiveID         INTEGER PRIMARY KEY AUTOINCREMENT,
                            OriginalID        INTEGER,
                            Username          TEXT,
                            PasswordHash      TEXT,
                            Name              TEXT,
                            Phone             TEXT,
                            Age               INTEGER,
                            Address           TEXT,
                            JoinDate          TEXT,
                            TrainerID         INTEGER,
                            ArchivedTrainerID INTEGER,
                            PlanID            INTEGER,
                            SessionsTotal     INTEGER,
                            SessionsRemaining INTEGER,
                            PlanStart         TEXT,
                            PlanExpiry        TEXT,
                            DeletedAt         TEXT);");
                try { Exec(c, "ALTER TABLE Payments   ADD COLUMN ArchivedMemberID INTEGER"); } catch { }
                try { Exec(c, "ALTER TABLE SessionLog ADD COLUMN ArchivedMemberID INTEGER"); } catch { }
                try { Exec(c, "ALTER TABLE Members    ADD COLUMN ArchivedTrainerID INTEGER"); } catch { }
                try { Exec(c, "ALTER TABLE Plans      ADD COLUMN ArchivedTrainerID INTEGER"); } catch { }
                try { Exec(c, "ALTER TABLE Plans      ADD COLUMN IsDeleted INTEGER DEFAULT 0"); } catch { }
                // Whether the member's current plan period has been paid for.
                try { Exec(c, "ALTER TABLE ArchivedMembers ADD COLUMN PlanPaid INTEGER DEFAULT 0"); } catch { }
                bool planPaidIsNew = false;
                try { Exec(c, "ALTER TABLE Members ADD COLUMN PlanPaid INTEGER DEFAULT 0"); planPaidIsNew = true; } catch { }
                if (planPaidIsNew)
                    Exec(c, @"UPDATE Members SET PlanPaid = 1
                               WHERE EXISTS (SELECT 1 FROM Payments p
                                              WHERE p.MemberID = Members.MemberID
                                                AND p.PlanID = Members.PlanID
                                                AND date(p.Date) >= date(Members.PlanStart))");
                // Single sessions have no plan, so the payment names its coach directly.
                try { Exec(c, "ALTER TABLE Payments   ADD COLUMN TrainerID INTEGER"); } catch { }
                try { Exec(c, "ALTER TABLE Payments   ADD COLUMN ArchivedTrainerID INTEGER"); } catch { }
                try { Exec(c, "ALTER TABLE Payments   ADD COLUMN SessionType TEXT"); } catch { }
                Exec(c, @"CREATE TABLE IF NOT EXISTS Products(
                            ProductID INTEGER PRIMARY KEY AUTOINCREMENT,
                            Category  TEXT,
                            Name      TEXT,
                            Size      TEXT,
                            Price     REAL,
                            Stock     INTEGER);");
                // A sale copies the product's details, so it survives the product being
                // edited or deleted.
                Exec(c, @"CREATE TABLE IF NOT EXISTS ProductSales(
                            SaleID      INTEGER PRIMARY KEY AUTOINCREMENT,
                            ProductID   INTEGER,
                            ProductName TEXT,
                            Category    TEXT,
                            Quantity    INTEGER,
                            UnitPrice   REAL,
                            Total       REAL,
                            Method      TEXT,
                            Date        TEXT);");
                // A member can hold one plan per coach, each with its own sessions and expiry.
                // The plan columns on Members and ArchivedMembers are only read by this one-time move.
                try { Exec(c, "ALTER TABLE SessionLog ADD COLUMN TrainerID INTEGER"); } catch { }
                try { Exec(c, "ALTER TABLE SessionLog ADD COLUMN ArchivedTrainerID INTEGER"); } catch { }
                bool memberPlansExist;
                using (var cmd = new SQLiteCommand(
                    "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='MemberPlans'", c))
                    memberPlansExist = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                if (!memberPlansExist)
                    using (var tx = c.BeginTransaction())
                    {
                        Exec(c, @"CREATE TABLE MemberPlans(
                                    MemberPlanID      INTEGER PRIMARY KEY AUTOINCREMENT,
                                    MemberID          INTEGER,
                                    ArchivedMemberID  INTEGER,
                                    PlanID            INTEGER,
                                    TrainerID         INTEGER,
                                    ArchivedTrainerID INTEGER,
                                    SessionsTotal     INTEGER,
                                    SessionsRemaining INTEGER,
                                    PlanStart         TEXT,
                                    PlanExpiry        TEXT,
                                    PlanPaid          INTEGER DEFAULT 0);");
                        Exec(c, @"INSERT INTO MemberPlans (MemberID, PlanID, TrainerID, ArchivedTrainerID, SessionsTotal, SessionsRemaining, PlanStart, PlanExpiry, PlanPaid)
                                  SELECT MemberID, PlanID, NULLIF(TrainerID,0), ArchivedTrainerID, SessionsTotal, SessionsRemaining, PlanStart, PlanExpiry, IFNULL(PlanPaid,0)
                                  FROM Members WHERE IFNULL(PlanID,0) > 0");
                        Exec(c, @"INSERT INTO MemberPlans (ArchivedMemberID, PlanID, TrainerID, ArchivedTrainerID, SessionsTotal, SessionsRemaining, PlanStart, PlanExpiry, PlanPaid)
                                  SELECT ArchiveID, PlanID, NULLIF(TrainerID,0), ArchivedTrainerID, SessionsTotal, SessionsRemaining, PlanStart, PlanExpiry, IFNULL(PlanPaid,0)
                                  FROM ArchivedMembers WHERE IFNULL(PlanID,0) > 0");
                        // Until now a member had one coach, so past sessions were with that coach.
                        Exec(c, @"UPDATE SessionLog SET
                                    TrainerID = (SELECT NULLIF(TrainerID,0) FROM Members m WHERE m.MemberID = SessionLog.MemberID),
                                    ArchivedTrainerID = (SELECT ArchivedTrainerID FROM Members m WHERE m.MemberID = SessionLog.MemberID)
                                  WHERE MemberID IS NOT NULL");
                        Exec(c, @"UPDATE SessionLog SET
                                    TrainerID = (SELECT NULLIF(TrainerID,0) FROM ArchivedMembers a WHERE a.ArchiveID = SessionLog.ArchivedMemberID),
                                    ArchivedTrainerID = (SELECT ArchivedTrainerID FROM ArchivedMembers a WHERE a.ArchiveID = SessionLog.ArchivedMemberID)
                                  WHERE ArchivedMemberID IS NOT NULL");
                        tx.Commit();
                    }
                // Older versions left members pointing at deleted coaches. Trainer IDs
                // are reused now, so those members would land on the next new coach.
                Exec(c, @"UPDATE Members SET TrainerID = NULL
                           WHERE TrainerID IS NOT NULL
                             AND TrainerID NOT IN (SELECT TrainerID FROM Trainers)");
            }

            SeedAdmin();
            SeedCoachesAndPlans();
        }

        // Lowest unused positive ID in a table, so the ID of a deleted row is given
        // to the next new row. Run this inside the insert's transaction.
        // Only for Members and Trainers: their old IDs are fully cleaned up on delete.
        // Plans must keep AUTOINCREMENT, because Payments still point at deleted plans
        // and a reused ID would hand those old payments to the new plan.
        private static long NextFreeId(SQLiteConnection conn, SQLiteTransaction tx, string table, string idColumn)
        {
            string sql = $@"SELECT CASE
                              WHEN NOT EXISTS (SELECT 1 FROM {table} WHERE {idColumn} = 1) THEN 1
                              ELSE (SELECT MIN(a.{idColumn}) + 1 FROM {table} a
                                     WHERE NOT EXISTS (SELECT 1 FROM {table} b WHERE b.{idColumn} = a.{idColumn} + 1))
                            END";
            using (var cmd = new SQLiteCommand(sql, conn, tx))
                return Convert.ToInt64(cmd.ExecuteScalar());
        }

        private const string MemberColumns = @"SELECT m.MemberID, m.UserID, m.Name, m.Phone, m.Age, m.Address, m.JoinDate,
                       mp.MemberPlanID, mp.TrainerID, mp.PlanID, mp.SessionsTotal, mp.SessionsRemaining, mp.PlanStart, mp.PlanExpiry,
                       COALESCE(t.Name, at2.Name || ' (deleted)') AS CoachName, p.PlanName AS PlanName
                FROM Members m ";
        private const string PlanJoins = @"
                LEFT JOIN Trainers t ON t.TrainerID=mp.TrainerID
                LEFT JOIN ArchivedTrainers at2 ON at2.ArchiveID=mp.ArchivedTrainerID
                LEFT JOIN Plans p ON p.PlanID=mp.PlanID ";

        // One row per person. The plan shown is their best one: usable first, then the latest expiry.
        private const string MemberSelect = MemberColumns + @"
                LEFT JOIN MemberPlans mp ON mp.MemberPlanID = (
                    SELECT x.MemberPlanID FROM MemberPlans x WHERE x.MemberID = m.MemberID
                    ORDER BY (x.SessionsRemaining > 0 AND date(x.PlanExpiry) >= date('now','localtime')) DESC, x.PlanExpiry DESC
                    LIMIT 1) " + PlanJoins;

        // One row per plan.
        private const string MemberRowSelect = MemberColumns + @"
                LEFT JOIN MemberPlans mp ON mp.MemberID = m.MemberID " + PlanJoins;

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
                // Only on a brand new database, so examples don't return after the gym
                // deletes all its coaches.
                using (var cmd = new SQLiteCommand(
                    "SELECT (SELECT COUNT(*) FROM Trainers) + (SELECT COUNT(*) FROM ArchivedTrainers)", conn))
                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0) return;

                void AddCoach(string name, string spec, string phone, (string, int, decimal, int, string)[] plans)
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
                    foreach (var (pn, sess, price, durVal, durUnit) in plans)
                        using (var cmd = new SQLiteCommand(
                            @"INSERT INTO Plans (TrainerID, PlanName, Sessions, Price, DurationValue, DurationUnit)
                              VALUES (@t,@n,@se,@pr,@dv,@du)", conn))
                        {
                            cmd.Parameters.AddWithValue("@t", coachId);
                            cmd.Parameters.AddWithValue("@n", pn);
                            cmd.Parameters.AddWithValue("@se", sess);
                            cmd.Parameters.AddWithValue("@pr", price);
                            cmd.Parameters.AddWithValue("@dv", durVal);
                            cmd.Parameters.AddWithValue("@du", durUnit);
                            cmd.ExecuteNonQuery();
                        }
                }

                // Examples showing off the per-plan duration: a short 2-week package
                // alongside the usual monthly ones.
                AddCoach("Coach Bilal", "MMA / Striking", "",
                    new[] { ("Plan 1", 8, 1000m, 2, PlanDuration.Week),
                            ("Plan 2", 10, 1200m, 1, PlanDuration.Month) });
                AddCoach("Coach Osama", "Boxing", "",
                    new[] { ("Starter", 6, 800m, 2, PlanDuration.Week),
                            ("Standard", 10, 1200m, 1, PlanDuration.Month),
                            ("Pro", 16, 1800m, 2, PlanDuration.Month) });
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
                using (var tx = conn.BeginTransaction())
                {
                    using (var cmd = new SQLiteCommand(
                        "INSERT INTO Trainers (TrainerID, Name, Specialty, Phone) VALUES (@id,@n,@s,@p)", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@id", NextFreeId(conn, tx, "Trainers", "TrainerID"));
                        cmd.Parameters.AddWithValue("@n", t.Name);
                        cmd.Parameters.AddWithValue("@s", t.Specialty ?? "");
                        cmd.Parameters.AddWithValue("@p", t.Phone ?? "");
                        cmd.ExecuteNonQuery();
                    }
                    tx.Commit();
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
                    long archiveID;
                    using (var cmd = new SQLiteCommand(
                        @"INSERT INTO ArchivedTrainers (OriginalID, Name, Specialty, Phone, DeletedAt)
                          SELECT TrainerID, Name, Specialty, Phone, @now FROM Trainers WHERE TrainerID=@id;
                          SELECT last_insert_rowid();", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@id", trainerID);
                        cmd.Parameters.AddWithValue("@now", Now());
                        archiveID = (long)cmd.ExecuteScalar();
                    }
                    // Everything linked to the coach now points at the archive, so a new
                    // coach who reuses this ID doesn't inherit it.
                    foreach (var table in new[] { "MemberPlans", "ArchivedMembers", "Plans", "Payments", "SessionLog" })
                        using (var cmd = new SQLiteCommand(
                            $"UPDATE {table} SET ArchivedTrainerID=@a, TrainerID=NULL WHERE TrainerID=@id", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@a", archiveID);
                            cmd.Parameters.AddWithValue("@id", trainerID);
                            cmd.ExecuteNonQuery();
                        }
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
                               FROM Plans p JOIN Trainers t ON t.TrainerID=p.TrainerID
                               WHERE IFNULL(p.IsDeleted,0)=0
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
                               WHERE p.TrainerID=@c AND IFNULL(p.IsDeleted,0)=0 ORDER BY p.Price";
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
            DurationValue = ReadDurationValue(r),
            DurationUnit = SafeGet(r, "DurationUnit") ?? PlanDuration.Month,
            CoachName = SafeGet(r, "CoachName")
        };

        private static int ReadDurationValue(IDataRecord r)
        {
            string v = SafeGet(r, "DurationValue");
            return int.TryParse(v, out int n) && n > 0 ? n : 1;
        }

        public static void AddPlan(MembershipPlan p)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    @"INSERT INTO Plans (TrainerID, PlanName, Sessions, Price, DurationValue, DurationUnit)
                      VALUES (@t,@n,@s,@pr,@dv,@du)", conn))
                {
                    cmd.Parameters.AddWithValue("@t", p.CoachID);
                    cmd.Parameters.AddWithValue("@n", p.PlanName);
                    cmd.Parameters.AddWithValue("@s", p.Sessions);
                    cmd.Parameters.AddWithValue("@pr", p.Price);
                    cmd.Parameters.AddWithValue("@dv", p.Duration.Value);
                    cmd.Parameters.AddWithValue("@du", p.Duration.Unit);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Note: this only changes the plan itself. Members already on it keep the
        // expiry date they were given - a duration change applies from their next
        // renewal onward, so nobody's paid-for time moves under them.
        public static void UpdatePlan(MembershipPlan p)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    @"UPDATE Plans SET TrainerID=@t, PlanName=@n, Sessions=@s, Price=@pr,
                        DurationValue=@dv, DurationUnit=@du
                      WHERE PlanID=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@t", p.CoachID);
                    cmd.Parameters.AddWithValue("@n", p.PlanName);
                    cmd.Parameters.AddWithValue("@s", p.Sessions);
                    cmd.Parameters.AddWithValue("@pr", p.Price);
                    cmd.Parameters.AddWithValue("@dv", p.Duration.Value);
                    cmd.Parameters.AddWithValue("@du", p.Duration.Unit);
                    cmd.Parameters.AddWithValue("@id", p.PlanID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Hidden rather than removed, so old payments still show their plan and coach.
        public static void DeletePlan(int planID)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("UPDATE Plans SET IsDeleted=1 WHERE PlanID=@id", conn))
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
                string sql = MemberSelect + @"
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
            // The window length comes from the plan the member is signing up for.
            var signupPlan = GetPlanById(m.PlanID);

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

                    // The plan window always starts the day the member signs up.
                    DateTime start = m.JoinDate == default(DateTime) ? DateTime.Now : m.JoinDate;

                    using (var cmd = new SQLiteCommand(
                        @"INSERT INTO Members (MemberID, UserID, Name, Phone, Age, Address, JoinDate)
                          VALUES (@id,@uid,@name,@phone,@age,@addr,@join);
                          INSERT INTO MemberPlans (MemberID, PlanID, TrainerID, SessionsTotal, SessionsRemaining, PlanStart, PlanExpiry)
                          VALUES (@id,@plan,@coach,@stot,@srem,@pstart,@pexp);", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@id", NextFreeId(conn, tx, "Members", "MemberID"));
                        cmd.Parameters.AddWithValue("@pstart", start.ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@pexp", ComputeExpiry(start, signupPlan).ToString("yyyy-MM-dd HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@uid", userID);
                        cmd.Parameters.AddWithValue("@name", m.Name);
                        cmd.Parameters.AddWithValue("@phone", m.Phone ?? "");
                        cmd.Parameters.AddWithValue("@age", m.Age);
                        cmd.Parameters.AddWithValue("@addr", m.Address ?? "");
                        cmd.Parameters.AddWithValue("@join", start.ToString("yyyy-MM-dd HH:mm:ss"));
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

        // A person's details only. Plans are changed with ChangeMemberPlan and RenewPlan.
        public static void UpdateMember(Member m)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    "UPDATE Members SET Name=@name, Phone=@phone, Age=@age, Address=@addr WHERE MemberID=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@name", m.Name);
                    cmd.Parameters.AddWithValue("@phone", m.Phone ?? "");
                    cmd.Parameters.AddWithValue("@age", m.Age);
                    cmd.Parameters.AddWithValue("@addr", m.Address ?? "");
                    cmd.Parameters.AddWithValue("@id", m.MemberID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Moves one of a member's plans onto another plan. Their sessions left and expiry
        // stay as they are. False if they already have a plan with that coach.
        public static bool ChangeMemberPlan(int memberPlanID, int memberID, MembershipPlan plan)
        {
            var other = GetMemberPlanFor(memberID, plan.CoachID);
            if (other != null && other.MemberPlanID != memberPlanID) return false;
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    @"UPDATE MemberPlans SET PlanID=@p, TrainerID=@t, ArchivedTrainerID=NULL, SessionsTotal=@s
                      WHERE MemberPlanID=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@p", plan.PlanID);
                    cmd.Parameters.AddWithValue("@t", plan.CoachID);
                    cmd.Parameters.AddWithValue("@s", plan.Sessions);
                    cmd.Parameters.AddWithValue("@id", memberPlanID);
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        public static void DeleteMember(int memberID)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    long archiveID;
                    using (var cmd = new SQLiteCommand(
                        @"INSERT INTO ArchivedMembers (OriginalID, Username, PasswordHash, Name, Phone, Age, Address,
                              JoinDate, TrainerID, ArchivedTrainerID, PlanID, SessionsTotal, SessionsRemaining,
                              PlanStart, PlanExpiry, PlanPaid, DeletedAt)
                          SELECT m.MemberID, u.Username, u.Password, m.Name, m.Phone, m.Age, m.Address,
                              m.JoinDate, m.TrainerID, m.ArchivedTrainerID, m.PlanID, m.SessionsTotal, m.SessionsRemaining,
                              m.PlanStart, m.PlanExpiry, m.PlanPaid, @now
                          FROM Members m LEFT JOIN Users u ON u.UserID=m.UserID WHERE m.MemberID=@id;
                          SELECT last_insert_rowid();", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@id", memberID);
                        cmd.Parameters.AddWithValue("@now", Now());
                        archiveID = (long)cmd.ExecuteScalar();
                    }

                    int userID = 0;
                    using (var cmd = new SQLiteCommand("SELECT UserID FROM Members WHERE MemberID=@id", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@id", memberID);
                        var r = cmd.ExecuteScalar();
                        if (r != null && r != DBNull.Value) userID = Convert.ToInt32(r);
                    }
                    // History moves to the archive record, so the ID is free to reuse and
                    // the money stays in the reports.
                    foreach (var table in new[] { "SessionLog", "Payments", "MemberPlans" })
                        using (var cmd = new SQLiteCommand(
                            $"UPDATE {table} SET ArchivedMemberID=@a, MemberID=NULL WHERE MemberID=@id", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@a", archiveID);
                            cmd.Parameters.AddWithValue("@id", memberID);
                            cmd.ExecuteNonQuery();
                        }
                    using (var cmd = new SQLiteCommand("DELETE FROM Members WHERE MemberID=@id", conn, tx))
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
            return GetMemberById(memberID);
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
                string sql = MemberSelect + @"
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
                string sql = MemberSelect + @"
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
            MemberPlanID = r["MemberPlanID"] != DBNull.Value ? Convert.ToInt32(r["MemberPlanID"]) : 0,
            CoachID = r["TrainerID"] != DBNull.Value ? Convert.ToInt32(r["TrainerID"]) : 0,
            PlanID = r["PlanID"] != DBNull.Value ? Convert.ToInt32(r["PlanID"]) : 0,
            SessionsTotal = r["SessionsTotal"] != DBNull.Value ? Convert.ToInt32(r["SessionsTotal"]) : 0,
            SessionsRemaining = r["SessionsRemaining"] != DBNull.Value ? Convert.ToInt32(r["SessionsRemaining"]) : 0,
            PlanStart = ParseDateOrDefault(SafeGet(r, "PlanStart")),
            PlanExpiry = ParseDateOrDefault(SafeGet(r, "PlanExpiry")),
            CoachName = SafeGet(r, "CoachName"),
            PlanName = SafeGet(r, "PlanName")
        };

        public static Member GetMemberById(int memberID)
        {
            var rows = ReadMembers(MemberSelect + " WHERE m.MemberID=@a", memberID);
            return rows.Count > 0 ? rows[0] : null;
        }

        // One row per plan, for the Members page. A member with no plan gets one row.
        public static List<Member> GetMemberPlanRows() => ReadMembers(
            MemberRowSelect + " ORDER BY m.Name, m.MemberID, CoachName");

        public static List<Member> GetMemberPlans(int memberID) => ReadMembers(
            MemberRowSelect + " WHERE m.MemberID=@a AND mp.MemberPlanID IS NOT NULL ORDER BY CoachName", memberID);

        // The member's plan with this coach, or null.
        public static Member GetMemberPlanFor(int memberID, int coachID)
        {
            var rows = ReadMembers(MemberRowSelect + " WHERE m.MemberID=@a AND mp.TrainerID=@b", memberID, coachID);
            return rows.Count > 0 ? rows[0] : null;
        }

        public static Member GetMemberPlanRow(int memberPlanID)
        {
            var rows = ReadMembers(MemberRowSelect + " WHERE mp.MemberPlanID=@a", memberPlanID);
            return rows.Count > 0 ? rows[0] : null;
        }

        private static List<Member> ReadMembers(string sql, int a = 0, int b = 0)
        {
            var list = new List<Member>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@a", a);
                    cmd.Parameters.AddWithValue("@b", b);
                    using (var r = cmd.ExecuteReader())
                        while (r.Read()) list.Add(ReadMember(r));
                }
            }
            return list;
        }

        // ===== SESSIONS =====

        // Use one session. Returns the remaining count, or -1 with `error` explaining
        // why not: out of sessions, or the plan month is over.
        public static int UseSession(int memberPlanID, out string error)
        {
            error = null;
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    int remaining;
                    DateTime expiry;
                    using (var cmd = new SQLiteCommand(
                        "SELECT SessionsRemaining, PlanExpiry FROM MemberPlans WHERE MemberPlanID=@id", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@id", memberPlanID);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (!r.Read())
                            {
                                tx.Rollback();
                                error = "This plan was not found.";
                                return -1;
                            }
                            remaining = r.IsDBNull(0) ? 0 : Convert.ToInt32(r.GetValue(0));
                            expiry = ParseDateOrDefault(r.IsDBNull(1) ? null : r.GetValue(1));
                        }
                    }

                    // The plan period is checked BEFORE the session count: an expired
                    // plan is dead even with sessions still on it.
                    if (expiry != default(DateTime) && DateTime.Now.Date > expiry.Date)
                    {
                        tx.Rollback();
                        error = $"This plan expired on {expiry:MMM dd, yyyy}"
                              + (remaining > 0 ? $" with {remaining} session(s) unused." : ".")
                              + "\n\nRenew it to start a new period.";
                        return -1;
                    }
                    if (remaining <= 0)
                    {
                        tx.Rollback();
                        error = "This plan has no sessions left. Renew it first.";
                        return -1;
                    }

                    remaining--;
                    using (var cmd = new SQLiteCommand(
                        @"UPDATE MemberPlans SET SessionsRemaining=@r WHERE MemberPlanID=@id;
                          INSERT INTO SessionLog (MemberID, TrainerID, ArchivedTrainerID, UsedAt)
                          SELECT MemberID, TrainerID, ArchivedTrainerID, @t FROM MemberPlans WHERE MemberPlanID=@id;", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@r", remaining);
                        cmd.Parameters.AddWithValue("@id", memberPlanID);
                        cmd.Parameters.AddWithValue("@t", Now());
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return remaining;
                }
            }
        }

        // Renew a member onto a plan, which always opens a fresh plan window.
        //
        //   expired month  -> the leftover sessions are forfeited and the member
        //                     starts clean on today + PlanDurationMonths
        //   month still running (early top-up)
        //                  -> leftover sessions are kept and added to, and the
        //                     window is pushed out from the existing expiry so
        //                     the days already paid for are not lost
        //
        // Returns the new remaining count, or -1 if the plan no longer exists.
        public static int RenewPlan(int memberID, int planID, out bool sessionsWereReset, out DateTime newExpiry)
        {
            sessionsWereReset = false;
            newExpiry = default(DateTime);

            var plan = GetPlanById(planID);
            if (plan == null || GetMemberById(memberID) == null) return -1;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    long rowID = 0;
                    int remaining = 0, total = 0;
                    DateTime expiry = default(DateTime);
                    using (var cmd = new SQLiteCommand(
                        @"SELECT MemberPlanID, SessionsRemaining, SessionsTotal, PlanExpiry FROM MemberPlans
                          WHERE MemberID=@m AND TrainerID=@t ORDER BY MemberPlanID DESC LIMIT 1", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@m", memberID);
                        cmd.Parameters.AddWithValue("@t", plan.CoachID);
                        using (var r = cmd.ExecuteReader())
                            if (r.Read())
                            {
                                rowID = r.GetInt64(0);
                                remaining = r.IsDBNull(1) ? 0 : Convert.ToInt32(r.GetValue(1));
                                total = r.IsDBNull(2) ? 0 : Convert.ToInt32(r.GetValue(2));
                                expiry = ParseDateOrDefault(r.IsDBNull(3) ? null : r.GetValue(3));
                            }
                    }

                    DateTime start = DateTime.Now;
                    bool periodIsOver = rowID == 0 || expiry == default(DateTime) || DateTime.Now.Date > expiry.Date;
                    if (periodIsOver)
                    {
                        sessionsWereReset = rowID != 0 && remaining > 0;   // these are being forfeited
                        remaining = plan.Sessions;
                        total = plan.Sessions;
                        newExpiry = ComputeExpiry(start, plan);
                    }
                    else
                    {
                        remaining += plan.Sessions;
                        total += plan.Sessions;
                        // Push the window out from the existing expiry so the days
                        // already paid for are not swallowed by the top-up.
                        newExpiry = plan.Duration.AddTo(expiry);
                    }

                    if (rowID == 0)
                    {
                        // A new period isn't paid for yet, unless a walk-in already paid for this plan.
                        using (var cmd = new SQLiteCommand(
                            @"INSERT INTO MemberPlans (MemberID, PlanID, TrainerID, SessionsTotal, SessionsRemaining, PlanStart, PlanExpiry, PlanPaid)
                              VALUES (@m,@p,@t,@stot,@srem,@pstart,@pexp,
                                      CASE WHEN NOT EXISTS (SELECT 1 FROM MemberPlans WHERE MemberID=@m)
                                            AND EXISTS (SELECT 1 FROM Payments WHERE MemberID=@m AND PlanID=@p) THEN 1 ELSE 0 END)", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@m", memberID);
                            cmd.Parameters.AddWithValue("@p", plan.PlanID);
                            cmd.Parameters.AddWithValue("@t", plan.CoachID);
                            cmd.Parameters.AddWithValue("@stot", total);
                            cmd.Parameters.AddWithValue("@srem", remaining);
                            cmd.Parameters.AddWithValue("@pstart", start.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.Parameters.AddWithValue("@pexp", newExpiry.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (var cmd = new SQLiteCommand(
                            @"UPDATE MemberPlans SET PlanID=@p, SessionsTotal=@stot, SessionsRemaining=@srem,
                                  PlanStart=@pstart, PlanExpiry=@pexp, PlanPaid=0
                              WHERE MemberPlanID=@id", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@p", plan.PlanID);
                            cmd.Parameters.AddWithValue("@stot", total);
                            cmd.Parameters.AddWithValue("@srem", remaining);
                            cmd.Parameters.AddWithValue("@pstart", start.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.Parameters.AddWithValue("@pexp", newExpiry.ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.Parameters.AddWithValue("@id", rowID);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();
                    return remaining;
                }
            }
        }

        // ===== PAYMENTS =====

        // Call after saving a plan payment. If it pays for the period the member already
        // has (they were just added, or renewed with Renew Plan), nothing more is given.
        // Otherwise it renews them onto the plan. Returns a line for the confirmation message.
        public static string ApplyPlanPayment(int memberID, MembershipPlan plan)
        {
            var current = GetMemberPlanFor(memberID, plan.CoachID);

            bool paysCurrentPeriod = false;
            if (current != null && current.PlanID == plan.PlanID && !current.IsExpired)
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT IFNULL(PlanPaid,0) FROM MemberPlans WHERE MemberPlanID=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", current.MemberPlanID);
                        paysCurrentPeriod = Convert.ToInt32(cmd.ExecuteScalar()) == 0;
                    }
                }

            string message;
            if (paysCurrentPeriod)
                message = $"This pays for their current plan with {plan.CoachName}: "
                        + $"{current.SessionsRemaining} sessions, valid until {current.PlanExpiry:MMM dd, yyyy}.";
            else
            {
                int sessions = RenewPlan(memberID, plan.PlanID, out bool wasReset, out DateTime expiry);
                if (sessions < 0) return "";
                message = (current == null ? $"New plan with {plan.CoachName}: " : $"Plan with {plan.CoachName} renewed: ")
                        + $"{sessions} sessions, valid until {expiry:MMM dd, yyyy}.";
                if (wasReset)
                    message += $"\n{current.SessionsRemaining} unused session(s) from the finished plan were dropped.";
            }

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("UPDATE MemberPlans SET PlanPaid=1 WHERE MemberID=@m AND TrainerID=@t", conn))
                {
                    cmd.Parameters.AddWithValue("@m", memberID);
                    cmd.Parameters.AddWithValue("@t", plan.CoachID);
                    cmd.ExecuteNonQuery();
                }
            }
            return message;
        }

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

        // Report periods: `from` is included, `to` isn't. Null means no limit.
        private static void AddPeriod(SQLiteCommand cmd, DateTime? from, DateTime? to)
        {
            cmd.Parameters.AddWithValue("@from", from?.ToString("yyyy-MM-dd HH:mm:ss") ?? "0000");
            cmd.Parameters.AddWithValue("@to", to?.ToString("yyyy-MM-dd HH:mm:ss") ?? "9999");
        }

        // Plan and single-session money, and product money, for a period.
        public static (decimal Memberships, decimal Products) GetRevenueTotals(DateTime? from = null, DateTime? to = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    @"SELECT IFNULL((SELECT SUM(Amount) FROM Payments WHERE Date>=@from AND Date<@to),0),
                             IFNULL((SELECT SUM(Total) FROM ProductSales WHERE Date>=@from AND Date<@to),0)", conn))
                {
                    AddPeriod(cmd, from, to);
                    using (var r = cmd.ExecuteReader())
                    {
                        r.Read();
                        return (Convert.ToDecimal(r.GetValue(0)), Convert.ToDecimal(r.GetValue(1)));
                    }
                }
            }
        }

        public static DataTable GetPaymentsReport(DateTime? from = null, DateTime? to = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = @"SELECT p.PaymentID,
                                      COALESCE(m.Name, am.Name || ' (deleted)') AS MemberName,
                                      COALESCE(COALESCE(t.Name, at2.Name || ' (deleted)') || ' - ' || COALESCE(p.SessionType, pl.PlanName), '-') AS Plan,
                                      p.Amount, p.Date, p.Method
                               FROM Payments p
                               LEFT JOIN Members m ON p.MemberID=m.MemberID
                               LEFT JOIN ArchivedMembers am ON am.ArchiveID=p.ArchivedMemberID
                               LEFT JOIN Plans pl ON pl.PlanID=p.PlanID
                               LEFT JOIN Trainers t ON t.TrainerID=COALESCE(pl.TrainerID, p.TrainerID)
                               LEFT JOIN ArchivedTrainers at2 ON at2.ArchiveID=COALESCE(pl.ArchivedTrainerID, p.ArchivedTrainerID)
                               WHERE (m.MemberID IS NOT NULL OR am.ArchiveID IS NOT NULL)
                                 AND p.Date>=@from AND p.Date<@to
                               ORDER BY p.Date DESC";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    AddPeriod(cmd, from, to);
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // One row per coach + member: what that member paid to that coach. A payment
        // belongs to the coach whose plan it was for. Members currently with the coach
        // appear even if they haven't paid (Amount 0), and a coach with no members
        // gets one row with a NULL MemberKey. Deleted people are marked "(deleted)" and
        // appear only if they have payments. Empty search = every coach.
        public static DataTable GetCoachPaymentsReport(string coachSearch, DateTime? from = null, DateTime? to = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = @"WITH
                               coaches AS (
                                   SELECT 'L' || TrainerID AS CoachKey, Name AS Coach FROM Trainers
                                   UNION ALL
                                   SELECT 'A' || ArchiveID, Name || ' (deleted)' FROM ArchivedTrainers),
                               people AS (
                                   SELECT 'L' || MemberID AS MemberKey, Name AS Member FROM Members
                                   UNION ALL
                                   SELECT 'A' || ArchiveID, Name || ' (deleted)' FROM ArchivedMembers),
                               withCoach AS (
                                   SELECT 'L' || MemberID AS MemberKey, 'L' || TrainerID AS CoachKey FROM MemberPlans
                                   WHERE MemberID IS NOT NULL AND TrainerID IS NOT NULL),
                               pay AS (
                                   SELECT p.PaymentID, p.Amount,
                                          CASE WHEN p.MemberID IS NOT NULL THEN 'L' || p.MemberID ELSE 'A' || p.ArchivedMemberID END AS MemberKey,
                                          CASE WHEN COALESCE(pl.TrainerID, p.TrainerID) IS NOT NULL
                                               THEN 'L' || COALESCE(pl.TrainerID, p.TrainerID)
                                               ELSE 'A' || COALESCE(pl.ArchivedTrainerID, p.ArchivedTrainerID) END AS CoachKey
                                   FROM Payments p LEFT JOIN Plans pl ON pl.PlanID = p.PlanID
                                   WHERE IFNULL(p.SessionType,'') <> 'Free Trial'
                                     AND p.Date>=@from AND p.Date<@to)
                               SELECT c.CoachKey, c.Coach, pe.MemberKey, pe.Member,
                                      COUNT(x.PaymentID) AS Payments, IFNULL(SUM(x.Amount),0) AS Amount
                               FROM coaches c
                               LEFT JOIN people pe
                                      ON pe.MemberKey IN (SELECT MemberKey FROM withCoach WHERE CoachKey = c.CoachKey)
                                      OR pe.MemberKey IN (SELECT MemberKey FROM pay WHERE CoachKey = c.CoachKey)
                               LEFT JOIN pay x ON x.MemberKey = pe.MemberKey AND x.CoachKey = c.CoachKey
                               WHERE c.Coach LIKE @s ESCAPE '\'
                                 AND (c.CoachKey LIKE 'L%' OR pe.MemberKey IS NOT NULL)
                               GROUP BY c.CoachKey, pe.MemberKey
                               ORDER BY c.Coach, c.CoachKey, pe.Member";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    string term = (coachSearch ?? "").Trim()
                        .Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
                    cmd.Parameters.AddWithValue("@s", "%" + term + "%");
                    AddPeriod(cmd, from, to);
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // ===== MONTHLY REPORT =====
        public class MonthlyReport
        {
            public decimal Memberships, Products;
            public int NewMembers, Sessions, FreeSessions;
            public DataTable Coaches, TopProducts, Methods;
        }

        public static MonthlyReport GetMonthlyReport(DateTime month)
        {
            var from = new DateTime(month.Year, month.Month, 1);
            var to   = from.AddMonths(1);
            var totals = GetRevenueTotals(from, to);
            var report = new MonthlyReport { Memberships = totals.Memberships, Products = totals.Products };
            using (var conn = GetConnection())
            {
                conn.Open();
                string f = from.ToString("yyyy-MM-dd HH:mm:ss"), t = to.ToString("yyyy-MM-dd HH:mm:ss");
                report.NewMembers = ScalarInt(conn,
                    @"SELECT (SELECT COUNT(*) FROM Members WHERE JoinDate>=@f AND JoinDate<@t)
                           + (SELECT COUNT(*) FROM ArchivedMembers WHERE JoinDate>=@f AND JoinDate<@t)", ("@f", f), ("@t", t));
                report.Sessions = ScalarInt(conn, "SELECT COUNT(*) FROM SessionLog WHERE UsedAt>=@f AND UsedAt<@t", ("@f", f), ("@t", t));
                report.FreeSessions = ScalarInt(conn,
                    "SELECT COUNT(*) FROM Payments WHERE SessionType='Free Trial' AND Date>=@f AND Date<@t", ("@f", f), ("@t", t));

                report.Coaches = PeriodTable(conn,
                    @"WITH
                      coaches AS (
                          SELECT 'L' || TrainerID AS K, Name AS Coach FROM Trainers
                          UNION ALL SELECT 'A' || ArchiveID, Name || ' (deleted)' FROM ArchivedTrainers),
                      pay AS (
                          SELECT p.Amount,
                                 COALESCE('L' || p.MemberID, 'A' || p.ArchivedMemberID) AS Who,
                                 CASE WHEN COALESCE(pl.TrainerID, p.TrainerID) IS NOT NULL
                                      THEN 'L' || COALESCE(pl.TrainerID, p.TrainerID)
                                      ELSE 'A' || COALESCE(pl.ArchivedTrainerID, p.ArchivedTrainerID) END AS K
                          FROM Payments p LEFT JOIN Plans pl ON pl.PlanID = p.PlanID
                          WHERE p.Amount > 0 AND p.Date>=@f AND p.Date<@t),
                      used AS (
                          SELECT CASE WHEN TrainerID IS NOT NULL THEN 'L' || TrainerID ELSE 'A' || ArchivedTrainerID END AS K
                          FROM SessionLog WHERE UsedAt>=@f AND UsedAt<@t)
                      SELECT c.Coach,
                             (SELECT COUNT(DISTINCT Who) FROM pay WHERE K = c.K) AS Members,
                             (SELECT COUNT(*) FROM pay WHERE K = c.K) AS Payments,
                             (SELECT COUNT(*) FROM used WHERE K = c.K) AS Sessions,
                             IFNULL((SELECT SUM(Amount) FROM pay WHERE K = c.K),0) AS Revenue
                      FROM coaches c
                      WHERE Payments > 0 OR Sessions > 0
                      ORDER BY Revenue DESC, c.Coach", f, t);

                report.TopProducts = PeriodTable(conn,
                    @"SELECT ProductName AS Product, SUM(Quantity) AS Sold, SUM(Total) AS Revenue
                      FROM ProductSales WHERE Date>=@f AND Date<@t
                      GROUP BY ProductName ORDER BY Revenue DESC, Sold DESC LIMIT 5", f, t);

                report.Methods = PeriodTable(conn,
                    @"SELECT Method, SUM(Amount) AS Revenue FROM (
                          SELECT Method, Amount FROM Payments WHERE Amount > 0 AND Date>=@f AND Date<@t
                          UNION ALL SELECT Method, Total FROM ProductSales WHERE Date>=@f AND Date<@t)
                      GROUP BY Method ORDER BY Revenue DESC", f, t);
            }
            return report;
        }

        private static DataTable PeriodTable(SQLiteConnection conn, string sql, string from, string to)
        {
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@f", from);
                cmd.Parameters.AddWithValue("@t", to);
                using (var adapter = new SQLiteDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        // ===== SINGLE SESSIONS =====
        private const string FreeTrial     = "Free Trial";
        private const string SingleSession = "Single Session";

        // A person's first session is free, once. The phone number identifies them,
        // including anyone deleted, so deleting someone doesn't give another free session.
        public static int RecordFreeSession(string name, string phone, int trainerID)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    using (var cmd = new SQLiteCommand(
                        @"SELECT Name FROM Members WHERE TRIM(Phone)=@p
                          UNION ALL SELECT Name FROM ArchivedMembers WHERE TRIM(Phone)=@p", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@p", phone.Trim());
                        object owner = cmd.ExecuteScalar();
                        if (owner != null)
                            throw new InvalidOperationException(
                                $"This phone number already belongs to {owner}, so the free first session isn't available.\n\nUse Pay One Session instead.");
                    }
                    long memberID = InsertWalkIn(conn, tx, name, phone);
                    InsertSession(conn, tx, memberID, trainerID, 0m, "Free", FreeTrial);
                    tx.Commit();
                    return (int)memberID;
                }
            }
        }

        // One paid session, for someone in the list (memberID) or someone new (memberID 0).
        // A member's plan and remaining sessions aren't touched.
        public static int RecordPaidSession(int memberID, string name, string phone, int trainerID, decimal amount, string method)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    long id = memberID;
                    if (id == 0)
                    {
                        using (var cmd = new SQLiteCommand(
                            "SELECT COUNT(*) FROM Members WHERE TRIM(Name)=@n AND IFNULL(TRIM(Phone),'')=@p", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@n", name.Trim());
                            cmd.Parameters.AddWithValue("@p", (phone ?? "").Trim());
                            if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                                throw new InvalidOperationException(
                                    $"{name} is already a member. Please choose them from the Person list.");
                        }
                        id = InsertWalkIn(conn, tx, name, phone);
                    }
                    InsertSession(conn, tx, id, trainerID, amount, method, SingleSession);
                    tx.Commit();
                    return (int)id;
                }
            }
        }

        // A member with no plan and no login, shown with the status "Walk-in".
        private static long InsertWalkIn(SQLiteConnection conn, SQLiteTransaction tx, string name, string phone)
        {
            long id = NextFreeId(conn, tx, "Members", "MemberID");
            using (var cmd = new SQLiteCommand(
                @"INSERT INTO Members (MemberID, Name, Phone, Age, Address, JoinDate)
                  VALUES (@id,@n,@p,0,'',@d)", conn, tx))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@n", name.Trim());
                cmd.Parameters.AddWithValue("@p", (phone ?? "").Trim());
                cmd.Parameters.AddWithValue("@d", Now());
                cmd.ExecuteNonQuery();
            }
            return id;
        }

        private static void InsertSession(SQLiteConnection conn, SQLiteTransaction tx, long memberID, int trainerID,
                                          decimal amount, string method, string sessionType)
        {
            using (var cmd = new SQLiteCommand(
                @"INSERT INTO Payments (MemberID, TrainerID, Amount, Date, Method, SessionType)
                  VALUES (@m,@t,@a,@d,@me,@s);
                  INSERT INTO SessionLog (MemberID, TrainerID, UsedAt) VALUES (@m,@t,@d);", conn, tx))
            {
                cmd.Parameters.AddWithValue("@m", memberID);
                cmd.Parameters.AddWithValue("@t", trainerID);
                cmd.Parameters.AddWithValue("@a", amount);
                cmd.Parameters.AddWithValue("@d", Now());
                cmd.Parameters.AddWithValue("@me", method);
                cmd.Parameters.AddWithValue("@s", sessionType);
                cmd.ExecuteNonQuery();
            }
        }

        // What this coach's last single session cost, or 150 for their first.
        public static decimal GetSingleSessionPrice(int trainerID)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    @"SELECT Amount FROM Payments WHERE TrainerID=@t AND SessionType=@s
                      ORDER BY Date DESC, PaymentID DESC LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@t", trainerID);
                    cmd.Parameters.AddWithValue("@s", SingleSession);
                    object v = cmd.ExecuteScalar();
                    return v == null || v == DBNull.Value ? 150m : Convert.ToDecimal(v);
                }
            }
        }

        // ===== DELETED RECORDS =====
        public static DataTable GetDeletedMembers() => QueryTable(
            @"SELECT ArchiveID, OriginalID AS [Old ID], Name, Phone, DeletedAt AS [Deleted On]
              FROM ArchivedMembers ORDER BY DeletedAt DESC");

        public static DataTable GetDeletedCoaches() => QueryTable(
            @"SELECT ArchiveID, OriginalID AS [Old ID], Name, Specialty, Phone, DeletedAt AS [Deleted On]
              FROM ArchivedTrainers ORDER BY DeletedAt DESC");

        private static DataTable QueryTable(string sql)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var adapter = new SQLiteDataAdapter(sql, conn))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        // Their old ID if it's still free, otherwise the lowest free one.
        private static long RestoredId(SQLiteConnection conn, SQLiteTransaction tx, string table, string idColumn, long oldID)
        {
            using (var cmd = new SQLiteCommand($"SELECT COUNT(*) FROM {table} WHERE {idColumn}=@id", conn, tx))
            {
                cmd.Parameters.AddWithValue("@id", oldID);
                if (Convert.ToInt32(cmd.ExecuteScalar()) == 0) return oldID;
            }
            return NextFreeId(conn, tx, table, idColumn);
        }

        // Puts a deleted member back with their history. Returns the ID they got.
        // `username` is their login, which gets a number added if someone took it since.
        public static int RestoreMember(int archiveID, out string username)
        {
            username = null;
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    string name, phone, hash;
                    long oldID;
                    using (var cmd = new SQLiteCommand("SELECT * FROM ArchivedMembers WHERE ArchiveID=@a", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@a", archiveID);
                        using (var r = cmd.ExecuteReader())
                        {
                            if (!r.Read()) throw new InvalidOperationException("This record no longer exists.");
                            oldID    = Convert.ToInt64(r["OriginalID"]);
                            name     = r["Name"].ToString();
                            phone    = r["Phone"]?.ToString() ?? "";
                            username = SafeGet(r, "Username");
                            hash     = SafeGet(r, "PasswordHash");
                        }
                    }

                    using (var cmd = new SQLiteCommand(
                        "SELECT COUNT(*) FROM Members WHERE TRIM(Name)=@n AND IFNULL(TRIM(Phone),'')=@p", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@n", name.Trim());
                        cmd.Parameters.AddWithValue("@p", phone.Trim());
                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            throw new InvalidOperationException(
                                $"A member named \"{name}\" with phone \"{phone}\" already exists. Change or delete that member first.");
                    }

                    object userID = DBNull.Value;
                    if (!string.IsNullOrEmpty(username))
                    {
                        string baseName = username;
                        for (int n = 2; UsernameTaken(conn, tx, username); n++) username = baseName + n;
                        using (var cmd = new SQLiteCommand(
                            "INSERT INTO Users (Username, Password, Role) VALUES (@u,@p,'Member'); SELECT last_insert_rowid();", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@u", username);
                            cmd.Parameters.AddWithValue("@p", hash);
                            userID = cmd.ExecuteScalar();
                        }
                    }

                    long newID = RestoredId(conn, tx, "Members", "MemberID", oldID);
                    using (var cmd = new SQLiteCommand(
                        @"INSERT INTO Members (MemberID, UserID, Name, Phone, Age, Address, JoinDate, TrainerID, ArchivedTrainerID,
                              PlanID, SessionsTotal, SessionsRemaining, PlanStart, PlanExpiry, PlanPaid)
                          SELECT @id, @uid, Name, Phone, Age, Address, JoinDate, TrainerID, ArchivedTrainerID,
                              PlanID, SessionsTotal, SessionsRemaining, PlanStart, PlanExpiry, PlanPaid
                          FROM ArchivedMembers WHERE ArchiveID=@a", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@id", newID);
                        cmd.Parameters.AddWithValue("@uid", userID);
                        cmd.Parameters.AddWithValue("@a", archiveID);
                        cmd.ExecuteNonQuery();
                    }
                    foreach (var table in new[] { "SessionLog", "Payments", "MemberPlans" })
                        using (var cmd = new SQLiteCommand(
                            $"UPDATE {table} SET MemberID=@id, ArchivedMemberID=NULL WHERE ArchivedMemberID=@a", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", newID);
                            cmd.Parameters.AddWithValue("@a", archiveID);
                            cmd.ExecuteNonQuery();
                        }
                    using (var cmd = new SQLiteCommand("DELETE FROM ArchivedMembers WHERE ArchiveID=@a", conn, tx))
                    { cmd.Parameters.AddWithValue("@a", archiveID); cmd.ExecuteNonQuery(); }

                    tx.Commit();
                    return (int)newID;
                }
            }
        }

        private static bool UsernameTaken(SQLiteConnection conn, SQLiteTransaction tx, string username)
        {
            using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Users WHERE Username=@u", conn, tx))
            {
                cmd.Parameters.AddWithValue("@u", username);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // Puts a deleted coach back with their plans. Members who moved to another coach
        // in the meantime stay there. Returns the ID the coach got.
        public static int RestoreCoach(int archiveID)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    long oldID;
                    using (var cmd = new SQLiteCommand("SELECT OriginalID FROM ArchivedTrainers WHERE ArchiveID=@a", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@a", archiveID);
                        object v = cmd.ExecuteScalar();
                        if (v == null) throw new InvalidOperationException("This record no longer exists.");
                        oldID = Convert.ToInt64(v);
                    }

                    long newID = RestoredId(conn, tx, "Trainers", "TrainerID", oldID);
                    using (var cmd = new SQLiteCommand(
                        @"INSERT INTO Trainers (TrainerID, Name, Specialty, Phone)
                          SELECT @id, Name, Specialty, Phone FROM ArchivedTrainers WHERE ArchiveID=@a", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@id", newID);
                        cmd.Parameters.AddWithValue("@a", archiveID);
                        cmd.ExecuteNonQuery();
                    }
                    foreach (var table in new[] { "Plans", "MemberPlans", "ArchivedMembers", "Payments", "SessionLog" })
                    {
                        using (var cmd = new SQLiteCommand(
                            $"UPDATE {table} SET TrainerID=@id WHERE ArchivedTrainerID=@a AND TrainerID IS NULL", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", newID);
                            cmd.Parameters.AddWithValue("@a", archiveID);
                            cmd.ExecuteNonQuery();
                        }
                        using (var cmd = new SQLiteCommand(
                            $"UPDATE {table} SET ArchivedTrainerID=NULL WHERE ArchivedTrainerID=@a", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@a", archiveID);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    using (var cmd = new SQLiteCommand("DELETE FROM ArchivedTrainers WHERE ArchiveID=@a", conn, tx))
                    { cmd.Parameters.AddWithValue("@a", archiveID); cmd.ExecuteNonQuery(); }

                    tx.Commit();
                    return (int)newID;
                }
            }
        }

        // ===== PRODUCTS =====
        public static List<Product> GetAllProducts()
        {
            var list = new List<Product>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT * FROM Products ORDER BY Category, Name, Size", conn))
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add(new Product
                        {
                            ProductID = Convert.ToInt32(r["ProductID"]),
                            Category  = r["Category"].ToString(),
                            Name      = r["Name"].ToString(),
                            Size      = r["Size"]?.ToString() ?? "",
                            Price     = Convert.ToDecimal(r["Price"]),
                            Stock     = Convert.ToInt32(r["Stock"])
                        });
            }
            return list;
        }

        public static void AddProduct(Product p) => SaveProduct(p,
            "INSERT INTO Products (Category, Name, Size, Price, Stock) VALUES (@c,@n,@s,@p,@st)");

        public static void UpdateProduct(Product p) => SaveProduct(p,
            "UPDATE Products SET Category=@c, Name=@n, Size=@s, Price=@p, Stock=@st WHERE ProductID=@id");

        private static void SaveProduct(Product p, string sql)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@c", p.Category);
                    cmd.Parameters.AddWithValue("@n", p.Name);
                    cmd.Parameters.AddWithValue("@s", p.Size ?? "");
                    cmd.Parameters.AddWithValue("@p", p.Price);
                    cmd.Parameters.AddWithValue("@st", p.Stock);
                    cmd.Parameters.AddWithValue("@id", p.ProductID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteProduct(int productID)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM Products WHERE ProductID=@id", conn))
                { cmd.Parameters.AddWithValue("@id", productID); cmd.ExecuteNonQuery(); }
            }
        }

        // Lowers the stock and records the sale. False + `error` if there isn't enough stock.
        public static bool SellProduct(int productID, int quantity, string method, out string error)
        {
            error = null;
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    using (var cmd = new SQLiteCommand(
                        "UPDATE Products SET Stock=Stock-@q WHERE ProductID=@id AND Stock>=@q", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@q", quantity);
                        cmd.Parameters.AddWithValue("@id", productID);
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            tx.Rollback();
                            error = "There isn't enough stock for this sale.";
                            return false;
                        }
                    }
                    using (var cmd = new SQLiteCommand(
                        @"INSERT INTO ProductSales (ProductID, ProductName, Category, Quantity, UnitPrice, Total, Method, Date)
                          SELECT ProductID, TRIM(Name || ' ' || IFNULL(Size,'')), Category, @q, Price, Price * @q, @m, @d
                          FROM Products WHERE ProductID=@id", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@q", quantity);
                        cmd.Parameters.AddWithValue("@m", method);
                        cmd.Parameters.AddWithValue("@d", Now());
                        cmd.Parameters.AddWithValue("@id", productID);
                        cmd.ExecuteNonQuery();
                    }
                    tx.Commit();
                    return true;
                }
            }
        }

        public static DataTable GetProductSales(DateTime? from = null, DateTime? to = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    @"SELECT SaleID, Date, ProductName AS Product, Category, Quantity, UnitPrice, Total, Method
                      FROM ProductSales WHERE Date>=@from AND Date<@to
                      ORDER BY Date DESC, SaleID DESC", conn))
                {
                    AddPeriod(cmd, from, to);
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // Removes a mistaken sale and returns the items to stock (if the product still exists).
        public static void UndoProductSale(int saleID)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    using (var cmd = new SQLiteCommand(
                        @"UPDATE Products SET Stock = Stock + (SELECT Quantity FROM ProductSales WHERE SaleID=@id)
                          WHERE ProductID = (SELECT ProductID FROM ProductSales WHERE SaleID=@id)", conn, tx))
                    { cmd.Parameters.AddWithValue("@id", saleID); cmd.ExecuteNonQuery(); }
                    using (var cmd = new SQLiteCommand("DELETE FROM ProductSales WHERE SaleID=@id", conn, tx))
                    { cmd.Parameters.AddWithValue("@id", saleID); cmd.ExecuteNonQuery(); }
                    tx.Commit();
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
                // "Active" must match what the rest of the app enforces: sessions left
                // AND the plan period still running. An expired member with sessions
                // on the clock cannot train, so they are not active.
                int active = ScalarInt(conn,
                    @"SELECT COUNT(DISTINCT MemberID) FROM MemberPlans
                       WHERE MemberID IS NOT NULL AND SessionsRemaining > 0
                         AND (IFNULL(PlanExpiry,'') = '' OR date(PlanExpiry) >= date('now','localtime'))");
                const string revenueSince = @"SELECT IFNULL((SELECT SUM(Amount) FROM Payments WHERE Date>=@d),0)
                                                   + IFNULL((SELECT SUM(Total) FROM ProductSales WHERE Date>=@d),0)";
                decimal monthRev = ScalarDec(conn, revenueSince, ("@d", monthStart));
                int weekSess = ScalarInt(conn, "SELECT COUNT(*) FROM SessionLog WHERE UsedAt>=@d", ("@d", weekAgo));
                decimal yearRev = ScalarDec(conn, revenueSince, ("@d", yearStart));
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
                               FROM (SELECT Date, Amount FROM Payments
                                     UNION ALL SELECT Date, Total FROM ProductSales)
                               WHERE Date>=@s GROUP BY M ORDER BY M";
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
                               JOIN Trainers t ON t.TrainerID=p.TrainerID
                               LEFT JOIN MemberPlans m ON m.PlanID=p.PlanID
                               WHERE IFNULL(p.IsDeleted,0)=0
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
                               FROM (SELECT Method, Amount FROM Payments
                                     UNION ALL SELECT Method, Total FROM ProductSales)
                               GROUP BY 1 ORDER BY Total DESC";
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

        // Same, but an absent value stays absent instead of becoming "now".
        // A member with no plan window is treated as never-expiring, not as expired.
        private static DateTime ParseDateOrDefault(object o)
        {
            if (o == null || o == DBNull.Value) return default(DateTime);
            return DateTime.TryParse(o.ToString(), out var d) ? d : default(DateTime);
        }
    }
}
