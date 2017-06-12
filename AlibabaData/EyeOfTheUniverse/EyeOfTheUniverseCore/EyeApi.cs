using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace EyeOfTheUniverseCore
{
    public class EyeApi
    {
        private string _token = "390252714:AAE0YvbmlmPOkyPp-JbmW33ujEOuL8qOgAw";
        private TelegramBotClient _tbot;
        private BurningLibrarian _lib = new BurningLibrarian();

        public EyeApi()
        {
            _tbot = new TelegramBotClient(_token);
        }

        public void StartListening()
        {
            _tbot.StartReceiving();
        }

        public void StopListening()
        {
            _tbot.StopReceiving();
        }

        public void SpreadMessage(string msg)
        {
            var chats = _lib.GetAllChats();
            foreach (var chat in chats)
                SendMessage(chat.ID, msg);
        }

        public async void SendMessage(int chatId, string msg)
        {
            await _tbot.SendTextMessageAsync(chatId, msg);
        }

        public event EventHandler<Telegram.Bot.Args.MessageEventArgs> OnMessage
        {
            add { _tbot.OnMessage += value; }
            remove { _tbot.OnMessage -= value; }
        }

        public event EventHandler<Telegram.Bot.Args.UpdateEventArgs> OnUpdate
        {
            add { _tbot.OnUpdate += value; }
            remove { _tbot.OnUpdate -= value; }
        }
    }

    internal class BurningLibrarian
    {
        public List<(string Name, int ID)> GetAllChats()
        {
            var chats = new List<(string Name, int ID)> { ("Arseniy", 364448153) };
            // DO FIREBASE 
            return chats;
        }

        public void AddPerson(string name, int id)
        {
            // ADD PERSON TO FIREBASE
        }
    }
}
