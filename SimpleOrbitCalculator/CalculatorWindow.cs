using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleOrbitCalculator
{
    internal partial class CalculatorWindow : MonoBehaviour
    {
        /// <summary>
        /// Called when the calculator is being loaded.
        /// </summary>
        public void Awake()
        {
            // Window setup.
            id = Guid.NewGuid().GetHashCode();

            // Load celestials.
            if (celestialBodies == null)
            {
                LoadAllCelestialInformation();
            }

            // Add event handlers.
            GameEvents.onShowUI.Add(ShowUI);
            GameEvents.onHideUI.Add(HideUI);
            GameEvents.onGameSceneLoadRequested.Add(GameSceneLoadRequested);
        }

        /// <summary>
        /// Called when destroyed.
        /// </summary>
        public void OnDestroy()
        {
            // Remove event handlers.
            GameEvents.onShowUI.Remove(ShowUI);
            GameEvents.onHideUI.Remove(HideUI);
            GameEvents.onGameSceneLoadRequested.Remove(GameSceneLoadRequested);

            // Need to tell the app launcher to turn off the button.
            SimpleOrbitCalculatorController.SetApplauncherButtonFalse();
        }

        /// <summary>
        /// Called for rendering and handling the GUI.
        /// </summary>
        public void OnGUI()
        {
            if (isWindowOpen)
            {
                windowPosition = GUILayout.Window(id, windowPosition, RenderWindow, WindowTitle);
            }
        }

        /// <summary>
        /// Shows the UI.
        /// </summary>
        private void ShowUI()
        {
            isWindowOpen = true;
        }

        /// <summary>
        /// Hides the UI.
        /// </summary>
        private void HideUI()
        {
            isWindowOpen = false;
        }

        /// <summary>
        /// Hides the UI during game scene change.
        /// </summary>
        /// <param name="newGameScene">the new game scene</param>
        private void GameSceneLoadRequested(GameScenes newGameScene)
        {
            HideUI();
        }
    }
}
