﻿using Assets.Scripts.Enum;
using Assets.Scripts.Interface.Exchange;
using System;
using UnityEngine;

namespace Assets.Scripts.Interface.DTO
{
	public interface IExchangeAction
	{
		Guid Id { get; set; }
		string Name { get; set; }
		IAttack Attack { get; set; }
		Texture2D ActionTexture { get; set; }
		float Cooldown { get; set; }
		IKit ParentKit { get; set; }
		string PrimaryActionName { get; set; }
		ModuleType Type { get; }

		void InitiateAttack(IBattlefieldController bc, BattlefieldZone zone);
	}
}