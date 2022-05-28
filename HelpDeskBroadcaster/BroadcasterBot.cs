using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Viber.Bot;

namespace HelpDeskBroadcaster
{
    public class BroadcasterBot : INotifyPropertyChanged
    {
        public enum eMessengerIDFromFields
        {
            e_ID_Viber = 8,
            e_ID_Telegram = 7
        };

        private readonly TelegramBotClient m_telegramBot;
        private readonly ViberBotClient m_viberBot;

        private int _countOfMessages = 1; //
        private int _progress = 0;

        public int CountMessages
        {
            get { return _countOfMessages; }
            set
            {
                if (_countOfMessages != value)
                {
                    _countOfMessages = value;
                    OnPropertyChanged("CountMessages");
                }
            }
        }
        public int Progress
        {
            get { return _progress; }
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged("Progress");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public BroadcasterBot()
        {
            m_telegramBot = new TelegramBotClient(ConfigurationManager.AppSettings["TelegramBotToken"].ToString());
            m_viberBot = new ViberBotClient(ConfigurationManager.AppSettings["ViberBotToken"].ToString());
        }

        public async Task SendAllAsync(List<OrganizationWithUsers> organizations, string text, eMessengerIDFromFields eFlags)
        {
            var viberTokens = organizations.GetUniqueTokens(eMessengerIDFromFields.e_ID_Viber);
            var telegramTokens = organizations.GetUniqueTokens(eMessengerIDFromFields.e_ID_Telegram);

            CountMessages = 0;
            if ((eFlags & eMessengerIDFromFields.e_ID_Telegram) == eMessengerIDFromFields.e_ID_Telegram)
                CountMessages += telegramTokens.Count;

            if ((eFlags & eMessengerIDFromFields.e_ID_Viber) == eMessengerIDFromFields.e_ID_Viber)
                CountMessages += viberTokens.Count;

            if ((eFlags & eMessengerIDFromFields.e_ID_Telegram) == eMessengerIDFromFields.e_ID_Telegram)
            {
                foreach (var userID in telegramTokens)
                {
                    try
                    {
                        await m_telegramBot.SendTextMessageAsync(userID, text);
                        Thread.Sleep(200);
                        ++Progress;
                    }
                    catch (Telegram.Bot.Exceptions.ApiRequestException ex) { continue; }
                }
            }

            if ((eFlags & eMessengerIDFromFields.e_ID_Viber) == eMessengerIDFromFields.e_ID_Viber)
            {
                while (viberTokens.Count > 0)
                {
                    var count = viberTokens.Count < 50 ? viberTokens.Count : 50; // viber broadcastMessage can send 50 messages at time!
                    var br = new BroadcastMessage() { BroadcastList = viberTokens.GetRange(0, count), Text = text };
                    try
                    {
                        await m_viberBot.SendBroadcastMessageAsync(br);
                        Progress += br.BroadcastList.Count;
                    }
                    catch (Viber.Bot.ViberRequestApiException ex) { }
                    finally { viberTokens.RemoveRange(0, count); }
                }
            }
        }
    }
}
