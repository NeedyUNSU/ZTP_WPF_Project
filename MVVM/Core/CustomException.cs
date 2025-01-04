using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZTP_WPF_Project.MVVM.Core
{
    public class CustomException : Exception
    {
        public CustomException()
        {
            MessageBox.Show("Error has occured", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public CustomException(string message)
            : base(message)
        {
            MessageBox.Show($"Error has occured: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        }

        public CustomException(string message, Exception inner)
            : base(message, inner)
        {
            MessageBox.Show($"Error has occured: {message} {inner.Source}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        }
    }
}
