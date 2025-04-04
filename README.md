# Coursework DAL

Ergonmic DAL for Microsoft SQL Server

## Download



## Usage

First, you need to create models of your tables

In your model classes, implement the `CourseworkUtils.DAL.IDatabaseModel` interface

```cs
class Employee : IDatabaseModel {}
```

Next, label any primary or foreign keys with the corresponding attribute

```cs
[PrimaryKey]
public int Id { get; set; }

[ForeignKey(User)]
public int UserId { get; set; }
```

To query your Database, call the methods from the `DatabaseLayer` class

```cs
List<Employee> result = DatabaseLayer.Query<Employee>();
```
