using UnityEngine;

namespace InteractionSystem.Runtime.Core
{
    /// <summary>
    /// Anahtar tipi tanımı. Her asset = bir anahtar tipi; kapı kilidi eşleşmesi ScriptableObject referansı ile yapılır.
    /// </summary>
    /// <remarks>
    /// CreateAssetMenu ile oluşturulur. Birden fazla tip için ayrı asset'ler (örn: SO_Key_Rusted, SO_Key_Gold).
    /// Bir kapı sadece tek bir KeyItemData ile açılır; eşleşme referans (aynı asset) ile kontrol edilir.
    /// Color: color-coded locks için (UI ikonu, kilit göstergesi vb.).
    /// </remarks>
    [CreateAssetMenu(fileName = "SO_Key_", menuName = "Interaction System/Items/Key Item", order = 0)]
    public class KeyItemData : ScriptableObject
    {
        #region Fields

        [SerializeField] private string m_KeyName = "Key";
        [SerializeField] [Tooltip("Color-coded locks: UI ve kilit göstergesi için kullanılır.")]
        private Color m_KeyColor = Color.gray;

        #endregion

        #region Properties

        /// <summary>
        /// Anahtarın görüntülenen adı.
        /// </summary>
        public string KeyName => m_KeyName;

        /// <summary>
        /// Anahtar rengi. Color-coded locks / UI ikonu için.
        /// </summary>
        public Color KeyColor => m_KeyColor;

        #endregion
    }
}
