. "$PSScriptRoot\LoggingFunctions.ps1"
. "$PSScriptRoot\FileFunctions.ps1"

function GetConfigFromFile {
    param (
        [string] $surveyType
    )

    $configFolder = "$PSScriptRoot\..\..\configuration"
    $configFile = "$configFolder\$surveyType.json"

    If (Test-Path $configFile) {
        LogInfo("Found config file for survey - $configFile")
        return ConvertJsonFileToObject($configFile)
    }

    LogInfo("No config file found for survey $surveyType, using default config")
    return ConvertJsonFileToObject("$configFolder\default.json")
}
