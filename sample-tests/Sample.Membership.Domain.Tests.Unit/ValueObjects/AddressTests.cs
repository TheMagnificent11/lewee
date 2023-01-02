using FluentAssertions;
using Sample.Membership.Domain.ValueObjects;
using Xunit;

namespace Sample.Membership.Domain.Tests.Unit.ValueObjects;

public static class AddressTests
{
    [Theory]
    [InlineData("123 Test Street", null, null, "Testville", "QLD", "Australia", "4999", null, "123 Test Street", null, null, "Testville", "QLD", "Australia", "4999", null, true)] // exact match
    [InlineData("123 Test Street", null, null, "Testville", "QLD", "Australia", "4999", null, "123 test street", null, null, "testville", "qld", "australia", "4999", null, true)] // case-insensitive match
    [InlineData("Unit 11", "123 Test Street", null, "Testville", "QLD", "Australia", "4999", "ABC123", "123 Test Street", null, null, "Testville", "QLD", "Australia", "4999", "ABC123", true)] // different, but same pic
    [InlineData("Unit 11", "123 Test Street", null, "Testville", "QLD", "Australia", "4999", "ABC123", "123 Test Street", null, null, "Testville", "QLD", "Australia", "4999", "abc123", true)] // different, but same case-insensitive pic
    [InlineData("123 Test Street", null, null, "Testville", "QLD", "Australia", "4999", "ABC123", "123 Test Street", null, null, "Testville", "QLD", "Australia", "4999", null, true)] // only one has pic
    [InlineData("Unit 11", "123 Test Street", null, "Testville", "QLD", "Australia", "4999", null, "123 Test Street", null, null, "Testville", "QLD", "Australia", "4999", null, false)] // different
    [InlineData("123 Test Street", null, null, "Testville", "QLD", "Australia", "4999", null, "1234 Test Street", null, null, "Testville", "QLD", "Australia", "4999", null, false)] // different address 1
    [InlineData("123 Test Street", null, null, "Testville", "QLD", "Australia", "4999", null, "123 Test Street", null, null, "Testington", "QLD", "Australia", "4999", null, false)] // different locality
    [InlineData("123 Test Street", null, null, "Testville", "QLD", "Australia", "4999", null, "123 Test Street", null, null, "Testville", "NSW", "Australia", "4999", null, false)] // different state
    [InlineData("123 Test Street", null, null, "Testville", "QLD", "Australia", "4999", null, "123 Test Street", null, null, "Testville", "QLD", "New Zealand", "4999", null, false)] // different country
    [InlineData("123 Test Street", null, null, "Testville", "QLD", "Australia", "4999", null, "123 Test Street", null, null, "Testville", "QLD", "Australia", "4998", null, false)] // different postcode
    public static void Equals_GivesCorrectResult_WhenComparingTwoAddresses(
        string prop1Address1,
        string prop1Address2,
        string prop1Address3,
        string prop1Locality,
        string prop1StateProvince,
        string prop1Country,
        string prop1Postcode,
        string prop1Pic,
        string prop2Address1,
        string prop2Address2,
        string prop2Address3,
        string prop2Locality,
        string prop2StateProvince,
        string prop2Country,
        string prop2Postcode,
        string prop2Pic,
        bool expectedResult)
    {
        // Arrange
        var address1 = new Address(prop1Address1, prop1Address2, prop1Address3, prop1Locality, prop1StateProvince, prop1Country, prop1Postcode, prop1Pic);
        var address2 = new Address(prop2Address1, prop2Address2, prop2Address3, prop2Locality, prop2StateProvince, prop2Country, prop2Postcode, prop2Pic);

        // Act
        var result = address1.Equals(address2);

        // Assert
        result.Should().Be(expectedResult);
    }
}
