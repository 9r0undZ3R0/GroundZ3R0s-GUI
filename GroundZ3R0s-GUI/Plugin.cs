using BepInEx;
using Steamworks;
using System;
using UnityEngine;
using Utilla;

namespace GroundZ3R0s_GUI
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;
        bool showGUI = false;
        bool spidermonkenabled = false;
        bool flyenabled = false;
        bool settings = false;
        bool moddedmods = false;
        public static Rect menuSize = new Rect(50f, 100f, 500f, 500f);
        string openorclose = "Open";
        string openorclosemodded = "Open";
        GameObject tpc;

        void Start()
        {
            /* A lot of Gorilla Tag systems will not be set up when start is called /*
			/* Put code in OnGameInitialized to avoid null references */

            Utilla.Events.GameInitialized += __init__;
        }

        void OnEnable()
        {
            /* Set up your mod here */
            /* Code here runs at the start and whenever your mod is enabled*/

            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            /* Undo mod setup here */
            /* This provides support for toggling mods with ComputerInterface, please implement it :) */
            /* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

            HarmonyPatches.RemoveHarmonyPatches();
        }

        void __init__(object sender, EventArgs e)
        {
            tpc = GameObject.Find("Third Person Camera/Shoulder Camera");
        }

        void OnGUI()
        {
            GUI.backgroundColor = Color.magenta;
            GUI.color = Color.magenta;

            bool TOGGLEBUTTON = GUI.Button(new Rect(20f, 20f, 130f, 40f), "Toggle GUI");

            if (TOGGLEBUTTON)
            {
                showGUI = !showGUI;
            }

            if (showGUI)
            {
                GUI.Box(menuSize, "GroundZ3R0's GUI");
                bool OPENSETTINGS = GUI.Button(new Rect(60f, 140f, 130f, 40f), $"{openorclose} Settings");
                bool OPENMODDEDMODS = GUI.Button(new Rect(60f, 200f, 130f, 40f), $"{openorclosemodded} Modded Mods");

                if (OPENSETTINGS && !moddedmods)
                {
                    settings = !settings;
                }
                if (OPENMODDEDMODS && !settings)
                {
                    moddedmods = !moddedmods;
                }

                if (settings && !moddedmods)
                {
                    GUI.Box(new Rect(650f, 100f, 500f, 500f), "Settings");
                    bool FPC = GUI.Button(new Rect(700f, 140f, 140f, 40f), "Toggle FPC");

                    if (FPC)
                    {
                        if (tpc != null)
                        {
                            tpc.SetActive(!tpc.activeSelf);
                        }
                    }
                }
                if (moddedmods && !settings)
                {
                    GUI.Box(new Rect(650f, 100f, 500f, 500f), "Modded Mods");
                    bool SPIDERMONK = GUI.Button(new Rect(700f, 140f, 140f, 60f), "Toggle Spider Monk\n(BUGGY, MODDED)");
                    GUI.Label(new Rect(850f, 140f, 140f, 40f), $"Spider Monk On: {spidermonkenabled}");
                    bool FLY = GUI.Button(new Rect(700f, 210f, 140f, 40f), "Toggle Fly (MODDED)");
                    GUI.Label(new Rect(850f, 210f, 140f, 40f), $"Fly On: {flyenabled}");

                    if (SPIDERMONK)
                    {
                        if (inRoom)
                        {
                            spidermonkenabled = !spidermonkenabled;
                        }
                    }
                    if (FLY)
                    {
                        if (inRoom)
                        {
                            flyenabled = !flyenabled;
                        }
                    }
                }
            }
        }
        void SpiderMonkMod()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat >= 0.7f)
            {
                Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position, GorillaLocomotion.Player.Instance.rightControllerTransform.forward, out var hitinfo);
                GameObject lineObject = new GameObject("Line");

                LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
                lineRenderer.startColor = Color.white;
                lineRenderer.endColor = Color.white;
                lineRenderer.startWidth = 0.02f;
                lineRenderer.endWidth = 0.02f;
                lineRenderer.positionCount = 2;
                lineRenderer.useWorldSpace = true;
                lineRenderer.SetPosition(0, GorillaLocomotion.Player.Instance.rightControllerTransform.position);
                lineRenderer.SetPosition(1, hitinfo.point);
                lineRenderer.material.shader = Shader.Find("GorillaTag/UberShader");

                UnityEngine.Object.Destroy(lineObject, Time.deltaTime);
            }
            if (ControllerInputPoller.instance.leftControllerIndexFloat >= 0.7f)
            {
                Physics.Raycast(GorillaLocomotion.Player.Instance.leftControllerTransform.position, GorillaLocomotion.Player.Instance.leftControllerTransform.forward, out var hitinfo);
                GameObject lineObject = new GameObject("Line");

                LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
                lineRenderer.startColor = Color.white;
                lineRenderer.endColor = Color.white;
                lineRenderer.startWidth = 0.02f;
                lineRenderer.endWidth = 0.02f;
                lineRenderer.positionCount = 2;
                lineRenderer.useWorldSpace = true;
                lineRenderer.SetPosition(0, GorillaLocomotion.Player.Instance.leftControllerTransform.position);
                lineRenderer.SetPosition(1, hitinfo.point);
                lineRenderer.material.shader = Shader.Find("GorillaTag/UberShader");

                UnityEngine.Object.Destroy(lineObject, Time.deltaTime);
            }
        }
        void FlyMod()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                GorillaLocomotion.Player.Instance.transform.position += GorillaLocomotion.Player.Instance.headCollider.transform.forward * Time.deltaTime * 5f;
                GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }

        void Update()
        {
            if (spidermonkenabled && inRoom)
            {
                SpiderMonkMod();
            }
            if (flyenabled && inRoom)
            {
                FlyMod();
            }

            if (settings)
            {
                openorclose = "Close";
            }
            else if (!settings)
            {
                openorclose = "Open";
            }

            if (moddedmods)
            {
                openorclosemodded = "Close";
            }
            else if (!moddedmods)
            {
                openorclosemodded = "Open";
            }
        }

        /* This attribute tells Utilla to call this method when a modded room is joined */
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            /* Activate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = true;
        }

        /* This attribute tells Utilla to call this method when a modded room is left */
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            spidermonkenabled = false;
            flyenabled = false;

            inRoom = false;
        }
    }
}
