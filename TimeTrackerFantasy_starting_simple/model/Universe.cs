namespace TimeTrackerFantasy_starting_simple.model
{
    public class Universe
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public WeatherTable CurrentWeatherTable { get; set; }
        public WeatherType CurrentWeatherType { get; set; }
        public int SimpleTime { get; set; } = 0;
        public String Description { get; set; } = "";

        public Universe(string name, WeatherTable currentWeatherTable, WeatherType currentWeatherType, int simpleTime = 0, string description = "", int id = -1)
        {
            Id = id;
            Name = name;
            SimpleTime = simpleTime;
            CurrentWeatherTable = currentWeatherTable;
            CurrentWeatherType = currentWeatherType;
            Description = description;
        }
    }
}
