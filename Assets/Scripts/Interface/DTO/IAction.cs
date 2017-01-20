using Assets.Scripts.Interface.Exchange;
using UnityEngine;

namespace Assets.Scripts.Interface.DTO
{
	public interface IExchangeAction
	{
		string Name { get; set; }
		IAttack Attack { get; set; }
		Color ActionTexture { get; set; }
		float Cooldown { get; set; }
		IModule ParentModule { get; set; }
		string PrimaryActionName { get; set; }

		void InitiateAttack(IBattlefieldController bc);

		IExchangeAction GetRightAction();
		IExchangeAction GetLeftAction();	
	}
}
