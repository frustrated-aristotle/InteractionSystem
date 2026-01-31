using UnityEngine;
using InteractionSystem.Runtime.Core;
using InteractionSystem.Runtime.UI;

namespace InteractionSystem.Runtime.Player
{
    /// <summary>
    /// Oyuncunun etkileşim kaynağını temsil eden bileşen.
    /// </summary>
    /// <remarks>
    /// Kamera yönünde Physics.Raycast ile bakılan IInteractable nesneyi tespit eder.
    /// Sadece bakıldığında prompt gösterilir ve etkileşim yapılabilir.
    /// </remarks>
    [RequireComponent(typeof(Inventory))]
    public class Interactor : MonoBehaviour, IInteractor
    {
        #region Fields

        [SerializeField] private float m_InteractionRange = 2f;
        [SerializeField] [Tooltip("Bu mesafeye kadar bakılan nesne tespit edilir; menzil dışındayken 'Out of range' göstermek için Interaction Range'dan büyük olmalı.")]
        private float m_DetectionRange = 5f;
        [SerializeField] [Tooltip("Raycast bu katmanlara çarpar. Oyuncu önce vuruluyorsa Player katmanını kaldır.")]
        private LayerMask m_RaycastLayers = ~0;
        [SerializeField] private KeyCode m_InteractKey = KeyCode.E;
        [SerializeField] private bool m_UseOutlineHighlight = true;
        [SerializeField] [Tooltip("Boş bırakılırsa Camera.main kullanılır.")] private Camera m_ViewCamera;
        [SerializeField] [Tooltip("Item bilgisi (sandık/key) gösterilecek. Boşsa sahnede aranır.")] private InfoDisplay m_InfoDisplay;

        private Inventory m_Inventory;
        private IInteractable m_CurrentTarget;
        private bool m_IsTargetInRange;
        private bool m_IsHolding;
        private Interaction m_ActiveHoldInteraction;
        private GameObject m_LastOutlinedRoot;

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

        /// <summary>
        /// Hold etkileşimi devam ediyorsa true.
        /// </summary>
        public bool IsHolding => m_IsHolding;

        /// <summary>
        /// Mevcut hold ilerlemesi (0-1). UI progress bar bu property'e bağlanır.
        /// </summary>
        public float HoldProgress => m_ActiveHoldInteraction is HoldInteraction hold ? hold.Progress : 0f;

        /// <summary>
        /// Hedef nesne etkileşim menzilinde mi? False ise UI "Out of range" gösterebilir.
        /// </summary>
        public bool IsTargetInRange => m_IsTargetInRange;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            m_Inventory = GetComponent<Inventory>();
            if (m_Inventory == null)
            {
                Debug.LogError($"[Interactor] Inventory component not found on {gameObject.name}. Add Inventory to the same GameObject.");
            }

            if (m_ViewCamera == null)
            {
                m_ViewCamera = Camera.main;
            }

            if (m_InfoDisplay == null)
            {
                m_InfoDisplay = FindAnyObjectByType<InfoDisplay>();
            }

            if (m_DetectionRange < m_InteractionRange)
            {
                m_DetectionRange = m_InteractionRange + 1f;
            }
        }

        /// <inheritdoc/>
        public void ShowItemInfo(string info, float displayDurationSeconds = 3f)
        {
            if (m_InfoDisplay == null)
            {
                m_InfoDisplay = FindAnyObjectByType<InfoDisplay>();
            }
            m_InfoDisplay?.ShowInfo(info ?? "", displayDurationSeconds);
        }

        /// <inheritdoc/>
        public void ShowItemInfo(ItemInfoKind kind, string subjectName, float displayDurationSeconds = 3f)
        {
            string message = ItemInfoFormatter.GetMessage(kind, subjectName);
            ShowItemInfo(message, displayDurationSeconds);
        }

