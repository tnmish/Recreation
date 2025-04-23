using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Recreation.Converters;
using Recreation.Data.Entities;
using Recreation.Data.Repository;
using Recreation.Models;
using System.Diagnostics;
using System.Text.Json;

namespace Recreation.Controllers
{
    public class HomeController(ILogger<HomeController> logger,
        IRepository repository) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly IRepository _repository = repository;

        private static JsonSerializerOptions GetSerializerOptions()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            serializeOptions.Converters.Add(new GeometryJsonConverter());
            return serializeOptions;
        }

        /// <summary>
        /// Главная страница
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var parkItems = await _repository.GetItemTypeAsync(Data.Enums.RecreationType.Park);
            var bikeLaneItems = await _repository.GetItemTypeAsync(Data.Enums.RecreationType.BikeLane);

            ViewData["parkItems"] = new SelectList(parkItems, "Id", "Name");
            ViewData["bikeLaneItems"] = new SelectList(bikeLaneItems, "Id", "Name");

            _logger.Log(LogLevel.Information, "{Path}", Request.Path);
            return View();
        }

        /// <summary>
        /// Получить список рекреационных объектов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var result = await _repository.GetRecreationItemsAsync();
            var jsonString = JsonSerializer.Serialize(result, GetSerializerOptions());

            _logger.Log(LogLevel.Information, "{Path}", Request.Path);
            return Ok(jsonString);
        }

        /// <summary>
        /// Добавить парк
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddPark([FromBody] AddParkModel dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto.Geometry is null)
            {
                return BadRequest(dto.Geometry);
            }

            try
            {
                var geoJsonReader = new GeoJsonReader();
                var geometry = geoJsonReader.Read<Geometry>(dto.Geometry);

                var recreationItem = new Park()
                {
                    Id = Guid.NewGuid(),
                    Type = Data.Enums.RecreationType.Park,
                    Name = dto.Name,
                    ItemTypeId = dto.ItemTypeId,
                    Geometry = geometry,
                    Square = dto.Square,
                };

                await _repository.AddRecreationItemAsync(recreationItem);
                return Json(new { success = true });
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                return BadRequest(dto.Geometry);
            }            
        }

        /// <summary>
        /// Добавить велодорожку
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddBikeLane([FromBody] AddBikeLaneModel dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto.Geometry is null)
            {
                return BadRequest(dto.Geometry);
            }

            try
            {
                var geoJsonReader = new GeoJsonReader();
                var geometry = geoJsonReader.Read<Geometry>(dto.Geometry);

                var recreationItem = new BikeLane()
                {
                    Id = Guid.NewGuid(),
                    Type = Data.Enums.RecreationType.BikeLane,
                    Name = dto.Name,
                    ItemTypeId = dto.ItemTypeId,
                    Geometry = geometry,
                    Length = dto.Length,
                };

                await _repository.AddRecreationItemAsync(recreationItem);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(dto.Geometry);
            }
        }

        /// <summary>
        /// Добавить зону отдыха
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddRecreationArea([FromBody] AddRecreationAreaModel dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto.Geometry is null)
            {
                return BadRequest(dto.Geometry);
            }

            try
            {
                var geoJsonReader = new GeoJsonReader();
                var geometry = geoJsonReader.Read<Geometry>(dto.Geometry);

                var recreationItem = new RecreationArea()
                {
                    Id = Guid.NewGuid(),
                    Type = Data.Enums.RecreationType.RecreationArea,
                    Name = dto.Name,
                    Geometry = geometry,
                    Director = dto.Director,
                    PricePerHour = dto.PricePerHour,
                };

                await _repository.AddRecreationItemAsync(recreationItem);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(dto.Geometry);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.Log(LogLevel.Information, "{Path}", Request.Path);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
