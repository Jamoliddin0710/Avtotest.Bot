using Autosest.Models;
using Avtotestbot.Databases;
using Avtotestbot.Models;
using Avtotestbot.NewFolder;
using Avtptestbot.Enum;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace Avtotestbot.Services
{
    public class ExaminationService
    {
        
        private Dictionary<long, Ticket> _exams = new();
        private readonly TelegrambotService _telegrambotService;
        
        public ExaminationService(TelegrambotService telegrambotService)
        {
            _telegrambotService = telegrambotService;
        }
        
        public void StartExam(User user)
        {
            var ticket = CreateTicket(user.ChatId);
            DisplayTicket(user , ticket.Count);
            user.SetStep(EUserStep.Exam);
        }
      
        List<string> list = new List<string> { "Start" };
        void DisplayTicket(User user, int count)
        {
            var buttons = _telegrambotService.MenuButtonShow(new List<string>() { "Menu" });
            var button = _telegrambotService.ExamButton(list);
            string message = $"Exam started \n Question count : {count}";
            _telegrambotService.SendMessageTextInline(user.ChatId, message,
             button);
            _telegrambotService.SendMessageText(user.ChatId, reply: buttons);
        }

        public List<QuestionEntity>  ExamQueue(long chatId)
        {
            Random rand = new Random();

            var randomnumber = rand.Next(0, Database.Db.QuestionDb.Questions.Count / TicketSettings.ticketQuestioncount);
            var questions = Database.Db.QuestionDb.CreateTicket(randomnumber * 10, TicketSettings.ticketQuestioncount);
            return questions;
        }

        public Queue<QuestionEntity> CreateTicket( long chatId)
        {
            var questions = ExamQueue(chatId); // biletlarni yasab beradi 
            var ticket = new Ticket(chatId , new Queue<QuestionEntity>( questions));
            _exams.Add(chatId, ticket);
            return ticket.Questions;
        }

         public void SendTicketQuestion(User user )
        {
            QuestionEntity question;
            var ticketQueue = _exams[user.ChatId].Questions;

            if (ticketQueue == null || ticketQueue.Count == 0)
            {
                TicketFinished(user);
                return;
            }
                question = ticketQueue.Dequeue();
                SendQuestion(user, question);
                user.SetStep(EUserStep.StartExam);
            
        }

        public void SendQuestion(User user, QuestionEntity question)
        {
            
            var questiontext = "[" + _exams[user.ChatId].CurrentQuestion.ToString() + "/" +
                 _exams[user.ChatId].QuestionCount.ToString()  + "] "  + question.Question;
         
            
            var correctanswer = question.Choices.First(answer => answer.Answer == true);
            var correctanswerindex = question.Choices.IndexOf(correctanswer);
            var choices = question.Choices.Select(p => p.Text).ToList();
            
            if (question.Media.Exist)
            {

                _telegrambotService.SendQuestion(
                        chatId: user.ChatId,
                       image: Database.GetQuestionMedia
                        (question.Media.Name),
                       message: questiontext,
                       reply: _telegrambotService.ExamButtonforChoice(choices, correctanswerindex));

                
                return;
            }

            
            _telegrambotService.SendQuestion(chatId: user.ChatId,
               message: questiontext,
            reply: _telegrambotService.ExamButtonforChoice(choices, correctanswerindex));
            
        }

        public void TicketFinished(User user)
        {
            user.SetStep(EUserStep.Menu);
            _telegrambotService.SendMessage(user.ChatId, $"Exam finished  \n Result :" +
                $" {_exams[user.ChatId].CorrectAnswerCount}/{_exams[user.ChatId].QuestionCount}");
            _exams.Remove(user.ChatId);
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

            if (user_and_true_answerindex_array[0] == user_and_true_answerindex_array[1])
            {
                var ticket = _exams[user.ChatId];
                check = " ✅";
                ticket.CorrectAnswerCount ++;
            }
            
            
            
            _reply[user_and_true_answerindex_array[1]].ToArray()[0].Text += check;

            _telegrambotService.EditMessageButton(user.ChatId, messageId,
             new InlineKeyboardMarkup(_reply));

            SendTicketQuestion(user);
        }
    }


}
