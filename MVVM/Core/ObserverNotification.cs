﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZTP_WPF_Project.MVVM.Core
{
    public interface IObserver
    {
        void Update(double expense);
    }

    public class Notifications
    {
        private readonly List<IObserver> observers = new();

        private double expense;

        public double Expense
        {
            get => expense;
            set
            {
                expense = value;
                Notify();
            }
        }

        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            observers.Remove(observer);
        }

        private void Notify()
        {
            foreach (var observer in observers)
            {
                observer.Update(expense);
            }
        }
    }

    public class BudgetOverNinety : IObserver
    {
        private readonly double budget;
        private double expensesSum;

        public BudgetOverNinety(double budget)
        {
            this.budget = budget;
        }

        public void Update(double expense)
        {
            expensesSum += expense;
            if (expensesSum > (budget * 0.9))
            {
                MessageBox.Show("You do Overrun your budget after a month.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public class Overrun : IObserver
    {
        private readonly double budget;
        private double expensesSum;

        public Overrun(double budget)
        {
            this.budget = budget;
        }

        public void Update(double expense)
        {
            expensesSum += expense;
            if (expensesSum > budget)
            {
                MessageBox.Show("You do Overrun your budget after a month.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public class Congratulation : IObserver
    {
        private readonly double budget;
        private double expensesSum;

        public Congratulation(double budget)
        {
            this.budget = budget;
        }

        public void Update(double expense)
        {
            expensesSum += expense;
            if (expense == 0 && expensesSum < budget)
            {
                MessageBox.Show("Congratulation You do not overrun your budget after a month!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

}
