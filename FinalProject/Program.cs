using static System.Console;

using Entities;

using (World world = new())
{
    bool deleted = world.Database.EnsureDeleted();
    WriteLine($"Database deleted: {deleted}");
    bool created = world.Database.EnsureCreated();
    WriteLine($"Database created: {created}");

    // Create some entities
    List<Person> people = [ ///change later
        new Person("Nora",new Coords(), 40, 15),
        new Person("Piper",new Coords(), 35, 20),
        new Person("Mayor McDonough",new Coords(), 25, 5),
        new Person("Nate",new Coords(1,1), 50, 20)
    ];
    WriteLine($"Created {people.Count} people");

    List<Ghoul> ghouls = [
        new Ghoul("Hancock", false, new Coords(), 75, 25),
        new Ghoul("Raul", false, new Coords(), 75, 25),
        new Ghoul("Mr. Sumner", true, new Coords(), 100, 30)
    ];

    List<RadRoach> radRoaches = [
        new RadRoach("Common",15, 5, new Coords()),
        new RadRoach("Common",15, 5, new Coords()),
        new RadRoach("Common",15, 5, new Coords()),
        new RadRoach("Common",15, 5, new Coords()),
        new RadRoach("Common",15, 5, new Coords()),
        new RadRoach("Common",15, 5, new Coords()),
        new RadRoach("Irradiated",45, 15, new Coords())
    ];

    List<DeathClaw> deathClaws = [
        new DeathClaw("Chameleon",200, 50, new Coords()),
        new DeathClaw("Alpha",250, 75, new Coords())
    ];

    List<Dog> dogs = [
        new Dog("Dog Meat", 25, 15, new Coords())
    ];

    // Add entities to database:
    // NOTE: This doesn't actually store them, EF Core starts tracking them
    world.People.AddRange(people);
    world.Ghouls.AddRange(ghouls);
    world.RadRoaches.AddRange(radRoaches);
    world.DeathClaws.AddRange(deathClaws);
    world.Dogs.AddRange(dogs);

    /*
    You can also start tracking individual entities by calling world.People.Add()
    Also, you can add entities to multiple DbSets by calling world.Add() and
    world.AddRange().
    */

    // Save the entities to the database:
    int saved = world.SaveChanges();
    WriteLine($"SaveChanges returned: {saved} entities saved");


    // Now let's query the database
    Person bob = world.People.Single(p => p.Name=="Bob");
    Person sue = world.People.Single(p => p.Name=="Sue");
    Ghoul normalGhoul = world.Ghouls.Single(g => g.Name == "Hancock");
    Dog dogmeat = world.Dogs.Single(d => d.Name == "Dog Meat");
    RadRoach roach = world.RadRoaches.First();
    DeathClaw alpha = world.DeathClaws.Single(d => d.Species == "Alpha");

    dogmeat.Converse();
    dogmeat.Attack(roach);

    Weapon gun=new Weapon ("10mm gun", 10, new Coords());
    WriteLine("Hancock found a 10mm gun!");
    gun.Equip(normalGhoul);

    WriteLine("Hancock attacks dogmeat with a gun!");
    normalGhoul.Attack(dogmeat);

    world.SaveChanges();
   
}
