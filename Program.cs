using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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

            var startTime = DateTime.UtcNow.AddHours(9).StartOfWeek(DayOfWeek.Sunday);
            var endTime = startTime.AddDays(7);

            var results = conversationHistory
                .Where(x => x.Date >= startTime && x.Date <= endTime)
                .Where(x => Regex.IsMatch(x.Message, @"[0-9]+\/4"))
                .OrderBy(x => x.User)
                .ToList();

            foreach (var result in results)
            {
                Console.Write(result.User);
                Console.Write(result.Message.PadLeft(40));
                Console.WriteLine();
            }
        }
    }
}
