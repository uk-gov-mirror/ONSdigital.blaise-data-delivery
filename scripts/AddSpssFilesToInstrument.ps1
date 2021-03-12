###############################
# SPSS file script
###############################

param([string]$instrumentName)

If ([string]::IsNullOrEmpty($instrumentName)) {
    throw [System.IO.ArgumentException] "No instrument name argument provided"
}

$instrumentPackage = "$instrumentName.zip"

If (-not (Test-Path $instrumentPackage)) {
    throw [System.IO.FileNotFoundException] "$instrumentPackage file not found"
}

# Create temporary folder to extract the package
$tempPath = Get-Date -format "yyyyMMddHHmmss"

# Extract the instrument package into the temporary folder
Expand-Archive $instrumentPackage -DestinationPath $tempPath
Write-Host "Extracting instrument package to path '$tempPath'"
Write-EventLog -LogName "Application" -Source "DataDelivery" -EntryType Information -Message "Data Delivery: Extracting instrument package to path '$tempPath'"

# Download manipula package from CGP bucket
$manipulaPackage = "C:\blaise\SpssPackage.zip"

If (-not (Test-Path $manipulaPackage)) {
    throw [System.IO.FileNotFoundException] "$manipulaPackage file not found"
}

# Extract the manipula package into the temporary folder
Expand-Archive $manipulaPackage -DestinationPath $tempPath
Write-Host "Extracting manipula package to path '$tempPath'"
Write-EventLog -LogName "Application" -Source "DataDelivery" -EntryType Information -Message "Data Delivery: Extracting manipula package to path '$tempPath'"


# Generate SPS file
& cmd.exe /c .\$tempPath\Manipula.exe "$tempPath\GenerateStatisticalScript.msux" -K:meta="$instrumentName.bmix" -H:"" -L:"" -N:oScript="$instrumentName,iFNames=,iData=$instrumentName.bdix" -P:"SPSS;;;;;;$instrumentName.asc;;;2;;64;;Y" -Q:True | Out-Null
Write-Host "Generated the .SPS file"
Write-EventLog -LogName "Application" -Source "DataDelivery" -EntryType Information -Message "Data Delivery: Generated the .SPS file"

# Generate .ASC file
& cmd.exe /c .\$tempPath\Manipula.exe "$tempPath\ExportData_$instrumentName.msux" -A:True -Q:True | Out-Null
Write-Host "Generated the .ASC file"
Write-EventLog -LogName "Application" -Source "DataDelivery" -EntryType Information -Message "Data Delivery: Generated the .ASC file"

# Add the SPS, ASC & FPS files to the instrument package
Compress-Archive -Path "$tempPath\*.sps","$tempPath\*.asc","$tempPath\*.fps" -Update -DestinationPath $instrumentPackage
Write-Host "Added the SPS, ASC & FPS files to the instrument package '$instrumentPackage'"
Write-EventLog -LogName "Application" -Source "DataDelivery" -EntryType Information -Message "Added the SPS, ASC & FPS files to the instrument package '$instrumentPackage'"

# Remove the temporary files
Remove-Item $tempPath -Recurse
Write-Host "Removed the temporary folder '$tempPath'"

