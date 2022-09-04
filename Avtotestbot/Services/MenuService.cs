using Autosest.Models;
using Avtptestbot.Enum;
using Models;
using Telegram.Bot.Types.ReplyMarkups;
using User = Avtotestbot.Models.User;

namespace Avtotestbot.Services
{

    public class MenuService
    {
        List<string> buttontext = new List<string>() { "Ticket", "Examination" };

        private readonly ExaminationService exam;
        private readonly TelegrambotService _telegrambotService;
        private readonly TicketService _ticketService;
        private readonly MenuService _menu;
        public MenuService(TelegrambotService telegrambotService,
            ExaminationService examinationService
            ,TicketService ticketService)
            
        {

            _telegrambotService = telegrambotService;
            exam = examinationService;
            _ticketService = ticketService;
                     
        }

        

        public void SendMenu(User user, string message)
        {
            
            var buttons = _telegrambotService.MenuButtonShow(buttontext);
            _telegrambotService.SendMessageText(user.ChatId, "Menu tanlang", buttons);

            TextFilter(user, message);

            user.SetStep(EUserStep.Menu);
        }



        public void TextFilter(User user, string message)
        {
            switch (message)
            {
                case "Ticket": _ticketService.ShowTickets(user); break;
                case "Examination": exam.StartExam(user); break;
            }

        }

     
            public void FilterTickets(User user, string message, int messageId)
        {
            switch (message)
            {
                case "Menu": SendMenu(user, message); break;
                default: _ticketService.StartTicket(user, message, messageId); break;
               
            }
        }
        // for exam
        public void TextFilterExam(User user, string message)
        {
            switch (message)
            {
                case "Start": exam.SendTicketQuestion(user); break;
                case "Menu": SendMenu(user, message); break;
            }
        }

    }

}

