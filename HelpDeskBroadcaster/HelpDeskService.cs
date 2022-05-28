using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace HelpDeskBroadcaster
{
    public class HelpDeskService
    {
        private string m_sKeyAPI { get; set; }
        private string m_sRequestAPI { get; set; }

        private readonly List<OrganizationWithUsers> _organizationsList = new();

        private int _currentPage;

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
            //var pathFile = Directory.GetCurrentDirectory(); // test
            //pathFile = pathFile.Remove(pathFile.IndexOf("bin"));//
            //pathFile = pathFile.Insert(pathFile.Length, "users.json");//
            //var text = File.ReadAllText(pathFile);
            if (m_sKeyAPI == null || m_sRequestAPI == null)
            {
                return _organizationsList;
            }

            string html = string.Empty;
            {
                string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(m_sKeyAPI));
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(m_sRequestAPI);
                request.Headers.Add("Authorization", "Basic " + svcCredentials);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }
            }

            //GET request
            var returnFileFromAPI = JsonConvert.DeserializeObject<DataRoot>(html);
            if (returnFileFromAPI != null)
            {
                _currentPage = returnFileFromAPI.pagination.current_page;
                ReadPage(returnFileFromAPI);
                _currentPage++;
                while (_currentPage <= returnFileFromAPI.pagination.total_pages)
                {
                    string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(m_sKeyAPI));
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(m_sRequestAPI + "?page=" + _currentPage);
                    request.Headers.Add("Authorization", "Basic " + svcCredentials);
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        html = reader.ReadToEnd();
                    }
                    returnFileFromAPI = JsonConvert.DeserializeObject<DataRoot>(html);
                    ReadPage(returnFileFromAPI);
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

                if (!_organizationsList.Exists(x => x.m_organizationID == user.organization.id))
                    _organizationsList.Add(new OrganizationWithUsers() { m_organizationID = user.organization.id, m_organizationName = user.organization.name });

                var org = _organizationsList.Find(x => x.m_organizationID == user.organization.id);
                if (org == null)
                    continue;

                foreach (var messenger in user.custom_fields)
                {
                    if ((messenger.id == (int)BroadcasterBot.eMessengerIDFromFields.e_ID_Telegram) 
                        && (messenger.field_value.ToString().Length > 0)
                        && (org._telegramUsersToken.Find(x => x == messenger.field_value.ToString()) == null))
                        org._telegramUsersToken.Add(messenger.field_value.ToString());

                    if ((messenger.id == (int)BroadcasterBot.eMessengerIDFromFields.e_ID_Viber)
                        && (messenger.field_value.ToString().Length > 0)
                        && (org._viberUsersToken.Find(x => x == messenger.field_value.ToString()) == null))
                        org._viberUsersToken.Add(messenger.field_value.ToString());
                }
            }
        }
    }
}
