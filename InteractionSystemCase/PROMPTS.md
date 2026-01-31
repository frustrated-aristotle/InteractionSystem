# LLM Kullanım Dokümantasyonu

## Özet
- Toplam prompt sayıs: 90-100
- Kullanılan araçlar: ChatGPT / Copilot / Cursor / Gemini
- En çok yardım alınan konular: [Refactoring, Naming Conventions, XML Dökümantasyonu, İsimlendirmeyle ilgili konularda nasıl davranmalıyım, Kod Yükünün Azaltılması (Benzer sınıflara benzer kodları hızlıca yazdırmak) ]

---

## Prompt 1: [Ana Tasarım]

**Araç:** ChatGPT-4
**Tarih/Saat:** 2026-01-31 12:32

**Prompt:**
Ekteki projeyi tamamlamam gerekiyor. Bana IInteractible ve projenin devamı için fikir verebilir misin?
Raycast veya collider odaklı değil de hibrit bir yapı düşünüyorum. 

**Alınan Cevap (Özet):**
Bu cevap direkt seni case’i bitirecek noktaya götürür.

1) IInteractable Interface Nasıl Tasarlanmalı?

Bu interface’in amacı:

Detector aynı kalacak

Nesneler farklı davranacak

UI prompt ve state yönetilecek

Minimum ihtiyaçlar:

Prompt text

Interaction type

Interact çağrısı

Interactable mı değil mi kontrolü

**Nasıl Kullandım:**
- [ ] Direkt kullandım
- [x] Adapte ettim
- [ ] Reddettim

**Açıklama:**
Beklediğim cevabı aldım ve uyguladım.

---

## Prompt 2: [Kodların Taslağı]

**Araç:** ChatGPT-4
**Tarih/Saat:** 2026-01-31 13:07

**Prompt:**
Şimdi bana gerekli olan scriptlerin taslaklarını yazar mısın? IInterractable'ı da ver yeniden. Not alacağım.

**Alınan Cevap (Özet):**
1) IInteractable Interface Nasıl Tasarlanmalı?

Bu interface’in amacı:

Detector aynı kalacak

Nesneler farklı davranacak

UI prompt ve state yönetilecek

Minimum ihtiyaçlar:

Prompt text

Interaction type

Interact çağrısı

Interactable mı değil mi kontrolü

Önerilen Interface
public interface IInteractable
{
    string Prompt { get; }

    bool CanInteract(Interactor interactor);

    void Interact(Interactor interactor);

    Transform InteractionPoint { get; }
}

Neden böyle?
Member	Sebep
Prompt	UI’da “Press E to Open” göstermek
CanInteract	Locked door vs chest opened gibi durumlar
Interact	Eylemi başlatmak
InteractionPoint	Raycast yerine mesafe kontrolü için doğru nokta
2) Detection: Raycast mi Trigger mı?

Bu soru interview’de çok sorulur.

Raycast Detection

Nasıl çalışır?

Kamera forward ray atarsın

Crosshair hangi objeye bakıyorsa onu seçersin

Physics.Raycast(camera.position, camera.forward, out hit, range);

Avantajlar

✅ FPS oyunları için ideal
✅ Oyuncu tam baktığı şeyi seçer
✅ UI prompt net olur
✅ Single target doğal olarak sağlanır

Dezavantajlar

❌ Arkandaki nesneyi algılamaz
❌ Obje collider gerektirir
❌ Çok geniş interaction alanlarında zor

Trigger Detection

Nasıl çalışır?

Player etrafında sphere collider trigger olur

İçeri giren interactable listelenir

OnTriggerEnter(Collider other)

Avantajlar

✅ Third-person RPG için ideal
✅ Yakındaki her şeyi bilir
✅ En yakın seçme kolay
✅ Raycast kaçırma problemi yok

Dezavantajlar

❌ Aynı anda çok obje varsa selection logic şart
❌ “Baktığım değil yakınımdaki” hissi olabilir
❌ Trigger spam olabilir

Case için En Doğru Seçim?

Bu task requirement:

Range içinde birden fazla interactable varsa en yakını seç

Single interaction point

Bu doğrudan trigger sistemine daha uygun.

📌 Ben sana öneriyorum:

Hybrid Sistem (En iyi çözüm)

Trigger ile adayları bul

En yakını seç

İstersen raycast ile doğrula

Bu senior yaklaşımıdır.

3) InteractionDetector Tasarımı
Interactor (Player Component)
public class Interactor : MonoBehaviour
{
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private IInteractable currentTarget;

