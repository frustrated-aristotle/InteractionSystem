using System;
using System.Collections.Generic;
using UnityEngine;
using InteractionSystem.Runtime.Core;
using InteractionSystem.Runtime.Interactables;

namespace InteractionSystem.Runtime.Player
{
    /// <summary>
    /// Oyuncunun topladığı item'ları (anahtar vb.) tutan basit envanter bileşeni.
    /// </summary>
    /// <remarks>
    /// Key pickup ve locked door gibi etkileşimler için kullanılır.
    /// Aynı GameObject üzerindeki Interactor ile birlikte çalışır.
    /// Envanter değişince OnInventoryChanged tetiklenir (UI güncelleme vb.).
    /// </remarks>
    public class Inventory : MonoBehaviour, IInventory
    {
        #region Events

        /// <summary>
        /// Envanter içeriği değiştiğinde tetiklenir (AddItem, LoadState vb.).
        /// UI güncellemesi için dinlenebilir.
        /// </summary>
        public event Action OnInventoryChanged;

        #endregion

        #region Fields

        [SerializeField] private List<ScriptableObject> m_Items = new List<ScriptableObject>();
        [SerializeField] [Tooltip("LoadState için KeyName eşlemesi. Boşsa Resources'tan KeyItemData aranır.")]
        private KeyItemData[] m_AllKeyTypesInGame;

        #endregion

        #region Properties

        /// <summary>
        /// Envanterdeki tüm item'ların readonly listesi.
        /// </summary>
        public IReadOnlyList<ScriptableObject> Items => m_Items;

        #endregion

        #region Methods

        /// <summary>
        /// Belirtilen item'ı envantere ekler.
        /// </summary>
        /// <param name="item">Eklenecek item (ScriptableObject referansı).</param>
        public void AddItem(ScriptableObject item)
        {
            if (item == null)
            {
                Debug.LogWarning("[Inventory] AddItem: item is null.");
                return;
            }

            m_Items.Add(item);
            OnInventoryChanged?.Invoke();
        }

        /// <summary>
        /// Envanterde belirtilen item türünden en az bir adet olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="item">Aranacak item (ScriptableObject referansı, aynı asset = aynı key tipi).</param>
        /// <returns>Varsa true, yoksa false.</returns>
        public bool HasItem(ScriptableObject item)
        {
            if (item == null)
            {
                return false;
            }

            return m_Items.Contains(item);
        }

        /// <inheritdoc/>
        public bool HasKey(KeyItemData key)
        {
            if (key == null)
            {
                return false;
            }

            for (int i = 0; i < m_Items.Count; i++)
            {
                if (m_Items[i] is KeyItemData invKey)
                {
                    if (object.ReferenceEquals(invKey, key))
                    {
                        return true;
                    }

                    if (invKey.KeyName == key.KeyName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Kayıt için envanterdeki anahtar adlarını döndürür.
        /// </summary>
        public string[] GetSaveState()
        {
            var list = new List<string>();
            foreach (var item in m_Items)
            {
                if (item is KeyItemData key)
                {
                    list.Add(key.KeyName);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Kayıtlı anahtar adlarını envantere uygular. KeyItemData eşlemesi m_AllKeyTypesInGame, sonra sahnedeki KeyPickup/Chest/Door referansları, yoksa Resources üzerinden yapılır.
        /// </summary>
        public void LoadState(string[] keyNames)
        {
            m_Items.Clear();
            if (keyNames == null || keyNames.Length == 0)
            {
                OnInventoryChanged?.Invoke();
                return;
            }

            KeyItemData[] allKeys = GetAllKeyTypesInGame();

            foreach (string name in keyNames)
            {
                foreach (var key in allKeys)
                {
                    if (key != null && key.KeyName == name)
                    {
                        m_Items.Add(key);
                        break;
                    }
                }
            }

            OnInventoryChanged?.Invoke();
        }

        /// <summary>
        /// Oyunda kullanılan tüm KeyItemData referanslarını döndürür (Inventory load için eşleme kaynağı).
        /// Önce m_AllKeyTypesInGame, yoksa sahnedeki KeyPickup/Chest/Door referansları, son çare Resources.
        /// </summary>
        private KeyItemData[] GetAllKeyTypesInGame()
        {
            if (m_AllKeyTypesInGame != null && m_AllKeyTypesInGame.Length > 0)
                return m_AllKeyTypesInGame;

            var list = new List<KeyItemData>();

            foreach (var kp in FindObjectsOfType<KeyPickup>())
            {
                if (kp.KeyData != null && !list.Contains(kp.KeyData))
                    list.Add(kp.KeyData);
            }
            foreach (var chest in FindObjectsOfType<Chest>())
            {
                if (chest.ItemInside != null && !list.Contains(chest.ItemInside))
                    list.Add(chest.ItemInside);
            }
            foreach (var door in FindObjectsOfType<Door>())
            {
                if (door.RequiredKey != null && !list.Contains(door.RequiredKey))
                    list.Add(door.RequiredKey);
            }

            if (list.Count > 0)
                return list.ToArray();

            return Resources.LoadAll<KeyItemData>("");
        }

        #endregion
    }
}
