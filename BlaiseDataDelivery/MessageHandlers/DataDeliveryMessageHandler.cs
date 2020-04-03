using Blaise.Queue.Contracts.Interfaces.MessageHandlers;
using BlaiseDataDelivery.Interfaces.Mappers;
using BlaiseDataDelivery.Interfaces.Services.File;
using log4net;
using System;

namespace BlaiseDataDelivery.MessageHandlers
{
    public class DataDeliveryMessageHandler : IMessageHandlerCallback
    {
        private readonly ILog _logger;
        private readonly IMessageModelMapper _mapper;
        private readonly IFileService _fileService;

        public DataDeliveryMessageHandler(
            ILog logger,
            IMessageModelMapper mapper,
            IFileService fileService)
        {
            _logger = logger;
            _mapper = mapper;
            _fileService = fileService;
        }

        public bool HandleMessage(string messageType, string message)
        {            
            string temporaryFilePath = string.Empty;

            try
            {
                var messageModel = _mapper.MapToMessageModel(message);

                temporaryFilePath = _fileService.MoveFilesToTemporaryLocation(messageModel.SourceFilePath);
                var encryptedFilePath = _fileService.EncryptFiles(temporaryFilePath);
                var zipFilePath = _fileService.CreateZipFile(encryptedFilePath);

                _fileService.DeployZipFile(zipFilePath, messageModel.OutputFilePath);
            }
            catch(Exception ex)
            {
                _logger.Error($"An exception occured in processing messge {message} - {ex.Message}");
               // _fileService.RestoreFilesToOriginalLocation(temporaryFilePath, messageModel.SourceFilePath);
               //return false;
            }

            _fileService.DeleteTemporaryFiles(temporaryFilePath);

            return true;
        }
    }
}
