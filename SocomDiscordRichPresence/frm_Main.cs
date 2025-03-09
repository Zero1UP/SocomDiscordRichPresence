using DiscordRPC;
using MemoryIO.Pcsx2;
using SocomDiscordRichPresence.Models;
using System.Linq;
using System.Security.Cryptography;

namespace SocomDiscordRichPresence
{
    public partial class frm_Main : Form
    {
        Pcsx2MemoryIO m = null;
        public DiscordRpcClient client;
        bool gameStarted = false;

        private static RichPresence presence = new RichPresence()
        {
            Details = "Not currently in a game.",
            State = "",
            Assets = new Assets()
            {
                LargeImageKey = "default",
                LargeImageText = "Not in a room.",
                SmallImageKey = "default"
            }
        };

        public frm_Main()
        {
            InitializeComponent();
            m = new Pcsx2MemoryIO();
            client = new DiscordRpcClient("662019219030016001");
            client.Initialize();
            client.SetPresence(presence);
        }

        private void tmr_GameScan_Tick(object sender, EventArgs e)
        {
            try
            {
                if(m.Pcsx2Running)
                {
                    //Figure out what game is being played
                    string gameIndentifier = GenerateMD5Hash(m.ReadData(0x0040CC60, 16));
                    GameInformationModel gameInfo = Globals.GameList.FirstOrDefault(g => g.GameHash == gameIndentifier);

                    List<byte> mapIdList = new List<byte>() { 0x00 };
                    if (gameInfo != null)
                    {
                        pct_CurrentGame.Image = GetBitmap(gameInfo.GameLogo);
                        this.Text = "Game Detected";
                        // When you are a late joiner to a game (SOCOM II at least) gbMyClientIsJoined doesn't get set. There seems to be
                        // a different flow for joining a game depending if the game is in lobby or actually playing. The variable does get set after
                        // your player object is created during the loading screen though. Still weird.
                        if (m.Read<int>(gameInfo.JoinedGameAddress) == 1 || m.Read<short>(gameInfo.JoinedGameLateAddress) == 1)
                        {
                            
                            // The biggest differences between all these games comes down to processing some of the information
                            // like room name and map information.
                            switch (gameInfo.GameHash)
                            {
                                case "23E3C7B5CF948896368F4003DE8C4715": // SOCOM 1 Disk 1 - 4
                                case "DBD2D4D90EA1ED1F5A382050CE969CCF": // SOCOM 1 Disk 8
                                case "610767D2D8D253C38CA2595F87E178C5": // SOCOM 1 Disks 10 - 11
                                    // This fucking game I swear
                                    // Player Valve will only get set if you create the room. Joining one will not set it and neither will getting host.
                                    // I am pretty certain this is what causes rooms to go over the max player count because it never gets set outside of creating
                                    if(gameInfo.GameHash == "23E3C7B5CF948896368F4003DE8C4715")
                                    {
                                        m.Write(0x0034062c, 0x00000000);
                                    }
                                    
                                    ProcessSocom1Data(ref gameInfo);
                                    break;

                                case "5C47974E99064D3BC9D07D90FEC2D1E2": // SOCOM II
                                    ProcessSocom2Data(ref gameInfo);
                                    break;
                                case "2C7B35F81D54D3E45E072C1ACA6D759C": // SOCOM 3
                                case "7F509DFA68DAF0CC87C7096C24BC29E4": // SOCOM CA
                                    ProcessSocom3CaData(ref gameInfo);
                                    break;
                            }
                            gameInfo.MaxPlayersValue = m.Read<short>(gameInfo.MaxPlayersAddress);
                            if ((m.ReadArray<byte>(gameInfo.PlayerPointerAddress, 4) != null) && (!m.ReadArray<byte>(gameInfo.PlayerPointerAddress, 4).SequenceEqual(new byte[] { 0, 0, 0, 0 })))
                            {
                                // In game data
                                int playerDataLocationAddress = m.Read<int>(gameInfo.PlayerPointerAddress);
                                gameInfo.PlayerKillsValue = m.Read<short>(playerDataLocationAddress + gameInfo.PlayerKillsOffset);
                                gameInfo.PlayerDeathsValue = m.Read<short>(playerDataLocationAddress + gameInfo.PlayerDeathsOffset);
                                gameInfo.SealWinCounterValue = m.Read<byte>(gameInfo.SealWinCounterAddress);
                                gameInfo.TerroristWinCounterValue = m.Read<byte>(gameInfo.TerroristWinCounterAddress);

                                if (!gameStarted)
                                {
                                    presence.Timestamps = new Timestamps()
                                    {
                                        Start = DateTime.UtcNow

                                    };

                                    gameStarted = true;
                                }
                            }
                            else
                            {
                                // Reset some data
                                gameInfo.PlayerKillsValue = 0;
                                gameInfo.PlayerDeathsValue = 0;
                                gameInfo.SealWinCounterValue = 0;
                                gameInfo.TerroristWinCounterValue = 0;
                                gameInfo.CurrentMap = gameInfo.MapList.FirstOrDefault(m => m.MapId == 0x00);
                                if (gameInfo.GameHash == "2C7B35F81D54D3E45E072C1ACA6D759C")
                                {
                                    gameInfo.CurrentMap.AltDiscordKey = "s3_logo";
                                    gameInfo.CurrentMap.AltText = "SOCOM 3";
                                }
                               
                            }
                                setPresence(gameInfo);
                        }
                        else
                        {
                            resetPresence();
                        }
                    }
                    else
                    {
                        resetPresence();
                    }
                }       
            }
            catch (Exception)
            {
                //This only happens if the game isn't actually running but pcsx2 is.
                resetPresence();
            }
        }
        private void ProcessSocom1Data(ref GameInformationModel gameInfo)
        {
            string[] roomName = null;
            List<byte> mapIdList = new List<byte>();
            int currentMapIndexValue = 0;

            int roomNameAddress = m.Read<int>(gameInfo.RoomNameAddress);
            roomName = m.ReadString(roomNameAddress).Split(new string[] { "$~" }, StringSplitOptions.None);
            gameInfo.RoomnameValue = roomName[0];
            gameInfo.TotalPlayersValue = m.Read<short>(gameInfo.SealPlayerCountAddress);
            mapIdList = m.ReadArray<byte>(gameInfo.MapListArrayAddress, 12).ToList();
            currentMapIndexValue = m.Read<byte>(gameInfo.CurrentMapIndexAddress);
            gameInfo.CurrentMap = gameInfo.MapList.FirstOrDefault(m => m.MapId == mapIdList[currentMapIndexValue]);
        }
        private void ProcessSocom2Data (ref GameInformationModel gameInfo)
        {
            string[] roomName = null;
            List<byte> mapIdList = new List<byte>();
            int currentMapIndexValue = 0;

            int roomNameAddress = m.Read<int>(gameInfo.RoomNameAddress);
            roomNameAddress = m.Read<int>(roomNameAddress + 4);
            roomName = m.ReadString(roomNameAddress).Split(new string[] { "$~" }, StringSplitOptions.None);
            gameInfo.RoomnameValue = roomName[0];
            gameInfo.TotalPlayersValue = (short)(m.Read<short>(gameInfo.SealPlayerCountAddress) + m.Read<short>(gameInfo.TerroristPlayerCountAddress));
            mapIdList = m.ReadArray<byte>(gameInfo.MapListArrayAddress, 12).ToList();
            currentMapIndexValue = m.Read<byte>(gameInfo.CurrentMapIndexAddress);
            gameInfo.CurrentMap = gameInfo.MapList.FirstOrDefault(m => m.MapId == mapIdList[currentMapIndexValue]);
        }

