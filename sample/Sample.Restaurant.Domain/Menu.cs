namespace Sample.Restaurant.Domain;

public static class Menu
{
    public static readonly MenuItem Pizza = new(
        new Guid("7fabe425-1d65-48d3-9ae4-caf5f27bbde8"),
        "Pizza",
        15,
        MenuItemType.Food);

    public static readonly MenuItem Pasta = new(
        new Guid("70da3fcf-381d-4285-88e6-794b4b57e5b5"),
        "Pasta",
        15,
        MenuItemType.Food);

    public static readonly MenuItem GarlicBread = new(
        new Guid("89669f56-63fe-4966-9028-f22f8d5a72f5"),
        "Garlic Bread",
        15,
        MenuItemType.Food);

    public static readonly MenuItem IceCream = new(
        new Guid("1e1670cf-a80e-4421-a04f-6e28dc32a5d4"),
        "Ice Cream",
        15,
        MenuItemType.Food);

    public static readonly MenuItem Beer = new(
        new Guid("76495fb6-323e-4fbd-b0a4-5dbfcf61cef8"),
        "Beer",
        15,
        MenuItemType.Drink);

    public static readonly MenuItem Wine = new(
        new Guid("58b91a73-682d-4696-b545-b493b56a0335"),
        "Wine",
        15,
        MenuItemType.Food);

    public static readonly MenuItem SoftDrink = new(
        new Guid("110d16d7-3ce5-49dd-a187-d3640fdb42b5"),
        "Soft Drink",
        15,
        MenuItemType.Food);
}
