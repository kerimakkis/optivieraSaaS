# Custom NSIS script for Optiviera ERP
# Handles previous installations and running processes with detailed logging

!macro customInit
  # Create log file
  FileOpen $0 "$TEMP\OptivieraInstall.log" w
  FileWrite $0 "=== Optiviera ERP Installation Log ===$\r$\n"
  FileWrite $0 "Installation started at: $\r$\n"
  System::Call 'kernel32::GetSystemTime(*i.r0)'
  FileWrite $0 "System time: $0$\r$\n"
  FileClose $0
  
  # Log current directory and system info
  FileOpen $0 "$TEMP\OptivieraInstall.log" a
  FileWrite $0 "Installation directory: $INSTDIR$\r$\n"
  FileWrite $0 "Temp directory: $TEMP$\r$\n"
  FileWrite $0 "Windows version: $\r$\n"
  FileClose $0
  
  # Check for existing installations
  FileOpen $0 "$TEMP\OptivieraInstall.log" a
  FileWrite $0 "Checking for existing installations...$\r$\n"
  FileClose $0
  
  # Check Program Files
  IfFileExists "$PROGRAMFILES\OptvieraERP" 0 +3
    FileOpen $0 "$TEMP\OptivieraInstall.log" a
    FileWrite $0 "Found existing installation in: $PROGRAMFILES\OptvieraERP$\r$\n"
    FileClose $0
  
  # Check AppData Local
  IfFileExists "$LOCALAPPDATA\Programs\OptivieraERP" 0 +3
    FileOpen $0 "$TEMP\OptivieraInstall.log" a
    FileWrite $0 "Found existing installation in: $LOCALAPPDATA\Programs\OptivieraERP$\r$\n"
    FileClose $0
  
  # Kill any running Optiviera processes
  FileOpen $0 "$TEMP\OptivieraInstall.log" a
  FileWrite $0 "Attempting to kill running processes...$\r$\n"
  FileClose $0
  
  nsExec::ExecToLog 'taskkill /F /IM "OptivieraERP.exe" /T'
  Pop $0
  FileOpen $1 "$TEMP\OptivieraInstall.log" a
  FileWrite $1 "taskkill OptivieraERP.exe result: $0$\r$\n"
  FileClose $1
  
  nsExec::ExecToLog 'taskkill /F /IM "Optiviera.exe" /T'
  Pop $0
  FileOpen $1 "$TEMP\OptivieraInstall.log" a
  FileWrite $1 "taskkill Optiviera.exe result: $0$\r$\n"
  FileClose $1
  
  nsExec::ExecToLog 'taskkill /F /IM "electron.exe" /T'
  Pop $0
  FileOpen $1 "$TEMP\OptivieraInstall.log" a
  FileWrite $1 "taskkill electron.exe result: $0$\r$\n"
  FileClose $1
  
  # Wait a bit for processes to terminate
  Sleep 2000
  
  FileOpen $0 "$TEMP\OptivieraInstall.log" a
  FileWrite $0 "Process termination completed$\r$\n"
  FileClose $0
!macroend

!macro customInstall
  FileOpen $0 "$TEMP\OptivieraInstall.log" a
  FileWrite $0 "Starting custom installation...$\r$\n"
  FileClose $0
  
  # Clean up old installations with detailed logging
  FileOpen $0 "$TEMP\OptivieraInstall.log" a
  FileWrite $0 "Cleaning up old installation directory: $INSTDIR\resources\app$\r$\n"
  FileClose $0
  
  RMDir /r "$INSTDIR\resources\app"
  Pop $0
  FileOpen $1 "$TEMP\OptivieraInstall.log" a
  FileWrite $1 "RMDir result: $0$\r$\n"
  FileClose $1
  
  CreateDirectory "$INSTDIR\resources\app"
  Pop $0
  FileOpen $1 "$TEMP\OptivieraInstall.log" a
  FileWrite $1 "CreateDirectory result: $0$\r$\n"
  FileClose $1
  
  # Check if directory was created successfully
  IfFileExists "$INSTDIR\resources\app" 0 +3
    FileOpen $0 "$TEMP\OptivieraInstall.log" a
    FileWrite $0 "Directory created successfully$\r$\n"
    FileClose $0
  Goto +3
    FileOpen $0 "$TEMP\OptivieraInstall.log" a
    FileWrite $0 "ERROR: Failed to create directory$\r$\n"
    FileClose $0
  
  # Log that we're checking for ASP.NET Core files
  FileOpen $0 "$TEMP\OptivieraInstall.log" a
  FileWrite $0 "Checking for ASP.NET Core files in installer...$\r$\n"
  FileClose $0
  
  FileOpen $0 "$TEMP\OptivieraInstall.log" a
  FileWrite $0 "Custom installation completed$\r$\n"
  FileClose $0
!macroend

!macro customUnInstall
  FileOpen $0 "$TEMP\OptivieraUninstall.log" w
  FileWrite $0 "=== Optiviera ERP Uninstallation Log ===$\r$\n"
  FileClose $0
  
  # Kill processes before uninstall
  FileOpen $0 "$TEMP\OptivieraUninstall.log" a
  FileWrite $0 "Killing processes before uninstall...$\r$\n"
  FileClose $0
  
  nsExec::ExecToLog 'taskkill /F /IM "OptivieraERP.exe" /T'
  Pop $0
  FileOpen $1 "$TEMP\OptivieraUninstall.log" a
  FileWrite $1 "taskkill OptivieraERP.exe result: $0$\r$\n"
  FileClose $1
  
  nsExec::ExecToLog 'taskkill /F /IM "Optiviera.exe" /T'
  Pop $0
  FileOpen $1 "$TEMP\OptivieraUninstall.log" a
  FileWrite $1 "taskkill Optiviera.exe result: $0$\r$\n"
  FileClose $1
  
  Sleep 2000
  
  FileOpen $0 "$TEMP\OptivieraUninstall.log" a
  FileWrite $0 "Uninstall process termination completed$\r$\n"
  FileClose $0
!macroend
