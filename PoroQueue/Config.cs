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

        public List<int> EnabledForURF = new List<int>();
        public List<int> EnabledForARAM = new List<int>();
        public List<int> EnabledForBlitz = new List<int>();

        public int URFIterator = 0;
        public int ARAMIterator = 0;
        public int BlitzIterator = 0;

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

        public void AddURFIcon(int IconID)
        {
            if (EnabledForURF.Any(i => i == IconID))
                return;

            EnabledForURF.Add(IconID);
            Save();
        }

        public void RemoveURFIcon(int IconID)
        {
            EnabledForURF.RemoveAll(i => i == IconID);
            Save();
        }

        public void AddARAMIcon(int IconID)
        {
            if (EnabledForARAM.Any(i => i == IconID))
                return;

            EnabledForARAM.Add(IconID);
            Save();
        }

        public void RemoveARAMIcon(int IconID)
        {
            EnabledForARAM.RemoveAll(i => i == IconID);
            Save();
        }

        public void AddBlitzIcon(int IconID)
        {
            if (EnabledForBlitz.Any(i => i == IconID))
                return;

            EnabledForBlitz.Add(IconID);
            Save();
        }

        public void RemoveBlitzIcon(int IconID)
        {
            EnabledForBlitz.RemoveAll(i => i == IconID);
            Save();
        }

        public bool IsEnabledForARAM(int IconID)
        {
            return EnabledForARAM.Any(i => i == IconID);
        }

        public bool IsEnabledForBlitz(int IconID)
        {
            return EnabledForBlitz.Any(i => i == IconID);
        }

        public bool IsEnabledForURF(int IconID)
        {
            return EnabledForURF.Any(i => i == IconID);
        }

        public void Save()
        {
            File.WriteAllText(LocalLocation, JsonConvert.SerializeObject(this));
        }
    }
}
