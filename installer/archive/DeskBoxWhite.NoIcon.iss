; DeskBoxWhite 安装脚本
; 构建命令：
; dotnet publish ..\src\DeskBoxWhite\DeskBoxWhite.csproj --configuration Release -p:Platform=x64 -p:RuntimeIdentifier=win-x64 -p:SelfContained=false -p:WindowsAppSDKSelfContained=false -o ..\artifacts\publish\DeskBoxWhite\x64 -v:minimal

#define MyAppName "DeskBoxWhite"
#define MyAppVersion "1.0.9"
#define MyAppPublisher "DeskBoxWhite 开发者"
#define MyAppExeName "DeskBoxWhite.exe"
#define MyAppOutputBaseName "DeskBoxWhite_Setup"
#define MyAppReleaseDir "..\artifacts\publish\DeskBoxWhite\x64"

[Setup]
; AppId 用于唯一标识同一个应用。
AppId={{1AB95974-9E77-46AC-8D97-8CA215DB684F}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppComments=安装包会按需检测并下载 .NET 8 Runtime 和 Windows App Runtime。
UninstallDisplayName={#MyAppName} {#MyAppVersion}
UninstallDisplayIcon={app}\Assets\deskboxwhite.ico
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
DefaultDirName={localappdata}\Programs\{#MyAppName}
DisableProgramGroupPage=yes
DisableDirPage=no
PrivilegesRequired=lowest
UsePreviousAppDir=no
UsePreviousPrivileges=no
CloseApplications=yes
CloseApplicationsFilter={#MyAppExeName}
RestartApplications=no
OutputDir=..\Output
OutputBaseFilename={#MyAppOutputBaseName}_{#MyAppVersion}_x64
VersionInfoVersion=1.0.9.0
VersionInfoProductVersion={#MyAppVersion}
VersionInfoTextVersion={#MyAppVersion}
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "chinesesimplified"; MessagesFile: "Languages\ChineseSimplified.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "autostart"; Description: "{cm:AutoStart}"; GroupDescription: "{cm:WindowsIntegration}"; Flags: unchecked

[InstallDelete]
Type: files; Name: "{userdesktop}\{#MyAppName}.lnk"; Tasks: desktopicon
Type: filesandordirs; Name: "{app}\Microsoft.WindowsAppRuntime"
Type: files; Name: "{app}\Microsoft.WinUI.dll"
Type: files; Name: "{app}\Microsoft.Windows.SDK.NET.dll"
Type: files; Name: "{app}\DirectML.dll"
Type: files; Name: "{app}\onnxruntime.dll"

[Files]
Source: "{#MyAppReleaseDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{userprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\Assets\deskboxwhite.ico"
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\Assets\deskboxwhite.ico"; Tasks: desktopicon
Name: "{userstartup}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; IconFilename: "{app}\Assets\deskboxwhite.ico"; Parameters: "--startup"; Tasks: autostart

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent runasoriginaluser

#include "DeskBoxWhite.Migration.iss"
#include "DeskBoxWhite.Dependencies.iss"
#include "DeskBoxWhite.Uninstall.iss"
