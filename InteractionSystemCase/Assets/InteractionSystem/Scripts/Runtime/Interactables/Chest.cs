using System;
using UnityEngine;
using InteractionSystem.Runtime.Core;

namespace InteractionSystem.Runtime.Interactables
{
    [Serializable]
    internal class ChestState
    {
        public bool consumed;
    }

    /// <summary>
    /// Hold interaction ile açılan sandık. Bir kez açıldıktan sonra tekrar etkileşime girilemez.
    /// </summary>
    /// <remarks>
    /// HoldInteraction component gerektirir. UI progress bar Interactor.HoldProgress'e bağlanır.
    /// İlk açılışta içindeki item verilir ve info gösterilir; sonrasında "Already opened" ve etkileşim kapanır.
    /// </remarks>
    [RequireComponent(typeof(HoldInteraction))]
    public class Chest : Interactable
    {
        #region Fields

        [SerializeField] [Tooltip("Kapalıyken ve daha önce açıldıysa prompt altında detay olarak gösterilir.")]
        private string m_AlreadyOpenedPrompt = "Already opened";
        [SerializeField] [Tooltip("Sandık açıldığında verilecek item (KeyItemData). Boşsa sadece açılır.")]
        private KeyItemData m_ItemInside;

        private bool m_IsOpened;
        private bool m_WasOpenedBefore;
        private bool m_Consumed;
        private HoldInteraction m_HoldInteraction;

        #endregion

        #region Properties

        /// <summary>
        /// Sandık şu an açık mı?
        /// </summary>
        public bool IsOpened => m_IsOpened;

        /// <summary>
        /// Sandıktan verilecek item (KeyItemData). Save/Load eşlemesi için kullanılır.
        /// </summary>
        public KeyItemData ItemInside => m_ItemInside;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            base.Awake();
            m_HoldInteraction = GetComponent<HoldInteraction>();

            if (m_HoldInteraction == null)
            {
                Debug.LogError("[Chest] HoldInteraction component not found.");
                return;
            }

            m_HoldInteraction.OnHoldComplete += HandleHoldComplete;
        }

        private void OnDestroy()
        {
            if (m_HoldInteraction != null)
            {
                m_HoldInteraction.OnHoldComplete -= HandleHoldComplete;
            }
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override string GetUnableToInteractPrompt(IInteractor interactor) => m_AlreadyOpenedPrompt;

        /// <inheritdoc/>
        protected override string GetClosedStateName() => "Idle";

        /// <inheritdoc/>
        protected override bool IsInActiveState() => m_IsOpened;

        /// <inheritdoc/>
        public override bool CanInteract(IInteractor interactor) => !m_Consumed;

        /// <inheritdoc/>
        public override string SerializeState()
        {
            return JsonUtility.ToJson(new ChestState { consumed = m_Consumed });
        }

        /// <inheritdoc/>
        public override void LoadState(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            var s = JsonUtility.FromJson<ChestState>(json);
            m_Consumed = s.consumed;
            m_IsOpened = s.consumed;
            m_WasOpenedBefore = s.consumed;
            if (m_HoldInteraction != null) m_HoldInteraction.enabled = !s.consumed;
            SyncAnimatorToState(m_IsOpened);
        }

        /// <inheritdoc/>
        public override string GetInteractionPrompt()
        {
            if (m_Consumed)
            {
                return base.GetInteractionPrompt();
            }
            string main = base.GetInteractionPrompt();
            if (!m_IsOpened && m_WasOpenedBefore && !string.IsNullOrEmpty(m_AlreadyOpenedPrompt))
            {
                return main + "\n" + m_AlreadyOpenedPrompt;
            }
            return main;
        }

        /// <inheritdoc/>
        public override void Interact(IInteractor interactor)
        {
            if (m_Consumed)
            {
                return;
            }
        }

        /// <summary>
        /// Hold tamamlandığında sandığı açar; içindeki item envantere eklenir ve info gösterilir. Açıkken kapatmak için Hold devre dışı bırakılır.
        /// </summary>
        private void HandleHoldComplete(IInteractor interactor)
        {
            m_IsOpened = true;
            m_WasOpenedBefore = true;
            m_Consumed = true;
            if (m_HoldInteraction != null)
            {
                m_HoldInteraction.enabled = false;
            }
            PlayInteractionFeedback(true);

            if (m_ItemInside != null && interactor != null)
            {
                if (interactor.Inventory != null)
                {
                    interactor.Inventory.AddItem(m_ItemInside);
                }
                interactor.ShowItemInfo(ItemInfoKind.ChestItemReceived, m_ItemInside.KeyName, 3f);
            }

            Debug.Log("[Chest] Sandık açıldı!");
        }

        #endregion
    }
}
