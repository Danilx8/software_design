using Software_Design._5._Grades_Calculator;
using Xunit;

namespace Software_Design._5._Grades_Calculator;

public class GradesCalculatorTest
{
    // Тестируем стандартные валидные данные
    [Theory]
    [InlineData(new[] { 3, 4, 5 }, 4.00)] // Валидные оценки
    [InlineData(new[] { 1, 1, 1 }, 1.00)] // Минимальные оценки
    public void EquivalencePartitioning_ValidGrades_ReturnsCorrectAverage(int[] grades, decimal expected)
    {
        var calc = new GradesCalculator(1, 5);
        var result = calc.CalculateAverage(grades.ToList());
        Assert.Equal(expected, result);
    }

    // Тест граничных значений
    [Theory]
    [InlineData(1, 2.00)] // Минимальное значение
    [InlineData(5, 4.00)] // Максимальное значение
    public void BoundaryValue_MinMaxGrades_ReturnsAverage(int boundaryGrade, decimal expected)
    {
        var calc = new GradesCalculator(1, 5);
        var result = calc.CalculateAverage(new List<int> { boundaryGrade, 3 });
        Assert.Equal(expected, result);
    }

    // Проверка инвариантов: среднее значение оценок не зависит от порядка в списке
    [Fact]
    public void PropertyBased_OrderIndependent_SameResult()
    {
        var calc = new GradesCalculator(1, 5);
        var list1 = new List<int> { 2, 3, 4, 5 };
        var list2 = new List<int> { 5, 4, 3, 2 };

        var avg1 = calc.CalculateAverage(list1);
        var avg2 = calc.CalculateAverage(list2);

        Assert.Equal(avg1, avg2);
    }

    // Лайтовое Fuzz-тестирование
    [Fact]
    public void FuzzTesting_RandomInputs_NoExceptions()
    {
        var random = new Random(42);

        for (int i = 0; i < 100; i++)
        {
            int min = random.Next(-100, 100);
            int max = random.Next(min + 1, 200);
            var calc = new GradesCalculator(min, max);

            int listSize = random.Next(1, 1000);
            var grades = Enumerable.Range(0, listSize)
                .Select(_ => random.Next(min, max + 1))
                .ToList();

            var result = calc.CalculateAverage(grades);
            Assert.InRange(result, min, max);
        }
    }

    // Проверяем, что списки с разной длиной и разными значениями могут иметь одинаковое среднее значение
    [Fact]
    public void FuzzTesting_DifferentListsSameAverage_EqualResults()
    {
        var calc = new GradesCalculator(1, 100);

        // Список 1: 10 элементов со средним 50
        var list1 = Enumerable.Repeat(50, 10).ToList();

        // Список 2: 1000 элементов со средним 50
        var list2 = Enumerable.Repeat(50, 1000).ToList();

        var avg1 = calc.CalculateAverage(list1);
        var avg2 = calc.CalculateAverage(list2);

        Assert.Equal(avg1, avg2);
        Assert.Equal(50.00m, avg1);
    }
}