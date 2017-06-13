using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Telegram.Bot;

using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

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

        public async void SendMessage(long chatId, string msg)
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

        public void AddNewGod((long id, string name, bool flag) newGod)
        {
            new BurningLibrarian().AddPerson(newGod.name, newGod.id);
        }
    }

    public class BurningLibrarian // internal
    {
        private FirebaseClient getFireBase()
        {
            var AuthSecret = "SvHo982KfZBN8uZcLQGzMbf8fyRKJBl3QWXUADZQ";
            var BasePath = "https://eye-of-the-universe.firebaseio.com";
            IFirebaseConfig Config = new FirebaseConfig
            {
                AuthSecret = AuthSecret,
                BasePath = BasePath
            };
            var Client = new FirebaseClient(Config);
            return Client;
        }
        public List<(string Name, long ID)> GetAllChats()
        {
            var chats = new List<(string Name, long ID)> { ("Arseniy", 364448153) };
            // DO FIREBASE
            var Client = getFireBase();
            FirebaseResponse response = Client.Get("");
            chats = response.ResultAs<List<(string, long)>>();
            return chats;
        }

        public void AddPerson(string name, long id)
        {
            // ADD PERSON TO FIREBASE
            var Client = getFireBase();
            Client.Set(id.ToString(), name);
        }
    }
}
