using UnityEngine;
using System.Collections;
using Assets.Scripts.Interface;
using Assets.Scripts.Utilities;
using UnityEngine.Networking;
using Assets.Scripts.Enum;
using Assets.Scripts.Exchange.Attacks;

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
	private ActionObject _actionObject;

	private bool _warningsEnabled = false;
	private bool _outOfBounds = false;

	public void Awake()
	{
		bc = FindObjectOfType<ExchangeBattlefieldController>();
		cm = FindObjectOfType<CoroutineManager>();
		_actionObject = GetComponent<ActionObject>();
	}

	public void Init(int row, int column, float movementSpeed, bool warningsEnabled = false)
	{
		CurrentRow = row;
		CurrentColumn = column;
		_movementSpeed = movementSpeed;
		_warningsEnabled = warningsEnabled;
		cm.StartFixedCoroutineThread(MovingAction, ref _movingCoroutine);

		if (_warningsEnabled)
		{
			bc.SetGridSpaceColor(CurrentRow, CurrentColumn, Color.yellow);
		}

		RpcInit();
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
			PositionCheck();
		}
		else
		{
			transform.position = Vector3.Lerp(transform.position, _realPosition, Time.deltaTime);
			transform.rotation = Quaternion.Lerp(transform.rotation, _realRotation, Time.deltaTime);
		}
	}

	private void PositionCheck()
	{
		if (ChangedPos(transform.position))
		{
			if (_warningsEnabled && bc.IsInsideBattlefieldBoundaries(CurrentRow, CurrentColumn))
			{
				bc.ResetGridSpaceColor(CurrentRow, CurrentColumn);
			}

			if (bc.IsInsideBattlefieldBoundaries(transform.position))
			{
				int newCurrentRow = Mathf.RoundToInt(transform.position.z);
				int newCurrentColumn = Mathf.RoundToInt(transform.position.x);

				if (_warningsEnabled)
				{
					bc.SetGridSpaceColor(newCurrentRow, newCurrentColumn, Color.yellow);
				}

				CurrentRow = newCurrentRow;
				CurrentColumn = newCurrentColumn;
				_actionObject.OnTileEnter();
			}
			else
			{
				_outOfBounds = true;
			}
		}
	}

	private bool ChangedPos(Vector3 pos)
	{
		if (CurrentRow + 0.5f < pos.z || pos.z < CurrentRow - 0.5f)
		{
			return true;
		}
		else if (CurrentColumn + 0.5f < pos.x || pos.x < CurrentColumn - 0.5f)
		{
			return true;
		}

		return false;
	}

	[ClientRpc]
	private void RpcInit()
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
			if (_warningsEnabled && !_outOfBounds)
			{
				bc.ResetGridSpaceColor(CurrentRow, CurrentColumn);
			}
		}
	}
}
