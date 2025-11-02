using Software_Design._11._Functional_Class.Mocks;
using Software_Design._11._Functional_Class.Mocks.MatrixElements;

namespace Software_Design._11._Functional_Class;

// Я буду переписывать класс из последнего раздела по ООАП - матрицу и обёртку вокруг неё
// Чтобы сделать весь класс работающим, мне бы пришлось по-хорошему весь проект переделывать, поэтому все классы, кроме
// этого, - моковыые
// Всю статистику из этого класса я вытащил, подразумевая, что она будет вестись в пайплайнах над этим классом

/**
public class MatrixManipulator
 {
     private static MatrixManipulator? _matrixManipulator;
     private readonly StatisticsCounter _statistics;
     private readonly Matrix _matrix;
     private readonly Iterator _iterator;

     private readonly Dictionary<Type, int> _bonuses = new()
     {
         { typeof(LaneRemover), 0 },
         { typeof(TypeRemover), 0 }
     };

     private MatrixManipulator()
     {
         _matrix = Matrix.Instance;
         _iterator = new Iterator(_matrix);
         _statistics = StatisticsCounter.Instance;
     }

     public static MatrixManipulator Instance => _matrixManipulator ??= new MatrixManipulator();

     public void SwitchPlaces(MoveOption move)
     {
         _statistics.AccountStep(move);
         var fromElement = _matrix.GetByCoordinates(move.FromCoordinate);
         var toElement = _matrix.GetByCoordinates(move.ToCoordinate);

         _matrix.SetByCoordinates(move.ToCoordinate, fromElement);
         _matrix.SetByCoordinates(move.FromCoordinate, toElement);
         ProcessField();
     }

     public void ActivateBonus(Bonus bonus, Coordinate? coordinate, MoveOption? move)
     {
         switch (bonus)
         {
             case TypeRemover typeRemover:
                 RemoveByType(_matrix.GetByCoordinates(coordinate ??
                                                       throw new ArgumentException(
                                                           "Coordinate can't be null for RemoveByType bonus")));
                 _statistics.AccountBonusUse(bonus, coordinate);
                 break;
             case LaneRemover laneRemover:
                 RemoveLane(move ?? throw new ArgumentException("Move can't be null for RemoveLane bonus"));
                 _statistics.AccountBonusUse(bonus, move.FromCoordinate);
                 break;
         }

         ProcessField();
     }

     private void ProcessField()
     {
         var result = _iterator.ProcessMatches();
         _statistics.AccountCombinations(result.AllMatches);

         if (result.TotalElementsRemoved > 10)
         {
             AddBonus(typeof(TypeRemover));
         }

         if (result.AllMatches.Find(row => row.Count > 5) != null)
         {
             AddBonus(typeof(LaneRemover));
         }
     }

     public void AddBonus(Type bonus)
     {
         ++_bonuses[bonus];
     }

     private void RemoveLane(MoveOption move)
     {
         if (move.FromCoordinate.ColIndex == move.ToCoordinate.ColIndex)
         {
             int columnIndex = move.FromCoordinate.ColIndex;
             for (var rowIndex = 0; rowIndex < 8; ++rowIndex)
             {
                 var coordinate = new Coordinate(rowIndex, columnIndex);
                 _matrix.SetByCoordinates(coordinate, new EmptyElement());
             }
         }

         if (move.FromCoordinate.RowIndex == move.ToCoordinate.RowIndex)
         {
             int rowIndex = move.ToCoordinate.RowIndex;
             for (var columnIndex = 0; columnIndex < 8; ++columnIndex)
             {
                 var coordinate = new Coordinate(rowIndex, columnIndex);
                 _matrix.SetByCoordinates(coordinate, new EmptyElement());
             }
         }
     }

     private void RemoveByType(MatrixElement elementOfType)
     {
         for (int i = 0; i < 8; i++)
         {
             Console.Write(i + 1 + " ");
             for (int j = 0; j < 8; j++)
             {
                 var coordinate = new Coordinate(i, j);
                 if (_matrix.GetByCoordinates(coordinate).Equals(elementOfType))
                     _matrix.SetByCoordinates(coordinate, new EmptyElement());
             }
         }
     }

     public MatrixElement[][] GetMatrixField()
     {
         return _matrix.GetField();
     }

     public Dictionary<Bonus, int> GetBonuses()
     {
         return new Dictionary<Bonus, int>
         {
             { new TypeRemover(), _bonuses[typeof(TypeRemover)] },
             { new LaneRemover(), _bonuses[typeof(LaneRemover)] }
         };
     }
 }
 **/
