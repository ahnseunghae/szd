using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SDZ;

namespace szd
{
    public class Parser
    {
        public IEnumerable<string> GetExceptionUsers(IEnumerable<ConversationHistory> conversationHistories)
        {
            var users = new List<string>();

            // ex.) @칠구/28/여/대구 예외
            var regex = new Regex("^@.* 예외$");

            foreach (var conversationHistory in conversationHistories)
            {
                if (regex.IsMatch(conversationHistory.Message))
                {
                    var user = conversationHistory.Message.Replace("@", "").Split(" ")[0];

                    users.Add(user);
                }
            }
            // Remove duplicated users.
            users = users.Distinct().ToList();

            return users;
        }

        public IEnumerable<ConversationHistory> GetWorkouts(IEnumerable<ConversationHistory> conversationHistories)
        {
            // ex.) 1/4
            var regex = new Regex(@"[0-9]+\/4");

            var results = conversationHistories
                .Where(x => regex.IsMatch(x.Message))
                .ToList();

            return results;
        }

        public IEnumerable<string> GetUsers(IEnumerable<ConversationHistory> conversationHistories)
        {
            return conversationHistories
                .Select(x => x.User)
                .Distinct();
        }
    }
}
