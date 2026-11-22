using System.Diagnostics.CodeAnalysis;

namespace Pizzeria.Store.Domain;

public static class Menu
{
    public static class PizzaIds
    {
        public static readonly Guid Margherita = Guid.Parse("00000001-0000-0000-0000-000000000001");
        public static readonly Guid Marinara = Guid.Parse("00000001-0000-0000-0000-000000000002");
        public static readonly Guid QuattroStagioni = Guid.Parse("00000001-0000-0000-0000-000000000003");
        public static readonly Guid Carbonara = Guid.Parse("00000001-0000-0000-0000-000000000004");
        public static readonly Guid FruttiDiMare = Guid.Parse("00000001-0000-0000-0000-000000000005");
        public static readonly Guid QuattroFormaggi = Guid.Parse("00000001-0000-0000-0000-000000000006");
        public static readonly Guid Crudo = Guid.Parse("00000001-0000-0000-0000-000000000007");
        public static readonly Guid Napoletana = Guid.Parse("00000001-0000-0000-0000-000000000008");
        public static readonly Guid Pugliese = Guid.Parse("00000001-0000-0000-0000-000000000009");
        public static readonly Guid Montanara = Guid.Parse("00000001-0000-0000-0000-000000000010");
    }

    public static class PizzaNames
    {
        public const string Margherita = nameof(Margherita);

        public const string Marinara = nameof(Marinara);

        public const string QuattroStagioni = "Quattro Stagioni";

        public const string Carbonara = nameof(Carbonara);

        public const string FruttiDiMare = "Frutti di Mare";

        public const string QuattroFormaggi = "Quattro Formaggi";

        public const string Crudo = nameof(Crudo);

        public const string Napoletana = nameof(Napoletana);

        public const string Pugliese = nameof(Pugliese);

        public const string Montanara = nameof(Montanara);
    }

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:A field should not follow a class", Justification = "Pizzas array is logically grouped after its related constant classes for better readability")]
    public static readonly Pizza[] Pizzas =
    [
        new(
            PizzaIds.Margherita,
            PizzaNames.Margherita,
            "Tomato sauce, mozzarella, and oregano",
            5.00m),

        new(
            PizzaIds.Marinara,
            PizzaNames.Marinara,
            "Tomato sauce, garlic and basil",
            5.50m),

        new(
            PizzaIds.QuattroStagioni,
            PizzaNames.QuattroStagioni,
            "Tomato sauce, mozzarella, mushrooms, ham, artichokes, olives, and oregano",
            8.00m),

        new(
            PizzaIds.Carbonara,
            PizzaNames.Carbonara,
            "Tomato sauce, mozzarella, parmesan, eggs, and bacon",
            8.50m),

        new(
            PizzaIds.FruttiDiMare,
            PizzaNames.FruttiDiMare,
            "Tomato sauce and seafood",
            8.50m),

        new(
            PizzaIds.QuattroFormaggi,
            PizzaNames.QuattroFormaggi,
            "Tomato sauce, mozzarella, parmesan, gorgonzola cheese, artichokes, and oregano",
            8.50m),

        new(
            PizzaIds.Crudo,
            PizzaNames.Crudo,
            "Tomato sauce, mozzarella, Parma ham, parmesan, mushrooms, and oregano",
            9.00m),

        new(
            PizzaIds.Napoletana,
            PizzaNames.Napoletana,
            "Tomato sauce, mozzarella, oregano, anchovies",
            9.00m),

        new(
            PizzaIds.Pugliese,
            PizzaNames.Pugliese,
            "Tomato sauce, mozzarella, oregano, and onions",
            9.00m),

        new(
            PizzaIds.Montanara,
            PizzaNames.Montanara,
            "Tomato sauce, mozzarella, mushrooms, pepperoni, and oregano",
            9.00m)
    ];

    public static Pizza GetPizzaByName(string name)
    {
        return Pizzas.Single(x => string.Equals(x.Name, name, StringComparison.Ordinal));
    }
}
