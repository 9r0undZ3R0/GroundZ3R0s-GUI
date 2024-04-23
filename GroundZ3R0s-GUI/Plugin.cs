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
            bool TOGGLEBUTTON = GUI.Button(new Rect(20f, 20f, 140f, 40f), "Toggle GUI");

            if (TOGGLEBUTTON)
            {
                showGUI = !showGUI;
            }

            if (showGUI)
            {
                GUI.contentColor = Color.magenta;
                GUI.Box(new Rect(20f, 100f, 450f, 450f), "GroundZ3R0's GUI");

                bool FPC = GUI.Button(new Rect(40f, 140f, 140f, 40f), "Toggle FPC");
                bool SPIDERMONK = GUI.Button(new Rect(40f, 200f, 140f, 40f), "Toggle Spider Monk (BUGGY, MODDED)");
                GUI.Label(new Rect(240f, 200f, 140f, 40f), $"Spider Monk On: {spidermonkenabled}");
                bool FLY = GUI.Button(new Rect(40f, 260f, 140f, 40f), "Toggle Fly (MODDED)");
                GUI.Label(new Rect(280f, 260f, 140f, 40f), $"Fly On: {flyenabled}");

                if (FPC)
                {
                    if (tpc != null)
                    {
                        tpc.SetActive(!tpc.activeSelf);
                    }
                }
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
            if (spidermonkenabled)
            {
                SpiderMonkMod();
            }
            if (flyenabled)
            {
                FlyMod();
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
