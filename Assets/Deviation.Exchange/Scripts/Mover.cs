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
		if (isClient)
		{
			CmdUpdateRow(value);
		}

		_currentRow = value;
		
	}

	[Command]
	private void CmdUpdateRow(int value)
	{
		_currentRow = value;
	}

	public void UpdateColumn(int value)
	{
		if (isClient)
		{
			CmdUpdateColumn(value);
		}
		
		_currentColumn = value;
	}

	[Command]
	private void CmdUpdateColumn(int value)
	{
		_currentColumn = value;
	}

	public GridCoordinate CurrentCoordinate
	{
		get
		{
			return new GridCoordinate(_currentRow, _currentColumn, _zone);
		}
		set
		{
			UpdateColumn(value.Column);
			UpdateRow(value.Row);
			UpdateZone(value.Zone);
		}
	}

	private MovingDetails _movingDetails;
	private MovingDetails _movingDetailsNext;

	private ICoroutineManager cm;
	private IGridManager gm;

	private BattlefieldZone _zone;

	public void UpdateZone(BattlefieldZone value)
	{
		if (isClient)
		{
			CmdUpdateZone(value);
		}

		_zone = value;
	}

	[Command]
	private void CmdUpdateZone(BattlefieldZone value)
	{
		_zone = value;
	}

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
		GridCoordinate destination;

		if (_rooted)
		{
			return;
		}

		if (_movingDetails != null)
		{
			if (_movingDetails.GetDistanceTraveledPercentage() >= 0.5f)
			{
				destination = CurrentCoordinate.GetAdjacentGridCoordinate(direction, distance, _movingDetails.Destination, true);
			}
			else
			{
				return;
			}
		}
		else
		{
			destination = CurrentCoordinate.GetAdjacentGridCoordinate(direction, distance);
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
		CmdUpdateBattlefield(oldCoordinate.Row, oldCoordinate.Column, _movingDetails.Destination.Row, _movingDetails.Destination.Column, _zone);
		_movingDetails = null;
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
