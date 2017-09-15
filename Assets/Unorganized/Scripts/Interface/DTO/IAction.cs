﻿using Assets.Scripts.Enum;
using Assets.Scripts.Interface.Exchange;
using UnityEngine;

namespace Assets.Scripts.Interface.DTO
{
	public interface IExchangeAction
	{
		string Name { get; set; }
		IAttack Attack { get; set; }
		Texture2D ActionTexture { get; set; }
		float Cooldown { get; set; }
		IModule ParentModule { get; set; }
		string PrimaryActionName { get; set; }
		ModuleType Type { get; }

		void InitiateAttack(IBattlefieldController bc, BattlefieldZone zone);
	}
}
