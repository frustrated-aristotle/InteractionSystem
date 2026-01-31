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

        /// <summary>
        /// Item bilgisini infoText'te gösterir; belirtilen süre sonra silinir (örn: sandık / key pickup).
        /// </summary>
        /// <param name="info">Gösterilecek metin.</param>
        /// <param name="displayDurationSeconds">Kaç saniye sonra silineceği (varsayılan 3).</param>
        void ShowItemInfo(string info, float displayDurationSeconds = 3f);

        /// <summary>
        /// Olay türüne göre formatlanmış mesajı infoText'te gösterir (örn: "Gold Key alındı", "Etkileşimli Kapı açıldı").
        /// </summary>
        /// <param name="kind">Olay türü (KeyPickedUp, DoorOpened, ChestItemReceived).</param>
        /// <param name="subjectName">Özne adı (anahtar/kapı/item adı).</param>
        /// <param name="displayDurationSeconds">Kaç saniye sonra silineceği (varsayılan 3).</param>
        void ShowItemInfo(ItemInfoKind kind, string subjectName, float displayDurationSeconds = 3f);
    }
}
