
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
        public void Given_I_Call_MapToMessageModel_With_A_JsonMessage_Then_A_MessageModel_Is_Returned()
        {
            //arrange
            var message = "Message";

            //act
            var result = _sut.MapToMessageModel(message);

            //assert
            Assert.NotNull(result);
            Assert.IsInstanceOf<MessageModel>(result);
        }

        [Test]
        public void Given_I_Call_MapToMessageModel_With_A_JsonMessage_Then_A_The_Correct_Service_Call_Is_Made()
        {
            //arrange
            var message = "Message";

            //act
            var result = _sut.MapToMessageModel(message);

            //assert
            _serializerMock.Verify(v => v.DeserializeJsonMessage<Dictionary<string, string>>(message));
        }

        [Test]
        public void Given_I_Call_MapToMessageModel_With_A_Valid_Json_Message_Then_A_MessageModel_With_The_Correct_Data_Is_Returned()
        {
            //arrange
            var message = "Message";

            Dictionary<string, string> messageDictionary = new Dictionary<string, string>
            {
                {"source_hostname", "hostname" },
                {"source_instrument", "OPN2004A" },
                {"source_server_park", "LocalDevelopment" },
                {"source_file", "D:\\Temp\\OPN" },
                {"output_filepath", "D:\\Temp\\OPN\\Output" },
            };

            _serializerMock.Setup(s => s.DeserializeJsonMessage<Dictionary<string, string>>(It.IsAny<string>())).Returns(messageDictionary);

            //act
            var result = _sut.MapToMessageModel(message);

            //assert
            Assert.AreEqual("hostname", result.HostName);
            Assert.AreEqual("OPN2004A", result.InstrumentName);
            Assert.AreEqual("LocalDevelopment", result.ServerPark);
            Assert.AreEqual("D:\\Temp\\OPN", result.SourceFilePath);
            Assert.AreEqual("D:\\Temp\\OPN\\Output", result.OutputFilePath);
        }

        [Test]
        public void Given_I_Call_MapToMessageModel_With_A_Valid_Json_Message_But_An_Expected_Field_Is_Missing_Then_The_Remainder_Of_The_Mode_Is_Returned()
        {
            //arrange
            var message = "Message";

            Dictionary<string, string> messageDictionary = new Dictionary<string, string>
            {
                {"source_instrument", "OPN2004A" },
                {"source_server_park", "LocalDevelopment" },
                {"source_file", "D:\\Temp\\OPN" },
                {"output_filepath", "D:\\Temp\\OPN\\Output" },
            };

            _serializerMock.Setup(s => s.DeserializeJsonMessage<Dictionary<string, string>>(It.IsAny<string>())).Returns(messageDictionary);

            //act
            var result = _sut.MapToMessageModel(message);

            //assert
            Assert.IsNull(result.HostName);

            Assert.AreEqual("OPN2004A", result.InstrumentName);
            Assert.AreEqual("LocalDevelopment", result.ServerPark);
            Assert.AreEqual("D:\\Temp\\OPN", result.SourceFilePath);
            Assert.AreEqual("D:\\Temp\\OPN\\Output", result.OutputFilePath);
        }

        [Test]
        public void Given_I_Call_MapToMessageModel_With_A_Null_Message_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            var errorMessage = $"The parameter 'serialisedMessage' must be supplied";
            string message = null;

            //act && assert
            var result = Assert.Throws<ArgumentNullException>(() => _sut.MapToMessageModel(message));
        }

        [Test]
        public void Given_I_Call_MapToMessageModel_With_An_Empty_Message_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            string message = string.Empty;
            var errorMessage = $"A value for the parameter 'serialisedMessage' must be supplied";

            //act && assert
            var result = Assert.Throws<ArgumentException>(() => _sut.MapToMessageModel(message));
            Assert.AreEqual(errorMessage, result.Message);
        }
    }
}
