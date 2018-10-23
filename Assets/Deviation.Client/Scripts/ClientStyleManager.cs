﻿using Assets.Deviation.MasterServer.Scripts;
using Assets.Scripts.Utilities;
using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Deviation.Exchange.Scripts.Client
{
	public class ClientStyleManager : MonoBehaviour
	{
		public static ClientStyleManager instance = null;

		private int Width = 1280;
		private int Height = 720;
		public RectTransform draggableZonePanel;

		const int SWP_SHOWWINDOW = 0x0040;
		const int GWL_STYLE = -16;
		const int WS_BORDER = 1;
		const int Windowed = 349110272;

		POINT oldPos;
		bool borderless = false;
		IntPtr _currentWindow;
		bool _forceBorderless = true;

		private Transform MenuBar;
		public Button ExitButton;
		public Button MinimizeButton;
		public Button ToggleBorderlessButton;

		public void QuitClient()
		{
			Msf.Client.Connection.SendMessage((short)ExchangePlayerOpCodes.Logout, ClientDataRepository.Instance.PlayerAccount, (status, data) => { StartCoroutine(WaitForDisconnect()); });
		}

		private IEnumerator WaitForDisconnect()
		{
			Msf.Connection.Disconnect();
			if (Msf.Connection.IsConnected)
			{
				yield return new WaitUntil(() => Msf.Connection.IsConnected == false);
			}
			else
			{
				yield return null;
			}
			Quit();
		}

		private void Quit()
		{
#if UNITY_EDITOR
			// Application.Quit() does not work in the editor so
			// UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}

		public void Awake()
		{
			InstanceExists();
			var menu = transform.Find("Menu Bar");
			draggableZonePanel = transform.Find("DraggableZone").GetComponent<RectTransform>();
			foreach (var button in menu.GetComponentsInChildren<Button>())
			{
				switch (button.gameObject.name)
				{
					case "Borderless Button":
						ToggleBorderlessButton = button;
						ToggleBorderlessButton.onClick.AddListener(ToggleBorderless);
						break;
					case "Exit Button":
						ExitButton = button;
						ExitButton.onClick.AddListener(QuitClient);
						break;
					case "Minimize Button":
						MinimizeButton = button;
						MinimizeButton.onClick.AddListener(MinimizeClient);
						break;
				}
			}
		}

		public void InstanceExists()
		{
			if (instance == null)
			{
				instance = this;
			}
			else if (instance != this)
			{
				Destroy(gameObject.transform.parent.gameObject);
			}
		}

		public void Update()
		{
			var worldCorners = new Vector3[4];
			draggableZonePanel.GetWorldCorners(worldCorners);
			Rect draggableZone = new Rect(
				worldCorners[0].x,
				worldCorners[0].y,
				worldCorners[2].x - worldCorners[0].x,
				worldCorners[2].y - worldCorners[0].y);

			if (Input.GetMouseButtonDown(0) && draggableZone.Contains(Input.mousePosition))
			{
				if (!Application.isEditor)
				{
					POINT lpPoint;
					GetCursorPos(out lpPoint);
					oldPos = lpPoint;
					StartCoroutine(DragClient());
				}
			}

			if ((!borderless || GetClientWindow() == IntPtr.Zero || Screen.width != Width || Screen.height != Height) && _forceBorderless)
			{
				ChangeStyle(WS_BORDER);
			}
		}

		public IEnumerator DragClient()
		{
			while (Input.GetMouseButton(0))
			{
				RECT rct;
				GetWindowRect(GetActiveWindow(), out rct);
				POINT lpPoint;
				GetCursorPos(out lpPoint);
				int newX = rct.Left + lpPoint.x - oldPos.x;
				int newY = rct.Top + lpPoint.y - oldPos.y;
				SetWindowPos(GetClientWindow(), 0, newX, newY, Width, Height, SWP_SHOWWINDOW);
				oldPos = lpPoint;
				yield return null;
			}
		}

		public void MinimizeClient()
		{
			if (!Application.isEditor)
			{
				ShowWindow(GetClientWindow(), 2);
			}
		}

		public IntPtr GetClientWindow()
		{
			if (_currentWindow == IntPtr.Zero)
			{
				_currentWindow = GetActiveWindow();
			}

			return _currentWindow;
		}

		public void ToggleBorderless()
		{
			if (!Application.isEditor)
			{

				if (borderless)
				{
					ChangeStyle(Windowed);
				}
				else
				{
					ChangeStyle(WS_BORDER);
				}

				_forceBorderless = borderless;
			}
		}

		public void ChangeStyle(int windowStyle)
		{
			if (windowStyle == WS_BORDER)
			{
				borderless = true;
			}
			else
			{
				borderless = false;
			}

			if (!Application.isEditor)
			{
				RECT rct;
				GetWindowRect(GetClientWindow(), out rct);
				SetWindowLong(GetClientWindow(), GWL_STYLE, windowStyle);
				SetWindowPos(GetClientWindow(), 0, rct.Left, rct.Top, Width, Height, SWP_SHOWWINDOW);
			}
		}

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
		[DllImport("user32.dll")]
		private static extern IntPtr GetActiveWindow();
		[DllImport("user32.dll")]
		static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);
		[DllImport("user32.dll")]
		static extern UInt32 GetWindowLong(IntPtr hWnd, int _nIndex);
		[DllImport("user32.dll")]
		static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();
		[DllImport("user32.dll")]
		static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(out POINT lpPoint);

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;        // x position of upper-left corner
			public int Top;         // y position of upper-left corner
			public int Right;       // x position of lower-right corner
			public int Bottom;      // y position of lower-right corner
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int x;
			public int y;
		}
	}
}
