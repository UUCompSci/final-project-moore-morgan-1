using Microsoft.EntityFrameworkCore;
using Entities;

public class World : DbContext
{
    public DbSet<Person> People {get;set;}
    public DbSet<House> Houses{get;set;}
    public DbSet<Car> Cars{get;set;}
    public DbSet<Ghoul> Ghouls{get;set;}
    public DbSet<RadRoach> RadRoaches{get; set;}
    public DbSet<DeathClaw> DeathClaws{get;set;}
    public DbSet<Dog> Dogs{get;set;}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string path = Path.Combine(Environment.CurrentDirectory, "World.db");
        string connection = $"Filename={path}";

        Console.WriteLine($"Connection: {connection}");
        optionsBuilder.UseSqlite(connection);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ghoul>();
        modelBuilder.Entity<RadRoach>();
        modelBuilder.Entity<DeathClaw>();
        modelBuilder.Entity<Dog>();
        modelBuilder.Entity<Weapon>();
    }
}