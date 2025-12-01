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
        new Person("Bob",new Coords()),
        new Person("Sue",new Coords()),
        new Person("Sally",new Coords()),
        new Person("Jimmy",new Coords(1,1))
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

    // Add entities to database:
    // NOTE: This doesn't actually store them, EF Core starts tracking them
    world.People.AddRange(people);
    world.Cars.AddRange(cars);
    world.Houses.AddRange(houses);

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