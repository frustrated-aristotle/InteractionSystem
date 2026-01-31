namespace InteractionSystem.Runtime.Core
{
    /// <summary>
    /// Etkileşimi başlatan entity'yi temsil eden arayüz.
    /// </summary>
    /// <remarks>
    /// IInteractable implementasyonları (Door, KeyPickup vb.) bu arayüz üzerinden
    /// envantere erişir. Interactor bu arayüzü implement eder.
    /// </remarks>
    public interface IInteractor
    {
        /// <summary>
        /// Bu interactor'a ait envanter. Key toplama ve kapı kilidi kontrolü için kullanılır.
        /// </summary>
        IInventory Inventory { get; }
    }
}
