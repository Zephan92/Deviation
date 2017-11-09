using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Barebones.MasterServer;
using System.Runtime.InteropServices;

public class ClientController : MonoBehaviour
{
	private int Width = 1280;
	private int Height = 720;

	EventSystem system;
	public InputField Username;
	public InputField Password;
	private Button[] buttons;
	public RectTransform draggableZonePanel;

	const int SWP_SHOWWINDOW = 0x0040;
	const int GWL_STYLE = -16;
	const int WS_BORDER = 1;
	int Windowed;

	POINT oldPos;
	bool borderless = false;
	IntPtr _currentWindow;
	bool _forceBorderless = true;

	void Start()
	{
		Windowed = (int) GetWindowLong(GetClientWindow(), GWL_STYLE);

		system = EventSystem.current;
		buttons = FindObjectsOfType<Button>();

		foreach (Button button in buttons)
		{
			if (button.name.Equals("Sign In Button") &&          
				button.interactable)
			{
				button.interactable = false;
			}
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab) && system.currentSelectedGameObject != null)
		{
			Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

			if (next != null)
			{
				InputField inputfield = next.GetComponent<InputField>();
				if (inputfield != null)
				{
					inputfield.OnPointerClick(new PointerEventData(system));
				}

				system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
			}
		}

		foreach (Button button in buttons)
		{
			if (button.name.Equals("Sign In Button") &&
				!button.interactable &&
				!Username.text.Equals("") && 
				!Password.text.Equals(""))
			{
				button.interactable = true;
			}
		}

		var worldCorners = new Vector3[4];
		draggableZonePanel.GetWorldCorners(worldCorners);
		Rect draggableZone = new Rect(
			worldCorners[0].x,
			worldCorners[0].y,
			worldCorners[2].x - worldCorners[0].x,
			worldCorners[2].y - worldCorners[0].y);

		if (Input.GetMouseButtonDown(0) && draggableZone.Contains(Input.mousePosition))
		{
			POINT lpPoint;
			GetCursorPos(out lpPoint);
			oldPos = lpPoint;
			StartCoroutine(Test());
		}

		if (!borderless && _forceBorderless)
		{
			ChangeStyle(WS_BORDER);
			borderless = true;
		}
	}

	public void Login()
	{
		Msf.Client.Auth.LogIn(Username.text, Password.text, (successful, error) =>
		{
			UnityEngine.Debug.Log("Is successful: " + successful + "; Error (if exists): " + error);
		});
	}

	public void CreateAccount()
	{
		Application.OpenURL("http://unity3d.com/");
	}

	public void QuitClient()
	{
		Application.Quit();
	}

	public void OpenStandalone()
	{
		UnityEngine.Debug.Log("Opening Standalone");
		var commandLineArgs = "-show-screen-selector false -screen-height 900 -screen-width 1600";
		var exePath = Environment.GetEnvironmentVariable("DeviationStandalone", EnvironmentVariableTarget.User) + "/DeviationStandalone.exe";
		Process.Start(exePath, commandLineArgs);
	}

	public IEnumerator Test()
	{
		while(Input.GetMouseButton(0))
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
		ShowWindow(GetClientWindow(), 2);
	}

	public IntPtr GetClientWindow()
	{
		if (_currentWindow != null)
		{
			_currentWindow = GetActiveWindow();
		}

		return _currentWindow;
	}

	public void ToggleBorderless()
	{
		if (!Application.isEditor)
		{
			_forceBorderless = borderless ? false : true;

			if (borderless)
			{
				ChangeStyle(Windowed);
				borderless = false;
			}
			else
			{
				ChangeStyle(WS_BORDER);
				borderless = true;
			}
		}
	}

	public void ChangeStyle(int windowStyle)
	{
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
