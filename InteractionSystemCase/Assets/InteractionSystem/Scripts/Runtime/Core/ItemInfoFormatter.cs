namespace InteractionSystem.Runtime.Core
{
    /// <summary>
    /// ItemInfoKind + subjectName ile info mesajı üretir. Tüm kullanıcıya dönük metinler tek yerde; ileride lokalizasyon eklenebilir.
    /// </summary>
    public static class ItemInfoFormatter
    {
        private const string KeyPickedUpFormat = "{0} alındı";
        private const string DoorOpenedFormat = "{0} açıldı";
        private const string ChestItemReceivedFormat = "{0} alındı";

        /// <summary>
        /// Olay türüne ve özne adına göre gösterilecek mesajı döndürür.
        /// </summary>
        /// <param name="kind">Olay türü (anahtar alındı, kapı açıldı, sandıktan item alındı).</param>
        /// <param name="subjectName">Özne adı (örn: "Gold Key", "Etkileşimli Kapı").</param>
        /// <returns>Örn: "Gold Key alındı", "Etkileşimli Kapı açıldı".</returns>
        public static string GetMessage(ItemInfoKind kind, string subjectName)
        {
            string name = string.IsNullOrWhiteSpace(subjectName) ? "?" : subjectName.Trim();
            return kind switch
            {
                ItemInfoKind.KeyPickedUp => string.Format(KeyPickedUpFormat, name),
                ItemInfoKind.DoorOpened => string.Format(DoorOpenedFormat, name),
                ItemInfoKind.ChestItemReceived => string.Format(ChestItemReceivedFormat, name),
                _ => name
            };
        }
    }
}
