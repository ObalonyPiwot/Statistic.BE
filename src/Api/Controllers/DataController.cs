﻿using Microsoft.AspNetCore.Mvc;
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
                       (!request.Brand.Any() || request.Brand.Contains(x.Brand)) &&
                       (!request.Model.Any() || request.Model.Contains(x.Model)) &&
                       (!request.Transmission.Any() || request.Transmission.Contains(x.Transmission)) &&
                       (!request.Owner.Any() || request.Owner.Contains(x.Owner)) &&
                       (!request.FuelType.Any() || request.FuelType.Contains(x.FuelType)) &&
                       (request.YearFrom == null || x.Year >= request.YearFrom) &&
                       (request.YearTo == null || x.Year <= request.YearTo) &&
                       (request.KmDrivenFrom == null || x.KmDrivenVal >= request.KmDrivenFrom) &&
                       (request.KmDrivenTo == null || x.KmDrivenVal <= request.KmDrivenTo)
                   )
                   .ToList();
            }
            List<string> labels;
            if (request.Model.Any())
                labels = request.Model;
            else if (request.Brand.Any())
                labels = request.Brand;
            else
                labels = csvModels.Select(x=>x.Brand).Distinct().ToList();
            var data = new List<float>();
            foreach (var label in labels)
            {
                if (request.Model.Any())
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
                Brand = csvModels.Select(x => x.Brand).Distinct().ToList(),
                Transmission = csvModels.Select(x => x.Transmission).Distinct().ToList(),
                Owner = csvModels.Select(x => x.Owner).Distinct().ToList(),
                FuelType = csvModels.Select(x => x.FuelType).Distinct().ToList(),
            };
            var carModel = new List<BrandModel>();
            foreach(var brand in result.Brand)
            {
                carModel.Add(new BrandModel()
                {
                    Brand = brand,
                    Model = csvModels.Where(x => x.Brand == brand).Select(x => x.Model).Distinct().ToList()
                });

            }
            result.Model = carModel;
            return result;
        }

    }
}
