using Blaise.Queue.Contracts.Interfaces.MessageHandlers;
using BlaiseDataDelivery.Interfaces.Mappers;
using BlaiseDataDelivery.Interfaces.Providers;
using BlaiseDataDelivery.Interfaces.Services.Files;
using BlaiseDataDelivery.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlaiseDataDelivery.MessageHandlers
{
    public class DataDeliveryMessageHandler : IMessageHandlerCallback
    {
        private readonly ILog _logger;
        private readonly IConfigurationProvider _configuration;
        private readonly IMessageModelMapper _mapper;
        private readonly IFileService _fileService;

        public DataDeliveryMessageHandler(
            ILog logger,
            IConfigurationProvider configuration,
            IMessageModelMapper mapper,
            IFileService fileService)
        {
            _logger = logger;
            _configuration = configuration;
            _mapper = mapper;
            _fileService = fileService;
        }

        public bool HandleMessage(string messageType, string message)
        {            
            try
            {
                var messageModel = _mapper.MapToMessageModel(message);

                //get a list of available files for data delivery
                var filesToProcess = _fileService.GetFiles(messageModel.SourceFilePath, _configuration.FilePattern);

                //no files available - an error must have occured
                if(!filesToProcess.Any())
                {
                    _logger.Info($"No files are available to process in the path '{messageModel.SourceFilePath}' for the file pattern '{_configuration.FilePattern}'");
                    return false;
                }

                //encrypt files
                _fileService.EncryptFiles(filesToProcess);

                //create zip file from the encrypted files
                var zipFile = GenerateZipFilePath(messageModel);
                _fileService.CreateZipFile(filesToProcess, zipFile);

                //upload the zip to bucket
                _fileService.UploadFileToBucket(zipFile, _configuration.BucketName);

                //clean up files
                _fileService.DeleteFile(zipFile);
                _fileService.DeleteFiles(filesToProcess);

            }
            catch(Exception ex)
            {
                _logger.Error($"An exception occured in processing messge {message} - {ex.Message}");
            }

            return true;
        }

        private string GenerateZipFilePath(MessageModel messageModel)
        {
            var dateTime = DateTime.Now;

            //generate a file name in the agreed format
            return $"{messageModel.SourceFilePath}\\dd_{messageModel.InstrumentName}_{dateTime:ddmmyy}_{dateTime:hhmmss}.zip";
        }
    }
}
