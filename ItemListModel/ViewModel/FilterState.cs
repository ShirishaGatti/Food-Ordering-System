public class FilterState
{
    public int pageNumber { get; set; } = 1;
    public int pageSize { get; set; } = 20;
    public int status { get; set; } = 1;
    public int? SelectedCategoryId { get; set; }
    public string RestaurantName { get; set; }
}