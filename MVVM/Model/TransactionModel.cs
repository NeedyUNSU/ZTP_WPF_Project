using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTP_WPF_Project.MVVM.Model
{
    public class TransactionModel : BaseModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
		public float Amount { get; set; }
		public TransactionType _Type { get; set; }
		private DateTime date = DateTime.Now;
        public TransactionCategoryModel _categoryModel;
		public string? CategoryId => _categoryModel.Id;
        public string? CategoryName => _categoryModel.Name;
        public string? CategoryDesc => _categoryModel.Description;

        public DateTime AddedDate
		{
			get { return date; }
			set { date = value; }
		}

		public string StringType
		{
			get => _Type.ToString();
			set
			{
				if (Enum.TryParse(value,true,out TransactionType result))
				{
					_Type = result;
				}
				else
				{
					_Type = TransactionType.None;
                    throw new ArgumentException($"Invalid UserType value: {value}");
                }
			}
		}

        public TransactionModel(string title, string desc, float amount, TransactionType type, TransactionCategoryModel category, DateTime date)
        {
            Title = title;
			Description = desc;
			Amount = amount;
			_Type = type;
			_categoryModel = category;
			this.date = date;
        }

        public TransactionModel(string title, string desc, float amount, TransactionType type, TransactionCategoryModel category)
        {
            Title = title;
            Description = desc;
            Amount = amount;
            _Type = type;
            _categoryModel = category;
        }
    }

	public enum TransactionType
    {
		None, Income, Expense
	}
}
