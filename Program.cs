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

            var conversationHistory = csv.GetRecords<ConversationHistory>().ToList();


            // This week.
            var startTime = DateTime.UtcNow.AddHours(9).StartOfWeek(DayOfWeek.Sunday);
            // Last week.
            // var startTime = DateTime.UtcNow.AddHours(9).AddDays(-7).StartOfWeek(DayOfWeek.Sunday);
            var endTime = startTime.AddDays(7).AddSeconds(-1);

            Console.WriteLine("시작일: " + startTime.Date.ToString("yyyy/MM/dd"));
            Console.WriteLine("종료일: " + endTime.Date.ToString("yyyy/MM/dd"));
            Console.WriteLine();

            var existingUsers = conversationHistory
                .Where(x => x.Date >= startTime && x.Date <= endTime)
                .Select(x => x.User)
                .Distinct();

            var results = conversationHistory
                            .Where(x => x.Date >= startTime && x.Date <= endTime)
                            .Where(x => Regex.IsMatch(x.Message, @"[0-9]+\/4"))
                            // Too long message can be result message.
                            .Where(x => x.Message.Count() < 100)
                            .OrderBy(x => x.User)
                            .ToList();

            var users = results.Select(x => x.User).Distinct();

            System.Console.WriteLine($"지금까지 {users.Count()}명이 인증에 참여해주셨습니다.");
            System.Console.WriteLine();

            var userCounters = new Dictionary<string, int>();

            // 가장 열심히 하는 사람 찾기.
            foreach (var user in users)
            {
                userCounters[user] = results.Count(x => x.User == user);
            }

            var topCountUser = userCounters.OrderByDescending(x => x.Value).First();
            foreach (var userCounter in userCounters)
            {
                if (userCounter.Value == topCountUser.Value)
                {
                    System.Console.WriteLine($"이번 주에는 {userCounter.Key}님이 가장 열심히 하셨네요!");
                }
            }
            System.Console.WriteLine();

            // 인증에 참여 안한 유저 찾기
            foreach (var existingUser in existingUsers)
            {
                if (users.Any(x => x == existingUser) == false)
                {
                    System.Console.WriteLine(existingUser + " 님은 인증에 참여하셨나요?");
                }
            }
            System.Console.WriteLine();

            // 인증 안한 유저 찾기
            foreach (var user in users)
            {
                var result = results
                    .Where(x => x.User == user)
                    .Any(x => x.Message.Contains("4/4"));

                if (result == false)
                {
                    Console.WriteLine(user + " 님은 주 4회 이상 인증을 하셨나요?");
                }
            }

            Console.WriteLine();
            Console.WriteLine("----- 카운팅 기록 -----");
            Console.WriteLine();

            foreach (var result in results)
            {
                Console.Write(result.Date + "    ");
                Console.Write(result.User);
                Console.Write(result.Message.PadLeft(35));
                Console.WriteLine();
            }
        }
    }
}
