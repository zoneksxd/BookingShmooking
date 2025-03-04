using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace testprojekt.Hubs
{
    public class WorkspacesHub : Hub
    {
        // Метод для вещания обновлённого рабочего пространства всем клиентам
        public async Task BroadcastWorkspaceUpdate(object workspace)
        {
            await Clients.All.SendAsync("ReceiveWorkspaceUpdate", workspace);
        }
    }
}
