using FluentAssertions;
using FluentAssertions.Collections;

namespace RenderWareIoTwo.Tests;

public static class AssertionExtensions
{
    public static void ShouldBeEquivalentToWithOptionalPadding(
        this GenericCollectionAssertions<byte> assertion, 
        ICollection<byte> expected)
    {
        var expectedByteArray = expected.ToArray();

        for (int i = 0; i < assertion.Subject.Count(); i++)
        {
            if (i >= expectedByteArray.Length)
                assertion.Subject.ElementAt(i).Should().Be(0, $"Because the byte at {i} should be 0, since it's longer than expected");
            else
                assertion.Subject.ElementAt(i).Should().Be(expectedByteArray[i], $"Because the byte at {i} should match");
        }
    }
}
