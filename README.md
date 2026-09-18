# MSC CargoYHavy — Phase 1.1 Enhanced

Enhanced C# WinForms + Microsoft WebView2 desktop client.

Includes:
- persistent WebView2 profile/session
- registration startup URL with invite code
- animated loading overlay and progress indicator
- retry/error screen
- Back, Forward, Refresh and Home controls
- download handling
- external protocol handling
- WebView2 process-failure handling
- single-instance protection
- GitHub Actions x64/x86 build
- Inno Setup installer

Startup URL:
https://cargyhavy.icu/pages/login/register?invite=08OGL9

The dashboard URL is intentionally not invented. If the website redirects authenticated users, WebView2 follows it.

Build locally:
dotnet restore src/MSC-CargoYHavy.csproj
dotnet publish src/MSC-CargoYHavy.csproj -c Release -r win-x64 --self-contained true
