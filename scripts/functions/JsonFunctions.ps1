. "$PSScriptRoot\LoggingFunctions.ps1"
. "$PSScriptRoot\FileFunctions.ps1"
function AddJSONFileToDeliveryPackage {
    param(
        [string] $processingFolder,
        [string] $questionnaireName,
        [string] $subFolder # Optional: if provided, copy files here from $processingFolder
    )

    If (-not (Test-Path $processingFolder)) {
        throw "$processingFolder not found" 
    }
    If ([string]::IsNullOrEmpty($questionnaireName)) {
        throw "No questionnaire name provided" 
    }

    # Copy Manipula xml files to the processing folder
    Copy-Item -Path "$PSScriptRoot\..\manipula\json\GenerateJSON.msux" -Destination $processingFolder

    $jsonOutputPath = Join-Path $processingFolder "$($questionnaireName).json"
    $jsonBdixOutputPath = Join-Path $processingFolder "$($questionnaireName).json.bdix"

    try {
        # Generate JSON file
        $manipulaExePath = Join-Path $processingFolder "Manipula.exe"
        $msuxPathInProcessing = Join-Path $processingFolder "GenerateJSON.msux"
        $bmixPath = Join-Path $processingFolder "$($questionnaireName).bmix"
        $bdbxPath = Join-Path $processingFolder "$($questionnaireName).bdbx"
        
        & cmd.exe /c "$manipulaExePath" "$msuxPathInProcessing" -A:True -Q:True -K:Meta="$bmixPath" -I:"$bdbxPath" -O:"$jsonOutputPath"
        LogInfo("Generated .JSON file: $jsonOutputPath")
    }
    catch {
        LogWarning("Generating Json Failed: $($_.Exception.Message)")
        return
    }

    # If a subFolder is specified, copy the generated .json and .json.bdix files to it
    if (-not [string]::IsNullOrEmpty($subFolder)) {
        LogInfo("Copying JSON related files from $processingFolder to subfolder: $subFolder")
        if (Test-Path $jsonOutputPath) {
            Move-Item -Path $jsonOutputPath -Destination (Join-Path $subFolder "$($questionnaireName).json") -Force
            LogInfo("Copied $jsonOutputPath to $subFolder")
        }
        if (Test-Path $jsonBdixOutputPath) {
            Move-Item -Path $jsonBdixOutputPath -Destination (Join-Path $subFolder "$($questionnaireName).json.bdix") -Force
            LogInfo("Copied $jsonBdixOutputPath to $subFolder")
        }
    }
    else {
        LogInfo("JSON files generated in $processingFolder. No subfolder specified for further copying.")
    }
}
