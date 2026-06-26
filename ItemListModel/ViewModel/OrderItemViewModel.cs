namespace ItemListModel.ViewModel
{
    public class OrderItemViewModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string RestaurantName { get; set; }
        public int OrderItemId { get; set; }
        public decimal SubTotal
        {
            get
            {
                return Price * Quantity;
            }
        }
    }
}