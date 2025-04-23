using System.Text.Json.Serialization;

namespace Recreation.Data.Entities
{    
    public class Park : RecreationItem
    {
        /// <summary>
        /// Площадь парка
        /// </summary>
        public float Square { get; set; }
    }
}