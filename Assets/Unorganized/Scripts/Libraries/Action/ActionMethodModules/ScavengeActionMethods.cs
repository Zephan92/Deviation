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

					System.Action<Collider, GameObject, IAttack> onTriggerEnterMethod = delegate(Collider other, GameObject actionGO, IAttack actionAttack)
					{
						IExchangePlayer otherPlayer = other.GetComponent<IExchangePlayer>();
						actionAttack.InitiateAttack(new List<IExchangePlayer>{ otherPlayer}, AttackAlignment.Enemies );
						actionAttack.ApplyEffect(new List<IExchangePlayer>{ otherPlayer}, StatusEffect.Root, 1f);
						actionAttack.ApplyEffect(new List<IExchangePlayer>{ otherPlayer}, StatusEffect.HealthRate, 1f, -0.005f);
						Destroy(actionGO);
					};

					int numStuns = 5;
					int[,] stunLocations = new int[numStuns, 2];
					stunLocations = ActionUtilities.InitializeZones(stunLocations, numStuns);
					for (int i = 0; i < numStuns; i++)
					{
						stunLocations = ActionUtilities.PickZone(stunLocations, i);
						int x = (int) originX + stunLocations[i,0];
						int z = (int) originZ + stunLocations[i,1];

						System.Action onDelayStart = delegate()
						{
							bc.SetGridSpaceColor(x,z,Color.yellow);
						};

						System.Action onDelayEnd = delegate()
						 {
							 if(zone == BattlefieldZone.Left)
							 {
								bc.SetGridSpaceColor(x,z,Color.red);
							 }
							 else
							 {
								 bc.SetGridSpaceColor(x,z,Color.blue);
							 }
						 };

						bc.SpawnActionObject(0.5f, 4f, "StunTrigger", new Vector3(x, 0, z), attack, 
							onTriggerAction: onTriggerEnterMethod, 
							onDelayStartAction: onDelayStart, 
							onDelayEndAction: onDelayEnd );
						bc.SetGridSpaceColor(x,z,Color.yellow);
					}
				}
			},
		};
	}
}
