using Blaise.Queue.Contracts.Interfaces.MessageHandlers;
using BlaiseDataDelivery.Interfaces.Mappers;
using BlaiseDataDelivery.Interfaces.Services.File;

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
            _fileService = fileService;
        }

        public bool HandleMessage(string messageType, string message)
        {
            var messageModel = _mapper.MapToMessageModel(message);
            string temporaryFilePath = string.Empty;

            try
            {
                temporaryFilePath = _fileService.MoveFilesToTemporaryLocation(messageModel.SourceFilePath);
                var encryptedFilePath = _fileService.EncryptFiles(temporaryFilePath);
                var zipFilePath = _fileService.CreateZipFile(encryptedFilePath);

                _fileService.DeployZipFile(zipFilePath, messageModel.OutputFilePath);
            }
            catch
            {
                _fileService.RestoreFilesToOriginalLocation(temporaryFilePath, messageModel.SourceFilePath);
                _fileService.DeleteTemporaryFiles(temporaryFilePath);

                return false;
            }

            _fileService.DeleteTemporaryFiles(temporaryFilePath);

            return true;
        }
    }
}
