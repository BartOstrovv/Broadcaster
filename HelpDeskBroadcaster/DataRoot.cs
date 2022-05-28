using System.Collections.Generic;

namespace HelpDeskBroadcaster
{
// Root myDeserializedClass = JsonConvert.DeserializeObject<DataRoot>(myJsonResponse);
public class Name
{
    public string ua { get; set; }
    public string ru { get; set; }
    public string en { get; set; }
}

public class Organization
{
    public string id { get; set; }
    public string name { get; set; }
    public string domains { get; set; }
    public string address { get; set; }
    public string phone { get; set; }
    public string email { get; set; }
    public string web { get; set; }
}

public class Group
{
    public int id { get; set; }
    public string type { get; set; }
    public Name name { get; set; }
    public int disable { get; set; }
}

public class CustomField
{
    public int id { get; set; }
    public string field_type { get; set; }
    public object field_value { get; set; }
}

public class Datum
{
    public int id { get; set; }
    public string date_created { get; set; }
    public string date_updated { get; set; }
    public string name { get; set; }
    public string lastname { get; set; }
    public string email { get; set; }
    public string phone { get; set; }
    public string skype { get; set; }
    public string website { get; set; }
    public Organization organization { get; set; }
    public string status { get; set; }
    public string language { get; set; }
    public string notifications { get; set; }
    public Group group { get; set; }
    public object ldap_username { get; set; }
    public string user_status { get; set; }
    public List<int> department { get; set; }
    public List<CustomField> custom_fields { get; set; }
    public string api_key { get; set; }
}

public class Pagination
{
    public int total { get; set; }
    public int per_page { get; set; }
    public int current_page { get; set; }
    public int total_pages { get; set; }
}

public class DataRoot
{
    public List<Datum> data { get; set; }
    public Pagination pagination { get; set; }
}
}
