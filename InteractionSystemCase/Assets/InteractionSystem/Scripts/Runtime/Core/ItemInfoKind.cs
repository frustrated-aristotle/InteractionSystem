namespace InteractionSystem.Runtime.Core
{
    /// <summary>
    /// Info mesajının türü; formatter doğru cümleyi üretir (örn: "Gold Key alındı", "Etkileşimli Kapı açıldı").
    /// </summary>
    public enum ItemInfoKind
    {
        /// <summary>Anahtar / item toplandı → "{subjectName} alındı"</summary>
        KeyPickedUp,

        /// <summary>Kapı açıldı → "{subjectName} açıldı"</summary>
        DoorOpened,

        /// <summary>Sandıktan item alındı → "{subjectName} alındı"</summary>
        ChestItemReceived
    }
}
