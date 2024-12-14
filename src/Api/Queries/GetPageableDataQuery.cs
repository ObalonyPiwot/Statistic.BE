namespace Api.Queries
{
    public enum GraphTypes
    {
        Bar,
        Line,
        Pie,
        Doughnut
    }
    public class GetPageableDataQuery
    {
    }
    public class DataViewModel
    {
        public string Type{ get; set; }
        public string Name{ get; set; }
        public string Title{ get; set; }
        public List<float> Data { get; set; }
        public List<string> Labels { get; set; }
    }
    public class CsvModel
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Age { get; set; }
        public string KmDriven { get; set; }
        public string Transmission { get; set; }
        public string Owner { get; set; }
        public string FuelType { get; set; }
        public string PostedDate { get; set; }
        public string AskPrice { get; set; }
    }
    
    public class BrandModel
    {
        public string Brand { get; set; }
        public List<string> Model { get; set; }
    }
    public class SelectListFromCsvModel
    {
        public List<string> Brand { get; set; }
        public List<BrandModel> Model { get; set; }
        public List<string> Transmission { get; set; }
        public List<string> Owner { get; set; }
        public List<string> FuelType { get; set; }
    }

}
