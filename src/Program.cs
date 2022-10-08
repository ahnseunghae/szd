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
            var startTime = DateTime.UtcNow.AddHours(9).AddDays(-7).StartOfWeek(DayOfWeek.Sunday);

            // This week.
            // var startTime = DateTime.UtcNow.AddHours(9).StartOfWeek(DayOfWeek.Sunday);
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
            var users = parser.GetUsers(conversationHistories);
            // Finds exception users.
            var exceptionUsers = parser.GetExceptionUsers(users, conversationHistories);
            // Finds unreasonable exception user.
            var whyExceptionUsers = parser.GetWhyExceptionUsers(users, conversationHistories);
            // Finds users that already pay tax.
            var alreadyPayTaxUsers = parser.GetAlreadyPayTaxUsers(conversationHistories);

            var workoutUsers = new List<string>();
            // Finds workout done users.
            foreach (var user in users)
            {
                var workouts = parser.GetWorkouts(user, conversationHistories);
                if (workouts.Any())
                {
                    workoutUsers.Add(user);
                }
            }

            var participantCount = exceptionUsers.ToList().Concat(workoutUsers).Distinct().Count();
            System.Console.WriteLine($"{participantCount}명이 인증 참여, 예외 신청을 하였습니다.");
            System.Console.WriteLine($"{workoutUsers.Count()}명이 인증에 참여해주셨습니다 ୧('ധ')୨");
            System.Console.WriteLine($"{exceptionUsers.Count()}명이 예외 신청을 하였습니다.");

            foreach (var whyExceptionUser in whyExceptionUsers)
            {
                Console.WriteLine($"{whyExceptionUser} 님은 예외를 하였지마는 예외를 아니하였습니다.");
            }
            Console.WriteLine();

            foreach (var exceptionUser in exceptionUsers)
            {
                Console.WriteLine($"{exceptionUser} 님은 이번 주 예외를 신청하셨죠?");
            }
            Console.WriteLine();

            foreach (var user in users)
            {
                if (workoutUsers.Any(x => x == user) == false &&
                    exceptionUsers.Any(x => x == user) == false)
                {
                    System.Console.WriteLine(user + " 님은 인증에 참여하셨나요?");
                }
            }
            Console.WriteLine();

            // 인증 안한 유저 찾기
            var regex = new Regex(@"[4-9]+\/4");
            foreach (var user in users)
            {
                var workouts = parser.GetWorkouts(user, conversationHistories);
                if (workouts.Count() < 4 &&
                   (workouts.Any(x => regex.IsMatch(x.Message)) == false) &&
                   exceptionUsers.Any(x => x == user) == false)
                {
                    Console.WriteLine(user + " 님은 주 4회 이상 인증을 하셨나요?");
                }
            }
            Console.WriteLine();

            foreach (var user in alreadyPayTaxUsers)
            {
                Console.WriteLine($"{user} 님께 성실납세자표창 드립니다.");
            }
            Console.WriteLine();

            Console.WriteLine("예외 신청 내역");
            var exceptions = parser.GetExceptions(users, conversationHistories);
            foreach (var exceptionRequest in exceptions)
            {
                Console.Write(exceptionRequest.Date + "    ");
                Console.Write(exceptionRequest.User);
                Console.Write(exceptionRequest.Message.PadLeft(35));
                Console.WriteLine();
            }
            Console.WriteLine();

            Console.WriteLine("인증 기록");
            foreach (var user in users)
            {
                var workouts = parser.GetWorkouts(user, conversationHistories);

                foreach (var workout in workouts)
                {
                    Console.Write(workout.Date + "    ");
                    Console.Write(workout.User);
                    Console.Write(workout.Message.PadLeft(35));
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
        }
    }
}
