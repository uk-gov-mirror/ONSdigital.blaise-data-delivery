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

    # Ensure Manipula SPSS scripts are in the $processingFolder for execution
    # Assuming 'GenerateStatisticalScript.msux' is the key script.
    # This copy is redundant if AddManipulaToProcessingFolder is called correctly, but kept for safety based on original code pattern.
    Copy-Item -Path "$PSScriptRoot\..\manipula\spss\*" -Destination $processingFolder -Force

    # Generate SPS file (Manipula outputs to its current working directory, which is $processingFolder here)
    try {
        $manipulaExePath = Join-Path $processingFolder "Manipula.exe"
        $msuxPathInProcessing = Join-Path $processingFolder "GenerateStatisticalScript.msux"
        $bmixPath = Join-Path $processingFolder "$($questionnaireName).bmix"
        $bdixPath = Join-Path $processingFolder "$($questionnaireName).bdix"
        $ascPathForSpss = Join-Path $processingFolder "$($questionnaireName).asc"

        & cmd.exe /c "$manipulaExePath" "$msuxPathInProcessing" -K:meta="$bmixPath" -H:"" -L:"" -N:oScript="$questionnaireName,iFNames=,iData=$bdixPath" -P:"SPSS;;;;;;$ascPathForSpss;;;2;;64;;Y" -Q:True
        LogInfo("Generated the .SPS file in $processingFolder")

        # Determine the target folder for SPSS files (either $processingFolder\spss or $subFolder\spss)
        $spssBaseOutputFolder = if ([string]::IsNullOrEmpty($subFolder)) { $processingFolder } else { $subFolder }
        $spssSpecificFolder = CreateANewFolder -folderPath $spssBaseOutputFolder -folderName "spss"

        # Move the generated .sps file(s) from $processingFolder to $spssSpecificFolder
        Get-ChildItem -Path $processingFolder -Filter "$($questionnaireName)*.sps" | ForEach-Object {
            Move-Item -Path $_.FullName -Destination $spssSpecificFolder -Force
            LogInfo("Moved $($_.Name) to $spssSpecificFolder")
        }
        # LogInfo("Uploading delivery file $deliveryZip to $dqsBucket as part of SPSS processing")
        # UploadFileToBucket -filePath $deliveryZip -bucketName $dqsBucket -deliveryFileName "$($questionnaireName).bpkg"
        # LogInfo("Uploaded $deliveryZip")
    }
    catch {
        LogWarning("Generating SPS or Uploading Failed for $questionnaireName : $($_.Exception.Message)")
    }
}