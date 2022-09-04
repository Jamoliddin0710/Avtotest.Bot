using Avtotestbot.Databases;
using Avtotestbot.Models;

namespace Avtotestbot.Services
{
    public class UserServices
    {
        private readonly TelegrambotService _telegrambotService;

        public UserServices(TelegrambotService telegrambotService)
        {
            _telegrambotService = telegrambotService;
        }
        public User AddUser(Telegram.Bot.Types.User user)
        {
            string name = String.IsNullOrEmpty(user.Username) ? user.FirstName : user.Username;

            return Database.Db.UserDb.AddUser(user.Id, name);
        }
    }
}
