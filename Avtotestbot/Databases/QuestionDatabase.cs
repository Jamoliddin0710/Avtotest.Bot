using Models;

namespace Avtotestbot.Databases
{
    public class QuestionDatabase
    {
        public List<QuestionEntity> Questions { get; set; }

        public QuestionDatabase(List<QuestionEntity> questions)
        {
            Questions = questions;
        }

        public List<QuestionEntity> CreateTicket(int from = 0, int count = 10)
        {
            return Questions.Skip(from).Take(count).ToList();
        }
    }
}
