namespace Sample.Pizzeria.Domain;

public static class Menu
{
    public static readonly Pizza[] Pizzas =
    [
        new(
            1,
            "Margherita",
            "Tomato sauce, mozzarella, and oregano",
            5.00m),

        new(2,
            "Marinara",
            "Tomato sauce, garlic and basil",
            5.50m),

        new(
            3,
            "Quattro Stagioni",
            "Tomato sauce, mozzarella, mushrooms, ham, artichokes, olives, and oregano",
            8.00m),

        new(
            4,
            "Carbonara",
            "Tomato sauce, mozzarella, parmesan, eggs, and bacon",
            8.50m),

        new(
            5,
            "Frutti di Mare",
            "Tomato sauce and seafood",
            8.50m),

        new(
            6,
            "Quattro Formaggi",
            "Tomato sauce, mozzarella, parmesan, gorgonzola cheese, artichokes, and oregano",
            8.50m),

        new(
            7,
            "Crudo",
            "Tomato sauce, mozzarella, Parma ham, parmesan, mushrooms, and oregano",
            9.00m),

        new(
            8,
            "Napoletana",
            "Tomato sauce, mozzarella, oregano, anchovies",
            9.00m),

        new(
            9,
            "Pugliese",
            "Tomato sauce, mozzarella, oregano, and onions",
            9.00m),

        new(
            10,
            "Montanara",
            "Tomato sauce, mozzarella, mushrooms, pepperoni, and oregano",
            9.00m)
    ];
}
