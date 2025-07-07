. "$PSScriptRoot\LoggingFunctions.ps1"
. "$PSScriptRoot\FileFunctions.ps1"

function AddXmlToDelivery {
    param(
        [string] $processingFolder,
        [string] $questionnaireName,
        [string] $subFolder
    )

    If ([string]::IsNullOrEmpty($processingFolder)) {
        throw "processingFolder not provided"
    }

    If (-not (Test-Path $processingFolder)) {
        throw "$processingFolder not found" 
    }
    
    If ([string]::IsNullOrEmpty($questionnaireName)) {
        throw "questionnaireName not provided" 
    }

    # Copy Manipula XML scripts to processing folder
    Copy-Item -Path "$PSScriptRoot\..\manipula\xml\*" -Destination $processingFolder -Force

    # MetaViewer.exe exports as <QuestionnaireName>_meta.xml in the same folder as .bmix
    $xmlOutputPathInProcessing = Join-Path $processingFolder "$($questionnaireName)_meta.xml"

    # Generate XML (metadata only)
    try {
        $metaViewerPath = Join-Path $processingFolder "MetaViewer.exe" # Assuming MetaViewer.exe is part of Manipula package
        $bmixPath = Join-Path $processingFolder "$questionnaireName.bmix"
        $outputPath = Join-Path $processingFolder "$questionnaireName.xml"
        $arguments = @(
            "-F:`"$bmixPath`"",
            "-Export"
        )
        $process = Start-Process -FilePath $metaViewerPath -ArgumentList $arguments -Wait -PassThru -NoNewWindow
        if ($process.ExitCode -eq 0) {
            LogInfo("Successfully generated XML for $questionnaireName")
        }
        else {
            LogWarning("Failed generating XML for $questionnaireName")
            LogWarning("Manipula exit code - $process.ExitCode")
        }
    }
    catch {
        LogWarning("Failed generating XML for $questionnaireName")
        LogWarning("$($_.Exception.Message)")
        return
    }

    # Move output to sufolder if specified
    if (-not [string]::IsNullOrEmpty($subFolder)) {
        LogInfo("Copying XML output to subfolder")
        Move-Item -Path "$processingFolder\*.xml" -Destination $subFolder -Force -ErrorAction SilentlyContinue
        LogInfo("Copied XML output to subfolder")
    }
    else {
        LogInfo("XML output not copied to subfolder")
    }
}
