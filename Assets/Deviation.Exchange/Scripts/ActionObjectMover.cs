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
	public int _currentRow;
	[SyncVar]
	public int _currentColumn;
	[SyncVar]
	private bool _stopped = false;
	[SyncVar]
	public float _movementSpeed;

	public void UpdateRow(int value)
	{
		_currentRow = value;
		_currentCoordinate.Row = value;
	}

	public void UpdateColumn(int value)
	{
		_currentColumn = value;
		_currentCoordinate.Column = value;
	}

	private GridCoordinate _currentCoordinate;

	public GridCoordinate CurrentCoordinate
	{
		get
		{
			return _currentCoordinate;
		}
		set
		{
			_currentCoordinate = value;
			_currentColumn = value.Column;
			_currentRow = value.Row;
		}
	}

	private ICoroutineManager cm;
	private IGridManager gm;
	private IEnumerator _movingCoroutine;
	private ActionObject _actionObject;

	private bool _warningsEnabled = false;
	private bool _occupiedEnabled = false;
	private bool _outOfBounds = false;

	public void Awake()
	{
		gm = FindObjectOfType<GridManager>();
		cm = FindObjectOfType<CoroutineManager>();
		_actionObject = GetComponent<ActionObject>();
	}

	public void Init(GridCoordinate currentCoordinate, float movementSpeed, bool warningsEnabled = false, bool occupiedEnabled = false)
	{
		CurrentCoordinate = currentCoordinate;
		_movementSpeed = movementSpeed;
		_warningsEnabled = warningsEnabled;
		_occupiedEnabled = occupiedEnabled;
		cm.StartFixedCoroutineThread(MovingAction, ref _movingCoroutine);

		if (_warningsEnabled && CurrentCoordinate.Valid())
		{
			gm.SetGridSpaceColor(CurrentCoordinate, Color.yellow);
		}

		if (_occupiedEnabled && CurrentCoordinate.Valid())
		{
			gm.SetGridspaceOccupied(CurrentCoordinate, true);
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
			GridCoordinate newCoordinate = new GridCoordinate(transform.position);

			if (_warningsEnabled && CurrentCoordinate.Valid())
			{
				gm.ResetGridSpaceColor(CurrentCoordinate);
			}

			if (_occupiedEnabled && CurrentCoordinate.Valid())
			{
				gm.SetGridspaceOccupied(CurrentCoordinate, false);
			}

			if (_warningsEnabled && newCoordinate.Valid())
			{
				gm.SetGridSpaceColor(newCoordinate, Color.yellow);
			}

			if (_occupiedEnabled && newCoordinate.Valid())
			{
				gm.SetGridspaceOccupied(newCoordinate, true);
			}

			CurrentCoordinate = newCoordinate;
			_actionObject.OnTileEnter();
		}
	}

	private bool ChangedPos(Vector3 pos)
	{
		if (CurrentCoordinate.Row + 0.5f < pos.z || pos.z < CurrentCoordinate.Row - 0.5f)
		{
			return true;
		}
		else if (CurrentCoordinate.Column + 0.5f < pos.x || pos.x < CurrentCoordinate.Column - 0.5f)
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
		if (isServer)
		{
			StopObjectServer();
			RpcStopObject(CurrentCoordinate.Position_Vector3());
		}
	}

	public void StopObjectServer()
	{
		if (_stopped)
		{
			return;
		}

		_stopped = true;
		cm.StopCoroutineThread(ref _movingCoroutine);
		transform.position = CurrentCoordinate.Position_Vector3();

		if (_warningsEnabled && CurrentCoordinate.Valid())
		{
			gm.ResetGridSpaceColor(CurrentCoordinate);
		}

		if (_occupiedEnabled && CurrentCoordinate.Valid())
		{
			gm.SetGridspaceOccupied(CurrentCoordinate, false);
		}
	}

	[ClientRpc]
	public void RpcStopObject(Vector3 destination)
	{
		if (_stopped)
		{
			return;
		}

		_stopped = true;
		cm.StopCoroutineThread(ref _movingCoroutine);
		transform.position = destination;
	}
}
