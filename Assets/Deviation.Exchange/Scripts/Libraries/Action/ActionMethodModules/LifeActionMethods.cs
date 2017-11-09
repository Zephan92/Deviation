using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utilities;

namespace Assets.Scripts.Library.Action.ModuleActions
{
	public class LifeActions : MonoBehaviour
	{
		public static readonly Dictionary<string, System.Action<IBattlefieldController, IAttack, IExchangePlayer, BattlefieldZone>> ActionMethodLibraryTable = new Dictionary<string, System.Action<IBattlefieldController, IAttack, IExchangePlayer, BattlefieldZone>>
		{
			{"Drain", //this method steals health from an enemy
				delegate (IBattlefieldController bc, IAttack attack, IExchangePlayer player, BattlefieldZone zone)
				{
					var enemyZone = ActionUtilities.GetEnemyBattlefieldZone(zone);
					List<IExchangePlayer> enemies = bc.GetPlayers(enemyZone);
					attack.InitiateAttack(new List<IExchangePlayer>{ player}, enemies);
				}
			},
		};
		
	}
}
