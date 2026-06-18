/*using ItemListDal.dal;
using ItemListModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using ItemListModel.Model;
using ItemListModel.ViewModel;
using System.Runtime.Caching;
namespace ItemListDal
{
    public class ItemRepository : IItemRepository
    {
        private Database _db;
        public ItemRepository()
        {
            _db = DatabaseFactory.CreateDatabase();
        }
        private static readonly MemoryCache _cache =
        MemoryCache.Default;
        private string GenerateCacheKey(ItemViewModel vm)
        {
            string categories =
                vm.SelectedCategoryIds != null
                ? string.Join(",", vm.SelectedCategoryIds)
                : "";

            return string.Format(
                "Items_{0}_{1}_{2}_{3}_{4}_{5}_{6}",
                vm.pageNumber,
                vm.pageSize,
                categories,
                vm.RestaurantName,
                vm.status,
                vm.SortField,
                vm.SortDirection
            );
        }

        private void ClearItemCache()
        {
            foreach (var item in _cache)
            {
                if (item.Key.StartsWith("Items_"))
                {
                    _cache.Remove(item.Key);
                }
            }
        }
        public List<FoodCategory> GetFilterValues()
        {
            List<FoodCategory> categories = new List<FoodCategory>();
            try
            {
                DbCommand com = _db.GetStoredProcCommand("GetFilterValues");
                DataSet ds = _db.ExecuteDataSet(com);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    foreach (DataRow row in dt.Rows)
                    {
                        FoodCategory category = new FoodCategory
                        {
                            FoodCategoryId = row["FoodCategoryId"] != DBNull.Value
                                                ? Convert.ToInt32(row["FoodCategoryId"])
                                                : 0,

                            FoodCategoryName = row["FoodCategoryName"] != DBNull.Value
                                                ? row["FoodCategoryName"].ToString()
                                                : ""
                        };
                        categories.Add(category);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return categories; 
}

        public ItemViewModel GetList(ItemViewModel itemViewModel)
        {
            string cacheKey =
                GenerateCacheKey(itemViewModel);

            // CACHE HIT
            if (_cache.Contains(cacheKey))
            {
                return (ItemViewModel)_cache.Get(cacheKey);
            }

            ItemViewModel vm = new ItemViewModel();
            vm.Items = new List<Item>();

            try
            {
                 DbCommand com =
                    _db.GetStoredProcCommand("GetItemList");

                _db.AddInParameter(
                    com,
                    "@PageNumber",
                    DbType.Int32,
                    itemViewModel.pageNumber);

                _db.AddInParameter(
                    com,
                    "@PageSize",
                    DbType.Int32,
                    itemViewModel.pageSize);

                _db.AddInParameter(
                    com,
                    "@Status",
                    DbType.Int32,
                    itemViewModel.status);

                string categoryIds =
                    itemViewModel.SelectedCategoryIds != null
                    ? string.Join(",", itemViewModel.SelectedCategoryIds)
                    : null;

                _db.AddInParameter(
                    com,
                    "@selectedCategories",
                    DbType.String,
                    string.IsNullOrEmpty(categoryIds)
                        ? (object)DBNull.Value
                        : categoryIds);

                _db.AddInParameter(
                    com,
                    "@SearchRestaurant",
                    DbType.String,
                    string.IsNullOrWhiteSpace(
                        itemViewModel.RestaurantName)
                    ? (object)DBNull.Value
                    : itemViewModel.RestaurantName);

                _db.AddInParameter(
                    com,
                    "@SortField",
                    DbType.String,
                    itemViewModel.SortField);

                _db.AddInParameter(
                    com,
                    "@SortDirection",
                    DbType.String,
                    itemViewModel.SortDirection);

                DataSet ds =
                    _db.ExecuteDataSet(com);

                if (ds.Tables.Count > 0 &&
                    ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        Item item = new Item();

                        item.ItemId =
                            row["ItemId"] != DBNull.Value
                            ? Convert.ToInt32(row["ItemId"])
                            : 0;

                        item.ItemName =
                            row["ItemName"]?.ToString() ?? "";

                        item.description =
                            row["description"]?.ToString() ?? "";

                        item.price =
                            row["price"] != DBNull.Value
                            ? Convert.ToDecimal(row["price"])
                            : 0;

                        item.FoodCategoryId =
                            new FoodCategory
                            {
                                FoodCategoryName =
                                row["FoodCategoryName"]?.ToString()
                            };

                        item.RestaurantId =
                            new Restaurant
                            {
                                RestaurantName =
                                row["RestaurantName"]?.ToString()
                            };

                        vm.Items.Add(item);
                    }

                    vm.ItemTotalCount =
                        Convert.ToInt32(
                            ds.Tables[0].Rows[0]["TotalCount"]);
                }

                vm.pageNumber =
                    itemViewModel.pageNumber;

                vm.pageSize =
                    itemViewModel.pageSize;

                vm.ItemTotalPages =
                    (int)Math.Ceiling(
                        (double)vm.ItemTotalCount /
                        vm.pageSize);
                vm.SortField = itemViewModel.SortField;
                vm.SortDirection = itemViewModel.SortDirection;
                // CACHE STORE

                CacheItemPolicy policy =
                    new CacheItemPolicy
                    {
                        AbsoluteExpiration =
                            DateTimeOffset.Now.AddMinutes(10)
                    };

                _cache.Add(
                    cacheKey,
                    vm,
                    policy);

                return vm;
            }
            catch
            {
                throw;
            }
        }
        /* public ItemViewModel GetList(ItemViewModel itemViewModel)
         {
             ItemViewModel vm = new ItemViewModel();
             vm.Items = new List<Item>();

             try
             {
                 DbCommand com = _db.GetStoredProcCommand("GetItemList");

                 _db.AddInParameter(com, "@pageNumber", DbType.Int32, itemViewModel.pageNumber);
                 _db.AddInParameter(com, "@pageSize", DbType.Int32, itemViewModel.pageSize);
                 _db.AddInParameter(com, "@Status", DbType.Int32, itemViewModel.status);
                 string categoryIds = itemViewModel.SelectedCategoryIds != null
                 ? string.Join(",", itemViewModel.SelectedCategoryIds)
                 : null;
                 /*_db.AddInParameter(
                     com,
                     "@selectedCategory",
                     DbType.Int32,
                     itemViewModel.SelectedCategoryId > 0
                         ? itemViewModel.SelectedCategoryId
                         : (object)DBNull.Value
                 );*/
