using System;

namespace testprojekt.Models
{
    public class Comment
    {
        public int Id { get; set; } // Уникальный идентификатор
        public string UserName { get; set; } // Имя пользователя
        public string Text { get; set; } // Текст комментария
        public DateTime CreatedAt { get; set; } // Дата создания
    }
}
