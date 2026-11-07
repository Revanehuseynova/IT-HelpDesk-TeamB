# Git workflow

Repository names: `IT-HelpDesk-Team-1`, `IT-HelpDesk-Team-2`, `IT-HelpDesk-Team-3`, `IT-HelpDesk-Team-4`.

## 1. Rules

1. Nobody pushes directly to `main`.
2. One branch per role: `feature/frontend`, `feature/backend`, `feature/database`, `feature/security`,
   `feature/testing`, `feature/documentation`.
3. Work → commit (small, meaningful messages) → push the branch → open a Pull Request into `main`.
4. The Team Lead reviews, may request changes, and merges.
5. Every member makes **his/her own commits** with his/her own name and e-mail. No one commits for another person.
6. Never commit `bin/`, `obj/`, passwords or personal connection strings.

Recommended GitHub setting for the Team Lead: *Settings → Branches → Add rule → `main`* → require a Pull Request
before merging, 1 approval.

## 2. Team Lead — initial setup

Create the empty repository on GitHub, then on your computer (inside the folder that contains only the Team Lead
files from `docs/TEAM-ROLES.md`):

```bash
git config --global user.name "Team Lead Name"
git config --global user.email "email@example.com"

git init
git branch -M main
git remote add origin REPOSITORY_URL
```

`README.md` stub (the line `Project Status: In Development` is needed for the conflict exercise):

```markdown
# IT Help Desk

Project Status: In Development
```

```bash
git add .
git commit -m "Initial project setup"
git push -u origin main
```

Invite the 6 team members to the repository (*Settings → Collaborators*).

## 3. Team members — start working

```bash
git config --global user.name "Your Name"
git config --global user.email "your@email.com"

git clone REPOSITORY_URL
cd IT-HelpDesk-Team-1

git switch -c feature/frontend        # your own branch (see names above)
```

Copy/create **your role's files** (see `TEAM-ROLES.md`), then:

```bash
git status                            # what changed?
git add .                             # or add file by file
git diff --staged                     # review exactly what will be committed
git commit -m "Add frontend ticket pages"
git push -u origin feature/frontend
```

On GitHub: **Compare & pull request** → base `main` ← compare `feature/frontend` → fill in the PR template.

### Keep your branch up to date

```bash
git fetch origin
git switch feature/frontend
git merge origin/main                 # or: git pull origin main
```

## 4. Pull Request review (Team Lead)

1. Open the PR → *Files changed*. Check: only the owner's files, no `bin/obj`, no secrets, clear commit messages.
2. Not OK → *Review changes → Request changes* and write what to fix. The member fixes on the same branch, commits and pushes
   (the PR updates automatically).
3. OK → *Approve* → *Merge pull request*.
4. Locally: `git switch main`, `git pull origin main`, `dotnet build`.
5. Write the result into `docs/MERGE-LOG.md`.

## 5. Mandatory scenario: merge conflict

**Preparation (teacher):** pick two students — Frontend and Backend Developer.
Do this **after** the README PR is merged and everybody has done `git pull origin main`, so that `README.md`
contains the line:

```
Project Status: In Development
```

**Step 1 — both developers (independently, from the updated main):**

```bash
git switch main && git pull origin main
git switch -c feature/frontend-readme-status        # Backend: feature/backend-readme-status
# edit README.md — change ONLY that line
git add README.md
git commit -m "Update project status (frontend)"    # Backend: "(backend)"
git push -u origin feature/frontend-readme-status
```

Frontend writes `Project Status: Frontend Completed`, Backend writes `Project Status: Backend Completed`.
Both open a Pull Request into `main`.

**Step 2 — Team Lead:** merges the first PR. The second PR shows *"This branch has conflicts"*.

**Step 3 — Team Lead resolves it locally:**

```bash
git switch main
git pull origin main
git merge feature/backend-readme-status
```

Git reports `CONFLICT (content): Merge conflict in README.md`. Check the state:

