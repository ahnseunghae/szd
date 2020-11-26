using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SDZ;

namespace szd
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void ExceptionCounterTest()
        {
            var conversationHistories = new List<ConversationHistory>();

            conversationHistories.Add(new ConversationHistory
            {
                Date = DateTime.UtcNow,
                User = "사과🍎/향년30세/남/일산",
                Message = "@칠구/28/여/대구 예외"
            });

            var sut = new Parser();

            var users = sut.GetExceptionUsers(conversationHistories);

            users.First().Should().Be("칠구/28/여/대구");
        }
    }
}
