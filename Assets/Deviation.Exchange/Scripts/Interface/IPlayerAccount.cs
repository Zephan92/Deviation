using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Deviation.Exchange.Scripts.Interface
{
	public interface IPlayerAccount
	{
		long Id { get; set; }
		string Name { get; set; }
		string Alias { get; set; }
	}
}
