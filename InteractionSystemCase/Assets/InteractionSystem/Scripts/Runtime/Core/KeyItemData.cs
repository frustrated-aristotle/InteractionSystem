using UnityEngine;

namespace InteractionSystem.Runtime.Core
{
    /// <summary>
    /// Anahtar item tanımı. Kapı kilidi eşleştirmesi için ScriptableObject referansı kullanılır.
    /// </summary>
    /// <remarks>
    /// CreateAssetMenu ile oluşturulur. Farklı kapılar için farklı KeyItemData asset'leri tanımlanır.
    /// </remarks>
    [CreateAssetMenu(fileName = "SO_Key_", menuName = "Interaction System/Items/Key Item", order = 0)]
    public class KeyItemData : ScriptableObject
    {
        #region Fields

        [SerializeField] private string m_KeyName = "Key";

        #endregion

        #region Properties

        /// <summary>
        /// Anahtarın görüntülenen adı.
        /// </summary>
        public string KeyName => m_KeyName;

        #endregion
    }
}
