using Microsoft.Data.SqlClient;

namespace CourseworkUtils.DAL;
public static class DatabaseLayer
{
    /// <summary>
    /// Connection String for database
    /// </summary>
    public static string ConnectionString { get; set; } = "";

    /// <summary>
    /// Query a database via class (and with selected column names)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="names"></param>
    /// <returns></returns>
    public static List<T> Query<T>() where T : IDatabaseModel
    {
        if(ConnectionString == "")
        {
            throw new Exception("Please populate the ConnectionString property");
        }

        var type = typeof(T);

        using var conn = new SqlConnection($"{ConnectionString}");
        conn.Open();
        using var command = new SqlCommand($"select * from {type.Name} ;", conn);

        // Grab results from query
        using var reader = command.ExecuteReader();

        List<T> results = [];

        // This will loop through all rows in query
        while (reader.Read())
        {
            // Create instance of the DatabaseModel
            T obj = Activator.CreateInstance<T>();
            foreach (var property in type.GetProperties())
            {
                // Grab the column cooresponding to the name
                var ord = reader.GetOrdinal(property.Name);

                var t = property.PropertyType;

                // Properly cast to type based on SQL type
                switch (property.PropertyType.Name)
                {
                    case "Int32":
                    case "System.Int32":
                        var num = reader.GetInt32(ord);
                        var prop = type.GetProperty(property.Name);
                        if (prop is null)
                            break;
                        prop.SetValue(obj, num);
                        break;
                    case "Double":
                        var doub = reader.GetInt32(ord);
                        var p = type.GetProperty(property.Name);
                        if (p is null)
                            break;
                        p.SetValue(obj, doub);
                        break;
                    case "DateTime":
                        var dt = reader.GetDateTime(ord);
                        type.GetProperty(property.Name)!.SetValue(obj, dt);
                        break;
                    case "Boolean":
                        var b = reader.GetBoolean(ord);
                        type.GetProperty(property.Name)!.SetValue(obj, b);
                        break;
                    default:
                        var str = reader.GetString(ord);
                        type.GetProperty(property.Name)!.SetValue(obj, str);
                        break;
                };
            }
            results.Add(obj);
        }

        conn.Close();

        return results;
    }


    /// <summary>
    /// Update SQL statement
    /// </summary>
    /// <typeparam name="T">The type of DatabaseModel to update</typeparam>
    /// <param name="obj">The instance of DatabaseModel to update</param>
    /// <returns></returns>
    public static bool Update<T>(this T obj) where T : IDatabaseModel
    {
        var type = typeof(T);

        using var conn = new SqlConnection($"{ConnectionString}");
        conn.Open();

        string updates = type.GetProperties().Select(x => x.Name)
            .Zip(type.GetProperties()
                .Select(x => x.GetValue(obj))
                .Select(y => y == null ? "" : y.ToString()))
            .Select((c, _) => $"{c.First} = '{c.Second}'")
            .Aggregate((x, y) => $"{x}, {y}");

        using var command = new SqlCommand($"update {type.Name} set {updates} where {obj.FormatKeys()};", conn);
        int res = command.ExecuteNonQuery();

        conn.Close();

        return res == 0;
    }

    public static bool Delete<T>(this T obj) where T : IDatabaseModel
    {
        var type = typeof(T);

        using var conn = new SqlConnection($"{ConnectionString}");
        conn.Open();

        using var command = new SqlCommand($"delete from {type.Name} where {obj.FormatKeys()};", conn);
        return command.ExecuteNonQuery() == 0;
    }

    public static bool Create<T>(this T obj) where T : IDatabaseModel
    {
        var type = typeof(T);
        using var conn = new SqlConnection($"{ConnectionString}");
        conn.Open();

        var props = type.GetProperties();
        var names = props.Select(x => x.Name);
        var aggr = names.Aggregate((x, y) => $"{x}, {y}");
        string vals = type.GetProperties().
            Select(x => x.GetValue(obj)).
            Select(x => x!.ToString()).
            Select(x => x!.Replace("'", "''")).
            Aggregate((x, y) => $"{x}, '{y}'")!;

        using var command = new SqlCommand($"insert into {type.Name} ({aggr}) values ({vals})", conn);

        return command.ExecuteNonQuery() == 0;
    }
}
