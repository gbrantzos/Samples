# ------------------------------------------------------------------------------
# Script parameters
param(
    [string] $version,
    [bool]   $publish = $true
)
# ------------------------------------------------------------------------------


# ------------------------------------------------------------------------------
# Variables

# Location 
$originalPath = Get-Location
$workingPath  = ($PSScriptRoot)
$rootPath     = (Get-Item $workingPath).Parent

# Project
$project      = "SEnRestAPI"

# Build info
$gitHash      = (git rev-parse HEAD).Substring(0, 10)
$gitBranch    = (git rev-parse --abbrev-ref HEAD)
$timeStamp    = Get-Date -Format 'yyyyMMddHHmm'
$versionFull  = "1.0.0-beta1"
$banner       = ""

# Folders
$outputFolder  = Join-Path -Path $rootPath -ChildPath "tmp/$project"
$packageFolder = Join-Path -Path $rootPath -ChildPath "dist"

# ------------------------------------------------------------------------------


# ------------------------------------------------------------------------------
# Methods

function Prepare-Version {
    $script:version="1.3.0"
    $script:versionFull="$version-rc1"
}    

# Initialize folder if missing
function Initialize-Folder {
    param ([string] $folderName)
    if (!(test-path $folderName)) {
        New-Item -ItemType Directory -Force -Path $folderName | Out-Null
    }
}

# Clear temp folder
function Clear-Folder {
    param ([string] $path)
    if (Test-Path $path) {
        Remove-Item $path -Recurse
    }
}

# Initialize banner variable
function Initialize-Banner {
    $script:banner = "
   ______      __                 ______              
  / ____/_  __/ /_  ___  _____   / ____/___  ________ 
 / /   / / / / __ \/ _ \/ ___/  / /   / __ \/ ___/ _ \
/ /___/ /_/ / /_/ /  __(__  )  / /___/ /_/ / /  /  __/
\____/\__,_/_.___/\___/____/   \____/\____/_/   \___/ 


Commit         : $gitHash
Branch         : $gitBranch "
}

# Display banner
function Display-Banner {
    
    Write-Host $banner 
    Write-Host "Output folder  : $outputFolder"
    Write-Host "`n`n"
}

# Build
function Build-Solution {
    # Get MSBuild path
    # TODO Maybe add check that path exists
    $msBuild = (Resolve-Path "C:\Program Files (x86)\MSBuild\*\Bin\MSBuild.exe")[0].Path
    & $msBuild "ConsoleApp1\ConsoleApp1.csproj" `
        -t:Rebuild `
        -p:Configuration=Release `
        -p:Version=$version `
        -p:versionFull=$versionFull `
        -p:OutDir=$outputFolder -v:n
}

# Build information file
function Write-BuildInformation {
    param ([string] $path)
    $buildInfo = "$banner`n`n`nBuild at $(Get-Date)"
    
    $filePath = Join-Path $outputFolder -ChildPath "BuildInformation.txt"
    $buildInfo | Out-File -Path $filePath
}

# Create ZIP file
function Create-Package {
    if (!(test-path $packageFolder)) {
        New-Item -ItemType Directory -Force -Path $packageFolder | Out-Null
    }
    
    Compress-Archive `
        -Path "$outputFolder\*" `
        -Destination "$packageFolder\$project-v$versionFull.zip" `
        -Force
}

# Deploy package
function Deploy-Package {

}
# ------------------------------------------------------------------------------



# If version not defined, get from Git
Prepare-Version

# Banner, info
Initialize-Banner
Display-Banner

# Prepare new output folder
Initialize-Folder $outputFolder

# Build using version as parameter
Build-Solution

# Add BuildInfo on output folder
Write-BuildInformation

# Create archive
Create-Package

# If asked, Deploy somewhere
Deploy-Package

# Cleanup
Clear-Folder $outputFolder
