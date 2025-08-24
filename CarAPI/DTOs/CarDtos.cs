using System.ComponentModel.DataAnnotations;

namespace CarAPI.DTOs
{
        public class AddCarRequest 
        {
            [Required]
            [StringLength(50)]
            public string Make { get; set; } = string.Empty;

            [Required]
            [StringLength(100)]
            public string Model { get; set; } = string.Empty;

            [Required]
            [Range(1900, 2025)]
            public int Year { get; set; }

            [Range(0, int.MaxValue)]
            public int StockLevel { get; set; }

            [Required]
            [Range(0, double.MaxValue)]
            public decimal Price { get; set; }
        }

        public class UpdateCarRequest
        {
            [Range(0, int.MaxValue)]
            public int StockLevel { get; set; }
            
           [Range(0, double.MaxValue)]
            public decimal Price { get; set; }
        }

        public class LoginRequest 
        {
            [Required]
            public string Username { get; set; } = string.Empty;

            [Required]
            public string Password { get; set; } = string.Empty;
        }

        public class RegisterRequest
        {
            [Required]
            [StringLength(50)]
            public string Username { get; set; } = string.Empty;

            [Required]
            [MinLength(6)]
            public string Password { get; set; } = string.Empty;

            [Required]
            [StringLength(100)]
            public string CompanyName { get; set; } = string.Empty;
        }

        public class ApiResponse<T> // General wrapper for responses
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public T? Data { get; set; }
            public List<string> Errors { get; set; } = new();
        }

        public class CarResponse // Response for when Cars are fetched
        {
            public int Id { get; set; }
            public string Make { get; set; } = string.Empty;
            public string Model { get; set; } = string.Empty;
            public int Year { get; set; }
            public int StockLevel { get; set; }
            public decimal Price { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        public class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
            public string CompanyName { get; set; } = string.Empty;
            public DateTime ExpiresAt { get; set; }
    }
}
