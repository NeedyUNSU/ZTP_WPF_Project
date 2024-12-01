using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTP_WPF_Project.MVVM.Model
{
    public class TransactionCategoryModel : BaseModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        
        public TransactionCategoryModel(string name, string desc)
        {
            this.Name = name;
            this.Description = desc;
        }
    }
}
