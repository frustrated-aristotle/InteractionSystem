using UnityEngine;
using InteractionSystem.Runtime.Core;

namespace InteractionSystem.Runtime.Interactables
{
    /// <summary>
    /// Instant interaction ile toplanabilen anahtar. Envantere eklenir.
    /// </summary>
    /// <remarks>
    /// KeyItemData referansı ile hangi anahtar tipinin verileceği belirlenir.
    /// Farklı kapılar için farklı KeyItemData asset'leri kullanılabilir.
    /// Toplandıktan sonra nesne devre dışı bırakılır (tekrar toplanamaz).
    /// </remarks>
    public class KeyPickup : Interactable
    {
        #region Fields

        [SerializeField] private KeyItemData m_KeyData;
        [SerializeField] [Tooltip("Toplandıktan sonra nesne destroy edilsin mi? False ise SetActive(false) uygulanır.")]
        private bool m_DestroyOnPickup = false;

        private bool m_IsPickedUp;

        #endregion

        #region Properties

        /// <summary>
        /// Bu anahtarın verdiği KeyItemData. Kapı kilidi eşleşmesi için kullanılır.
        /// </summary>
        public KeyItemData KeyData => m_KeyData;

        /// <summary>
        /// Anahtar toplandı mı?
        /// </summary>
        public bool IsPickedUp => m_IsPickedUp;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            base.Awake();

            if (m_KeyData == null)
            {
                Debug.LogWarning($"[KeyPickup] KeyData atanmamış: {gameObject.name}. Inspector'dan bir KeyItemData asset'i atayın.");
            }
        }

        #endregion

        #region Interactable Overrides

        /// <inheritdoc/>
        public override string GetInteractionPrompt()
        {
            if (m_IsPickedUp)
            {
                return base.GetInteractionPrompt();
            }

            string keyName = m_KeyData != null ? m_KeyData.KeyName : "Key";
            return $"Press E to take {keyName}";
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override bool CanInteract(IInteractor interactor)
        {
            return interactor != null && !m_IsPickedUp && m_KeyData != null;
        }

        /// <inheritdoc/>
        public override void Interact(IInteractor interactor)
        {
            if (interactor == null || m_IsPickedUp || m_KeyData == null)
            {
                return;
            }

            interactor.Inventory.AddItem(m_KeyData);
            PlayInteractionFeedback(true);
            m_IsPickedUp = true;

            if (m_DestroyOnPickup)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }

            Debug.Log($"[KeyPickup] {m_KeyData.KeyName} alındı.");
        }

        #endregion
    }
}
