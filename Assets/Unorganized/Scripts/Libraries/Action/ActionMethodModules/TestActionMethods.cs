using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using Assets.Scripts.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ModuleActions
{
	public class TestActions : MonoBehaviour
	{
		public static readonly Dictionary<string, System.Action<IBattlefieldController, IAttack, IPlayer>> ActionMethodLibraryTable = new Dictionary<string, System.Action<IBattlefieldController, IAttack, IPlayer>>
		{
			{"OneHitKO", //Instant Death to the enemy DEBUG ONLY
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
