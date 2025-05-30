. "$PSScriptRoot\LoggingFunctions.ps1"
function GenerateDeliveryFilename {
    param (
        [string] $prefix,
        [string] $suffix,
        [string] $questionnaireName,
        [datetime] $dateTime = (Get-Date),
        [string] $fileExt
    )

    If ([string]::IsNullOrEmpty($prefix)) {
        throw "No prefix provided"
    }

    If ([string]::IsNullOrEmpty($questionnaireName)) {
        throw "No questionnaire name argument provided"
    }

    If ([string]::IsNullOrEmpty($fileExt)) {
        throw "No file extension argument provided"
    }

    return "$($prefix)_$($questionnaireName)_$($dateTime.ToString("ddMMyyyy"))_$($dateTime.ToString("HHmmss"))$($suffix).$fileExt"
}

function GenerateBatchFileName {
    param (
        [datetime] $dateTime = (Get-Date),
        [string] $surveyType
    )
    If ([string]::IsNullOrEmpty($surveyType)) {
        throw "No Survey Type has been provided"
    }

    return "$($surveyType)_$($dateTime.ToString("ddMMyyyy"))_$($dateTime.ToString("HHmmss"))"
}

function ExtractZipFile {
    [CmdletBinding()]
    param (
        [string] $pathTo7zip,
        [string] $zipFilePath,
        [string] $destinationPath
    )

    if (-not (Test-Path $zipFilePath)) {
        LogWarning "Zip file not found: '$zipFilePath'. Cannot extract."
        return $false
    }

    $exePath = Join-Path -Path $pathTo7zip -ChildPath "7za.exe"
    if (-not (Test-Path $exePath)) {
        LogWarning "7-Zip executable not found at '$exePath'."
        return $false
    }

    $arguments = @(
        "x", # Extract with full paths
        "`"$zipFilePath`"", # Source ZIP file, quoted
        "-o`"$destinationPath`"", # Output directory, quoted, no space after -o
        "-y"  # Assume Yes to all queries (e.g., overwrite files)
    )

    LogInfo "Attempting to extract zip file '$zipFilePath' to path '$destinationPath'..."
    try {
        $process = Start-Process -FilePath $exePath -ArgumentList $arguments -Wait -PassThru -NoNewWindow
        if ($process.ExitCode -eq 0) {
            LogInfo "Successfully extracted zip file '$zipFilePath' to '$destinationPath'."
            return $true
        }
        else {
            LogWarning "7-Zip failed to extract '$zipFilePath'. Exit code: $($process.ExitCode)."
            return $false
        }
    }
    catch {
        LogWarning "An error occurred while trying to run 7-Zip for extraction of '$zipFilePath': $($_.Exception.Message)"
        return $false
    }
}

function AddFilesToZip {
    [CmdletBinding()]
    param (
        [string] $pathTo7zip,
        [string[]] $files,
        [string] $zipFilePath
    )

    if ($null -eq $files -or $files.Count -eq 0) {
        LogWarning "No files provided to add to zip '$zipFilePath'."
        return $false 
    }

    $exePath = Join-Path -Path $pathTo7zip -ChildPath "7za.exe"
    if (-not (Test-Path $exePath)) {
        LogWarning "7-Zip executable not found at '$exePath'."
        return $false
    }

    $arguments = @(
        "a", # Add to archive
        "-tzip", # Specify ZIP archive type
        "`"$zipFilePath`"" # Output ZIP file path, quoted
    ) + $files.ForEach({ "`"$_`"" }) # Add each file, quoted

    LogInfo "Attempting to add files to '$zipFilePath'. Files: $($files -join ', ')"
    try {
        $process = Start-Process -FilePath $exePath -ArgumentList $arguments -Wait -PassThru -NoNewWindow
        if ($process.ExitCode -eq 0) {
            LogInfo "Successfully added file(s) to '$zipFilePath'."
            return $true
        }
        else {
            LogWarning "7-Zip failed to add files to '$zipFilePath'. Exit code: $($process.ExitCode). Files: $($files -join ', ')"
            return $false
        }
    }
    catch {
        LogWarning "An error occurred while trying to run 7-Zip to add files to '$zipFilePath': $($_.Exception.Message)"
        return $false
    }
}

function AddFolderToZip {
    [CmdletBinding()]
    param (
        [string] $pathTo7zip,
        [string] $folder,
        [string] $zipFilePath
    )

    if (-not (Test-Path $folder -PathType Container)) {
        LogWarning "Folder to zip not found or is not a directory: '$folder'."
        return $false
    }

    $exePath = Join-Path -Path $pathTo7zip -ChildPath "7za.exe"
    if (-not (Test-Path $exePath)) {
        LogWarning "7-Zip executable not found at '$exePath'."
        return $false
    }

    $sourcePathFor7zip = Join-Path -Path $folder -ChildPath "*" 

    $arguments = @(
        "a",
        "-tzip",
        "`"$zipFilePath`"",
        "`"$sourcePathFor7zip`""
    )

    LogInfo "Attempting to add contents of folder '$folder' to '$zipFilePath'."
    try {
        $process = Start-Process -FilePath $exePath -ArgumentList $arguments -Wait -PassThru -NoNewWindow
        if ($process.ExitCode -eq 0) {
            LogInfo "Successfully added folder contents of '$folder' to '$zipFilePath'."
            return $true
        }
        else {
            LogWarning "7-Zip failed to add folder '$folder' to '$zipFilePath'. Exit code: $($process.ExitCode)."
            return $false
        }
    }
    catch {
        LogWarning "An error occurred while trying to run 7-Zip to add folder '$folder' to '$zipFilePath': $($_.Exception.Message)"
        return $false
    }
}

function CreateANewFolder {
    param (
        [string] $folderPath,
        [string] $folderName
    )
    If ([string]::IsNullOrEmpty($folderPath)) {
        throw "No Path to the new folder provided"
    }
    If ([string]::IsNullOrEmpty($folderName)) {
        throw "No folder name provided"
    }

    if (-not (Test-Path $folderPath\$folderName)) {
        Write-Host "creating new folder: $folderName in $folderPath"
        New-Item -Path $folderPath -Name $folderName -ItemType "directory" | Out-Null
    }

    return "$folderPath\$folderName"
}

function GetFolderNameFromAPath {
    param (
        [string] $folderPath
    )
    If ([string]::IsNullOrEmpty($folderPath)) {
        throw "No Path to the new folder provided"
    }
    return Split-Path $processingFolder -Leaf
}

function ConvertJsonFileToObject {
    param (
        [string] $jsonFile
    )  

    return Get-Content -Path $jsonFile | ConvertFrom-Json
}
