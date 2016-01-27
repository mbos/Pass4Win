#
# Pass4Win NSIS script
# 

!define src_dir "."

!define APPNAME "Pass4Win"
!define DESCRIPTION "Windows version of Pass (http://www.passwordstore.org/) in the sense that the store (password structure) is and should be exactly the same between the two programs."


# These will be displayed by the "Click here for support information" link in "Add/Remove Programs"
# It is possible to use "mailto:" links in here to open the email client
!define HELPURL "https://github.com/mbos/Pass4Win" # "Support Information" link
!define UPDATEURL "https://github.com/mbos/Pass4Win" # "Product Updates" link
!define ABOUTURL "https://github.com/mbos/Pass4Win" # "Publisher" link
# This is the size (in kB) of all the files copied into "Program Files"
!define INSTALLSIZE 8091

#SilentInstall silent
RequestExecutionLevel admin ;Require admin rights on NT6+ (When UAC is turned on)
 
InstallDir "$PROGRAMFILES\${APPNAME}"
 
# This will be in the installer/uninstaller's title bar
Name "${APPNAME} version $%APPVEYOR_BUILD_VERSION%"
Icon "Pass4Win/icon/lock.ico"
outFile "Pass4Win-Setup-v$%APPVEYOR_BUILD_VERSION%-%PLATFORM%.exe"
 
!include LogicLib.nsh
 
# Just three pages - license agreement, install location, and installation
page directory
Page instfiles
 
!macro VerifyUserIsAdmin
UserInfo::GetAccountType
pop $0
${If} $0 != "admin" ;Require admin rights on NT4+
        messageBox mb_iconstop "Administrator rights required!"
        setErrorLevel 740 ;ERROR_ELEVATION_REQUIRED
        quit
${EndIf}
!macroend
 
 Function .onInit
  setShellVarContext all
  !insertmacro VerifyUserIsAdmin
  ReadRegStr $R0 HKLM \
  "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" \
  "UninstallString"
  StrCmp $R0 "" done
  ClearErrors
  HideWindow
  ClearErrors
  ExecWait '$R0 _?=$INSTDIR /S'
  BringToFront

  IfErrors no_remove_uninstaller done
  no_remove_uninstaller:
 
done:
 
FunctionEnd


section "install"
  # Files for the install directory - to build the installer, these should be in the same directory as the install script (this file)
  setOutPath $INSTDIR
  # Files added here should be removed by the uninstaller (see section "uninstall")
  file "${src_dir}\Pass4Win\bin\Release\Pass4Win.exe"
  file "${src_dir}\Pass4Win\bin\Release\Pass4Win.exe.config"

  SetOutPath "$INSTDIR\NativeBinaries\x86"
  file "${src_dir}\Pass4Win\bin\Release\NativeBinaries\x86\git2-e0902fb.dll"

  SetOutPath "$INSTDIR\NativeBinaries\amd64"
  file "${src_dir}\Pass4Win\bin\Release\NativeBinaries\amd64\git2-e0902fb.dll"  

  # Localezation
  SetOutPath "$INSTDIR\nl"
  file "${src_dir}\Pass4Win\bin\Release\nl\Pass4Win.resources.dll"
  SetOutPath "$INSTDIR\it"
  file "${src_dir}\Pass4Win\bin\Release\it\Pass4Win.resources.dll"
  SetOutPath "$INSTDIR\de"
  file "${src_dir}\Pass4Win\bin\Release\de\Pass4Win.resources.dll"
  SetOutPath "$INSTDIR\fr"
  file "${src_dir}\Pass4Win\bin\Release\fr\Pass4Win.resources.dll" 
 
  # Uninstaller - See function un.onInit and section "uninstall" for configuration
  writeUninstaller "$INSTDIR\uninstall.exe"
 
  # Start Menu
  createDirectory "$SMPROGRAMS\${APPNAME}"
  createShortCut "$SMPROGRAMS\${APPNAME}\${APPNAME}.lnk" "$INSTDIR\Pass4Win.exe" "" "$INSTDIR\Pass4Win.exe"

  # Desktop
  CreateShortCut "$DESKTOP\${APPNAME}.lnk" "$INSTDIR\Pass4Win.exe" "" "$INSTDIR\Pass4Win.exe"
 
  # Registry information for add/remove programs
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayName" "${APPNAME} - ${DESCRIPTION}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "QuietUninstallString" "$\"$INSTDIR\uninstall.exe$\" /S"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "InstallLocation" "$\"$INSTDIR$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayIcon" "$\"$INSTDIR\Pass4Win.exe$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "Publisher" "$\"Mike Bos$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "HelpLink" "$\"${HELPURL}$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "URLUpdateInfo" "$\"${UPDATEURL}$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "URLInfoAbout" "$\"${ABOUTURL}$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayVersion" "$\"$%APPVEYOR_BUILD_VERSION%$\""

  # There is no option for modifying or repairing the install
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoRepair" 1
  # Set the INSTALLSIZE constant (!defined at the top of this script) so Add/Remove Programs can accurately report the size
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "EstimatedSize" ${INSTALLSIZE}
sectionEnd
 
# Uninstaller
 
function un.onInit
  SetShellVarContext all
  #Verify the uninstaller - last chance to back out
  !insertmacro VerifyUserIsAdmin
functionEnd
 
section "uninstall"
 
  # Remove Start Menu launcher
  delete "$SMPROGRAMS\${APPNAME}\${APPNAME}.lnk"
  # Try to remove the Start Menu folder - this will only happen if it is empty
  rmDir "$SMPROGRAMS\${APPNAME}"
  # Remove desktop shortcut
  Delete "$DESKTOP\${APPNAME}.lnk"

 
  # Remove files
  delete $INSTDIR\Pass4Win.exe
 
  delete $INSTDIR\NativeBinaries\x86\git2-e0902fb.dll
  rmdir $INSTDIR\NativeBinaries\x86

  Delete $INSTDIR\NativeBinaries\amd64\git2-e0902fb.dll
  RMDir $INSTDIR\NativeBinaries\amd64

  RMDir $INSTDIR\NativeBinaries

  Delete $INSTDIR\nl\Pass4Win.resources.dll
  RMDir $INSTDIR\nl
  Delete $INSTDIR\it\Pass4Win.resources.dll
  RMDir $INSTDIR\it
  Delete $INSTDIR\de\Pass4Win.resources.dll
  RMDir $INSTDIR\de
  Delete $INSTDIR\fr\Pass4Win.resources.dll
  RMDir $INSTDIR\fr

  # Always delete uninstaller as the last action
  delete $INSTDIR\uninstall.exe
 
  # Try to remove the install directory - this will only happen if it is empty
  rmDir $INSTDIR
 
  # Remove uninstaller information from the registry
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
sectionEnd

