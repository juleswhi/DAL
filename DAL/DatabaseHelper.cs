namespace CourseworkUtils.DAL;

public static class ModelHelper
{
    public static IEnumerable<(string, int)> GetPrimaryKey<T>(this T model) where T : IDatabaseModel
    {
        foreach (var prop in model.GetType().GetProperties())
        {
            var attr = Attribute.GetCustomAttribute(prop, typeof(PrimaryKey));
            if (attr is null) continue;
            yield return (prop.Name, (int)prop.GetValue(model)!);
        }
    }

    public static string FormatKeys<T>(this T model) where T : IDatabaseModel =>
        model.GetPrimaryKey().Select(x => $"{x.Item1} = '{x.Item2}'").Aggregate((x, y) => $"{x} AND {y}");
}
