using System;
using UnityEngine;
using UnityEngine.Events;
using InteractionSystem.Runtime.Core;

namespace InteractionSystem.Runtime.Interactables
{
    [Serializable]
    internal class PressurePlateState
    {
        public bool isPressed;
    }

    /// <summary>
    /// Üzerine basıldığında (OnTriggerEnter) tetiklenir; E tuşu ile değil, trigger ile çalışır. Lever gibi başka nesneleri tetikleyebilir.
    /// </summary>
    /// <remarks>
    /// Collider Is Trigger = true olmalı. "Player" tag'li obje üzerine gelince OnActivated, çıkınca OnDeactivated tetiklenir.
    /// Inspector'dan OnActivated/OnDeactivated'e dinleyici bağlanır (örn: kapı açma, trap).
    /// </remarks>
    [RequireComponent(typeof(Collider))]
    public class PressurePlate : Interactable
    {
        #region Fields

        [SerializeField] [Tooltip("Hangi tag basınca tetiklensin (varsayılan: Player).")]
        private string m_TriggerTag = "Player";

        private bool m_IsPressed;
        private int m_TriggerCount;

        #endregion

        #region Events

        /// <summary>
        /// Üzerine basıldığında tetiklenir. Inspector'dan dinleyici bağlanır (örn: Door.Toggle, trap). 
        /// </summary>
        public UnityEvent OnActivated;

        /// <summary>
        /// Üzerinden çıkıldığında tetiklenir.
        /// </summary>
        public UnityEvent OnDeactivated;

        #endregion

        #region Properties

        /// <summary>
        /// Plate şu an basılı mı?
        /// </summary>
        public bool IsPressed => m_IsPressed;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            var col = GetComponent<Collider>();
            if (col != null && !col.isTrigger)
            {
                col.isTrigger = true;
                Debug.LogWarning("[PressurePlate] Collider was not trigger; set to trigger.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(m_TriggerTag)) return;

            m_TriggerCount++;
            if (m_TriggerCount == 1)
            {
                m_IsPressed = true;
                PlayInteractionFeedback(true);
                OnActivated?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(m_TriggerTag)) return;

            m_TriggerCount = Mathf.Max(0, m_TriggerCount - 1);
            if (m_TriggerCount == 0)
            {
                m_IsPressed = false;
                PlayInteractionFeedback(false);
                OnDeactivated?.Invoke();
            }
        }

        #endregion

        #region Interactable Overrides

        /// <inheritdoc/>
        protected override bool IsInActiveState() => m_IsPressed;

        /// <inheritdoc/>
        protected override string GetClosedStateName() => "Idle";

        /// <inheritdoc/>
        /// <summary>Pressure plate E ile etkileşilmez; sadece trigger ile.</summary>
        public override bool CanInteract(IInteractor interactor) => false;

        /// <inheritdoc/>
        public override void Interact(IInteractor interactor) { }

        /// <inheritdoc/>
        public override string SerializeState()
        {
            return JsonUtility.ToJson(new PressurePlateState { isPressed = m_IsPressed });
        }

        /// <inheritdoc/>
        public override void LoadState(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            var s = JsonUtility.FromJson<PressurePlateState>(json);
            m_IsPressed = s.isPressed;
            m_TriggerCount = m_IsPressed ? 1 : 0;
            SyncAnimatorToState(m_IsPressed);
            SyncTransformToState(m_IsPressed);
        }

        #endregion
    }
}
