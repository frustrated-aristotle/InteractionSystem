using UnityEngine;

namespace InteractionSystem.Runtime.Core
{
    /// <summary>
    /// Tek tuş basımı ile anında gerçekleşen etkileşim.
    /// </summary>
    /// <remarks>
    /// Pickup item, button press gibi senaryolar için kullanılır.
    /// Execute bir kez çağrıldığında hemen tamamlanır.
    /// </remarks>
    public class InstantInteraction : Interaction
    {
        #region Properties

        /// <inheritdoc/>
        public override InteractionType Type => InteractionType.Instant;

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override void Execute(IInteractor interactor)
        {
            if (interactor == null)
            {
                Debug.LogWarning("[InstantInteraction] Execute called with null interactor.");
                return;
            }

            OnExecute(interactor);
        }

        /// <summary>
        /// Anında etkileşim mantığı. Türeyen sınıflar veya Inspector'dan atanacak event ile override edilebilir.
        /// </summary>
        /// <param name="interactor">Etkileşimi başlatan Interactor.</param>
        protected virtual void OnExecute(IInteractor interactor)
        {
            Debug.Log("[InstantInteraction] Execute completed.");
        }

        #endregion
    }
}
