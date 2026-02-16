using Xunit;
using Recreation.Data.Repository;
using Recreation.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Recreation.Data.Entities;
using System;

namespace Recreation.Tests
{
    public class RepositoryTests
    {
        private DbContextOptions<RecreationContext> GetDbContextOptions()
        {
            return new DbContextOptionsBuilder<RecreationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task Create_ShouldAddEntityToDatabase()
        {
            var options = GetDbContextOptions();
            
            using var context = new RecreationContext(options);
            var repository = new Repository<RecreationArea>(context);

            var entity = new RecreationArea
            {
                Name = "Test Recreation Area",
                Description = "A test area",
                Geometry = null // Simplified for test
            };

            var result = await repository.Create(entity);

            Assert.NotNull(result);
            Assert.Equal("Test Recreation Area", result.Name);
            Assert.Equal(1, await context.RecreationAreas.CountAsync());
        }

        [Fact]
        public async Task GetById_ShouldReturnCorrectEntity()
        {
            var options = GetDbContextOptions();
            
            using var context = new RecreationContext(options);
            var repository = new Repository<RecreationArea>(context);

            var entity = new RecreationArea
            {
                Name = "Test Recreation Area",
                Description = "A test area",
                Geometry = null
            };
            context.RecreationAreas.Add(entity);
            await context.SaveChangesAsync();

            var result = await repository.GetById(entity.Id);

            Assert.NotNull(result);
            Assert.Equal("Test Recreation Area", result.Name);
        }

        [Fact]
        public async Task Update_ShouldModifyExistingEntity()
        {
            var options = GetDbContextOptions();
            
            using var context = new RecreationContext(options);
            var repository = new Repository<RecreationArea>(context);

            var entity = new RecreationArea
            {
                Name = "Original Name",
                Description = "Original description",
                Geometry = null
            };
            context.RecreationAreas.Add(entity);
            await context.SaveChangesAsync();

            entity.Name = "Updated Name";
            var result = await repository.Update(entity);

            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);

            var updatedEntity = await context.RecreationAreas.FindAsync(entity.Id);
            Assert.Equal("Updated Name", updatedEntity.Name);
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntityFromDatabase()
        {
            var options = GetDbContextOptions();
            
            using var context = new RecreationContext(options);
            var repository = new Repository<RecreationArea>(context);

            var entity = new RecreationArea
            {
                Name = "Test Recreation Area",
                Description = "A test area",
                Geometry = null
            };
            context.RecreationAreas.Add(entity);
            await context.SaveChangesAsync();

            var result = await repository.Delete(entity.Id);

            Assert.True(result);
            Assert.Equal(0, await context.RecreationAreas.CountAsync());
        }
    }
}