/* _db.AddInParameter(
 com,
 "@selectedCategories",
 DbType.String,
 string.IsNullOrEmpty(categoryIds)
     ? (object)DBNull.Value
     : categoryIds
);

 _db.AddInParameter(
     com,
     "@SearchRestaurant",
     DbType.String,
     itemViewModel.RestaurantName ?? ""
 );
 _db.AddInParameter(
     com,
     "@SortField",
     DbType.String,
     itemViewModel.SortField
 );

 _db.AddInParameter(
     com,
     "@SortDirection",
     DbType.String,
     itemViewModel.SortDirection
 );
 //   _db.AddOutParameter(com, "@ItemTotalCount", DbType.Int32, 4);

 DataSet ds = _db.ExecuteDataSet(com);

 if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
 {
     foreach (DataRow row in ds.Tables[0].Rows)
     {
         Item item = new Item();

         item.ItemId = row["ItemId"] != DBNull.Value ? Convert.ToInt32(row["ItemId"]) : 0;
         item.description = row["description"]?.ToString() ?? "";
         item.price = row["price"] != DBNull.Value ? Convert.ToDecimal(row["price"]) : 0;
         item.ItemName =row["ItemName"]?.ToString() ?? "";

         item.FoodCategoryId = new FoodCategory
         {
             FoodCategoryName = row["FoodCategoryName"]?.ToString() ?? ""
         };


         item.RestaurantId = new Restaurant
         {
             RestaurantName = row["RestaurantName"]?.ToString() ?? ""
         };

         vm.Items.Add(item);
     }
     vm.ItemTotalCount = ds.Tables[0].Rows[0]["TotalCount"] != DBNull.Value
     ? Convert.ToInt32(ds.Tables[0].Rows[0]["TotalCount"])
     : 0;

 }

 vm.pageNumber = itemViewModel.pageNumber;
 vm.pageSize = itemViewModel.pageSize;

 vm.ItemTotalPages = (int)Math.Ceiling((double)vm.ItemTotalCount / vm.pageSize);

 /* vm.ItemTotalCount = com.Parameters["@ItemTotalCount"].Value != DBNull.Value
  ? Convert.ToInt32(com.Parameters["@ItemTotalCount"].Value)
  : 0;*/

