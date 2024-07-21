./ffcmd.exe update all
./ffcmd.exe completion powershell --install | Out-String | Invoke-Expression
Remove-Item firststart.ps1
