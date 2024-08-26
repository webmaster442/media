$answer = Read-Host "Do you want to install media tools to your powershell profile? (yes/no or y/n)"
$answer = $answer.ToLower()
if ($answer -eq 'yes' -or $answer -eq 'y') {
    ./media.exe update all
    ./media.exe completion powershell --install | Out-String | Invoke-Expression
}
elseif ($answer -eq 'no' -or $answer -eq 'n') 
{
    Write-Host "Exiting without installation"
} 
else 
{
    Write-Host "Invalid input. Please enter yes/no or y/n."
}
