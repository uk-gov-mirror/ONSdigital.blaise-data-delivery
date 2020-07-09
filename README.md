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

#Topics & Subscriptions

This service needs to listen to messages put on the 'data-delivery-topic'. It does this by creating a subscription to this topic. There is an existing subscription called 'data-delivery-subscription', and even though
this service is only installed on TEL at present it could be installed on multiple VMs and therefore will need to create a subscription on initialisation in order for it to receive every message published to the 'data-delivery-subscription'. 
If all the services listened to the same subscription 'data-delivery-subscription' the the messages would be shared between the services, and as each service will have access to different server parks on the different VMs 
this would result in data loss. Each subscription created will consist of the subscription name 'data-delivery-subscription' suffixed with the VM name i.e. 'data-delivery-subscription-TEL-C0C0C0'.

This service does not publish messages.

#debugging
Due to the nature of the GCP pubsub implementation, it will be listening on a worker thread. If you wish to debug the service locally you will
need to add a Thread.Sleep(n seconds) just after the subscription is setup to push the service to use the worker thread in the 'InitialiseSservice' and set a breakpoint. If a breakpoint is not set,
the service will just drop out as pubsub works off a streaming pull mechanism on background worker threads and not events.

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


