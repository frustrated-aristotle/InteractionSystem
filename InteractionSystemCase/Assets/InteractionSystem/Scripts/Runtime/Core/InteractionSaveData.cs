using System;
using UnityEngine;

namespace InteractionSystem.Runtime.Core
{
    /// <summary>
    /// Tüm interactable state'leri ve envanteri tek yapıda toplar; PlayerPrefs + JSON ile kaydedilir.
    /// </summary>
    [Serializable]
    public class InteractionSaveData
    {
        [Serializable]
        public class Entry
        {
            public string id;
            public string state;
        }

        public Entry[] doors = Array.Empty<Entry>();
        public Entry[] chests = Array.Empty<Entry>();
        public Entry[] keyPickups = Array.Empty<Entry>();
        public Entry[] switches = Array.Empty<Entry>();
        public string[] inventoryKeyNames = Array.Empty<string>();
    }
}
