using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Enum;
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
		IExchangePlayer Player { get; set; }
		string PrimaryActionName { get; set; }
		TraderType Type { get; }
		string Description { get; set; }
		void InitiateAttack(IBattlefieldController bc, BattlefieldZone zone);
		int GetHashCode();
		bool Equals(object obj);
	}
}
