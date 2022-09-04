using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace Avtotestbot.Services
{
    public class TelegrambotService
    {
        private TelegramBotClient Bot;
        private const string BotToken = "5762235550:AAHrSI9NPYmaYaeX20aSvRfW6EgE6FBvJu0";


        public TelegrambotService()
        {
            Bot = new TelegramBotClient(BotToken);
        }

        public void GetUpdate(Func<ITelegramBotClient, Update, CancellationToken, Task> update)
        {
            Bot.StartReceiving(updateHandler: update,
                errorHandler: (_, ex, _) =>
                {
                    Console.WriteLine(ex.Message);
                    return Task.CompletedTask;
                },
                new ReceiverOptions()
                {
                    ThrowPendingUpdates = true,
                });
        }

        public void SendMessage(long chatId, string message)
        {
            Bot.SendTextMessageAsync(chatId, message);
        }

        public void SendMessageText(long chatId, string message = null,
            ReplyKeyboardMarkup reply = null)
        {
            Bot.SendTextMessageAsync(chatId, message, replyMarkup: reply);
        }

        public void SendMessageTextInline(long chatId, string message,
            InlineKeyboardMarkup reply = null)
        {
            Bot.SendTextMessageAsync(chatId, message, replyMarkup: reply);
        }


        public void SendPhoto(long chatId, string message, Stream image, ReplyKeyboardMarkup reply = null)
        {
            Bot.SendPhotoAsync(chatId, new InputOnlineFile(image), message, replyMarkup: reply);
        }


        // menu buttonlar
        public ReplyKeyboardMarkup MenuButtonShow(List<string> buttontext)
        {

            KeyboardButton[][] buttons = new KeyboardButton[buttontext.Count][];
            for (int i = 0; i < buttontext.Count; i++)
            {
                buttons[i] = new KeyboardButton[] { new KeyboardButton(buttontext[i]) };
            }

            return new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
        }

        public InlineKeyboardMarkup ExamButton(List<string> buttonText)
        {
            var buttons = new InlineKeyboardButton[buttonText.Count][];

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i] = new InlineKeyboardButton[] { InlineKeyboardButton
                    .WithCallbackData(buttonText[i], "Start") };
            }

            return new InlineKeyboardMarkup(buttons);
        }

        public InlineKeyboardMarkup ExamButton(List<string> buttonText, string message)
        {
            var buttons = new InlineKeyboardButton[buttonText.Count][];

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i] = new InlineKeyboardButton[] { InlineKeyboardButton
                    .WithCallbackData(buttonText[i], message) };
            }

            return new InlineKeyboardMarkup(buttons);
        }

        public InlineKeyboardMarkup GetInlineKeyboardMatrix(Dictionary<string, string> keys,
            int columns = 2)
        {
            int row = 0;

            var buttonMatrix = new List<List<InlineKeyboardButton>>();

            while (keys.Skip(row).Take(columns)?.Count() > 0)
            {
                var buttons = keys.Skip(row * columns).Take(columns).Select(k => InlineKeyboardButton.WithCallbackData(k.Value, k.Key)).ToList();

                buttonMatrix.Add(buttons);

                row++;
            }

            return new InlineKeyboardMarkup(buttonMatrix.ToArray());
        }

        public InlineKeyboardMarkup ExamButtonTicket(List<string> buttonText)
        {
            var buttons = new InlineKeyboardButton[buttonText.Count][];

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i] = new InlineKeyboardButton[] { InlineKeyboardButton
                    .WithCallbackData(text : "Tickets " + buttonText[i] , callbackData: $"{i}") };
            }

            return new InlineKeyboardMarkup(buttons);
        }


        public InlineKeyboardMarkup ExamButtonforChoice(List<string> buttonText,
            int? correctanswerindex = null, int? questionindex = null)
        {

            InlineKeyboardButton[][] buttons = new InlineKeyboardButton[buttonText.Count][];

            for (var i = 0; i < buttonText.Count; i++)
            {
                buttons[i] = new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData(
                text: buttonText[i],
                callbackData: correctanswerindex == null ? buttonText[i] : $"{correctanswerindex},{i} "),  };
            }

            return new InlineKeyboardMarkup(buttons);
        }



        public InlineKeyboardMarkup ExamButtonforChoicesexam (List<string> buttonText,
            int  correctanswerindex  , int questionindex  , int ticketindex )
        {

            InlineKeyboardButton[][] buttons = new InlineKeyboardButton[buttonText.Count][];

            for (var i = 0; i < buttonText.Count; i++)
            {
                buttons[i] = new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData(
                text: buttonText[i],
                callbackData: correctanswerindex == null ? buttonText[i] :
                $"{correctanswerindex},{i} , {questionindex} , {ticketindex}") };

               
                }

            return new InlineKeyboardMarkup(buttons);
        }


        public void SendQuestion(long chatId, Stream image = default, string message = default,
            IReplyMarkup reply = default)
        {
            Bot.SendPhotoAsync(chatId, new InputOnlineFile(image), message, replyMarkup: reply);
        }

        public void SendQuestion(long chatId, string message = default,
            IReplyMarkup reply = default)
        {
            Bot.SendTextMessageAsync(chatId, message, replyMarkup: reply);
        }


        public void EditMessageButton(long chatId, int messageId, InlineKeyboardMarkup reply)
        {
            Bot.EditMessageReplyMarkupAsync(chatId, messageId, replyMarkup: reply);
        }


    }
}
