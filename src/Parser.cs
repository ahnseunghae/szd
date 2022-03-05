using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SDZ;

namespace szd
{
    public class Parser
    {
        public IEnumerable<string> GetExceptionUsers(
            IEnumerable<string> users,
            IEnumerable<ConversationHistory> conversationHistories)
        {
            var exceptionUsers = new List<string>();

            foreach (var user in users)
            {
                foreach (var conversationHistory in conversationHistories)
                {
                    if (conversationHistory.Message.Contains(user) &&
                        conversationHistory.Message.Contains(" 예외"))
                    {
                        exceptionUsers.Add(user);
                        break;
                    }
                }
            }

            return exceptionUsers;
        }

        public IEnumerable<string> GetWhyExceptionUsers(
            IEnumerable<string> users,
            IEnumerable<ConversationHistory> conversationHistories)
        {
            var exceptionUsers = new List<string>();

            foreach (var user in users)
            {
                foreach (var conversationHistory in conversationHistories)
                {
                    if (conversationHistory.Message.Contains(user) &&
                        conversationHistory.Message.Contains("4/예외"))
                    {
                        exceptionUsers.Add(user);
                        break;
                    }
                }
            }
            return exceptionUsers;
        }

        public IEnumerable<ConversationHistory> GetExceptions(
            IEnumerable<string> users,
            IEnumerable<ConversationHistory> conversationHistories)
        {
            var exceptions = new List<ConversationHistory>();

            foreach (var conversationHistory in conversationHistories)
            {
                foreach (var user in users)
                {
                    if (conversationHistory.Message.Contains(user) &&
                        conversationHistory.Message.Contains(" 예외"))
                    {
                        exceptions.Add(conversationHistory);
                        break;
                    }
                }
            }
            return exceptions;
        }

        public IEnumerable<ConversationHistory> GetWorkouts(
            string user,
            IEnumerable<ConversationHistory> conversationHistories)
        {
            var regex = new Regex(@"[0-9]+\/4");

            var results = conversationHistories
                .Where(x => regex.IsMatch(x.Message) && x.Message.Contains(user))
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
