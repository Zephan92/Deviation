using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Interface
{
	public interface IPlayerAccount
	{
		string AccountName { get; set; }
		string AccountAlias { get; set; }
		IResourceBag ResourceBag { get; set; }
	}
}
