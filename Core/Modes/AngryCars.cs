using GTA;
using System;
using GTA.Math;
using GTA.Native;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using NativeUI;
using Zombies.Core.Interfaces;

namespace Zombies
{
    public class AngryCars : Script, IGameMode
    {
        public static bool Started = false;

        public AngryCars()
        {

        }

        public  void Start(bool notify = true)
        {
            Started = true;

            if (notify)
            {
                UI.Notify("Angry Cars Activated");
            }
        }

        public  void Stop(bool notify = true)
        {
            Started = false;

            if (notify)
            {
                UI.Notify("Angry Cars De-Activated");
            }
        }

        public  void ProcessTick()
        {
            
        }
    }
}


