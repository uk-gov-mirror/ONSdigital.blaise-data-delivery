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

    $exePath = Join-Path -Path $pathTo7zip -ChildPath "7za.exe"
    if (-not (Test-Path $exePath)) {
        throw "7-Zip executable for extraction not found at '$exePath'"
    }

    LogInfo("Using 7-Zip executable for extraction: $exePath")
    $arguments = @(
        "x", # Extract with full paths
        "`"$zipFilePath`"", # Source ZIP file, quoted
        "-o`"$destinationPath`"", # Output directory, quoted, no space after -o
        "-y"  # Assume Yes to all queries (e.g., overwrite files)
    )

    LogInfo("Attempting to extract zip file '$zipFilePath' to path '$destinationPath' using '$exePath'...")
    
    $tempDirForLogs = if (-not [string]::IsNullOrWhiteSpace($pathTo7zip)) { $pathTo7zip } else { $env:TEMP }
    $stdOutFile = Join-Path -Path $tempDirForLogs -ChildPath "7zip_extract_stdout_$(Get-Random).log"
    $stdErrFile = Join-Path -Path $tempDirForLogs -ChildPath "7zip_extract_stderr_$(Get-Random).log"

    try {
        $process = Start-Process -FilePath $exePath -ArgumentList $arguments -Wait -PassThru -NoNewWindow -RedirectStandardOutput $stdOutFile -RedirectStandardError $stdErrFile
        
        $stdOutput = if (Test-Path $stdOutFile) { Get-Content $stdOutFile -Raw } else { "No standard output." }
        $stdError = if (Test-Path $stdErrFile) { Get-Content $stdErrFile -Raw } else { "No standard error." }

        if ($process.ExitCode -eq 0) {
            LogInfo("Successfully extracted zip file '$zipFilePath' to '$destinationPath'.")
            if ($stdOutput -ne "No standard output." -and -not [string]::IsNullOrWhiteSpace($stdOutput)) { LogInfo("7-Zip Output: $stdOutput") }
            if ($stdError -ne "No standard error." -and -not [string]::IsNullOrWhiteSpace($stdError)) { LogInfo("7-Zip Error Output (though successful, could be warnings): $stdError") }
        }
        else {
            $errorMessage = "7-Zip failed to extract '$zipFilePath'. Exit code: $($process.ExitCode)."
            $errorMessage += "`n7-Zip Standard Output:`n$stdOutput"
            $errorMessage += "`n7-Zip Standard Error:`n$stdError"
            throw $errorMessage
        }
    }
    catch {
        throw "An error occurred while trying to run 7-Zip for extraction of '$zipFilePath': $($_.Exception.Message)"
    }
    finally {
        if (Test-Path $stdOutFile) { Remove-Item $stdOutFile -ErrorAction SilentlyContinue }
        if (Test-Path $stdErrFile) { Remove-Item $stdErrFile -ErrorAction SilentlyContinue }
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
        throw "No files provided to add to zip '$zipFilePath'."
    }

    $parentDirOfZip = Split-Path -Path $zipFilePath -Parent
    $tempPath = if (-not [string]::IsNullOrWhiteSpace($parentDirOfZip)) {
        $parentDirOfZip
    } else {
        $PWD.Path # PWD is the current working directory
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

        # Use Start-Process to capture output for this specific 7-Zip call
        $createArguments = @(
            "a", # Add to archive
            "-tzip", # Specify ZIP archive type
            "`"$zipFilePath`"", # Output ZIP file path, quoted
            "`"$tempWorkingDir\*`"", # Add all contents from the temporary directory
            "-y"  # Assume Yes to all queries
        )
        $createStdOutFile = Join-Path -Path $tempPath -ChildPath "7zip_create_final_stdout_$(Get-Random).log"
        $createStdErrFile = Join-Path -Path $tempPath -ChildPath "7zip_create_final_stderr_$(Get-Random).log"

        $createProcess = Start-Process -FilePath $exe -ArgumentList $createArguments -Wait -PassThru -NoNewWindow -RedirectStandardOutput $createStdOutFile -RedirectStandardError $createStdErrFile
        
        $createStdOutput = if (Test-Path $createStdOutFile) { Get-Content $createStdOutFile -Raw } else { "No standard output." }
        $createStdError = if (Test-Path $createStdErrFile) { Get-Content $createStdErrFile -Raw } else { "No standard error." }

        if ($createProcess.ExitCode -eq 0) {
            LogInfo("Successfully created new zip archive '$zipFilePath' from contents of '$tempWorkingDir'.")
            if ($createStdOutput -ne "No standard output." -and -not [string]::IsNullOrWhiteSpace($createStdOutput)) { LogInfo("7-Zip Create Output: $createStdOutput") }
            if ($createStdError -ne "No standard error." -and -not [string]::IsNullOrWhiteSpace($createStdError)) { LogInfo("7-Zip Create Error Output (though successful, could be warnings): $createStdError") }
        } else {
            $errorMessage = "Failed to create new zip archive '$zipFilePath' from temporary directory '$tempWorkingDir'. Exit code: $($createProcess.ExitCode)."
            $errorMessage += "`n7-Zip Create Standard Output:`n$createStdOutput"
            $errorMessage += "`n7-Zip Create Standard Error:`n$createStdError"
            throw $errorMessage
        }
    }
    catch {
        throw "An error occurred during the zip update workaround for '$zipFilePath': $($_.Exception.Message)"
    }
    finally {
        if (Test-Path $tempWorkingDir) {
            LogInfo("Cleaning up temporary working directory '$tempWorkingDir'.")
            Remove-Item -Path $tempWorkingDir -Recurse -Force -ErrorAction SilentlyContinue
            if (Test-Path $createStdOutFile) { Remove-Item $createStdOutFile -ErrorAction SilentlyContinue }
            if (Test-Path $createStdErrFile) { Remove-Item $createStdErrFile -ErrorAction SilentlyContinue }
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
