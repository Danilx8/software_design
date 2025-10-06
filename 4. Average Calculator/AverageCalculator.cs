namespace Software_Design._4._Average_Calculator;

public class AverageCalculator
{
    public static double calculateAverage(int[] numbers)
    {
        if (numbers == null || numbers.Length == 0)
            throw new ArgumentException("Array is null or empty");

        long sum = 0; // Используем long для предотвращения переполнения
        foreach (int num in numbers)
        {
            sum += num;
        }

        return (double)sum / numbers.Length;
    }
}