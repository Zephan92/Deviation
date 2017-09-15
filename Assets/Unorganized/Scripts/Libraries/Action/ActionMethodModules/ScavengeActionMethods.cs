using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using Assets.Scripts.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ModuleActions
{
	public class ScavengeActions : MonoBehaviour
	{
		public static readonly Dictionary<string, System.Action<IBattlefieldController, IAttack, IExchangePlayer, BattlefieldZone>> ActionMethodLibraryTable = new Dictionary<string, System.Action<IBattlefieldController, IAttack, IExchangePlayer, BattlefieldZone>>
		{
			{"StunField", //this method spawns 5 stun traps around the opponent, each trap will stun and damage the opponent if touched 
				delegate (IBattlefieldController bc, IAttack attack, IExchangePlayer player, BattlefieldZone zone)
				{
					var enemyZone = ActionUtilities.GetEnemyBattlefieldZone(zone);
					attack.InitiateAttack(new List<IExchangePlayer>{ player}, AttackAlignment.Allies );
					
					Vector3 origin = bc.GetBattlefieldCoordinates(enemyZone);
					float originX = origin.x;
					float originZ = origin.z;

					int numStuns = 5;
					int[,] stunLocations = new int[numStuns, 2];
					stunLocations = ActionUtilities.InitializeZones(stunLocations, numStuns);
					for (int i = 0; i < numStuns; i++)
					{
						stunLocations = ActionUtilities.PickZone(stunLocations, i);
						bc.Spawn(4f, "StunTrigger", new Vector3(originX + stunLocations[i,0], 0, originZ + stunLocations[i,1]));
					}
				}
			},
		};
	}
}
