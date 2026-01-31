using UnityEngine;
using UnityEngine.UI;
using InteractionSystem.Runtime.Core;
using InteractionSystem.Runtime.Player;
using TMPro;

namespace InteractionSystem.Runtime.UI
{
    /// <summary>
    /// Etkileşim prompt ve hold progress bar UI'ını yönetir.
    /// </summary>
    /// <remarks>
    /// Interactor.CurrentTarget değişince prompt güncellenir.
    /// Out of range veya hedef yokken gizlenir.
    /// Hold etkileşiminde progress bar gösterilir.
    /// Hiyerarşi: InteractionPrompt script parent'ta, PromptRoot child olmalı (SetActive ile script kapanmasın).
    /// </remarks>
    public class InteractionPrompt : MonoBehaviour
    {
        #region Fields

        [Header("References")]
        [SerializeField] [Tooltip("Boş bırakılırsa sahnede aranır.")] private Interactor m_Interactor;
        [SerializeField] [Tooltip("Göster/gizle için. CHILD olmalı - script parent'ta kalır.")] private GameObject m_PromptRoot;
        [SerializeField] private TextMeshProUGUI m_PromptText;
        [SerializeField] [Tooltip("Hold progress bar fill image. Image Type = Filled olmalı.")] private Image m_ProgressBarFill;

        [Header("Out of Range")]
        [SerializeField] [Tooltip("Hedef menzil dışındayken gösterilir.")] private string m_OutOfRangeText = "Out of range";

        [Header("Progress Bar Colors")]
        [SerializeField] [Tooltip("Hold başlangıcında progress bar rengi.")] private Color m_ProgressBarColorStart = new Color(0.8f, 0.2f, 0.2f);
        [SerializeField] [Tooltip("Hold tamamlanmaya yaklaşırken (yeşil).")] private Color m_ProgressBarColorEnd = new Color(0.2f, 0.8f, 0.2f);

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (m_Interactor == null)
            {
                m_Interactor = FindAnyObjectByType<Interactor>();
                if (m_Interactor == null)
                {
                    Debug.LogWarning("[InteractionPrompt] Interactor not assigned and none found in scene.");
                }
            }
        }

        private void Update()
        {
            RefreshUI();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prompt ve progress bar UI'ını günceller.
        /// </summary>
        private void RefreshUI()
        {
            if (m_Interactor == null)
            {
                SetVisible(false);
                return;
            }

            IInteractable currentTarget = m_Interactor.CurrentTarget;

            if (currentTarget == null)
            {
                SetVisible(false);
                return;
            }

            SetVisible(true);

            if (m_PromptText != null)
            {
                bool inRange = m_Interactor.IsTargetInRange;
                string prompt;
                if (!inRange)
                {
                    prompt = string.IsNullOrEmpty(m_OutOfRangeText) ? "Out of range" : m_OutOfRangeText;
                }
                else
                {
                    bool canInteract = currentTarget.CanInteract(m_Interactor);
                    prompt = canInteract
                        ? currentTarget.GetInteractionPrompt()
                        : currentTarget.GetUnableToInteractPrompt(m_Interactor);
                }
                string objectName = currentTarget.ObjectName ?? "";
                m_PromptText.text = string.IsNullOrEmpty(objectName) ? prompt : $"{objectName}\n{prompt}";
            }

            if (m_ProgressBarFill != null)
            {
                bool showProgress = m_Interactor.IsHolding;
                m_ProgressBarFill.gameObject.SetActive(showProgress);

                if (showProgress)
                {
                    m_ProgressBarFill.type = Image.Type.Filled;
                    m_ProgressBarFill.fillMethod = Image.FillMethod.Horizontal;
                    float progress = m_Interactor.HoldProgress;
                    m_ProgressBarFill.fillAmount = progress;
                    m_ProgressBarFill.color = Color.Lerp(m_ProgressBarColorStart, m_ProgressBarColorEnd, progress);
                }
            }
        }

        /// <summary>
        /// Prompt root görünürlüğünü ayarlar.
        /// </summary>
        /// <param name="visible">Gösterilecekse true.</param>
        private void SetVisible(bool visible)
        {
            if (m_PromptRoot != null)
            {
                m_PromptRoot.SetActive(visible);
            }
        }

        #endregion
    }
}
