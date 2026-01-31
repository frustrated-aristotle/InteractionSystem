using System.Collections;
using UnityEngine;
using TMPro;

namespace InteractionSystem.Runtime.UI
{
    /// <summary>
    /// Item bilgisini (sandık / key pickup) TextMeshProUGUI'da gösterir; belirtilen süre sonra temizler.
    /// </summary>
    public class InfoDisplay : MonoBehaviour
    {
        [SerializeField] [Tooltip("Bilgi metninin yazılacağı TextMeshProUGUI. Boşsa aynı objede veya child'da aranır.")]
        private TextMeshProUGUI m_InfoText;

        private Coroutine m_ClearRoutine;

        private void Awake()
        {
            if (m_InfoText == null)
            {
                m_InfoText = GetComponentInChildren<TextMeshProUGUI>(true);
                if (m_InfoText == null)
                {
                    Debug.LogWarning("[InfoDisplay] Info Text atanmamış ve child'da TextMeshProUGUI bulunamadı. Inspector'dan ata veya aynı objeye TMP ekleyin.");
                }
            }
        }

        /// <summary>
        /// Bilgi metnini gösterir; displayDuration saniye sonra siler.
        /// </summary>
        /// <param name="info">Gösterilecek metin.</param>
        /// <param name="displayDuration">Kaç saniye sonra silineceği (varsayılan 3).</param>
        public void ShowInfo(string info, float displayDuration = 3f)
        {
            if (m_InfoText == null)
            {
                m_InfoText = GetComponentInChildren<TextMeshProUGUI>(true);
            }

            if (m_InfoText == null)
            {
                return;
            }

            if (m_ClearRoutine != null)
            {
                StopCoroutine(m_ClearRoutine);
            }

            string text = info ?? "";
            m_InfoText.text = text;
            m_InfoText.gameObject.SetActive(true);
            m_InfoText.enabled = true;

            if (displayDuration > 0f)
            {
                m_ClearRoutine = StartCoroutine(ClearAfter(displayDuration));
            }
        }

        private IEnumerator ClearAfter(float delay)
        {
            yield return new WaitForSeconds(delay);
            m_ClearRoutine = null;
            if (m_InfoText != null)
            {
                m_InfoText.text = "";
                m_InfoText.gameObject.SetActive(false);
            }
        }
    }
}

