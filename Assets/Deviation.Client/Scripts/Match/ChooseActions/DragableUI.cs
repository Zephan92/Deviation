using Assets.Scripts.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.ModuleEditor
{
	public class DragableUI : MonoBehaviour
	{
		Vector2 Offset;
		public SnapPoint[] SnapPoints;
		private Vector2 _oldPos;

		public delegate bool onEndDrag<T>(SnapPoint snap, T type);

		private Transform _origParent;

		public void Awake()
		{
			_origParent = gameObject.transform.parent;
		}

		public void Start()
		{
			SnapPoints = FindObjectsOfType<SnapPoint>();
		}

		public void BeginDrag()
		{
			_oldPos = transform.position;
			transform.SetParent(transform.root);

			Offset = new Vector2(transform.position.x,transform.position.y) - new Vector2( Input.mousePosition.x, Input.mousePosition.y);
		}

		public void OnDrag()
		{
			transform.position = new Vector3(Offset.x + Input.mousePosition.x, Offset.y + Input.mousePosition.y, 0);
		}

		public void EndDrag<T>(onEndDrag<T> validSnapCheck, T type)
		{
			foreach (SnapPoint snap in SnapPoints)
			{
				if (snap.Area.Contains(Input.mousePosition) && validSnapCheck(snap, type))
				{
					if (snap.IsOccupied)
					{
						if (snap.CurrentOccupant != null)
						{
							snap.CurrentOccupant.GetComponent<DragableUI>().ReturnToOrignalParent();
						}
					}

					float x = snap.Area.x + snap.Area.width / 2;
					float y = snap.Area.y + snap.Area.height / 2;
					Vector2 newPos = new Vector2(x, y);
					transform.position = newPos;
					transform.SetParent(snap.transform, true);

					snap.OccupySnap(gameObject);
					return;
				}
			}

			ReturnToOrignalParent();
		}

		public void ReturnToOrignalParent()
		{
			transform.SetParent(_origParent);
			transform.position = _oldPos;
		}
	}
}
