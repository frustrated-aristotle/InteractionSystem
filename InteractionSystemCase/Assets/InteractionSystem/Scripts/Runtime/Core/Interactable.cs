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

        [Header("Prompts")]
        [SerializeField] [Tooltip("Gösterilir: nesne kapalı/kapalı durumundayken (örn: Press E to open).")]
        private string m_PromptPositive = "Press E to interact";

        [SerializeField] [Tooltip("Gösterilir: nesne açık/açık durumundayken (örn: Press E to close).")]
        private string m_PromptNegative = "Press E to interact";

        [SerializeField] [Tooltip("Etkileşim yapılamadığında gösterilir (örn: Anahtar gerekli).")]
        private string m_PromptUnable = "Cannot interact";

        [Header("Animation & Audio")]
        [SerializeField] [Tooltip("Boş bırakılırsa aynı GameObject'te aranır.")] private Animator m_Animator;
        [SerializeField] private string m_OpenTrigger = "Open";
        [SerializeField] private string m_CloseTrigger = "Close";
        [SerializeField] private AudioClip m_OpenSound;
        [SerializeField] private AudioClip m_CloseSound;
        [SerializeField] [Tooltip("Boş bırakılırsa GetComponent ile aranır.")] private AudioSource m_AudioSource;

        #endregion

        #region Properties

        /// <summary>
        /// Aktif olmayan durumda gösterilecek prompt (örn: açmak için).
        /// </summary>
        protected string PromptPositive => m_PromptPositive;

        /// <summary>
        /// Aktif durumda gösterilecek prompt (örn: kapatmak için).
        /// </summary>
        protected string PromptNegative => m_PromptNegative;

        /// <inheritdoc/>
        public virtual Transform InteractionPoint => transform;

        #endregion

        #region Unity Methods

        protected virtual void Awake()
        {
            if (m_Animator == null)
            {
                m_Animator = GetComponent<Animator>();
            }

            if (m_AudioSource == null)
            {
                m_AudioSource = GetComponent<AudioSource>();
                if (m_AudioSource == null && (m_OpenSound != null || m_CloseSound != null))
                {
                    m_AudioSource = gameObject.AddComponent<AudioSource>();
                }
            }
        }

        #endregion

        #region IInteractable Implementation

        /// <inheritdoc/>
        public virtual string GetInteractionPrompt()
        {
            return IsInActiveState() ? m_PromptNegative : m_PromptPositive;
        }

        /// <inheritdoc/>
        public virtual string GetUnableToInteractPrompt(IInteractor interactor)
        {
            return m_PromptUnable;
        }

        /// <summary>
        /// Nesnenin aktif durumda olup olmadığını döndürür (açık, çalışıyor vb.).
        /// Alt sınıflar override ederek kendi state mantığını sağlar.
        /// </summary>
        /// <returns>Aktif durumdaysa true (m_PromptNegative gösterilir), değilse false (m_PromptPositive).</returns>
        protected virtual bool IsInActiveState() => false;

        /// <inheritdoc/>
        public abstract bool CanInteract(IInteractor interactor);

        /// <inheritdoc/>
        public abstract void Interact(IInteractor interactor);

        #endregion

        #region Methods

        /// <summary>
        /// Etkileşim sonrası animasyon ve ses çalınır.
        /// Açma/kapama seslerinden biri yoksa diğeri kullanılır.
        /// </summary>
        /// <param name="isOpening">Açılıyorsa true, kapanıyorsa false.</param>
        protected void PlayInteractionFeedback(bool isOpening)
        {
            if (m_Animator != null)
            {
                string trigger = isOpening ? m_OpenTrigger : m_CloseTrigger;
                if (!string.IsNullOrEmpty(trigger))
                {
                    m_Animator.SetTrigger(trigger);
                }
            }

            if (m_AudioSource == null)
            {
                return;
            }

            AudioClip clip = isOpening ? m_OpenSound : m_CloseSound;
            if (clip == null)
            {
                clip = isOpening ? m_CloseSound : m_OpenSound;
            }

            if (clip != null)
            {
                m_AudioSource.PlayOneShot(clip);
            }
        }

        #endregion
    }
}
