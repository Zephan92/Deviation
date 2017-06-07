using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Library.Action.ModuleActions
{
	public interface IActionMethodLibraryModule
	{
		Dictionary<string, System.Action<IBattlefieldController, IAttack, IPlayer>> ActionMethods { get; }
	}
}
