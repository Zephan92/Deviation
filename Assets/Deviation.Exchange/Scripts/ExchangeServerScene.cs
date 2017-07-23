using Barebones.MasterServer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExchangeServerScene : MonoBehaviour
{
	public HelpBox _header = new HelpBox("Connects to Master server, " + "and switches to an appropriate scene");
	public string scene = "1v1Exchange";
	// Use this for initialization
	void Start()
	{
		// Wait until we're connected to master server
		Msf.Connection.AddConnectionListener(OnConnectedToMaster, true);
	}

	private void OnConnectedToMaster()
	{
		if (Msf.Args.IsProvided(Msf.Args.LoadScene))
		{
			Logs.Info("Load Scene is: " + Msf.Args.LoadScene);
			scene = Msf.Args.LoadScene;
		}

		if (scene == null || scene == "")
		{
			Logs.Error("A scene to load was not provided");
			return;
		}

		SceneManager.LoadScene(scene);
	}
}
