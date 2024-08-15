./media.exe update all
./media.exe completion powershell --install | Out-String | Invoke-Expression
Remove-Item firststart.ps1
