using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Assets.Scripts.Enum;
using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Utilities;
using Assets.Scripts.Interface;

public class Mover : NetworkBehaviour
{
	[SyncVar]
	private bool _rooted;

	[SyncVar]
	private int _currentRow;

	[SyncVar]
	private int _currentColumn;

	[SyncVar]
	public float MovementSpeed;

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

	private MovingDetails _movingDetails;
	private MovingDetails _movingDetailsNext;

	private ICoroutineManager cm;
	private IGridManager gm;

	private BattlefieldZone _zone;

	public void Awake()
	{
		gm = FindObjectOfType<GridManager>();
		cm = FindObjectOfType<CoroutineManager>();	
	}

	public void FixedUpdate()
	{
		if (_movingDetails == null && _movingDetailsNext != null)
		{
			_movingDetails = _movingDetailsNext;
			_movingDetailsNext = null;
			Move();
		}
	}

	public void Init(BattlefieldZone zone, float movementSpeed)
	{
		CurrentCoordinate = new GridCoordinate(ExchangeConstants.PLAYER_INITIAL_ROW, ExchangeConstants.PLAYER_INITIAL_COLUMN, zone, true);
		MovementSpeed = movementSpeed;
		_zone = zone;
		_rooted = false;
	}

	public void SetRoot(bool root)
	{
		_rooted = root;
	}

	public void Move(Direction direction, int distance, bool force = false)
	{
		if (_rooted)
		{
			return;
		}

		GridCoordinate currentPosition = new GridCoordinate(transform.position);
		bool createMoveCoroutine = false;

		GridCoordinate destination = new GridCoordinate();
		float destPoint = 0f;

		if (_movingDetails != null)
		{
			if (_movingDetails.GetDistanceTraveledPercentage() >= 0.5f)
			{
				currentPosition = _movingDetails.Destination;
			}
			else
			{
				return;
			}
		}

		int zoneModifier = _zone == BattlefieldZone.Left ? 1 : -1;

		switch (direction)
		{
			case Direction.Up:
				destPoint = currentPosition.Column + distance * zoneModifier;
				destination = new GridCoordinate(currentPosition.Row, destPoint, _zone);
				break;

			case Direction.Down:
				destPoint = currentPosition.Column - distance * zoneModifier;
				destination = new GridCoordinate(currentPosition.Row, destPoint, _zone);
				break;

			case Direction.Left:
				destPoint = currentPosition.Row + distance * zoneModifier;
				destination = new GridCoordinate(destPoint, currentPosition.Column, _zone);
				break;

			case Direction.Right:
				destPoint = currentPosition.Row - distance * zoneModifier;
				destination = new GridCoordinate(destPoint, currentPosition.Column, _zone);
				break;
		}

		if (destination.Valid(_zone) && !gm.GetGridspaceOccupied(destination, _zone))
		{
			if (_movingDetails == null)
			{
				_movingDetails = new MovingDetails(destination, direction, distance);
				Move();
			}
			else
			{
				_movingDetailsNext = new MovingDetails(destination, direction, distance);
			}
		}
	}

	public void Move()
	{
		Vector3 directionVector;

		switch (_movingDetails.MovingDirection)
		{
			case Direction.Up:
				directionVector = Vector3.forward;
				break;

			case Direction.Down:
				directionVector = Vector3.back;
				break;

			case Direction.Left:
				directionVector = Vector3.left;
				break;

			case Direction.Right:
				directionVector = Vector3.right;
				break;

			default:
				directionVector = Vector3.zero;
				break;
		}

		_movingDetails.AddDistanceTraveled(MovementSpeed);
		transform.Translate(directionVector * MovementSpeed);
		GridCoordinate oldCoordinate = CurrentCoordinate;
		CurrentCoordinate = _movingDetails.Destination;
		transform.position = _movingDetails.Destination.Position_Vector3();
		_movingDetails = null;
		CmdUpdateBattlefield(oldCoordinate.Row, oldCoordinate.Column, CurrentCoordinate.Row, CurrentCoordinate.Column, _zone);
	}

	[Command]
	private void CmdUpdateBattlefield(int oldRow, int oldColumn,int newRow, int newColumn, BattlefieldZone zone)
	{
		GridCoordinate oldCoordinate = new GridCoordinate(oldRow, oldColumn, zone);
		GridCoordinate newCoordinate = new GridCoordinate(newRow, newColumn, zone);
		if (!gm.GetGridSpaceDamaged(oldCoordinate, zone))
		{
			gm.SetGridspaceOccupied(oldCoordinate, false, zone);
			gm.ResetGridSpaceColor(oldCoordinate, zone);
		}

		gm.SetGridSpaceColor(newCoordinate, Color.gray, zone);
		gm.SetGridspaceOccupied(newCoordinate, true, zone);
	}
}
