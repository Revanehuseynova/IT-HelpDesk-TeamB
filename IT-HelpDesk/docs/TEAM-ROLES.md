# Team roles, files and tasks

7 members: 1 Team Lead + 6 developers. **Every member writes/commits the files of his or her own role
from his or her own computer and own branch.** Only the Team Lead merging and the others only committing
is not acceptable.

## How the ZIP is meant to be used

The ZIP is a **complete reference solution**, not a Git repository (it contains no `.git` folder and no commits).

1. Team Lead creates the empty GitHub repository, copies **only the Team Lead files** (table below) and makes the
   initial commit (see `GIT-WORKFLOW.md`, section 2).
2. Each member clones the repository, creates his/her branch and brings **only the files of his/her role**
   from the ZIP (copy, read, understand, adapt if needed), then commits **under his/her own Git name**.
3. After all Pull Requests are merged, `main` is identical to the complete project.

> Do not run `dotnet build` in the repository before `.gitignore` is merged (or copy `.gitignore` first),
> otherwise `bin/` and `obj/` folders may be committed by accident.

## Roles

### 1. Team Lead — branch: `main` (initial commit) + reviews

| Files | |
|-------|--|
| `HelpDesk.sln`, `src/HelpDesk.Web/HelpDesk.Web.csproj`, `src/HelpDesk.Web/Properties/launchSettings.json` | Solution skeleton (initial commit) |
| `README.md` (short stub only, see `GIT-WORKFLOW.md`) | Needed for the merge-conflict exercise |
| `.github/pull_request_template.md` | PR checklist |
| `docs/MERGE-LOG.md` | Filled in during the work |

Tasks: create GitHub repository `IT-HelpDesk-Team-N` and add members; initial commit; define branch policy
(protect `main`: PR required, 1 approval); review every Pull Request (request changes when needed); decide the merge
order; resolve the README **merge conflict**; **`git revert`** the wrong commit; verify `dotnet build` after merges;
tag the release (`git tag v1.0`).

### 2. Frontend Developer — branch: `feature/frontend`

Files: `Views/_ViewImports.cshtml`, `Views/_ViewStart.cshtml`, `Views/Shared/*` (Layout, badges, validation scripts, Error),
`Views/Home/Index.cshtml`, `Views/Ticket/*.cshtml`, `wwwroot/css/site.css`, `wwwroot/js/site.js`, `wwwroot/js/prototype.js`,
`wwwroot/prototype/index.html`, `wwwroot/prototype/tickets.html`.

Tasks: home page with statistics cards; ticket list with filter form; create form; details page; responsive Bootstrap
layout; client-side validation scripts; JavaScript (alerts, auto-submit filters, character counter).

### 3. Backend Developer — branch: `feature/backend`

Files: `Program.cs`, `appsettings.json`, `appsettings.Development.json`, `Models/Ticket.cs`, `Models/Category.cs`,
`Models/User.cs`, `Models/TicketStatus.cs`, `Models/TicketPriority.cs`, `Data/AppDbContext.cs`,
`Controllers/TicketController.cs`, `Controllers/HomeController.cs`, `ViewModels/Ticket*ViewModel.cs`,
`ViewModels/HomeViewModel.cs`, `Helpers/DisplayExtensions.cs`.

Tasks: C# models; DbContext mapping to the SQL schema; create ticket; accept ticket; change status; filtering by
category/status; statistics; ownership rules (user sees own tickets).

### 4. Database Developer — branch: `feature/database`

Files: `database/database.sql`, `database/queries.sql`.

Tasks: tables (`Users`, `Categories`, `Tickets`), primary/foreign keys, unique and check constraints, indexes,
statistics view, sample data (users with hashed passwords, categories, tickets), useful SQL queries.

### 5. Authentication & Security Developer — branch: `feature/security`

Files: `Models/Roles.cs`, `Security/PasswordService.cs`, `Security/SecurityExtensions.cs`, `Controllers/AccountController.cs`,
`ViewModels/LoginViewModel.cs`, `ViewModels/RegisterViewModel.cs`, `Helpers/ClaimsPrincipalExtensions.cs`,
`Views/Account/*.cshtml`, `docs/SECURITY.md`.

Tasks: login form, registration, roles (Student / Employee / ITSpecialist) and their description, password hashing,
cookie authentication, input validation, anti-forgery, open-redirect protection, security headers.

### 6. QA / Test Engineer — branch: `feature/testing`

Files: `tests/test-cases.md`, `tests/bug-report.md`.

Tasks: run at least 5 (here 21) test scenarios on the integrated build, fill in *Actual result* and *Status*, report
**really found** defects in `bug-report.md` and tell the owner (open a GitHub Issue). Retest after fixes.

### 7. Documentation & DevOps Engineer — branch: `feature/documentation`

Files: `README.md` (full version), `.gitignore`, `.github/workflows/build.yml` (automatic build check on every PR), `docs/TEAM-ROLES.md`, `docs/GIT-WORKFLOW.md`.

Tasks: setup instructions (verify them on a clean machine), project structure, `.gitignore`, Git documentation,
check that nobody committed `bin/`, `obj/` or secrets, GitHub Actions build workflow (it shows a green/red check on every Pull Request; before the Backend and Security PRs are both merged it is expected to be red).

## Recommended integration order (Team Lead)

| Step | Branch | Remark |
|------|--------|--------|
| 1 | initial commit on `main` | skeleton |
| 2 | `feature/documentation` | brings `.gitignore` early |
| 3 | `feature/database` | SQL only, no C# dependency |
| 4 | `feature/backend` **and** `feature/security` | they depend on each other (`Program.cs` ↔ `SecurityExtensions`, `Roles`), so the project compiles only after **both** are merged → run `dotnet build` after the second merge |
| 5 | `feature/frontend` | views need the view models/controllers |
| 6 | `feature/testing` | QA tests the integrated `main` build |

## Git commands each role should practise

| Role | Minimum |
|------|---------|
| Everyone | `git clone`, `git switch -c`, `git status`, `git add`, `git diff --staged`, `git commit`, `git push -u origin <branch>`, `git log`, `git pull` |
| Frontend | + `git restore`, `git stash` |
| Backend | + `git diff`, `git show` |
| Database | + `git restore`, `git log` |
| Security | + `git blame`, `git show` |
| QA | + `git fetch`, `git switch` (test other branches), `git log` |
| Documentation | + `git mv`, `git rm`, `git remote -v`, `git tag` (optional) |
| Team Lead | + `git fetch`, `git merge`, conflict resolution, `git revert`, `git tag` |
