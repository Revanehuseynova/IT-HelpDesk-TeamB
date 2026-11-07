# IT Help Desk — Texniki Dəstək Müraciətlərinin İdarə Edilməsi Sistemi

Project Status: In Development

Universitet əməkdaşları və tələbələri üçün texniki problem müraciətlərinin qeydiyyatı və IT mütəxəssisləri
tərəfindən idarə olunması üçün **ASP.NET Core MVC** prototipi. 7 nəfərlik komandanın Git/GitHub ilə
inteqrasiya təcrübəsi üçün hazırlanıb.

## 1. Funksionallıq

* Ana səhifə + **ümumi say və statuslara görə statistika**
* Müraciətlərin siyahısı, **kateqoriya / status filtri** və başlıq üzrə axtarış
* Yeni müraciət: başlıq, təsvir, kateqoriya, prioritet
* Müraciətin **ayrıca detal səhifəsi**
* IT mütəxəssisi müraciəti **qəbul edir** (New → In Progress) və statusu dəyişir (New / In Progress / Resolved / Closed)
* Qeydiyyat, giriş, rollar (Student, Employee, ITSpecialist) və input validation
* İstifadəçi yalnız öz müraciətlərini görür və izləyir; IT mütəxəssisi hamısını görür

## 2. Texnologiyalar

HTML, CSS, Bootstrap 5 (CDN), JavaScript, C# **ASP.NET Core MVC (.NET 8)**, Entity Framework Core 8, **SQL Server**.

## 3. Quraşdırma

### 3.1 Tələblər

| Alət | Versiya |
|------|---------|
| .NET SDK | 8.0 və ya daha yeni (`dotnet --version`) |
| SQL Server | LocalDB / Express / Developer / Docker (2019+) |
| Git | 2.30+ |
| İnternet | Bootstrap və jQuery CDN-dən yüklənir |

### 3.2 Verilənlər bazasını yaradın

**Variant A — LocalDB (Windows, Visual Studio ilə gəlir):**
```bash
sqlcmd -S "(localdb)\MSSQLLocalDB" -i database/database.sql
```

**Variant B — SSMS / Azure Data Studio:** `database/database.sql` faylını açın və **Execute** edin.

**Variant C — Docker (macOS / Linux / Windows):**
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong!Passw0rd" -p 1433:1433 --name helpdesk-sql -d mcr.microsoft.com/mssql/server:2022-latest
sqlcmd -S localhost -U sa -P "YourStrong!Passw0rd" -C -i database/database.sql
```

Skript `ITHelpDeskDb` bazasını, cədvəlləri, əlaqələri, statistika view-nu və nümunə məlumatları yaradır.
> Skript yenidən işlədilə bilər, lakin cədvəlləri **silib yenidən yaradır** (yalnız development üçün).

### 3.3 Connection string

Default (`src/HelpDesk.Web/appsettings.json`) LocalDB üçündür. Docker/başqa server üçün **faylı dəyişib commit etməyin** —
user-secrets istifadə edin:

```bash
cd src/HelpDesk.Web
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=ITHelpDeskDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"
```

### 3.4 Build və işə salma

```bash
dotnet restore
dotnet build HelpDesk.sln
dotnet dev-certs https --trust            # yalnız bir dəfə (HTTPS sertifikatı)
dotnet run --project src/HelpDesk.Web --launch-profile https
```

Brauzerdə: **https://localhost:7080** (HTTP profili: `--launch-profile http` → http://localhost:5080).

### 3.5 Nümunə istifadəçilər (yalnız development)

| Rol | E-poçt | Parol |
|-----|--------|-------|
| IT Specialist | `it.specialist@university.edu` | `IT@12345` |
| Student | `student@university.edu` | `Student@123` |
| Employee | `employee@university.edu` | `Employee@123` |

Yeni Student/Employee `/Account/Register` səhifəsindən yaradıla bilər. IT Specialist yalnız SQL ilə əlavə olunur.

### 3.6 Tez-tez rast gəlinən problemlər

| Problem | Həll |
|---------|------|
| `Cannot open database "ITHelpDeskDb"` | `database/database.sql` işlədilməyib və ya connection string yanlışdır |
| `A connection was successfully established ... certificate chain` | Connection string-də `TrustServerCertificate=True` olmalıdır |
| HTTPS xəta / sertifikat xəbərdarlığı | `dotnet dev-certs https --trust` |
| Stillər görünmür | İnternet bağlantısını yoxlayın (Bootstrap CDN) |
| Giriş işləmir | SQL skriptini yenidən işlədin (nümunə hash-lər onunla gəlir) |

## 4. Layihənin strukturu

```
IT-HelpDesk/
├── HelpDesk.sln
├── README.md                     ← Documentation & DevOps
├── .gitignore                    ← Documentation & DevOps
├── .github/pull_request_template.md   ← Team Lead
├── .github/workflows/build.yml        ← Documentation & DevOps (CI build)
├── database/
│   ├── database.sql              ← Database Developer (cədvəllər, əlaqələr, nümunə data)
│   └── queries.sql               ← Database Developer (statistika və filtr sorğuları)
├── docs/
│   ├── TEAM-ROLES.md             ← kim hansı faylı və tapşırığı icra edir
│   ├── GIT-WORKFLOW.md           ← branch, PR, merge conflict, revert
│   ├── SECURITY.md               ← Security Developer
│   └── MERGE-LOG.md              ← Team Lead
├── tests/
│   ├── test-cases.md             ← QA Engineer
│   └── bug-report.md             ← QA Engineer
└── src/HelpDesk.Web/
    ├── Program.cs                ← Backend
    ├── appsettings*.json         ← Backend
    ├── Models/                   ← Ticket, Category, User, enum-lar (Backend); Roles.cs (Security)
    ├── Data/AppDbContext.cs      ← Backend
    ├── Controllers/              ← Home, Ticket (Backend); Account (Security)
    ├── ViewModels/               ← Ticket*/Home* (Backend); Login/Register (Security)
    ├── Helpers/                  ← DisplayExtensions (Backend); ClaimsPrincipalExtensions (Security)
    ├── Security/                 ← PasswordService, SecurityExtensions (Security)
    ├── Views/                    ← Layout, Home, Ticket, Shared (Frontend); Account (Security)
    └── wwwroot/                  ← css, js, prototype/index.html, prototype/tickets.html (Frontend)
```

Rollar üzrə dəqiq fayl və tapşırıq cədvəli: [`docs/TEAM-ROLES.md`](docs/TEAM-ROLES.md).

## 5. Komanda ilə iş (Git)

* Hər üzv **öz branch-ində** işləyir (`feature/frontend`, `feature/backend`, ...), `main`-ə **birbaşa push yoxdur**.
* İnteqrasiya Pull Request vasitəsilə Team Lead tərəfindən edilir.
* Merge conflict və `git revert` ssenariləri: [`docs/GIT-WORKFLOW.md`](docs/GIT-WORKFLOW.md).

## 6. Məlum məhdudiyyətlər

Bu, tədris prototipidir: parol sıfırlama, e-poçt təsdiqi, login bloklanması, fayl əlavələri və şərhlər yoxdur
(bax: `docs/SECURITY.md`, bölmə 5).
