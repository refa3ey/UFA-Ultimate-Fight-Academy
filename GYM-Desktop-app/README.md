# GYM PRO — Gym Management System

A full-featured desktop application for managing gym memberships, attendance, payments, and analytics, built with WinForms .NET Framework 4.8 and a modern teal UI.

---

## Features

| Module | Highlights |
|--------|-----------|
| **Authentication** | BCrypt-hashed passwords, admin & member roles, change-password flow |
| **Member Management** | Add/edit/delete members, real-time search, status filter (Active / Expired / Expiring Soon), row colour coding |
| **Membership Plans** | Create and manage plans with pricing and duration |
| **Payments** | Record payments, method breakdown, payment history per member |
| **Attendance** | Check-in / check-out, attendance history, weekly stats |
| **QR Code System** | Per-member QR cards, kiosk Check-In Station (fullscreen), print & save member cards |
| **Analytics Dashboard** | Revenue line chart, member growth area chart, plan distribution doughnut, peak-hours bar chart, 4 live stat cards, 5-min auto-refresh |
| **Reports & Export** | Export to Excel (EPPlus) and PDF (PdfSharp) |
| **Trainers** | Manage trainer profiles |

---

## Screenshots
> <img width="892" height="538" alt="image" src="https://github.com/user-attachments/assets/9c416ace-e399-4368-9d93-938baa03985e" />
 
> <img width="1198" height="901" alt="image" src="https://github.com/user-attachments/assets/d61d0d45-2fe1-46c7-b096-1bfc8f13160f" />

> <img width="1094" height="672" alt="image" src="https://github.com/user-attachments/assets/1dc9c63f-f835-4636-9c3f-66afa4a02a7b" />

><img width="957" height="682" alt="image" src="https://github.com/user-attachments/assets/b0fe570a-e8ab-42f9-bf11-261b588208a6" />


---

## Installation (End Users)

### Prerequisites
- Windows 10 / 11 (Windows 7 / 8.1 also supported)
- [.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48) _(usually pre-installed on Win 10+)_
- [SQL Server LocalDB 2019+](https://aka.ms/sqllocaldb) — ships free with [SQL Server Express](https://aka.ms/sqlexpress)

### Steps
1. Download `GYMPRO-Setup-v1.0.exe`
2. Right-click → **Run as Administrator**
3. Follow the installer wizard
4. Launch **GYM PRO** from the Desktop or Start Menu

**Default login:** `admin` / `admin123` — change it immediately after first launch.

---

## Build Instructions (Developers)

### Requirements
- Visual Studio 2022 (Community or higher)
- .NET Framework 4.8 SDK
- NuGet packages (restored automatically on build)

### Steps
```
git clone https://github.com/refa3ey/GYM-Management-System.git
cd GYM-Management-System\GYM-Desktop-app
```

Open `GYM-Desktop-app.sln` in Visual Studio, or build from the command line:

```
"C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" ^
    GYM-Desktop-app.csproj /p:Configuration=Release /p:Platform=AnyCPU
```

Output: `bin\Release\GYM-Desktop-app.exe`

### Building the Installer
1. Install [Inno Setup 6](https://jrsoftware.org/isinfo.php)
2. Open `gym-pro-installer.iss` (in the repo root) in Inno Setup Compiler
3. Press **F9** (Build) or click **Build → Compile**
4. Installer output: `Output\GYMPRO-Setup-v1.0.exe`

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Language | C# (.NET Framework 4.8) |
| UI Framework | WinForms + Guna.UI2 2.0.4.7 |
| Database | SQL Server LocalDB (MDF file, no server install needed) |
| ORM / Data | ADO.NET (`SqlConnection`, `SqlCommand`) |
| Password Hashing | BCrypt.Net-Next 4.0.3 |
| QR Codes | QRCoder 1.4.3 |
| Charts | System.Windows.Forms.DataVisualization |
| Excel Export | EPPlus 4.5.3 |
| PDF Export | PdfSharp 1.50 |
| Installer | Inno Setup 6 |

---

## Project Structure

```
GYM-Desktop-app/
├── Database/         DatabaseHelper.cs — all SQL queries
├── Forms/            All WinForms screens
├── Helpers/          Theme, QR, Export, ModernMessageBox
├── Models/           Member, Payment, Attendance, Plan, Trainer, User
├── Properties/       AssemblyInfo.cs
├── Resources/        app.ico
├── app.manifest      UAC requireAdministrator
├── App.config        Connection string (uses |DataDirectory|)
└── GymDB.mdf         SQL Server LocalDB database
```

---

## License

MIT License — see [LICENSE.txt](LICENSE.txt)
