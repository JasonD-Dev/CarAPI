using CarAPI.DTOs;
using CarAPI.Models;

namespace CarAPI.UnitTests
{
    public class DataHelper
    {

        
        public static Dealer CreateTestDealer(int id = 1)
        {
            return new Dealer
            {
                Id = id,
                Username = "testdealer",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                CompanyName = "test",
                CreatedAt = DateTime.Now
            };
        }

        public static Car CreateTestCar(int id = 1)
        {
            return new Car
            {
                DealerId = 1,
                Id = id,
                Make = "toyota",
                Model = "camry",
                Year = 2023,
                StockLevel = 20,
                Price = Convert.ToDecimal(250000.50),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        public static AddCarRequest CreateAddCarRequest()
        {
            return new AddCarRequest();
        }
        public static LoginRequest CreateLoginRequest()
        {
            return new LoginRequest();
        }
        public static RegisterRequest CreateRegisterRequest()
        {
            return new RegisterRequest();
        }

        public static UpdateCarRequest CreateUpdateCarRequest()
        {
            return new UpdateCarRequest();
        }


    }
}
