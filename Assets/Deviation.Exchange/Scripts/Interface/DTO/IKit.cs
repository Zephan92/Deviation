using Assets.Scripts.Interface.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Interface.DTO
{
	public interface IKit
	{
		IExchangePlayer Player { get; set; }
		IExchangeAction[] Actions { get; }
		string[] ActionsNames { get; }
		Guid[] ActionsGuids { get; }

	}
}
