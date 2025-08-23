using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CarAPI.Services;
using CarAPI.DTOs;
using System.Security.Claims;

namespace CarAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CarsController : ControllerBase
    {
        private readonly CarService _carService;

        public CarsController(CarService carService)
        {
            _carService = carService;
        }

        private int GetDealerId()
        {
            var dealerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(dealerIdClaim ?? "0");
        }

        [HttpGet]
        public async Task<IActionResult> GetCars()
        {
            var dealerId = GetDealerId();
            var result = await _carService.GetCarsAsync(dealerId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCar(int id)
        {
            var dealerId = GetDealerId();
            var result = await _carService.GetCarByIdAsync(dealerId, id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddCar([FromBody] AddCarRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid input",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var dealerId = GetDealerId();
            var result = await _carService.AddCarAsync(dealerId, request);
            return result.Success ? CreatedAtAction(nameof(GetCar), new { id = result.Data?.Id }, result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveCar(int id)
        {
            var dealerId = GetDealerId();
            var result = await _carService.RemoveCarAsync(dealerId, id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid input",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var dealerId = GetDealerId();
            var result = await _carService.UpdateStockLevelAsync(dealerId, id, request);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCars([FromQuery] string? make, [FromQuery] string? model)
        {
            var dealerId = GetDealerId();
            var result = await _carService.SearchCarsAsync(dealerId, make, model);
            return Ok(result);
        }
    }
}
