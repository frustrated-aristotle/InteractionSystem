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
        [SerializeField] [Tooltip("Prompt başlığında gösterilen obje adı.")]
        private string m_ObjectName = "";
        [SerializeField] [Tooltip("Gösterilir: nesne kapalı/kapalı durumundayken (örn: Press E to open).")]
        private string m_PromptPositive = "Press E to interact";

        [SerializeField] [Tooltip("Gösterilir: nesne açık/açık durumundayken (örn: Press E to close).")]
        private string m_PromptNegative = "Press E to interact";

        [SerializeField] [Tooltip("Etkileşim yapılamadığında gösterilir (örn: Anahtar gerekli).")]
        private string m_PromptUnable = "Cannot interact";

        [Header("Save/Load")]
        [SerializeField] [Tooltip("Kayıt için benzersiz ID. Boşsa gameObject.name kullanılır.")]
        private string m_SaveId = "";

        [Header("Open/Closed Rotation (Load)")]
        [SerializeField] [Tooltip("Açık/kapalı rotasyonları yüklemede kullanılsın mı? Kapalı/Açık Rotation'ları doldur.")]
        private bool m_UseOpenClosedRotation = true;
        [SerializeField] [Tooltip("Kapalı (normal) pozisyon – Local Euler (X,Y,Z). Yüklemede kapalı state için uygulanır.")]
        private Vector3 m_ClosedRotationEuler = Vector3.zero;
        [SerializeField] [Tooltip("Açık (ikinci) pozisyon – Local Euler (X,Y,Z). Yüklemede açık state için uygulanır.")]
        private Vector3 m_OpenRotationEuler = Vector3.zero;
        [SerializeField] [Tooltip("Rotasyon uygulanacak transform. Boşsa bu objenin transform'u kullanılır.")]
        private Transform m_RotationTarget;

        [Header("Animation & Audio")]
        [SerializeField] [Tooltip("Boş bırakılırsa aynı GameObject'te aranır.")] private Animator m_Animator;
        [SerializeField] [Tooltip("Başlangıçta kapalı pozu göstermek için kapanma state adı (örn: A_Door_Close). Boşsa atlanır.")]
        private string m_ClosedStateName = "";
        [SerializeField] [Tooltip("LoadState sonrası açık pozu göstermek için açılma state adı (örn: A_Lever_Open). Boşsa sync atlanır.")]
        private string m_OpenStateName = "";
        [SerializeField] private string m_OpenTrigger = "Open";
        [SerializeField] private string m_CloseTrigger = "Close";
        [SerializeField] [Tooltip("Etkileşimde animasyon parametreleri ve state debug loglansın mı?")]
        private bool m_LogAnimationDebug = true;
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

        /// <inheritdoc/>
        public virtual string ObjectName => string.IsNullOrEmpty(m_ObjectName) ? gameObject.name : m_ObjectName;

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

            // Başlangıçta kapalı state son karesi (Open/Close Trigger olduğu için Awake'te set edilmez)
            if (m_Animator != null)
            {
                string closedState = GetClosedStateName();
                if (!string.IsNullOrEmpty(closedState))
                {
                    m_Animator.Play(closedState, 0, 1f);
                }
            }

            if (m_UseOpenClosedRotation)
            {
                SyncTransformToState(false);
            }
        }

        /// <summary>
        /// Başlangıçta gösterilecek kapalı state adı (kapanma animasyonunun son karesi).
        /// Alt sınıflar override ederek kendi state adını döner (örn: A_Door_Close).
        /// </summary>
        protected virtual string GetClosedStateName() => m_ClosedStateName;

        /// <summary>
        /// LoadState sonrası açık pozu göstermek için kullanılan state adı. Alt sınıflar override edebilir.
        /// </summary>
        protected virtual string GetOpenStateName() => m_OpenStateName;

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

        #region Save/Load

        /// <summary>
        /// Kayıt/yükleme için benzersiz ID. Varsayılan: gameObject.name.
        /// </summary>
        public virtual string GetSaveId() => string.IsNullOrEmpty(m_SaveId) ? gameObject.name : m_SaveId;

        /// <summary>
        /// Mevcut state'i JSON string olarak döndürür. Alt sınıflar override eder.
        /// </summary>
        public virtual string SerializeState() => "";

        /// <summary>
        /// Kayıtlı state'i uygular. Alt sınıflar override eder.
        /// </summary>
        /// <param name="json">SerializeState ile üretilmiş JSON.</param>
        public virtual void LoadState(string json) { }

        #endregion

        #region Methods

        /// <summary>
        /// Etkileşim sonrası animasyon ve ses çalınır.
        /// Open/Close parametreleri Trigger olarak kullanılır (SetTrigger).
        /// Açma/kapama seslerinden biri yoksa diğeri kullanılır.
        /// </summary>
        /// <param name="isOpening">Açılıyorsa true, kapanıyorsa false.</param>
        protected void PlayInteractionFeedback(bool isOpening)
        {
            if (m_Animator == null)
            {
                if (m_LogAnimationDebug)
                {
                    Debug.Log($"[Interactable] {gameObject.name} PlayInteractionFeedback: Animator yok, atlanıyor. isOpening={isOpening}");
                }

                PlayInteractionSound(isOpening);
                return;
            }

            if (isOpening)
            {
                m_Animator.SetTrigger(m_OpenTrigger);
            }
            else
            {
                m_Animator.SetTrigger(m_CloseTrigger);
            }

            if (m_LogAnimationDebug)
            {
                LogAnimationDebug(isOpening);
            }

            PlayInteractionSound(isOpening);
        }

        /// <summary>
        /// Kayıt yüklendiğinde açık/kapalı rotasyonu uygular. Inspector'daki Closed/Open Rotation (Euler) kullanılır.
        /// LoadState override'larında state flag'leri set edildikten sonra çağrılmalı.
        /// </summary>
        /// <param name="isActive">Açık ise true (Open Rotation), kapalı ise false (Closed Rotation).</param>
        protected void SyncTransformToState(bool isActive)
        {
            if (!m_UseOpenClosedRotation) return;

            Transform target = m_RotationTarget != null ? m_RotationTarget : transform;
            target.localRotation = Quaternion.Euler(isActive ? m_OpenRotationEuler : m_ClosedRotationEuler);
        }

        /// <summary>
        /// Kayıt yüklendiğinde animatorı mantıksal durumla senkronize eder (geçiş oynatmadan doğrudan state son karesi).
        /// LoadState override'larında state flag'leri set edildikten sonra çağrılmalı.
        /// </summary>
        /// <param name="isActive">Açık/toggled ise true, kapalı ise false.</param>
        protected void SyncAnimatorToState(bool isActive)
        {
            if (m_Animator == null)
            {
                Debug.LogWarning($"[Interactable] {gameObject.name}: Animator is null; SyncAnimatorToState skipped.");
                return;
            }

            if (isActive)
            {
                string openState = GetOpenStateName();
                if (!string.IsNullOrEmpty(openState))
                {
                    m_Animator.Play(openState, 0, 1f);
                }
            }
            else
            {
                string closedState = GetClosedStateName();
                if (!string.IsNullOrEmpty(closedState))
                {
                    m_Animator.Play(closedState, 0, 1f);
                }
            }
        }

        /// <summary>
        /// Animasyon parametreleri ve mevcut state için debug log yazar.
        /// </summary>
        private void LogAnimationDebug(bool isOpening)
        {
            string controllerName = m_Animator.runtimeAnimatorController != null
                ? m_Animator.runtimeAnimatorController.name
                : "(controller yok)";

            AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
            string paramDetail = isOpening ? $"SetTrigger('{m_OpenTrigger}')" : $"SetTrigger('{m_CloseTrigger}')";

            Debug.Log($"[Interactable] {gameObject.name} etkileşim -> PlayInteractionFeedback | isOpening={isOpening} | " +
                      $"Parametre tipi=Trigger | {paramDetail} | " +
                      $"Animator controller='{controllerName}' | " +
                      $"Animator state: layer=0 shortNameHash={stateInfo.shortNameHash} normalizedTime={stateInfo.normalizedTime:F2} fullPathHash={stateInfo.fullPathHash}");
        }

        /// <summary>
        /// Etkileşim sesini çalar. Açma/kapama seslerinden biri yoksa diğeri kullanılır.
        /// </summary>
        private void PlayInteractionSound(bool isOpening)
        {
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
