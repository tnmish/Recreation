using Recreation.Data.Enums;
using System.Text.Json.Serialization;

namespace Recreation.Data.Entities
{   
    public class ItemType
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Применяемый тип
        /// </summary>
        public RecreationType Type { get; set; }
    }
}
