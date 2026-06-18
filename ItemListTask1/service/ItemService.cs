// =============================================
// FILE: ItemListTask1\Service\IItemService.cs
// FILE: ItemListTask1\Service\ItemService.cs
// PROJECT: FoodOrderingSystem (MVC project)
// =============================================

using ItemListDal;
using ItemListModel.Exceptions;
using ItemListModel.Model;
using ItemListModel.ViewModel;
using System.Collections.Generic;

namespace ItemListTask1.Service
{
    public interface IItemService
    {
        List<FoodCategory> GetFilterValues();
        ItemViewModel GetItems(ItemViewModel filter);
        ItemViewModel GetItemById(int id);
        bool SaveItem(ItemViewModel vm);
        bool DeleteItem(int id);
    }

    public class ItemService : IItemService
    {
        private readonly ItemRepository _repo;

        public ItemService()
        {
            _repo = new ItemRepository();
        }

        public List<FoodCategory> GetFilterValues()
        {
            // No business rules — just delegate
            return _repo.GetFilterValues();
        }

        public ItemViewModel GetItems(ItemViewModel filter)
        {
            if (filter.pageSize <= 0 || filter.pageSize > 200)
                throw new BusinessException("Page size must be between 1 and 200.", "INVALID_PAGE_SIZE");

            return _repo.GetList(filter);
        }

        public ItemViewModel GetItemById(int id)
        {
            // DAL throws NotFoundException if not found → no log, friendly message
            return _repo.LoadItem(id);
        }

        public bool SaveItem(ItemViewModel vm)
        {
            // ── Business validations → throw BusinessException → DB log ────────
            if (string.IsNullOrWhiteSpace(vm.ItemName))
                throw new BusinessException("Item name is required.", "ITEM_NAME_REQUIRED");

            if (vm.price <= 0)
                throw new BusinessException("Price must be greater than zero.", "INVALID_PRICE");

            if (!vm.ItemCategoryId.HasValue || vm.ItemCategoryId <= 0)
                throw new BusinessException("Please select a valid food category.", "CATEGORY_REQUIRED");

            if (vm.RestaurantId <= 0)
                throw new BusinessException("Please select a valid restaurant.", "RESTAURANT_REQUIRED");

            if (vm.ItemName.Length > 100)
                throw new BusinessException("Item name cannot exceed 100 characters.", "ITEM_NAME_TOO_LONG");

            // ── All rules passed → delegate to DAL ────────────────────────────
            return _repo.SaveItem(vm);
        }

        public bool DeleteItem(int id)
        {
            if (id <= 0)
                throw new BusinessException("Invalid item ID.", "INVALID_ITEM_ID");

            return _repo.ItemsDelete(id);
        }
    }
}