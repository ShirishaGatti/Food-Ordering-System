using ItemListDal.dal;
using ItemListModel.Exceptions;
using ItemListModel.ViewModel;
using System;
using System.Collections.Generic;

namespace ItemListDal.Services
{
    public class OrderService
    {
        private readonly OrderRepository _repo = new OrderRepository();

     
        public OrderViewModel GetOrderList(OrderViewModel filters)
        {
            
            if (filters.DateFrom.HasValue && filters.DateTo.HasValue
                && filters.DateFrom.Value > filters.DateTo.Value)
            {
                throw new BusinessException("'Date From' cannot be later than 'Date To'.");
            }
            if (filters.Price.HasValue && filters.Price.Value < 0)
            {
                throw new BusinessException("Price filter cannot be negative.");
            }
            
            if (!string.IsNullOrEmpty(filters.PriceRange) && !filters.Price.HasValue)
            {
                throw new BusinessException("Please enter an amount when using a price filter.");
            }

            return _repo.GetOrderList(filters);
        }

      
        public OrderViewModel LoadOrder(int orderId)
        {
            if (orderId <= 0)
                throw new BusinessException("Invalid order ID.");

            var order = _repo.LoadOrder(orderId);

            if (order == null)
                throw new NotFoundException($"Order #{orderId} was not found.");

            return order;
        }

 
        public void SaveOrder(OrderViewModel model)
        {
         
            if (model.Items == null || model.Items.Count == 0)
                throw new BusinessException("An order must have at least one item.");

     
            foreach (var item in model.Items)
            {
                if (item.Quantity <= 0)
                    throw new BusinessException($"Quantity for '{item.ItemName}' must be greater than zero.");

                if (item.Price <= 0)
                    throw new BusinessException($"Price for '{item.ItemName}' must be greater than zero.");
            }

          
            if (model.OrderDate.HasValue && model.OrderDate.Value.Date > DateTime.Today)
                throw new BusinessException("Order date cannot be in the future.");

            _repo.SaveOrder(model);
        }

      
        public void DeleteOrder(int orderId)
        {
            if (orderId <= 0)
                throw new BusinessException("Invalid order ID.");

            _repo.OrderDelete(orderId);
        }

      
        public List<OrderItemViewModel> GetAllItems()
        {
            return _repo.GetAllItems();
        }
    }
}