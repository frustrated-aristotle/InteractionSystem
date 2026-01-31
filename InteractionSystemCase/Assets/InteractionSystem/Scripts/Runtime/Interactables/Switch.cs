using UnityEngine;
using UnityEngine.Events;
using InteractionSystem.Runtime.Core;

namespace InteractionSystem.Runtime.Interactables
{
    /// <summary>
    /// Toggle interaction ile başka nesneleri tetikleyen anahtar/kol.
    /// </summary>
    /// <remarks>
    /// OnToggle event'i Inspector'dan bağlanır (örn: door açma, platform hareketi).
    /// </remarks>
    public class Switch : Interactable
    {
        #region Fields

        private bool m_IsOn;

        #endregion

        #region Events

        /// <summary>
        /// Toggle edildiğinde tetiklenir. Inspector'dan dinleyiciler bağlanabilir.
        /// </summary>
        public UnityEvent OnToggle;

        #endregion

        #region Properties

        /// <summary>
        /// Kol şu an açık (on) durumda mı?
        /// </summary>
        public bool IsOn => m_IsOn;

        #endregion

        #region Interactable Overrides

        /// <inheritdoc/>
        protected override bool IsInActiveState() => m_IsOn;

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override bool CanInteract(IInteractor interactor) => true;

        /// <inheritdoc/>
        public override void Interact(IInteractor interactor)
        {
            m_IsOn = !m_IsOn;
            PlayInteractionFeedback(m_IsOn);
            OnToggle?.Invoke();
        }

        #endregion
    }
}
