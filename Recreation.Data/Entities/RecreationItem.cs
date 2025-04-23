using NetTopologySuite.Geometries;
using Recreation.Data.Enums;
using System.Text.Json.Serialization;

namespace Recreation.Data.Entities
{
    [JsonDerivedType(typeof(Park))]
    [JsonDerivedType(typeof(BikeLane))]
    [JsonDerivedType(typeof(RecreationArea))]
    public abstract class RecreationItem
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Тип объекта
        /// </summary>
        public RecreationType Type { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Тип велодолрожки или парка
        /// </summary>
        public Guid? ItemTypeId { get; set; }

        /// <summary>
        /// Тип велодолрожки или парка
        /// </summary>
        public virtual ItemType? ItemType { get; set; }

        /// <summary>
        /// Координаты
        /// </summary>        
        public Geometry Geometry { get; set; } = default!;
    }
}