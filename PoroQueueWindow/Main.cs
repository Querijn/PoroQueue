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
        }

        private async void InitLeagueData()
        {
            UpdateLeagueStatus();

            await Task.Run(() => PoroQueue.Icon.LoadCurrentIntoPictureBox(DefaultIcon));
            await SetupIcons();

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
                LeagueStatus.Text = "You are logged in!";
            else if (PoroQueue.LeagueOfLegends.IsActive)
                LeagueStatus.Text = "League of Legends is active!";
            else
                LeagueStatus.Text = "Waiting for League of Legends..";
        }

        private async Task SetupIcons()
        {
            try
            {
                var Icons = await PoroQueue.Icon.GetOwnedIconIDs();
                var AllowedIcons = await PoroQueue.Icon.GetAllowedIcons();
                int x = IconGroup.Margin.Left + IconGroup.Padding.Left + 10, y = IconGroup.Margin.Top + IconGroup.Padding.Top + 16;
                int InitialX = x, InitialY = y;
                int Width = IconGroup.Width - (IconGroup.Margin.Right + IconGroup.Padding.Right);
                foreach (var AllowedIcon in AllowedIcons)
                {
                    var Icon = Icons.FirstOrDefault(i => i == AllowedIcon.ID);
                    if (Icon == 0)
                        continue;

                    var ElementPair = IconElements.FirstOrDefault((e) => e.Key == Icon);
                    IconSet Element = ElementPair.Value;
                    if (ElementPair.Value == null)
                    {
                        Element = new IconSet();
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
                        if (AllowedIcon.GameModes.Contains(PoroQueue.Icon.GameMode.ARAM))
                        {
                            Element.EnableARAM = new CheckBox
                            {
                                Name = "ARAMEnabled " + Icon,
                                Text = "ARAM",
                                Location = new Point(0, 103 + CheckboxY)
                            };
                            Element.Container.Controls.Add(Element.EnableARAM);
                            CheckboxY += Element.EnableARAM.Height - 5;
                            Element.Container.Height += Element.EnableARAM.Height;
                        }

                        if (AllowedIcon.GameModes.Contains(PoroQueue.Icon.GameMode.NexusBlitz))
                        {
                            Element.EnableBlitz = new CheckBox
                            {
                                Name = "BlitzEnabled " + Icon,
                                Text = "Nexus Blitz",
                                Location = new Point(0, 103 + CheckboxY),
                            };
                            Element.Container.Controls.Add(Element.EnableBlitz);
                            CheckboxY += Element.EnableBlitz.Height - 5;
                            Element.Container.Height += Element.EnableBlitz.Height;
                        }

                        if (AllowedIcon.GameModes.Contains(PoroQueue.Icon.GameMode.URF))
                        {
                            Element.EnableUrf = new CheckBox
                            {
                                Name = "URFEnabled " + Icon,
                                Text = "URF",
                                Location = new Point(0, 103 + CheckboxY),
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

                        if (this.Height < y + 230)
                        {
                            this.Height = y + 230;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Debug.Break();
            }
        }
    }
}
