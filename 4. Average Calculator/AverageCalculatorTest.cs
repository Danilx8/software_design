using Xunit;

namespace Software_Design._4._Average_Calculator;

public class AverageCalculatorTest
{
    public static IEnumerable<object[]> GenerateArrays()
    {
        var random = new Random();
        for (int i = 0; i < 1000; i++)
        {
            int[] array = new int[100];
            long sum = 0;
            for (int j = 0; j < array.Length; j++)
            {
                {
                    array[j] = random.Next(int.MinValue, int.MaxValue);
                    sum += array[j];
                }
            }

            double expectedAverage = (double)sum / array.Length;
            yield return [array, expectedAverage];
        }
    }

    [Fact]
    public void EmptyArray()
    {
        int[] empty = [];
        Assert.Throws<ArgumentException>(() => AverageCalculator.calculateAverage(empty));
    }

    [Fact]
    public void NullArray()
    {
        int[] nullArray = null!;
        Assert.Throws<ArgumentException>(() => AverageCalculator.calculateAverage(nullArray));
    }

    [Theory]
    [MemberData(nameof(GenerateArrays))]
    public void CalculateAverage_LargeArrays_ReturnsCorrectAverage(int[] numbers, double expectedAverage)
    {
        double result = AverageCalculator.calculateAverage(numbers);
        Assert.Equal(expectedAverage, result);
    }
}