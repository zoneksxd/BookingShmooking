using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using testprojekt.Data;
using testprojekt.Models;

namespace testprojekt.Hubs
{
    public class CommentsHub : Hub
    {
        private readonly AppDbContext _context;

        public CommentsHub(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendComment(string user, string message)
        {
            // Создаем новый комментарий
            var comment = new Comment
            {
                UserName = user,
                Text = message,
                CreatedAt = DateTime.UtcNow
            };

            // Сохраняем комментарий в базе данных
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Отправляем комментарий всем подключенным клиентам
            await Clients.All.SendAsync("ReceiveComment", user, message, comment.CreatedAt);
        }
    }
}
