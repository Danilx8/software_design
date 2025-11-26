using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;

public class AuthorsDatabase : IDisposable
{
    private static AuthorsDatabase? _instance = null;
    private static readonly object _lock = new object();
    
    private readonly SqliteConnection _db;

    // Подготовленные команды
    private readonly SqliteCommand _checkBlacklistCmd;
    private readonly SqliteCommand _insertBlacklistCmd;
    private readonly SqliteCommand _getAllBlacklistedCmd;

    private AuthorsDatabase(string databasePath = ":memory:")
    {
        // Создаем папку, если путь не ":memory:"
        if (databasePath != ":memory:")
        {
            var dbDir = Path.GetDirectoryName(databasePath);
            if (!string.IsNullOrEmpty(dbDir) && !Directory.Exists(dbDir))
            {
                Directory.CreateDirectory(dbDir);
                Console.WriteLine($"Created directory: {dbDir}");
            }
        }

        // Создаем подключение
        var connectionString = databasePath == ":memory:" 
            ? "Data Source=:memory:" 
            : $"Data Source={databasePath}";
        
        _db = new SqliteConnection(connectionString);
        _db.Open();

        // Создаем таблицу с индексом
        using (var cmd = _db.CreateCommand())
        {
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS blacklist (
                    entry TEXT PRIMARY KEY
                );
                CREATE INDEX IF NOT EXISTS idx_blacklist_entry ON blacklist(entry);
            ";
            cmd.ExecuteNonQuery();
        }

        // Подготавливаем команды
        _checkBlacklistCmd = _db.CreateCommand();
        _checkBlacklistCmd.CommandText = "SELECT EXISTS (SELECT 1 FROM blacklist WHERE entry = @entry)";
        _checkBlacklistCmd.Parameters.Add("@entry", SqliteType.Text);
        _checkBlacklistCmd.Prepare();

        _insertBlacklistCmd = _db.CreateCommand();
        _insertBlacklistCmd.CommandText = "INSERT OR IGNORE INTO blacklist (entry) VALUES (@entry)";
        _insertBlacklistCmd.Parameters.Add("@entry", SqliteType.Text);
        _insertBlacklistCmd.Prepare();

        _getAllBlacklistedCmd = _db.CreateCommand();
        _getAllBlacklistedCmd.CommandText = "SELECT entry FROM blacklist";
        _getAllBlacklistedCmd.Prepare();
    }

    /// <summary>
    /// Получить единственный экземпляр класса (Singleton)
    /// </summary>
    public static AuthorsDatabase GetInstance(string databasePath = ":memory:")
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new AuthorsDatabase(databasePath);
                }
            }
        }
        return _instance;
    }

    /// <summary>
    /// Проверяет, разрешён ли автор (т.е. его нет в чёрном списке)
    /// </summary>
    public bool IsAuthorAllowed(string authorId)
    {
        return !IsInBlacklist(authorId);
    }

    /// <summary>
    /// Проверяет, находится ли автор в чёрном списке
    /// </summary>
    public bool IsInBlacklist(string authorId)
    {
        _checkBlacklistCmd.Parameters["@entry"].Value = authorId;
        var result = _checkBlacklistCmd.ExecuteScalar();
        return Convert.ToInt64(result) == 1;
    }

    /// <summary>
    /// Добавить автора в чёрный список (если ещё не добавлен)
    /// </summary>
    public void AddToBlacklist(string authorId)
    {
        _insertBlacklistCmd.Parameters["@entry"].Value = authorId;
        _insertBlacklistCmd.ExecuteNonQuery();
    }

    /// <summary>
    /// Получить все записи из чёрного списка
    /// </summary>
    public List<string> GetAllBlacklisted()
    {
        var result = new List<string>();
        
        using (var reader = _getAllBlacklistedCmd.ExecuteReader())
        {
            while (reader.Read())
            {
                result.Add(reader.GetString(0));
            }
        }
        
        return result;
    }

    /// <summary>
    /// Периодическая очистка WAL-файла
    /// </summary>
    public void Checkpoint()
    {
        using (var cmd = _db.CreateCommand())
        {
            cmd.CommandText = "PRAGMA wal_checkpoint(TRUNCATE);";
            cmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Закрыть соединение с базой данных
    /// </summary>
    public void Close()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (_db != null)
        {
            Checkpoint(); // Очистка WAL перед закрытием
            
            _checkBlacklistCmd?.Dispose();
            _insertBlacklistCmd?.Dispose();
            _getAllBlacklistedCmd?.Dispose();
            
            _db.Close();
            _db.Dispose();
            
            lock (_lock)
            {
                _instance = null;
            }
        }
    }
}