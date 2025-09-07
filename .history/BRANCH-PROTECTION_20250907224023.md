# Branch Protection Configuration

## Main Branch Protection Rules

The following rules should be configured in GitHub repository settings for the `main` branch:

### Required Status Checks
- ✅ Require status checks to pass before merging
- ✅ Require branches to be up to date before merging
- ✅ Required checks:
  - `Code Quality & Security`
  - `Build & Test`
  - `Quality Gate`

### Protect Matching Branches
- ✅ Require a pull request before merging
- ✅ Require approvals: **1**
- ✅ Dismiss stale reviews when new commits are pushed
- ✅ Require review from code owners (if CODEOWNERS file exists)

### Restrictions
- ✅ Restrict pushes that create files larger than 100MB
- ✅ Block force pushes
- ✅ Block deletions

### Rules Applied To
- Branch name pattern: `main`
- Include administrators: **No** (for flexibility during setup)

## How to Apply These Rules

1. Go to GitHub repository → Settings → Branches
2. Click "Add rule" for branch `main`
3. Configure the settings as described above
4. Save the protection rule

## Development Workflow

With these protections in place:

1. **Feature Development**: Work on feature branches
2. **Pull Request**: Create PR to `main` when ready
3. **CI Validation**: All CI checks must pass
4. **Code Review**: At least 1 approval required
5. **Merge**: Only after all checks pass and approval

This ensures production-quality code in the main branch.
