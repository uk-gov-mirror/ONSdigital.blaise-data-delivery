. "$PSScriptRoot\LoggingFunctions.ps1"
. "$PSScriptRoot\FileFunctions.ps1"

function AddSpssToDelivery {
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

    # Copy Manipula SPSS scripts to processing folder
    Copy-Item -Path "$PSScriptRoot\..\manipula\spss\*" -Destination $processingFolder -Force

    # Generate SPSS (SPS file with metadata only)
    try {
        $manipulaPath = Join-Path $processingFolder "Manipula.exe"
        $msuxPath = Join-Path $processingFolder "GenerateStatisticalScript.msux"
        $bmixPath = Join-Path $processingFolder "$questionnaireName.bmix"
        $bdix = "$questionnaireName.bdix"
        $asc = "$questionnaireName.asc"
        $arguments = @(
            "`"$msuxPath`"",
            "-K:meta=`"$bmixPath`"",
            "-H:""",
            "-L:""",
            "-N:oScript=$questionnaireName,iFNames=,iData=$bdix",
            "-P:SPSS;;;;;;$asc;;;2;;64;;Y",
            "-Q:True"
        )
        $process = Start-Process -FilePath $manipulaPath -ArgumentList $arguments -Wait -PassThru -NoNewWindow
        if ($process.ExitCode -eq 0) {
            LogInfo("Successfully generated SPSS for $questionnaireName")
        }
        else {
            LogWarning("Failed generating SPSS for $questionnaireName")
            LogWarning("Manipula exit code - $process.ExitCode")
        }
    }
    catch {
        LogWarning("Failed generating SPSS for $questionnaireName")
        LogWarning("$($_.Exception.Message)")
        return
    }

    # Copy output to sufolder if specified
    if (-not [string]::IsNullOrEmpty($subFolder)) {
        LogInfo("Copying SPSS output to subfolder")
        Copy-Item -Path "$processingFolder\*.sps" -Destination $subFolder -Force -ErrorAction SilentlyContinue
        LogInfo("Copied SPSS output to subfolder")
    }
    else {
        LogInfo("SPSS outpit not copied to subfolder")
    }
}
