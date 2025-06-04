. "$PSScriptRoot\LoggingFunctions.ps1"
. "$PSScriptRoot\FileFunctions.ps1"
function AddXMLFiletoDeliveryPackage {
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
    Copy-Item -Path "$PSScriptRoot\..\manipula\xml\GenerateXML.msux" -Destination $processingFolder

    # MetaViewer.exe exports as <QuestionnaireName>_meta.xml in the same folder as .bmix
    $xmlOutputPathInProcessing = Join-Path $processingFolder "$($questionnaireName)_meta.xml"

    try {
        # Generate XML file, Export function no longer works in Blaise 5
        $metaViewerExePath = Join-Path $processingFolder "MetaViewer.exe" # Assuming MetaViewer.exe is part of Manipula package
        $bmixPath = Join-Path $processingFolder "$($questionnaireName).bmix"
        & cmd.exe /c "$metaViewerExePath" -F:"$bmixPath" -Export
        LogInfo("Generated .XML File in $processingFolder (expected: $xmlOutputPathInProcessing)")
    }
    catch {
        LogWarning("Generating XML Failed: $($_.Exception.Message)")
        return
    }

    # If a subFolder is specified, copy and rename the generated _meta.xml file to it
    if (-not [string]::IsNullOrEmpty($subFolder)) {
        LogInfo("Copying XML related files from $processingFolder to subfolder: $subFolder")
        if (Test-Path $xmlOutputPathInProcessing) {
            $finalXmlNameInSubFolder = Join-Path $subFolder "$($questionnaireName).xml"
            Copy-Item -Path $xmlOutputPathInProcessing -Destination $finalXmlNameInSubFolder -Force
            LogInfo("Copied $xmlOutputPathInProcessing to $finalXmlNameInSubFolder")
        }
        else {
            LogWarning("Expected XML file $xmlOutputPathInProcessing not found for copying.")
        }
    }
    else {
        LogInfo("XML file generated in $processingFolder. No subfolder specified for further copying.")
    }
}
