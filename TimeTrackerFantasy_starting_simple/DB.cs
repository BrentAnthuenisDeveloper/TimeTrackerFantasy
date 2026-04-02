using Microsoft.Data.Sqlite;
using TimeTrackerFantasy_starting_simple.model;

namespace TimeTrackerFantasy_starting_simple
{
    public class DB
    {
        private String source { get; set; } = "TimeTracker.sqllite";
        private String fullpath { get; }
        SqliteConnection connection { get; set; }
        public List<WeatherTable> WeatherTables { get; set; }
        public List<WeatherType> WeatherTypes { get; set; }
        public List<Universe> Universes { get; set; }
        public Universe? CurrentUniverse { get; set; }

        public DB()
        {
            fullpath = Path.GetFullPath(source);
            string connectionString = $"Data Source={source}";
            connection = new SqliteConnection(connectionString);
            DeleteTables();
            CreateDB();
            WeatherTables = new List<WeatherTable>();
            WeatherTypes = new List<WeatherType>();
            Universes = new List<Universe>();
            Seed();
            loadValues();
        }
        public void loadValues()
        {
            //weathertables
            List<WeatherType> weathertypes = new List<WeatherType>();
            List<WeatherTable> weathertables = new List<WeatherTable>();

            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT wty.id, wty.orderid, wty.name, temperature_class, wty.description, wta.id,wta.name, wta.description
                FROM weathertype wty inner join weathertable wta on wty.tableId = wta.id
                """;

            using (SqliteDataReader? reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int ordernumber = 0;

                    int typeid = reader.GetInt32(ordernumber);
                    ordernumber += 1;
                    int orderid = reader.GetInt32(1);
                    ordernumber += 1;
                    string weatherTypeName = reader.GetString(ordernumber);
                    ordernumber += 1;
                    string tempclass = reader.GetString(ordernumber);
                    ordernumber += 1;
                    string weatherTypeDescription = reader.GetString(ordernumber);
                    ordernumber += 1;
                    int tableid = reader.GetInt32(ordernumber);
                    ordernumber += 1;
                    string weatherTableName = reader.GetString(ordernumber);
                    ordernumber += 1;
                    string weatherTableDescription = reader.GetString(ordernumber);


                    WeatherTable? loadingWeatherTable = weathertables.Find(wt => wt.Name == weatherTableName);
                    WeatherType loadingweathertype = new WeatherType(weatherTypeName, tempclass, weatherTypeDescription, orderid, typeid);

                    if (loadingWeatherTable == null)
                    {
                        loadingWeatherTable = new WeatherTable(weatherTableName, weatherTableDescription, tableid);
                        loadingWeatherTable.Weathertypes.Add(loadingweathertype);
                        weathertables.Add(loadingWeatherTable);
                    }
                    else loadingWeatherTable.Weathertypes.Add(loadingweathertype);

                    weathertypes.Add(loadingweathertype);

                }
            }
            WeatherTables = weathertables;
            WeatherTypes = weathertypes;
            //universes
            List<Universe> universes = new List<Universe>();
            command.CommandText = """
                SELECT id, name, currentweathertableid, currentweathertypeid, simpletime, description
                FROM universe
                """;
            using (SqliteDataReader? reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    WeatherTable currentweathertable = weathertables.First(wt => wt.Id == reader.GetInt32(2));
                    WeatherType currentweathertype = WeatherTypes.First(wt => wt.Id == reader.GetInt32(3));
                    int simpletime = reader.GetInt32(4);
                    string description = reader.GetString(5);

                    Universe loadingUniverse = new Universe(name, currentweathertable, currentweathertype, simpletime, description, id);

                    universes.Add(loadingUniverse);

                }
            }
            connection.Close();
            Universes = universes;
        }
        public Universe insertUniverse(Universe universe)
        {
            if (!Universes.Any(u => u.Name == universe.Name))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = """
                INSERT INTO universe (name, currentweathertableid,currentweathertypeid,simpletime, description) VALUES ($name, $currentWeatherTable, $currentWeatherType, $simpletime, $description);
                select last_insert_rowid();
                """;
                command.Parameters.AddWithValue("$name", universe.Name);
                command.Parameters.AddWithValue("$currentWeatherTable", universe.CurrentWeatherTable.Id);
                command.Parameters.AddWithValue("$currentWeatherType", universe.CurrentWeatherType.Id);
                command.Parameters.AddWithValue("$simpletime", universe.SimpleTime);
                command.Parameters.AddWithValue("$description", universe.Description);
                long? tableId = (long)command.ExecuteScalar();
                connection.Close();
                loadValues();
                universe.Id = (int)tableId;
                return universe;
            }
            return universe;
        }
        public WeatherTable insertWeatherTable(WeatherTable weatherTable)
        {
            if (!WeatherTables.Any(wt => wt.Name == weatherTable.Name))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = """
                INSERT INTO weathertable (name, description) VALUES ($name, $description);
                SELECT last_insert_rowid();
                """;
                command.Parameters.AddWithValue("$name", weatherTable.Name);
                command.Parameters.AddWithValue("$description", weatherTable.Description);
                long tableId = (long)command.ExecuteScalar();

                foreach (var weatherType in weatherTable.Weathertypes)
                {
                    var typeCommand = connection.CreateCommand();
                    typeCommand.CommandText = """
                    INSERT OR REPLACE INTO weathertype (name,orderid, description, temperature_class, tableId) VALUES ($name,$orderid, $description, $tempclass, $tableId);
                    """;
                    typeCommand.Parameters.AddWithValue("$name", weatherType.Name);
                    typeCommand.Parameters.AddWithValue("$description", weatherType.Description);
                    typeCommand.Parameters.AddWithValue("$tempclass", weatherType.Tempclass);
                    typeCommand.Parameters.AddWithValue("$orderid", weatherType.OrderId);
                    typeCommand.Parameters.AddWithValue("$tableId", tableId);
                    typeCommand.ExecuteNonQuery();
                }
                connection.Close();
                loadValues();
                return weatherTable;
            }
            return weatherTable;
        }
        public void Seed()
        {
            WeatherType[] weathertypes = {
                new WeatherType("Sunny", "Warm", "A bright and sunny day.", 0),
                new WeatherType("Rainy", "Cold", "A wet and rainy day.", -1),
                new WeatherType("Snowy", "Cold", "A snowy and cold day.", -2)
            };
            WeatherTable weatherTable = new WeatherTable("Default Weather Table", weathertypes.ToList(), "A default weather table for testing.");

            insertWeatherTable(weatherTable);
            loadValues();
            Universe universe = new Universe("testuniverse", WeatherTables[0], WeatherTables[0].Weathertypes[0]);
            insertUniverse(universe);
        }
        public void DeleteTables()
        {
            //delete tables
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = """
                drop table if exists universe;
                drop table if exists weathertype;
                drop table if exists weathertable;
                """;
            command.ExecuteNonQuery();
            connection.Close();
        }
        public void CreateDB()
        {


            //create tables
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
             @"
             CREATE TABLE IF NOT EXISTS weathertable (
                id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL UNIQUE,
                description TEXT);
             CREATE TABLE IF NOT EXISTS weathertype(
                id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                orderid INTEGER NOT NULL,
                name TEXT NOT NULL,
                description TEXT,
                temperature_class TEXT NOT NULL,
                tableId INTEGER NOT NULL,
                FOREIGN KEY(tableId) REFERENCES weathertable(id)
                );
             CREATE TABLE IF NOT EXISTS universe (
                id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL,
                description TEXT,
                simpletime integer,
                currentweathertableid INTEGER,
                currentweathertypeid integer,
                Foreign key(currentweathertableid) references weathertable(id),
                Foreign key(currentweathertypeid) references weathertype(id)
                );
             ";

            command.ExecuteNonQuery();
            connection.Close();
            Console.WriteLine("Table created.");
        }

    }
}
