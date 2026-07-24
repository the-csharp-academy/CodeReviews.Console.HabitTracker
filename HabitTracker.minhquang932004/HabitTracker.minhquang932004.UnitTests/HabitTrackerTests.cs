namespace HabitTracker.minhquang932004.UnitTests;

public class HabitTrackerTests
{
    private static readonly object[] IsNotValidStringCases_ReturnFalse =
    {
        new TestCaseData("Drinking Milk"),
        new TestCaseData("Lap"),
        new TestCaseData("Talk To Random")
    };

    private static readonly object[] IsNotValidStringCases_ReturnTrue =
    {
        new TestCaseData("Drinking Milk04"),
        new TestCaseData("Lap2004"),
        new TestCaseData(" "),
        new TestCaseData("")
    };

    [TestCaseSource(nameof(IsNotValidStringCases_ReturnFalse))]
    public void GivenStrings_WhenIsNotValidStringCalled_ReturnFalse(string input)
    {
        var isNotValidString = Program.IsNotValidString(input);

        Assert.That(isNotValidString, Is.False);
    }

    [TestCaseSource(nameof(IsNotValidStringCases_ReturnTrue))]
    public void GivenEmpty_Or_StringWithNumbers_WhenIsNotValidStringCalled_ReturnTrue(string input)
    {
        var isNotValidString = Program.IsNotValidString(input);

        Assert.That(isNotValidString, Is.True);
    }
}
