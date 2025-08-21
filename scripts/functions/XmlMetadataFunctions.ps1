. "$PSScriptRoot\LoggingFunctions.ps1"
. "$PSScriptRoot\FileFunctions.ps1"

function AddXmlMetadataToDelivery {
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

    # Generate XML metadata
    try {
        $metaViewerPath = Join-Path $processingFolder "MetaViewer.exe"
        $bmixPath = Join-Path $processingFolder "$questionnaireName.bmix"
        $arguments = @(
            "-F:`"$bmixPath`"",
            "-Export"
        )
        $process = Start-Process -FilePath $metaViewerPath -ArgumentList $arguments -Wait -PassThru -NoNewWindow
        if ($process.ExitCode -eq 0) {
            LogInfo("Successfully generated XML metadata for $questionnaireName")
        }
        else {
            LogWarning("Failed generating XML metadata for $questionnaireName")
            LogWarning("MetaViewer exit code - $process.ExitCode")
        }
    }
    catch {
        LogWarning("Failed generating XML metadata for $questionnaireName")
        LogWarning("$($_.Exception.Message)")
        return
    }

    # Temporarily remove "_meta" suffix from XML metadata files to fix LMS DAP delivery
    # Could clash with XML data files
    # Needs to be removed once LMS DAP delivery can handle "_meta" suffix
    try {
        Get-ChildItem -Path $processingFolder -Filter "*_meta*" -File | ForEach-Object {
            $newName = $_.Name -replace "_meta", ""
            LogInfo("Renaming $($_.Name) to $newName")
            Rename-Item -Path $_.FullName -NewName $newName -Force
        }
    }
    catch {
        LogWarning("Failed removing _meta suffix")
        LogWarning("$($_.Exception.Message)")
    }

    # Move output to subfolder if specified
    if (-not [string]::IsNullOrEmpty($subFolder)) {
        LogInfo("Moving XML metadata output to subfolder")
        Move-Item -Path "$processingFolder\*.xml" -Destination $subFolder -Force -ErrorAction SilentlyContinue
        LogInfo("Moved XML metadata output to subfolder")
    }
    else {
        LogInfo("XML metadata output not moved to subfolder")
    }
}
