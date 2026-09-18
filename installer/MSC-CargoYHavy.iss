#define MyAppName "MSC CargoYHavy"
#define MyAppVersion "1.1.0"
#define MyAppExeName "MSC-CargoYHavy.exe"
[Setup]
AppId={{9C4B4F7B-5C3D-4A75-9D73-2D9E0A5B1A11}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
DefaultDirName={autopf}\MSC CargoYHavy
DefaultGroupName={#MyAppName}
OutputDir=output
OutputBaseFilename=MSC-CargoYHavy-Setup
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
[Files]
Source: "..\publish\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion
[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent
