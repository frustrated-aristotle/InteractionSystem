using UnityEngine;

namespace InteractionSystem.Runtime.Core
{
    /// <summary>
    /// Etkileşim türleri.
    /// </summary>
    public enum InteractionType
    {
        /// <summary>Tek tuş basımı ile anında gerçekleşir.</summary>
        Instant,

        /// <summary>Belirli süre basılı tutma gerektirir.</summary>
        Hold,

        /// <summary>Açık/kapalı durumlar arasında geçiş yapar.</summary>
        Toggle
    }

    /// <summary>
    /// Etkileşim davranışları için abstract base class.
    /// </summary>
    /// <remarks>
    /// Instant, Hold ve Toggle türleri bu sınıftan türetilir.
    /// Interactable nesneler bu component'leri kullanarak etkileşim mantığını uygular.
    /// </remarks>
    public abstract class Interaction : MonoBehaviour
    {
        #region Properties

        /// <summary>
        /// Bu etkileşimin türü. Interactor input işleme stratejisini belirler.
        /// </summary>
        public abstract InteractionType Type { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Etkileşimi gerçekleştirir.
        /// </summary>
        /// <param name="interactor">Etkileşimi başlatan Interactor referansı.</param>
        /// <remarks>
        /// Instant/Toggle: Tek seferlik çağrılır.
        /// Hold: Her frame çağrılır (tuş basılı iken), tamamlanınca OnHoldComplete tetiklenir.
        /// </remarks>
        public abstract void Execute(IInteractor interactor);

        #endregion
    }
}
