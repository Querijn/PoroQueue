using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PoroQueue
{
    public class Config
    {
        private const string WriteURL = "https://querijn.codes/poro_queue/write/";
        private const string ReadURL = "https://querijn.codes/poro_queue/read/";
        private const string DeleteURL = "https://querijn.codes/poro_queue/delete/";

        private static string LocalLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PoroQueue", "Config.json");

        public class SummonerConfig
        {
            public List<int> EnabledForURF = new List<int>();
            public List<int> EnabledForARAM = new List<int>();
            public List<int> EnabledForBlitz = new List<int>();

            public int URFIterator = 0;
            public int ARAMIterator = 0;
            public int BlitzIterator = 0;

            public bool UnderstandsServerSync = false;
            public bool WantsServerSync = false;
        };
        public Dictionary<string, SummonerConfig> Entries = new Dictionary<string, SummonerConfig>();
        
        private static Config Instance = null;
        public static Config Current
        {
            get
            {
                if (Instance == null)
                {
                    if (File.Exists(LocalLocation))
                        Instance = JsonConvert.DeserializeObject<Config>(File.ReadAllText(LocalLocation));
                    else
                    {
                        Instance = new Config();
                        Instance.Save();
                    }
                }

                return Instance;
            }
        }

        public SummonerConfig Summoner
        {
            get
            {
                return Entries[GetEntryIDForCurrentSummonerSync()];
            }
        }

        private List<string> LookedUpSummoners = new List<string>();
        public async Task<string> GetEntryIDForCurrentSummoner()
        {
            if (LeagueOfLegends.CurrentSummoner == null)
                return "Default";

            if (LookedUpSummoners.Any(s => s == LeagueOfLegends.CurrentSummoner.puuid) == false)
            {
                SummonerConfig Result;
                try
                {
                    var URL = $"{ReadURL}?p={LeagueOfLegends.CurrentSummoner.puuid}";
                    Result = await JSONRequest.Get<SummonerConfig>(URL);
                    
                    if (Result != null)
                    {
                        if (!Entries.Any(e => e.Key == LeagueOfLegends.CurrentSummoner.puuid))
                            Entries.Add(LeagueOfLegends.CurrentSummoner.puuid, Result);
                        else Entries[LeagueOfLegends.CurrentSummoner.puuid] = Result;
                    }
                    
                }
                catch (System.Net.Http.HttpRequestException e)
                {
                    int i = 0; i++;
                }

                LookedUpSummoners.Add(LeagueOfLegends.CurrentSummoner.puuid);
            }

            if (!Entries.Any(e => e.Key == LeagueOfLegends.CurrentSummoner.puuid))
            {
                Entries.Add(LeagueOfLegends.CurrentSummoner.puuid, new SummonerConfig());
                Save();
            }

            return LeagueOfLegends.CurrentSummoner.puuid;
        }

        public string GetEntryIDForCurrentSummonerSync()
        {
            if (LeagueOfLegends.CurrentSummoner == null)
                return "Default";

            if (!Entries.Any(e => e.Key == LeagueOfLegends.CurrentSummoner.puuid))
            {
                Entries.Add(LeagueOfLegends.CurrentSummoner.puuid, new SummonerConfig());
                Save();
            }

            return LeagueOfLegends.CurrentSummoner.puuid;
        }

        public async void AddURFIcon(int IconID)
        {
            string ID = await GetEntryIDForCurrentSummoner();
            Entries[ID].EnabledForURF.Add(IconID);
            Save();
        }

        public async void RemoveURFIcon(int IconID)
        {
            string ID = await GetEntryIDForCurrentSummoner();
            Entries[ID].EnabledForURF.RemoveAll(i => i == IconID);
            Save();
        }

        public async void AddARAMIcon(int IconID)
        {
            string ID = await GetEntryIDForCurrentSummoner();
            Entries[ID].EnabledForARAM.Add(IconID);
            Save();
        }

        public async void RemoveARAMIcon(int IconID)
        {
            string ID = await GetEntryIDForCurrentSummoner();
            Entries[ID].EnabledForARAM.RemoveAll(i => i == IconID);
            Save();
        }

        public async void AddBlitzIcon(int IconID)
        {
            string ID = await GetEntryIDForCurrentSummoner();
            Entries[ID].EnabledForBlitz.Add(IconID);
            Save();
        }

        public async void RemoveBlitzIcon(int IconID)
        {
            string ID = await GetEntryIDForCurrentSummoner();
            Entries[ID].EnabledForBlitz.RemoveAll(i => i == IconID);
            Save();
        }

        public async Task<bool> IsEnabledForARAM(int IconID)
        {
            string ID = await GetEntryIDForCurrentSummoner();
            return Entries[ID].EnabledForARAM.Any(i => i == IconID);
        }

        public async Task<bool> IsEnabledForBlitz(int IconID)
        {
            string ID = await GetEntryIDForCurrentSummoner();
            return Entries[ID].EnabledForBlitz.Any(i => i == IconID);
        }

        public async Task<bool> IsEnabledForURF(int IconID)
        {
            string ID = await GetEntryIDForCurrentSummoner();
            return Entries[ID].EnabledForURF.Any(i => i == IconID);
        }

        public async void Save()
        {
            foreach (var Entry in Entries)
            {
                Entry.Value.EnabledForURF = Entry.Value.EnabledForURF.Distinct().ToList();
                Entry.Value.EnabledForARAM = Entry.Value.EnabledForARAM.Distinct().ToList();
                Entry.Value.EnabledForBlitz = Entry.Value.EnabledForBlitz.Distinct().ToList();

                Entry.Value.EnabledForURF.Sort();
                Entry.Value.EnabledForARAM.Sort();
                Entry.Value.EnabledForBlitz.Sort();
            }

            File.WriteAllText(LocalLocation, JsonConvert.SerializeObject(this));

            if (Summoner != null && Summoner.UnderstandsServerSync)
            {
                var S = LeagueOfLegends.CurrentSummoner;
                if (!Summoner.WantsServerSync)
                    await JSONRequest.Get<bool>($"{DeleteURL}?p={S.puuid}");

                var IconsARAM = string.Join(",", Summoner.EnabledForARAM);
                var IconsBlitz = string.Join(",", Summoner.EnabledForBlitz);
                var IconsURF = string.Join(",", Summoner.EnabledForURF);
                await Request.Get($"{WriteURL}?p={S.puuid}" +
                    $"&a={IconsARAM}&ai={Summoner.ARAMIterator}" +
                    $"&b={IconsBlitz}&bi={Summoner.BlitzIterator}" +
                    $"&u={IconsURF}&ui={Summoner.URFIterator}");
            }
            
        }
    }
}
