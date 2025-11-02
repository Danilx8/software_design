namespace Software_Design._11._Functional_Class.Mocks.MatrixElements;

public class MatrixElement
{
    public string GetName() => "";

    public bool IsEmpty()
    {
        return GetType() == typeof(EmptyElement);
    }

    public override bool Equals(object? obj)
    {
        return GetType() == obj?.GetType() && !IsEmpty();
    }
}