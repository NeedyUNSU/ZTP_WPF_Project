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

        public TransactionCategoryModel(TransactionCategoryModel obj)
        {
            this.Name = obj.Name;
            this.Description = obj.Description;
        }

        // XML Handler
        public TransactionCategoryModel() {}

        public override bool Validate()
        {
            if (!string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Description)) { return true; } else { return false; }
        }
        public override string ToString()
        {
            return $"{Name}";
        }

    }
}
