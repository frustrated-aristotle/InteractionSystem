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
                bool canInteract = m_Interactor != null && currentTarget.CanInteract(m_Interactor);
                m_PromptText.text = canInteract
                    ? currentTarget.GetInteractionPrompt()
                    : currentTarget.GetUnableToInteractPrompt(m_Interactor);
            }

            if (m_ProgressBarFill != null)
            {
                bool showProgress = m_Interactor.IsHolding;
                m_ProgressBarFill.gameObject.SetActive(showProgress);

                if (showProgress)
                {
                    m_ProgressBarFill.type = Image.Type.Filled;
                    m_ProgressBarFill.fillMethod = Image.FillMethod.Horizontal;
                    m_ProgressBarFill.fillAmount = m_Interactor.HoldProgress;
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
