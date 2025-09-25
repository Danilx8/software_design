namespace Software_Design._1._Storage;

public interface IStorage
{
    public void Save(string data);
    public string Retrieve(int id);
}