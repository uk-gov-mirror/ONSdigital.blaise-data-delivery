using Blaise.Queue.Contracts.Interfaces.MessageHandlers;
using BlaiseDataDelivery.Interfaces.Mappers;
using BlaiseDataDelivery.Interfaces.Services.File;
using System;

namespace BlaiseDataDelivery.MessageHandlers
{
    public class DataDeliveryMessageHandler : IMessageHandlerCallback
    {
        private readonly IMessageModelMapper _mapper;
        private readonly IFileService _fileService;

        public DataDeliveryMessageHandler(
            IMessageModelMapper mapper,
            IFileService fileService)
        {
            _mapper = mapper; 
        }

        public bool HandleMessage(string messageType, string message)
        {
            var messageModel = _mapper.MapToMessageModel(message);
            var temporaryFilePath = GetTemporaryFilePath(messageModel.SourceFilePath);

            try
            {
                _fileService.MoveFiles(messageModel.SourceFilePath, temporaryFilePath);
                var encryptedFilePath = _fileService.EncryptFiles(temporaryFilePath);
                var zipFilePath = _fileService.CreateZipFile(encryptedFilePath);

                _fileService.DeployZipFile(zipFilePath, messageModel.OutputFilePath);
                _fileService.DeleteTemporaryFiles(temporaryFilePath);

                return true;
            }
            catch
            {
                _fileService.RestoreFilesToOriginalLocation(temporaryFilePath, messageModel.SourceFilePath);
                _fileService.DeleteTemporaryFiles(temporaryFilePath);

                return false;
            }
        }

        private string GetTemporaryFilePath(string sourceFilePath)
        {
            var uniqueFolderId = Guid.NewGuid().ToString();
            return $"{sourceFilePath}\\{uniqueFolderId}";
        }
    }
}
