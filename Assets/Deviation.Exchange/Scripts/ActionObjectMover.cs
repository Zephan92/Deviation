using UnityEngine;
using System.Collections;
using Assets.Scripts.Interface;
using Assets.Scripts.Utilities;
using UnityEngine.Networking;
using Assets.Scripts.Enum;

public class ActionObjectMover : NetworkBehaviour
{
	[SyncVar]
	private Vector3 _realPosition = Vector3.zero;
	[SyncVar]
	private Quaternion _realRotation;
	[SyncVar]
	public int CurrentRow;
	[SyncVar]
	public int CurrentColumn;
	[SyncVar]
	private bool _stopped = false;
	[SyncVar]
	public float _movementSpeed;

	private ICoroutineManager cm;
	private ExchangeBattlefieldController bc;
	private IEnumerator _movingCoroutine;

	public void Awake()
	{
		bc = FindObjectOfType<ExchangeBattlefieldController>();
		cm = FindObjectOfType<CoroutineManager>();
	}

	public void SetMovingDetails(int row, int column, BattlefieldZone zone, float movementSpeed)
	{
		CurrentRow = row;
		CurrentColumn = column;
		_movementSpeed = movementSpeed;

		cm.StartFixedCoroutineThread(MovingAction, ref _movingCoroutine);
		bc.SetGridSpaceColor(CurrentRow, CurrentColumn, Color.yellow);
		RpcSetMovingDetails();
	}

	public void MovingAction()
	{
		if (_stopped)
		{
			return;
		}

		transform.Translate(Vector3.forward * _movementSpeed * Time.deltaTime);

		if (isServer)
		{
			_realPosition = transform.position;
			_realRotation = transform.rotation;
			UpdateWarning();
		}
		else
		{
			transform.position = Vector3.Lerp(transform.position, _realPosition, Time.deltaTime);
			transform.rotation = Quaternion.Lerp(transform.rotation, _realRotation, Time.deltaTime);
		}
	}

	private void UpdateWarning()
	{
		bool changedPos = false;
		int newCurrentRow = CurrentRow;
		int newCurrentColumn = CurrentColumn;

		if (Mathf.RoundToInt( transform.position.z) != CurrentRow)
		{
			newCurrentRow = Mathf.RoundToInt(transform.position.z);
			changedPos = true;
		}

		if (Mathf.RoundToInt(transform.position.x) != CurrentColumn)
		{
			newCurrentColumn = Mathf.RoundToInt(transform.position.x);
			changedPos = true;
		}

		if (changedPos)
		{
			bc.ResetGridSpaceColor(CurrentRow, CurrentColumn);

			if (bc.GetBattlefieldBoundaries().Contains(new Vector2(newCurrentColumn, newCurrentRow)))
			{
				bc.SetGridSpaceColor(newCurrentRow, newCurrentColumn, Color.yellow);
				CurrentRow = newCurrentRow;
				CurrentColumn = newCurrentColumn;
			}
		}
	}

	[ClientRpc]
	private void RpcSetMovingDetails()
	{
		cm.StartFixedCoroutineThread(MovingAction, ref _movingCoroutine);
	}

	public void OnDestroy()
	{
		StopObject();
	}

	public void StopObject()
	{
		if (_stopped)
		{
			return;
		}

		_stopped = true;
		cm.StopCoroutineThread(ref _movingCoroutine);

		if (isServer)
		{
			bc.ResetGridSpaceColor(CurrentRow, CurrentColumn);
		}
	}
}
