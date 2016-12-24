using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		void InitiateAttack(IBattlefieldController bc);

		IAction GetRightAction();
		IAction GetLeftAction();	
	}
}
