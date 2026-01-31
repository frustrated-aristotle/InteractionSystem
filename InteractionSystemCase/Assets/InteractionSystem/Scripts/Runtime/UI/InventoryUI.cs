using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using InteractionSystem.Runtime.Core;
using InteractionSystem.Runtime.Player;

namespace InteractionSystem.Runtime.UI
{
    /// <summary>
    /// Envanter içeriğini TextMeshProUGUI'da listeler (örn: Gold Key (x1), Rusty Key (x2)).
    /// Envanter her güncellendiğinde OnInventoryChanged ile tetiklenir ve metin yenilenir.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] [Tooltip("Envanteri tutan oyuncu bileşeni. Boşsa sahnede Inventory aranır.")]
        private Inventory m_Inventory;

        [SerializeField] [Tooltip("Envanter listesinin yazılacağı TextMeshProUGUI. Boşsa aynı objede veya child'da aranır.")]
        private TextMeshProUGUI m_ListText;

        private void Awake()
        {
            if (m_Inventory == null)
            {
                m_Inventory = FindFirstObjectByType<Inventory>();
                if (m_Inventory == null)
                {
                    Debug.LogWarning("[InventoryUI] Inventory atanmamış ve sahnede bulunamadı. Inspector'dan ata.");
                }
            }

            if (m_ListText == null)
            {
                m_ListText = GetComponentInChildren<TextMeshProUGUI>(true);
                if (m_ListText == null)
                {
                    Debug.LogWarning("[InventoryUI] List Text atanmamış ve child'da TextMeshProUGUI bulunamadı. Inspector'dan ata veya aynı objeye TMP ekleyin.");
                }
            }
        }

        private void OnEnable()
        {
            if (m_Inventory != null)
            {
                m_Inventory.OnInventoryChanged += RefreshDisplay;
                RefreshDisplay();
            }
        }

        private void OnDisable()
        {
            if (m_Inventory != null)
            {
                m_Inventory.OnInventoryChanged -= RefreshDisplay;
            }
        }

        /// <summary>
        /// Envanterdeki anahtarları gruplayıp sayı ile birlikte metne yazar (örn: Gold Key (x1), Rusty Key (x2)).
        /// </summary>
        private void RefreshDisplay()
        {
            if (m_ListText == null)
            {
                return;
            }

            if (m_Inventory == null)
            {
                m_ListText.text = "";
                return;
            }

            var grouped = GroupKeysByName(m_Inventory.Items);
            m_ListText.text = string.Join("\n", grouped.Select(p => $"{p.Key} (x{p.Value})"));
        }

        /// <summary>
        /// Envanterdeki KeyItemData'ları isim bazında gruplayıp (isim, adet) döndürür.
        /// </summary>
        private static IEnumerable<KeyValuePair<string, int>> GroupKeysByName(IReadOnlyList<ScriptableObject> items)
        {
            var counts = new Dictionary<string, int>();

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is KeyItemData key)
                {
                    string name = key.KeyName ?? "Key";
                    if (!counts.ContainsKey(name))
                    {
                        counts[name] = 0;
                    }
                    counts[name]++;
                }
            }

            return counts.OrderBy(p => p.Key);
        }
    }
}
