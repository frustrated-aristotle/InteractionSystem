using UnityEngine;
using InteractionSystem.Runtime.Core;

namespace InteractionSystem.Runtime.Interactables
{
    /// <summary>
    /// Hold interaction ile açılan sandık.
    /// </summary>
    /// <remarks>
    /// HoldInteraction component gerektirir. UI progress bar Interactor.HoldProgress'e bağlanır.
    /// Açıldıktan sonra tekrar etkileşime girilemez.
    /// </remarks>
    [RequireComponent(typeof(HoldInteraction))]
    public class Chest : Interactable
    {
        #region Fields

        private bool m_IsOpened;
        private HoldInteraction m_HoldInteraction;

        #endregion

        #region Properties

        /// <summary>
        /// Sandık açıldı mı?
        /// </summary>
        public bool IsOpened => m_IsOpened;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            m_HoldInteraction = GetComponent<HoldInteraction>();

            if (m_HoldInteraction == null)
            {
                Debug.LogError("[Chest] HoldInteraction component not found.");
                return;
            }

            m_HoldInteraction.OnHoldComplete += HandleHoldComplete;
        }

        private void OnDestroy()
        {
            if (m_HoldInteraction != null)
            {
                m_HoldInteraction.OnHoldComplete -= HandleHoldComplete;
            }
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override bool CanInteract(IInteractor interactor) => !m_IsOpened;

        /// <inheritdoc/>
        public override void Interact(IInteractor interactor)
        {
        }

        /// <summary>
        /// Hold tamamlandığında sandığı açar.
        /// </summary>
        private void HandleHoldComplete()
        {
            m_IsOpened = true;
            Debug.Log("[Chest] Sandık açıldı!");
        }

        #endregion
    }
}
