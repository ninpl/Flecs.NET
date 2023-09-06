// Transitive relationships make it possible to tell the ECS that if an entity
// has a relationship (R, X) and X has relationship (R, Y), the entity should be
// treated as if it also has (R, Y). In practice this is useful for expressing
// things like:
//
// Bob lives in SanFrancisco
// San Francisco is located in the United States
// Therefore Bob also lives in the United States.
//
// An example of transitivity can be seen in the component_inheritance example.
// This example uses the builtin IsA relationship, which is transitive. This
// example shows how to achieve similar behavior with a user-defined relationship.
#if Cpp_Rules_TransitiveQueries

using Flecs.NET.Core;
using static Flecs.NET.Bindings.Native;

using World world = World.Create();

// Register the LocatedIn relationship as transitive
world.Component<LocatedIn>().Entity.Add(EcsTransitive);

// Populate the store with locations
Entity earth = world.Entity("Earth")
    .Add<Planet>();

// Continents
Entity northAmerica = world.Entity("NorthAmerica")
    .Add<Continent>()
    .Add<LocatedIn>(earth);

Entity europe = world.Entity("Europe")
    .Add<Continent>()
    .Add<LocatedIn>(earth);

// Countries
Entity unitedStates = world.Entity("UnitedStates")
    .Add<Country>()
    .Add<LocatedIn>(northAmerica);

Entity netherlands = world.Entity("Netherlands")
    .Add<Country>()
    .Add<LocatedIn>(europe);

// States
Entity california = world.Entity("California")
    .Add<State>()
    .Add<LocatedIn>(unitedStates);

Entity washington = world.Entity("Washington")
    .Add<State>()
    .Add<LocatedIn>(unitedStates);

Entity noordHolland = world.Entity("NoordHolland")
    .Add<State>()
    .Add<LocatedIn>(netherlands);

// Cities
Entity sanFrancisco = world.Entity("SanFrancisco")
    .Add<City>()
    .Add<LocatedIn>(california);

Entity seattle = world.Entity("Seattle")
    .Add<City>()
    .Add<LocatedIn>(washington);

Entity amsterdam = world.Entity("Amsterdam")
    .Add<City>()
    .Add<LocatedIn>(noordHolland);

// Inhabitants
world.Entity("Bob")
    .Add<Person>()
    .Add<LocatedIn>(sanFrancisco);

world.Entity("Alice")
    .Add<Person>()
    .Add<LocatedIn>(seattle);

world.Entity("Job")
    .Add<Person>()
    .Add<LocatedIn>(amsterdam);

// Create a query that finds the countries persons live in. Note that these
// have not been explicitly added to the Person entities, but because the
// LocatedIn is transitive, the rule engine will traverse the relationship
// until it found something that is a country.
//
// The equivalent of this query in the DSL is:
//   Person, (LocatedIn, $Location), Country($Location)
Rule r = world.Rule(
    filter: world.FilterBuilder()
        .With<Person>()
        .With<LocatedIn>("$Location")
        .With<Country>().Src("$Location")
);

// Lookup the index of the variable. This will let us quickly lookup its
// value while we're iterating.
int locationVar = r.FindVar("Location");

// Iterate the rule
r.Iter((Iter it) =>
{
    foreach (int i in it)
        Console.WriteLine($"{it.Entity(i).Name()} lives in {it.GetVar(locationVar).Name()}");
});

// Rules need to be explicitly deleted.
r.Destruct();

public struct LocatedIn { }
public struct Planet { }
public struct Continent { }
public struct Country { }
public struct State { }
public struct City { }
public struct Person { }

#endif

// Output:
// Bob lives in UnitedStates
// Alice lives in UnitedStates
// Job lives in Netherlands
