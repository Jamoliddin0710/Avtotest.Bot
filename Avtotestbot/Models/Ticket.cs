using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
namespace Autosest.Models
{
    public class Ticket
    {
        public long ChatId { get; set; } // chatid
        public int QuestionCount { get; set; } // savollar soni
        public int CorrectAnswerCount { get; set; } // to'gri javoblar soni
        
        public Queue<QuestionEntity> Questions { get; set; } // savol har bitta biletdagi
        public int Index { get; set; }  // savol indeksi ya'ni random qilib tanlanganda siz
                                        // yechayotgan savol nechadan boshlanganligi
        
        public Ticket(long chatId , Queue<QuestionEntity> questions)
        {
            ChatId = chatId;
            QuestionCount = questions.Count;
            CorrectAnswerCount = 0;
            Questions = questions;
        }


        public Ticket(long chatId,  Queue<QuestionEntity> questions  , int index)
        {
            ChatId = chatId;
            QuestionCount = questions.Count;
            CorrectAnswerCount = 0;
            Questions = questions;
            Index = index;
        }
        public int CurrentQuestion
        {
            get
            {
                return QuestionCount - Questions.Count;
                // bu user nechinchi savolni ishlayotganini ko'rsatish uchun kerak
                // umumiy savollar sonidan qolgan savollar sonini ayiramiz

            }
        }

        public bool isCompleted
        {
            get
            {
                return QuestionCount == CorrectAnswerCount;
            }
        }
        public Ticket(long chatId,  int index , Queue<QuestionEntity> questions)
        {
            QuestionCount = questions.Count;
            CorrectAnswerCount = 0;
            ChatId = chatId;
            Questions = questions;
            this.Index = index;
        }
    }
}
