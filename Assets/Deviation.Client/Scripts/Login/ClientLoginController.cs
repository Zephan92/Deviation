using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Barebones.MasterServer;
using UnityEngine.SceneManagement;
using Assets.Deviation.Exchange.Scripts.Client;
using Assets.Deviation.Client.Scripts;
public class ClientLoginController : ControllerBase
{
	public InputField Username;
	public InputField Password;
	public Toggle RememberMe;

	private EventSystem system;
	private Button[] buttons;

	public override void Start()
	{
		base.Start();
		ClientDataRepository.Instance.State = ClientState.Login;
		ClientDataRepository.Instance.OnLoginServer += LoginSuccessful;

		system = EventSystem.current;
		var signInPanel = GameObject.Find("Sign In Panel");
		Username = signInPanel.transform.Find("Username").GetComponentInChildren<InputField>();
		Password = signInPanel.transform.Find("Password").GetComponentInChildren<InputField>();
		RememberMe = signInPanel.transform.Find("Remember Me").GetComponentInChildren<Toggle>();
		if (PlayerPrefs.HasKey("RememberUsername"))
		{
			string remUser = PlayerPrefs.GetString("RememberUsername");

			if(remUser.Equals("True"))
			{
				RememberMe.isOn = true;
			}
		} 

		if (PlayerPrefs.HasKey("Username") && RememberMe.isOn)
		{
			Username.text = PlayerPrefs.GetString("Username");
		}

		buttons = FindObjectsOfType<Button>();

		foreach (Button button in buttons)
		{
			if (button.name.Equals("Sign In Button") &&          
				button.interactable)
			{
				button.interactable = false;
				button.onClick.AddListener(Login);;
			}
		}
	}

	internal override void OnDataCreated()
	{
		//We don't want this method to do anything in the base class when we test
	}

	public override void Update()
	{
		base.Update();

		//put this in a method
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

		//this too
		foreach (Button button in buttons)
		{
			if (button.name.Equals("Sign In Button") &&
				!button.interactable &&
				!Username.text.Equals("") && 
				!Password.text.Equals("") &&
				Msf.Client.Connection.IsConnected)
			{
				button.interactable = true;
			}
		}
	}

	public void Login()
	{
		OnRememberMe();

		ClientDataRepository.Instance.LoginAsGuest();

		//Msf.Client.Auth.LogIn(Username.text, Password.text, (successful, error) =>
		//{
		//	UnityEngine.Debug.Log("Is successful: " + successful + "; Error (if exists): " + error);
		//});

	}

	private void LoginSuccessful(AccountInfoPacket successful, string error)
	{
		if (error != null && error != "")
		{
			UnityEngine.Debug.LogError("Error when trying to login: " + error);
			return;
		}
		
		ClientDataRepository.Instance.GetPlayerAccount();
		ClientDataRepository.Instance.PlayerAccountRecieved += (playerAccount) => {
			SceneManager.LoadScene("DeviationClient - Client");
		};
	}

	public void CreateAccount()
	{
		Application.OpenURL("https://deviationgame.com/");
	}

	public void OnRememberMe()
	{
		if (!Username.text.Equals("") && RememberMe.isOn)
		{
			PlayerPrefs.SetString("Username", Username.text);
			PlayerPrefs.SetString("RememberUsername", "True");
			PlayerPrefs.Save();
		}
		else if(!RememberMe.isOn)
		{
			PlayerPrefs.SetString("Username", "");
			PlayerPrefs.SetString("RememberUsername", "False");
		}
	}
}
