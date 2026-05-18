# GitHub Actions Deployment

## `deploy-api.yml` — API → site4now (FTP)

### One-time setup

In the GitHub repo, go to **Settings → Secrets and variables → Actions → New repository secret**, and add:

| Secret           | Value                                                                 |
|------------------|-----------------------------------------------------------------------|
| `FTP_SERVER`     | `win8082.site4now.net` (from your hosting panel → FTP Address)        |
| `FTP_USERNAME`   | `smwadwaa-002` (from your hosting panel → FTP Login ID)               |
| `FTP_PASSWORD`   | Your FTP password (set / reset in the hosting panel if you forgot it) |
| `FTP_REMOTE_DIR` | `/site5/wwwroot/` — the remote folder for the target site             |

> **Find the right remote dir**: In the hosting panel, the API site listed as `shulenova-api` maps to `smwadwaa-002-site5.qtempurl.com` — site number `5`, so the FTP path is usually `/site5/wwwroot/`. For other sites, change the digit.

### How it runs

- **Auto**: every push to `main` that changes anything under `SchoolWebApp-API/` triggers a deploy.
- **Manual**: in the GitHub repo → **Actions** tab → **Deploy API to site4now (FTP)** → **Run workflow**.

### What it does

1. Checks out the repo.
2. Sets up .NET 8.
3. `dotnet publish` the API project in Release mode to `./publish`.
4. FTPs the result to the configured remote folder using `SamKirkland/FTP-Deploy-Action`. Only changed files are uploaded on subsequent runs (the action keeps a state file on the server).

### After deploy

- The host's IIS auto-reloads when files change. If your DB connection string or other secrets differ in production, set them in `appsettings.Production.json` on the server (the FTP excludes `appsettings.Development.json` so it won't overwrite local-only dev settings).
- First-time only: ensure `appsettings.Production.json` exists on the server with the live DB connection string. The deploy will not overwrite if you've added it manually under a name the workflow doesn't deploy.

### Troubleshooting

- **500.30** on first request after deploy → the app failed to start. SSH/RDP into the host (or open the hosting panel's Application Logs) and look at `stdout` logs. Usually a missing connection string or a migration error.
- **502.5** → wrong .NET runtime version on the host. Hosting panels usually let you pick the version; make sure it's set to **.NET 8 (LTS)**.
- **FTP connection refused** → some Windows hosts need `secure: false` plus passive mode; the `SamKirkland/FTP-Deploy-Action` defaults to FTP (port 21). If your host requires FTPS, add `protocol: ftps` to the workflow step.
