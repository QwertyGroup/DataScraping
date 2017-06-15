using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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
            if (chats == null) return;
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

        BurningLibrarian _burnLib = new BurningLibrarian();
        public void AddNewGod((long id, string name, bool flag) newGod)
        {
            _burnLib.AddPerson(newGod.name, newGod.id);
        }

        public void RemoveGod(long id)
        {
            _burnLib.RemovePerson(id);
        }

        public List<(string Name, long ID)> GetGods()
        {
            return _burnLib.GetAllChats();
        }

        public bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }

    internal class BurningLibrarian
    {
        private FirebaseClient GetFireBase()
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
            var Client = GetFireBase();
            FirebaseResponse response = Client.Get("God");
            var chats = response.ResultAs<Dictionary<long, string>>() ?? new Dictionary<long, string>();

            return chats.Select(god => (god.Value, god.Key)).ToList();
        }

        public async void AddPerson(string name, long id)
        {
            var client = GetFireBase();
            await client.SetAsync($"God/{id}", name);
        }

        public async void RemovePerson(long id)
        {
            var client = GetFireBase();
            await client.DeleteAsync($"God/{id}");
        }
    }
}
