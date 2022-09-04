using Autosest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Models;
using Avtotestbot.Models;

namespace Avtotestbot.Databases
{
    public class Database
    {
        private static Database _database;
            public static Database Db
              {
                  get
                  {
                      if (_database == null)
                      {
                          _database = new Database();
                      }
                      return _database;
                  }
              }
        public  UserDatabase UserDb;
        public  QuestionDatabase QuestionDb;
        public TicketDatabase TicketDb;
        public Database()
        {
            QuestionDb = new QuestionDatabase(ReadQuestionJson());
            UserDb = new UserDatabase(ReadUserJson());
            TicketDb = new TicketDatabase();
        }
        
        private const string QuestionJsonPath = "JsonData/uzlotin.json";
        private const string UsersJsonPath = "/JsonData/Users";
        private const string ImagePath = "Images";

        public static List<User> ReadUserJson()
        {
            if (File.Exists(UsersJsonPath))
            {
                try   // bizga kerakli fayl yoq bo'lishi yoki file o'zgargan bo'lishi mumkin shuning uchun try catchga olamiz
                {
                    var json = File.ReadAllText(UsersJsonPath);
                    return JsonConvert.DeserializeObject<List<User>>(json);
                }
                catch
                {
                    Console.WriteLine("Cannot convert file...........");
                    return new List<User>();
                }
            }


            return new List<User>();
        }



        public static List<QuestionEntity> ReadQuestionJson()
        {
            if (File.Exists(QuestionJsonPath))
            {
                var json = File.ReadAllText(QuestionJsonPath);
                try
                {
                    var res  = JsonConvert.DeserializeObject<List<QuestionEntity>>(json);
                    return res;
                }
                catch
                {
                    Console.WriteLine("Cannot convert question...........");
                    return new List<QuestionEntity>();
                }
            }

            return new List<QuestionEntity>();

        }


        


        public static Stream GetQuestionMedia(string imagename) // vazifasi Images faylidagi rasmni bizga qaytaradi 
        {
            // imagename - bu image name
            //var image = ImagePath + "/" + ImagePath + ".png";
            var image = Path.Combine(ImagePath, $"{imagename}.png"); // fayl manzilini yaratib beradi 
            if(File.Exists(image))
            {
                var bytes = File.ReadAllBytes(image); // bytes typegi o'zgartiradi 
                return new MemoryStream(bytes); // stream ga o'zgartiradi 
            }
            return null;
        }

    }
}
