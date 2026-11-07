# Security notes  (owner: Authentication & Security Developer)

## 1. Roles

| Role | Who | What the role can do |
|------|-----|----------------------|
| `Student` | Registered student | Create tickets, see **only own** tickets, see own statistics |
| `Employee` | University staff member | Same as Student |
| `ITSpecialist` | IT support staff | See **all** tickets and statistics, accept tickets, change status |

* Roles are stored in `Users.Role` and added to the cookie as a `ClaimTypes.Role` claim.
* Self-registration offers only `Student` and `Employee` (server-side regex `^(Student|Employee)$`).
  IT specialists are inserted by an administrator through SQL (see `database/database.sql`).
* Permissions are enforced on the **server** (`[Authorize]`, `[Authorize(Roles = Roles.ITSpecialist)]`,
  and query filtering in `TicketController.VisibleTickets()`), never only by hiding buttons.

## 2. Authentication

* Cookie authentication (`HelpDesk.Auth`), HttpOnly, SameSite=Lax, 8 hours sliding expiration.
* Passwords are never stored in plain text: PBKDF2-HMAC-SHA256, 100 000 iterations, random 16-byte salt
  (`Security/PasswordService.cs`). Hash comparison is constant-time (`CryptographicOperations.FixedTimeEquals`).
* The same error message is returned for an unknown e-mail and a wrong password (no user enumeration).
* `returnUrl` is accepted only if `Url.IsLocalUrl(...)` (open-redirect protection).

## 3. Input validation

| Place | Rules |
|-------|-------|
| Login | E-mail required + valid format; password required |
| Register | Full name 3–100; e-mail valid, ≤150, unique; password ≥8 with upper, lower and digit; passwords must match |
| New ticket | Title 5–150; description 10–2000; category required and must exist in DB; priority must be a defined enum value |
| Status change | Value must be a defined `TicketStatus`; only `ITSpecialist` |

Validation runs **twice**: in the browser (jQuery Validation, user experience) and on the server
(`ModelState.IsValid`, the real protection).

## 4. Other protections

| Threat | Protection |
|--------|------------|
| SQL injection | Entity Framework Core (parameterised queries); no string-concatenated SQL |
| XSS | Razor HTML-encodes all output; no `Html.Raw` is used; CSP header in non-Development |
| CSRF | `AutoValidateAntiforgeryTokenAttribute` on all unsafe HTTP methods + `<form>` tag helper tokens |
| Over-posting | Forms bind to dedicated view models, not to entities; status/assignee/creator are set on the server |
| IDOR (reading another user's ticket) | Ticket queries are filtered by `CreatedById`; foreign tickets return 404 |
| Clickjacking / sniffing | `X-Frame-Options: DENY`, `X-Content-Type-Options: nosniff` |
| Secrets in Git | `.gitignore`, connection string via user-secrets / environment variable |

## 5. Known limitations of this teaching prototype

* No account lockout / rate limiting after repeated failed logins.
* No e-mail confirmation or password reset.
* CDN scripts (Bootstrap, jQuery) are loaded without Subresource Integrity hashes.
* No audit log of status changes.
