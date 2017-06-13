using EyeOfTheUniverseCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
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
        protected override void OnStart(string[] args)
        {
            _api = new EyeApi();
            _api.OnMessage += OnMessgeReceived;
            _api.StartListening();
            _api.SpreadMessage("Listening service: ON.");
        }

        (long id, string name, bool flag) _NewGod = (0, string.Empty, false);
        private void OnMessgeReceived(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Text == "Add me to Gods")
            {
                if (_NewGod.flag) throw new Exception("Gods overflow");
                var id = e.Message.Chat.Id;
                _api.SendMessage(e.Message.Chat.Id, "Give me your name, bro!");
                _NewGod.id = id;
                _NewGod.flag = true;
                return;
            }

            if (_NewGod.flag)
            {
                _NewGod.name = e.Message.Text;

                _api.AddNewGod(_NewGod);

                _NewGod = (0, string.Empty, false);
                return;
            }
        }

        protected override void OnStop()
        {
            _api.SpreadMessage("Listening service: OFF.");
            _api.StopListening();
        }
    }
}
