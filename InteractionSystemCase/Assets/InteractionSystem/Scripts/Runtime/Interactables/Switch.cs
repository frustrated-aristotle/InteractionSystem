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
        #region Events

        /// <summary>
        /// Toggle edildiğinde tetiklenir. Inspector'dan dinleyiciler bağlanabilir.
        /// </summary>
        public UnityEvent OnToggle;

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override bool CanInteract(IInteractor interactor) => true;

        /// <inheritdoc/>
        public override void Interact(IInteractor interactor)
        {
            OnToggle?.Invoke();
        }

        #endregion
    }
}