```bash
git status                  # README.md is "both modified"
git diff                    # shows the markers
```

The file now contains:

```
<<<<<<< HEAD
Project Status: Frontend Completed
=======
Project Status: Backend Completed
>>>>>>> feature/backend-readme-status
```

* `HEAD` part = what is already in `main` (Frontend's change); the part below `=======` = Backend's change.
* **Cause:** both branches changed the *same line* of the *same file* starting from the same base commit,
  so Git cannot know which version is correct.
* **Decision:** both statements are true, so the final text must reflect both works.

Remove the three marker lines and write:

```
Project Status: Frontend and Backend Completed
```

```bash
git add README.md
git commit -m "Resolve README merge conflict"
git push origin main
```

**Step 4 — verify:** open `README.md` on GitHub, check that no `<<<<<<<` markers remain, and that both PRs are shown as merged/closed.
Write the cause, decision and commands into `docs/MERGE-LOG.md`.

> If the conflict appears in the GitHub web editor, it can be resolved there ("Resolve conflicts" button) or locally;
> the requirement is that the Team Lead explains the cause and the solution.

Abort option if you are lost: `git merge --abort`.

## 6. Mandatory scenario: `git revert`

**Situation (teacher):** *"A wrong commit was made in the first version of the project."* (e.g. somebody committed `bin/` or a
broken file.) The Team Lead must undo it **without deleting history**.

```bash
git switch main
git pull origin main
git log --oneline                     # find the hash of the wrong commit
git show <hash>                       # check that it really is the wrong one
git revert <hash>                     # creates a NEW commit that undoes it
git push origin main
```

* If the wrong commit is a **merge commit** (from a PR): `git revert -m 1 <merge-hash>` (`-m 1` = keep the `main` side).
* If a conflict appears during revert: fix files → `git add` → `git revert --continue`. To cancel: `git revert --abort`.
* Share the result with the team (GitHub Issue / message): which commit, why, the new revert hash. Everyone updates with `git pull`.
* Why not `git reset --hard` + force push? It rewrites shared history and breaks other people's clones.

## 7. Additional scenarios (teacher may give them during the seminar)

| Situation | Command |
|-----------|---------|
| Developer realises he is on the wrong branch | `git switch feature/xxx` (uncommitted changes move along; if Git refuses, use `git stash` first) |
| Must switch to another task without committing | `git stash` → switch/work → `git switch back` → `git stash pop` |
| Accidentally edited a file, want the previous state | `git restore path/to/file` (unstaged) / `git restore --staged path/to/file` (unstage) |
| See history of recent commits | `git log --oneline --graph --decorate -10` |
| See content of a specific commit | `git show <hash>` |
| Look at remote changes without merging | `git fetch origin`, then `git log HEAD..origin/main --oneline` |
| Bring remote changes into the current branch | `git pull origin main` |
| Undo a bad commit safely | `git revert <hash>` |
| Compare before/after of a file | `git diff`, `git diff --staged`, `git diff main..feature/xxx -- path/file` |
| Mark a release version | `git tag -a v1.0 -m "First working prototype"` then `git push origin v1.0` |
| Rename a file inside the repository | `git mv OldName.cs NewName.cs` |
| Remove a file from the repository | `git rm path/to/file` (keep on disk: `git rm --cached path/to/file`) |
| Who last changed a line | `git blame path/to/file` |
| Check remote addresses | `git remote -v` |

Bonus (fast teams): `git reset` (local only!), `git rebase`, `git cherry-pick <hash>`.

## 8. Every member's command checklist (tick in your PR description)

`git status` · `git add` · `git commit` · `git log` · `git diff` · `git branch` · `git switch` · `git push` · `git pull` ·
`git fetch` · `git restore` · `git stash` · `git show` · `git remote -v`

Not everybody must use every command equally, but everybody must use the commands that match his/her role (see `TEAM-ROLES.md`).
