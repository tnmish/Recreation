using System.Text.Json.Serialization;

namespace Recreation.Data.Entities
{   
    public class BikeLane : RecreationItem
    {
        /// <summary>
        /// Длина велодорожки
        /// </summary>
        public float Length { get; set; }
    }
}
