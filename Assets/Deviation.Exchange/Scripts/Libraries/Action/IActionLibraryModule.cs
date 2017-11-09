using Assets.Scripts.Enum;
using Assets.Scripts.Interface.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Library.Action
{
	public interface IActionLibraryModule
	{
		Dictionary<Guid, IExchangeAction> Actions_ByGuid { get; }
		Dictionary<string, IExchangeAction> Actions_ByName { get; }
		ModuleType Type { get; }
	}
}
