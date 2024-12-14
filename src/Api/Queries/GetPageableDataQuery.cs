using CsvHelper.Configuration;

namespace Api.Queries
{
    public enum GraphTypes
    {
        Bar,
        Line,
        Pie,
        Doughnut
    }
    public class DataViewModel
    {
        public string Type{ get; set; }
        public string Name{ get; set; }
        public string Title{ get; set; }
        public List<float> Data { get; set; }
        public List<string> Labels { get; set; }
    }

    public class DataViewModelMap : ClassMap<CsvModel>
    {
        public DataViewModelMap()
        {
            Map(m => m.Brand);
            Map(m => m.Model);
            Map(m => m.Year);
            Map(m => m.Age);
            Map(m => m.KmDrivenVal).Convert(args =>
            {
                var kmString = args.Row.GetField("KmDriven");
                if (kmString == null) return 0;

                kmString = kmString.Replace("km", "").Replace(",", "").Trim();
                return int.TryParse(kmString, out var km) ? km : 0;
            });
            Map(m => m.Transmission);
            Map(m => m.Owner);
            Map(m => m.FuelType);
            Map(m => m.PostedDate);
            Map(m => m.AskPrice);
        }
    }

    public class CsvModel
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Age { get; set; }
        public string KmDriven { get; set; }
        public int KmDrivenVal { get; set; }
        public string Transmission { get; set; }
        public string Owner { get; set; }
        public string FuelType { get; set; }
        public string PostedDate { get; set; }
        public string AskPrice { get; set; }
    }
    
    public class BrandModel
    {
        public string Brand { get; set; }
        public List<string> Model { get; set; } = new List<string>();
    }
    public class SelectListFromCsvModel
    {
        public List<string> Brand { get; set; } = new List<string>();
        public List<BrandModel> Model { get; set; } = new List<BrandModel>();
        public List<string> Transmission { get; set; } = new List<string>();
        public List<string> Owner { get; set; } = new List<string>();
        public List<string> FuelType { get; set; } = new List<string>();
    }
    public class GetPageableDataQuery
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public List<string> Brand { get; set; } = new List<string>();
        public List<string> Model { get; set; } = new List<string>();
        public List<string> Transmission { get; set; } = new List<string>();
        public List<string> Owner { get; set; } = new List<string>();
        public List<string> FuelType { get; set; } = new List<string>();
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public int? KmDrivenFrom { get; set; }
        public int? KmDrivenTo { get; set; }
    }

}
