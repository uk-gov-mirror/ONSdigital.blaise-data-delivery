using Blaise.Queue.Contracts.Interfaces.MessageHandlers;
using BlaiseDataDelivery.Interfaces.Mappers;
using BlaiseDataDelivery.Interfaces.Providers;
using BlaiseDataDelivery.Interfaces.Services.File;
using BlaiseDataDelivery.Models;
using log4net;
using System;

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
                var temporaryFilePath = MoveFilesToTemporaryPath(messageModel);
                var encryptedFilePath = _fileService.EncryptFiles(temporaryFilePath);
                var zipFilePath = _fileService.CreateZipFile(encryptedFilePath);

                _fileService.DeployZipFile(zipFilePath, messageModel.OutputFilePath);

                _fileService.DeleteTemporaryFiles(temporaryFilePath);
            }
            catch(Exception ex)
            {
                _logger.Error($"An exception occured in processing messge {message} - {ex.Message}");

                // unhappy path
               // _fileService.RestoreFilesToOriginalLocation(temporaryFilePath, messageModel.SourceFilePath);
               //return false;
            }

            return true;
        }

        private string MoveFilesToTemporaryPath(MessageModel messageModel)
        {
            //unique path to temporarily store files for processing
            var temporarySubFolder = Guid.NewGuid().ToString();
            var temporaryFilePath = $"{messageModel.SourceFilePath}\\{temporarySubFolder}";
            _fileService.MoveFiles(messageModel.SourceFilePath, temporaryFilePath, _configuration.FilePattern);

            return temporaryFilePath;
        }
    }
}
