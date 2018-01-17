using UnityEngine;
using Assets.Scripts.Enum;

public struct GridCoordinate
{
	#region Public Variables

	public int Row;
	public int Column;
	public BattlefieldZone Zone;

	public int LocalColumn
	{ 
		get
		{
			if (Zone == BattlefieldZone.Right)
			{
				return Column - 5;
			}
			else
			{
				return Column;
			}
		}
	}

	public Vector2 Position_Vector2()
	{
		return new Vector2(Column, Row);
	}

	public Vector3 Position_Vector3()
	{
		return new Vector3(Column, 0, Row);
	}

	#endregion

	#region Constructors

	public GridCoordinate(BattlefieldZone zone = BattlefieldZone.All, bool localCoordinates = false)
	{
		Row = 0;
		Column = 0;
		Zone = zone;

		if (localCoordinates && zone == BattlefieldZone.Right)
		{
			Column += 5;
		}
	}

	public GridCoordinate(int row, int column, BattlefieldZone zone = BattlefieldZone.All, bool localCoordinates = false)
	{
		Row = row;
		Column = column;
		Zone = zone;

		if(localCoordinates && zone == BattlefieldZone.Right)
		{
			Column += 5;
		}
	}

	public GridCoordinate(float row, float column, BattlefieldZone zone = BattlefieldZone.All, bool localCoordinates = false)
	{
		Row = Mathf.RoundToInt(row);
		Column = Mathf.RoundToInt(column);
		Zone = zone;

		if(localCoordinates && zone == BattlefieldZone.Right)
		{
			Column += 5;
		}
	}

	public GridCoordinate(Vector3 gridCoordinate, BattlefieldZone zone = BattlefieldZone.All)
	{
		Row = Mathf.RoundToInt(gridCoordinate.z);
		Column = Mathf.RoundToInt(gridCoordinate.x);
		Zone = zone;
	}

	#endregion

	public bool Valid(BattlefieldZone zone = BattlefieldZone.All)
	{
		int maxColumn = zone == BattlefieldZone.Left ? ExchangeConstants.BATTLEFIELD_LOCAL_COLUMN_COUNT : ExchangeConstants.BATTLEFIELD_COLUMN_COUNT;
		int minColumn = zone == BattlefieldZone.Right ? ExchangeConstants.BATTLEFIELD_LOCAL_COLUMN_COUNT : 0;
		//Debug.LogErrorFormat("Valid {0}. maxColumn: {1}. minColumn: {2}.", ToString(), maxColumn, minColumn);

		if (ExchangeConstants.BATTLEFIELD_ROW_COUNT > Row &&
			Row >= 0 &&
			maxColumn > Column &&
			Column >= minColumn)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public GridCoordinate GetAdjacentGridCoordinate(Direction direction, int distance, GridCoordinate startCoordinate = new GridCoordinate(), bool useStartPosition = false)
	{
		GridCoordinate currentPosition;
		GridCoordinate destination = new GridCoordinate();
		int zoneModifier = Zone == BattlefieldZone.Left ? 1 : -1;
		float destPoint = 0f;

		if (useStartPosition)
		{
			currentPosition = startCoordinate;
		}
		else
		{
			currentPosition = new GridCoordinate(Position_Vector3());
		}

		switch (direction)
		{
			case Direction.Up:
				destPoint = currentPosition.Column + distance * zoneModifier;
				destination = new GridCoordinate(currentPosition.Row, destPoint, Zone);
				break;

			case Direction.Down:
				destPoint = currentPosition.Column - distance * zoneModifier;
				destination = new GridCoordinate(currentPosition.Row, destPoint, Zone);
				break;

			case Direction.Left:
				destPoint = currentPosition.Row + distance * zoneModifier;
				destination = new GridCoordinate(destPoint, currentPosition.Column, Zone);
				break;

			case Direction.Right:
				destPoint = currentPosition.Row - distance * zoneModifier;
				destination = new GridCoordinate(destPoint, currentPosition.Column, Zone);
				break;
		}
		return destination;
	}

	public override string ToString()
	{
		return string.Format("({0}, {1}): {2}", Row, Column, Zone);
	}

	public bool Equals(GridCoordinate compare)
	{
		return	Row == compare.Row && 
				Column == compare.Column && 
				Zone == compare.Zone;
	}
}
