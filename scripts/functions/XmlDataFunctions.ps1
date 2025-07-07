. "$PSScriptRoot\LoggingFunctions.ps1"
. "$PSScriptRoot\FileFunctions.ps1"

function AddXmlDataToDelivery {
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

    # Copy Manipula XML data scripts to processing folder
    Copy-Item -Path "$PSScriptRoot\..\manipula\xml\*" -Destination $processingFolder -Force

    # Generate XML data
    try {
        $manipulaPath = Join-Path $processingFolder "Manipula.exe"
        $msuxPath = Join-Path $processingFolder "GenerateXml.msux"
        $bmixPath = Join-Path $processingFolder "$questionnaireName.bmix"
        $bdbxPath = Join-Path $processingFolder "$questionnaireName.bdbx"
        $outputPath = Join-Path $processingFolder "$questionnaireName.xml"
        $arguments = @(
            "`"$msuxPath`"",
            "-K:Meta=`"$bmixPath`"",
            "-I:`"$bdbxPath`"",
            "-O:`"$outputPath`"",
            "-Q:True"            
        )
        $process = Start-Process -FilePath $manipulaPath -ArgumentList $arguments -Wait -PassThru -NoNewWindow
        if ($process.ExitCode -eq 0) {
            LogInfo("Successfully generated XML data for $questionnaireName")
        }
        else {
            LogWarning("Failed generating XML data for $questionnaireName")
            LogWarning("Manipula exit code - $process.ExitCode")
        }
    }
    catch {
        LogWarning("Failed generating XML data for $questionnaireName")
        LogWarning("$($_.Exception.Message)")
        return
    }

    # Move output to subfolder if specified
    if (-not [string]::IsNullOrEmpty($subFolder)) {
        LogInfo("Copying XML data output to subfolder")
        Move-Item -Path "$processingFolder\*.xml" -Destination $subFolder -Force -ErrorAction SilentlyContinue
        LogInfo("Copied XML data output to subfolder")
    }
    else {
        LogInfo("XML data output not copied to subfolder")
    }
}
