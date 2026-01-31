using UnityEngine;

namespace InteractionSystem.Runtime.Interactables
{
    /// <summary>
    /// Tetiklenince bir GameObject prefab'ı spawn edip fırlatır. Chest.OnOpened veya PressurePlate.OnActivated gibi UnityEvent'lerden Launch() çağrılabilir.
    /// </summary>
    /// <remarks>
    /// Fırlatılacak prefab'ta Rigidbody olmalı (force uygulanır). Dinleyici yoksa trap değildir; Chest'te OnOpened boş bırakılabilir.
    /// </remarks>
    public class Launcher : MonoBehaviour
    {
        #region Fields

        [SerializeField] [Tooltip("Fırlatılacak prefab. Rigidbody içermeli.")]
        private GameObject m_ProjectilePrefab;
        [SerializeField] [Tooltip("Spawn noktası. Boşsa bu objenin pozisyonu kullanılır.")]
        private Transform m_SpawnPoint;
        [SerializeField] [Tooltip("Fırlatma yönü (bu objenin local space'inde). Boşsa forward kullanılır.")]
        private Vector3 m_LaunchDirection = Vector3.forward;
        [SerializeField] [Tooltip("Uygulanacak kuvvet (Impulse).")]
        private float m_Force = 15f;
        [SerializeField] [Tooltip("Spawn sonrası otomatik yok etme süresi (saniye). 0 ise yok etme.")]
        private float m_AutoDestroyAfterSeconds;

        #endregion

        #region Methods

        /// <summary>
        /// Prefab'ı spawn edip fırlatır. UnityEvent'ten çağrılabilir (örn: Chest OnOpened, PressurePlate OnActivated).
        /// </summary>
        public void Launch()
        {
            if (m_ProjectilePrefab == null)
            {
                Debug.LogWarning("[Launcher] Projectile prefab is null; cannot launch.");
                return;
            }

            Vector3 pos = m_SpawnPoint != null ? m_SpawnPoint.position : transform.position;
            Quaternion rot = m_SpawnPoint != null ? m_SpawnPoint.rotation : transform.rotation;
            GameObject instance = Instantiate(m_ProjectilePrefab, pos, rot);

            Vector3 dir = (m_SpawnPoint != null ? m_SpawnPoint.TransformDirection(m_LaunchDirection.normalized) : transform.TransformDirection(m_LaunchDirection.normalized)).normalized;

            Rigidbody rb = instance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(dir * m_Force, ForceMode.Impulse);
            }
            else
            {
                Debug.LogWarning("[Launcher] Projectile prefab has no Rigidbody; force not applied.");
            }

            if (m_AutoDestroyAfterSeconds > 0f)
            {
                Destroy(instance, m_AutoDestroyAfterSeconds);
            }
        }

        #endregion
    }
}
