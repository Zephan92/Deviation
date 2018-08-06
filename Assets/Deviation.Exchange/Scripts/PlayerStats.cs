using Barebones.Networking;
using LiteDB;
using UnityEngine.Networking;

namespace Assets.Deviation.Exchange.Scripts
{
	public class PlayerStats : NetworkBehaviour
	{
		[SyncVar]
		public int Wins;
		[SyncVar]
		public int Losses;
		[SyncVar]
		public int Draws;
		[SyncVar]
		public bool Winner;
		[SyncVar]
		public int DamageDealt;
		[SyncVar]
		public int DamageTaken;
		[SyncVar]
		public int KnockoutsDealt;
		[SyncVar]
		public int KnockoutsTaken;
		[SyncVar]
		public int TotalHealed;
		[SyncVar]
		public int AbilitiesUsed;

		public PlayerStatsPacket Packet
		{
			get { return new PlayerStatsPacket(this); }
		}
	}

	public class PlayerStatsPacket : SerializablePacket
	{
		public int Wins;
		public int Losses;
		public int Draws;
		public bool Winner;
		public int DamageDealt;
		public int DamageTaken;
		public int KnockoutsDealt;
		public int KnockoutsTaken;
		public int TotalHealed;
		public int AbilitiesUsed;

		public PlayerStatsPacket(){}

		public PlayerStatsPacket(PlayerStats stats)
		{
			Wins = stats.Wins;
			Losses = stats.Losses;
			Draws = stats.Draws;
			Winner = stats.Winner;
			DamageDealt = stats.DamageDealt;
			DamageTaken = stats.DamageTaken;
			KnockoutsDealt = stats.KnockoutsDealt;
			KnockoutsTaken = stats.KnockoutsTaken;
			TotalHealed = stats.TotalHealed;
			AbilitiesUsed = stats.AbilitiesUsed;
		}

		public BsonDocument ToBsonDocument()
		{
			var retVal = new BsonDocument();

			retVal.Add("Wins", Wins);
			retVal.Add("Losses", Losses);
			retVal.Add("Draws", Draws);
			retVal.Add("Winner", Winner);
			retVal.Add("DamageDealt", DamageDealt);
			retVal.Add("DamageTaken", DamageTaken);
			retVal.Add("KnockoutsDealt", KnockoutsDealt);
			retVal.Add("KnockoutsTaken", KnockoutsTaken);
			retVal.Add("TotalHealed", TotalHealed);
			retVal.Add("AbilitiesUsed", AbilitiesUsed);

			return retVal;
		}

		public PlayerStatsPacket(BsonDocument stats)
		{
			Wins = stats["Wins"];
			Losses = stats["Losses"];
			Draws = stats["Draws"];
			Winner = stats["Winner"];
			DamageDealt = stats["DamageDealt"];
			DamageTaken = stats["DamageTaken"];
			KnockoutsDealt = stats["KnockoutsDealt"];
			KnockoutsTaken = stats["KnockoutsTaken"];
			TotalHealed = stats["TotalHealed"];
			AbilitiesUsed = stats["AbilitiesUsed"];
		}

		public override void FromBinaryReader(EndianBinaryReader reader)
		{
			Wins = reader.ReadInt32();
			Losses = reader.ReadInt32();
			Draws = reader.ReadInt32();
			Winner = reader.ReadBoolean();
			DamageDealt = reader.ReadInt32();
			DamageTaken = reader.ReadInt32();
			KnockoutsDealt = reader.ReadInt32();
			KnockoutsTaken = reader.ReadInt32();
			TotalHealed = reader.ReadInt32();
			AbilitiesUsed = reader.ReadInt32();
		}

		public override void ToBinaryWriter(EndianBinaryWriter writer)
		{
			writer.Write(Wins);
			writer.Write(Losses);
			writer.Write(Draws);
			writer.Write(Winner);
			writer.Write(DamageDealt);
			writer.Write(DamageTaken);
			writer.Write(KnockoutsDealt);
			writer.Write(KnockoutsTaken);
			writer.Write(TotalHealed);
			writer.Write(AbilitiesUsed);
		}

		public override string ToString()
		{
			return $"--PlayerStatsPacket--" +
					$"\nWins: {Wins}" +
					$"\nLosses: {Losses}" + 
					$"\nDraws: {Draws}" +
					$"\nWinner: {Winner}" +
					$"\nDamageDealt: {DamageDealt}" +
					$"\nDamageTaken: {DamageTaken}" +
					$"\nKnockoutsDealt: {KnockoutsDealt}" +
					$"\nKnockoutsTaken: {KnockoutsTaken}" +
					$"\nTotalHealed: {TotalHealed}" +
					$"\nAbilitiesUsed: {AbilitiesUsed}";
		}
	}
}
