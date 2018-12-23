using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PoroQueue
{
    public class Config
    {
        private static string LocalLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PoroQueue", "Config.json");

        public class SummonerConfig
        {
            public List<int> EnabledForURF = new List<int>();
            public List<int> EnabledForARAM = new List<int>();
            public List<int> EnabledForBlitz = new List<int>();

            public int URFIterator = 0;
            public int ARAMIterator = 0;
            public int BlitzIterator = 0;
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

        public string GetEntryIDForCurrentSummoner()
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

        public void AddURFIcon(int IconID)
        {
            string ID = GetEntryIDForCurrentSummoner();
            if (Entries[ID].EnabledForURF.Any(i => i == IconID))
                return;

            Entries[ID].EnabledForURF.Add(IconID);
            Save();
        }

        public void RemoveURFIcon(int IconID)
        {
            string ID = GetEntryIDForCurrentSummoner();
            Entries[ID].EnabledForURF.RemoveAll(i => i == IconID);
            Save();
        }

        public void AddARAMIcon(int IconID)
        {
            string ID = GetEntryIDForCurrentSummoner();
            if (Entries[ID].EnabledForARAM.Any(i => i == IconID))
                return;

            Entries[ID].EnabledForARAM.Add(IconID);
            Save();
        }

        public void RemoveARAMIcon(int IconID)
        {
            string ID = GetEntryIDForCurrentSummoner();
            Entries[ID].EnabledForARAM.RemoveAll(i => i == IconID);
            Save();
        }

        public void AddBlitzIcon(int IconID)
        {
            string ID = GetEntryIDForCurrentSummoner();
            if (Entries[ID].EnabledForBlitz.Any(i => i == IconID))
                return;

            Entries[ID].EnabledForBlitz.Add(IconID);
            Save();
        }

        public void RemoveBlitzIcon(int IconID)
        {
            string ID = GetEntryIDForCurrentSummoner();
            Entries[ID].EnabledForBlitz.RemoveAll(i => i == IconID);
            Save();
        }

        public bool IsEnabledForARAM(int IconID)
        {
            string ID = GetEntryIDForCurrentSummoner();
            return Entries[ID].EnabledForARAM.Any(i => i == IconID);
        }

        public bool IsEnabledForBlitz(int IconID)
        {
            string ID = GetEntryIDForCurrentSummoner();
            return Entries[ID].EnabledForBlitz.Any(i => i == IconID);
        }

        public bool IsEnabledForURF(int IconID)
        {
            string ID = GetEntryIDForCurrentSummoner();
            return Entries[ID].EnabledForURF.Any(i => i == IconID);
        }

        public void Save()
        {
            File.WriteAllText(LocalLocation, JsonConvert.SerializeObject(this));
        }
    }
}
