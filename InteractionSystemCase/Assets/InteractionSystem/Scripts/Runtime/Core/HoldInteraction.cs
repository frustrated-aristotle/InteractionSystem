using System;
using UnityEngine;

namespace InteractionSystem.Runtime.Core
{
    /// <summary>
    /// Belirli süre basılı tutma gerektiren etkileşim.
    /// </summary>
    /// <remarks>
    /// Chest açma, kapı kilidi kırma gibi senaryolar için kullanılır.
    /// Execute her frame çağrılmalıdır (tuş basılı iken). Progress 0-1 aralığında UI progress bar'a bağlanabilir.
    /// Tuş bırakıldığında Reset() çağrılmalıdır.
    /// </remarks>
    public class HoldInteraction : Interaction
    {
        #region Fields

        [SerializeField] private float m_HoldTime = 2f;

        private float m_Timer;

        #endregion

        #region Events

        /// <summary>
        /// Hold süresi tamamlandığında tetiklenir. UI progress bar ve sonuç işlemleri için kullanılır.
        /// </summary>
        public event Action OnHoldComplete;

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override InteractionType Type => InteractionType.Hold;

        /// <summary>
        /// Mevcut hold ilerlemesi (0-1). UI progress bar bu property'e bağlanır.
        /// </summary>
        public float Progress => m_HoldTime > 0 ? Mathf.Clamp01(m_Timer / m_HoldTime) : 1f;

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override void Execute(IInteractor interactor)
        {
            if (interactor == null)
            {
                Debug.LogWarning("[HoldInteraction] Execute called with null interactor.");
                return;
            }

            m_Timer += Time.deltaTime;

            if (m_Timer >= m_HoldTime)
            {
                m_Timer = 0f;
                OnExecute(interactor);
                OnHoldComplete?.Invoke();
            }
        }

        /// <summary>
        /// Hold tamamlandığında çağrılır. Türeyen sınıflar veya event ile override edilebilir.
        /// </summary>
        /// <param name="interactor">Etkileşimi başlatan Interactor.</param>
        protected virtual void OnExecute(IInteractor interactor)
        {
            Debug.Log("[HoldInteraction] Hold completed.");
        }

        /// <summary>
        /// Hold ilerlemesini sıfırlar. Tuş bırakıldığında Interactor tarafından çağrılmalıdır.
        /// </summary>
        public void ResetProgress()
        {
            m_Timer = 0f;
        }

        #endregion
    }
}
