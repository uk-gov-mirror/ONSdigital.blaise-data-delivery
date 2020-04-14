# Blaise_Data_Delivery

Blaise Data Delivery is a Windows service for encrypting data files for a survey and uploading them to a bucket. The service is triggered by listening for messages on a RabbitMQ queue, the message should contain details of where to find the source data files. see examples below

# Setup Development Environment

Clone the git respository to your IDE of choice. Visual Studio 2019 is recomended.

Populate the key values in the App.config file accordingly. **Never committ App.config with key values.**

Build the solution to obtain the necessary references.

# Example Message - Server Source

```
{
  "source_instrument":"OPN1911a"
  ,"source_file":"C:\\Blaise_SPSS_Output\\"
}                  
```

# Installing the Service

  - Build the Solution
    - In Visual Studio select "Release" as the Solution Configuration
    - Select the "Build" menu
    - Select "Build Solution" from the "Build" menu
  - Copy the release files (/bin/release/) to the install location on the server
  - Uninstall any previous installs
    - Stop the service from running
    - Open a command prompt as administrator
    - Navigate to the windows service installer location
      - cd c:\Windows\Microsoft.NET\Framework\v4.0.30319\
    - Run installUtil.exe /U from this location and pass it the location of the service executable
      - InstallUtil.exe /U {install location}\BlaiseDataDelivery.exe
  - Run the installer against the release build
    - Open a command prompt as administrator
    - Navigate to the windows service installer location
      - cd c:\Windows\Microsoft.NET\Framework\v4.0.30319\
    - Run installUtil.exe from this location and pass it the location of the service executable
      - InstallUtil.exe {install location}\BlaiseDataDelivery.exe
    - Set the service to delayed start
    - Start the service

### To run this locally


