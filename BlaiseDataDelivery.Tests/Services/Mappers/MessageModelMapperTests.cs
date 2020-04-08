
using BlaiseDataDelivery.Interfaces.Services.Json;
using BlaiseDataDelivery.Mappers;
using BlaiseDataDelivery.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BlaiseDataDelivery.Tests.Services.Mappers
{
    public class MessageModelMapperTests
    {
        private MessageModelMapper _sut;

        private Mock<ISerializerService> _serializerMock;

        [SetUp]
        public void SetUpTests()
        {
            _serializerMock = new Mock<ISerializerService>();
            _serializerMock.Setup(s => s.DeserializeJsonMessage<Dictionary<string, string>>(It.IsAny<string>())).Returns(new Dictionary<string, string>());

            _sut = new MessageModelMapper(_serializerMock.Object);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_MapToMessageModel_Then_A_MessageModel_Is_Returned()
        {
            //arrange
            var message = "Message";

            Dictionary<string, string> messageDictionary = new Dictionary<string, string>
            {
                {"source_instrument", "OPN2004A" },
                {"source_file", "D:\\Temp\\OPN" }
            };

            _serializerMock.Setup(s => s.DeserializeJsonMessage<Dictionary<string, string>>(It.IsAny<string>())).Returns(messageDictionary);

            //act
            var result = _sut.MapToMessageModel(message);

            //assert
            Assert.NotNull(result);
            Assert.IsInstanceOf<MessageModel>(result);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_MapToMessageModel_Then_A_The_Correct_Service_Call_Is_Made()
        {
            //arrange
            var message = "Message";

            Dictionary<string, string> messageDictionary = new Dictionary<string, string>
            {
                {"source_instrument", "OPN2004A" },
                {"source_file", "D:\\Temp\\OPN" }
            };

            _serializerMock.Setup(s => s.DeserializeJsonMessage<Dictionary<string, string>>(It.IsAny<string>())).Returns(messageDictionary);

            //act
            var result = _sut.MapToMessageModel(message);

            //assert
            _serializerMock.Verify(v => v.DeserializeJsonMessage<Dictionary<string, string>>(message));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_MapToMessageModel_Then_A_MessageModel_With_The_Correct_Data_Is_Returned()
        {
            //arrange
            var message = "Message";

            Dictionary<string, string> messageDictionary = new Dictionary<string, string>
            {
                {"source_instrument", "OPN2004A" },
                {"source_file", "D:\\Temp\\OPN" }
            };

            _serializerMock.Setup(s => s.DeserializeJsonMessage<Dictionary<string, string>>(It.IsAny<string>())).Returns(messageDictionary);

            //act
            var result = _sut.MapToMessageModel(message);

            //assert
            Assert.AreEqual("OPN2004A", result.InstrumentName);
            Assert.AreEqual("D:\\Temp\\OPN", result.SourceFilePath);
        }

        [Test]
        public void Given_A_Value_Is_Not_Provided_For_Source_File_When_I_Call_MapToMessageModel_Then_An_ArgumentException_Is_thrown()
        {
            //arrange
            var errorMessage = $"Expected value for 'source_file' in the message";
            var message = "Message";

            Dictionary<string, string> messageDictionary = new Dictionary<string, string>
            {
                {"source_instrument", "OPN2004A" },
            };

            _serializerMock.Setup(s => s.DeserializeJsonMessage<Dictionary<string, string>>(It.IsAny<string>())).Returns(messageDictionary);

            //act && assert
            var result = Assert.Throws<ArgumentException>(() => _sut.MapToMessageModel(message));
            Assert.AreEqual(errorMessage, result.Message);
        }

        [Test]
        public void Given_A_Value_Is_Not_Provided_For_Source_Instrument_When_I_Call_MapToMessageModel_Then_An_ArgumentException_Is_thrown()
        {
            //arrange
            var errorMessage = $"Expected value for 'source_instrument' in the message";
            var message = "Message";

            Dictionary<string, string> messageDictionary = new Dictionary<string, string>
            {
                { "source_file", "D:\\Temp\\OPN" }
            };

            _serializerMock.Setup(s => s.DeserializeJsonMessage<Dictionary<string, string>>(It.IsAny<string>())).Returns(messageDictionary);

            //act && assert
            var result = Assert.Throws<ArgumentException>(() => _sut.MapToMessageModel(message));
            Assert.AreEqual(errorMessage, result.Message);
        }

        [Test]
        public void Given_A_Null_Message_WhenI_Call_MapToMessageModel_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            var paramNamessage = $"serialisedMessage";
            string message = null;

            //act && assert
            var result = Assert.Throws<ArgumentNullException>(() => _sut.MapToMessageModel(message));
            Assert.AreEqual(paramNamessage, result.ParamName);
        }

        [Test]
        public void Given_An_Empty_Message_WhenI_Call_MapToMessageModel_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            string message = string.Empty;
            var errorMessage = $"A value for the argument 'serialisedMessage' must be supplied";

            //act && assert
            var result = Assert.Throws<ArgumentException>(() => _sut.MapToMessageModel(message));
            Assert.AreEqual(errorMessage, result.Message);
        }
    }
}
