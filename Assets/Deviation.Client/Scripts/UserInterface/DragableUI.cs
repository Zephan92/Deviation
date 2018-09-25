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
		private GameObject _clone;
		private SnapPoint _currentSnap;

		public UnityAction OnBeginDragAction;
		public UnityAction<GameObject> OnCloneBeginDragAction;
		public UnityAction OnDragAction;
		public UnityAction OnEndDragSuccessAction;
		public UnityAction<GameObject> OnCloneEndDragSuccessAction;
		public UnityAction OnEndDragFailureAction;
		public UnityAction<GameObject> OnCloneReturnToOriginalParent;

		public void Awake()
		{
			_origParent = gameObject.transform.parent;
		}

		public void Start()
		{
			SnapPoints = FindObjectsOfType<SnapPoint>();
		}

		public void BeginDrag(bool copyPanel = false)
		{
			OnBeginDragAction?.Invoke();
			_oldPos = transform.position;

			if (copyPanel)
			{
				_clone = Instantiate(gameObject, gameObject.transform.parent);
				_clone.name = gameObject.name;
				OnCloneBeginDragAction?.Invoke(_clone);
				_clone.transform.SetParent(_clone.transform.root);
			}
			else
			{
				transform.SetParent(transform.root);
			}

			Offset = new Vector2(transform.position.x,transform.position.y) - new Vector2( Input.mousePosition.x, Input.mousePosition.y);
		}

		public void OnDrag(bool copyPanel = false)
		{
			OnDragAction?.Invoke();

			if (copyPanel)
			{
				_clone.transform.position = new Vector3(Offset.x + Input.mousePosition.x, Offset.y + Input.mousePosition.y, 0);
			}
			else
			{
				transform.position = new Vector3(Offset.x + Input.mousePosition.x, Offset.y + Input.mousePosition.y, 0);
			}
		}

		public void EndDrag<T>(onEndDrag<T> validSnapCheck, T type, bool copyPanel = false)
		{
			foreach (SnapPoint snap in SnapPoints)
			{
				if (snap.Area.Contains(Input.mousePosition) && validSnapCheck(snap, type))
				{
					if (snap.IsOccupied)
					{
						snap.CurrentOccupant?.GetComponent<DragableUI>().ReturnToOriginalParent();
					}

					float x = snap.Area.x + snap.Area.width / 2;
					float y = snap.Area.y + snap.Area.height / 2;
					Vector2 newPos = new Vector2(x, y);

					if (copyPanel)
					{
						_clone.transform.position = newPos;
						_clone.transform.SetParent(snap.transform, true);

						snap.OccupySnap(_clone);
						OnCloneEndDragSuccessAction?.Invoke(_clone);
					}
					else
					{
						transform.position = newPos;
						transform.SetParent(snap.transform, true);

						snap.OccupySnap(gameObject);
						_currentSnap = snap;
					}

					OnEndDragSuccessAction?.Invoke();
					return;
				}
			}

			ReturnToOriginalParent(copyPanel);
		}

		public void ReturnToOriginalParent(bool copyPanel = false)
		{
			if (copyPanel)
			{
				OnCloneReturnToOriginalParent?.Invoke(_clone);
				if (_clone != null)
				{
					_clone.transform.SetParent(_origParent);
					_clone.transform.position = _oldPos;
				}
			}
			else
			{
				_currentSnap?.UnoccupySnap();
				transform.SetParent(_origParent);
				transform.position = _oldPos;
			}

			OnEndDragFailureAction?.Invoke();
			_currentSnap?.UnoccupySnap();
		}
	}
}
