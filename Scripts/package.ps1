# ------------------------------------------------------------------------------
# Script parameters
param(
    [string] $version,
    [switch] $deploy,
    [switch] $allowDirty
)
# ------------------------------------------------------------------------------


# ------------------------------------------------------------------------------
# Exit codes

#   0   OK
#   1   No Git TAG on repository
#   2   Invalid version format
#   3   Repository has pending changes

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
$versionInfo  = "1.0.0-beta1"
$banner       = ""

# Folders
$outputFolder  = Join-Path -Path $rootPath -ChildPath "tmp/$project"
$packageFolder = Join-Path -Path $rootPath -ChildPath "dist"

# ------------------------------------------------------------------------------


# ------------------------------------------------------------------------------
# Methods

function Prepare-Version {
    $tmp    = ""
    $source = ""
    if (!$version) {
        $tag = (git describe --abbrev=0)
        if (!$tag) {
            Write-Host "No Git TAG defined!"
            exit 1
        }         
        $tmp=$tag
        $source="Git"
    } else {
        $tmp=$version
        $source="parameters"
    }
    
    # Sanitize version, remove extra v
    if ($tmp.StartsWith("v")) { $tmp=$tmp.SubString(1) }
    
    # Check version format
    $re=[regex]"([0-9]\.[0-9]\.[0-9])(\-){0,1}((.)*)"
    $m=$re.Match($tmp)
    if (!$m.Success) {
        Write-Host "Invalid version format! $tmp"
        exit 2
    }
    
    $script:version = $m.Groups[1].Value
    $info = $m.Groups[3].Value
    if ($info -eq "") {
        $script:versionInfo="$script:version"
    } else {
        
        $script:versionInfo="$script:version-$info"
    }
    
    Write-Host "Version $script:version, full version $script:versionInfo (from $source)..."
    Write-Host "Git commit $gitHash, branch $gitBranch"
}    

# Check for GIT pending changes
function Pending-Changes {
    if ($allowDirty) { return }
    
    $st = (git status -su)
    if (![string]::ISNullOrEmpty($st)) {
        Write-Host "`n`nGit repository has pending changes!`nAborting..."
        exit 3
    }
}
# Initialize folder if missing
function Initialize-Folder {
    param ([string] $folderName)
    if (!(Test-Path $folderName)) {
        New-Item -ItemType Directory -Force -Path $folderName | Out-Null
    }
    Write-Host "Output folder '$outputFolder'`n`n"
}

# Clear temp folder
function Clear-Folder {
    param ([string] $path)
    if (Test-Path $path) { Remove-Item $path -Recurse }
}

# Initialize banner variable
function Initialize-Banner {
    $script:banner = "
   _____ ______         _____           _              _____ _____ 
  / ____|  ____|       |  __ \         | |       /\   |  __ \_   _|
 | (___ | |__   _ __   | |__) |___  ___| |_     /  \  | |__) || |  
  \___ \|  __| | '_ \  |  _  // _ \/ __| __|   / /\ \ |  ___/ | |  
  ____) | |____| | | | | | \ \  __/\__ \ |_   / ____ \| |    _| |_ 
 |_____/|______|_| |_| |_|  \_\___||___/\__| /_/    \_\_|   |_____|


Packaging...
"
}

# Display banner
function Display-Banner { Write-Host $banner }

# Build
function Build-Solution {
    # Get MSBuild path
    # TODO Maybe add check that path exists
    $msBuild = (Resolve-Path "C:\Program Files (x86)\MSBuild\*\Bin\MSBuild.exe")[0].Path
    & $msBuild "ConsoleApp1\ConsoleApp1.csproj" `
        -t:Rebuild `
        -p:Configuration=Release `
        -p:Version=$version `
        -p:versionInfo=$versionInfo `
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
    if (!(Test-Path $packageFolder)) {
        New-Item -ItemType Directory -Force -Path $packageFolder | Out-Null
    }
    
    Compress-Archive `
        -Path "$outputFolder\*" `
        -Destination "$packageFolder\$project-v$versionInfo.zip" `
        -Force
}

# Deploy package
function Deploy-Package {
    if ($deploy) {
        Write-Host Deploying package...
    }
}
# ------------------------------------------------------------------------------



# Banner, info
Initialize-Banner
Display-Banner

# If version not defined, get from Git
Prepare-Version

# Check for pending changes
Pending-Changes

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
