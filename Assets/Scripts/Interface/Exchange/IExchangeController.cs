using Assets.Scripts.Enum;
using Assets.Scripts.Interface.Exchange;
using UnityEngine;

namespace Assets.Scripts.Interface
{
    public interface IExchangeController
    {
		int NumberOfPlayers { get; }
		Battlefield MainPlayerFieldNumber { get; }
		GameObject MainPlayerObject { get; set; }
		IPlayer[] Players { get; set; }
		ExchangeState ExchangeState { get; set; }

		void UpdateExchangeControlsDisplay();
		void ClickOnButton(string groupUIName, string buttonName);
		void ChangeStateToStart();
		void ChangeStateToPause();
		void Unpause();
	}
}

