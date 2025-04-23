namespace Recreation.Models
{
    public class AddRecreationAreaModel
    {
        public string Name { get; set; } = string.Empty;

       public string Geometry { get; set; } = string.Empty;

        public string Director { get; set; } = string.Empty;

        public decimal PricePerHour { get; set; }
    }
}