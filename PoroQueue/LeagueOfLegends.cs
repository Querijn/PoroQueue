﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Linq;
using WebSocketSharp;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace PoroQueue
{
    public static class LeagueOfLegends
    {
        public static event EventHandler Started;
        public static event EventHandler Stopped;
        public static event EventHandler LoggedIn;
        public static event EventHandler LoggedOut;
        public static event EventHandler IconChanged;

        public static bool IsActive { get; private set; }
        public static bool IsLoggedIn { get; private set; }
        public static Summoner CurrentSummoner { get; private set; }
        public static string APIDomain { get; private set; }
        public static string LatestVersion { get; private set; }

        private static string DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PoroQueue");
        private static FileSystemWatcher Watcher = new FileSystemWatcher();
        private static WebSocket Connection = null;
        private static int ForcedPoroIcon = 0;

        private const string SummonerIconChangedEvent = "OnJsonApiEvent_lol-summoner_v1_current-summoner";
        private const string LoggedInEvent = "OnJsonApiEvent_lol-login_v1_login-data-packet";
        private const string QueueUpEvent = "OnJsonApiEvent_lol-lobby-team-builder_v1_lobby";

#if DEBUG_EVENTS
        class DebugEventHelper
        {
            public Dictionary<string, string> events;
        };
#endif

        static LeagueOfLegends()
        {
            APIDomain = null;
            LatestVersion = "8.24.1";

            if (!Directory.Exists(DataDirectory))
                Directory.CreateDirectory(DataDirectory);

            if (LeaguePath != null)
            {
                WaitForLockFile();
                return;
            }

            var LeagueWatcher = new System.Timers.Timer(1000);
            LeagueWatcher.Elapsed += (a, b) =>
            {
                if (LeaguePath != null)
                    WaitForLockFile();
            };
            LeagueWatcher.Enabled = true;
        }


        private static void WaitForLockFile()
        {
            Watcher.Path = LeaguePath;
            Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastAccess;
            Watcher.Filter = "lockfile";

            FileSystemEventHandler OnLockFileChanged = (o, e) =>
            {
                if (IsActive || LockFile.Equals(""))
                    return;

                Init();
            };

            Watcher.Created += OnLockFileChanged;
            Watcher.Changed += OnLockFileChanged;
            Watcher.Deleted += (o, e) =>
            {
                IsActive = false;
                Stopped?.Invoke(null, EventArgs.Empty);

                Connection = null;
                CurrentSummoner = null;
                LoggedOut?.Invoke(null, EventArgs.Empty);
            };
            Watcher.EnableRaisingEvents = true;

            if (LockFileLocation != null && File.Exists(LockFileLocation))
            {
                Init();
            }
        }

        private static async void Init()
        {
            IsActive = true;
            
            var Parts = LockFile.Split(':');

            ushort Port = ushort.Parse(Parts[2]);
            var Password = Parts[3];
            var Protocol = Parts[4];

            APIDomain = String.Format("{0}://127.0.0.1:{1}", Protocol, Port);
            Request.SetUserData("riot", Password);

#if DEBUG_EVENTS
            var AllEventsText = await Request.Get(APIDomain + "/help");
            File.WriteAllText("events.json", AllEventsText);
#endif

            var Versions = await JSONRequest.Get<string[]>("http://ddragon.leagueoflegends.com/api/versions.json");
            LatestVersion = Versions[0];

            Started?.Invoke(null, EventArgs.Empty);

            Connection = new WebSocket("wss://127.0.0.1:" + Port + "/", "wamp");
            Connection.SetCredentials("riot", Password, true);
            Connection.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            Connection.OnMessage += OnWebsocketMessage;
            Connection.Connect();

#if !DEBUG_EVENTS
            Connection.Send("[5,\"" + SummonerIconChangedEvent + "\"]");
            Connection.Send("[5,\"" + QueueUpEvent + "\"]");
#else
            var HelpDocument = JsonConvert.DeserializeObject<DebugEventHelper>(AllEventsText);
            foreach (var EventName in HelpDocument.events)
            {
                var Event = EventName.Key;
                if (Event == "OnJsonApiEvent")
                    continue;
                Connection.Send("[5,\"" + Event + "\"]");
            }
#endif

            try
            {
                CurrentSummoner = await Summoner.GetCurrent();
                IsLoggedIn = true;
                LoggedIn?.Invoke(null, EventArgs.Empty);
            }

            // We aren't logged in!
            catch (System.Net.Http.HttpRequestException e)
            {
#if !DEBUG_EVENTS
                Connection.Send("[5,\"" + LoggedInEvent + "\"]");
#endif
            }
        }

        private static async void OnWebsocketMessage(object sender, MessageEventArgs e)
        {
            var Messages = JsonConvert.DeserializeObject<object[]>(e.Data);

            int MessageType = 0;
            if (!int.TryParse(Messages[0].ToString(), out MessageType) || MessageType != 8)
                return;

            var EventName = Messages[1].ToString();
#if DEBUG_EVENTS
            Debug.WriteLine("Received an event: " + EventName);
#endif

            switch (EventName)
            {
                case LoggedInEvent:
                    while (!IsLoggedIn)
                    {
                        try
                        {
                            CurrentSummoner = await Summoner.GetCurrent();
                            IsLoggedIn = true;
                            LoggedIn?.Invoke(null, EventArgs.Empty);
                            break;
                        }

                        // We aren't logged in!
                        catch (System.Net.Http.HttpRequestException)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(5));
                        }
                    }
                    break;

                case SummonerIconChangedEvent:
                    CurrentSummoner = await Summoner.GetCurrent();

                    if (CurrentSummoner.profileIconId != ForcedPoroIcon)
                        IconChanged?.Invoke(null, EventArgs.Empty);
                    else if (ForcedPoroIcon != 0)
                        Icon.Set(ForcedPoroIcon);
                    break;

                case QueueUpEvent:
                    var Message = JsonConvert.DeserializeObject<LobbyMatchmakingEvent>(Messages[2].ToString());
                    ForcedPoroIcon = Icon.SetToPoro();
                    break;
            }
        }

        private static string LockFileLocation
        {
            get
            {
                return LeaguePath != null ? Path.Combine(LeaguePath, "lockfile") : null;
            }
        }

        private static string LockFileCache = null;
        private static string LockFile
        {
            get
            {
                if (LockFileCache != null)
                    return LockFileCache;

                var TempFile = LockFileLocation + "-temp";
                if (File.Exists(TempFile))
                    File.Delete(TempFile);

                File.Copy(LockFileLocation, TempFile);
                LockFileCache = File.ReadAllText(TempFile, Encoding.UTF8);
                File.Delete(TempFile);
                return LockFileCache;
            }
        }

        private static string LeagueClientCache = null;
        public static string LeagueClient
        {
            get
            {
                if (LeagueClientCache != null)
                    return LeagueClientCache;

                string ConfigPath = System.IO.Path.Combine(DataDirectory, "LCUPath.txt");
                string LocationFromConfig = File.Exists(ConfigPath) ? File.ReadAllText(ConfigPath) : "C:/Riot Games/League of Legends/LeagueClient.exe";

                if (IsValidClientFolder(LocationFromConfig))
                {
                    LeagueClientCache = LocationFromConfig;
                    return LocationFromConfig;
                }

                var RunningPath = GetLCUPathWithRunningLeagueClient();
                if (RunningPath == null)
                    return null;

                RunningPath = Path.Combine(RunningPath, "LeagueClient.exe");
                if (!IsValidClientFolder(RunningPath))
                    return null;

                File.WriteAllText(ConfigPath, RunningPath);
                LeagueClientCache = RunningPath;
                return RunningPath;
            }
        }

        private static string LeaguePathCache = null;
        public static string LeaguePath
        {
            get
            {
                if (LeaguePathCache != null)
                    return LeaguePathCache;

                LeaguePathCache = LeagueClient != null ? Path.GetDirectoryName(LeagueClient) : null;
                return LeaguePathCache;
            }
        }

        public static string GetLCUPathWithRunningLeagueClient()
        {
            var LeagueProcesses = Process.GetProcesses().Where(p => p.ProcessName.Contains("League"));
            foreach (var Process in LeagueProcesses)
            {
                try
                {
                    string CommandLine = Process.GetCommandLine();
                    var InstallDirIndex = CommandLine.IndexOf("--install-directory");
                    if (InstallDirIndex == -1)
                        continue;

                    // Index started at "--league-directory=", but we now go to the start of the directory in the string
                    InstallDirIndex = CommandLine.IndexOf("=", InstallDirIndex) + 1;

                    // Take everything until the " behind the directory
                    return CommandLine.Substring(InstallDirIndex, CommandLine.IndexOf("\"", InstallDirIndex) - InstallDirIndex);
                }
                catch (Win32Exception ex) when ((uint)ex.ErrorCode == 0x80004005)
                {
                    // Intentionally empty.
                }
            }
            return null;
        }
        
        private static bool IsValidClientFolder(string Folder)
        {
            try
            {
                if (String.IsNullOrEmpty(Folder))
                    return false;

                Folder = Path.GetDirectoryName(Folder);
                return File.Exists(Folder + "/LeagueClient.exe") && Directory.Exists(Folder + "/Config") && Directory.Exists(Folder + "/Logs");
            }
            catch
            {
                return false;
            }
        }
    }
}