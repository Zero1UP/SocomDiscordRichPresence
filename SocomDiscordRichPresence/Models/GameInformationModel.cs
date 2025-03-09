
namespace SocomDiscordRichPresence.Models
{
    public class GameInformationModel
    {
        /// <summary>
        /// Hash of 16 bytes at the given memory address
        /// </summary>
        public string ?GameHash { get; set; }

        /// <summary>
        /// Resulting data is an int
        /// </summary>
        public int JoinedGameAddress { get; set; }

        /// <summary>
        /// Value retrieved from JoinGameAddress
        /// </summary>
        public int JounedGameValue { get; set; }
        /// <summary>
        /// Resulting data is a short
        /// </summary>
        public int JoinedGameLateAddress { get; set; }
        /// <summary>
        /// Value retrieved from JoinedGameLateAddress
        /// </summary>
        public short JoinedGameLateValue { get; set; }
        /// <summary>
        /// Resulting data is an int
        /// </summary>
        public int PlayerPointerAddress { get; set; }
        /// <summary>
        /// Value retrieved from PlayerPointerAddress
        /// </summary>
        public int PlayerPointerValue { get; set; }
        /// <summary>
        /// Resulting data is byte
        /// </summary>
        //public int GameEndedAddress { get; set; }
        /// <summary>
        /// Resulting data is an array of bytes (LEN of 12)
        /// </summary>
        public int MapListArrayAddress { get; set; }
        /// <summary>
        /// Resulting data is a int
        /// </summary>
        public int CurrentMapIndexAddress { get; set; }
        /// <summary>
        /// Holds the current MapInfo
        /// </summary>
        public MapDataModel? CurrentMap { get; set; }
        /// <summary>
        /// Resulting data is a short
        /// </summary>
        public int SealWinCounterAddress { get; set; }
        /// <summary>
        /// Value retrieved from SealWinCounterAddress
        /// </summary>
        public short SealWinCounterValue { get; set; } = -1;
        /// <summary>
        /// Resulting data is a short
        /// </summary>
        public int TerroristWinCounterAddress { get; set; }
        /// <summary>
        /// Value retrieved from TerroristWinCounterAddress
        /// </summary>
        public short TerroristWinCounterValue { get; set; } = -1;
        /// <summary>
        /// Resulting data is a string (Depending on the game there might need to be some offsets to deal with)
        /// </summary>
        public int RoomNameAddress { get; set; }
        /// <summary>
        /// Value retrieved from RoomNameAddress
        /// </summary>
        public string? RoomnameValue { get; set; } = "NOT IN A GAME";
        /// <summary>
        /// Resulting data is a short
        /// </summary>
        public int PlayerKillsOffset { get; set; }
        /// <summary>
        /// Value retrieved from PlayerKillsOffset
        /// </summary>
        public short PlayerKillsValue { get; set; }
        /// <summary>
        /// Resulting data is a short
        /// </summary>
        public int PlayerDeathsOffset { get; set; }
        /// <summary>
        /// Value retrieved from PlayerDeathsOffset
        /// </summary>
        public short PlayerDeathsValue { get; set; }
        /// <summary>
        /// Resulting data is a short
        /// </summary>
        public int SealPlayerCountAddress { get; set; }
        /// <summary>
        /// Value retrieved from SealPlayerCountAddress
        /// </summary>
        public short SealPlayerCountValue { get; set; }
        /// <summary>
        /// Resulting data is a short
        /// </summary>
        public int TerroristPlayerCountAddress { get; set; }
        /// <summary>
        /// Value retrieved from TerroristPlayerCountAddress
        /// </summary>
        public short TerroristPlayerCountValue { get; set; }
        /// <summary>
        /// Resulting data is a short
        /// </summary>
        public short TotalPlayersValue { get; set; }
        public int MaxPlayersAddress { get; set; }
        /// <summary>
        /// Value retrieved from MaxPlayersAddress
        /// </summary>
        public short MaxPlayersValue { get; set; }

        public List<MapDataModel>? MapList { get; set; } = new List<MapDataModel>();
        public short CurrentMapIndex { get; set; } = 0;

        public byte[] GameLogo { get; set; }
    }
}
