using System;
using UnityEngine;
using InteractionSystem.Runtime.Core;

namespace InteractionSystem.Runtime.Interactables
{
    [Serializable]
    internal class DoorState
    {
        public bool isOpen;
        public bool isLocked;
    }

    /// <summary>
    /// Açılıp kapanabilen kapı. Kilitli ise sadece atanmış KeyItemData tipindeki anahtar açar.
    /// Animator ile Open/Close trigger kullanır (base Interactable).
    /// </summary>
    /// <remarks>
    /// Bir kapı sadece tek bir anahtar tipi (bir KeyItemData asset'i) ile açılır.
    /// Eşleşme: Interactor envanterinde m_RequiredKey ile aynı ScriptableObject referansı varsa kapı açılır.
    /// Birden fazla kapı için farklı KeyItemData atanarak color-coded / çoklu anahtar tipi kullanılır.
    /// </remarks>
    public class Door : Interactable
    {
        #region Fields

        [SerializeField] private bool m_IsLocked = true;
        [Tooltip("Bu kapıyı açan tek anahtar tipi. Sadece bu KeyItemData envanterde varsa kapı açılır.")]
        [SerializeField] private KeyItemData m_RequiredKey;

        [Header("Prompts")]
        [SerializeField] private string m_KeyRequiredPrompt = "Anahtar gerekli";
        [SerializeField] [Tooltip("True ise prompt'ta anahtar adı gösterilir (örn: 'Rusty Key gerekli').")]
        private bool m_ShowKeyNameInPrompt = true;

        private bool m_IsOpen;

        #endregion

        #region Properties

        /// <summary>
        /// Kapı şu an açık mı?
        /// </summary>
        public bool IsOpen => m_IsOpen;

        /// <summary>
        /// Kapı kilitli mi?
        /// </summary>
        public bool IsLocked => m_IsLocked;

        /// <summary>
        /// Bu kapıyı açan anahtar tipi. Bir kapı sadece bu KeyItemData ile açılır.
        /// </summary>
        public KeyItemData RequiredKey => m_RequiredKey;

        #endregion

        #region Public Methods

        /// <summary>
        /// Verilen anahtar tipi bu kapıyı açar mı? (Aynı KeyItemData referansı ise true.)
        /// </summary>
        /// <param name="key">Kontrol edilecek anahtar tipi.</param>
        /// <returns>Aynı asset ise true.</returns>
        public bool DoesKeyUnlock(KeyItemData key)
        {
            return m_RequiredKey != null && key == m_RequiredKey;
        }

        /// <summary>
        /// Kapıyı açıp kapatır. Switch UnityEvent'inden çağrılabilir.
        /// </summary>
        public void Toggle()
        {
            m_IsOpen = !m_IsOpen;
            PlayInteractionFeedback(m_IsOpen);
            Debug.Log(m_IsOpen ? "[Door] Kapı açıldı (Switch)." : "[Door] Kapı kapatıldı (Switch).");
        }

        #endregion

        #region Interactable Overrides

        /// <inheritdoc/>
        protected override bool IsInActiveState() => m_IsOpen;

        /// <inheritdoc/>
        protected override string GetClosedStateName() => "Idle";

        /// <inheritdoc/>
        public override string SerializeState()
        {
            return JsonUtility.ToJson(new DoorState { isOpen = m_IsOpen, isLocked = m_IsLocked });
        }

        /// <inheritdoc/>
        public override void LoadState(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning($"[Door] LoadState: empty or null json for id={GetSaveId()}.");
                return;
            }
            var s = JsonUtility.FromJson<DoorState>(json);
            m_IsOpen = s.isOpen;
            m_IsLocked = s.isLocked;
            SyncAnimatorToState(m_IsOpen);
        }

        /// <inheritdoc/>
        public override string GetUnableToInteractPrompt(IInteractor interactor)
        {
            if (m_IsLocked && m_RequiredKey != null && (interactor == null || interactor.Inventory == null || !interactor.Inventory.HasKey(m_RequiredKey)))
            {
                if (m_ShowKeyNameInPrompt)
                {
                    return $"{m_RequiredKey.KeyName} gerekli";
                }

                return m_KeyRequiredPrompt;
            }

            return base.GetUnableToInteractPrompt(interactor);
        }

        /// <inheritdoc/>
        public override string GetInteractionPrompt()
        {
            return base.GetInteractionPrompt();
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override bool CanInteract(IInteractor interactor)
        {
            if (interactor == null || interactor.Inventory == null)
            {
                return false;
            }

            if (!m_IsLocked)
            {
                return true;
            }

            return m_RequiredKey != null && interactor.Inventory.HasKey(m_RequiredKey);
        }

        /// <inheritdoc/>
        public override void Interact(IInteractor interactor)
        {
            if (interactor == null || interactor.Inventory == null)
            {
                Debug.LogWarning("[Door] Interact: interactor or Inventory is null.");
                return;
            }

            if (m_IsLocked)
            {
                if (m_RequiredKey == null || !interactor.Inventory.HasKey(m_RequiredKey))
                {
                    Debug.Log($"[Door] Anahtar gerekli! Gerekli: {m_RequiredKey?.KeyName ?? "?"}");
                    return;
                }

                m_IsLocked = false;
            }

            m_IsOpen = !m_IsOpen;
            PlayInteractionFeedback(m_IsOpen);
            if (m_IsOpen)
            {
                interactor.ShowItemInfo(ItemInfoKind.DoorOpened, ObjectName, 3f);
            }
            Debug.Log(m_IsOpen ? "[Door] Kapı açıldı." : "[Door] Kapı kapatıldı.");
        }

        #endregion
    }
}
