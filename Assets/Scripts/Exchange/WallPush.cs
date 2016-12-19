
using System;
using Assets.Scripts.Controllers;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using UnityEngine;

namespace Assets.Scripts.Exchange
{
	public class WallPush : MonoBehaviour, IExchangeAttack
	{
		private static ExchangeController ec;
		private static BattlefieldController bc;
		private MovingDetails _movingDetails;
		public int CurrentColumn = 0;
		public int CurrentRow = 0;

		private Attack Attack;

		public void Awake()
		{
			_movingDetails = new MovingDetails(new Vector3(transform.position.x + 2, 0, transform.position.z),Direction.Right);

			if (bc == null)
			{
				var bcObject = GameObject.FindGameObjectWithTag("BattlefieldController");
				bc = bcObject.GetComponent<BattlefieldController>();
			}

			if (ec == null)
			{
				var ecObject = GameObject.FindGameObjectWithTag("ExchangeController");
				ec = ecObject.GetComponent<ExchangeController>();
			}

			CurrentColumn = (int) transform.localPosition.x;
			CurrentRow = (int) transform.localPosition.z;
		}

		public void Update()
		{
			if (_movingDetails != null)
			{
				if (transform.localPosition.x >= _movingDetails.Destination.x)
				{
					CurrentColumn += 2;
					UpdateTransform(CurrentRow, CurrentColumn);
					_movingDetails = null;
				}
				else
				{
					UpdateTransform(transform.localPosition.z, transform.localPosition.x + 0.1f);
				}
			}
		}

		private void UpdateTransform(float row, float column)
		{
			transform.localPosition = new Vector3(column, 0, row);
		}

		public void OnTriggerEnter(Collider other)
		{
			if(other.name.Equals("Player"))
			{
				Player player = other.gameObject.GetComponent<Player>();
				Attack.SetDefender(player);
				Attack.InitiateDrain();
				ec.UpdateExchangeControlsDisplay();
				if (player.CurrentColumn == -2)
				{
					player.MoveObject(Direction.Right, 2, true);
				}
				else
				{
					player.MoveObject(Direction.Right, 1, true);
				}
				bc.SetBattlefieldState(player.CurrentBattlefield, ConvertToArrayNumber(player.CurrentRow), ConvertToArrayNumber(player.CurrentColumn), true);
			}
		}

		private int ConvertToArrayNumber(int input)
		{
			return input + 2;
		}

		private int ConvertFromArrayNumber(int input)
		{
			return input - 2;
		}

		public void SetAttack(Attack attack)
		{
			Attack = attack;
		}
	}
}
