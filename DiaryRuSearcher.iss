﻿; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "DiaryRuSearcher"

; Take project folder from systen environment variable
#define MyProjectName MyAppName
#define MyGitProjects "MyGitProjects"
#if GetEnv(MyGitProjects) == ""
#define MyDistFolder "C:\Users\"+GetEnv("UserName")+"\Documents\Atlassian\"+MyProjectName
#else
#define MyDistFolder AddBackslash(GetEnv(MyGitProjects)) + MyProjectName
#endif
#define MyReleaseFolder MyDistFolder + "\DisryRuSearcher\bin\x86\Release"

#define MyAppPublisher "Yuriy Astrov"
#define MyAppURL "https://github.com/yastrov/" + MyProjectName
#define MyAppUpdatedUrl MyAppURL+"/releases"
#define MyAppExeName "DiaryRuSearcher.exe" 
#define MyAppVersion GetFileVersion(AddBackslash(MyReleaseFolder)+MyAppExeName)

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{2DBE4CB7-C4CF-449E-A74B-9DD0B192688E}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf32}\{#MyAppName}
DefaultGroupName={#MyAppName}

;Config Installator
;LicenseFile={#MyDistFolder}\LICENSE.md
VersionInfoVersion=0.0.0.0
VersionInfoDescription=Search utility for http://www.diary.ru
VersionInfoCopyright={#MyAppPublisher}
VersionInfoProductVersion={#MyAppVersion}

AllowNoIcons=yes
;SetupIconFile={#MyDistFolder}\images\icon.png
OutputBaseFilename={#MyAppName}_{#MyAppVersion}
;Win7, because .NET 4.5 needed
MinVersion=6.1.7600
Compression=lzma
SolidCompression=yes

;Previous installation
UsePreviousAppDir=yes 
UsePreviousGroup=yes
UsePreviousSetupType=yes
UsePreviousTasks=yes

;Uninstaller
Uninstallable=not IsTaskSelected('portablemode')
UninstallDisplayIcon={app}\{#MyAppExeName}
UninstallFilesDir={app}\uninst

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "ru"; MessagesFile: "compiler:Languages\Russian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1
Name: "portablemode"; Description: "{cm:PortableMode}"; Flags: unchecked
Name: "writeinstalledpath"; Description: "{cm:WriteInstalledPath}"; Flags: unchecked

[Registry]
Root: HKLM; Subkey: Software\YAstrov\{#MyAppName}; ValueType: string; ValueName: InstallPath; ValueData: {app}; Tasks: writeinstalledpath

[Files]
Source: "{#MyReleaseFolder}\DiaryRuSearcher.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyReleaseFolder}\DiaryAPIClient.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyReleaseFolder}\EntityFramework.dll"; DestDir: "{app}";
Source: "{#MyReleaseFolder}\EntityFramework.SqlServer.dll"; DestDir: "{app}";
Source: "{#MyReleaseFolder}\Newtonsoft.Json.dll"; DestDir: "{app}";
Source: "{#MyReleaseFolder}\sqlite3.dll"; DestDir: "{app}";
Source: "{#MyReleaseFolder}\System.Data.SQLite.dll"; DestDir: "{app}";
Source: "{#MyReleaseFolder}\System.Data.SQLite.EF6.dll"; DestDir: "{app}";
Source: "{#MyReleaseFolder}\System.Data.SQLite.Linq.dll"; DestDir: "{app}";
Source: "{#MyReleaseFolder}\x86\SQLite.Interop.dll"; DestDir: "{app}\x86";
Source: "dotNetFx40_Full_setup.exe"; DestDir: {tmp}; Flags: deleteafterinstall; Check: not IsRequiredDotNetDetected
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
Filename: {tmp}\dotNetFx40_Full_setup.exe; Parameters: "/q:a /c:""install /l /q"""; Check: not IsRequiredDotNetDetected; StatusMsg: {cm:FrameworkInstalled};

[CustomMessages]
en.PortableMode=Portable mode
ru.PortableMode=Портативная установка
en.WriteInstalledPath= Write path to registry
ru.WriteInstalledPath=Записать путь установки в ресстр системы
en.RequiresM=requires
ru.RequiresM=нуждается в
en.InstAttemptM=The installer will attempt to install it
ru.InstAttemptM=Программа установки попытается установить
en.FrameworkInstalled=Microsoft Framework 4.5 is beïng installed. Please wait...
ru.FrameworkInstalled=Microsoft Framework 4.5 Устанавливается. Пожалуйста, подождите...
en.DeleteSettingsM=Remove settings and local database?
ru.DeleteSettingsM=Удалить настройки и файл базы данных?

[UninstallDelete]
Type: files; Name: "{%username}\DiarySearchDB.db"; Check: DoesDeleteSettings
Type: filesandordirs; Name: "{%localappdata}\{#MyAppName}"; Check: DoesDeleteSettings
Type: filesandordirs; Name: "{app}";

[LangOptions]
DialogFontSize=12
WelcomeFontSize=16

[Code]
var
  deleteCHBox : Boolean;
// http://www.codeproject.com/Tips/506096/InnoSetup-with-NET-installer-x-x-sample
function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1.4322'     .NET Framework 1.1
//    'v2.0.50727'    .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//    'v4.5'          .NET Framework 4.5
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
    key: string;
    install, release, serviceCount: cardinal;
    check45, success: boolean;
var reqNetVer : string;
begin
    // .NET 4.5 installs as update to .NET 4.0 Full
    if version = 'v4.5' then begin
        version := 'v4\Full';
        check45 := true;
    end else
        check45 := false;

    // installation key group for all .NET versions
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + version;

    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;

    // .NET 4.0/4.5 uses value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;

    // .NET 4.5 uses additional value Release
    if check45 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Release', release);
        success := success and (release >= 378389);
    end;
    
    result := success and (install = 1) and (serviceCount >= service);
end;

function IsRequiredDotNetDetected(): Boolean;  
begin
    result := IsDotNetDetected('v4\Full', 0);
end;

function InitializeSetup(): Boolean;
begin
    if not IsDotNetDetected('v4\Full', 0) then begin
        MsgBox('{#MyAppName} {cm:RequiresM} Microsoft .NET Framework 4.5.'#13#13
          '{cm:InstAttemptM}', mbInformation, MB_OK);        
    end;
    result := true;
end;

function DoesDeleteSettings(): Boolean;
begin
  Result:= deleteCHBox;
end;

procedure TakeAnswerToDelete;
var
  answer : Integer;
  myS : String;
begin
  deleteCHBox:=false;
  myS := ExpandConstant('{cm:DeleteSettingsM}');
  answer := MsgBox(myS, mbInformation, MB_YESNO);
  if answer = IDYES then
    deleteCHBox:=true;
end;

function InitializeUninstall(): Boolean;
begin
    TakeAnswerToDelete;
    Result:=true;
end;