//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using ItemListModel.ViewModel;
//using System.Data.SqlClient;
//using ItemListModel.Exceptions;
//using System.Data;
//using System.Data.Common;
//using Microsoft.Practices.EnterpriseLibrary.Data;

//namespace ItemListDal.dal
//{
//    public class OrderRepository : IOrderRepository
//    {
//        private Database _db;

//        public OrderRepository()
//        {
//            _db = DatabaseFactory.CreateDatabase();
//        }

//        public List<OrderViewModel> GetOrderList()
//        {
//            try
//            {
//                List<OrderViewModel> orderList = new List<OrderViewModel>();

//                DbCommand cmd = _db.GetStoredProcCommand("GetOrderList");

//                DataSet ds = _db.ExecuteDataSet(cmd);

//                if (ds.Tables.Count > 0)
//                {
//                    foreach (DataRow row in ds.Tables[0].Rows)
//                    {
//                        int orderId = row["OrderId"] != DBNull.Value
//                            ? Convert.ToInt32(row["OrderId"])
//                            : 0;

//                        OrderViewModel existingOrder = orderList
//                            .FirstOrDefault(x => x.OrderId == orderId);

//                        if (existingOrder == null)
//                        {
//                            existingOrder = new OrderViewModel
//                            {
//                                OrderId = orderId,
//                                UserId = row["UserId"] != DBNull.Value
//                                    ? Convert.ToInt32(row["UserId"])
//                                    : 0,

//                                OrderDate = row["OrderDate"] != DBNull.Value
//                                    ? Convert.ToDateTime(row["OrderDate"])
//                                    : DateTime.MinValue,

//                                Items = new List<OrderItemViewModel>()
//                            };

//                            orderList.Add(existingOrder);
//                        }

//                        existingOrder.Items.Add(new OrderItemViewModel
//                        {
//                            ItemName = row["ItemName"] != DBNull.Value
//                                ? row["ItemName"].ToString()
//                                : "",

//                            Price = row["Price"] != DBNull.Value
//                                ? Convert.ToDecimal(row["Price"])
//                                : 0,

//                            Quantity = row["Quantity"] != DBNull.Value
//                                ? Convert.ToInt32(row["Quantity"])
//                                : 0
//                        });
//                    }
//                }

//                return orderList;
//            }
//            catch (SqlException ex)
//            {
//                throw new DataAccessException(
//                    "Failed to load orders.",
//                    "GetOrderList",
//                    ex);
//            }
//        }


//        public OrderViewModel LoadOrder(int ItemId)
//        {
//            throw new NotImplementedException();
//        }

//        public bool OrderDelete(int ItemId)
//        {
//            throw new NotImplementedException();
//        }

//        public bool SaveOrder(OrderViewModel orderViewModel)
//        {
//            throw new NotImplementedException();
//        }



//    }
//}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using ItemListModel.ViewModel;
using ItemListModel.Exceptions;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Linq;
namespace ItemListDal.dal
{
    public class OrderRepository : IOrderRepository
    {
        private Database _db;

        public OrderRepository()
        {
            _db = DatabaseFactory.CreateDatabase();
        }

        // ── Order list (main page) ────────────────────────────────────────
        public OrderViewModel GetOrderList(OrderViewModel orderViewModel)
        {
            try
            {
                List<OrderViewModel> orderList = new List<OrderViewModel>();

                DbCommand cmd = _db.GetStoredProcCommand("GetOrderList");
                _db.AddInParameter(cmd, "@Status", DbType.Int32, orderViewModel.Status);
                _db.AddInParameter(cmd, "@PriceRange", DbType.String,
                    string.IsNullOrEmpty(orderViewModel.PriceRange) ? (object)DBNull.Value : orderViewModel.PriceRange);
                _db.AddInParameter(cmd, "@Price", DbType.Decimal,
                    orderViewModel.Price.HasValue ? (object)orderViewModel.Price.Value : DBNull.Value);
                _db.AddInParameter(cmd, "@DateFrom", DbType.DateTime,
                    orderViewModel.DateFrom.HasValue ? (object)orderViewModel.DateFrom.Value : DBNull.Value);
                _db.AddInParameter(cmd, "@DateTo", DbType.DateTime,
                    orderViewModel.DateTo.HasValue ? (object)orderViewModel.DateTo.Value : DBNull.Value);
                _db.AddInParameter(cmd, "@PageNumber", DbType.Int32, orderViewModel.pageNumber);
                _db.AddInParameter(cmd, "@PageSize", DbType.Int32, orderViewModel.pageSize);
                DataSet ds = _db.ExecuteDataSet(cmd);

                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int orderId = row["OrderId"] != DBNull.Value
                            ? Convert.ToInt32(row["OrderId"]) : 0;

                        OrderViewModel existing = orderList
                            .FirstOrDefault(x => x.OrderId == orderId);

                        if (existing == null)
                        {
                            existing = new OrderViewModel
                            {
                                OrderId = orderId,
                                UserId = row["UserId"] != DBNull.Value ? Convert.ToInt32(row["UserId"]) : 0,
                                IsActive = row["IsActive"] != DBNull.Value ? Convert.ToBoolean(row["IsActive"]) : false,
                                OrderDate = row["OrderDate"] != DBNull.Value ? Convert.ToDateTime(row["OrderDate"]) : (DateTime?)null
                               

                            };
                            orderList.Add(existing);
                        }

                        existing.Items.Add(new OrderItemViewModel
                        {
                            ItemId = row["ItemId"] != DBNull.Value ? Convert.ToInt32(row["ItemId"]) : 0,
                            ItemName = row["ItemName"] != DBNull.Value ? row["ItemName"].ToString() : "",
                            Price = row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : 0,
                            Quantity = row["Quantity"] != DBNull.Value ? Convert.ToInt32(row["Quantity"]) : 0

                        });
                        orderViewModel.ItemTotalCount = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalCount"]);
                    }
                   
                }
                orderViewModel.Orders = orderList;

