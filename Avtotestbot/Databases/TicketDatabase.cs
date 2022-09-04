using Autosest.Models;
using Avtotestbot.Models;
using Avtotestbot.NewFolder;

namespace Avtotestbot.Databases
{
    public class TicketDatabase
    {
        public Dictionary<long, List<Ticket>> UserTickets;
        public TicketDatabase()
        {
            UserTickets = new Dictionary<long, List<Ticket>>();
        }

        public List<Ticket> OpenNewUserTickets(User user) // vazifasi user nechta  ishlaganini bilish uchun
        {
            if (UserTickets.ContainsKey(user.ChatId))
            {
                return UserTickets[user.ChatId];
                // Agar user test ishlagan bo'lsa o'sha listni aks holda 
                // yangi list ochib qaytaradi 
            }

            var tickets = new List<Ticket>();
            UserTickets.Add(user.ChatId, tickets);
            return tickets;

        }


        public List<Ticket> GetUserTicketValuechatId(long chatId)
        {
            if (UserTickets.ContainsKey(chatId))
            {
                return UserTickets[chatId];
            }

            var ticket = new List<Ticket>();
            UserTickets.Add(chatId, ticket);
            return ticket;
        }

        public int GetTicketCount()
        {
            return Database.Db.QuestionDb.Questions.Count / TicketSettings.ticketQuestioncount;
        }

        public Ticket AddOrGetTicket(long chatId, Ticket ticket) // bazaga ticket qo'shadi bu ticketda savollar va
                                                                 // nechta yechgani
                                                                 // mavjud bo'ladi 
        {
            var tickets = UserTickets[chatId]; // bizga list beradi 
            if (tickets.Any(t => t.Index == ticket.Index))
            {
                return tickets.First(t => t.Index == ticket.Index);
            }
            tickets.Add(ticket);
            return ticket;
        }
    }


}