public class FunctionalMatrix
{
    private readonly MatrixElement[][] _field;
    private readonly Dictionary<Type, int> _bonuses;

    public FunctionalMatrix(MatrixElement[][] field, Dictionary<Type, int>? bonuses)
    {
        if (field.Length < 2) throw new ArgumentException("Размер матрицы не может быть меньше 2x2");
        _field = field;
        _bonuses = bonuses ?? new Dictionary<Type, int>();
    }

    // Getters
    public MatrixElement[][] GetField() => _field;

    public MatrixElement GetByCoordinates(Coordinate coordinate) =>
        _field[coordinate.RowIndex][coordinate.ColIndex];

    public Dictionary<Bonus, int> GetBonuses()
    {
        return new Dictionary<Bonus, int>
        {
            { new TypeRemover(), _bonuses[typeof(TypeRemover)] },
            { new LaneRemover(), _bonuses[typeof(LaneRemover)] }
        };
    }

    // Setters
    public FunctionalMatrix SetByCoordinates(Coordinate coordinate, MatrixElement element)
    {
        var newField = _field.Select((row, rowIndex) =>
            rowIndex == coordinate.RowIndex
                ? row.Select((cell, colIndex) =>
                    colIndex == coordinate.ColIndex ? element : cell).ToArray()
                : row.ToArray()
        ).ToArray();

        return new FunctionalMatrix(newField, _bonuses);
    }

    public FunctionalMatrix SwitchPlaces(MoveOption move)
    {
        var fromElement = GetByCoordinates(move.FromCoordinate);
        var toElement = GetByCoordinates(move.ToCoordinate);

        var newMatrix = SetByCoordinates(move.ToCoordinate, fromElement);
        newMatrix = newMatrix.SetByCoordinates(move.FromCoordinate, toElement);

        return newMatrix.ProcessField();
    }

    public FunctionalMatrix ActivateBonus(Bonus bonus, Coordinate? coordinate, MoveOption? move)
    {
        FunctionalMatrix newMatrix = bonus switch
        {
            TypeRemover => RemoveByType(GetByCoordinates(coordinate ??
                                                         throw new ArgumentException(
                                                             "Coordinate can't be null for RemoveByType bonus"))),
            LaneRemover => RemoveLane(move ??
                                      throw new ArgumentException("Move can't be null for RemoveLane bonus")),
            _ => this
        };

        return newMatrix.ProcessField();
    }

    public FunctionalMatrix AddBonus(Type bonus)
    {
        var newBonuses = new Dictionary<Type, int>(_bonuses);
        newBonuses[bonus] = newBonuses.ContainsKey(bonus) ? newBonuses[bonus] + 1 : 1;

        return new FunctionalMatrix(_field, newBonuses);
    }

    private FunctionalMatrix ProcessField()
    {
        // Логика итератора должна возвращать новый FunctionalMatrix
        return this;
    }

    private FunctionalMatrix RemoveLane(MoveOption move)
    {
        FunctionalMatrix result = this;

        if (move.FromCoordinate.ColIndex == move.ToCoordinate.ColIndex)
        {
            int columnIndex = move.FromCoordinate.ColIndex;
            result = Enumerable.Range(0, 8)
                .Aggregate(result, (matrix, rowIndex) =>
                    matrix.SetByCoordinates(new Coordinate(rowIndex, columnIndex), new EmptyElement()));
        }

        if (move.FromCoordinate.RowIndex == move.ToCoordinate.RowIndex)
        {
            int rowIndex = move.ToCoordinate.RowIndex;
            result = Enumerable.Range(0, 8)
                .Aggregate(result, (matrix, columnIndex) =>
                    matrix.SetByCoordinates(new Coordinate(rowIndex, columnIndex), new EmptyElement()));
        }

        return result;
    }

    private FunctionalMatrix RemoveByType(MatrixElement elementOfType)
    {
        return Enumerable.Range(0, 8)
            .Aggregate(this, (matrix, i) =>
                Enumerable.Range(0, 8)
                    .Aggregate(matrix, (m, j) =>
                    {
                        var coordinate = new Coordinate(i, j);
                        return m.GetByCoordinates(coordinate).Equals(elementOfType)
                            ? m.SetByCoordinates(coordinate, new EmptyElement())
                            : m;
                    })
            );
    }
}