/*          if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
 {
vm.TotalCount = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);

  }*/
/*   }
   catch (Exception ex)
   {
       throw;
   }
   return vm;
}*/

/* public bool ItemsInsert(ItemViewModel itemViewModel)
 {
     try
     {
         DbCommand com = this._db.GetStoredProcCommand("ItemInsert");

         this._db.AddInParameter(com, "@ItemName", DbType.String,
             !string.IsNullOrEmpty(itemViewModel.ItemName)
                 ? (object)itemViewModel.ItemName : DBNull.Value);

         this._db.AddInParameter(com, "@description", DbType.String,
             !string.IsNullOrEmpty(itemViewModel.description)
                 ? (object)itemViewModel.description : DBNull.Value);

         this._db.AddInParameter(com, "@price", DbType.Decimal,
             itemViewModel.price > 0
                 ? (object)itemViewModel.price : DBNull.Value);

         // Use ItemCategoryId, not SelectedCategoryId
         this._db.AddInParameter(com, "@FoodCategoryId", DbType.Int32,
             itemViewModel.ItemCategoryId.HasValue
                 ? (object)itemViewModel.ItemCategoryId.Value : DBNull.Value);

         this._db.AddInParameter(com, "@RestaurantId", DbType.Int32,
             itemViewModel.RestaurantId > 0
                 ? (object)itemViewModel.RestaurantId : DBNull.Value);

         this._db.ExecuteNonQuery(com);
         return true;
     }
     catch
     {
         return false;
     }
 }//

public bool SaveItem(ItemViewModel itemViewModel)
{
    try
    {
        DbCommand com ;
        if (itemViewModel.ItemId > 0)
        {
            com = this._db.GetStoredProcCommand("ItemUpdate");
            this._db.AddInParameter(com, "@ItemId", DbType.Int32, itemViewModel.ItemId);
        }
        else
        {
            com= this._db.GetStoredProcCommand("ItemInsert");
        }               

        this._db.AddInParameter(com, "@ItemName", DbType.String,
            !string.IsNullOrEmpty(itemViewModel.ItemName)
                ? (object)itemViewModel.ItemName : DBNull.Value);

        this._db.AddInParameter(com, "@description", DbType.String,
            !string.IsNullOrEmpty(itemViewModel.description)
                ? (object)itemViewModel.description : DBNull.Value);

        this._db.AddInParameter(com, "@price", DbType.Decimal,
            itemViewModel.price > 0
                ? (object)itemViewModel.price : DBNull.Value);

        // Use ItemCategoryId, not SelectedCategoryId
        this._db.AddInParameter(com, "@FoodCategoryId", DbType.Int32,
            itemViewModel.ItemCategoryId.HasValue
                ? (object)itemViewModel.ItemCategoryId.Value : DBNull.Value);

        this._db.AddInParameter(com, "@RestaurantId", DbType.Int32,
            itemViewModel.RestaurantId > 0
                ? (object)itemViewModel.RestaurantId : DBNull.Value);

        this._db.ExecuteNonQuery(com);

        ClearItemCache();


        return true;
    }
    catch
    {
        return false;
    }
}

public ItemViewModel LoadItem(int ItemId)
{
    ItemViewModel vm = new ItemViewModel();
    try
    {
        DbCommand com = _db.GetStoredProcCommand("ItemsGetDetails");
        _db.AddInParameter(com, "@ItemId", DbType.Int32, ItemId);
        DataSet ds = _db.ExecuteDataSet(com);

        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            var row = ds.Tables[0].Rows[0];
            vm.ItemId = row["ItemId"] != DBNull.Value ? Convert.ToInt32(row["ItemId"]) : 0;
            vm.ItemName = row["ItemName"]?.ToString() ?? "";
            vm.description = row["description"]?.ToString() ?? "";
            vm.price = row["price"] != DBNull.Value ? Convert.ToDecimal(row["price"]) : 0;

            // Load into ItemCategoryId — NOT SelectedCategoryId
            vm.ItemCategoryId = row["FoodCategoryId"] != DBNull.Value
                ? Convert.ToInt32(row["FoodCategoryId"])
                : (int?)null;

            vm.SelectedCategoryName = row["FoodCategoryName"]?.ToString() ?? "";
            vm.RestaurantId = row["RestaurantId"] != DBNull.Value
                ? Convert.ToInt32(row["RestaurantId"]) : 0;

            vm.Restaurant = new List<Restaurant>();
        }
      //  vm.FoodCategory = GetFilterValues();
    }
    catch
    {
        throw;
    }
    return vm;
}
public bool ItemsDelete(int ItemId)
{
 //   ItemViewModel vm = new ItemViewModel();
    try
    {
        DbCommand com = this._db.GetStoredProcCommand("ItemsDelete");
        this._db.AddInParameter(com, "ItemId", DbType.Int32, ItemId);
        this._db.ExecuteNonQuery(com);
    }
    catch (Exception ex)
    {
        return false;
    }
    ClearItemCache();
    return true;
}

public bool Load()
{
    return false;
}

public bool Save()
{
    return false;
}




/*  public List<Item> GetList(ItemViewModel itemViewModel)
  {
      List<Item> itemList = new List<Item>();

      try
      {
          DbCommand com = _db.GetStoredProcCommand("GetItemList");
          _db.AddInParameter(com, "@pageNumber", DbType.Int32, itemViewModel.pageNumber);
          _db.AddInParameter(com, "@pageSize", DbType.Int32, itemViewModel.pageSize);
          // _db.AddInParameter(com, "@itemName", DbType.String, itemViewModel.itemName);

          _db.AddInParameter(
              com,
              "@selectedCategory",
              DbType.Int32,
              itemViewModel.SelectedCategoryId > 0
                  ? itemViewModel.SelectedCategoryId
                  : (object)DBNull.Value
          );
          _db.AddInParameter(com,
              "@SearchRestaurant",
              DbType.String, 
              itemViewModel.RestaurantName != null 
              ? itemViewModel.RestaurantName
              :"");
          DataSet ds = _db.ExecuteDataSet(com);

          if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
          {
              DataTable dt = ds.Tables[0];

              foreach (DataRow row in dt.Rows)
              {
                  Item item = new Item();

                   item.ItemId = row["ItemId"] != DBNull.Value ? Convert.ToInt32(row["ItemId"]) : 0;
                  item.description = row["description"] != DBNull.Value ? Convert.ToString(row["description"]) : "";
                  item.price = row["price"] != DBNull.Value ? Convert.ToInt32(row["price"]) : 0;


                  item.ItemName = new ItemName
                  {
                      Item_Name = row["ItemName"] != DBNull.Value ? row["ItemName"].ToString() : "",
                      FoodCategory = new FoodCategory
                      {
                          //   FoodCategoryId = row["FoodCategoryId"] != DBNull.Value ? Convert.ToInt32(row["FoodCategoryId"]) : 0,
                          FoodCategoryName = row["FoodCategoryName"] != DBNull.Value ? row["FoodCategoryName"].ToString() : ""
                      }
                  };
                  item.Restaurant = new Restaurant
                  {
                      RestaurantName = row["RestaurantName"] != DBNull.Value ? row["RestaurantName"].ToString() : ""
                  };

                  itemList.Add(item);
              }
          }
      }
      catch (Exception ex)
      {
          throw ex; // or log it
      }

      return itemList;
  }
}
}*/
// =============================================
// FILE: ItemListDal\dal\ItemRepository.cs  (updated)
// PROJECT: FoodOrderingSystemDal
//
// CHANGES from original:
//  1. using ItemListModel.Exceptions  ← exceptions now live in Model project
//  2. Every catch(SqlException) wraps into DataAccessException and re-throws
//  3. No more catch { return false; } — never swallow errors
//  4. LoadItem throws NotFoundException when record not found
// =============================================

