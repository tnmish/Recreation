namespace Recreation.Models
{
    public class AddParkModel
    {
        public string Name { get; set; } = string.Empty;

        public Guid ItemTypeId { get; set; }

        public string Geometry { get; set; } = string.Empty;

        public float Square { get; set; }
    }
}