namespace CourseworkUtils.DAL;

[AttributeUsage(AttributeTargets.Property)]
internal class PrimaryKey : Attribute { }

[AttributeUsage(AttributeTargets.Property)]
internal class ForeignKeyAttribute : Attribute {
    public Type Type { get; set; } = typeof(IDatabaseModel);
    public ForeignKeyAttribute(Type type) {
        Type = type;
    }
}
