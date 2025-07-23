namespace FruVa.Ordering.Ui.Models
{
    public interface IFilterItem
    {
        Guid? Id { get; }
        string? DisplayName { get; set; }
        bool IsChecked {  get; set; }
        string? SearchContent { get; }
    }
}
