using Microsoft.Data.Sqlite;

namespace DbConnection
{
    /// <summary>
    /// ORDER OF OPERATIONS FOR DB TRANSACTIONS IN .NET
    /// 1. Set up connection (using)
    /// 2. Open connection (connection.Open())
    /// 3. Set up command 
    /// 4. Set any params (if necessary)
    /// 5. Execute command
    /// </summary>
    public class Program
    {
        private static string connString = $"Data Source={FileRoot.root}{Path.DirectorySeparatorChar}data{Path.DirectorySeparatorChar}test.db";
        private static string root = FileRoot.root;

        public static void Main(string[] args)
        {
            //ReadUser();
            //ClearPokemonFromDb();
            //AddPokemonListToDb();
            var twoTypes = ReadPokemonFromDb();
            WriteList(twoTypes);

        }

        // read a user by their id
        static void ReadUser()
        {
            Console.Write("Enter the user ID you want to search for: ");
            int enteredId = int.Parse(Console.ReadLine());

            // this sets up the connection to test.db
            using (var connection = new SqliteConnection(connString))
            {
                // open the connection
                connection.Open();

                // create the command
                var command = connection.CreateCommand();

                // add text to the command
                command.CommandText =
                @"
                    SELECT First_Name, Last_Name
                    FROM Users
                    WHERE id = $id;
                ";

                // command parameters
                command.Parameters.AddWithValue("$id", enteredId);

                try
                {
                    using (var reader = command.ExecuteReader()) 
                    {
                        while(reader.Read())
                        {
                            Console.WriteLine($"The user's name that you selected is {reader.GetString(0)} {reader.GetString(1)}");
                        }
                    }
                } 
                catch (Exception e) 
                {
                    Console.ForegroundColor= ConsoleColor.Red;
                    Console.WriteLine(e);
                }
            }
        }

        static List<Pokemon> ReadPokemonFromCSV()
        {
            var pokemonList = new List<Pokemon>();

            var file = root + "\\data" + "\\pokemon.csv";

            using(var sr = new StreamReader(file)) 
            {
                while(!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var data = line.Split(",");

                    var dexNum = int.Parse(data[0]);
                    var name = data[1];
                    var level = int.Parse(data[2]);
                    var pType = data[3];
                    var sType = data[4];

                    pokemonList.Add(new Pokemon(dexNum, name, level, pType, sType));
                }
            }

            return pokemonList;
        }

        static List<Pokemon> ReadPokemonFromDb()
        {
            using(var connection = new SqliteConnection(connString))
            {
                connection.Open();
                List<Pokemon> twoTypes = new List<Pokemon>();
                var command = connection.CreateCommand();

                command.CommandText = @"SELECT * FROM Pokemon WHERE Secondary_Type is not ''";

                using(var reader = command.ExecuteReader()) 
                {

                    while(reader.Read()) 
                    {
                        var dexNum = reader.GetInt32(0);
                        var name = reader.GetString(1);
                        var level = reader.GetInt32(2);
                        var pType = reader.GetString(3);
                        var sType = reader.GetString(4);

                        twoTypes.Add(new Pokemon(dexNum, name, level, pType, sType));
                    }
                }
                return twoTypes;
            }
        }

        static void WriteList(List<Pokemon> pokemonList)
        {
            var file = root + "\\data" + "\\writelist.csv";
            using(var sw = new StreamWriter(file)) 
            { 
                foreach(var pokemon in pokemonList) 
                {
                    sw.WriteLine(pokemon);
                }
            }
        }

        static void AddPokemonToDb(Pokemon p)
        {
            // set up connection
            using(var connection = new SqliteConnection(connString))
            {
                connection.Open();

                // this makes our code ~pretty
                var cmdText = @"INSERT INTO Pokemon VALUES ($num, $name, $lvl, $pt, $st)";

                using(var command = new SqliteCommand(cmdText, connection)) 
                {
                    // optional -- but it may help with faster transaction time
                    command.CommandType = System.Data.CommandType.Text;

                    // add in parameters with values
                    command.Parameters.AddWithValue("$num", p.DexNumber);
                    command.Parameters.AddWithValue("$name", p.Name);
                    command.Parameters.AddWithValue("$lvl", p.Level);
                    command.Parameters.AddWithValue("$pt", p.PrimaryType);
                    command.Parameters.AddWithValue("$st",p.SecondaryType);

                    // we have the command, now let's run it
                    // remember, nonquerys are non-select read transactions
                    // e.g., insert, update, delete, etc.

                    command.ExecuteNonQuery();
                }
            }
        }

        static void AddPokemonListToDb()
        {
            var pokemonList = ReadPokemonFromCSV();
            foreach(var pokemon in pokemonList)
            {
                AddPokemonToDb(pokemon);
            }
        }

        static void ClearPokemonFromDb()
        {
            using(var connection = new SqliteConnection(connString))
            {
                connection.Open();

                using(var command = new SqliteCommand(@"DELETE FROM pokemon;", connection))
                {
                    command.CommandType= System.Data.CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}