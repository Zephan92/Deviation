using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
/// <summary>
/// Instead of editing this script, I would recommend to write your own
/// (or copy and change it). Otherwise, your changes will be overwriten when you
/// update project :)
/// </summary>
public class DeviationBuild
{
	/// <summary>
	/// Have in mind that if you change it, it might take "a while" 
	/// for the editor to pick up changes 
	/// </summary>
	public static string ClientLogin = "Assets/Deviation.Client/DeviationClient - Login.unity";
	public static string Client = "Assets/Deviation.Client/DeviationClient - Client.unity";
	public static string ClientMatch = "Assets/Deviation.Client/DeviationClient - Match.unity";
	public static string ClientExchange = "Assets/Deviation.Client/DeviationClient - Exchange.unity";
	public static string ClientResults = "Assets/Deviation.Client/DeviationClient - Results.unity";

	public static string MasterServer = "Assets/Deviation.MasterServer/MasterServer.unity";
	public static string Exchange1v1Scene = "Assets/Deviation.Exchange/1v1Exchange.unity";
	public static string Exchange1v1SpawnerServer = "Assets/Deviation.SpawnerServer/1v1ExchangeSpawnerServer.unity";
	public static string Exchange1v1GameServer = "Assets/Deviation.GameServer/1v1ExchangeGameServer.unity";

	public static BuildTarget TargetPlatform = BuildTarget.StandaloneWindows;

	private static Dictionary<string, string> _serverLocations = new Dictionary<string, string>();
	/// <summary>
	/// Build with "Development" flag, so that we can see the console if something 
	/// goes wrong
	/// </summary>
	public static BuildOptions BuildOptions = BuildOptions.Development;

	public static string PrevPath = null;

	[MenuItem("Tools/Deviation/Build All #F12", false, 0)]
	public static void BuildGame()
	{
		var path = GetPath();
		if (string.IsNullOrEmpty(path))
			return;

		UnityEngine.Debug.Log("Starting Deviation Build");
		BuildMaster(path);
		BuildSpawner(path);
		BuildClient(path);
		BuildGameServer(path);
		UnityEngine.Debug.Log("Finished Building");
		StartServer();
		StartClients();
	}

	[MenuItem("Tools/Deviation/Start Deviation Servers", false, 0)]
	public static void StartServer()
	{
		StartMaster();
		StartSpawner();
	}

	[MenuItem("Tools/Deviation/Start Deviation Clients", false, 0)]
	public static void StartClients()
	{
		var commandLineArgs = " -test GuestLogin";
		//var commandLineArgs = " -msfStartMaster -batchmode  -nographics";
		var exePath = GetServerLocation("DeviationClient");
		UnityEngine.Debug.Log(exePath + commandLineArgs);

		Process.Start(exePath, commandLineArgs);
		Process.Start(exePath, commandLineArgs);
	}


	[MenuItem("Tools/Deviation/Start Master Server", false, 11)]
	public static void StartMaster()
	{
		foreach (var process in Process.GetProcessesByName("MasterServer"))
		{
			UnityEngine.Debug.Log("Shutting down: " + process.ProcessName);
			process.Kill();
		}

		var commandLineArgs = " -msfStartMaster ";
		//var commandLineArgs = " -msfStartMaster -batchmode  -nographics";
		var exePath = GetServerLocation("MasterServer");
		UnityEngine.Debug.Log(exePath + commandLineArgs);

		Process.Start(exePath, commandLineArgs);
	}

	[MenuItem("Tools/Deviation/Start Spawner Server", false, 11)]
	public static void StartSpawner()
	{
		foreach (var process in Process.GetProcessesByName("1v1ExchangeSpawnerServer"))
		{
			UnityEngine.Debug.Log("Shutting down: " + process.ProcessName);
			process.Kill();
		}
		var gameServerExePath = GetServerLocation("1v1ExchangeGameServer");

		//var commandLineArgs = " -batchmode -nographics -msfStartSpawner -msfExe " + gameServerExePath;
		var commandLineArgs = "-msfStartSpawner -msfExe " + gameServerExePath;
		var exePath = GetServerLocation("1v1ExchangeSpawnerServer");

		UnityEngine.Debug.Log(exePath + commandLineArgs);

		Process.Start(exePath, commandLineArgs);
	}

	/// <summary>
	/// Creates a build for master
	/// </summary>
	/// <param name="path"></param>
	public static void BuildMaster(string path)
	{
		foreach (var process in Process.GetProcessesByName("MasterServer"))
		{
			UnityEngine.Debug.Log("Shutting down: " + process.ProcessName);
			process.Kill();
		}
		string exePath = path + "/MasterServer.exe";
		
		var masterScenes = new[]
		{
			MasterServer
		};
		AddServerLocationsToDict("MasterServer", exePath);
		UnityEngine.Debug.Log("Building MasterServer");

		BuildPipeline.BuildPlayer(masterScenes, exePath, TargetPlatform, BuildOptions);
	}

