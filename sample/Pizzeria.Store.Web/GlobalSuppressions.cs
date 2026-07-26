using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Performance",
    "CA1515:Consider making public types internal",
    Justification = "Public types are needed for bUnit testing")]
