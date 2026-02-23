using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MonoMod.RuntimeDetour;
using System.Reflection;
using System.Collections;


namespace ChangePunchoutTime
{
    [BepInDependency(ETGModMainBehaviour.GUID)]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static PunchoutController punchoutController = null;
        private static float targetTime = 120f;
        public const string GUID = "nexus.etg.punchouttimemod";
        public const string NAME = "Change Punchout Time";
        public const string VERSION = "1.0.0";
        public const string TEXT_COLOR = "#00FFFF";

        public void Start()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);

            Hook punchouthook = new Hook(
                typeof(PunchoutController).GetMethod("Start", BindingFlags.Instance | BindingFlags.Public),
                typeof(Plugin).GetMethod("StartPunchoutHook")
                );

            ETGModConsole.Commands.AddGroup("punchouttime", args =>
            {
                string punchouttime = args[0];
                try
                {
                    UInt32 punchouttimeseconds = UInt32.Parse(punchouttime);
                    try
                    {
                        punchoutController.TimerStartTime = float.Parse(punchouttimeseconds.ToString());
                    }
                    catch
                    {

                    }
                    targetTime = float.Parse(punchouttimeseconds.ToString());
                }
                catch
                {
                    Log($"command punchouttime accepts only positive neutral numbers", "#ff0000");
                }
            });
            ETGModConsole.CommandDescriptions["punchouttime"] = "Sets the time of Resourceful Rat Punch-Out phase in seconds. For example, typing punchouttime 180 will set the time of Resourceful Rat Punch-Out phase to 180 seconds. Default value is 120.";

            ETGModConsole.Commands.AddGroup("punchouttime_value", args =>
            {
                Log($"The current Resourceful Rat Punch-Out phase time is set to " +
                $"<color=#FF0000>{targetTime}</color> seconds");
            });
            ETGModConsole.CommandDescriptions["punchouttime_value"] = "Display the current time of Resourceful Rat Punch-Out Phase in seconds. Default value is 120.";
        }

        public void GMStart(GameManager g)
        {
            Log($"{NAME} v{VERSION} started successfully.", TEXT_COLOR);
        }

        public static void Log(string text, string color = "FFFFFF")
        {
            ETGModConsole.Log($"<color={color}>{text}</color>");
        }


        public static IEnumerator StartPunchoutHook(Func<PunchoutController, IEnumerator> orig, PunchoutController self)
        {
            punchoutController = self;
            self.TimerStartTime = targetTime;
            yield return orig(self);
        }
    }
}