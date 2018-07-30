using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Enum;
using Assets.Scripts.Exchange;
using Assets.Scripts.Exchange.Attacks;
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
					GridCoordinate originalCoordinate = player.Mover.CurrentCoordinate;

					System.Action onDelayStart = delegate()
					{
						if(ambushCoordinate.GetAdjacentGridCoordinate(Direction.Left, 1).Valid())
						{
							bc.gm.SetGridSpaceColor(ambushCoordinate.GetAdjacentGridCoordinate(Direction.Left, 1), Color.yellow, player.EnemyZone);
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

						attack.InitiateAttack(player, enemiesHit, AttackAlignment.Enemies);
						player.Mover.Move(originalCoordinate, new Vector3(0,180,0));
					};

					bc.ActionWarning(1f, onDelayStart, onDelayEnd);
					player.Mover.Move(ambushCoordinate.GetAdjacentGridCoordinate(Direction.Down, 1), new Vector3(0,180,0));
					player.Status.ApplyEffect(StatusEffect.Silence, 1f);
					player.Status.ApplyEffect(StatusEffect.Root, 1f);
				}
			},
			{"WallPush", // this method spawns a wall and pushes the opponent to the middle if it hits them dealing some damage
				delegate (IBattlefieldController bc, IAttack attack, IExchangePlayer player, BattlefieldZone zone)
				{
					for(int i = 0; i < 5; i++)
					{
						int startRow, endRow, column;

						if(player.EnemyZone == BattlefieldZone.Left)
						{
							startRow =  -1;
							endRow = 1;
							column = i;
						}
						else
						{
							startRow =  5;
							endRow = 3;
							column = i + 5;
						}

						var wallLocation = new GridCoordinate(startRow, column, player.EnemyZone);
						var endLocation =  new GridCoordinate(endRow, column, player.EnemyZone);

						System.Action<GameObject> onStartMethod = delegate(GameObject actionGO)
						{
							var mover = actionGO.GetComponent<ActionObjectMover>();
							mover.Init(wallLocation, 10, true, true);
						};

						System.Action<Collider, GameObject, IAttack> onTriggerEnter = delegate(Collider other, GameObject actionGO, IAttack actionAttack)
						{
							if(other.tag == "Player")
							{
								IExchangePlayer otherPlayer = other.GetComponent<IExchangePlayer>();
								if(!otherPlayer.Equals(player))
								{
									actionAttack.InitiateAttack(player, new List<IExchangePlayer>{ otherPlayer}, AttackAlignment.Enemies);
									otherPlayer.Mover.Move(Direction.Left, 1);
								}
							}
						};

						System.Action<GameObject> onUpdate = delegate(GameObject actionGO)
						{
							var mover = actionGO.GetComponent<ActionObjectMover>();
							if(player.EnemyZone == BattlefieldZone.Left && mover.transform.position.z >= endLocation.Row)
							{
								mover.StopObject();
							}
							else if(player.EnemyZone == BattlefieldZone.Right && mover.transform.position.z <= endLocation.Row)
							{
								mover.StopObject();
							}
						};

						bc.SpawnActionObject(i / 3f, 2f, "Wall", wallLocation.Position_Vector3(), attack,
							rotation: Quaternion.Euler(player.Rotation.eulerAngles + new Vector3(0,90,0)),
							onStartAction: onStartMethod,
							updateAction: onUpdate,
							onTriggerAction: onTriggerEnter);
					}
				}
			},
		};
	}
}
