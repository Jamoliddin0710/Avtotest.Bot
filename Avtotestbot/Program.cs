using Avtotestbot.Services;
using Avtptestbot.Enum;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using Telegram.Bot.Types.ReplyMarkups;

var bot = new TelegrambotService();
var userService = new UserServices(bot);
var exam = new ExaminationService(bot);
var ticketservice = new TicketService(bot);
var menu = new MenuService(bot, exam, ticketservice);
bot.GetUpdate((_, update, _) => Task.Run(() => GetUpdate(update)));

Console.ReadKey();


void GetUpdate(Update update)
{
    var (From, messageId, message, reply, issucces) = GetValue(update);
    if (!issucces) return;
    var user = userService.AddUser(From);


    StepFilter(user, message, messageId, reply);

}

void StepFilter(Avtotestbot.Models.User user, string message, int messageId, InlineKeyboardMarkup reply)
{
    switch (user.Step)
    {
        case EUserStep.NewUser: menu.SendMenu(user, message); break;
        case EUserStep.Menu: menu.TextFilter(user, message); break;
        case EUserStep.Exam: menu.TextFilterExam(user, message); break;
        case EUserStep.StartExam: exam.CheckAnswer(user, message, messageId, reply); break;
        case EUserStep.TicketsList: ticketservice.SaveTicket(user, message, messageId); break;
        case EUserStep.TicketStarting: menu.FilterTickets(user, message, messageId); break;
        case EUserStep.TicketStarted: ticketservice.CheckAnswer(user, message, messageId, reply); break;
    };
}

Tuple<Telegram.Bot.Types.User, int, string, InlineKeyboardMarkup, bool> GetValue(Update update)
{

    Telegram.Bot.Types.User From;
    string message;
    int messageId;
    InlineKeyboardMarkup reply = null; // buttonni edit qilish uchun
    if (UpdateType.CallbackQuery == update.Type)
    {
        From = update.CallbackQuery.From;

        message = update.CallbackQuery.Data;

        messageId = update.CallbackQuery.Message.MessageId;
        reply = update.CallbackQuery.Message.ReplyMarkup;
    }
    else if (UpdateType.Message == update.Type)
    {
        From = update.Message.From;
        message = update.Message.Text;
        messageId = update.Message.MessageId;
    }
    else return new(null, 0, null, null, false);
    return new(From, messageId, message, reply, true);
}


