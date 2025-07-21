using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

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
