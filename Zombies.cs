using GTA;
using System;
using GTA.Math;
using GTA.Native;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Zombies
{
    public class Zombies : Script
    {
        Random rnd = new Random();
        public bool Started = false;

        public Zombies()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Y)
            {
                if (Started)
                {
                    StopZombies();
                }
                else
                {
                    StartZombies();
                }
            }
        }

        private void StartZombies()
        {
            UI.Notify("Zombiez activated");
            Started = true;
            World.SetBlackout(true);
        }

        private void StopZombies()
        {
            UI.Notify("Zombiez should stop now");
            Started = false;
            World.SetBlackout(false);
            GTA.Native.Function.Call(GTA.Native.Hash.SET_MAX_WANTED_LEVEL, 5);
        }

        private void Zombify(Ped ped)
        {
            if (ped.IsPlayer == false)
            {
                if (!Function.Call<bool>(Hash.HAS_CLIP_SET_LOADED, new InputArgument[] { "move_m@drunk@verydrunk" }))
                {
                    Function.Call(Hash.REQUEST_CLIP_SET, new InputArgument[] { "move_m@drunk@verydrunk" });
                }

                if (Function.Call<bool>(Hash.HAS_CLIP_SET_LOADED, new InputArgument[] { "move_m@drunk@verydrunk" }))
                {
                    Function.Call(Hash.SET_PED_MOVEMENT_CLIPSET, new InputArgument[] { ped.Handle, "move_m@drunk@verydrunk", 1048576000 });
                }

                Function.Call(Hash.STOP_PED_SPEAKING, new InputArgument[] { ped.Handle, true });
                Function.Call(Hash.DISABLE_PED_PAIN_AUDIO, new InputArgument[] { ped.Handle, true });

                GTA.Native.Function.Call(GTA.Native.Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, ped, 1);
                GTA.Native.Function.Call(GTA.Native.Hash.SET_PED_FLEE_ATTRIBUTES, ped, 0, 0);
                GTA.Native.Function.Call(GTA.Native.Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 46, 1);
                // GTA.Native.Function.Call(GTA.Native.Hash._SET_MOVE_SPEED_MULTIPLIER, ped,9999.6F);		 

                ped.Task.GoTo(Game.Player.Character.Position, true);
                ped.AlwaysKeepTask = true;
                ped.IsEnemy = true;
                ped.Health = 3000;

                //Apply the blood to the ped!
                GTA.Native.Function.Call(GTA.Native.Hash.APPLY_PED_DAMAGE_PACK, ped, "BigHitByVehicle", 0.0, 9.0);
                GTA.Native.Function.Call(GTA.Native.Hash.APPLY_PED_DAMAGE_PACK, ped, "SCR_Dumpster", 0.0, 9.0);
                GTA.Native.Function.Call(GTA.Native.Hash.APPLY_PED_DAMAGE_PACK, ped, "SCR_Torture", 0.0, 9.0);

                if (ped.IsSittingInVehicle())
                {
                    //ped.CurrentVehicle.EngineHealth = 0;
                    ped.CurrentVehicle.HandbrakeOn = true;
                }
            }
        }

        private void SPZombified()
        {
            for (int z = 0; z < rnd.Next(1, 9); z++)
            {
                if (GTA.Native.Function.Call<bool>(GTA.Native.Hash.CAN_CREATE_RANDOM_PED)
                    || GTA.Native.Function.Call<bool>(GTA.Native.Hash.CAN_CREATE_RANDOM_DRIVER)
                    || GTA.Native.Function.Call<bool>(GTA.Native.Hash.CAN_CREATE_RANDOM_COPS)
                    || GTA.Native.Function.Call<bool>(GTA.Native.Hash.CAN_CREATE_RANDOM_BIKE_RIDER)
                )
                {
                    Vector3 SpawnPoint = new Vector3();

                    //More zombies!
                    if (z == 1)
                    {
                        SpawnPoint = Game.Player.Character.Position.Around(100);
                    }
                    else
                    {
                        SpawnPoint = SpawnPoint.Around(3);
                    }

                    SpawnPoint.Z = GTA.World.GetGroundHeight(SpawnPoint);
                    SpawnPoint.Z -= 0.9f;

                    Ped RandZomb = World.CreatePed(PedHash.Business01AMY, SpawnPoint);
                    Zombify(RandZomb);
                    RandZomb.MarkAsNoLongerNeeded();
                }

                Wait(0);
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (Started)
            {
                if (!GTA.Native.Function.Call<bool>(GTA.Native.Hash.IS_PLAYER_DEAD, Game.Player, true))
                {
                    //Attempt some spawning if the player is popping off rounds or the rando ticks
                    if (rnd.Next(0, 3) == 1 || Game.Player.Character.IsShooting)
                    {
                        SPZombified();
                    }

                    GTA.Native.Function.Call(GTA.Native.Hash.SET_MAX_WANTED_LEVEL, 0);
                    //World.Weather=GTA.Weather.ThunderStorm; //Fuck this weather
                    // GTA.Native.Function.Call(GTA.Native.Hash.SET_PED_DENSITY_MULTIPLIER_THIS_FRAME, 99.0F);
                    GTA.Native.Function.Call(GTA.Native.Hash.SET_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, 0.0F);
                    GTA.Native.Function.Call(GTA.Native.Hash.SET_RANDOM_VEHICLE_DENSITY_MULTIPLIER_THIS_FRAME, 0.0F);

                    Ped[] nearByPeople = World.GetNearbyPeds(Game.Player.Character, 400);
                    for (int i = 0; i < nearByPeople.Length; i++)
                    {
                        Vehicle V = World.GetClosestVehicle(Game.Player.Character.Position, 400);
                        Ped DriverZ = V.GetPedOnSeat(VehicleSeat.Driver);
                        if (DriverZ != null && DriverZ.IsPlayer == false)
                        {
                            Zombify(DriverZ);

                            //Get the 5 closest people by the driver we just zombied
                            Ped[] peopleByTheDriver = World.GetNearbyPeds(DriverZ, 300);
                            int MaxArrSize = peopleByTheDriver.Length;
                            if (MaxArrSize > 5)
                            {
                                MaxArrSize = 5;
                            }

                            for (int j = 0; j < MaxArrSize; j++)
                            {
                                Zombify(peopleByTheDriver[j]);
                                //if (peopleByTheDriver[j].IsPlayer == false)
                                //{
                                //    if (nearByPeople[i].Exists() && nearByPeople[i].IsAlive && !nearByPeople[i].IsRagdoll && !nearByPeople[i].IsGettingUp && nearByPeople[i].IsHuman
                                //        && !GTA.Native.Function.Call<bool>(GTA.Native.Hash.IS_PED_IN_GROUP, nearByPeople[i])
                                //    )
                                //    {
                                //        Zombify(peopleByTheDriver[j]);
                                //    }
                                //}

                                //Wait(0);
                            }

                        }

                        if (nearByPeople[i].Exists() && nearByPeople[i].IsAlive && !nearByPeople[i].IsRagdoll && !nearByPeople[i].IsGettingUp && nearByPeople[i].IsHuman
                            && !GTA.Native.Function.Call<bool>(GTA.Native.Hash.IS_PED_IN_GROUP, nearByPeople[i])
                        )
                        {
                            if (World.GetDistance(Game.Player.Character.Position, nearByPeople[i].Position) < 1.2F
                                && !Game.Player.Character.IsGettingUp && !Game.Player.Character.IsRagdoll
                                // &&!Game.Player.Character.IsSittingInVehicle()
                                // &&!GTA.Native.Function.Call<bool>(GTA.Native.Hash.IS_PED_ON_ANY_BIKE, Game.Player.Character)
                                && !GTA.Native.Function.Call<bool>(GTA.Native.Hash.IS_PED_CLIMBING, nearByPeople[i])
                                && !GTA.Native.Function.Call<bool>(GTA.Native.Hash.IS_PED_FLEEING, nearByPeople[i])
                            )
                            {
                                // GTA.Native.Function.Call(GTA.Native.Hash.TASK_CLIMB, nearByPeople[i],true);
                                GTA.Native.Function.Call(GTA.Native.Hash.APPLY_DAMAGE_TO_PED, Game.Player.Character, 15);
                                GTA.Native.Function.Call(Hash.SET_PED_TO_RAGDOLL, Game.Player.Character, 1, 9000, 9000, 1, 1, 1);
                                GTA.Native.Function.Call(Hash.SET_PED_TO_RAGDOLL, nearByPeople[i], 1, 100, 100, 1, 1, 1);
                                nearByPeople[i].ApplyForceRelative(new GTA.Math.Vector3(0, 1, 2));
                                Game.Player.Character.ApplyForceRelative(new GTA.Math.Vector3(0, -2, -10));
                            }

                            Zombify(nearByPeople[i]);
                            if (rnd.Next(0, 36) == 10 && nearByPeople[i].IsIdle && World.GetDistance(Game.Player.Character.Position, nearByPeople[i].Position) > 1.3F
                                && !GTA.Native.Function.Call<bool>(GTA.Native.Hash.IS_PED_CLIMBING, nearByPeople[i])
                            )
                            {
                                GTA.Native.Function.Call(GTA.Native.Hash.TASK_CLIMB, nearByPeople[i], true);
                                nearByPeople[i].ApplyForceRelative(new GTA.Math.Vector3(0, 4, 4));
                            }
                            Wait(0);
                        }
                    }
                }
                else
                {
                    StopZombies();
                }
            }
        }
    }
}


