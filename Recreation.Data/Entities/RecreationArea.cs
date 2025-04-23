namespace Recreation.Data.Entities
{
    public class RecreationArea : RecreationItem
    {
        /// <summary>
        /// Имя директора
        /// </summary>
        public string Director { get; set; } = default!;

        /// <summary>
        /// Цена за час
        /// </summary>
        public decimal PricePerHour { get; set; }
    }
}