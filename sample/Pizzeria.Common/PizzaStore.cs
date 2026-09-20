namespace Pizzeria.Common;

/// <summary>
/// Well-known identifiers for the single pizzeria tenant and the roles used to authorize Pizza Store API access.
/// </summary>
public static class PizzaStore
{
    public static class Tenant
    {
        public const string Code = "PIZZERIA";

        public const string Name = "Lewee Pizzeria";

        public static readonly Guid Id = Guid.Parse("00000002-0000-0000-0000-000000000001");
    }

    public static class Roles
    {
        public const string StoreManagerCode = "store-manager";

        public const string StoreManagerName = "Store Manager";

        public const string StoreStaffCode = "store-staff";

        public const string StoreStaffName = "Store Staff";
    }
}
