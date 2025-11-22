namespace Lewee.Domain.Tests.Unit;

internal class Address : ValueObject<Address>
{
    public Address(int streetNumber, string streetName, string suburb, string state, string postcode)
    {
        this.StreetNumber = streetNumber;
        this.StreetName = streetName;
        this.Suburb = suburb;
        this.State = state;
        this.Postcode = postcode;
    }

    public int StreetNumber { get; }
    public string StreetName { get; }
    public string Suburb { get; }
    public string State { get; }
    public string Postcode { get; }

    public override bool Equals(object obj)
    {
        return obj is Address address && this.Equals(address);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            this.StreetNumber,
            this.StreetName,
            this.Suburb,
            this.State,
            this.Postcode);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.StreetNumber;
        yield return this.StreetName.ToUpperInvariant();
        yield return this.Suburb.ToUpperInvariant();
        yield return this.State.ToUpperInvariant();
        yield return this.Postcode.ToUpperInvariant();
    }
}
