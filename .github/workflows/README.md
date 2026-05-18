# GitHub Actions Deployment

Two manual workflows ship code to site4now over FTP:

- `deploy-api.yml` → the .NET 8 API
- `deploy-ui.yml`  → the Angular client

## One-time setup (shared secrets)

In the GitHub repo, go to **Settings → Secrets and variables → Actions → New repository secret**, and add:

| Secret              | Used by         | Value                                                                          |
|---------------------|-----------------|--------------------------------------------------------------------------------|
| `FTP_SERVER`        | both            | `win8082.site4now.net` (from hosting panel → FTP Address)                      |
| `FTP_USERNAME`      | both            | `smwadwaa-002` (from hosting panel → FTP Login ID)                             |
| `FTP_PASSWORD`      | both            | Your FTP password                                                              |
| `FTP_REMOTE_DIR`    | `deploy-api.yml`| `/swikunda-api/` — the remote folder the API site is bound to in IIS          |
| `FTP_REMOTE_DIR_UI` | `deploy-ui.yml` | `/swikunda-ui/`  — the remote folder the UI site is bound to in IIS           |

> **Find the right remote dir**: Log into FileZilla with these credentials. The root listing shows the named site folders (`swikunda-api`, `swikunda-ui`, `shulenova-api`, …). Each named folder IS the IIS document root — files go directly inside it, with no `wwwroot/` subfolder. Use whichever name matches the site you're deploying to.

## How they run

Both workflows are **manual only** — no auto-deploy on push.

To deploy: GitHub repo → **Actions** tab → pick **Deploy API to site4now (FTP)** or **Deploy UI to site4now (FTP)** → **Run workflow** → pick `master` → **Run workflow**.

## What each one does

### `deploy-api.yml`
1. Checks out the repo.
2. Sets up .NET 8.
3. `dotnet publish` the API project in Release mode to `./publish`.
4. FTPs the output to `FTP_REMOTE_DIR`. The exclude list keeps `appsettings.Development.json` and `appsettings.Production.json` off the wire so the live config on the server is never overwritten.

### `deploy-ui.yml`
1. Checks out the repo.
2. Sets up Node 20 with npm cache.
3. `npm ci` then `npm run build` (which is `ng build --configuration production`).
4. FTPs `SchoolWebApp-Client/dist/shulenova/` to `FTP_REMOTE_DIR_UI`.

Both use `SamKirkland/FTP-Deploy-Action` with a per-site `.ftp-deploy-sync-state.json` on the server, so subsequent runs upload only what changed.

## After API deploy

- IIS auto-reloads when files change.
- **First-time only**: FTP an `appsettings.Production.json` into the API folder with the live DB connection string and JWT key. Without it the app starts with empty config and crashes on the first DB hit. Future deploys won't touch it (excluded).

## After UI deploy

- Reload the site in your browser. If the new bundle hash isn't loading, do a hard refresh (Ctrl+Shift+R) to bypass the browser cache for `index.html`.
- The Angular build copies `src/web.config` into the dist, which gives the SPA the IIS rewrite rules it needs for deep links.

## Troubleshooting

- **API 500.30** on first request → app failed to start. Check the hosting panel's Application Logs / `stdout` logs. Usually a missing connection string or a migration error.
- **502.5** → wrong .NET runtime version on the host. Set the site to **.NET 8 (LTS)** in the hosting panel.
- **CORS errors in the browser** → the IIS `WebDAVModule` intercepts `OPTIONS` preflights on Windows shared hosts. The included [`Project.API/web.config`](../../SchoolWebApp-API/Project.API/web.config) removes WebDAV; if you still see CORS errors after deploy, confirm that web.config landed on the server and contains the `<remove name="WebDAVModule" />` line.
- **`ECONNRESET (data socket)`** during FTP upload → transient hosting hiccup. Re-run the workflow. If persistent, your hosting quota may be full — clean up orphan folders from earlier mis-targeted deploys.
- **FTP connection refused** → if your host requires FTPS, add `protocol: ftps` and `security: loose` to the workflow's FTP-Deploy step.
