using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ModuleActions
{
	public class LifeActions : MonoBehaviour
	{

		public static readonly Dictionary<string, System.Action<IBattlefieldController, IAttack, IPlayer>> ActionMethodLibraryTable = new Dictionary<string, System.Action<IBattlefieldController, IAttack, IPlayer>>
		{
			{"Drain", //this method steals health from an enemy
				delegate (IBattlefieldController bc, IAttack attack, IPlayer player)
				{
					IPlayer enemy = player.Enemies[0];
					attack.Attacker = player;
					attack.Defender = enemy;
					attack.InitiateAttack();
				}
			},
		};
		
	}
}
