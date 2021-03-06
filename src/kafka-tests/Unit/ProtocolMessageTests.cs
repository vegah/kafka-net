﻿using System.Linq;
using KafkaNet;
using KafkaNet.Protocol;
using NUnit.Framework;


namespace kafka_tests
{
    [TestFixture]
    [Category("Unit")]
    public class ProtocolMessageTests
    {
        [Test]
        [ExpectedException(typeof(FailCrcCheckException))]
        public void DecodeMessageShouldThrowWhenCrcFails()
        {
            var testMessage = new Message
            {
                Key = "test",
                Value = "kafka test message."
            };

            var encoded = Message.EncodeMessage(testMessage);
            encoded[0] += 1;
            var result = Message.DecodeMessage(0, encoded).First();
        }

        [Test]
        [TestCase("test key", "test message")]
        [TestCase(null, "test message")]
        [TestCase("test key", null)]
        [TestCase(null, null)]
        public void EnsureMessageEncodeAndDecodeAreCompatible(string key, string value)
        {
            var testMessage = new Message
                {
                    Key = key,
                    Value = value
                };

            var encoded = Message.EncodeMessage(testMessage);
            var result = Message.DecodeMessage(0, encoded).First();

            Assert.That(testMessage.Key, Is.EqualTo(result.Key));
            Assert.That(testMessage.Value, Is.EqualTo(result.Value));
        }

        [Test]
        public void EncodeMessageSetEncodesMultipleMessages()
        {
            //expected generated from python library
            var expected = new byte[]
                {
                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16, 45, 70, 24, 62, 0, 0, 0, 0, 0, 1, 49, 0, 0, 0, 1, 48, 0, 0, 0,
                    0, 0, 0, 0, 0, 0, 0, 0, 16, 90, 65, 40, 168, 0, 0, 0, 0, 0, 1, 49, 0, 0, 0, 1, 49, 0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 0, 16, 195, 72, 121, 18, 0, 0, 0, 0, 0, 1, 49, 0, 0, 0, 1, 50
                };

            var messages = new[]
                {
                    new Message {Value = "0", Key = "1"},
                    new Message {Value = "1", Key = "1"},
                    new Message {Value = "2", Key = "1"}
                };

            var result = Message.EncodeMessageSet(messages);

            Assert.That(expected, Is.EqualTo(result));
        }
    }
}
