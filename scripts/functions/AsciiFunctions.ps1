. "$PSScriptRoot\LoggingFunctions.ps1"
. "$PSScriptRoot\FileFunctions.ps1"

function AddAsciiToDelivery {
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

    # Copy Manipula ASCII scripts to processing folder
    Copy-Item -Path "$PSScriptRoot\..\manipula\ascii\*" -Destination $processingFolder -Force

    # Generate ASCII (ASC file with response data only, also generates field property "remarks" FPS file)
    try {
        $manipulaPath = Join-Path $processingFolder "Manipula.exe"
        $msuxPath = Join-Path $processingFolder "GenerateAscii.msux"
        $bmixPath = Join-Path $processingFolder "$questionnaireName.bmix"
        $bdbxPath = Join-Path $processingFolder "$questionnaireName.bdbx"
        $outputPath = Join-Path $processingFolder "$questionnaireName.asc"
        $arguments = @(
            "`"$msuxPath`"",
            "-K:Meta=`"$bmixPath`"",
            "-I:`"$bdbxPath`"",
            "-O:`"$outputPath`"",
            "-Q:True"            
        )
        $process = Start-Process -FilePath $manipulaPath -ArgumentList $arguments -Wait -PassThru -NoNewWindow
        if ($process.ExitCode -eq 0) {
            LogInfo("Successfully generated ASCII for $questionnaireName")
        }
        else {
            LogWarning("Failed generating ASCII for $questionnaireName")
            LogWarning("Manipula exit code - $process.ExitCode")
        }
    }
    catch {
        LogWarning("Failed generating ASCII for $questionnaireName")
        LogWarning("$($_.Exception.Message)")
        return
    }

    # Move output to sufolder if specified
    if (-not [string]::IsNullOrEmpty($subFolder)) {
        LogInfo("Copying ASCII output to subfolder")
        Move-Item -Path "$processingFolder\*.asc", "$processingFolder\*.fps" -Destination $subFolder -Force -ErrorAction SilentlyContinue
        LogInfo("Copied ASCII output to subfolder")
    }
    else {
        LogInfo("ASCII output not copied to subfolder")
    }
}
