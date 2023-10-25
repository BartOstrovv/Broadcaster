using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Broadcaster
{
    public class RootService
    {
        private readonly List<OrganizationWithUsers> _organizationsList = new();

        private int _currentPage;
        public List<OrganizationWithUsers> GetUsersFromJSON() // change path to file
        {
            var pathFile = Directory.GetCurrentDirectory(); // test
            pathFile = pathFile.Remove(pathFile.IndexOf("bin"));//
            pathFile = pathFile.Insert(pathFile.Length, "users.json");//
            var text = File.ReadAllText(pathFile);
            //GET request
            var returnFileFromAPI = JsonConvert.DeserializeObject<Root>(text);
            if (returnFileFromAPI != null)
            {
                _currentPage = returnFileFromAPI.pagination.current_page;
                ReadPage(returnFileFromAPI);
                _currentPage++;
                while (_currentPage <= returnFileFromAPI.pagination.total_pages)
                {
                    var nextPage = pathFile.Insert(pathFile.IndexOf('.'), _currentPage.ToString());

                    ReadPage(JsonConvert.DeserializeObject<Root>(File.ReadAllText(nextPage)));
                    _currentPage++;
                }
            }
            return _organizationsList;
        }

        public void ReadPage(Root page)
        {
            if (page == null)
                return;

            foreach (var user in page.data)
            {
                if (user.organization == null)
                    continue;

                if (!_organizationsList.Exists(x => x.OrganizationName == user.organization))
                    _organizationsList.Add(new OrganizationWithUsers() { OrganizationName = user.organization });

                var org = _organizationsList.Find(x => x.OrganizationName == user.organization);
                if (org == null)
                    continue;

                foreach (var messenger in user.custom_fields)
                {
                    if ((messenger.id == (int)BroadcasterBot.EMessengerIDFromFields.e_ID_Telegram) &&
                        (org.TelegramUsersToken.Find(x => x == messenger.field_value.ToString()) == null))
                        org.TelegramUsersToken.Add(messenger.field_value.ToString());

                    if ((messenger.id == (int)BroadcasterBot.EMessengerIDFromFields.e_ID_Viber) &&
                         (org.ViberUsersToken.Find(x => x == messenger.field_value.ToString()) == null))
                        org.ViberUsersToken.Add(messenger.field_value.ToString());
                }
            }
        }
    }
}
