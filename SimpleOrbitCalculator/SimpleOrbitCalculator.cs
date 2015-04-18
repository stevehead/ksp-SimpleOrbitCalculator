using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleOrbitCalculator
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class OrbitCalculatorGUIController : UnityEngine.MonoBehaviour
    {
        private const string PluginName = "Simple Orbit Calculator";
        private const string PluginDirectoryName = "SimpleOrbitCalculator";
        private const string PluginIconButtonStock = "icon_button_stock";

        private bool windowOpen = false;
        private Rect windowPos = new Rect(100, 100, 800, 465);

        private ApplicationLauncherButton SOCButtonStock = null;

        public void Awake()
        {
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
        }

        void OnGUIAppLauncherReady()
        {
            if (ApplicationLauncher.Ready)
            {
                string pluginIconButtonStockPath = String.Format("{0}/Textures/{1}", PluginDirectoryName, PluginIconButtonStock);
                SOCButtonStock = ApplicationLauncher.Instance.AddModApplication(
                    OnAppLaunchToggleOn, OnAppLaunchToggleOff,
                    DummyVoid, DummyVoid,
                    DummyVoid, DummyVoid,
                    ApplicationLauncher.AppScenes.SPACECENTER,
                    (Texture)GameDatabase.Instance.GetTexture(pluginIconButtonStockPath, false));
            }
        }

        void OnAppLaunchToggleOn()
        {

        }

        void OnAppLaunchToggleOff()
        {

        }

        void DummyVoid() { }

        public void Start()
        {
            RenderingManager.AddToPostDrawQueue(0, new Callback(DrawGUI));
            print(PluginName + "GUI Loaded");
        }

        public void DrawGUI()
        {
            if (windowOpen)
            {
                windowPos = GUILayout.Window(PluginName.GetHashCode(), windowPos, MainWindow, PluginName,
                    GUILayout.Width(800), GUILayout.Height(465), GUILayout.ExpandWidth(false));
            }
        }

        private void MainWindow(int windowID)
        {

        }

        private void OnDestroy()
        {
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
            if (SOCButtonStock != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(SOCButtonStock);
            }
        }
    }
}