using ItemListDal.dal;
using ItemListModel;
using ItemListModel.Exceptions;   // ← DataAccessException, NotFoundException live here
using ItemListModel.Model;
using ItemListModel.ViewModel;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.Caching;

namespace ItemListDal
{
    public class ItemRepository : IItemRepository
    {
        private Database _db;
        private static readonly MemoryCache _cache = MemoryCache.Default;

        public ItemRepository()
        {
            _db = DatabaseFactory.CreateDatabase();
        }

        // ── RULE: DAL only throws — never returns false on error ──────────────

        public List<FoodCategory> GetFilterValues()
        {
            try
            {
                var categories = new List<FoodCategory>();
                DbCommand com = _db.GetStoredProcCommand("GetFilterValues");
                DataSet ds = _db.ExecuteDataSet(com);

                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        categories.Add(new FoodCategory
                        {
                            FoodCategoryId = row["FoodCategoryId"] != DBNull.Value ? Convert.ToInt32(row["FoodCategoryId"]) : 0,
                            FoodCategoryName = row["FoodCategoryName"] != DBNull.Value ? row["FoodCategoryName"].ToString() : ""
                        });
                    }
                }
                return categories;
            }
            catch (SqlException ex)
            {
                throw new DataAccessException("Failed to load filter values.", "GetFilterValues", ex);
            }
        }

        public ItemViewModel GetList(ItemViewModel itemViewModel)
        {
            string cacheKey = GenerateCacheKey(itemViewModel);
            if (_cache.Contains(cacheKey))
                return (ItemViewModel)_cache.Get(cacheKey);

            try
            {
                var vm = new ItemViewModel { Items = new List<Item>() };

                DbCommand com = _db.GetStoredProcCommand("GetItemList");
                _db.AddInParameter(com, "@PageNumber", DbType.Int32, itemViewModel.pageNumber);
                _db.AddInParameter(com, "@PageSize", DbType.Int32, itemViewModel.pageSize);
                _db.AddInParameter(com, "@Status", DbType.Int32, itemViewModel.status);
                _db.AddInParameter(com, "@selectedCategories", DbType.String,
                    itemViewModel.SelectedCategoryIds != null
                        ? (object)string.Join(",", itemViewModel.SelectedCategoryIds)
                        : DBNull.Value);
                _db.AddInParameter(com, "@SearchRestaurant", DbType.String,
                    string.IsNullOrWhiteSpace(itemViewModel.RestaurantName)
                        ? (object)DBNull.Value
                        : itemViewModel.RestaurantName);
                _db.AddInParameter(com, "@SortField", DbType.String, itemViewModel.SortField);
                _db.AddInParameter(com, "@SortDirection", DbType.String, itemViewModel.SortDirection);

                DataSet ds = _db.ExecuteDataSet(com);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        vm.Items.Add(new Item
                        {
                            ItemId = row["ItemId"] != DBNull.Value ? Convert.ToInt32(row["ItemId"]) : 0,
                            ItemName = row["ItemName"]?.ToString() ?? "",
                            description = row["description"]?.ToString() ?? "",
                            price = row["price"] != DBNull.Value ? Convert.ToDecimal(row["price"]) : 0,
                            FoodCategoryId = new FoodCategory { FoodCategoryName = row["FoodCategoryName"]?.ToString() },
                            RestaurantId = new Restaurant { RestaurantName = row["RestaurantName"]?.ToString() }
                        });
                    }
                    vm.ItemTotalCount = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalCount"]);
                }

                vm.pageNumber = itemViewModel.pageNumber;
                vm.pageSize = itemViewModel.pageSize;
                vm.ItemTotalPages = (int)Math.Ceiling((double)vm.ItemTotalCount / vm.pageSize);
                vm.SortField = itemViewModel.SortField;
                vm.SortDirection = itemViewModel.SortDirection;

                _cache.Add(cacheKey, vm, new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10)
                });

                return vm;
            }
            catch (SqlException ex)
            {
                throw new DataAccessException("Failed to retrieve item list.", "GetItemList", ex);
            }
        }

        public bool SaveItem(ItemViewModel itemViewModel)
        {
            try
            {
                DbCommand com = itemViewModel.ItemId > 0
                    ? _db.GetStoredProcCommand("ItemUpdate")
                    : _db.GetStoredProcCommand("ItemInsert");

                if (itemViewModel.ItemId > 0)
                    _db.AddInParameter(com, "@ItemId", DbType.Int32, itemViewModel.ItemId);

                _db.AddInParameter(com, "@ItemName", DbType.String,
                    string.IsNullOrEmpty(itemViewModel.ItemName) ? (object)DBNull.Value : itemViewModel.ItemName);
                _db.AddInParameter(com, "@description", DbType.String,
                    string.IsNullOrEmpty(itemViewModel.description) ? (object)DBNull.Value : itemViewModel.description);
                _db.AddInParameter(com, "@price", DbType.Decimal,
                    itemViewModel.price > 0 ? (object)itemViewModel.price : DBNull.Value);
                _db.AddInParameter(com, "@FoodCategoryId", DbType.Int32,
                    itemViewModel.ItemCategoryId.HasValue ? (object)itemViewModel.ItemCategoryId.Value : DBNull.Value);
                _db.AddInParameter(com, "@RestaurantId", DbType.Int32,
                    itemViewModel.RestaurantId > 0 ? (object)itemViewModel.RestaurantId : DBNull.Value);

                _db.ExecuteNonQuery(com);
                ClearItemCache();
                return true;
            }
            catch (SqlException ex)
            {
                throw new DataAccessException("Failed to save item.", "ItemInsert/ItemUpdate", ex);
            }
        }

        public ItemViewModel LoadItem(int itemId)
        {
            try
            {
                var vm = new ItemViewModel();
                DbCommand com = _db.GetStoredProcCommand("ItemsGetDetails");
                _db.AddInParameter(com, "@ItemId", DbType.Int32, itemId);
                DataSet ds = _db.ExecuteDataSet(com);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var row = ds.Tables[0].Rows[0];
                    vm.ItemId = row["ItemId"] != DBNull.Value ? Convert.ToInt32(row["ItemId"]) : 0;
                    vm.ItemName = row["ItemName"]?.ToString() ?? "";
                    vm.description = row["description"]?.ToString() ?? "";
                    vm.price = row["price"] != DBNull.Value ? Convert.ToDecimal(row["price"]) : 0;
                    vm.ItemCategoryId = row["FoodCategoryId"] != DBNull.Value ? Convert.ToInt32(row["FoodCategoryId"]) : (int?)null;
                    vm.SelectedCategoryName = row["FoodCategoryName"]?.ToString() ?? "";
                    vm.RestaurantId = row["RestaurantId"] != DBNull.Value ? Convert.ToInt32(row["RestaurantId"]) : 0;
                    vm.Restaurant = new List<Restaurant>();
                }
                else
                {
                    // Not found → throw NotFoundException (no log, just 404 to user)
                    throw new NotFoundException($"Item with ID {itemId} was not found.");
                }

                return vm;
            }
            catch (SqlException ex)
            {
                throw new DataAccessException("Failed to load item details.", "ItemsGetDetails", ex);
            }
        }

        public bool ItemsDelete(int itemId)
        {
            try
            {
                DbCommand com = _db.GetStoredProcCommand("ItemsDelete");
                _db.AddInParameter(com, "ItemId", DbType.Int32, itemId);
                _db.ExecuteNonQuery(com);
                ClearItemCache();
                return true;
            }
            catch (SqlException ex)
            {
                throw new DataAccessException("Failed to delete item.", "ItemsDelete", ex);
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private string GenerateCacheKey(ItemViewModel vm)
        {
            string cats = vm.SelectedCategoryIds != null
                ? string.Join(",", vm.SelectedCategoryIds) : "";
            return $"Items_{vm.pageNumber}_{vm.pageSize}_{cats}_{vm.RestaurantName}_{vm.status}_{vm.SortField}_{vm.SortDirection}";
        }

        private void ClearItemCache()
        {
            foreach (var item in _cache)
                if (item.Key.StartsWith("Items_"))
                    _cache.Remove(item.Key);
        }

        public bool Load() => false;
        public bool Save() => false;
    }
}