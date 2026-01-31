#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using InteractionSystem.Runtime.Player;

namespace InteractionSystem.Editor
{
    /// <summary>
    /// Editor'da Play modundan çıkarken SaveManager.Save() çağrılır; böylece state kaydedilir (Build'de OnApplicationQuit zaten çalışır).
    /// </summary>
    [InitializeOnLoad]
    public static class SaveManagerEditorHook
    {
        static SaveManagerEditorHook()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingPlayMode) return;

            var manager = Object.FindObjectOfType<SaveManager>();
            if (manager != null)
            {
                manager.Save();
                Debug.Log("[SaveManager] Editor: Play bitti, Save() çağrıldı.");
            }
        }
    }
}
#endif
