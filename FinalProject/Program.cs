using static System.Console;

using Entities;

using (World world = new())
{
    bool deleted = world.Database.EnsureDeleted();
    WriteLine($"Database deleted: {deleted}");
    bool created = world.Database.EnsureCreated();
    WriteLine($"Database created: {created}");

    // Creates some entities for use
    List<Person> people = [ 
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

    // Adds stuff to the database:
    world.People.AddRange(people);
    world.Ghouls.AddRange(ghouls);
    world.RadRoaches.AddRange(radRoaches);
    world.DeathClaws.AddRange(deathClaws);
    world.Dogs.AddRange(dogs);

    // Saves everything to the database
    int saved = world.SaveChanges();
    WriteLine($"SaveChanges returned: {saved} entities saved");


    // Requests the data from the database so we can use it
    Person nate = world.People.Single(p => p.Name=="Nate");
    Person nora = world.People.Single(p => p.Name=="Nora");
    Ghoul normalGhoul = world.Ghouls.Single(g => g.Name == "Hancock");
    Dog dogmeat = world.Dogs.Single(d => d.Name == "Dog Meat");
    RadRoach roach = world.RadRoaches.First();
    DeathClaw alpha = world.DeathClaws.Single(d => d.Species == "Alpha");

    //Selection keys
    const ConsoleKey Z = ConsoleKey.Z;
    const ConsoleKey X = ConsoleKey.X;
    const ConsoleKey C = ConsoleKey.C;
    const ConsoleKey V = ConsoleKey.V;
    const ConsoleKey B = ConsoleKey.B;


    bool playing=true;
    while (playing)
    {
        WriteLine("Move Nate(Z)");
        WriteLine("Make Dogmeat to speak(X)");
        WriteLine("Hancock attacks Dogmeat (C)");
        WriteLine("Give Hancock a gun (V)");
        WriteLine("Exit (B)");
        WriteLine("Choose an action: ");

        ConsoleKey choice = ReadKey(true).Key;
        
        switch (choice)
        {
            case Z:
                nate.Move(new Coords(nate.Location.X + 1, nate.Location.Y + 1));
                WriteLine($"Nate moved to {nate.Location}");
                break;

            case X:
                dogmeat.Converse();
                break;

            case C:
                WriteLine("Hancock attacks Dogmeat:");
                normalGhoul.Attack(dogmeat);
                break;

            case V:
                {
                    Weapon w = new Weapon("10 mm gun", 5, new Coords());
                    WriteLine("Hancock found a 10 mm gun!");
                    w.Equip(normalGhoul);
                }
                break;

            case B:
                WriteLine("Exiting the game!");
                playing = false;
                break;

            default:
                WriteLine("Invalid command.");
                break;
        }
        WriteLine("Press a key to continue");
        ReadKey(true);
    }

    world.SaveChanges();
   
}
