﻿using System;
using System.Collections.Generic;

namespace ZTP_WPF_Project.MVVM.Model
{
    public class ReportModel : BaseModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } = DateTime.Now;
        public List<TransactionModel> Transactions { get; set; } = new List<TransactionModel>();


        public override bool Validate()
        {
            return !string.IsNullOrWhiteSpace(Title) && Transactions.Count > 0 && StartDate < EndDate;
        }
    }
}
