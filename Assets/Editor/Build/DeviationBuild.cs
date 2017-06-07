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
	public static string Client = "Assets/Deviation.Client/DeviationClient.unity";
	public static string MasterServer = "Assets/Deviation.MasterServer/MasterServer.unity";
	public static string Exchange1v1Scene = "Assets/Deviation.Exchange/Exchange.1v1/GameServer/Scenes/1v1Exchange.unity";
	public static string Exchange1v1SpawnerServer = "Assets/Deviation.Exchange/Exchange.1v1/GameServer/Scenes/1v1ExchangeSpawnerServer.unity";
	public static string Exchange1v1GameServer = "Assets/Deviation.Exchange/Exchange.1v1/GameServer/Scenes/1v1ExchangeGameServer.unity";

	public static BuildTarget TargetPlatform = BuildTarget.StandaloneWindows;

	/// <summary>
	/// Build with "Development" flag, so that we can see the console if something 
	/// goes wrong
	/// </summary>
	public static BuildOptions BuildOptions = BuildOptions.Development;

	public static string PrevPath = null;

	[MenuItem("Tools/Deviation/Build All", false, 0)]
	public static void BuildGame()
	{
		var path = GetPath();
		if (string.IsNullOrEmpty(path))
			return;

		BuildMaster(path);
		BuildSpawner(path);
		BuildClient(path);
		BuildGameServer(path);
	}

	/// <summary>
	/// Creates a build for master
	/// </summary>
	/// <param name="path"></param>
	public static void BuildMaster(string path)
	{
		var masterScenes = new[]
		{
			MasterServer
		};

		BuildPipeline.BuildPlayer(masterScenes, path + "/MasterServer.exe", TargetPlatform, BuildOptions);
	}

	/// <summary>
	/// Creates a build for spawner
	/// </summary>
	/// <param name="path"></param>
	public static void BuildSpawner(string path)
	{
		var spawnerScenes = new[]
		{
			Exchange1v1SpawnerServer
		};

		BuildPipeline.BuildPlayer(spawnerScenes, path + "/1v1ExchangeSpawnerServer.exe", TargetPlatform, BuildOptions);
	}

	/// <summary>
	/// Creates a build for client
	/// </summary>
	/// <param name="path"></param>
	public static void BuildClient(string path)
	{
		var clientScenes = new[]
		{
			Client,

			// Add all the game scenes
			Exchange1v1Scene,

		};
		BuildPipeline.BuildPlayer(clientScenes, path + "/DeviationClient.exe", TargetPlatform, BuildOptions);
	}

	/// <summary>
	/// Creates a build for game server
	/// </summary>
	/// <param name="path"></param>
	public static void BuildGameServer(string path)
	{
		var gameServerScenes = new[]
		{
			Exchange1v1GameServer,
			// Add all the game scenes
			Exchange1v1Scene,

		};
		BuildPipeline.BuildPlayer(gameServerScenes, path + "/1v1ExchangeSpawnerServer.exe", TargetPlatform, BuildOptions);
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
		var prevPath = EditorPrefs.GetString("msf.buildPath", "");
		string path = EditorUtility.SaveFolderPanel("Choose Location for binaries", prevPath, "");

		if (!string.IsNullOrEmpty(path))
		{
			EditorPrefs.SetString("msf.buildPath", path);
		}
		return path;
	}
}