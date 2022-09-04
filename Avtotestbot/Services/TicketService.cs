using Autosest.Models;
using Avtotestbot.Databases;
using Avtotestbot.NewFolder;
using Avtptestbot.Enum;
using Models;
using Telegram.Bot.Types.ReplyMarkups;
using User = Avtotestbot.Models.User;

namespace Avtotestbot.Services
{
    public class TicketService
    {
        private readonly TelegrambotService _telegrambotService;

        public TicketService(TelegrambotService telegrambotService)
        {
            _telegrambotService = telegrambotService;
        }

        public void ShowTickets(User user)
        {

            var ticketscount = Database.Db.TicketDb.GetTicketCount();

            var tickets = Database.Db.TicketDb.GetUserTicketValuechatId(user.ChatId);
            // savolllar soni
            var TicketsList = new Dictionary<string, string>();
            for (int i = 0; i < ticketscount; i++)
            {
                var ticketText = $"Ticket {i + 1}";
                if (tickets.Any(t => t.Index == i))
                {
                    var ticket = tickets.First(t => t.Index == i);
                    if (ticket.isCompleted)
                    {
                        ticketText += " ✔";
                    }
                    else
                    {
                        ticketText += $" - [{ticket.CorrectAnswerCount}/{ticket.QuestionCount}]";
                    }

                }
                TicketsList.Add($"{i}", ticketText);
            }


            _telegrambotService.SendMessageTextInline(user.ChatId, "Biletni tanlang \n", reply:
                _telegrambotService.GetInlineKeyboardMatrix(TicketsList, 4));
            user.SetStep(EUserStep.TicketsList);
        }

        public void ShowStartTicket(User user, string message)
        {
            var list = new List<string> { "Start" };
            _telegrambotService.SendMessageTextInline(user.ChatId, $"Ticket{message}  - Question count : {TicketSettings.ticketQuestioncount}\nTesni boshlash uchun Startni bosing",
                    _telegrambotService.ExamButton(list, message));

        }
        public void SaveTicket(User user, string message, int messageId)
        {

            Databases.Database.Db.TicketDb.OpenNewUserTickets(user);
            int from = Convert.ToInt32(message) * TicketSettings.ticketQuestioncount;
            var questions = Databases.Database.Db.QuestionDb.CreateTicket(from, TicketSettings.ticketQuestioncount);
            var ticket = new Ticket(user.ChatId, new Queue<QuestionEntity>(questions), int.Parse(message)); // savol yasaydi
            // va userga saqlab qo'yadi




            Databases.Database.Db.TicketDb.AddOrGetTicket(user.ChatId, ticket);

            ShowStartTicket(user, message);
            user.SetStep(EUserStep.TicketStarting);
        }

        public void StartTicket(User user, string message, int messageId)
        {
            SendTicketQuestion(user, int.Parse(message));
        }

        public void SendTicketQuestion(User user, int ticketindex)
        {
            var tickets = Database.Db.TicketDb.GetUserTicketValuechatId(user.ChatId);

            var ticket = tickets.First(t => t.Index == ticketindex);

            if (ticket.Questions.Count == 0 || ticket.Questions.Dequeue == null)
            {
                string message = $"Question ended \n Result: {ticket.CorrectAnswerCount}/{ticket.QuestionCount}";
                _telegrambotService.SendMessage(user.ChatId, message);
                user.SetStep(EUserStep.Menu);
                return;
            }
            var question = ticket.Questions.Dequeue();

            DisplayQuestions(user, question, ticket);
            user.SetStep(EUserStep.TicketStarted);
        }
        public void DisplayQuestions(User user, QuestionEntity question, Ticket ticket)
        {


            var correctanswer = question.Choices.First(answer => answer.Answer == true);
            var correctanswerindex = question.Choices.IndexOf(correctanswer);
            var choices = question.Choices.Select(p => p.Text).ToList();

            if (question.Media.Exist)
            {

                _telegrambotService.SendQuestion(
                        chatId: user.ChatId,
                       image: Database.GetQuestionMedia
                        (question.Media.Name),
                       message: question.Question,
                       reply: _telegrambotService.ExamButtonforChoicesexam(choices, correctanswerindex, questionindex:
                       ticket.CurrentQuestion, ticketindex: ticket.Index));


                return;
            }


            _telegrambotService.SendQuestion(chatId: user.ChatId,
               message: question.Question,
            reply: _telegrambotService.ExamButtonforChoicesexam(choices, correctanswerindex, ticket.CurrentQuestion, ticket.Index));

        }

        public void CheckAnswer(User user, string message, int messageId,
            InlineKeyboardMarkup reply)
        {

            var _reply = reply.InlineKeyboard.ToArray();
            int[] user_and_true_answerindex_array;
            try
            {
                user_and_true_answerindex_array = message.Split(',').Select(int.Parse).ToArray();
            }
            catch
            {
                Console.WriteLine("tekshir");
                return;
            }
            string check = " ❌";
            var ticketindex = user_and_true_answerindex_array[3];
            if (user_and_true_answerindex_array[0] == user_and_true_answerindex_array[1])
            {
                var tickets = Database.Db.TicketDb.OpenNewUserTickets(user);
                check = " ✅";
                var ticket = tickets.First(p => p.Index == ticketindex);
                ticket.CorrectAnswerCount++;
            }



            _reply[user_and_true_answerindex_array[1]].ToArray()[0].Text += check;

            _telegrambotService.EditMessageButton(user.ChatId, messageId,
             new InlineKeyboardMarkup(_reply));
            // System.Threading.Thread.Sleep(1000);
            SendTicketQuestion(user, ticketindex);
        }
    }
}
