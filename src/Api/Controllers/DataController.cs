using Microsoft.AspNetCore.Mvc;
using Api.Queries;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using System.Linq;

namespace MyProject.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        [HttpGet("pageabletest")]
        public async Task<ActionResult<List<DataViewModel>>> GetPageableTest([FromQuery] GetPageableDataQuery request)
        {
            var test =  new DataViewModel()
            {
                Type = GraphTypes.Bar.ToString().ToLower(),
                Name = "Bar",
                Title = "Test",
                Data = new List<float> { 1, 2, 3 },
                Labels = new List<string> { "jeden", "dwa", "trzy" },
            };
            var test2 = new DataViewModel()
            {
                Type = GraphTypes.Line.ToString().ToLower(),
                Name = "Line",
                Title = "BlaBla",
                Data = new List<float> { 1, 2, 3, 4 ,5 ,6 ,7 },
                Labels = new List<string> { "jeden", "dwa", "trzy", "aaaa", "bbbb" },
            };
            var result = new List<DataViewModel>();
            result.Add(test);
            result.Add(test2);
            return result;
        }
        [HttpGet("pageable")]
        public async Task<ActionResult<List<DataViewModel>>> GetPageable([FromQuery] GetPageableDataQuery request)
        {
            var result = new List<DataViewModel>();
            var csvModels = new List<CsvModel>();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "plik.csv");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound($"The file {filePath} does not exist.");
            }

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HeaderValidated = null,
                MissingFieldFound = null
            }))
            {
                csv.Context.RegisterClassMap<DataViewModelMap>();
                csvModels = csv.GetRecords<CsvModel>().ToList();
                csvModels = csvModels
                   .Where(x =>
                       (request.Brand == null || request.Brand.Contains(x.Brand)) &&
                       (request.Model == null || request.Model.Contains(x.Model)) &&
                       (request.Transmission == null || request.Transmission.Contains(x.Transmission)) &&
                       (request.Owner == null || request.Owner.Contains(x.Owner)) &&
                       (request.FuelType == null || request.FuelType.Contains(x.FuelType)) &&
                       (request.YearFrom == null || x.Year >= request.YearFrom) &&
                       (request.YearTo == null || x.Year <= request.YearTo) &&
                       (request.KmDrivenFrom == null || x.KmDrivenVal >= request.KmDrivenFrom) &&
                       (request.KmDrivenTo == null || x.KmDrivenVal <= request.KmDrivenTo)
                   )
                   .ToList();
            }
            List<string> labels;
            if (request.Model!= null)
                labels = request.Model;
            else if (request.Brand != null)
                labels = request.Brand;
            else
                labels = csvModels.Select(x=>x.Brand).Distinct().ToList();
            labels = labels.OrderBy(x => x).ToList();
            var data = new List<float>();
            foreach (var label in labels)
            {
                if (request.Model != null)
                    data.Add(csvModels.Where(x => x.Model == label).Count());
                else
                    data.Add(csvModels.Where(x => x.Brand == label).Count());
            }
            var val = new DataViewModel()
            {
                Type = request.Type,
                Name = request.Name,
                Title = request.Title,
                Data = data,
                Labels = labels,
            };
            result.Add(val);
            labels = csvModels.Select(x=>x.Transmission).Distinct().ToList();
            data = new List<float>();
            labels = labels.OrderBy(x => x).ToList();
            foreach (var label in labels)
            {
                    data.Add(csvModels.Where(x => x.Transmission == label).Count());
            }
            val = new DataViewModel()
            {
                Type = request.Type,
                Name = request.Name,
                Title = request.Title,
                Data = data,
                Labels = labels,
            };

            result.Add(val);
            labels = csvModels.Select(x=>x.Year.ToString()).Distinct().ToList();
            data = new List<float>();
            labels = labels.OrderBy(x => x).ToList();
            foreach (var label in labels)
            {
                data.Add(csvModels.Where(x => x.Year.ToString() == label).Count());
            }
            val = new DataViewModel()
            {
                Type = request.Type,
                Name = request.Name,
                Title = request.Title,
                Data = data,
                Labels = labels,
            };

            result.Add(val);
            labels = csvModels.Select(x=>x.FuelType).Distinct().ToList();
            data = new List<float>();
            labels = labels.OrderBy(x => x).ToList();
            foreach (var label in labels)
            {
                data.Add(csvModels.Where(x => x.FuelType == label).Count());
            }
            val = new DataViewModel()
            {
                Type = request.Type,
                Name = request.Name,
                Title = request.Title,
                Data = data,
                Labels = labels,
            };

            result.Add(val);


            if (csvModels.Any())
            {
                var kmDrivenValues = csvModels.Select(x => x.KmDrivenVal).ToList();
                var mean = kmDrivenValues.Average();//srednia
                var stdDev = Math.Sqrt(kmDrivenValues.Average(v => Math.Pow(v - mean, 2)));//odchylenie

                var statisticsLabels = new List<string> { "Średnia", "Odchylenie standardowe" };
                var statisticsData = new List<float> { (float)mean, (float)stdDev };

                var statsViewModel = new DataViewModel()
                {
                    Type = "bar",
                    Name = request.Name,
                    Title = $"{request.Title} - Dane statystyczne przebiegu",
                    Data = statisticsData,
                    Labels = statisticsLabels,
                };

                result.Add(statsViewModel);
            }

            return result;
        }

        [HttpGet("selectable")]
        public async Task<ActionResult<SelectListFromCsvModel>> GetSelectable()
        {
            var csvModels = new List<CsvModel>();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "plik.csv");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound($"The file {filePath} does not exist.");
            }

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HeaderValidated = null,
                MissingFieldFound = null
            }))
            {
                csvModels = csv.GetRecords<CsvModel>().ToList();
            }
            var result = new SelectListFromCsvModel()
            {
                Brand = csvModels.Select(x => x.Brand).Distinct().OrderBy(x=>x).ToList(),
                Transmission = csvModels.Select(x => x.Transmission).Distinct().OrderBy(x => x).ToList(),
                Owner = csvModels.Select(x => x.Owner).Distinct().OrderBy(x => x).ToList(),
                FuelType = csvModels.Select(x => x.FuelType).Distinct().OrderBy(x => x).ToList(),
            };
            var carModel = new List<BrandModel>();
            foreach(var brand in result.Brand)
            {
                carModel.Add(new BrandModel()
                {
                    Brand = brand,
                    Model = csvModels.Where(x => x.Brand == brand).Select(x => x.Model).Distinct().OrderBy(x => x).ToList()
                });

            }
            result.Model = carModel;
            return result;
        }

    }
}
