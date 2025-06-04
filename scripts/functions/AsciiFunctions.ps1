. "$PSScriptRoot\LoggingFunctions.ps1"
. "$PSScriptRoot\FileFunctions.ps1"
function AddAsciiFilesToDeliveryPackage {
    param (
        [string] $processingFolder,
        [string] $questionnaireName,
        [string] $subFolder # Optional: if provided, copy files here from $processingFolder
    )

    # Ensure Manipula script for ASCII generation is in the processing folder
    $msuxFileName = "GenerateAscii.msux"
    $msuxSourcePath = Join-Path $PSScriptRoot "..\manipula\ascii" $msuxFileName
    Copy-Item -Path "$PSScriptRoot\..\manipula\ascii\GenerateAscii.msux" -Destination $processingFolder

    # Generate .ASC file
    try {
        # Manipula.exe should be in $processingFolder (copied by AddManipulaToProcessingFolder)
        & cmd.exe /c $processingFolder\Manipula.exe "$processingFolder\GenerateAscii.msux" -A:True -Q:True -K:Meta="$processingFolder/$questionnaireName.bmix" -I:$processingFolder/$questionnaireName.bdbx -O:$processingFolder/$questionnaireName.asc
        LogInfo("Generated the .ASC file")
    }
    catch {
        LogWarning("Generating ASCII Failed for $questionnaireName : $($_.Exception.Message)")
        return # Exit function if generation fails
    }

    # If a subFolder is specified, copy the generated .asc and any related .fps files to it
    # Otherwise, they remain in $processingFolder where they were generated.
    if (-not [string]::IsNullOrEmpty($subFolder)) {
        LogInfo("Copying ASCII related files from $processingFolder to subfolder: $subFolder")
        Copy-Item -Path "$processingFolder\*.asc", "$processingFolder\*.fps" -Destination $subFolder -Force -ErrorAction SilentlyContinue
        LogInfo("Copied .ASC and .FPS files to $subFolder")
    }
    else {
        LogInfo("ASCII files generated in $processingFolder. No subfolder specified for further copying.")
    }
}
