function GenerateDeliveryFileName {
    param (
        [string] $prefix,
        [string] $suffix,
        [string] $questionnaireName,
        [datetime] $dateTime = (Get-Date),
        [string] $fileExt
    )

    If ([string]::IsNullOrEmpty($prefix)) {
        throw "Prefix not provided"
    }

    If ([string]::IsNullOrEmpty($questionnaireName)) {
        throw "Questionnaire name not provided"
    }

    If ([string]::IsNullOrEmpty($fileExt)) {
        throw "File extension not provided"
    }

    return "$($prefix)_$($questionnaireName)_$($dateTime.ToString("ddMMyyyy"))_$($dateTime.ToString("HHmmss"))$($suffix).$fileExt"
}

function GenerateBatchFileName {
    param (
        [datetime] $dateTime = (Get-Date),
        [string] $surveyType
    )
    If ([string]::IsNullOrEmpty($surveyType)) {
        throw "Survey type not provided"
    }
    return "$($surveyType)_$($dateTime.ToString("ddMMyyyy"))_$($dateTime.ToString("HHmmss"))"
}

function ExtractZipFile {
    param (
        [string] $pathTo7zip,
        [string] $zipFilePath,
        [string] $destinationPath
    )

    If (-not (Test-Path $zipFilePath)) {
        throw "$zipFilePath not found"
    }

    & $pathTo7zip\7za x $zipFilePath -o"$destinationPath" # x = extract and keep folder structure
}

function AddFilesToZip {
    param (
        [string] $pathTo7zip,
        [string[]] $files,
        [string] $zipFilePath
    )

    If ($files.count -eq 0) {
        throw "No files provided"
    }

    & $pathTo7zip\7za a $zipFilePath $files # a = add
}

function AddFolderToZip {
    param (
        [string] $pathTo7zip,
        [string] $folder,
        [string] $zipFilePath
    )

    If (-not (Test-Path $folder)) {
        throw "$folder not found"
    }

    & $pathTo7zip\7za a $zipFilePath $folder # a = add
}

function CreateFolder {
    param (
        [string] $folderPath,
        [string] $folderName
    )
    If ([string]::IsNullOrEmpty($folderPath)) {
        throw "Folder path not provided"
    }
    If ([string]::IsNullOrEmpty($folderName)) {
        throw "Folder name not provided"
    }

    if (-not (Test-Path $folderPath\$folderName)) {
        New-Item -Path $folderPath -Name $folderName -ItemType "directory" | Out-Null
    }

    return "$folderPath\$folderName"
}

function ConvertJsonFileToObject {
    param (
        [string] $jsonFile
    )
    return Get-Content -Path $jsonFile | ConvertFrom-Json
}
