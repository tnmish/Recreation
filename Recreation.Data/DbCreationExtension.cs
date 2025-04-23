using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Recreation.Data.Entities;

namespace Recreation.Data
{
    public static class DbCreationxtension
    {
        public static void DatabaseCreate<T>(this IApplicationBuilder app) where T : DbContext
        {
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<T>();
            if (context.Database.EnsureCreated())
            {
                var items = new List<ItemType>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Спортивный",
                        Type = Enums.RecreationType.Park
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Прогулочный",
                        Type = Enums.RecreationType.Park
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Выставочный",
                        Type = Enums.RecreationType.Park
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Зоологический",
                        Type = Enums.RecreationType.Park
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Ботанический",
                        Type = Enums.RecreationType.Park
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Этнографический",
                        Type = Enums.RecreationType.Park
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Мемориальный",
                        Type = Enums.RecreationType.Park
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Односторонняя",
                        Type = Enums.RecreationType.BikeLane
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Двусторонняя",
                        Type = Enums.RecreationType.BikeLane
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Изолированная",
                        Type = Enums.RecreationType.BikeLane
                    }
                };

                context.Set<ItemType>().AddRange(items);
                context.SaveChanges();
            }
        }
    }
}
