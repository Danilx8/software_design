namespace Software_Design._5._Grades_Calculator;

public class GradesCalculator
{
    private readonly int _minimalGrade;
    private readonly int _maximalGrade;

    public GradesCalculator(int minimalGrade, int maximalGrade)
    {
        if (minimalGrade >= maximalGrade)
            throw new ArgumentException("Минимальное значение не может быть больше максимального");
        _minimalGrade = minimalGrade;
        _maximalGrade = maximalGrade;
    }

    public decimal CalculateAverage(List<int> grades)
    {
        if (grades.Count == 0 || grades is null) throw new ArgumentException("Список оценок не может быть пуст");

        long sum = 0;
        foreach (var grade in grades)
        {
            if (_minimalGrade > grade || grade > _maximalGrade)
                throw new ArgumentException($"Оценка {grade} не входит в заданную систему оценок");
            sum += grade;
        }

        return Math.Round((decimal)sum / grades.Count, 2);
    }
}