# Interaction System - Abdullah Seçgeler

> Ludu Arts Unity Developer Intern Case

## Proje Bilgileri

| Bilgi | Değer |
|-------|-------|
| Unity Versiyonu | 6000.0.37f1 |
| Render Pipeline | Built-in / URP / HDRP |
| Case Süresi | 12 saat |
| Tamamlanma Oranı | %100 |

---

## Kurulum

1. Repository'yi klonlayın:
```bash
git clone https://github.com/frustrated-aristotle/InteractionSystem.git
```

2. Unity Hub'da projeyi açın
3. `Assets/InteractionSystem/Scenes/TestScene.unity` sahnesini açın
4. Play tuşuna basın

---

## Nasıl Test Edilir

### Kontroller

| Tuş | Aksiyon |
|-----|---------|
| WASD | Hareket |
| Mouse | Bakış yönü |
| E | Etkileşim (basılı tutma: Chest için E'yi basılı tutun) |

### Test Senaryoları

1. **Door Test:**
   - Kapıya yaklaşın, "Press E to Open" mesajını görün
   - E'ye basın, kapı açılsın
   - Tekrar basın, kapı kapansın

2. **Key + Locked Door Test:**
   - Kilitli kapıya yaklaşın, "Anahtar gerekli" / key required mesajını görün
   - Anahtarı (Gold Key / Rusted Key) toplayın (E ile instant)
   - Kilitli kapıya geri dönün, şimdi açılabilir olmalı
   - Envanter UI'da anahtar listesi (Gold Key x1 vb.) görünür

3. **Switch Test:**
   - Switch'e yaklaşın ve aktive edin
   - Bağlı nesnenin (kapı/ışık vb.) tetiklendiğini görün

4. **Chest Test:**
   - Sandığa yaklaşın
   - E'ye basılı tutun, progress bar dolsun
   - Sandık açılsın ve içindeki item envantere alınsın (InfoDisplay + Inventory UI güncellenir)

5. **Out of range:** Interactable'a bakın ama menzilden uzak durun; altta "Out of range" yazmalı.

---

## Mimari Kararlar

### Interaction System Yapısı

```
Player (IInteractor)
  └── Interactor     → her frame raycast (DetectionRange), tek hedef (CurrentTarget, IsTargetInRange)
  └── Inventory      → key listesi, OnInventoryChanged, HasKey

Interactable (IInteractable) ← abstract Interactable.cs (prompt, animator, save/load, SyncAnimatorToState)
  └── Door, Chest, Switch, KeyPickup, PressurePlate, Launcher
  └── İsteğe bağlı: Interaction component (HoldInteraction / ToggleInteraction / InstantInteraction)
        → Interactor hedefte GetComponent<Interaction> bulursa Execute(interactor), yoksa Interact(interactor)

UI: InteractionPrompt ← Interactor.CurrentTarget, IsTargetInRange, HoldProgress
    InventoryUI      ← Inventory.OnInventoryChanged ile liste güncelleme
    InfoDisplay       ← IInteractor.ShowItemInfo (sandık/key bilgisi)

SaveManager → Interactable.SerializeState/LoadState, Inventory.GetSaveState/LoadState
```

**Neden bu yapıyı seçtim:**
> IInteractable/IInteractor ile etkileşim tarafı (kapı, sandık) ve oyuncu tarafı (envanter, prompt) ayrıldı; Interactor sadece raycast ve input yönetiyor, davranış Interactable ve isteğe bağlı Interaction component’te. Böylece yeni interactable eklemek için sadece Interactable’dan türetip SerializeState/LoadState override etmek yeterli. Hold/Toggle/Instant için ayrı Interaction sınıfları Interactable’a takılabiliyor, KeyPickup gibi basit olanlar doğrudan Interact() ile çalışıyor.

**Alternatifler:**
> Trigger-based detection (collider overlap) yerine raycast seçildi: bakılan tek nesne net, menzil ve “out of range” kontrolü kolay. State machine ile tek bir “interaction controller” yerine her Interactable kendi CanInteract/Interact/GetPrompt mantığını tutuyor; böylece kapı/sandık/anahtar farklı kurallara rağmen aynı detector’a bağlanabiliyor.

**Trade-off'lar:**
> Avantaj: Genişletmek kolay (yeni Interactable + isteğe bağlı Interaction), Save/Load her nesnede SerializeState/LoadState ile tutarlı, UI Interactor/Inventory property’lerine bağlı kalıyor. Dezavantaj: Çok sayıda interactable aynı frame’de ray’de olursa sadece ilk vuran seçiliyor (en yakın tercihi için ek mantık gerekebilir); Interaction component opsiyonel olduğu için bazen Interact() bazen Execute() çağrılıyor, kod okurken iki yol bilinmeli.

### Kullanılan Design Patterns

| Pattern | Kullanım Yeri | Neden |
|---------|---------------|-------|
| Interface / DIP | IInteractable, IInteractor, IInventory | Interactor somut Door/Chest’e değil arayüze bağlı; yeni interactable eklemek için interface implement etmek yeterli. |
| Template Method | Interactable (abstract), SerializeState / LoadState / GetClosedStateName override | Ortak prompt, animator, save alanları base’de; davranış farkı alt sınıflarda override ile. |
| Observer | Inventory.OnInventoryChanged, HoldInteraction.OnHoldComplete | UI (InventoryUI) envanter değişince güncellenir; hold bittiğinde Chest/Interactor haberdar olur. |
| Component / Strategy benzeri | Interaction (Hold/Toggle/Instant) component’leri | Etkileşim türü Interactable’a takılan component ile değişiyor; Interactor Execute(interactor) ile çalıştırıyor. |

Bilinçli olarak ekstra (ağır) pattern eklemedim: Bu case için interface + abstract base + event yeterli geldi. State machine, command pattern veya factory kullanmak karmaşıklığı artırırdı; mevcut yapı hem okunabilir hem yeni Door/Chest/Key eklemek için yeterince esnek.

---

## Ludu Arts Standartlarına Uyum

### C# Coding Conventions

| Kural | Uygulandı | Notlar |
|-------|-----------|--------|
| m_ prefix (private fields) | [x] | Tüm runtime scriptlerde |
| s_ prefix (private static) | [x] | Gerektiği yerde |
| k_ prefix (private const) | [x] | Gerektiği yerde |
| Region kullanımı | [x] | Fields, Properties, Unity Methods, Methods vb. |
| Region sırası doğru | [x] | Ludu Arts sırasına uygun |
| XML documentation | [x] | Public API ve önemli protected |
| Silent bypass yok | [x] | Log/return, null check |
| Explicit interface impl. | [x] | IInteractable, IInteractor, IInventory |

### Naming Convention

| Kural | Uygulandı | Örnekler |
|-------|-----------|----------|
| P_ prefix (Prefab) | [x] | P_Door, P_Chest, P_HandLever, P_Key_Golden |
| M_ prefix (Material) | [x] | M_Clay_01, DungeonKit materyalleri |
| T_ prefix (Texture) | [x] | Proje texture'ları |
| SO isimlendirme | [x] | SO_Key_Gold, SO_Key_Rusted (KeyItemData) |

### Prefab Kuralları

| Kural | Uygulandı | Notlar |
|-------|-----------|--------|
| Transform (0,0,0) | [x] | Prefab root'larda |
| Pivot bottom-center | [x] | Model/asset'e göre |
| Collider tercihi | [x] | Box / Capsule / Mesh |
| Hierarchy yapısı | [x] | Root, Mesh/Visual, Colliders (Prefab_Asset_Kurallari) |

### Zorlandığım Noktalar
> [Standartları uygularken zorlandığınız yerler]
Dışarıdan aldığım assetleri import ettikten sonra isimlendirmede ve prefab'e çevirdikten sonra bunları isimlendirip klasörlemede zorlandım.
Naming convention'ları yapay zekaya düzelttirmesem zorlanırdım çünkü bu convention'ı kullanmıyorum.
---

## Tamamlanan Özellikler

### Zorunlu (Must Have)

- [x] Core Interaction System
  - [x] IInteractable / IInteractor interface
  - [x] Interactor (raycast tabanlı detection)
  - [x] Interaction range + detection range (out of range feedback)

- [x] Interaction Types
  - [x] Instant (KeyPickup)
  - [x] Hold (Chest, HoldInteraction)
  - [x] Toggle (Door, Switch)

- [x] Interactable Objects
  - [x] Door (locked/unlocked, RequiredKey, HasKey)
  - [x] Key Pickup (instant, envantere ekleme)
  - [x] Switch/Lever (toggle, OnToggle event → kapı vb.)
  - [x] Chest/Container (hold, item inside, tek seferlik)

- [x] UI Feedback
  - [x] Interaction prompt (InteractionPrompt, dinamik metin)
  - [x] Dynamic text (nesneye göre prompt / unable / out of range)
  - [x] Hold progress bar
  - [x] Cannot interact feedback (kilitli kapı vb.)
  - [x] Out of range mesajı

- [x] Simple Inventory
  - [x] Key toplama (Inventory.AddItem, KeyItemData)
  - [x] UI listesi (InventoryUI, OnInventoryChanged)
  - [x] Locked door + key kontrolü (HasKey)

### Bonus (Nice to Have)

- [x] Animation entegrasyonu (Interactable animator, SyncAnimatorToState on LoadState)
- [x] Sound effects (açma/kapama sesleri, Interactable)
- [x] Multiple keys / color-coded (KeyItemData.KeyColor, Door.RequiredKey)
- [x] Interaction highlight (Quick Outline, menzilde)
- [x] Save/Load states (SaveManager, Door/Chest/Switch/KeyPickup/Inventory)
- [x] Chained interactions (Switch → Door, UnityEvent OnToggle)

---

## Bilinen Limitasyonlar

### Tamamlanamayan Özellikler
Yok.

### Bilinen Bug'lar
1. Animasyonu Interactable'lardan Lever ve Chest'in oyun yeniden başlatıldığında state'lerinin güncellenmesi fakat rotate eden parçalarının pozisyonlarının buna göre güncellenememesi - Herhangi bir Lever ve Chest ile etkileşime geçip toggle edin, oyunu kill edip tekrar açın, state'ler yüklenecek fakat rotate eden parçalar ilk hallerindeki gibi gözükecekler.
2. Launcher ve Harpoon fırlatma - Son odadaki Preassure Plate'e basın. Launcher tetiklenecek fakat Harpoon düzgünce fırlamayacak.
### İyileştirme Önerileri
1. **En yakın hedef seçimi** — Şu an raycast ilk vuran collider’ı alıyor. Aynı ray’de birden fazla interactable varsa `Physics.RaycastAll` + `InteractionPoint`’a göre mesafe sıralaması yapılıp en yakın nesne seçilebilir; böylece önünde ince bir obje varken arkadaki kapıya “bakıyorum” hissi artar.
2. **Interact vs Execute tek yolu** — Bazı nesnelerde `Interact()` bazılarında `Interaction.Execute()` çağrılıyor. Tek bir sözleşme (örn. Interactable her zaman kendi Interact’inde Interaction component varsa ona delege etsin) ve kısa bir akış diyagramı dokümanda olursa yeni interactable eklerken karışıklık azalır.
3. **Lever/Chest child animator veya transform state** — LoadState sonrası sadece root animator sync ediliyor; Lever/Chest’te dönen parça ayrı objede/animator’da ise onun da state’i kayda yazılıp yüklemede uygulanabilir (veya tek animator’da tüm hareket tek controller’da toplanabilir).
4. **Prefab hierarchy tutarlılığı** — Tüm interactable prefab’larda Prefab_Asset_Kurallari’ndaki Root → Mesh → Colliders yapısı ve isimlendirme (Collider_Main vb.) tek tek kontrol edilip eksikler tamamlanabilir.

---

## Ekstra Özellikler

Zorunlu gereksinimlerin dışında eklediklerim:

1. **PressurePlate & Launcher**
   - Basınç plakası (üzerine basılınca tetiklenir, E ile etkileşim yok) ve Launcher (tetiklenince prefab fırlatır).
   - Save/Load destekli (PressurePlate state kaydedilir).

2. **Save/Load (SaveManager)**
   - Door, Chest, KeyPickup, Switch, PressurePlate ve Inventory state'i PlayerPrefs + JSON ile kaydedilir/yüklenir.
   - Save ID varsayılan olarak GameObject adı (GetSaveId); isteğe bağlı m_SaveId override.
   - ClearSave() + Context menu "Clear Save"; opsiyonel "Clear Save On Start" (test için).
   - Editor'da Play'den çıkarken otomatik Save() (SaveManagerEditorHook).

3. **LoadState sonrası görsel senkron**
   - **SyncAnimatorToState:** Kayıt yüklendiğinde Animator state "Open"/"Close" (veya Idle) son karesine Play(stateName, 0, 1f). Trigger'lar Open/Close; ekstra parametre alanı yok.
   - **SyncTransformToState:** Açık/kapalı rotasyon (Closed/Open Rotation Euler + Rotation Target) LoadState ve gerekirse birkaç frame LateUpdate ile uygulanır.
   - **Rotation Driven By Animator:** Chest/Lever gibi Animator'ın döndürdüğü objelerde işaretlenir; sadece animator state senkronu kullanılır, transform rotasyonu atlanır (ezilmesin diye).
   - **DelayedSyncRotation:** Load'tan bir frame sonra hem transform (Rotation Driven değilse) hem animator state tekrar senkronize edilir.

4. **Chest davranışı**
   - Bir kez açıldıysa "Already opened" detay metni; consumed sonrası tekrar etkileşime girilemez.
   - OnOpened UnityEvent (trap sandık için dinleyici bağlanabilir).

5. **InfoDisplay & ShowItemInfo**
   - Sandık açılınca / anahtar alınınca kısa bilgi metni (ItemInfoKind + ItemInfoFormatter); varsayılan 3 saniye sonra silinir.
   - IInteractor.ShowItemInfo(string, float) ve ShowItemInfo(ItemInfoKind, subjectName, float).

6. **Prompt & UI**
   - Prompt metni: `ObjectName` + ana metin (IInteractable.ObjectName; boşsa gameObject.name).
   - Hold progress bar: tamamlanmaya yaklaşırken kırmızı→yeşil renk geçişi.
   - Outline sadece menzil içindeyken; menzil dışında "Out of range" metni.

7. **Inventory OnInventoryChanged event**
   - Envanter her değiştiğinde InventoryUI otomatik güncellenir (key toplama / LoadState sonrası liste güncel kalır).

---

## Dosya Yapısı

```
Assets/
├── InteractionSystem/
│   ├── Scripts/
│   │   ├── Runtime/
│   │   │   ├── Core/
│   │   │   │   ├── IInteractable.cs, IInteractor.cs, IInventory.cs
│   │   │   │   ├── Interactable.cs, Interaction.cs
│   │   │   │   ├── HoldInteraction.cs, InstantInteraction.cs, ToggleInteraction.cs
│   │   │   │   ├── KeyItemData.cs, ItemInfoFormatter.cs, ItemInfoKind.cs
│   │   │   │   └── InteractionSaveData.cs
│   │   │   ├── Interactables/
│   │   │   │   ├── Door.cs, Chest.cs, Switch.cs, KeyPickup.cs
│   │   │   │   ├── PressurePlate.cs, Launcher.cs
│   │   │   ├── Player/
│   │   │   │   ├── Interactor.cs, Inventory.cs, SaveManager.cs
│   │   │   └── UI/
│   │   │       ├── InteractionPrompt.cs, InfoDisplay.cs, InventoryUI.cs
│   │   └── Editor/
│   ├── ScriptableObjects/Items/   (SO_Key_Gold, SO_Key_Rusted)
│   ├── Prefabs/Interactables/    (P_Door, P_Chest, P_HandLever, P_Key_*)
│   ├── Objects/, Materials/, Sound/, Settings/
│   └── Scenes/
│       └── TestScene.unity
├── Docs/
│   ├── CSharp_Coding_Conventions.md
│   ├── Naming_Convention_Kilavuzu.md
│   └── Prefab_Asset_Kurallari.md
├── README.md
├── PROMPTS.md
└── .gitignore
```

---

## İletişim

| Bilgi | Değer |
|-------|-------|
| Ad Soyad | Abdullah Seçgeler |
| E-posta | a.secgeler@gmail.com |
| LinkedIn | https://www.linkedin.com/in/secgeler/ |
| GitHub | https://github.com/frustrated-aristotle |

---

*Bu proje Ludu Arts Unity Developer Intern Case için hazırlanmıştır.*
