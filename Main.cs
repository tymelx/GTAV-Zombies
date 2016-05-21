using GTA;
using System;
using GTA.Math;
using GTA.Native;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using NativeUI;

namespace Zombies
{
    public class Main : Script
    {
        //Menu components
        private static MenuPool _menuPool;
        public Zombies zombieMode = new Zombies();
        public AngryCars angryCarsMode = new AngryCars();

        public Main()
        {
            Tick += OnTick;

            //Menu initialization
            _menuPool = new MenuPool();
            var mainMenu = new UIMenu("tymeLX Mods", "~b~Select a Mode");
            _menuPool.Add(mainMenu);
            CreateModeMenu(mainMenu);
            _menuPool.RefreshIndex();
            KeyDown += (o, e) =>
            {
                if (e.KeyCode == Keys.Y && !_menuPool.IsAnyMenuOpen())
                {// Our menu on/off switch
                    mainMenu.Visible = !mainMenu.Visible;
                }
            };
        }

        public void CreateModeMenu(UIMenu menu)
        {
            menu.AddItem(new UIMenuListItem("Zombie Invasion", new List<dynamic>
            {
                "Off",
                "On"
            }, 0));

            menu.AddItem(new UIMenuListItem("Angry Cars", new List<dynamic>
            {
                "Off",
                "On"
            }, 0));

            menu.OnListChange += (sender, item, index) =>
            {
                UI.Notify(item.Text);
                if (index == 0)
                {
                    if (item.Text == "Zombie Invasion")
                    {
                        zombieMode.Stop();
                    }
                    else if (item.Text == "Angry Cars")
                    {
                        angryCarsMode.Stop();
                    }
                }
                else
                {
                    StopAllModes(menu, item.Text);

                    if (item.Text == "Zombie Invasion")
                    {
                        zombieMode.Start();
                    }
                    else if (item.Text == "Angry Cars")
                    {
                        angryCarsMode.Start();
                    }
                }
            };
        }

        private void StopAllModes(UIMenu menu, string mode)
        {
            zombieMode.Stop(false);
            if (mode != "Zombie Invasion")
            {
                ((UIMenuListItem)(menu.MenuItems[0])).Index = 0;
            }

            angryCarsMode.Stop(false);
            if (mode != "Angry Cars")
            {
                ((UIMenuListItem)menu.MenuItems[1]).Index = 0;
            }
        }

        public static void _Wait(int ms)
        {
            _menuPool.ProcessMenus();
            Wait(ms);
        }

        private void OnTick(object sender, EventArgs e)
        {
            _menuPool.ProcessMenus();

            //Process all modes
            zombieMode.ProcessTick();
            angryCarsMode.ProcessTick();
        }
    }
}


