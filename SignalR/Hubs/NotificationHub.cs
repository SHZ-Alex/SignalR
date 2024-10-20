using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs
{
    public class NotificationHub : Hub
    {
        private static int _notificationCounter;
        private static List<string> _messages = [];
        
        public async Task SendMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                _notificationCounter++;
                _messages.Add(message);
                await LoadMessages();
            }
        }
        public async Task LoadMessages()
        {
            await Clients.All.SendAsync("LoadNotification", _messages, _notificationCounter);
        }
    }
}