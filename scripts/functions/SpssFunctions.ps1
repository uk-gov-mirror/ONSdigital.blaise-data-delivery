. "$PSScriptRoot\LoggingFunctions.ps1"
. "$PSScriptRoot\FileFunctions.ps1"
. "$PSScriptRoot\CloudFunctions.ps1"

function AddSpssFilesToDeliveryPackage {
    param(
        [string] $processingFolder,
        [string] $deliveryZip,
        [string] $questionnaireName,
        [string] $subFolder,
        [string] $dqsBucket,
        [string] $tempPath
    )

    If (-not (Test-Path $processingFolder)) {
        throw "$processingFolder file not found"
    }

    If (-not (Test-Path $deliveryZip)) {
        throw "$deliveryZip file not found"
    }

    If ([string]::IsNullOrEmpty($questionnaireName)) {
        throw "No questionnaire name argument provided"
    }

    if (-not (Test-Path "$processingFolder\spss")) {
        # Copy Manipula spss files to the processing folder
        Copy-Item -Path "$PSScriptRoot\..\manipula\spss\*" -Destination $processingFolder

        # Generate SPS file
        try {
            & cmd.exe /c $processingFolder\Manipula.exe "$processingFolder\GenerateStatisticalScript.msux" -K:meta="$questionnaireName.bmix" -H:"" -L:"" -N:oScript="$questionnaireName,iFNames=,iData=$questionnaireName.bdix" -P:"SPSS;;;;;;$questionnaireName.asc;;;2;;64;;Y" -Q:True
            LogInfo("Generated the .SPSS file")
            #create an sps folder
            CreateANewFolder -folderPath $processingFolder -folderName "spss"
            #copying the files needed to create an sps file
            Copy-Item -Path "$processingFolder\*.sps" -Destination "$processingFolder/spss"
            #adding the above files to the delivery zip
            AddFolderToZip -pathTo7zip $tempPath -folder "$processingFolder/spss" -zipFilePath $deliveryZip
            #uploading the delivery file back to the dqs bucket with the
            UploadFileToBucket -filePath $deliveryZip -bucketName $dqsBucket -deliveryFileName "$($questionnaireName).bpkg"
        }
        catch {
            LogWarning("Generating SPS and FPS Failed for $questionnaireName : $($_.Exception.Message)")
            Get-Error
        }
    }
    else {
        Copy-Item -Path "$processingFolder/spss/*" -Destination $processingFolder -verbose
    }
    
    if (-not [string]::IsNullOrEmpty($subFolder)) {
        LogInfo("Copying spss related files from $processingFolder to subfolder: $subFolder")
        Copy-Item -Path "$processingFolder\*.sps" -Destination $subFolder
        LogInfo("Copied spss files to $subFolder")
    }
    else {
        LogInfo("spss files generated in $processingFolder. No subfolder specified for further copying.")
    }
}
