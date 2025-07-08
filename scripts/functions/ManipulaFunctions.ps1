. "$PSScriptRoot\FileFunctions.ps1"

function AddManipulaToProcessingFolder {
    param (
        [string] $manipulaPackage,
        [string] $processingFolder,
        [string] $processingPath
    )

    If ([string]::IsNullOrEmpty($manipulaPackage)) {
        throw "manipulaPackage not provided"
    }

    If (-not (Test-Path $manipulaPackage)) {
        throw "$manipulaPackage not found"
    }

    If ([string]::IsNullOrEmpty($processingFolder)) {
        throw "processingFolder not provided"
    }

    If (-not (Test-Path $processingFolder)) {
        throw "$processingFolder not found"
    }

    # Extract Manipula files to the processing folder
    ExtractZipFile -pathTo7zip $processingPath -zipFilePath $manipulaPackage -destinationPath $processingFolder
}
