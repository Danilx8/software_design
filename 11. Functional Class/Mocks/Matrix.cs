using Software_Design._11._Functional_Class.Mocks.MatrixElements;

namespace Software_Design._11._Functional_Class.Mocks;

public class Matrix
{
    public Matrix()
    {
    }

    // Getters
    public int GetSize(Matrix matrix) => 0;
    public MatrixElement GetByCoordinates(Matrix matrix, Coordinate coordinate) => new();

    // Setters
    public Matrix SetByCoordinates(Matrix matrix, Coordinate coordinate, MatrixElement element) => new();
}