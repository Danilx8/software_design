namespace Software_Design._1._Storage;

public class Example
{
    public void Run()
    {
        var _storage = new Storage();
        _storage.Save("first");
        _storage.Save("second");

        // Если база свежая то будут именно такие айдишники
        Console.WriteLine(_storage.Retrieve(1));
        Console.WriteLine(_storage.Retrieve(2));
    }
}