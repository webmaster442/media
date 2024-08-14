param (
    [Parameter(Mandatory=$true)]
    [ValidateScript({Test-Path $_ -PathType Container})]
    [string]$Source,

    [Parameter(Mandatory=$true)]
    [ValidateScript({Test-Path $_ -PathType Container})]
    [string]$Destination
)

function Move-Recursive {
    param (
        [string]$SourcePath,
        [string]$DestinationPath
    )
    
    # Ensure the source directory exists
    if (-not (Test-Path -Path $SourcePath)) {
        Write-Error "Source path '$SourcePath' does not exist."
        return
    }

    # Ensure the destination directory exists, if not, create it
    if (-not (Test-Path -Path $DestinationPath)) {
        New-Item -Path $DestinationPath -ItemType Directory -Force | Out-Null
    }

    # Get all items recursively and move them while preserving the directory structure
    Get-ChildItem -Path $SourcePath -Recurse | ForEach-Object {
        $destination = $_.FullName.Replace($SourcePath, $DestinationPath)
        $destinationDir = [System.IO.Path]::GetDirectoryName($destination)

        # Create destination directory if it doesn't exist
        if (-not (Test-Path -Path $destinationDir)) {
            New-Item -Path $destinationDir -ItemType Directory -Force | Out-Null
        }

        # Move the item
        Move-Item -Path $_.FullName -Destination $destination -Force
    }
}

function Wait-ForKeyPress {
    param (
        [string]$Message = "Press any key to continue..."
    )

    Write-Host $Message
    # Wait for any key press
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}

function Delete-Source {
    #delete source folder
    Wait-ForKeyPress -Messag "Update completed. Press any key to exit..."
    cd $destination
    Remove-Item -Path $Source -Recurse
}

function Self-Delete {
    $scriptPath = $MyInvocation.MyCommand.Path

    # Schedule deletion after the script exits
    Start-Sleep -Seconds 1
    Remove-Item -Path $scriptPath -Force
}

Write-Host "Updating to latest version..."

Move-Recursive -SourcePath $Source -DestinationPath $Destination

Delete-Source
Self-Delete
