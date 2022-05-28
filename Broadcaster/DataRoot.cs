// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
using System.Collections.Generic;

public class CustomField
{
    public int id { get; set; }
    public string field_type { get; set; }
    public string field_value { get; set; }
}

public class Datum
{
    public int id { get; set; }
    public string date_created { get; set; }
    public string date_updated { get; set; }
    public string name { get; set; }
    public string organization { get; set; }
    public string lastname { get; set; }
    public string email { get; set; }
    public string phone { get; set; }
    public List<CustomField> custom_fields { get; set; }
}

public class Pagination
{
    public int total { get; set; }
    public int per_page { get; set; }
    public int current_page { get; set; }
    public int total_pages { get; set; }
}

public class Root
{
    public List<Datum> data { get; set; }
    public Pagination pagination { get; set; }
}

