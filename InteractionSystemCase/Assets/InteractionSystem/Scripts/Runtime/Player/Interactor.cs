using UnityEngine;
using InteractionSystem.Runtime.Core;

namespace InteractionSystem.Runtime.Player
{
    /// <summary>
    /// Oyuncunun etkileşim kaynağını temsil eden bileşen.
    /// </summary>
    /// <remarks>
    /// Physics.OverlapSphere ile menzildeki IInteractable nesneleri tespit eder,
    /// en yakın ve etkileşime uygun olanı seçer (single interaction point).
    /// Hibrit genişletme: Raycast desteği InteractionDetector eklendiğinde eklenebilir.
    /// </remarks>
    [RequireComponent(typeof(Inventory))]
    public class Interactor : MonoBehaviour, IInteractor
    {
        #region Fields

        [SerializeField] private float m_InteractionRange = 2f;
        [SerializeField] private KeyCode m_InteractKey = KeyCode.E;

        private Inventory m_Inventory;
        private IInteractable m_CurrentTarget;

        #endregion

        #region Properties

        /// <summary>
        /// Bu interactor'a ait envanter. Key toplama ve kapı kilidi kontrolü için kullanılır.
        /// </summary>
        public IInventory Inventory => m_Inventory;

        /// <summary>
        /// Şu an hedeflenen etkileşim nesnesi. UI prompt için kullanılır.
        /// </summary>
        public IInteractable CurrentTarget => m_CurrentTarget;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            m_Inventory = GetComponent<Inventory>();
            if (m_Inventory == null)
            {
                Debug.LogError($"[Interactor] Inventory component not found on {gameObject.name}. Add Inventory to the same GameObject.");
            }
        }

        private void Update()
        {
            FindBestInteractable();

            if (m_CurrentTarget != null && Input.GetKeyDown(m_InteractKey))
            {
                m_CurrentTarget.Interact(this);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Menzildeki IInteractable nesneler arasından en yakın ve etkileşime uygun olanı seçer.
        /// </summary>
        private void FindBestInteractable()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, m_InteractionRange);

            float closestDistance = float.MaxValue;
            IInteractable best = null;

            foreach (Collider hit in hits)
            {
                if (hit.TryGetComponent(out IInteractable interactable))
                {
                    float distance = Vector3.Distance(transform.position, interactable.InteractionPoint.position);

                    if (distance < closestDistance && interactable.CanInteract(this))
                    {
                        closestDistance = distance;
                        best = interactable;
                    }
                }
            }

            m_CurrentTarget = best;
        }

        #endregion
    }
}
