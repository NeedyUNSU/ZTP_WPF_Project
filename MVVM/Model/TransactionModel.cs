﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZTP_WPF_Project.MVVM.Core;

namespace ZTP_WPF_Project.MVVM.Model
{
    public class TransactionModel : BaseModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
		public float Amount { get; set; }
		public TransactionType _Type { get; set; }
		public DateTime AddedDate { get; set; } = DateTime.Now;
        private TransactionCategoryModel? _categoryModel { get; set; }
        public string? CategoryId => _categoryModel?.Id;
        public string? CategoryName => _categoryModel?.Name;
        public string? CategoryDesc => _categoryModel?.Description;

        public string? CategoryIdXaml { get; set; }

        public TransactionCategoryModel? _category
        {
            get => _categoryModel;
            set
            {
                if (_Type == TransactionType.Expense) 
                {
                    try
                    {
                        _categoryModel = value;
                        CategoryIdXaml = value?.Id;
                    }
                    catch (Exception ex)
                    {
                        throw new CustomException(ex.Message, ex);
                    }
                }
                else
                {
                    _categoryModel = null;
                    throw new CustomException($"Invalid TransactionType refering to category.", new ArgumentException());
                }
            }
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
                    throw new CustomException($"Invalid TransactionType value: {value}", new ArgumentException());
                }
			}
		}

        public TransactionModel(string title, string desc, float amount, TransactionType type, TransactionCategoryModel category, DateTime date)
        {
            Title = title;
			Description = desc;
			Amount = amount;
			_Type = type;
            _category = category;
			AddedDate = date;
        }

        public TransactionModel(string title, string desc, float amount, TransactionType type, TransactionCategoryModel category)
        {
            Title = title;
            Description = desc;
            Amount = amount;
            _Type = type;
            _category = category;
        }

        public TransactionModel() {}
    }

	public enum TransactionType
    {
		None, Income, Expense
	}
}
