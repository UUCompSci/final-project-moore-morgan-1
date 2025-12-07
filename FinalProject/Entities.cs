using System.Collections.Concurrent;
using System.Dynamic;
using Interfaces;
using Microsoft.EntityFrameworkCore;
namespace Entities;
public abstract class BaseEntity
{
    /* Use a unique Guid as the Id. EF Core will automatically set
       Id as the primary field by convention (based on the variable name).
       If you want to name it something different, you have to decorate with 
       [Key] from System.ComponentModel.DataAnnotations namespace.
       
       The private set prevents the program from being able to modify the
       Id property, but allows EF Core to add it to the model without 
       explicitly adding it with Fluent API (get only properties are
       ignored by EF Core by convention).

       We initialize it with a new Guid object (which will be a unique
       128-bit integer represented as a 32-digit hexadecimal string).
       It will be saved as a text string in the database.
    */
    public Guid Id { get; private set;} = Guid.NewGuid();
    // Use Coords struct (defined below) to save Location
    public Coords Location {get; set;}

    // Define default and custom constructors
    public BaseEntity(){}
    public BaseEntity(Coords location) => Location = location;
}

//To give animals and people hitpoitns and make them targets for iAttack
public abstract class LivingThing : BaseEntity, iMove
{
    public LivingThing() : base(){}
    public LivingThing(int HealthPoints, int DMGStat) : this(HealthPoints,DMGStat, new Coords()){}
    public LivingThing(int HealthPoints,int DMGStat, Coords Location) : base(Location)
    {
        this.HealthPoints = HealthPoints;
        this.DMGStat = DMGStat;
    }
    public LivingThing(Coords Location) : base(Location){}

    public int HealthPoints { get; set; }
    public int DMGStat {get; set;}

    public void Move(Coords NextLocation)
    {
        Location = NextLocation;
    }
}

// Declared as an owned entity type: https://learn.microsoft.com/en-us/ef/core/modeling/owned-entities
[Owned]
public class Coords
{
    // Define X and Y
    public double X {get;set;}
    public double Y {get;set;}
    // Default constructor will set X=0 and Y=0
    public Coords(){}
    // Custom constructor sets X and Y.  
    public Coords(double x,double y) {X = x; Y = y;}
    // Override ToString() method
    public override string ToString() => $"({X:N2}, {Y:N2})";
}

// We will set up some relationships between classes for Person,
// House, and Car classes.

// We will define optional One-to-Many relationships between Person and House 
// (one house can hold multiple people) and Car and House (since a house can 
// have multiple cars parked at it).

// We will define an optional Many-to-Many relationship between Person and Car
// since a car can have multiple drivers, and a person can own multiple car.

// You can read more about configuring relationships between entities here:
// https://learn.microsoft.com/en-us/ef/core/modeling/relationships

public class Person : LivingThing, iConverse, iAttack
{
    public Person() : base(){}

    // Define constructor
    // Note, to use constructor, you must follow naming conventions
    // for parameter and properties. (There's currently no way to 
    // explicitly map constructor parameters to database properties.)
    // Note: you can only map constructor parameters to simple types
    // Coord type Location cannot be mapped so it will have to be set
    // with object initialization syntax.
    public Person(string name, Coords Location, int HealthPoints, int DMGStat) : base(HealthPoints, DMGStat, Location)
    {
        Name = name;
    }

    public string Name { get; set; }
    // Define One-to-Many relationship between Person and House
    // There are lots of ways to set this up. We will use private
    // foreign keys since they are only important for the database,
    // not our program. You can read more about setting up One-to-Many
    // relationships here: 
    // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/one-to-many#optional-one-to-many
    
    // Define private optional foreign key
    // Name is important for EF Core to automatically set foreign key (can be configured manually instead)
    private Guid? HouseId {get; set;} // Setting nullable type defines relationship as optional.
    // Define public reference navigation, again must be nullable for relationship to be optional.
    public House? Home {get;set;}

    // Setup many-to-many relationship between Person and Car. You can read more here:
    // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many
    public List<Car> Cars {get;set;} = new();
    public string? VoiceLine {get; set;}


