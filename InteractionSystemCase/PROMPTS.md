# LLM Kullanım Dokümantasyonu

## Özet
- Toplam prompt sayıs: X
- Kullanılan araçlar: ChatGPT / Claude / Copilot
- En çok yardım alınan konular: [liste]

---

## Prompt 1: [Ana Tasarım]

**Araç:** ChatGPT-4
**Tarih/Saat:** YYYY-MM-DD 12:32

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
**Tarih/Saat:** YYYY-MM-DD 13:07

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
Codebase'imin temellerini attı. Bunun üzerine XML dökümantasyonunu ve gerekli naming convention'ı uyguladım.

---

## Prompt 3: [Interactable Detayları]

**Araç:** Cursor - Agent Mode - Auto
**Tarih/Saat:** YYYY-MM-DD 14:11

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
**Tarih/Saat:** YYYY-MM-DD 14:28

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
**Tarih/Saat:** 2025-01-31 14:34

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

