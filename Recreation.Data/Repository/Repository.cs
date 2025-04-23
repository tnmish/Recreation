using Microsoft.Extensions.Configuration;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.HPRtree;
using NetTopologySuite.IO;
using Npgsql;
using Recreation.Data.Entities;
using Recreation.Data.Enums;
using System;
namespace Recreation.Data.Repository
{
    public class Repository(IConfiguration configuration) : IRepository
    {
        private readonly string connectionString = configuration.GetConnectionString("RecreationDatabase")
            ?? throw new ArgumentNullException("RecreationDatabase");

        private readonly WKTWriter wktWriter = new();
        private readonly WKTReader wktReader = new();

        public async Task AddRecreationItemAsync(RecreationItem item)
        {
            //var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);
            string geometry = wktWriter.Write(item.Geometry);

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(@"
                INSERT INTO recr.""RecreationItem"" (""Id"", ""Type"", ""Name"", ""ItemTypeId"", ""Geometry"", ""Length"", ""Square"", ""Director"", ""PricePerHour"")
                VALUES (@Id, @Type, @Name, @ItemTypeId, ST_GeomFromText(@Geometry, 4326), @Length, @Square, @Director, @PricePerHour);", connection);

            command.Parameters.AddWithValue("Id", item.Id);
            command.Parameters.AddWithValue("Type", (short)item.Type);
            command.Parameters.AddWithValue("Name", item.Name);
            command.Parameters.AddWithValue("ItemTypeId", item.ItemTypeId.HasValue ? item.ItemTypeId.Value : DBNull.Value);
            command.Parameters.AddWithValue("Geometry", geometry);

            if (item is Park parkItem)
            {
                command.Parameters.AddWithValue("Length", DBNull.Value);
                command.Parameters.AddWithValue("Square", parkItem.Square);
                command.Parameters.AddWithValue("Director", DBNull.Value);
                command.Parameters.AddWithValue("PricePerHour", DBNull.Value);
            }
            else if (item is BikeLane bikeLaneItem)
            {
                command.Parameters.AddWithValue("Length", bikeLaneItem.Length);
                command.Parameters.AddWithValue("Square", DBNull.Value);
                command.Parameters.AddWithValue("Director", DBNull.Value);
                command.Parameters.AddWithValue("PricePerHour", DBNull.Value);
            }
            else if (item is RecreationArea recreationItem)
            {
                command.Parameters.AddWithValue("Length", DBNull.Value);
                command.Parameters.AddWithValue("Square", DBNull.Value);
                command.Parameters.AddWithValue("Director", recreationItem.Director);
                command.Parameters.AddWithValue("PricePerHour", recreationItem.PricePerHour);
            }

            await command.ExecuteNonQueryAsync();
            connection.Close();
        }

        public async Task<IEnumerable<RecreationItem>> GetRecreationItemsAsync()
        {
            using var connection = new NpgsqlConnection(connectionString);
            var result = new List<RecreationItem>();

            connection.Open();

            using var command = new NpgsqlCommand(@"
                SELECT recr.""RecreationItem"".""Id"", recr.""RecreationItem"".""Type"", recr.""RecreationItem"".""Name"", ""ItemTypeId"", 
                    ST_AsText(""Geometry"") AS Geometry, ""Length"", ""Square"", ""Director"", ""PricePerHour"", 
                    recr.""ItemType"".""Name"" as ""ItemTypeName""
                FROM recr.""RecreationItem""
                LEFT JOIN recr.""ItemType"" ON recr.""ItemType"".""Id"" = ""ItemTypeId"";", connection);

            using var reader = command.ExecuteReader();
            while (await reader.ReadAsync())
            {
                RecreationType recreationType = (RecreationType)reader.GetInt16(1);
                Geometry geometry = wktReader.Read(reader.GetString(4));

                switch (recreationType)
                {
                    case RecreationType.Park:
                        result.Add(new Park()
                        {
                            Id = reader.GetGuid(0),
                            Type = recreationType,
                            Name = reader.GetString(2),
                            ItemTypeId = reader.IsDBNull(3) ? null : reader.GetGuid(3),
                            Geometry = geometry,
                            Square = reader.GetFloat(6),
                            ItemType = reader.IsDBNull(3) ? null : new ItemType()
                            {
                                Id = reader.GetGuid(3),
                                Name = reader.GetString(9),
                            }
                        });
                        break;
                    case RecreationType.BikeLane:
                        result.Add(new BikeLane()
                        {
                            Id = reader.GetGuid(0),
                            Type = recreationType,
                            Name = reader.GetString(2),
                            ItemTypeId = reader.IsDBNull(3) ? null : reader.GetGuid(3),
                            Geometry = geometry,
                            Length = reader.GetFloat(5),
                            ItemType = reader.IsDBNull(3) ? null : new ItemType()
                            {
                                Id = reader.GetGuid(3),
                                Name = reader.GetString(9),
                            }
                        });
                        break;
                    case RecreationType.RecreationArea:
                        result.Add(new RecreationArea()
                        {
                            Id = reader.GetGuid(0),
                            Type = recreationType,
                            Name = reader.GetString(2),
                            ItemTypeId = reader.IsDBNull(3) ? null : reader.GetGuid(3),
                            Geometry = geometry,
                            Director = reader.GetString(7),
                            PricePerHour = reader.GetDecimal(8),
                        });
                        break;
                    default: throw new InvalidDataException();
                }
            }

            connection.Close();
            return result;
        }

        public async Task<IEnumerable<ItemType>> GetItemTypeAsync(RecreationType type)
        {
            using var connection = new NpgsqlConnection(connectionString);
            var result = new List<ItemType>();

            connection.Open();

            using var command = new NpgsqlCommand(@"
                SELECT ""Id"", ""Name""
                FROM recr.""ItemType""
                WHERE ""Type"" = @Type;", connection);

            command.Parameters.AddWithValue("Type", (short)type);

            using var reader = command.ExecuteReader();
            while (await reader.ReadAsync())
            {
                result.Add(new ItemType()
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),                    
                });
            }

            connection.Close();
            return result;
        }
    }
}
