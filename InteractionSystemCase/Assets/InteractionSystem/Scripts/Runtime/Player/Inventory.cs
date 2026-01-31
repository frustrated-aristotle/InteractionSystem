using System.Collections.Generic;
using UnityEngine;
using InteractionSystem.Runtime.Core;

namespace InteractionSystem.Runtime.Player
{
    /// <summary>
    /// Oyuncunun topladığı item'ları (anahtar vb.) tutan basit envanter bileşeni.
    /// </summary>
    /// <remarks>
    /// Key pickup ve locked door gibi etkileşimler için kullanılır.
    /// Aynı GameObject üzerindeki Interactor ile birlikte çalışır.
    /// </remarks>
    public class Inventory : MonoBehaviour, IInventory
    {
        #region Fields

        [SerializeField] private List<ScriptableObject> m_Items = new List<ScriptableObject>();

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
                return;
            }

            m_Items.Add(item);
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

        #endregion
    }
}
