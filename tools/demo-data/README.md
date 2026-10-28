# Demo data

The `shulenova` demo site runs on a copy of a real school's database with
every person in it replaced by an invented one. Marks, fees, payroll, teaching
documents and elections are kept, so the demo shows a full, working school.

## Making a fresh copy

1. Back up the source database and restore it as `shulenova` (never work on
   the school's own database - the script refuses a name containing "swikunda"):

   ```
   mysqldump -uroot -p --single-transaction --routines --triggers --hex-blob swikunda > swikunda-backup.sql
   mysql -uroot -p -e "CREATE DATABASE shulenova CHARACTER SET utf8mb4"
   mysql -uroot -p shulenova < swikunda-backup.sql
   ```

2. Anonymise it, installing the demo licence (SN-DEMO-001 in
   `licensing-keys/issued-licences.csv`):

   ```
   MYSQL_PWD=... node tools/demo-data/anonymize.js --db shulenova --licence "SNL1...."
   ```

3. Check nothing is left from the original:

   ```
   MYSQL_PWD=... node tools/demo-data/check.js --source swikunda --db shulenova
   ```

   It lists any original name part, phone, email, ID number or the school's
   name still found, by table. Ordinary words that are also names (Junior,
   Christian, Adam and Love in CRE lessons) are expected.

4. Dump `shulenova` and restore it on the demo site's database.

## What changes

- Students and staff: names (same number of parts, title and capitals, and
  matching gender), phones, emails, addresses, ID / KRA / NHIF / NSSF
  numbers, staff numbers, dates of birth (moved up to four months), health
  notes and pick-up details. Admission numbers are kept.
- Logins: user names and names follow their staff member; every login except
  `admin` gets the password `Demo@2026` (change with `--password`), so the demo
  can sign in as a teacher or head teacher. "Created by" stamps everywhere
  follow the new user names.
- The school: ShuleNova Demo School, the ShuleNova logo, invented contacts.
- Sponsors and former schools get invented names; election candidate photos
  are removed.
- Deleted: audit trail, error logs, message history, login tokens.
- SMS and email are switched off with their credentials cleared, so the demo
  can never message anyone.

The same `--seed` gives the same names every run.