        private void ProcessSocom3CaData(ref GameInformationModel gameInfo)
        {
            int mapId = 0;
            gameInfo.RoomnameValue = m.ReadString(gameInfo.RoomNameAddress);

            gameInfo.TotalPlayersValue = (short)(m.Read<short>(gameInfo.SealPlayerCountAddress) + m.Read<short>(gameInfo.TerroristPlayerCountAddress));
            mapId = m.Read<byte>(gameInfo.MapListArrayAddress);
            gameInfo.CurrentMap = gameInfo.MapList.FirstOrDefault(m => m.MapId == mapId)
                      ?? gameInfo.MapList.FirstOrDefault(m => m.MapId == 0x00);

            if (gameInfo.GameHash == "2C7B35F81D54D3E45E072C1ACA6D759C")
            {
                gameInfo.CurrentMap.AltDiscordKey = "s3_logo";
                gameInfo.CurrentMap.AltText = "SOCOM 3";
            }         
        }

        private void resetPresence()
        {
            presence.Timestamps = null;
            setPresence(new GameInformationModel());
            gameStarted = false;

        }
        private void setPresence(GameInformationModel gameInfo)
        {
            string details = $"In-Game: {gameInfo.RoomnameValue}";
            string state = $"SW: {gameInfo.SealWinCounterValue} - {gameInfo.TerroristWinCounterValue} TW | K/D {gameInfo.PlayerKillsValue}/{gameInfo.PlayerDeathsValue}";
            presence.Assets = new Assets();
            presence.Party = new Party()
            {
                ID = new Guid().ToString(),
                Size = gameInfo.TotalPlayersValue,
                Max = gameInfo.MaxPlayersValue,
                Privacy = Party.PrivacySetting.Private

            };
            presence.Details = details;
            presence.HasParty();

            if (gameInfo.SealWinCounterValue == -1 && gameInfo.TerroristWinCounterValue == -1)
            {
                client.SetPresence(presence);
                return;
            }

            presence.Assets.LargeImageKey = gameInfo.CurrentMap.DiscordKey;
            presence.Assets.SmallImageKey = gameInfo.CurrentMap.AltDiscordKey;
            presence.Assets.LargeImageText = gameInfo.CurrentMap.MapName;
            presence.Assets.SmallImageText = gameInfo.CurrentMap.AltText;


            presence.State = state;

            client.SetPresence(presence);
        }

        private static string GenerateMD5Hash(byte[] data)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashedBytes = md5.ComputeHash(data);

                return BitConverter.ToString(hashedBytes).Replace("-", "");
            }
        }

        public static Bitmap GetBitmap(byte[] imageData)
        {
            using var ms = new MemoryStream(imageData);
            return new Bitmap(ms);
        }
    }
}
