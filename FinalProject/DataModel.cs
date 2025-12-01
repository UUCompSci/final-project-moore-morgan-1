using Microsoft.EntityFrameworkCore;
using Entities;

public class World : DbContext
{
    public DbSet<Person> People {get;set;}
    public DbSet<House> Houses{get;set;}
    public DbSet<Car> Cars{get;set;}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string path = Path.Combine(Environment.CurrentDirectory, "World.db");
        string connection = $"Filename={path}";

        Console.WriteLine($"Connection: {connection}");
        optionsBuilder.UseSqlite(connection);
    }
}