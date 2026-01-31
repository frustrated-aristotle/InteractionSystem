# Objects Klasör Yapısı ve İsimlendirme

Bu doküman, `Assets/InteractionSystem/Objects` altındaki klasör yapısı ve isimlendirme kurallarını özetler. Detaylar için `MDs/Prefab_Asset_Kurallari.md` ve `MDs/Naming_Convention_Kilavuzu.md` kullanılmalıdır.

---

## Klasör Hiyerarşisi

```
Objects/
├── Architecture/
│   └── {BuildingName}/
└── Props/
    └── {PropName}/
```

- **Architecture:** Bina, duvar, zemin gibi mimari asset'ler.
- **Props:** Kapı, sandık, anahtar, fener gibi taşınabilir / etkileşimli objeler.

---

## Prop Klasör Yapısı (Standart)

Her prop için PascalCase klasör adı ve tutarlı alt klasörler:

```
📁 Props/{PropName}/
├── Animations/          # A_*, AC_* (animasyon ve controller)
├── Materials/            # M_* (materyaller)
├── Textures/            # T_* (texture'lar; _BC, _N, _AO vb. suffix)
├── Source/              # Ham mesh: .fbx, .obj (isteğe bağlı)
│   └── Materials/       # FBX ile gelen materyaller burada da olabilir
└── P_{PropName}.prefab # Prefab (varsa)
```

### Klasör İsimlendirme

| Kural | Örnek |
|-------|--------|
| Tüm klasörler **PascalCase** | `Textures`, `Source`, `Materials` |
| Küçük harfli klasör kullanılmaz | ~~textures~~, ~~source~~ |

---

## Asset İsimlendirme

| Tür | Prefix / Kural | Örnek |
|-----|----------------|--------|
| Material | `M_` | `M_Chest_Full`, `M_Harpoon`, `M_Clay_01` |
| Texture | `T_` + suffix (_BC, _N, _AO, _MS) | `T_Chest_BC`, `T_Chest_N` |
| Prefab | `P_` | `P_Door`, `P_Chest` |
| Static Mesh | `SM_` | `SM_Key_Rusted_01` |
| Skeletal Mesh | `SK_` | `SK_HandLever` |
| Animation Clip | `A_` | `A_Chest_Open` |
| Animator Controller | `AC_` | `AC_Chest` |

---

## Mevcut Prop'lar (Düzenleme Sonrası)

- **Chest** – Animations, Source, Textures, Source/Materials
- **Door** – Animations, prefab, FBX
- **DungeonKit** – Materials (M_*), level_0.fbx, Door alt prefab
- **HandLever** – Animations, Materials, Textures, SK_HandLever.fbx
- **Harpoon** – Source, Textures, Source/Materials
- **Key** – Materials (M_Key, M_Key_Gold), Textures, SM_Key_Rusted_01.obj
- **Lantern** – Source, Textures
- **Launcher** – Source, Textures, Source/Materials

---

## Yeni Prop Ekleme Kontrol Listesi

- [ ] Klasör adı PascalCase (`{PropName}`)
- [ ] `Textures` ve `Source` klasörleri küçük harfsiz (PascalCase)
- [ ] Materyaller `M_` prefix ile
- [ ] Prefab varsa `P_{PropName}.prefab`
- [ ] Mesh: `SM_` veya `SK_` prefix (statik / skeletal)
