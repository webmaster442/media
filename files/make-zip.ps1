$sourceFolder = "portable_config"
$destinationZip = "..\src\Embedded\files.zip"

# Ensure the source folder exists
if (-Not (Test-Path $sourceFolder)) {
    Write-Host "Source folder '$sourceFolder' does not exist. Exiting."
    exit 1
}

# Remove the existing zip file if it exists
if (Test-Path $destinationZip) {
    Remove-Item $destinationZip -Force
}

# Create the zip file
Compress-Archive -Path $sourceFolder -DestinationPath $destinationZip -Force

Write-Host "Compression complete: '$destinationZip'"