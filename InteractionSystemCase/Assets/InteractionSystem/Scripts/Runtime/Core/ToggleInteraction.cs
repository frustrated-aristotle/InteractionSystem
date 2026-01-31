using UnityEngine;

namespace InteractionSystem.Runtime.Core
{
    /// <summary>
    /// Açık/kapalı durumlar arasında geçiş yapan etkileşim.
    /// </summary>
    /// <remarks>
    /// Light switch, door toggle gibi senaryolar için kullanılır.
    /// Her Execute çağrısında state tersine çevrilir.
    /// </remarks>
    public class ToggleInteraction : Interaction
    {
        #region Fields

        private bool m_State;

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override InteractionType Type => InteractionType.Toggle;

        /// <summary>
        /// Mevcut toggle durumu (true = açık, false = kapalı).
        /// </summary>
        public bool State => m_State;

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override void Execute(IInteractor interactor)
        {
            if (interactor == null)
            {
                Debug.LogWarning("[ToggleInteraction] Execute called with null interactor.");
                return;
            }

            m_State = !m_State;
            OnExecute(interactor, m_State);
        }

        /// <summary>
        /// Toggle gerçekleştiğinde çağrılır. Türeyen sınıflar override edebilir.
        /// </summary>
        /// <param name="interactor">Etkileşimi başlatan Interactor.</param>
        /// <param name="newState">Yeni toggle durumu.</param>
        protected virtual void OnExecute(IInteractor interactor, bool newState)
        {
            Debug.Log($"[ToggleInteraction] Toggled: {newState}");
        }

        #endregion
    }
}
