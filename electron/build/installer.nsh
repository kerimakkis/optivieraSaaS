# Custom NSIS script for Optiviera ERP
# Handles previous installations and running processes

!macro customInit
  # Kill any running Optiviera processes
  nsExec::ExecToLog 'taskkill /F /IM "Optiviera ERP.exe" /T'
  nsExec::ExecToLog 'taskkill /F /IM "Optiviera.exe" /T'
  Pop $0
!macroend

!macro customInstall
  # Clean up old installations
  RMDir /r "$INSTDIR\resources\app"
  CreateDirectory "$INSTDIR\resources\app"
!macroend

!macro customUnInstall
  # Kill processes before uninstall
  nsExec::ExecToLog 'taskkill /F /IM "Optiviera ERP.exe" /T'
  nsExec::ExecToLog 'taskkill /F /IM "Optiviera.exe" /T'
  Pop $0
  Sleep 2000
!macroend
