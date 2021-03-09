###############################
# SPSS file script
###############################

param([string]$instrumentPackage)

If ([string]::IsNullOrEmpty($instrumentPackage)) {
    throw [System.IO.ArgumentException] "No instrument package argument provided"
}

If (-not (Test-Path $instrumentPackage)) {
    throw [System.IO.FileNotFoundException] "$instrumentPackage file not found"
}

# Create temporary folder to extract the package
$tempPath = [guid]::NewGuid()

# Expand the package into the temporary folder
Expand-Archive $instrumentPackage -DestinationPath $tempPath

# Add the SPSS file to the instrument package
Compress-Archive -Path $tempPath\*.txt -Update -DestinationPath $instrumentPackage

# Remove the temporary files
Remove-Item $tempPath -Recurse
