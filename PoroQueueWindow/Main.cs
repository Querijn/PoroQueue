using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PoroQueueWindow
{
    public partial class Main : Form
    {
        class IconSet
        {
            public Panel Container;
            public PictureBox Icon;
            public CheckBox EnableARAM;
            public CheckBox EnableBlitz;
            public CheckBox EnableUrf;
            public int IconID;
        };
        Dictionary<int, IconSet> IconElements = new Dictionary<int, IconSet>();
        Image DefaultImage;
        SynchronizationContext UIThread;

        public Main()
        {
            InitializeComponent();
            DefaultImage = DefaultIcon.Image;
            UIThread = SynchronizationContext.Current;

            if (PoroQueue.LeagueOfLegends.IsLoggedIn)
                InitLeagueData();
            else PoroQueue.LeagueOfLegends.LoggedIn += (object sender, EventArgs e) =>
                UIThread.Post(o => InitLeagueData(), null);

            PoroQueue.LeagueOfLegends.LoggedOut += (object sender, EventArgs e) =>
                UIThread.Post(o => RemoveLeagueData(), null);

            PoroQueue.LeagueOfLegends.GameModeUpdated += (object sender, EventArgs e) =>
                UIThread.Post(o => UpdateLeagueStatus(), null);
        }

        private async Task SetCurrentIcon()
        {
            await Task.Run(() => PoroQueue.Icon.LoadCurrentIntoPictureBox(DefaultIcon));
        }

        private async void InitLeagueData()
        {
            UpdateLeagueStatus();

            await SetCurrentIcon();
            await SetupIcons();

            PoroQueue.LeagueOfLegends.IconChanged += (s,e) => UIThread.Post(async o => await SetCurrentIcon(), null);

            IconGroup.Resize += async (a, b) => await SetupIcons();
        }

        private void RemoveLeagueData()
        {
            UpdateLeagueStatus();
            DefaultIcon.Image = DefaultImage;
            IconGroup.Controls.Clear();
        }

        private void UpdateLeagueStatus()
        {
            if (PoroQueue.LeagueOfLegends.IsLoggedIn)
            {
                switch (PoroQueue.LeagueOfLegends.CurrentGameMode)
                {
                    case PoroQueue.LeagueOfLegends.GameMode.Unknown:
                        LeagueStatus.Text = "You are logged in!";
                        break;

                    case PoroQueue.LeagueOfLegends.GameMode.ARAM:
                        LeagueStatus.Text = "You're detected as queueing up for ARAM.";
                        break;

                    case PoroQueue.LeagueOfLegends.GameMode.NexusBlitz:
                        LeagueStatus.Text = "You're detected as queueing up for Nexus Blitz.";
                        break;

                    case PoroQueue.LeagueOfLegends.GameMode.URF:
                        LeagueStatus.Text = "You're detected as queueing up for URF.";
                        break;
                }

            }
            else if (PoroQueue.LeagueOfLegends.IsActive)
                LeagueStatus.Text = "League of Legends is active!";
            else
                LeagueStatus.Text = "Waiting for League of Legends..";
        }

        private async Task SetupIcons()
        {
            try
            {
                // Start requests for our icons and the list of icons with effects
                var IconsTask = PoroQueue.Icon.GetOwnedIconIDs();
                var EffectIconsTask = PoroQueue.Icon.GetEffectIcons();

                // Set initial positions
                int x = IconGroup.Margin.Left + IconGroup.Padding.Left + 10, y = IconGroup.Margin.Top + IconGroup.Padding.Top + 16;
                int InitialX = x, InitialY = y;
                int Width = IconGroup.Width - (IconGroup.Margin.Right + IconGroup.Padding.Right);

                await Task.WhenAll(new Task[] { IconsTask, EffectIconsTask });
                var Icons = IconsTask.Result;
                var EffectIcons = EffectIconsTask.Result;

                foreach (var EffectIcon in EffectIcons)
                {
                    var Icon = Icons.FirstOrDefault(i => i == EffectIcon.ID);
                    if (Icon == 0)
                        continue;

                    var ElementPair = IconElements.FirstOrDefault((e) => e.Key == Icon);
                    IconSet Element = ElementPair.Value;
                    if (ElementPair.Value == null)
                    {
                        Element = new IconSet();

                        Element.IconID = Icon;

                        Element.Container = new Panel
                        {
                            Name = "IconContainer" + Icon,
                            Size = new Size(100, 100),
                            Location = new Point(x, y),
                            Margin = new Padding(0),
                            Padding = new Padding(0),
                        };

                        Element.Icon = new PictureBox
                        {
                            Name = "OwnedIcon " + Icon,
                            Size = new Size(100, 100),
                            SizeMode = PictureBoxSizeMode.StretchImage,
                        };

                        int CheckboxY = 0;
                        if (EffectIcon.GameModes.Contains(PoroQueue.LeagueOfLegends.GameMode.ARAM))
                        {
                            Element.EnableARAM = new CheckBox
                            {
                                Name = "ARAMEnabled " + Icon,
                                Text = "ARAM",
                                Location = new Point(0, 103 + CheckboxY),
                                Checked = PoroQueue.Config.Current.IsEnabledForARAM(Element.IconID)
                            };

                            Element.EnableARAM.CheckedChanged += (object s, EventArgs e) =>
                            {
                                if (Element.EnableARAM.Checked)
                                    PoroQueue.Config.Current.AddARAMIcon(Element.IconID);
                                else PoroQueue.Config.Current.RemoveARAMIcon(Element.IconID);
                            };

                            Element.Container.Controls.Add(Element.EnableARAM);
                            CheckboxY += Element.EnableARAM.Height - 5;
                            Element.Container.Height += Element.EnableARAM.Height;
                        }

                        if (EffectIcon.GameModes.Contains(PoroQueue.LeagueOfLegends.GameMode.NexusBlitz))
                        {
                            Element.EnableBlitz = new CheckBox
                            {
                                Name = "BlitzEnabled " + Icon,
                                Text = "Nexus Blitz",
                                Location = new Point(0, 103 + CheckboxY),
                                Checked = PoroQueue.Config.Current.IsEnabledForBlitz(Element.IconID)
                            };

                            Element.EnableBlitz.CheckedChanged += (object s, EventArgs e) =>
                            {
                                if (Element.EnableBlitz.Checked)
                                    PoroQueue.Config.Current.AddBlitzIcon(Element.IconID);
                                else PoroQueue.Config.Current.RemoveBlitzIcon(Element.IconID);
                            };

                            Element.Container.Controls.Add(Element.EnableBlitz);
                            CheckboxY += Element.EnableBlitz.Height - 5;
                            Element.Container.Height += Element.EnableBlitz.Height;
                        }

                        if (EffectIcon.GameModes.Contains(PoroQueue.LeagueOfLegends.GameMode.URF))
                        {
                            Element.EnableUrf = new CheckBox
                            {
                                Name = "URFEnabled " + Icon,
                                Text = "URF",
                                Location = new Point(0, 103 + CheckboxY),
                                Checked = PoroQueue.Config.Current.IsEnabledForURF(Element.IconID)
                            };

                            Element.EnableUrf.CheckedChanged += (object s, EventArgs e) =>
                            {
                                if (Element.EnableUrf.Checked)
                                    PoroQueue.Config.Current.AddURFIcon(Element.IconID);
                                else PoroQueue.Config.Current.RemoveURFIcon(Element.IconID);
                            };

                            Element.Container.Controls.Add(Element.EnableUrf);
                            CheckboxY += Element.EnableUrf.Height - 5;
                            Element.Container.Height += Element.EnableUrf.Height;
                        }

                        Element.Container.Controls.Add(Element.Icon);
                        IconGroup.Controls.Add(Element.Container);

                        PoroQueue.Icon.LoadIntoPictureBox(Icon, Element.Icon);
                        IconElements[Icon] = Element;
                    }
                    else Element.Container.Location = new Point(x, y);

                    x += Element.Container.Size.Width + 10;
                    if (x + 100 > Width)
                    {
                        x = InitialX;
                        y += Element.Container.Size.Height + 20;

                        if (Height < y + 230)
                            Height = y + 230;
                    }
                }
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                // Debug.Break();
            }
        }
    }
}
