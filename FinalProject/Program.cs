using static System.Console;

using Entities;

using (World world = new())
{
    bool deleted = world.Database.EnsureDeleted();
    Console.WriteLine($"Database deleted: {deleted}");
    bool created = world.Database.EnsureCreated();
    Console.WriteLine($"Database created: {created}");

    // Create some entities
    List<Person> people = [
        new Person("Bob",new Coords(), 40, 15),
        new Person("Sue",new Coords(), 35, 20),
        new Person("Sally",new Coords(), 25, 5),
        new Person("Jimmy",new Coords(1,1), 50, 20)
    ];

    List<Car> cars = [
        new Car("Ford","Focus") {Location = new Coords()},
        new Car("Toyota","Camry") {Location = new Coords()},
        new Car("Ford","Mustang") {Location = new Coords(1,1)}
    ];

    List<House> houses = [
        new House("123 Union University Dr.") {Location = new Coords()},
        new House("456 Madison Ave.") {Location = new Coords(1,1)}
    ];

    List<Ghoul> ghouls = [
        new Ghoul("Hancock", false, new Coords(), 75, 25),
        new Ghoul("Raul", false, new Coords(), 75, 25),
        new Ghoul("Mr. Sumner", true, new Coords(), 100, 30)
    ];

    List<RadRoach> radRoaches = [
        new RadRoach("Common",15, 5),
        new RadRoach("Common",15, 5),
        new RadRoach("Common",15, 5),
        new RadRoach("Common",15, 5),
        new RadRoach("Common",15, 5),
        new RadRoach("Common",15, 5),
        new RadRoach("Irradiated",15, 5)
    ];

    List<DeathClaw> deathClaws = [
        new DeathClaw("Chameleon",200, 50),
        new DeathClaw("Alpha",250, 75)
    ];

    // Add entities to database:
    // NOTE: This doesn't actually store them, EF Core starts tracking them
    world.People.AddRange(people);
    world.Cars.AddRange(cars);
    world.Houses.AddRange(houses);
    world.Ghouls.AddRange(ghouls);
    world.RadRoaches.AddRange(radRoaches);
    world.DeathClaws.AddRange(deathClaws);

    /*
    You can also start tracking individual entities by calling world.People.Add()
    Also, you can add entities to multiple DbSets by calling world.Add() and
    world.AddRange().
    */

    // Save the entities to the database:
    world.SaveChanges();

    // Now let's query the database
    Person bob = world.People.Where(p => p.Name=="Bob").Single();
    Person sue = world.People.Single(p => p.Name=="Sue");
    House UUHouse = world.Houses.Single(h => h.Address=="123 Union University Dr.");
    List<Car> fords = world.Cars.Where(c=>c.Make=="Ford").ToList();

    UUHouse.Residents = [bob,sue];
    UUHouse.Garage = fords;
    foreach (Car c in fords)
    {
        c.Passengers = [bob,sue];
    }
    bob.Cars = fords;
    sue.Cars = [fords[0]];
    world.SaveChanges();

}