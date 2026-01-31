using UnityEngine;
using InteractionSystem.Runtime.Core;
using InteractionSystem.Runtime.Interactables;

namespace InteractionSystem.Runtime.Player
{
    /// <summary>
    /// Interactable state'leri ve envanteri PlayerPrefs + JSON ile kaydeder/yükler. Sahne yüklenince Load, çıkışta Save çağrılabilir.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        private const string k_SaveKey = "InteractionSaveData";

        [SerializeField] [Tooltip("Sahne yüklenince otomatik yükle.")]
        private bool m_LoadOnStart = true;
        [SerializeField] [Tooltip("Uygulama kapanırken otomatik kaydet.")]
        private bool m_SaveOnQuit = true;

        #region Unity Methods

        private void Start()
        {
            if (m_LoadOnStart)
            {
                Load();
            }
        }

        private void OnApplicationQuit()
        {
            if (m_SaveOnQuit)
            {
                Save();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Tüm Door, Chest, KeyPickup, Switch ve Inventory state'ini toplar ve PlayerPrefs'e yazar.
        /// </summary>
        public void Save()
        {
            var data = new InteractionSaveData();

            var doors = FindObjectsOfType<Door>();
            data.doors = new InteractionSaveData.Entry[doors.Length];
            for (int i = 0; i < doors.Length; i++)
            {
                data.doors[i] = new InteractionSaveData.Entry { id = doors[i].GetSaveId(), state = doors[i].SerializeState() };
            }

            var chests = FindObjectsOfType<Chest>();
            data.chests = new InteractionSaveData.Entry[chests.Length];
            for (int i = 0; i < chests.Length; i++)
            {
                data.chests[i] = new InteractionSaveData.Entry { id = chests[i].GetSaveId(), state = chests[i].SerializeState() };
            }

            var keyPickups = FindObjectsOfType<KeyPickup>();
            data.keyPickups = new InteractionSaveData.Entry[keyPickups.Length];
            for (int i = 0; i < keyPickups.Length; i++)
            {
                data.keyPickups[i] = new InteractionSaveData.Entry { id = keyPickups[i].GetSaveId(), state = keyPickups[i].SerializeState() };
            }

            var switches = FindObjectsOfType<Switch>();
            data.switches = new InteractionSaveData.Entry[switches.Length];
            for (int i = 0; i < switches.Length; i++)
            {
                data.switches[i] = new InteractionSaveData.Entry { id = switches[i].GetSaveId(), state = switches[i].SerializeState() };
            }

            var pressurePlates = FindObjectsOfType<PressurePlate>();
            data.pressurePlates = new InteractionSaveData.Entry[pressurePlates.Length];
            for (int i = 0; i < pressurePlates.Length; i++)
            {
                data.pressurePlates[i] = new InteractionSaveData.Entry { id = pressurePlates[i].GetSaveId(), state = pressurePlates[i].SerializeState() };
            }

            var inventory = FindObjectOfType<Inventory>();
            data.inventoryKeyNames = inventory != null ? inventory.GetSaveState() : new string[0];

            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(k_SaveKey, json);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// PlayerPrefs'ten okuyup state'i tüm interactable'lara ve envantere uygular.
        /// </summary>
        public void Load()
        {
            if (!PlayerPrefs.HasKey(k_SaveKey)) return;

            string json = PlayerPrefs.GetString(k_SaveKey);
            var data = JsonUtility.FromJson<InteractionSaveData>(json);
            if (data == null)
            {
                Debug.LogError("[SaveManager] Corrupt or invalid save data; load aborted.");
                return;
            }

            foreach (var door in FindObjectsOfType<Door>())
            {
                foreach (var e in data.doors ?? System.Array.Empty<InteractionSaveData.Entry>())
                {
                    if (e.id == door.GetSaveId()) { door.LoadState(e.state); break; }
                }
            }

            foreach (var chest in FindObjectsOfType<Chest>())
            {
                foreach (var e in data.chests ?? System.Array.Empty<InteractionSaveData.Entry>())
                {
                    if (e.id == chest.GetSaveId()) { chest.LoadState(e.state); break; }
                }
            }

            foreach (var kp in FindObjectsOfType<KeyPickup>())
            {
                foreach (var e in data.keyPickups ?? System.Array.Empty<InteractionSaveData.Entry>())
                {
                    if (e.id == kp.GetSaveId()) { kp.LoadState(e.state); break; }
                }
            }

            foreach (var sw in FindObjectsOfType<Switch>())
            {
                foreach (var e in data.switches ?? System.Array.Empty<InteractionSaveData.Entry>())
                {
                    if (e.id == sw.GetSaveId()) { sw.LoadState(e.state); break; }
                }
            }

            foreach (var pp in FindObjectsOfType<PressurePlate>())
            {
                foreach (var e in data.pressurePlates ?? System.Array.Empty<InteractionSaveData.Entry>())
                {
                    if (e.id == pp.GetSaveId()) { pp.LoadState(e.state); break; }
                }
            }

            var inv = FindObjectOfType<Inventory>();
            if (inv != null && data.inventoryKeyNames != null)
            {
                inv.LoadState(data.inventoryKeyNames);
            }
        }

        #endregion
    }
}
