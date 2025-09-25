using Microsoft.Data.SqlClient;

namespace Software_Design._1._Storage;

// JDBC - чисто джавовская тема, использую Microsoft.Data.SqlClient

// Допустим, у нас есть таблица phrases, состоящая из полей id и text

public class Storage : IStorage
{
    private const string ConnectionString =
        "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;";

    public void Save(string data)
    {
        using SqlConnection connection = new SqlConnection(ConnectionString);
        string query = $"INSERT INTO phrases values {data}";
        connection.Open();
        SqlCommand cmd = new SqlCommand(query);
        cmd.ExecuteReader();

        // здесь можно добавить логи но я не буду
    }

    public string Retrieve(int id)
    {
        using SqlConnection connection = new SqlConnection(ConnectionString);
        string query = $"SELECT * FROM phrases WHERE id = {id}";
        connection.Open();
        SqlCommand cmd = new SqlCommand(query);
        cmd.ExecuteReader();
        
        return Convert.ToString(cmd.ExecuteScalar()) ?? "";
    }
}