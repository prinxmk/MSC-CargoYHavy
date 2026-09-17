# MSC CargoYHavy — Phase 1

Complete GitHub-ready Windows desktop wrapper for the CargoYHavy web application.

## Startup

The app uses a persistent WebView2 profile. It starts at:

https://cargyhavy.icu/pages/login/register?invite=08OGL9

If an existing authenticated web session causes the website to redirect to the dashboard, the dashboard is shown automatically. Otherwise the registration page is shown, as requested. The user's Login button remains available on the website.

`src/AppConfig.cs` contains `DashboardUrl` if the exact dashboard URL is later confirmed.

## Included

- C# .NET 8 Windows Forms
- Microsoft WebView2
- Persistent cookies/session data
- Navigation toolbar
- Downloads to Windows Downloads folder
- Upload/file picker support through WebView2
- External protocol handling
- Connection status
- GitHub Actions
- Windows x64 and x86 builds
- Inno Setup installers
- Desktop and Start Menu shortcuts

## GitHub

Upload the contents of this folder to a repository, commit to `main`, then open **Actions → Build MSC CargoYHavy → Run workflow**. Download the generated installer artifact for testing.

## Important

The desktop shell does not access PHP/MySQL directly. Authentication is retained through WebView2 cookies/session storage, which keeps the website's own authentication mechanism intact.

Microsoft Edge WebView2 Runtime must be present on the PC. Modern Windows installations commonly have it; if it is missing, install the Evergreen WebView2 Runtime before testing.
