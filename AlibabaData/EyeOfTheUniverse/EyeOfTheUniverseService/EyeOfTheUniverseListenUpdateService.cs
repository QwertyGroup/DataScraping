using EyeOfTheUniverseCore;

using System;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace EyeOfTheUniverseService
{
    public partial class EyeOfTheUniverseListenUpdateService : ServiceBase
    {
        public EyeOfTheUniverseListenUpdateService()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        private EyeApi _api;
        protected override async void OnStart(string[] args)
        {
            _api = new EyeApi();
            _api.OnMessage += OnMessgeReceived;
            _api.StartListening();
            while (!_api.CheckForInternetConnection())
                await Task.Delay(TimeSpan.FromSeconds(5));
            _api.SpreadMessage($"Listening service is On now.{Environment.NewLine}But you will never know when it stops.");
        }

        (long id, string name, bool flag) _NewGod = (0, string.Empty, false);
        private void OnMessgeReceived(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Text == "Add me to Gods")
            {
                if (_NewGod.flag) throw new Exception("Gods overflow");
                var id = e.Message.Chat.Id;
                _api.SendMessageAsync(e.Message.Chat.Id, "Give me your name, bro!");
                _NewGod.id = id;
                _NewGod.flag = true;
                return;
            }

            if (_NewGod.flag)
            {
                _NewGod.name = e.Message.Text;

                _api.AddNewGod(_NewGod);
                _api.SendMessageAsync(_NewGod.id, "My regards.");

                _NewGod = (0, string.Empty, false);
                return;
            }

            // YOU HAVE TO BE GOD NOW!
            if (!_api.GetGods().Select(god => god.ID).ToList().Contains(e.Message.Chat.Id))
            {
                _api.SendMessageAsync(e.Message.Chat.Id, "Sry, but you are not a God.");
                return;
            }

            if (e.Message.Text == "Remove me" || e.Message.Text == "/remove_me")
            {
                var id = e.Message.Chat.Id;
                _api.RemoveGod(id);
                _api.SendMessageAsync(id, "You are no longer in Gods party.");
                return;
            }

            if (e.Message.Text == "View Gods" || e.Message.Text == "/view_gods")
            {
                var gods = _api.GetGods();
                var msg = $"Count: {gods.Count}{Environment.NewLine}";
                msg += gods.Select(god => $"{god.Name}{Environment.NewLine}").Aggregate((acc, god) => acc += god);
                _api.SendMessageAsync(e.Message.Chat.Id, msg);
                return;
            }
        }

        protected override void OnStop()
        {
            _api.StopListening();
        }
    }
}
