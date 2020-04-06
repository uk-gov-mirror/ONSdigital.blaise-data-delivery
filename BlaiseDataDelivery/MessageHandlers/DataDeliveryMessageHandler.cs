using Blaise.Queue.Contracts.Interfaces.MessageHandlers;
using BlaiseDataDelivery.Interfaces.Mappers;
using BlaiseDataDelivery.Interfaces.Providers;
using BlaiseDataDelivery.Interfaces.Services.Files;
using BlaiseDataDelivery.Models;
using log4net;
using System;
using System.Linq;

namespace BlaiseDataDelivery.MessageHandlers
{
    public class DataDeliveryMessageHandler : IMessageHandlerCallback
    {
        private readonly ILog _logger;
        private readonly IConfigurationProvider _configuration;
        private readonly IMessageModelMapper _mapper;
        private readonly IFileProcessingService _fileService;

        public DataDeliveryMessageHandler(
            ILog logger,
            IConfigurationProvider configuration,
            IMessageModelMapper mapper,
            IFileProcessingService fileService)
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
                var filesToProcess = _fileService.GetFiles(messageModel.SourceFilePath, _configuration.FilePattern);

                if(!filesToProcess.Any())
                {
                    _logger.Info($"No files are available to process in the path '{messageModel.SourceFilePath}' for the file pattern '{_configuration.FilePattern}'");
                    return false;
                }

                _fileService.EncryptFiles(filesToProcess);
                _fileService.CreateZipFile(filesToProcess, GenerateZipFilePath(messageModel));
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
            return $"{messageModel.SourceFilePath}\\dd_{messageModel.InstrumentName}.zip";
        }
    }
}
