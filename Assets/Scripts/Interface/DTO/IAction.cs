using Assets.Scripts.Interface.Exchange;
using UnityEngine;

namespace Assets.Scripts.Interface.DTO
{
	public interface IAction
	{
		string Name { get; set; }
		IAttack Attack { get; set; }
		Color ActionTexture { get; set; }
		float Cooldown { get; set; }
		IModule ParentModule { get; set; }
		string PrimaryActionName { get; set; }

		void InitiateAttack(IBattlefieldController bc);

		IAction GetRightAction();
		IAction GetLeftAction();	
	}
}