    private void Update()
    {
        FindBestInteractable();

        if (currentTarget != null && Input.GetKeyDown(interactKey))
        {
            currentTarget.Interact(this);
        }
    }

    private void FindBestInteractable()
    {
        // Sphere overlap ile en yakın seç
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionRange);

        float closestDistance = float.MaxValue;
        IInteractable best = null;

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IInteractable interactable))
            {
                float dist = Vector3.Distance(transform.position,
                                             interactable.InteractionPoint.position);

                if (dist < closestDistance && interactable.CanInteract(this))
                {
                    closestDistance = dist;
                    best = interactable;
                }
            }
        }

        currentTarget = best;
    }
}

Bu sistem şunları karşılar:

✅ Range kontrol
✅ En yakın interactable seçme
✅ Tek target
✅ Input configurable
✅ CanInteract logic

4) Interaction Types Sistemi (Instant / Hold / Toggle)

Bunu düzgün yapmak için:

Base InteractionBehaviour
public abstract class Interaction : MonoBehaviour
{
    public abstract void Execute(Interactor interactor);
}

InstantInteraction
public class InstantInteraction : Interaction
{
    public override void Execute(Interactor interactor)
    {
        Debug.Log("Instant interaction executed!");
    }
}

HoldInteraction
public class HoldInteraction : Interaction
{
    [SerializeField] private float holdTime = 2f;
    private float timer;

    public override void Execute(Interactor interactor)
    {
        timer += Time.deltaTime;

        if (timer >= holdTime)
        {
            Debug.Log("Hold completed!");
            timer = 0;
        }
    }
}


UI Progress bar burada bağlanır.

ToggleInteraction
public class ToggleInteraction : Interaction
{
    private bool state;

    public override void Execute(Interactor interactor)
    {
        state = !state;
        Debug.Log("Toggled: " + state);
    }
}

5) Interactable Base Class (Clean)

Her object bunu inherit eder.

public abstract class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;

    public string Prompt => prompt;

    public virtual Transform InteractionPoint => transform;

    public abstract bool CanInteract(Interactor interactor);

    public abstract void Interact(Interactor interactor);
}

6) Door Example
public class Door : Interactable
{
    [SerializeField] private bool locked;
    [SerializeField] private KeyItem requiredKey;

    private bool open;

    public override bool CanInteract(Interactor interactor)
    {
        if (!locked) return true;

        return interactor.Inventory.HasKey(requiredKey);
    }

    public override void Interact(Interactor interactor)
    {
        if (locked)
        {
            if (!interactor.Inventory.HasKey(requiredKey))
            {
                Debug.Log("Key required!");
                return;
            }

            locked = false;
        }

        open = !open;
        Debug.Log(open ? "Door opened" : "Door closed");
    }
}

7) Switch Event-based Connection
using UnityEngine.Events;

public class Switch : Interactable
{
    public UnityEvent OnToggle;

    public override bool CanInteract(Interactor interactor) => true;

    public override void Interact(Interactor interactor)
    {
        OnToggle?.Invoke();
    }
}


Inspector’dan door.Open() bağlanır.

8) Chest Hold Interaction
public class Chest : Interactable
{
    private bool opened;

    public override bool CanInteract(Interactor interactor)
        => !opened;

    public override void Interact(Interactor interactor)
    {
        opened = true;
        Debug.Log("Chest opened!");
    }
}


Hold interaction UI progress ile bağlanmalı.

9) UI Feedback Sistemi

Interactor currentTarget değişince:

Prompt text güncelle

Hold progress göster

Out of range → hide

if (currentTarget != null)
    promptText.text = currentTarget.Prompt;
else
    promptText.text = "";

**Nasıl Kullandım:**
- [ ] Direkt kullandım
- [x] Adapte ettim
- [ ] Reddettim

**Açıklama:**
Codebase'imin temellerini attı. Bunun üzerine XML dökümantasyonunu ve gerekli naming convention'ı uyguladım. Sistemi color coded anahtar mantığı için revize etmem gerekecekti. Bazı değişiklikler yaptım. Buradaki aynen kullanmadım. 

---

## Prompt 3: [Interactable Detayları]

**Araç:** Cursor - Agent Mode - Auto
**Tarih/Saat:** 2026-01-31 14:11

