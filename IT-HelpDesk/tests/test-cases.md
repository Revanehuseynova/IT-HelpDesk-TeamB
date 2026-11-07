# Test cases  (owner: QA / Test Engineer)

**Environment:** local, `https://localhost:7080`, database created from `database/database.sql`.
**Test accounts:** `it.specialist@university.edu / IT@12345`, `student@university.edu / Student@123`,
`employee@university.edu / Employee@123`.

> The *Actual result* and *Status* columns are filled in by the QA Engineer **after really running** each case.
> Do not mark a case "Pass" without executing it.

| ID | Area | Scenario | Steps | Expected result | Actual result | Status |
|----|------|----------|-------|-----------------|---------------|--------|
| TC-01 | Auth | Successful login (student) | Open `/Account/Login`, enter student credentials, submit | Redirect to home page; user name and role badge visible in navbar | | Not run |
| TC-02 | Auth | Login with wrong password | Enter valid e-mail, wrong password | Stay on login page, message "E-poçt və ya parol yanlışdır." | | Not run |
| TC-03 | Auth | Register with weak password | Register with password `abc` | Validation errors, user is not created | | Not run |
| TC-04 | Auth | Register with existing e-mail | Register using `student@university.edu` | Error "Bu e-poçt artıq qeydiyyatdan keçib." | | Not run |
| TC-05 | Tickets | Create a valid ticket | Login as student → *Yeni müraciət* → fill title (≥5), description (≥10), category, priority → submit | Ticket created with status **New**, details page opens, success message shown | | Not run |
| TC-06 | Tickets | Create ticket with empty/short fields | Leave title empty and description "abc" | Validation messages for title and description; nothing saved | | Not run |
| TC-07 | Tickets | Create ticket without category | Do not choose a category | Message "Kateqoriya seçilməlidir."; nothing saved | | Not run |
| TC-08 | List | Student sees only own tickets | Login as student, open *Müraciətlər* | Only tickets created by this student are listed | | Not run |
| TC-09 | List | IT specialist sees all tickets | Login as IT specialist, open *Müraciətlər* | All tickets are listed, "Yaradan" column visible | | Not run |
| TC-10 | Filter | Filter by category | Choose a category in the filter | Only tickets of that category are shown; "Sıfırla" restores the list | | Not run |
| TC-11 | Filter | Filter by status | Choose status *In Progress* | Only *In Progress* tickets are shown | | Not run |
| TC-12 | Filter | Combined filter + search | Choose category + status + enter part of a title | Result satisfies all three conditions | | Not run |
| TC-13 | Details | Ticket details page | Click *Ətraflı* on a ticket | Separate page shows title, description, category, priority, status, creator, assignee, dates | | Not run |
| TC-14 | Status | IT specialist accepts ticket | Login as IT specialist, open a **New** ticket, click *Müraciəti qəbul et* | Status becomes **In Progress**, assignee = current IT specialist | | Not run |
| TC-15 | Status | IT specialist changes status | Change status to *Resolved*, then *Closed* | Status and "Yenilənib" date are updated each time | | Not run |
| TC-16 | Status | Student cannot change status | Login as student, open own ticket | No "IT mütəxəssisi üçün əməliyyatlar" panel; a manual POST to `/Ticket/ChangeStatus/{id}` returns 403 / access denied | | Not run |
| TC-17 | Security | Access another user's ticket | Login as student, open `/Ticket/Details/{id of employee's ticket}` | HTTP 404 (ticket is not shown) | | Not run |
| TC-18 | Security | Anonymous access | Logged out, open `/Ticket` | Redirect to login page | | Not run |
| TC-19 | Security | XSS attempt | Create ticket with title `<script>alert(1)</script>` | Text is shown encoded, no script executes | | Not run |
| TC-20 | Stats | Statistics on home page | Compare numbers on home page with the list (and `SELECT * FROM vw_TicketStatistics` for IT specialist) | Total and per-status numbers match | | Not run |
| TC-21 | Security | Logout | Click *Çıxış* | Session ends; protected pages redirect to login | | Not run |

## Found defects

See `tests/bug-report.md`.
