. "$PSScriptRoot\LoggingFunctions.ps1"
. "$PSScriptRoot\ConfigFunctions.ps1"
. "$PSScriptRoot\SpssFunctions.ps1"
. "$PSScriptRoot\XmlFunctions.ps1"
. "$PSScriptRoot\JsonFunctions.ps1"
. "$PSScriptRoot\AsciiFunctions.ps1"

function AddAdditionalFilesToDeliveryPackage {
    param(
        [string] $surveyType,
        [string] $processingFolder,
        [string] $questionnaireName,
        [string] $subFolder,
        [string] $deliveryFile
    )
          
    If ([string]::IsNullOrEmpty($surveyType)) {
        throw "No surveyType argument provided"
    }

    If ([string]::IsNullOrEmpty($processingFolder)) {
        throw "No processingFolder argument provided"
    }

    If ([string]::IsNullOrEmpty($questionnaireName)) {
        throw "No questionnaire name argument provided"
    }
          
    # Get configuration for survey type
    $config = GetConfigFromFile -surveyType $surveyType
    LogInfo("Add additional files config - $($config.deliver) $($config)")

    # Generate and add Ascii files if configured
    if($config.deliver.ascii -eq $true) {
        LogInfo("Adding ASCII files")
        AddAsciiToDelivery -processingFolder $processingFolder -questionnaireName $questionnaireName -subFolder $subFolder
    }

    # Generate and add SPSS files if configured
    if($config.deliver.spss -eq $true) {
        LogInfo("Adding SPSS files")
        AddSpssToDelivery -deliveryZip $deliveryFile -processingFolder $processingFolder -questionnaireName $questionnaireName -subFolder $subFolder
    }

    # Generate and add XML Files if configured
    if($config.deliver.xml -eq $true) {
        LogInfo("Adding XML files")
        AddXmlToDelivery -processingFolder $processingFolder -questionnaireName $questionnaireName -subFolder $subFolder
    }

    # Generate and add json Files if configured
    if($config.deliver.json -eq $true) {
        LogInfo("Adding JSON files")
        AddJsonToDelivery -processingFolder $processingFolder -questionnaireName $questionnaireName -subFolder $subFolder
    }
}
