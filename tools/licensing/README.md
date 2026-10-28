# ShuleNova licences

Each school's installation has a licence key listing the modules it bought
and when the licence ends. Core (dashboard, school, students, staff, settings,
reports, security) is always on. The paid modules are:

| Module | Covers |
|---|---|
| `Academics` | Academics menu (assessments, exams, learner development, academic reports) and Teaching Management |
| `Finance` | Fees, sponsors, budgets, expenses, finance reports |
| `Payroll` | Salaries, loans, payroll processing and reports |
| `Elections` | Student elections |
| `Communications` | SMS and messaging add-on (also the "send to parents" buttons in other modules) |

The API refuses calls to a module the licence does not include; menus,
pages, settings sections and dashboard cards hide it.

## Issuing a key

Keys are signed with Kodetek's private key, kept in `licensing-keys/`
(git-ignored). **Back that folder up somewhere safe**: without the private key
no new keys can be issued, and anyone holding it can issue them.

```
node tools/licensing/issue-licence.js \
  --school "Swikunda Primary School" \
  --host swikunda.kodetek.co.ke \
  --modules Academics,Finance,Payroll,Elections,Communications \
  --expires 2027-12-31
```

- `--host` ties the key to the school's site (repeat or comma-separate for
  several). Without it the key works on any site. `localhost` always works,
  for development.
- `--id` sets the licence id (default: generated).
- Every key issued is recorded in `licensing-keys/issued-licences.csv`.

The school's administrator pastes the key into **Settings > Licence**. The key
is checked before it replaces the current one.

## Expiry

Administrators see a warning from 30 days before the expiry date. After it,
the modules keep working for a 14-day grace period, then only core is
available until a renewed key is installed. No data is removed.

## Changing the signing key

The public key is in `SchoolWebApp-API/Project.API/Licensing/LicenceService.cs`
(`PublicKey`). Replacing the key pair invalidates every key already issued.
