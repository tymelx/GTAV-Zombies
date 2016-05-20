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
            var zombies = new List<dynamic>
            {
                "On",
                "Off"
            };

            var newitem = new UIMenuListItem("Zombie Invasion", zombies, 0);
            menu.AddItem(newitem);
            menu.OnListChange += (sender, item, index) =>
            {
                if (index == 0)
                {
                    Zombies.StartZombies();
                }
                else
                {
                    Zombies.StopZombies();
                }
            };
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
            Zombies.ProcessTick();
        }
    }
}


