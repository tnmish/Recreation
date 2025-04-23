using Recreation.Data.Entities;
using Recreation.Data.Enums;

namespace Recreation.Data.Repository
{
    public interface IRepository
    {
        Task<IEnumerable<RecreationItem>> GetRecreationItemsAsync();

        Task AddRecreationItemAsync(RecreationItem item);

        Task<IEnumerable<ItemType>> GetItemTypeAsync(RecreationType type);
    }
}
