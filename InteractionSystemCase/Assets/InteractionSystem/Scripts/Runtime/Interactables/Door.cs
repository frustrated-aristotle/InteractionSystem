using UnityEngine;
using InteractionSystem.Runtime.Core;

namespace InteractionSystem.Runtime.Interactables
{
    /// <summary>
    /// Açılıp kapanabilen kapı. Kilitli olabilir, anahtar gerektirebilir.
    /// Animator ile Open/Close trigger kullanır (base Interactable).
    /// </summary>
    /// <remarks>
    /// Toggle interaction type. Kilitli ise doğru anahtar envanterde olmalıdır.
    /// </remarks>
    public class Door : Interactable
    {
        #region Fields

        [SerializeField] private bool m_IsLocked = true;
        [SerializeField] private KeyItemData m_RequiredKey;

        [Header("Prompts")]
        [SerializeField] private string m_KeyRequiredPrompt = "Anahtar gerekli";

        private bool m_IsOpen;

        #endregion

        #region Properties

        /// <summary>
        /// Kapı şu an açık mı?
        /// </summary>
        public bool IsOpen => m_IsOpen;

        /// <summary>
        /// Kapı kilitli mi?
        /// </summary>
        public bool IsLocked => m_IsLocked;

        #endregion

        #region Public Methods

        /// <summary>
        /// Kapıyı açıp kapatır. Switch UnityEvent'inden çağrılabilir.
        /// </summary>
        public void Toggle()
        {
            m_IsOpen = !m_IsOpen;
            PlayInteractionFeedback(m_IsOpen);
            Debug.Log(m_IsOpen ? "[Door] Kapı açıldı (Switch)." : "[Door] Kapı kapatıldı (Switch).");
        }

        #endregion

        #region Interactable Overrides

        /// <inheritdoc/>
        protected override bool IsInActiveState() => m_IsOpen;

        /// <inheritdoc/>
        protected override string GetClosedStateName() => "Idle";

        /// <inheritdoc/>
        public override string GetUnableToInteractPrompt(IInteractor interactor)
        {
            if (m_IsLocked && m_RequiredKey != null && (interactor == null || !interactor.Inventory.HasItem(m_RequiredKey)))
            {
                return m_KeyRequiredPrompt;
            }

            return base.GetUnableToInteractPrompt(interactor);
        }

        /// <inheritdoc/>
        public override string GetInteractionPrompt()
        {
            return base.GetInteractionPrompt();
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override bool CanInteract(IInteractor interactor)
        {
            if (interactor == null)
            {
                return false;
            }

            if (!m_IsLocked)
            {
                return true;
            }
            return m_RequiredKey != null && interactor.Inventory.HasItem(m_RequiredKey);
        }

        /// <inheritdoc/>
        public override void Interact(IInteractor interactor)
        {
            if (interactor == null)
            {
                return;
            }

            if (m_IsLocked)
            {
                if (m_RequiredKey == null || !interactor.Inventory.HasItem(m_RequiredKey))
                {
                    Debug.Log("[Door] Anahtar gerekli!");
                    return;
                }

                m_IsLocked = false;
            }

            m_IsOpen = !m_IsOpen;
            PlayInteractionFeedback(m_IsOpen);
            Debug.Log(m_IsOpen ? "[Door] Kapı açıldı." : "[Door] Kapı kapatıldı.");
        }

        #endregion
    }
}
