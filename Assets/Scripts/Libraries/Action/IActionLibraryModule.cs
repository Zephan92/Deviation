using Assets.Scripts.Interface.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Library.Action
{
	public interface IActionLibraryModule
	{
		Dictionary<string, IExchangeAction> Actions { get; }
		string ModuleName { get; }
	}
}