**Prompt:**
I want to implement dynamic prompts for our interactables. For example, if a lever is on, the prompt should say Press E to turn off. If a door is open, it should say Press E to close the door. To do this, I would like to have two serializable strings for the positive and negative states. The prompt text should automatically get its content based on the current state of the object.

**Alınan Cevap (Özet):**
-Agent Mode'da olduğu için bir cevap vermek yerine direkt kodları kendisi yazdı.

**Nasıl Kullandım:**
- [x] Direkt kullandım
- [ ] Adapte ettim
- [ ] Reddettim

**Açıklama:**
Tüm detayları açıkladığım için direkt olarak implemente etmesini istedim.

---

## Prompt 4: [Etkileşim]

**Araç:** Cursor - Agent Mode - Auto
**Tarih/Saat:** 2026-01-31 14:28

**Prompt:**
I want my interactables to have sounds and animations for toggle. If there is no sound for open or close, then on of them should played. 
Add needed code lines considering our naming convention. 

**Alınan Cevap (Özet):**

Agent olduğu için kodu yazdı.

**Nasıl Kullandım:**
- [x] Direkt kullandım
- [ ] Adapte ettim
- [ ] Reddettim

**Açıklama:**
Beklediğim cevabı aldım ve uyguladım.

---

## Prompt 5: [Props için animasyon: DOTween vs Animator]

**Araç:** Cursor - Agent Mode - Auto
**Tarih/Saat:** 2026-01-31 14:34

**Prompt:**
For my interactable props (doors, chests, switches, keys), should I animate them with DOTween or Unity Animator? I want a clear recommendation and when to use which.

**Alınan Cevap (Özet):**
Use DOTween for simple one-off prop animations (open/close, pickup); use Animator for characters or complex state-driven motion. Can use both in the same project—DOTween for props is the practical choice if it’s already in the project.

**Nasıl Kullandım:**
- [ ] Direkt kullandım
- [ ] Adapte ettim
- [X] Reddettim

**Açıklama:**
Öneriyi reddettim çünkü Animator kullanmak daha rahat ve daha uygun. 

---

## Prompt 6: [Interaction UI Details]

**Araç:** Cursor - Agent Mode - Auto
**Tarih/Saat:** 2026-01-31 18:19

**Prompt:**
bir interactable'a baktığımızda prompt gözüküyor ya o prompt textinin başında objenin adı olsun
obje adı \n prompt gibi bunu y

**Alınan Cevap (Özet):**
Agent modunda olduğu için kendisi uyguladı.

**Nasıl Kullandım:**
- [X] Direkt kullandım
- [ ] Adapte ettim
- [ ] Reddettim

**Açıklama:**
Daha interaktif bir uygulama olması için objelerin isimlerini de göstermek istedim. Diğer parçalarla uğraşırken bu kısmı Cursor'a yaptırdım.

---

## Prompt 7: [Interaction UI Details]

**Araç:** Cursor - Agent Mode - Auto
**Tarih/Saat:** 2026-01-31 18:19

**Prompt:**

şuan gold key alınınca gol key yazıyor oysa biz burada gold key alındıysa gold key alındı, etkileşimli kapı açıldıysa etkileşimlikapı açıldı falan dememiz lazım. bunu en iyi hangi mimari sağlar burada?

**Alınan Cevap (Özet):**
Bunu en iyi olay türü + merkezi formatter mimarisi sağlar. Her olayı bir enum ile (örn. anahtar alındı, kapı açıldı, sandıktan item alındı) tanımlarsın; tüm kullanıcıya dönük cümleleri tek bir formatter sınıfında format string’lerle tutarsın. Caller’lar sadece “ne oldu” (kind) ve “neyin adı” (subjectName) verir; cümleyi formatter üretir. Böylece “Gold Key alındı”, “Etkileşimli Kapı açıldı” gibi metinler tek yerden gelir, dil değişimi ve yeni olay eklemek kolaylaşır. Interactable’lar ham string üretmez, sadece olay türü ve isim geçer; UI metni merkezi kalır.

**Nasıl Kullandım:**
- [ ] Direkt kullandım
- [X] Adapte ettim
- [ ] Reddettim

**Açıklama:**
ItemInfoKind enum'ı (KeyPickedUp, DoorOpened, ChestItemReceived vb.) eklendi; ItemInfoFormatter.GetMessage(kind, subjectName) ile tüm kullanıcıya dönük cümleler tek yerden üretiliyor. KeyPickup ve Door gibi yerlerde Interactor.ShowItemInfo(ItemInfoKind.KeyPickedUp, key.KeyName) veya ShowItemInfo(ItemInfoKind.DoorOpened, ObjectName) çağrılıyor. Böylece "Gold Key alındı", "Etkileşimli Kapı açıldı" metinleri merkezi formatter'dan geliyor; yeni olay veya dil değişimi tek dosyadan yönetilebiliyor.

