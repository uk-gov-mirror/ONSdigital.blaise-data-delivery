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
        throw "Zip file to extract not found: '$zipFilePath'"
    }
    if (-not (Test-Path $destinationPath -PathType Container)) {
        LogInfo("Destination path '$destinationPath' does not exist or is not a directory. Creating it.")
        New-Item -Path $destinationPath -ItemType Directory -Force -ErrorAction Stop | Out-Null
    }

    $exe = Join-Path -Path $pathTo7zip -ChildPath "7za.exe"
    if (-not (Test-Path $exe)) {
        throw "7-Zip executable not found at '$exe'"
    }

    LogInfo("Attempting to extract zip file '$zipFilePath' to path '$destinationPath' using '$exe'...")
    & $exe x "`"$zipFilePath`"" -o"`"$destinationPath`"" -y
    if ($LASTEXITCODE -ne 0) {
        throw "7-Zip failed to extract '$zipFilePath'. Exit code: $LASTEXITCODE."
    }
    LogInfo("Successfully extracted zip file '$zipFilePath' to path '$destinationPath'")
}

function AddFilesToZip {
    [CmdletBinding()]
    param (
        [string] $pathTo7zip,
        [string[]] $files,
        [string] $zipFilePath,
        [string] $tempPath
    )

    if ($null -eq $files -or $files.Count -eq 0) {
        throw "No files provided to add to zip '$zipFilePath'."
    }
    if ([string]::IsNullOrWhiteSpace($tempPath)) {
        throw "tempPath parameter is required for AddFilesToZip workaround. Please provide a valid temporary path."
    }

    $exe = Join-Path -Path $pathTo7zip -ChildPath "7za.exe"
    if (-not (Test-Path $exe)) {
        throw "7-Zip executable not found at '$exe'"
    }

    $tempWorkingDir = Join-Path -Path $tempPath -ChildPath "7zip_add_temp_$(Get-Random)"
    LogInfo("Using temporary working directory for zip update workaround: $tempWorkingDir")

    try {
        New-Item -Path $tempWorkingDir -ItemType Directory -ErrorAction Stop | Out-Null

        if (Test-Path $zipFilePath) {
            LogInfo("Extracting existing zip '$zipFilePath' to temporary directory '$tempWorkingDir'...")
            & $exe x "`"$zipFilePath`"" -o"`"$tempWorkingDir`"" -y
            if ($LASTEXITCODE -ne 0) {
                throw "Failed to extract existing zip '$zipFilePath' (Exit code: $LASTEXITCODE) for update workaround."
            }
            LogInfo("Successfully extracted existing zip.")
        } else {
            LogInfo("Zip file '$zipFilePath' does not exist. A new one will be created.")
        }

        LogInfo("Copying new files/patterns to temporary directory '$tempWorkingDir'...")
        foreach ($fileOrPattern in $files) {
            $itemsToCopy = Get-Item -Path $fileOrPattern -ErrorAction SilentlyContinue
            if ($null -ne $itemsToCopy) {
                foreach ($item in $itemsToCopy) {
                    Copy-Item -Path $item.FullName -Destination $tempWorkingDir -Force -ErrorAction Stop
                    LogInfo("Copied '$($item.FullName)' to '$tempWorkingDir'.")
                }
            } else {
                LogInfo("No items found matching '$fileOrPattern'. Skipping.")
            }
        }
        LogInfo("Successfully copied new files/patterns to temporary directory.")

        LogInfo("Creating new zip archive '$zipFilePath' from temporary directory '$tempWorkingDir'...")
        if (Test-Path $zipFilePath) {
            Remove-Item -Path $zipFilePath -Force -ErrorAction SilentlyContinue
            LogInfo("Removed old zip file '$zipFilePath' before recreation.")
        }

        & $exe a "`"$zipFilePath`"" "`"$tempWorkingDir\*`"" -y
        if ($LASTEXITCODE -ne 0) {
            throw "Failed to create new zip archive '$zipFilePath' from temporary directory '$tempWorkingDir'. Exit code: $LASTEXITCODE."
        }
        LogInfo("Successfully created new zip archive '$zipFilePath' from contents of '$tempWorkingDir'.")
    }
    catch {
        throw "An error occurred during the zip update workaround for '$zipFilePath': $($_.Exception.Message)"
    }
    finally {
        if (Test-Path $tempWorkingDir) {
            LogInfo("Cleaning up temporary working directory '$tempWorkingDir'.")
            Remove-Item -Path $tempWorkingDir -Recurse -Force -ErrorAction SilentlyContinue
        }
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
        throw "Source folder to add not found or is not a directory: '$folder'"
    }

    $exe = Join-Path -Path $pathTo7zip -ChildPath "7za.exe"
    if (-not (Test-Path $exe)) {
        throw "7-Zip executable not found at '$exe'"
    }

    # 7za a archive.zip C:\source_folder  -> adds 'source_folder' and its content into archive.zip
    LogInfo("Attempting to add folder '$folder' to zip file '$zipFilePath' using '$exe'...")
    & $exe a "`"$zipFilePath`"" "`"$folder`"" -y # Add -y for non-interactive
    if ($LASTEXITCODE -ne 0) {
        throw "7-Zip failed to add folder '$folder' to '$zipFilePath'. Exit code: $LASTEXITCODE."
    }
    LogInfo("Successfully added the folder '$folder' to the zip file '$zipFilePath'")
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

    $fullFolderPath = Join-Path -Path $folderPath -ChildPath $folderName
    if (-not (Test-Path $fullFolderPath)) {
        Write-Host "creating new folder: $folderName in $folderPath"
        New-Item -Path $fullFolderPath -ItemType "directory" -ErrorAction Stop | Out-Null
    }

    return $fullFolderPath
}

function GetFolderNameFromAPath {
    param (
        [string] $folderPath
    )
    If ([string]::IsNullOrEmpty($folderPath)) {
        throw "No folder path provided to get folder name from"
    }
    return Split-Path $folderPath -Leaf
}

function ConvertJsonFileToObject {
    param (
        [string] $jsonFile
    )  

    return Get-Content -Path $jsonFile | ConvertFrom-Json
}
