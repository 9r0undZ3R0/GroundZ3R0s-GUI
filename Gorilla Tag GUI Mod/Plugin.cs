using BepInEx;
using System;
using UnityEngine;
using Utilla;

namespace Gorilla_Tag_GUI_Mod
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
        GameObject shape = null;
        bool shapeexists = false;
        GameObject tpc;

        void Start()
        {
            /* A lot of Gorilla Tag systems will not be set up when start is called /*
			/* Put code in OnGameInitialized to avoid null references */

            Utilla.Events.GameInitialized += OnGameInitialized;
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

        void OnGameInitialized(object sender, EventArgs e)
        {
            tpc = GameObject.Find("Player Objects/Third Person Camera");
        }
        void CreateObject(UnityEngine.PrimitiveType Shape)
        {
            shape = GameObject.CreatePrimitive(Shape);

            GameObject.Destroy(shape.GetComponent<SphereCollider>());
            shape.transform.localPosition = GorillaTagger.Instance.rightHandTransform.position;
            shape.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            shape.GetComponent<Renderer>().material.color = Settings.shapecolor;
            shape.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");

            shapeexists = true;
        }
        void Update()
        {
            /* Code here runs every frame when the mod is enabled */
        }
        void OnGUI()
        {
            bool ToggleButton = GUI.Button(new Rect(20f, 20f, 130f, 30f), "Toggle GUI");

            if (ToggleButton)
            {
                Settings.shouldshowGUI = !Settings.shouldshowGUI;
            }

            if (Settings.shouldshowGUI)
            {
                bool FPCBUTTON = GUI.Button(new Rect(20f, 60f, 130f, 30f), "Toggle FPC");

                if (FPCBUTTON)
                {
                    if (tpc != null)
                    {
                        tpc.SetActive(tpc.activeSelf);
                    }
                }
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
            /* Deactivate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = false;
        }
    }
}
