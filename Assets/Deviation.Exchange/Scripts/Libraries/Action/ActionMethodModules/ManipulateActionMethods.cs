using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Enum;
using Assets.Scripts.Exchange;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using Assets.Scripts.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ModuleActions
{
	public class ManipulateActions : MonoBehaviour
	{
		public static readonly Dictionary<string, System.Action<IBattlefieldController, IAttack, IExchangePlayer, BattlefieldZone>> ActionMethodLibraryTable = new Dictionary<string, System.Action<IBattlefieldController, IAttack, IExchangePlayer, BattlefieldZone>>
		{
			{"ShortAttack", //this is a generic attack that does no battlefield affects
				delegate (IBattlefieldController bc, IAttack attack, IExchangePlayer player, BattlefieldZone zone)
				{
					//IPlayer enemy = player.Enemies[0];
					//attack.Attacker = player;
					//attack.Defender = enemy;
					//attack.InitiateAttack();
				}
			},
			{"MiddleAttack", //this is a generic attack that does no battlefield affects
				delegate (IBattlefieldController bc, IAttack attack, IExchangePlayer player, BattlefieldZone zone)
				{
					//IPlayer enemy = player.Enemies[0];
					//attack.Attacker = player;
					//attack.Defender = enemy;
					//attack.InitiateAttack();
				}
			},
			{"Steal",  //this is a generic attack that does no battlefield affects
				delegate (IBattlefieldController bc, IAttack attack, IExchangePlayer player, BattlefieldZone zone)
				{
					//IPlayer enemy = player.Enemies[0];
					//attack.Attacker = player;
					//attack.Defender = enemy;
					//attack.InitiateAttack();
				}
			},
			{"Ambush", //this makes you appear behind a random enemy and slash
				delegate (IBattlefieldController bc, IAttack attack, IExchangePlayer player, BattlefieldZone zone)
				{
					var enemies = bc.GetPlayers(player.EnemyZone);
					var target = enemies[Random.Range(0, enemies.Count)];

					GridCoordinate ambushCoordinate = target.Mover.CurrentCoordinate;

					System.Action<GameObject> onDestoyMethod = delegate(GameObject actionGo)
					{
						if(ambushCoordinate.GetAdjacentGridCoordinate(Direction.Down, 1).Valid(player.EnemyZone))
						{
							bc.gm.SetGridspaceOccupied(ambushCoordinate.GetAdjacentGridCoordinate(Direction.Down, 1), false, player.EnemyZone);
						}
					};

					System.Action onDelayStart = delegate()
					{
						if(ambushCoordinate.GetAdjacentGridCoordinate(Direction.Left, 1).Valid())
						{
							bc.gm.ResetGridSpaceColor(ambushCoordinate.GetAdjacentGridCoordinate(Direction.Left, 1), player.EnemyZone);
						}

						if(ambushCoordinate.Valid())
						{
							bc.gm.SetGridSpaceColor(ambushCoordinate, Color.yellow, player.EnemyZone);
						}

						if(ambushCoordinate.GetAdjacentGridCoordinate(Direction.Right, 1).Valid())
						{
							bc.gm.SetGridSpaceColor(ambushCoordinate.GetAdjacentGridCoordinate(Direction.Right, 1), Color.yellow, player.EnemyZone);
						}
					};

					System.Action onDelayEnd = delegate()
					{
						List<IExchangePlayer> enemiesHit = new List<IExchangePlayer>();

						if(ambushCoordinate.GetAdjacentGridCoordinate(Direction.Left, 1).Valid())
						{
							bc.gm.ResetGridSpaceColor(ambushCoordinate.GetAdjacentGridCoordinate(Direction.Left, 1), player.EnemyZone);
						}

						if(ambushCoordinate.Valid())
						{
							bc.gm.ResetGridSpaceColor(ambushCoordinate,  player.EnemyZone);
						}

						if(ambushCoordinate.GetAdjacentGridCoordinate(Direction.Right, 1).Valid())
						{
							bc.gm.ResetGridSpaceColor(ambushCoordinate.GetAdjacentGridCoordinate(Direction.Right, 1), player.EnemyZone);
						}

						foreach(var enemy in enemies)
						{
							if(	enemy.Mover.CurrentCoordinate.Equals(ambushCoordinate.GetAdjacentGridCoordinate(Direction.Left, 1)) ||
								enemy.Mover.CurrentCoordinate.Equals(ambushCoordinate) ||
								enemy.Mover.CurrentCoordinate.Equals(ambushCoordinate.GetAdjacentGridCoordinate(Direction.Right, 1)))
							{
								enemiesHit.Add(enemy);
							}
						}

						attack.InitiateAttack(enemiesHit, AttackAlignment.Enemies);
					};

					bc.ActionWarning(1f, onDelayStart, onDelayEnd);
					player.Hide(1f);
					if(ambushCoordinate.GetAdjacentGridCoordinate(Direction.Down, 1).Valid(player.EnemyZone))
					{
						bc.gm.SetGridspaceOccupied(ambushCoordinate.GetAdjacentGridCoordinate(Direction.Down, 1), true, player.EnemyZone);
					}
					bc.SpawnActionObject(0.0f, 1f, "PlayerPlaceholder", target.Position, attack, rotation: Quaternion.Euler(target.Rotation.eulerAngles),
						onDestroyAction: onDestoyMethod);
				}
			},
			{"WallPush", // this method spawns a wall and pushes the opponent to the middle if it hits them dealing some damage
				delegate (IBattlefieldController bc, IAttack attack, IExchangePlayer player, BattlefieldZone zone)
				{
					//IPlayer enemy = player.Enemies[0];
					//attack.Attacker = player;
					//attack.Defender = enemy;
					//attack.InitiateRecoil();
					//Vector3 origin = ActionUtilities.FindOrigin(enemy);
					////float originX = origin.x;
					//float originZ = origin.z;
					//BattlefieldZone field = enemy.BattlefieldZone;
					//float timeout = 0.5f;

					//System.Action<Collider, GameObject, IAttack> onTriggerEnterMethod = delegate(Collider other, GameObject projectile, IAttack thing)
					//{
					//	if(other.name.Equals("Player"))
					//	{
					//		//Player otherPlayer = other.gameObject.GetComponent<Player>();
					//		attack.SetDefender(player);
					//		attack.InitiateDrain();
					//		if (player.CurrentColumn == -2)
					//		{
					//			player.MoveObject(Direction.Right, 2, true);
					//		}
					//		else
					//		{
					//			player.MoveObject(Direction.Right, 1, true);
					//		}
					//		bc.SetBattlefieldState(player.BattlefieldZone, ExchangeUtilities.ConvertToArrayNumber(player.CurrentRow), ExchangeUtilities.ConvertToArrayNumber(player.CurrentColumn), true);
					//	}
					//};

					//System.Action<GameObject> onStartAction = delegate(GameObject projectile)
					//{
					//};

					//bc.SpawnProjectile(timeout, "Wall", new Vector3(0,0,originZ), enemy.Transform.rotation, attack, onTriggerEnterMethod, onStartAction);

					//for(int i = -2; i <= 2; i++)
					//{
					//	bc.SetBattlefieldState(field, ExchangeUtilities.ConvertToArrayNumber(i), ExchangeUtilities.ConvertToArrayNumber(-1), true);
					//	bc.SetBattlefieldState(field, ExchangeUtilities.ConvertToArrayNumber(i), ExchangeUtilities.ConvertToArrayNumber(-2), true);
					//	bc.SetBattlefieldStateAfterTimout(timeout, field, ExchangeUtilities.ConvertToArrayNumber(i), ExchangeUtilities.ConvertToArrayNumber(-1), false);
					//	bc.SetBattlefieldStateAfterTimout(timeout, field, ExchangeUtilities.ConvertToArrayNumber(i), ExchangeUtilities.ConvertToArrayNumber(-2), false);
					//}

				}
			},
		};
	}
}
