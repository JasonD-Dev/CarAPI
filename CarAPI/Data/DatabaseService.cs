using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace CarAPI.Data
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Database") ?? "Data Source=carstock.db";
        }

        public async Task InitializeDatabaseAsync()
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var createDealersTable = @"
                CREATE TABLE IF NOT EXISTS Dealers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    CompanyName TEXT NOT NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                )";

            var createCarsTable = @"
                CREATE TABLE IF NOT EXISTS Cars (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    DealerId INTEGER NOT NULL,
                    Make TEXT NOT NULL,
                    Model TEXT NOT NULL,
                    Year INTEGER NOT NULL,
                    StockLevel INTEGER NOT NULL DEFAULT 0,
                    Price DECIMAL(10,2) NOT NULL,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (DealerId) REFERENCES Dealers (Id)
                )";

            var createIndexes = @"
                CREATE INDEX IF NOT EXISTS idx_cars_dealer_id ON Cars(DealerId);
                CREATE INDEX IF NOT EXISTS idx_cars_make_model ON Cars(Make, Model);
            ";

            await connection.ExecuteAsync(createDealersTable);
            await connection.ExecuteAsync(createCarsTable);
            await connection.ExecuteAsync(createIndexes);
        }

        public IDbConnection CreateConnection() => new SqliteConnection(_connectionString);
    }
}
