using UnityEngine;

namespace InteractionSystem.Runtime.Core
{
    /// <summary>
    /// Envanter işlemleri için arayüz.
    /// </summary>
    /// <remarks>
    /// Key toplama, kapı kilidi kontrolü gibi işlemlerde IInteractor üzerinden erişilir.
    /// </remarks>
    public interface IInventory
    {
        /// <summary>
        /// Belirtilen item'ı envantere ekler.
        /// </summary>
        /// <param name="item">Eklenecek item (ScriptableObject referansı).</param>
        void AddItem(ScriptableObject item);

        /// <summary>
        /// Envanterde belirtilen item'ın olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="item">Aranacak item.</param>
        /// <returns>Varsa true, yoksa false.</returns>
        bool HasItem(ScriptableObject item);

        /// <summary>
        /// Envanterde belirtilen anahtar tipinin olup olmadığını kontrol eder.
        /// Kapı kilidi eşleşmesi için kullanılır (referans veya aynı KeyName ile eşleşme).
        /// </summary>
        /// <param name="key">Aranacak anahtar tipi (KeyItemData).</param>
        /// <returns>Varsa true, yoksa false.</returns>
        bool HasKey(KeyItemData key);
    }
}
