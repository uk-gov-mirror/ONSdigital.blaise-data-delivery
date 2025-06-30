. "$PSScriptRoot\LoggingFunctions.ps1"
. "$PSScriptRoot\FileFunctions.ps1"
. "$PSScriptRoot\CloudFunctions.ps1"

function AddSpssFilesToDeliveryPackage {
    param(
        [string] $processingFolder,
        [string] $deliveryZip,
        [string] $questionnaireName,
        [string] $subFolder
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

        # If a subFolder is specified, copy the generated .sps files to it
        # Otherwise, they remain in $processingFolder where they were generated.
        if (-not [string]::IsNullOrEmpty($subFolder)) {
            LogInfo("Copying SPSS related files from $processingFolder to subfolder: $subFolder")
            Move-Item -Path "$processingFolder\*.sps" -Destination $subFolder -Force -ErrorAction SilentlyContinue
            LogInfo("Copied .SPS files to $subFolder")
        }
        else {
            LogInfo("SPSS files generated in $processingFolder. No subfolder specified for further copying.")
        }
    }
    catch {
        LogWarning("Generating SPS or Uploading Failed for $questionnaireName : $($_.Exception.Message)")
    }
}