    public void Converse()
    {
        Console.WriteLine(VoiceLine);
        throw new NotImplementedException();
    }

    public void Attack(LivingThing target)
    {
        int TargetAdjustedHealth = target.HealthPoints - DMGStat;
        throw new NotImplementedException();
    }
}

public class House : BaseEntity
{
    public House(string address) : base() => Address = address;
    public string Address {get;set;}

    // Setup one-to-many relationship with person
    public List<Person> Residents {get;set;} = new();

    // Setup one-to-many with car, using backing field to 
    // ensure no more than 2 cars garaged at house
    // Learn more about backing fields and EF Core here:
    // https://learn.microsoft.com/en-us/ef/core/modeling/backing-field
    private List<Car> _garage = [];
    public List<Car> Garage // NOTE: Setter logic can be bypassed with Garage.Add(). I'm trying to keep the code simple, 
    {                       // but you could use IReadOnlyList<Car> and use class methods to check number of cars.  
        get => _garage;     
        set
        {
            if (value.Count > 2)
            {
                Console.WriteLine("Garage can only hold 2");
                return;
            }
            else
            {
                _garage = value;
            }
        }
    } 

}

public class Car : BaseEntity
{
    public Car(string make,string model) : base()
    {
        Make = make;
        Model = model;
    }
    public string Make {get;set;}
    public string Model {get;set;}

    // Many-to-many with Person
    public List<Person> Passengers {get;set;} = new();

    // Setup one-to-many with House
    private Guid? HouseId {get;set;}
    public House? Garage {get;set;}
}

public class Ghoul : Person
{
    public Ghoul() : base(){}
    public bool FeralState {get; set;}
    public Ghoul(string name, bool FeralState, int HealthPoints, int DMGStat) : this(name, FeralState, new Coords(), HealthPoints, DMGStat)
    {
    }

    public Ghoul(string name, bool FeralState, Coords location, int HealthPoints, int DMGStat) : base(name, location, HealthPoints, DMGStat)
    {
        Name = name;
        this.FeralState = FeralState;
    }

}

public class RadRoach : LivingThing, iAttack
{
    public RadRoach() : base(){}
    public string Type {get; set;}
    public RadRoach(string Type, int HealthPoints, int DMGStat, Coords Location) : base(HealthPoints, DMGStat, Location)
    {
        this.Type = Type;
    }

    public void Attack(LivingThing target)
    {
        int TargetAdjustedHealth = target.HealthPoints - DMGStat;
        throw new NotImplementedException();
    }
}

public class DeathClaw : LivingThing, iAttack
{
    public DeathClaw() : base(){}
    public string Species {get; set;}
    public DeathClaw(string Species, int HealthPoints, int DMGStat, Coords Location) : base(HealthPoints, DMGStat, Location)
    {
        this.Species = Species;
    }

    public void Attack(LivingThing target)
    {
        int TargetAdjustedHealth = target.HealthPoints - DMGStat;
        throw new NotImplementedException();
    }
}

public class Dog : LivingThing, iConverse, iAttack
{
    public Dog() : base(){}
    public string Name {get; set;}
    public Dog(string name, int HealthPoints, int DMGStat, Coords Location) : base(HealthPoints, DMGStat, Location)
    {
        Name = name;
    }

    public string? VoiceLine { get; set; }

    public void Attack(LivingThing target)
    {
        int TargetAdjustedHealth = target.HealthPoints - DMGStat;
        throw new NotImplementedException();
    }

    public void Converse()
    {
        Console.WriteLine(VoiceLine);
        throw new NotImplementedException();
    }
}

public class Weapon : BaseEntity, iEquipable
{
    public string Type {get; set;}
    public int StatIncrease {get; set;}
    public Weapon() : base(){}
    public Weapon(string Type, int StatIncrease, Coords Location) : base(Location)
    {
        this.Type = Type;
        this.StatIncrease = StatIncrease;
    }

    public void Equip(LivingThing target)
    {
        int targetAdjustedDMG = target.DMGStat + StatIncrease;
        throw new NotImplementedException();
    }
}
