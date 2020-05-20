﻿using BlaiseDataDelivery.Helpers;
using BlaiseDataDelivery.Interfaces.Mappers;
using BlaiseDataDelivery.Interfaces.Services.Json;
using BlaiseDataDelivery.Models;
using System;
using System.Collections.Generic;

namespace BlaiseDataDelivery.Mappers
{
    public class MessageModelMapper : IMessageModelMapper
    {
        private readonly ISerializerService _serializerService;

        public MessageModelMapper(ISerializerService serializerService)
        {
            _serializerService = serializerService;
        }

        public MessageModel MapToMessageModel(string message)
        {
            message.ThrowExceptionIfNullOrEmpty("message");

            var messageDictionary = _serializerService.DeserializeJsonMessage<Dictionary<string, string>>(message);

            return new MessageModel
            {
                InstrumentName = GetValue("source_instrument", messageDictionary),
                SourceFilePath = GetValue("source_file", messageDictionary)
            };
        }

        private string GetValue(string key, IReadOnlyDictionary<string, string> messageDictionary)
        {
            if (messageDictionary.ContainsKey(key))
            {
                return messageDictionary[key];
            }

            throw new ArgumentException($"Expected value for '{key}' in the message");
        }
    }
}
