using CarAPI.Data;
using CarAPI.DTOs;
using CarAPI.Models;
using Dapper;

namespace CarAPI.Services
{
    public class CarService
    {
        private readonly DatabaseService _dbService;

        public CarService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<ApiResponse<CarResponse>> AddCarAsync(int dealerId, AddCarRequest request)
        {
            using var connection = _dbService.CreateConnection();

            var id = await connection.QuerySingleAsync<int>(@"
                INSERT INTO Cars (DealerId, Make, Model, Year, StockLevel, Price, CreatedAt, UpdatedAt)
                VALUES (@DealerId, @Make, @Model, @Year, @StockLevel, @Price, @CreatedAt, @UpdatedAt);
                SELECT last_insert_rowid();",
                new
                {
                    DealerId = dealerId,
                    request.Make,
                    request.Model,
                    request.Year,
                    request.StockLevel,
                    request.Price,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });

            var car = await GetCarByIdAsync(dealerId, id);

            return new ApiResponse<CarResponse>
            {
                Success = true,
                Message = "Car added successfully",
                Data = car.Data
            };
        }

        public async Task<ApiResponse<bool>> RemoveCarAsync(int dealerId, int carId)
        {
            using var connection = _dbService.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(
                "DELETE FROM Cars WHERE Id = @Id AND DealerId = @DealerId",
                new { Id = carId, DealerId = dealerId });

            if (rowsAffected == 0)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Car not found"
                };
            }

            return new ApiResponse<bool>
            {
                Success = true,
                Message = "Car removed successfully",
                Data = true
            };
        }

        public async Task<ApiResponse<List<CarResponse>>> GetCarsByMakeAndModelAsync(int dealerId, string? make, string? model)
        {
            using var connection = _dbService.CreateConnection();

            var sql = "SELECT * FROM Cars WHERE DealerId = @DealerId";
            var parameters = new { DealerId = dealerId, Make = make, Model = model };

            if (!string.IsNullOrEmpty(make))
                sql += " AND LOWER(Make) LIKE LOWER('%' || @Make || '%')";

            if (!string.IsNullOrEmpty(model))
                sql += " AND LOWER(Model) LIKE LOWER('%' || @Model || '%')";

            sql += " ORDER BY Make, Model";

            var cars = await connection.QueryAsync<Car>(sql, parameters);

            var carResponses = cars.Select(c => new CarResponse
            {
                Id = c.Id,
                Make = c.Make,
                Model = c.Model,
                Year = c.Year,
                StockLevel = c.StockLevel,
                Price = c.Price,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList();

            return new ApiResponse<List<CarResponse>>
            {
                Success = true,
                Data = carResponses
            };
        }

        public async Task<ApiResponse<CarResponse>> GetCarByIdAsync(int dealerId, int carId)
        {
            using var connection = _dbService.CreateConnection();

            var car = await connection.QueryFirstOrDefaultAsync<Car>(
                "SELECT * FROM Cars WHERE Id = @Id AND DealerId = @DealerId",
                new { Id = carId, DealerId = dealerId });

            if (car == null)
            {
                return new ApiResponse<CarResponse>
                {
                    Success = false,
                    Message = "Car not found"
                };
            }

            return new ApiResponse<CarResponse>
            {
                Success = true,
                Data = new CarResponse
                {
                    Id = car.Id,
                    Make = car.Make,
                    Model = car.Model,
                    Year = car.Year,
                    StockLevel = car.StockLevel,
                    Price = car.Price,
                    CreatedAt = car.CreatedAt,
                    UpdatedAt = car.UpdatedAt
                }
            };
        }

        public async Task<ApiResponse<CarResponse>> UpdateCarAsync(int dealerId, int carId, UpdateCarRequest request)
        {
            using var connection = _dbService.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync(
                "UPDATE Cars SET StockLevel = @StockLevel, price = @price, UpdatedAt = @UpdatedAt WHERE Id = @Id AND DealerId = @DealerId",
                new { StockLevel = request.StockLevel, price = request.Price, UpdatedAt = DateTime.Now, Id = carId, DealerId = dealerId });

            if (rowsAffected == 0)
            {
                return new ApiResponse<CarResponse>
                {
                    Success = false,
                    Message = "Car not found"
                };
            }

            var car = await GetCarByIdAsync(dealerId, carId);
            return new ApiResponse<CarResponse>
            {
                Success = true,
                Message = "Car updated successfully",
                Data = car.Data
            };
        }

        
    }
}