## Prompt 7: [Interaction UI Details]

**Araç:** Cursor - Agent Mode - Auto
**Tarih/Saat:** 2026-01-31 19:46

**Prompt:**

save load interaction states ile kastedilen nedir ve nasıl implemente edebilirim yazar mısın? 

**Alınan Cevap (Özet):**
Oyun kapatılıp tekrar açıldığında (veya sahne yeniden yüklendiğinde) etkileşimli nesnelerin durumlarının korunması: sandık açıksa yüklemede de açık, kapı açıksa açık, toplanan anahtar tekrar sahnede olmasın.
Hangi state’ler?
Tablo ile: Door (açık/kapalı, kilitli), Chest (consumed), KeyPickup (toplandı mı), Switch (açık/kapalı), Inventory (hangi key’ler).
Nasıl implemente edilir?
1) Her interactable’a kaydedilebilir benzersiz ID.
2) Tüm state’leri toplayan bir DTO (örn. InteractionSaveData) ve JSON serialize.
3) Kaydetme: state’leri topla, dosyaya/PlayerPrefs’e yaz.
4) Yükleme: sahne yüklendikten sonra veriyi oku, ID’ye göre eşleştir, ilgili interactable’lara state’i uygula.
5) Basit yol: tek JSON veya PlayerPrefs + sahne load sonrası bir loader script’i.
Bu bölüm bonus tablosunun hemen altında, "Repository Yapısı"ndan önce yer alıyor.

**Nasıl Kullandım:**
- [ ] Direkt kullandım
- [X] Adapte ettim
- [ ] Reddettim

**Açıklama:**
SaveManager eklendi; InteractionSaveData DTO ile Door (isOpen, isLocked), Chest (consumed), Switch (isOn), KeyPickup (collected) ve Inventory (keyNames) state'leri JSON olarak toplanıp PlayerPrefs'e yazılıyor. Yüklemede sahne/menü sonrası Load() çağrılıyor, ID (GetSaveId()) ile eşleşen interactable'lara LoadState(json) uygulanıyor, Inventory.LoadState(keyNames) ile anahtar listesi geri yükleniyor. LoadState sonrası animator ile görsel uyum için Interactable.SyncAnimatorToState çağrılıyor; böylece kapı/lever/chest kayıtta açıksa yüklemede de açık pozu gösteriliyor.

## Prompt 8: [Error Handling]

**Araç:** ChatGPT 4
**Tarih/Saat:** 2026-01-31 20:06

**Prompt:**

bizim projemizde nerelere error handlig kullanabiliriz?

**Alınan Cevap (Özet):**

Ludu Arts: **Silent bypass yapılmamalı; hatalar loglanmalı veya fırlatılmalı.**

Aşağıda projede error handling eklenebilecek yerler listelenmiştir. `return` veya `return false` ile sessizce geçen null/geçersiz durumlar **Debug.LogWarning** / **Debug.LogError** ile loglanmalıdır.

---

1. Inventory

| Yer | Durum | Öneri |
|-----|--------|--------|
| `AddItem(ScriptableObject item)` | `item == null` → sessiz return | `Debug.LogWarning("[Inventory] AddItem: item is null.")` |
| `HasItem(null)` / `HasKey(null)` | Return false | İsteğe bağlı: `Debug.LogWarning` (çağıran yanlış kullanıyorsa) |

---

2. Interactor

| Yer | Durum | Öneri |
|-----|--------|--------|
| `FindBestInteractable()` | `m_ViewCamera == null` → return | `Debug.LogWarning("[Interactor] View camera is null; interaction detection disabled.")` |
| `TryExecuteInteraction()` | `m_CurrentTarget == null` → return | Akış kontrolü; log gerekmez. |
| `GetInteractionFromTarget()` | `interactable?.InteractionPoint == null` → return null | İsteğe bağlı: InteractionPoint null ise `Debug.LogWarning` (misconfiguration). |

---

3. SaveManager

| Yer | Durum | Öneri |
|-----|--------|--------|
| `Load()` | `!PlayerPrefs.HasKey(k_SaveKey)` → return | Normal (henüz kayıt yok); log gerekmez. |
| `Load()` | `JsonUtility.FromJson` sonrası `data == null` → return | `Debug.LogError("[SaveManager] Corrupt or invalid save data; load aborted.")` |

