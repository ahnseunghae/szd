using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper;
using SDZ;

namespace szd
{
    class Program
    {
        static void Main(string[] args)
        {
            using var reader = new StreamReader("data.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var conversationHistories = csv.GetRecords<ConversationHistory>().ToList();

            // Last week.
            // var startTime = DateTime.UtcNow.AddHours(9).AddDays(-7).StartOfWeek(DayOfWeek.Sunday);

            // This week.
            var startTime = DateTime.UtcNow.AddHours(9).StartOfWeek(DayOfWeek.Sunday);
            var endTime = startTime.AddDays(7).AddSeconds(-1);

            Console.WriteLine("시작일: " + startTime.Date.ToString("yyyy/MM/dd"));
            Console.WriteLine("종료일: " + endTime.Date.ToString("yyyy/MM/dd"));
            Console.WriteLine();

            conversationHistories = conversationHistories
                .Where(x => x.Date >= startTime && x.Date <= endTime)
                .Where(x => x.User != "방장봇")
                // Too long message can be result message.
                .Where(x => x.Message.Count() < 100)
                .OrderBy(x => x.User)
                .ToList();

            var parser = new Parser();

            // All SZD members.
            var existingUsers = parser.GetUsers(conversationHistories);
            // Finds exception users.
            var exceptionUsers = parser.GetExceptionUsers(conversationHistories);
            // Finds workout done users.
            var workoutUsers = parser.GetWorkoutUsers(conversationHistories);

            foreach (var existingUser in existingUsers)
            {
                if (workoutUsers.Any(x => x == existingUser) == false &&
                    exceptionUsers.Any(x => x == existingUser) == false)
                {
                    System.Console.WriteLine(existingUser + " 님은 인증에 참여하셨나요?");
                }
            }
            System.Console.WriteLine();

            // 인증 안한 유저 찾기
            foreach (var workoutUser in workoutUsers)
            {
                var result = conversationHistories
                    .Where(x => x.User == workoutUser)
                    .Any(x => x.Message.Contains("4/4"));

                if (result == false &&
                    exceptionUsers.Any(x => x == workoutUser) == false
                    )
                {
                    Console.WriteLine(workoutUser + " 님은 주 4회 이상 인증을 하셨나요?");
                }
            }
            Console.WriteLine();
        }
    }
}
