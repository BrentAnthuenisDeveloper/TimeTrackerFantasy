using TimeTrackerFantasy_starting_simple.model;

namespace TimeTrackerFantasy_starting_simple
{
    internal class Program
    {
        public delegate string CommandFunction(DB db);
        static void Main(string[] args)
        {
            DB database = new DB();
            SelectUniverse(database);

            Dictionary<string, CommandFunction> possibleCommands = new Dictionary<string, CommandFunction>()
            {
                ["selectuniverse"] = SelectUniverse,
                ["advance"] = AdvanceInTime,
                ["exit"] = Exit,
            };
            bool commandloop = true;
            while (commandloop)
            {
                Console.WriteLine("geef een commando");
                string? input = Console.ReadLine();

                CommandFunction? commandToExecute = possibleCommands.FirstOrDefault(x => x.Key == input).Value;
                string response = "";
                if (commandToExecute == null) Console.WriteLine("please give a valid command"); else response = commandToExecute(database);
                if (response == "exit") commandloop = false;
            }
        }
        private static string AdvanceInTime(DB db)
        {
            Console.WriteLine($"advancing time by 1");
            string rolledchance = db.CurrentUniverse.CurrentWeatherTable.rollChance();
            WeatherType rolledweather = db.CurrentUniverse.CurrentWeatherTable.RollWeather();

            switch (rolledchance)
            {
                case "random" when rolledchance == "random":
                    Console.WriteLine("rolling a random weather");
                    rolledweather = db.CurrentUniverse.CurrentWeatherTable.RollWeather();
                    break;
                case "+1" when rolledchance == "+1":
                    Console.WriteLine("advancing the weather by one");
                    WeatherType? newweather = db.CurrentUniverse.CurrentWeatherTable.Weathertypes.FirstOrDefault(w => w.OrderId == db.CurrentUniverse.CurrentWeatherType.OrderId + 1);
                    rolledweather = (newweather == null ? db.CurrentUniverse.CurrentWeatherType : newweather);
                    break;
                case "even" when rolledchance == "even":
                    Console.WriteLine("keeping the weather stable");
                    rolledweather = db.CurrentUniverse.CurrentWeatherType;
                    break;
                case "-1" when rolledchance == "-1":
                    Console.WriteLine("reducing the weather by one");
                    newweather = db.CurrentUniverse.CurrentWeatherTable.Weathertypes.FirstOrDefault(w => w.OrderId == db.CurrentUniverse.CurrentWeatherType.OrderId - 1);
                    rolledweather = (newweather == null ? db.CurrentUniverse.CurrentWeatherType : newweather);
                    break;
            }
            db.CurrentUniverse.SimpleTime += 1;
            db.CurrentUniverse.CurrentWeatherType = rolledweather;
            Console.WriteLine($"time is: {db.CurrentUniverse.SimpleTime}");
            Console.WriteLine($"the weather is {rolledweather.Name}");
            Console.WriteLine($"description: {rolledweather.Description}");
            return "100";
        }
        private static string SelectUniverse(DB db)
        {
            Universe? chosenUniverse = null;
            bool redo = false;
            do
            {
                Console.WriteLine("please choose a universe by inputting it's id");
                foreach (Universe u in db.Universes)
                {
                    Console.WriteLine($"\t{u.Id}-{u.Name}");
                }
                try
                {
                    int inputId = int.Parse(Console.ReadLine().ToLower().Trim());
                    chosenUniverse = db.Universes.FirstOrDefault(u => u.Id == inputId);
                    if (chosenUniverse == null) { Console.WriteLine("please enter a valid id"); redo = true; }
                }
                catch (Exception)
                {
                    Console.WriteLine("please enter a valid id");
                    redo = true;
                }
            } while (redo);
            db.CurrentUniverse = chosenUniverse;
            return "100";
        }
        private static string Exit(DB db)
        {
            return "exit";
        }
        private static void WriteWeatherTable(WeatherTable wt)
        {
            Console.WriteLine(wt.Name);
            Console.WriteLine(wt.Description);
            foreach (WeatherType wty in wt.Weathertypes)
            {
                Console.WriteLine($"\t{wty.Name}");
                Console.WriteLine($"\t{wty.Description}");
            }
        }
    }
}