                orderViewModel.ItemTotalPages =
                    (int)Math.Ceiling(
                        (double)orderViewModel.ItemTotalCount /
                        orderViewModel.pageSize
                    );

                return orderViewModel;
            }
            catch (SqlException ex)
            {
                throw new DataAccessException("Failed to load orders.", "GetOrderList", ex);
            }
        }

        // ── Load single order for edit modal ─────────────────────────────
        public OrderViewModel LoadOrder(int orderId)
        {
            try
            {
                OrderViewModel vm = new OrderViewModel();

                DbCommand cmd = _db.GetStoredProcCommand("OrderGetDetails");
                _db.AddInParameter(cmd, "@OrderId", DbType.Int32, orderId);

                DataSet ds = _db.ExecuteDataSet(cmd);

                // Table 0: header
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    vm.OrderId = Convert.ToInt32(row["OrderId"]);
                    vm.UserId = Convert.ToInt32(row["UserId"]);
                    vm.OrderDate = Convert.ToDateTime(row["OrderDate"]);
                }

                // Table 1: items
                if (ds.Tables.Count > 1)
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        vm.Items.Add(new OrderItemViewModel
                        {
                            ItemId = Convert.ToInt32(row["ItemId"]),
                            ItemName = row["ItemName"].ToString(),
                            Price = Convert.ToDecimal(row["Price"]),
                            Quantity = Convert.ToInt32(row["Quantity"])
                        });
                    }
                }

                // AllItems: for dropdown
                vm.AllItems = GetAllItems();

                return vm;
            }
            catch (SqlException ex)
            {
                throw new DataAccessException("Failed to load order.", "OrderGetDetails", ex);
            }
        }

        // ── All items for dropdown ────────────────────────────────────────
        public List<OrderItemViewModel> GetAllItems()
        {
            try
            {
                List<OrderItemViewModel> list = new List<OrderItemViewModel>();

                DbCommand cmd = _db.GetStoredProcCommand("GetAllItems");
                DataSet ds = _db.ExecuteDataSet(cmd);

                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(new OrderItemViewModel
                        {
                            ItemId = Convert.ToInt32(row["ItemId"]),
                            ItemName = row["ItemName"].ToString(),
                            Price = Convert.ToDecimal(row["Price"])
                        });
                    }
                }

                return list;
            }
            catch (SqlException ex)
            {
                throw new DataAccessException("Failed to load items.", "GetAllItems", ex);
            }
        }

        public bool SaveOrder(OrderViewModel model)
        {
            try
            {
                // Build the DataTable matching your TVP columns
                var itemsTable = new DataTable();
                itemsTable.Columns.Add("ItemId", typeof(int));
                itemsTable.Columns.Add("Quantity", typeof(int));
                itemsTable.Columns.Add("Price", typeof(decimal));

                foreach (var item in model.Items ?? Enumerable.Empty<OrderItemViewModel>())
                    itemsTable.Rows.Add(item.ItemId, item.Quantity, item.Price);

                DbCommand cmd = _db.GetStoredProcCommand("OrderSave");
                _db.AddInParameter(cmd, "@OrderId", DbType.Int32, model.OrderId);
                _db.AddInParameter(cmd, "@UserId", DbType.Int32, model.UserId > 0 ? (object)model.UserId : DBNull.Value);
                _db.AddInParameter(cmd, "@OrderDate", DbType.DateTime, model.OrderDate.HasValue ? (object)model.OrderDate.Value : DBNull.Value);

                // TVP parameter needs SqlParameter directly
                var tvpParam = new SqlParameter("@Items", SqlDbType.Structured)
                {
                    Value = itemsTable,
                    TypeName = "dbo.OrderItemType"
                };
                cmd.Parameters.Add(tvpParam);

                object result = _db.ExecuteScalar(cmd);
                model.OrderId = Convert.ToInt32(result);
                return true;
            }
            catch (SqlException ex)
            {
                throw new DataAccessException("Failed to save order.", "OrderSave", ex);
            }
        }
        public bool OrderDelete(int orderId)
        {
            try
            {
                DbCommand cmd =
                    _db.GetStoredProcCommand("OrderDelete");

                _db.AddInParameter(
                    cmd,
                    "@OrderId",
                    DbType.Int32,
                    orderId);

                _db.ExecuteNonQuery(cmd);

                return true;
            }
            catch (SqlException ex)
            {
                throw new DataAccessException(
                    "Failed to delete order.",
                    "OrderDelete",
                    ex);
            }
        }
        //public bool SaveOrder(OrderViewModel model)
        //{
        //    try
        //    {
        //        int orderId = model.OrderId;
        //        if (orderId > 0)
        //        {
        //            DbCommand cmd =
        //                _db.GetStoredProcCommand("OrderUpdate");

        //            _db.AddInParameter(
        //                cmd,
        //                "@OrderId",
        //                DbType.Int32,
        //                orderId);

        //            _db.AddInParameter(
        //                cmd,
        //                "@UserId",
        //                DbType.Int32,
        //                model.UserId);

        //            _db.AddInParameter(
        //                cmd,
        //                "@OrderDate",
        //                DbType.DateTime,
        //                model.OrderDate);

        //            _db.ExecuteNonQuery(cmd);
        //        }
        //        else
        //        {
        //            DbCommand cmd =
        //                _db.GetStoredProcCommand("OrderInsert");

        //            _db.AddInParameter(
        //                cmd,
        //                "@UserId",
        //                DbType.Int32,
        //                model.UserId);

        //            _db.AddInParameter(
        //                cmd,
        //                "@OrderDate",
        //                DbType.DateTime,
        //                model.OrderDate);

        //            orderId = Convert.ToInt32(
        //                _db.ExecuteScalar(cmd));
        //        }


        //        if (model.Items != null)
        //        {
        //            foreach (var item in model.Items)
        //            {
        //                DbCommand itemCmd;

        //                if (model.OrderId > 0)
        //                {
        //                    itemCmd =
        //                        _db.GetStoredProcCommand(
        //                            "OrderItemUpdate");

        //                    _db.AddInParameter(
        //                        itemCmd,
        //                        "@ItemId",
        //                        DbType.Int32,
        //                        item.ItemId);
        //                }
        //                else
        //                {
        //                    itemCmd =
        //                        _db.GetStoredProcCommand(
        //                            "OrderItemInsert");

        //                    _db.AddInParameter(
        //                        itemCmd,
        //                        "@ItemId",
        //                        DbType.Int32,
        //                        item.ItemId);
        //                }

        //                _db.AddInParameter(
        //                    itemCmd,
        //                    "@OrderId",
        //                    DbType.Int32,
        //                    orderId);

        //                _db.AddInParameter(
        //                    itemCmd,
        //                    "@Quantity",
        //                    DbType.Int32,
        //                    item.Quantity);

        //                _db.AddInParameter(
        //                    itemCmd,
        //                    "@Price",
        //                    DbType.Decimal,
        //                    item.Price);

        //                _db.ExecuteNonQuery(itemCmd);
        //            }
        //        }

        //        return true;
        //    }
        //    catch (SqlException ex)
        //    {
        //        throw new DataAccessException(
        //            "Failed to save order.",
        //            "OrderInsert/OrderUpdate",
        //            ex);
        //    }
        //}
        /*  public bool SaveOrder(OrderViewModel model)
          {
              try
              {
                  DbCommand cmd = model.OrderId > 0
                      ? _db.GetStoredProcCommand("OrderUpdate")
                      : _db.GetStoredProcCommand("OrderInsert");

                  if (model.OrderId > 0)
                  { _db.AddInParameter(cmd, "@OrderId", DbType.Int32, model.OrderId); }

                  _db.AddInParameter(cmd, "@UserId", DbType.Int32, model.UserId > 0 ? (object)model.UserId : DBNull.Value);
                  _db.AddInParameter(cmd, "@OrderDate", DbType.DateTime,
              model.OrderDate.HasValue ? (object)model.OrderDate.Value : DBNull.Value);
                  _db.ExecuteNonQuery(cmd);

                  // Update each item individually — same pattern as SaveItem
                  if (model.Items != null)
                  {
                      foreach (var item in model.Items)
                      {
                          DbCommand itemCmd = item.OrderItemId > 0
       ? _db.GetStoredProcCommand("OrderItemUpdate")
       : _db.GetStoredProcCommand("OrderItemInsert");

                          if (item.ItemId > 0)
                              _db.AddInParameter(itemCmd, "@ItemId", DbType.Int32, item.ItemId);

                          _db.AddInParameter(itemCmd, "@OrderId", DbType.Int32, model.OrderId);
                          _db.AddInParameter(itemCmd, "@Quantity", DbType.Int32, item.Quantity > 0 ? (object)item.Quantity : DBNull.Value);
                          _db.AddInParameter(itemCmd, "@Price", DbType.Decimal, item.Price > 0 ? (object)item.Price : DBNull.Value);

                          _db.ExecuteNonQuery(itemCmd);
                      }
                  }

                  return true;
              }
              catch (SqlException ex)
              {
                  throw new DataAccessException("Failed to save order.", "OrderUpdate/OrderItemUpdate", ex);
              }
          }*/       
    }
}
