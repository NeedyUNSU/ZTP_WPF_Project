using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTP_WPF_Project.MVVM.Model
{
    public abstract class BaseModel
    {
		private string id = Guid.NewGuid().ToString();

		public string Id
		{
			get { return id; }
			set { id = value; }
		}

		public abstract bool Validate();
	}
}
