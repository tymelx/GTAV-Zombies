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
        public Random rnd = new Random();

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

        public void Stop(bool notify = true)
        {
            Started = false;

            if (notify)
            {
                UI.Notify("Angry Cars De-Activated");
            }
        }

        public void ProcessTick()
        {
            if (Started)
            {
                if (!GTA.Native.Function.Call<bool>(GTA.Native.Hash.IS_PLAYER_DEAD, Game.Player, true))
                {
                    Vehicle[] vehicles = World.GetNearbyVehicles(Game.Player.Character.Position, 500);

                    foreach (Vehicle vehicle in vehicles)
                    {
                        Ped person = vehicle.GetPedOnSeat(VehicleSeat.Driver);
                        if (person != null)
                        {
                            if (person.IsPlayer == false && person.IsInVehicle() && person.SeatIndex == VehicleSeat.Driver)
                            {
                                MakeDriverAngry(person);
                            }
                        }
                    }
                }
                else
                {
                    Stop();
                }
            }
        }

        private void MakeDriverAngry(Ped driver)
        {
            if (rnd.Next(0, 15) == 7)
            {
                driver.DrivingStyle = DrivingStyle.IgnoreLights;
                driver.Task.DriveTo(driver.CurrentVehicle, Game.Player.Character.Position, 1, 30);
                driver.AlwaysKeepTask = true;
                driver.IsEnemy = true;
                driver.StaysInVehicleWhenJacked = true;

                Main._Wait(0);
            }
        }
    }
}


