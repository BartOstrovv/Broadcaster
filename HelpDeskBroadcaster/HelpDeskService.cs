using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace HelpDeskBroadcaster
{
    public class HelpDeskService
    {
        private string m_sKeyAPI { get; set; }
        private string m_sRequestAPI { get; set; }

        private List<OrganizationWithUsers> _organizationsList = new List<OrganizationWithUsers>();

        private int _currentPage = 0;

        public HelpDeskService()
        {
        }

        public bool SetCurrentUser(Tuple<string, string> sval)
        {
            m_sKeyAPI = sval.Item1;
            m_sRequestAPI = sval.Item2;
            return true;
            // if not found or bad request return false;
        }

        public List<OrganizationWithUsers> GetClientsData()
        {
            var pathFile = Directory.GetCurrentDirectory(); // test
            pathFile = pathFile.Remove(pathFile.IndexOf("bin"));//
            pathFile = pathFile.Insert(pathFile.Length, "users.json");//
            var text = File.ReadAllText(pathFile);
            //GET request
            var returnFileFromAPI = JsonConvert.DeserializeObject<DataRoot>(text);
            if (returnFileFromAPI != null)
            {
                _currentPage = returnFileFromAPI.pagination.current_page;
                ReadPage(returnFileFromAPI);
                _currentPage++;
                while (_currentPage <= returnFileFromAPI.pagination.total_pages)
                {
                    var nextPage = pathFile.Insert(pathFile.IndexOf('.'), _currentPage.ToString());

                    ReadPage(JsonConvert.DeserializeObject<DataRoot>(File.ReadAllText(nextPage)));
                    _currentPage++;
                }
            }
            return _organizationsList;
        }

        public void ReadPage(DataRoot page)
        {
            if (page == null)
                return;

            foreach (var user in page.data)
            {
                if (user.organization == null)
                    continue;

                if (!_organizationsList.Exists(x => x.OrganizatioId == user.organization.id))
                    _organizationsList.Add(new OrganizationWithUsers() { OrganizatioId = user.organization.id, OrganizationName = user.organization.name });

                var org = _organizationsList.Find(x => x.OrganizatioId == user.organization.id);
                if (org == null)
                    continue;

                foreach (var messenger in user.custom_fields)
                {
                    if ((messenger.id == (int)BroadcasterBot.eMessengerIDFromFields.e_ID_Telegram) &&
                        (org.TelegramUsersToken.Find(x => x == messenger.field_value.ToString()) == null))
                        org.TelegramUsersToken.Add(messenger.field_value.ToString());

                    if ((messenger.id == (int)BroadcasterBot.eMessengerIDFromFields.e_ID_Viber) &&
                         (org.ViberUsersToken.Find(x => x == messenger.field_value.ToString()) == null))
                        org.ViberUsersToken.Add(messenger.field_value.ToString());
                }
            }
        }
    }
}
