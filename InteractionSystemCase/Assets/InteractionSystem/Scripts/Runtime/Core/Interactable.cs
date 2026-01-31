using UnityEngine;

namespace InteractionSystem.Runtime.Core
{
    /// <summary>
    /// IInteractable implementasyonu için abstract base class.
    /// </summary>
    /// <remarks>
    /// Tüm etkileşimli nesneler (Door, Chest, Switch, KeyPickup vb.) bu sınıftan türetilir.
    /// Prompt ve InteractionPoint için varsayılan değerler sağlar.
    /// </remarks>
    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        #region Fields

        [SerializeField] private string m_Prompt = "Interact";

        #endregion

        #region Properties

        /// <summary>
        /// UI'da gösterilecek prompt metni.
        /// </summary>
        public string Prompt => m_Prompt;

        /// <inheritdoc/>
        public virtual Transform InteractionPoint => transform;

        #endregion

        #region IInteractable Implementation

        /// <inheritdoc/>
        public virtual string GetInteractionPrompt() => m_Prompt;

        /// <inheritdoc/>
        public abstract bool CanInteract(IInteractor interactor);

        /// <inheritdoc/>
        public abstract void Interact(IInteractor interactor);

        #endregion
    }
}
