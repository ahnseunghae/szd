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
            conversationHistories.Add(new ConversationHistory
            {
                Date = DateTime.UtcNow,
                User = "사과🍎/향년30세/남/일산",
                Message = "@사과🍎/향년30세/남/일산 예외"
            });

            var sut = new Parser();

            var users = sut.GetExceptionUsers(new List<string>
            {
                "사과🍎/향년30세/남/일산",
                "칠구/28/여/대구"
            }, conversationHistories);

            users.Should().Contain(x => x == "사과🍎/향년30세/남/일산");
            users.Should().Contain(x => x == "칠구/28/여/대구");
            users.Should().HaveCount(2);
        }

        [TestMethod]
        public void RequestForExceptionWithMultipleUsers()
        {
            var conversationHistories = new List<ConversationHistory>();
            conversationHistories.Add(new ConversationHistory
            {
                Date = DateTime.UtcNow,
                User = "사과🍎/향년30세/남/일산",
                Message = "@라마🦙/37/남/성남 예외, @빵디킹🍑/26/일산,신사 예외"
            });

            var sut = new Parser();

            var users = sut.GetExceptionUsers(new List<string>
            {
                "사과🍎/향년30세/남/일산",
                "칠구/28/여/대구",
                "라마🦙/37/남/성남",
                "빵디킹🍑/26/일산,신사"
            }, conversationHistories);

            users.Should().Contain(x => x == "라마🦙/37/남/성남");
            users.Should().Contain(x => x == "빵디킹🍑/26/일산,신사");
            users.Should().HaveCount(2);
        }

        [TestMethod]
        public void GetMultipleWorkouts()
        {
            var conversationHistories = new List<ConversationHistory>();
            conversationHistories.Add(new ConversationHistory
            {
                Date = DateTime.UtcNow,
                User = "라마🦙/37/남/성남",
                Message = "@일개미🐜/35/송파/여 1/4, @인백/남/37/왕십리 2/4"
            });

            var sut = new Parser();

            var workouts = sut.GetWorkouts("인백/남/37/왕십리", conversationHistories);
            workouts.Should().Contain(x => x.Message == "@일개미🐜/35/송파/여 1/4, @인백/남/37/왕십리 2/4");
            workouts.Should().HaveCount(1);
        }

        [TestMethod]
        public void GetWorkouts()
        {
            var conversationHistories = new List<ConversationHistory>();
            conversationHistories.Add(new ConversationHistory
            {
                Date = DateTime.UtcNow,
                User = "라마🦙/37/남/성남",
                Message = "@라마🦙/37/남/성남 1/4"
            });
            conversationHistories.Add(new ConversationHistory
            {
                Date = DateTime.UtcNow,
                User = "라마🦙/37/남/성남",
                Message = "@라마🦙/37/남/성남 2/4"
            });
            conversationHistories.Add(new ConversationHistory
            {
                Date = DateTime.UtcNow,
                User = "스딩/33/여/성남",
                Message = "@스딩/33/여/성남 1/4"
            });

            var sut = new Parser();

            var workouts = sut.GetWorkouts("라마🦙/37/남/성남", conversationHistories);
            workouts.Should().HaveCount(2);
        }
    }
}