        private void Update()
        {
            FindBestInteractable();
            UpdateOutlineHighlight();

            if (m_IsHolding)
            {
                if (!Input.GetKey(m_InteractKey))
                {
                    CancelHold();
                }
                else if (m_ActiveHoldInteraction != null)
                {
                    m_ActiveHoldInteraction.Execute(this);
                }

                return;
            }

            if (m_CurrentTarget != null && m_IsTargetInRange && Input.GetKeyDown(m_InteractKey))
            {
                TryExecuteInteraction();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Hold tamamlandığında çağrılır.
        /// </summary>
        private void HandleHoldComplete(IInteractor interactor)
        {
            if (m_ActiveHoldInteraction is HoldInteraction holdInteraction)
            {
                holdInteraction.OnHoldComplete -= HandleHoldComplete;
            }

            m_IsHolding = false;
            m_ActiveHoldInteraction = null;
        }

        /// <summary>
        /// Tuş bırakıldığında hold iptal edilir.
        /// </summary>
        private void CancelHold()
        {
            if (m_ActiveHoldInteraction is HoldInteraction holdInteraction)
            {
                holdInteraction.OnHoldComplete -= HandleHoldComplete;
                holdInteraction.ResetProgress();
            }

            m_IsHolding = false;
            m_ActiveHoldInteraction = null;
        }

        /// <summary>
        /// Hedef nesnenin Interaction component'ini kullanarak veya doğrudan Interact çağırarak etkileşimi başlatır.
        /// </summary>
        private void TryExecuteInteraction()
        {
            if (m_CurrentTarget == null)
            {
                return;
            }

            if (!m_CurrentTarget.CanInteract(this))
            {
                return;
            }

            Interaction interaction = GetInteractionFromTarget(m_CurrentTarget);

            if (interaction != null)
            {
                if (interaction.Type == InteractionType.Hold && interaction is HoldInteraction holdInteraction)
                {
                    m_IsHolding = true;
                    m_ActiveHoldInteraction = interaction;
                    holdInteraction.OnHoldComplete += HandleHoldComplete;
                }

                interaction.Execute(this);
            }
            else
            {
                m_CurrentTarget.Interact(this);
            }
        }

        /// <summary>
        /// IInteractable nesnesinden Interaction component'ini alır.
        /// </summary>
        /// <param name="interactable">Hedef etkileşim nesnesi.</param>
        /// <returns>Interaction component veya null.</returns>
        private static Interaction GetInteractionFromTarget(IInteractable interactable)
        {
            if (interactable?.InteractionPoint == null)
            {
                return null;
            }

            return interactable.InteractionPoint.GetComponentInParent<Interaction>();
        }

        /// <summary>
        /// Kamera yönünde raycast ile bakılan IInteractable nesneyi tespit eder.
        /// Detection range içinde hedef seçilir; interaction range içindeyse etkileşim mümkün.
        /// </summary>
        private void FindBestInteractable()
        {
            m_CurrentTarget = null;
            m_IsTargetInRange = false;

            if (m_ViewCamera == null)
            {
                Debug.LogWarning("[Interactor] View camera is null; interaction detection disabled.");
                return;
            }

            float maxDistance = Mathf.Max(m_InteractionRange, m_DetectionRange);
            Ray ray = new Ray(m_ViewCamera.transform.position, m_ViewCamera.transform.forward);

            if (!Physics.Raycast(ray, out RaycastHit hit, maxDistance, m_RaycastLayers))
            {
                return;
            }

            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                m_CurrentTarget = interactable;
                m_IsTargetInRange = hit.distance <= m_InteractionRange;
            }
        }

        /// <summary>
        /// Etkileşim hedefi değişince Quick Outline'ı açar/kapatır.
        /// Sadece menzil içindeyken (CurrentTarget ve IsTargetInRange) outline aktif olur.
        /// </summary>
        private void UpdateOutlineHighlight()
        {
            if (!m_UseOutlineHighlight)
            {
                SetOutlineEnabled(m_LastOutlinedRoot, false);
                m_LastOutlinedRoot = null;
                return;
            }

            bool shouldOutline = m_CurrentTarget != null && m_IsTargetInRange;
            GameObject currentRoot = shouldOutline ? GetOutlineRoot(m_CurrentTarget) : null;

            if (m_LastOutlinedRoot != currentRoot)
            {
                SetOutlineEnabled(m_LastOutlinedRoot, false);
                m_LastOutlinedRoot = currentRoot;
                SetOutlineEnabled(m_LastOutlinedRoot, true);
            }
        }

        /// <summary>
        /// IInteractable nesnesinin outline uygulanacak root GameObject'ini döndürür.
        /// </summary>
        /// <param name="interactable">Hedef etkileşim nesnesi.</param>
        /// <returns>Outline component'lerinin aranacağı root veya null.</returns>
        private static GameObject GetOutlineRoot(IInteractable interactable)
        {
            if (interactable == null)
            {
                return null;
            }

            if (interactable is MonoBehaviour mb)
            {
                return mb.gameObject;
            }

            return interactable.InteractionPoint != null ? interactable.InteractionPoint.root.gameObject : null;
        }

        /// <summary>
        /// Verilen root altındaki tüm Outline component'lerini açar veya kapatır.
        /// </summary>
        /// <param name="root">Outline'ların aranacağı root GameObject.</param>
        /// <param name="enabled">true ise outline gösterilir, false ise kapatılır.</param>
        private static void SetOutlineEnabled(GameObject root, bool enabled)
        {
            if (root == null)
            {
                return;
            }

            Outline[] outlines = root.GetComponentsInChildren<Outline>(true);
            foreach (Outline outline in outlines)
            {
                outline.enabled = enabled;
            }
        }

        #endregion
    }
}
