. "$PSScriptRoot\LoggingFunctions.ps1"
. "$PSScriptRoot\FileFunctions.ps1"
function AddAsciiFilesToDeliveryPackage {
    param (
        [string] $processingFolder,
        [string] $deliveryZip,
        [string] $questionnaireName,
        [string] $subFolder,
        [string] $tempPath
    )

    Copy-Item -Path "$PSScriptRoot\..\manipula\ascii\GenerateAscii.msux" -Destination $processingFolder
    # Generate .ASC file
    try {
        $manipulaPath = Join-Path -Path $processingFolder -ChildPath "Manipula.exe"
        $msuxPath = Join-Path -Path $processingFolder -ChildPath "GenerateAscii.msux"
        $bmixPath = Join-Path -Path $processingFolder -ChildPath "$questionnaireName.bmix"
        $bdbxPath = Join-Path -Path $processingFolder -ChildPath "$questionnaireName.bdbx"
        $ascPath = Join-Path -Path $processingFolder -ChildPath "$questionnaireName.asc"

        $arguments = @(
            "`"$msuxPath`"",
            "-A:True",
            "-Q:True",
            "-K:Meta=`"$bmixPath`"",
            "-I:`"$bdbxPath`"", 
            "-O:`"$ascPath`""      
        )
        $process = Start-Process -FilePath $manipulaPath -ArgumentList $arguments -Wait -PassThru -NoNewWindow
        if ($process.ExitCode -eq 0) {
            LogInfo("Generated the .ASC file successfully for $questionnaireName")
        }
        else {
            LogWarning("Generating ASCII Failed for $questionnaireName. Manipula.exe exited with code $($process.ExitCode)")
        }
    }
    catch {
        LogWarning("Generating ASCII Failed for $questionnaireName : $($_.Exception.Message)")
    }

    if ([string]::IsNullOrEmpty($subFolder)) {
        # Add the ASC & FPS files to the questionnaire package
        AddFilesToZip -pathTo7zip $tempPath -files "$processingFolder\*.asc", "$processingFolder\*.fps" -zipFilePath $deliveryZip
        LogInfo("Added .ASC File to $deliveryZip")
    }
    else {
        Copy-Item -Path "$processingFolder\*.asc", "$processingFolder\*.fps" -Destination $subFolder

        AddFolderToZip -pathTo7zip $tempPath -folder $subFolder -zipFilePath $deliveryZip
        LogInfo("Added '$subFolder' to '$deliveryZip'")
    }
}