	/// <summary>
	/// Creates a build for spawner
	/// </summary>
	/// <param name="path"></param>
	public static void BuildSpawner(string path)
	{
		foreach (var process in Process.GetProcessesByName("1v1ExchangeSpawnerServer"))
		{
			UnityEngine.Debug.Log("Shutting down: " + process.ProcessName);
			process.Kill();
		}

		string exePath = path + "/1v1ExchangeSpawnerServer.exe";

		var spawnerScenes = new[]
		{
			Exchange1v1SpawnerServer
		};
		AddServerLocationsToDict("1v1ExchangeSpawnerServer", exePath);
		UnityEngine.Debug.Log("Building 1v1ExchangeSpawnerServer");

		BuildPipeline.BuildPlayer(spawnerScenes, exePath, TargetPlatform, BuildOptions);
	}

	/// <summary>
	/// Creates a build for client
	/// </summary>
	/// <param name="path"></param>
	public static void BuildClient(string path)
	{
		foreach (var process in Process.GetProcessesByName("DeviationClient"))
		{
			UnityEngine.Debug.Log("Shutting down: " + process.ProcessName);
			process.Kill();
		}

		string clientExePath = path + "/DeviationClient.exe";
		var clientScenes = new[]
		{
			ClientLogin,
			Client,
			ClientMatch,
			ClientExchange,
			ClientResults,

			Exchange1v1Scene,
		};

		AddServerLocationsToDict("DeviationClient", clientExePath);

		UnityEngine.Debug.Log("Building DeviationClient");
		BuildPipeline.BuildPlayer(clientScenes, clientExePath, TargetPlatform, BuildOptions);
	}

	/// <summary>
	/// Creates a build for game server
	/// </summary>
	/// <param name="path"></param>
	public static void BuildGameServer(string path)
	{
		foreach (var process in Process.GetProcessesByName("1v1ExchangeGameServer"))
		{
			UnityEngine.Debug.Log("Shutting down: " + process.ProcessName);
			process.Kill();
		}

		string exePath = path + "/1v1ExchangeGameServer.exe";
		var gameServerScenes = new[]
		{
			Exchange1v1GameServer,

			// Add all the game scenes
			Exchange1v1Scene,

		};
		AddServerLocationsToDict("1v1ExchangeGameServer", exePath);
		UnityEngine.Debug.Log("Building 1v1ExchangeGameServer");

		BuildPipeline.BuildPlayer(gameServerScenes, exePath, TargetPlatform, BuildOptions);
	}

	#region Editor Menu

	[MenuItem("Tools/Deviation/Build Master", false, 11)]
	public static void BuildMasterMenu()
	{
		var path = GetPath();
		if (!string.IsNullOrEmpty(path))
		{
			BuildMaster(path);
		}
	}

	[MenuItem("Tools/Deviation/Build Spawner", false, 11)]
	public static void BuildSpawnerMenu()
	{
		var path = GetPath();
		if (!string.IsNullOrEmpty(path))
		{
			BuildSpawner(path);
		}
	}

	[MenuItem("Tools/Deviation/Build Client", false, 11)]
	public static void BuildClientMenu()
	{
		var path = GetPath();
		if (!string.IsNullOrEmpty(path))
		{
			BuildClient(path);
		}
	}

	[MenuItem("Tools/Deviation/Build Game Server", false, 11)]
	public static void BuildGameServerMenu()
	{
		var path = GetPath();
		if (!string.IsNullOrEmpty(path))
		{
			BuildGameServer(path);
		}
	}


	#endregion

	public static string GetPath()
	{
		string prevPath = EditorPrefs.GetString("msf.buildPath", "");
		string path = "";
		if (prevPath == "")
		{
			path = EditorUtility.SaveFolderPanel("Choose Location for binaries", prevPath, "");
		}
		else
		{
			path = prevPath;
		}

		if (!string.IsNullOrEmpty(path))
		{
			EditorPrefs.SetString("msf.buildPath", path);
		}
		return path;
	}

	public static void AddServerLocationsToDict(string serverExe, string path)
	{
		if (_serverLocations.ContainsKey(serverExe))
		{
			_serverLocations[serverExe] = path;
		}
		else
		{
			_serverLocations.Add(serverExe, path);
		}
	}

	public static string GetServerLocation(string serverName)
	{
		if (_serverLocations.ContainsKey(serverName))
		{
			return _serverLocations[serverName];
		}
		else
		{
			var path = GetPath();
			if (string.IsNullOrEmpty(path))
				return "";
			var exePath = path + "/" + serverName + ".exe";
			AddServerLocationsToDict(serverName, exePath);
			return exePath;
		}
	}
}