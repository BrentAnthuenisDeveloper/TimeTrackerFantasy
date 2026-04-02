namespace TimeTrackerFantasy_starting_simple.model
{
    public class WeatherTable
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public List<WeatherType> Weathertypes { get; set; }
        public string Description { get; set; }

        public Dictionary<string, double> chances { get; set; } = new Dictionary<string, double>()
        {
            ["random"] = 10,
            ["+1"] = 40,
            ["even"] = 70,
            ["-1"] = 100,
        };
        public WeatherTable()
        {
            Id = -1;
            Name = string.Empty;
            Weathertypes = new List<WeatherType>();
            Description = string.Empty;
        }

        public WeatherTable(string name, List<WeatherType> weathertypes, string description, int id = -1)
        {
            Id = id;
            Name = name;
            Weathertypes = weathertypes;
            Description = description;
            Description = description;
        }

        public WeatherTable(string weatherTableName, string weatherTableDescription, int id = -1)
        {
            Id = id;
            Name = weatherTableName;
            Description = weatherTableDescription;
            Weathertypes = new List<WeatherType>();

        }

        public WeatherType RollWeather()
        {
            Random random = new Random();
            int roll = random.Next(0, Weathertypes.Count);
            return Weathertypes[roll];
        }
        public string rollChance()
        {
            Random rnd = new Random();
            double roll = rnd.Next(1, 101);
            string result = chances.First((chance) => chance.Value > roll).Key;
            return result;
        }
    }
}

