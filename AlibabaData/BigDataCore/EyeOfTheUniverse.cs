using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Telegram.Bot;

namespace BigDataCore
{
    public class EyeOfTheUniverse // telegram bot
    {
        private string _token = "390252714:AAE0YvbmlmPOkyPp-JbmW33ujEOuL8qOgAw";
        private TelegramBotClient _eye;

        public EyeOfTheUniverse()
        {
            _eye = new TelegramBotClient(_token);
        }

        public async void SendMessage(string message)
        {
            var updates = await _eye.GetUpdatesAsync(); // Тут можно найти свой id
            await _eye.SendTextMessageAsync(364448153, $"Minecraft {message}");
            _eye.OnUpdate += _eye_UpdateReceived;
        }

        private void _eye_UpdateReceived(object sender, Telegram.Bot.Args.UpdateEventArgs e)
        {
            Console.WriteLine("Update");
        }
    }
}
