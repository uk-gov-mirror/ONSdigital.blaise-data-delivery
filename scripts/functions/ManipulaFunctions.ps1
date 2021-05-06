. "$PSScriptRoot\FileFunctions.ps1"
function AddManipulaToProcessingFolder {
    param (
        [string] $manipulaPackage = "$env:TempPath/manipula.zip",
        [string] $processingFolder,
        [string] $deliveryFile
    )

    If (-not (Test-Path $manipulaPackage)) {
        throw "$manipulaPackage not found" }

    If (-not (Test-Path $processingFolder)) {
        throw "$processingFolder not found" }

    If (-not (Test-Path $deliveryFile)) {
        throw "$deliveryFile not found" }
        
    # Extract Manipula files to the processing folder
    ExtractZipFile -zipFilePath $manipulaPackage -destinationPath $processingFolder

    # Extact Instrument Package 
    ExtractZipFile -zipFilePath $deliveryFile -destinationPath $processingFolder
}
