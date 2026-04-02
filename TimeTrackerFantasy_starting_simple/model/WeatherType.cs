namespace TimeTrackerFantasy_starting_simple.model
{
    public class WeatherType
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public string Name { get; }
        public string Tempclass { get; }
        WeatherTable? WeatherTable { get; set; }
        public string Description { get; }

        public WeatherType(string name, string tempclass, string description, int orderid, int typeid = -1)
        {
            Id = typeid;
            OrderId = orderid;
            Name = name;
            Tempclass = tempclass;
            Description = description;
        }

        public WeatherType(string name, string tempclass, WeatherTable weatherTable, string description, int orderid, int typeid = -1)
        {
            Id = typeid;
            OrderId = orderid;
            Name = name;
            Tempclass = tempclass;
            WeatherTable = weatherTable;
            Description = description;
        }
    }
}
