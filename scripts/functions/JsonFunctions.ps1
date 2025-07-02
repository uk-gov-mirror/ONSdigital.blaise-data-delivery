. "$PSScriptRoot\LoggingFunctions.ps1"
. "$PSScriptRoot\FileFunctions.ps1"

function AddJsonToDelivery {
    param(
        [string] $processingFolder,
        [string] $questionnaireName,
        [string] $subFolder
    )

    If (-not (Test-Path $processingFolder)) {
        throw "$processingFolder not found" 
    }
    If ([string]::IsNullOrEmpty($questionnaireName)) {
        throw "questionnaireName not provided" 
    }

    # Copy Manipula JSON scripts to processing folder
    Copy-Item -Path "$PSScriptRoot\..\manipula\json\*" -Destination $processingFolder -Force

    # Generate JSON
    try {
        $manipulaPath = Join-Path $processingFolder "Manipula.exe"
        $msuxPath = Join-Path $processingFolder "GenerateJSON.msux"
        $bmixPath = Join-Path $processingFolder "$questionnaireName.bmix"
        $bdbxPath = Join-Path $processingFolder "$questionnaireName.bdbx"
        $outputPath = Join-Path $processingFolder "$questionnaireName.json"
        $arguments = @(
            "`"$msuxPath`"",
            "-Q:True",
            "-K:Meta=`"$bmixPath`"",
            "-I:`"$bdbxPath`"",
            "-O:`"$outputPath`""
        )
        $process = Start-Process -FilePath $manipulaPath -ArgumentList $arguments -Wait -PassThru -NoNewWindow
        if ($process.ExitCode -eq 0) {
            LogInfo("Successfully generated JSON for $questionnaireName")
        }
        else {
            LogWarning("Failed generating JSON for $questionnaireName")
            LogWarning("Manipula exit code - $process.ExitCode")
        }
    }
    catch {
        LogWarning("Failed generating JSON for $questionnaireName")
        LogWarning("$($_.Exception.Message)")
        return
    }

    # Copy output to sufolder if specified
    if (-not [string]::IsNullOrEmpty($subFolder)) {
        LogInfo("Copying JSON output to subfolder")
        Copy-Item -Path "$processingFolder\*.json" -Destination $subFolder -Force -ErrorAction SilentlyContinue
        LogInfo("Copied JSON output to subfolder")
    }
    else {
        LogInfo("JSON outpit not copied to subfolder")
    }
}
