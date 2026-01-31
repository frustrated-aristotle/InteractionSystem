using UnityEngine;

namespace InteractionSystem.Runtime.Core
{
    /// <summary>
    /// Etkileşime girilebilir nesnelerin implement etmesi gereken arayüz.
    /// </summary>
    /// <remarks>
    /// Bu arayüz, Interactor tarafından tespit edilen ve oyuncu ile etkileşime girebilen
    /// tüm nesneler (kapı, sandık, anahtar, switch vb.) için temel sözleşmeyi tanımlar.
    /// InteractionPoint, mesafe hesabında en yakın nesne seçimi için kullanılır.
    /// </remarks>
    public interface IInteractable
    {
        /// <summary>
        /// Etkileşim noktası. Mesafe hesabı için kullanılır (genellikle nesnenin transform'u).
        /// </summary>
        Transform InteractionPoint { get; }

        /// <summary>
        /// Etkileşimi gerçekleştirir.
        /// </summary>
        /// <param name="interactor">Etkileşimi başlatan Interactor referansı.</param>
        void Interact(IInteractor interactor);

        /// <summary>
        /// Belirtilen interactor'ın bu nesne ile etkileşime girip giremeyeceğini kontrol eder.
        /// </summary>
        /// <param name="interactor">Etkileşimi deneyen Interactor referansı.</param>
        /// <returns>Etkileşime izin veriliyorsa true, aksi halde false (örn: kilitli kapı).</returns>
        bool CanInteract(IInteractor interactor);

        /// <summary>
        /// UI'da gösterilecek etkileşim mesajını döndürür.
        /// </summary>
        /// <returns>Dinamik prompt metni (örn: "Press E to Open", "Anahtar gerekli").</returns>
        string GetInteractionPrompt();
    }
}
