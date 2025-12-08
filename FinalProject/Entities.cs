using System.Collections.Concurrent;
using static System.Console;
using System.Dynamic;
using Interfaces;
using Microsoft.EntityFrameworkCore;
namespace Entities;
public abstract class BaseEntity
{
    //Gives each entity a unique ID
    public Guid Id { get; private set;} = Guid.NewGuid();
    public Coords Location {get; set;}
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

[Owned]
public class Coords
{
    public double X {get;set;}
    public double Y {get;set;}
    public Coords(){}
    public Coords(double x,double y) {X = x; Y = y;}
    public override string ToString() => $"({X:N2}, {Y:N2})";
}

public class Person : LivingThing, iConverse, iAttack
{
    public Person() : base(){}
    public Person(string name, Coords Location, int HealthPoints, int DMGStat) : base(HealthPoints, DMGStat, Location)
    {
        Name = name;
    }

    public string Name { get; set; }
  
    //allows the voiceline to be set in the constructor. The ? allows the field to be null.
    public string? VoiceLine {get; set;}


    public void Converse()
    {
        //set default "voiceline" if left empty
        if (string.IsNullOrWhiteSpace(VoiceLine))
            WriteLine($"{Name} has nothing to say.");
        else
            WriteLine($"{Name} says: {VoiceLine}");
    }


    public void Attack(LivingThing target)
    {
        int TargetAdjustedHealth = target.HealthPoints - DMGStat;
        throw new NotImplementedException();
    }
}


// A type of creature that was once human in the game which is why it inherits from person.
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
    public new void Attack(LivingThing target)
{
    // if the ghoul is feral then it deals double damage
    int dmg = FeralState ? DMGStat * 2 : DMGStat;

    target.HealthPoints -= dmg;

    //Messages meant for combat
    if (target.HealthPoints <= 0)
        WriteLine($"{target.GetType().Name} has died!");
    else
        WriteLine($"{target.GetType().Name} now has {target.HealthPoints} HP remaining.");
}


}

//Giant bug which is a staple of the franchise
public class RadRoach : LivingThing, iAttack
{
    public void Attack(LivingThing target)
{
    if (target.HealthPoints <= 0)
    {
        WriteLine($"{this.GetType().Name} tries to attack {target.GetType().Name}, but they are already dead.");
        return;
    }

    target.HealthPoints -= DMGStat;

    WriteLine($"{this.GetType().Name} attacks {target.GetType().Name} for {DMGStat} damage!");

    if (target.HealthPoints <= 0)
        WriteLine($"{target.GetType().Name} has died!");
    else
        WriteLine($"{target.GetType().Name} now has {target.HealthPoints} HP remaining.");
}

    public RadRoach() : base(){}
    public string Type {get; set;}
    public RadRoach(string Type, int HealthPoints, int DMGStat, Coords Location) : base(HealthPoints, DMGStat, Location)
    {
        this.Type = Type;
    }
}

//Genetically modified lizards from fallout
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
        if (target.HealthPoints <= 0)
        {
            WriteLine($"{this.GetType().Name} tries to attack {target.GetType().Name}, but they are already dead.");
            return;
        }

        target.HealthPoints -= DMGStat;

        WriteLine($"{this.GetType().Name} attacks {target.GetType().Name} for {DMGStat} damage!");

        if (target.HealthPoints <= 0)
            WriteLine($"{target.GetType().Name} has died!");
        else
            WriteLine($"{target.GetType().Name} now has {target.HealthPoints} HP remaining.");
    }

}

// Inherits from iConverse just so it can bark.
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
        if (target.HealthPoints <= 0)
        {
            WriteLine($"{this.GetType().Name} tries to attack {target.GetType().Name}, but they are already dead.");
            return;
        }

        target.HealthPoints -= DMGStat;

        WriteLine($"{this.GetType().Name} attacks {target.GetType().Name} for {DMGStat} damage!");

        if (target.HealthPoints <= 0)
            WriteLine($"{target.GetType().Name} has died!");
        else
            WriteLine($"{target.GetType().Name} now has {target.HealthPoints} HP remaining.");
    }


    public void Converse()
    {
        if (string.IsNullOrWhiteSpace(VoiceLine))
            WriteLine($"{Name} barks happily.");
        else
            WriteLine($"{Name} says: {VoiceLine}");
    }

}

// Made simply because we wanted to know if we could
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

    public void Equip(Person target)
{
    target.DMGStat += StatIncrease;

    WriteLine($"{target.GetType().Name} equipped {Type}!");
    WriteLine($"{target.GetType().Name}'s damage increased by {StatIncrease}.");
    WriteLine($"New DMG stat: {target.DMGStat}");
}

}