---

4. LoadState (Door, Chest, KeyPickup, Switch)

| Yer | Durum | Öneri |
|-----|--------|--------|
| Her birinde `string.IsNullOrEmpty(json)` → return | Geçersiz/boş JSON | `Debug.LogWarning($"[{GetType().Name}] LoadState: empty or null json for id={GetSaveId()}.")` |

---

5. Interactable (base)

| Yer | Durum | Öneri |
|-----|--------|--------|
| `SyncAnimatorToState()` | `m_Animator == null` → return | İsteğe bağlı: `Debug.LogWarning` (animator bekleniyorsa). |
| `PlayInteractionSound()` | `m_AudioSource == null` → return | Sessiz; isteğe bağlı log. |

---

6. Door

| Yer | Durum | Öneri |
|-----|--------|--------|
| `Interact(IInteractor interactor)` | `interactor == null` veya `interactor.Inventory == null` → return | `Debug.LogWarning("[Door] Interact: interactor or Inventory is null.")` |

---

7. KeyPickup

| Yer | Durum | Öneri |
|-----|--------|--------|
| `Interact(IInteractor interactor)` | `interactor == null` / `m_KeyData == null` → return | `Debug.LogWarning("[KeyPickup] Interact: interactor or KeyData is null.")` |

---

8. UI

| Yer | Durum | Öneri |
|-----|--------|--------|
| **InventoryUI** | `m_Interactor == null` veya `m_ListText == null` → return | `Debug.LogWarning("[InventoryUI] Interactor or ListText not assigned.")` |
| **InfoDisplay.ShowInfo()** | `m_InfoText == null` → return | Awake’te zaten uyarı var; ShowInfo’da da tekrar `Debug.LogWarning` eklenebilir. |
| **InteractionPrompt** | `m_Interactor == null` / `currentTarget == null` → SetVisible(false); return | Akış kontrolü; isteğe bağlı bir kez log. |

---

Öncelik

1. **Yüksek:** Null/geçersiz parametre (AddItem, Interact, Load corrupt data) → **LogWarning/LogError**.
2. **Orta:** Eksik referans (camera, animator, UI ref) → **LogWarning**.
3. **Düşük:** Normal akış (hedef yok, kayıt yok) → log gerekmez.
**Nasıl Kullandım:**
- [ ] Direkt kullandım
- [X] Adapte ettim
- [ ] Reddettim

**Açıklama:**
Debug.LogWarning ile eklemeler yapmayı uygun buldu. Try-Catch kullanabileceğim pek bir durum olmadığı için bununla yetinmek bana da doğru geldi. Verdiği önerileri izledim. 



## Genel Değerlendirme

### LLM'in En Çok Yardımcı Olduğu Alanlar
1. Naming Convention ve File Structure
2. XML Dokümantasyonu
3. Hızlıca bu proje nasıl biter konusunda fikir alışverişi

### LLM'in Yetersiz Kaldığı Alanlar
1. Animasyon ve state storing konusunda çok yetersiz kaldı, o kısımlarda yardım aldığım için kodunu değiştiremedim zamanım kalmamıştı bu yüzden bug'a sahibiz.

### LLM Kullanımı Hakkında Düşüncelerim
Unity ile çalışırken LLM kullanmaktan nefret ediyorum. Logic kısmına dahil ettikten sonra işin içinden çıkılmayacak kadar saçma yerlere getiriyor kodu. Sanırım en başından bir tasarım şeması verip buna uymasını isteyebiliriz. Fakat bunu birçok kez denedim, istediğim sonucu hiç alamadım. Mobil geliştiricilerin aksine oyun geliştiricilerin mesleklerinin ellerinden gitmesine bir süre daha var gibi duruyor.

LLM kullanmasam da aynı sürede aynı işi yapardım. Belki maksimum 2 saate daha ihityacım olurdu. Emin olamıyorum. Case'in isterleri çok belirliydi aynı sürede yapabileceğime inanıyorum.

Bu projede beni naming convention, asset olmaması ve ortada tasarlanacak tam bir oyun olmaması çok zorladı. Bunlar olduğu durumda kaygılanacağım ya da düşüneceğim temel şey codebase'im olacağı için elimde kağıt ve kalemle birden fazla agent kullanarak kod yükünü LLM'lere yükleyebileceğimi düşünüyorum.
---
