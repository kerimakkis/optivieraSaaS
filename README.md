# Optiviera ERP

**Küçük İşletmeler İçin Eko Seviye ERP Çözümü**

![Optiviera ERP](https://img.shields.io/badge/Optiviera-ERP-blue)
![License](https://img.shields.io/badge/License-AkkisTech-green)
![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20macOS%20%7C%20Linux-lightgrey)
![Languages](https://img.shields.io/badge/Languages-8%20Languages-orange)

## 🚀 Özellikler

### ✨ **Ana Özellikler**
- **🎯 Tek Tıkla Kurulum**: Otomatik kurulum ve veritabanı oluşturma
- **🌍 8 Dil Desteği**: Türkçe, İngilizce, Almanca, Fransızca, İspanyolca, İtalyanca, Portekizce, Hollandaca
- **🔐 Güvenli Lisans Sistemi**: Makine bazlı lisans, otomatik aktivasyon
- **📊 Gelişmiş Raporlama**: Chart.js ile interaktif grafikler
- **👥 Kullanıcı Yönetimi**: Rol bazlı erişim kontrolü
- **🎫 Ticket Sistemi**: Müşteri destek talebi yönetimi

### 🛠️ **Teknik Özellikler**
- **ASP.NET Core 6.0** - Modern web framework
- **SQLite Database** - Self-contained veritabanı
- **Entity Framework Core** - ORM ve migration desteği
- **ASP.NET Identity** - Güvenli kullanıcı yönetimi
- **Electron Desktop App** - Cross-platform masaüstü uygulaması
- **Bootstrap 5** - Responsive ve modern UI
- **Font Awesome 6** - Zengin ikon kütüphanesi

## 📦 Kurulum

### 🖥️ **Desktop Uygulaması (Önerilen)**

#### Windows
```bash
# İndir ve çalıştır
Optiviera ERP Setup 1.0.0.exe
```

#### macOS
```bash
# DMG dosyasını aç ve uygulamayı Applications klasörüne sürükle
Optiviera ERP-1.0.0.dmg
```

#### Linux
```bash
# AppImage dosyasını çalıştırılabilir yap ve çalıştır
chmod +x Optiviera\ ERP-1.0.0-arm64.AppImage
./Optiviera\ ERP-1.0.0-arm64.AppImage
```

### 🌐 **Web Uygulaması**

#### Gereksinimler
- .NET 6.0 Runtime
- SQLite (otomatik oluşturulur)

#### Kurulum
```bash
# Repository'yi klonla
git clone https://github.com/akkistech/optiviera.git
cd optiviera

# Bağımlılıkları yükle
dotnet restore

# Veritabanını oluştur
dotnet ef database update

# Uygulamayı çalıştır
dotnet run
```

## 🎯 Kullanım

### 🚀 **İlk Kurulum**
1. **İndir**: Platformunuza uygun dosyayı indirin
2. **Kur**: Kurulum sihirbazını takip edin
3. **Başlat**: Uygulama otomatik olarak açılacak
4. **Lisans**: 1 yıllık ücretsiz trial otomatik aktif

### 👤 **Kullanıcı Yönetimi**
- **Admin**: Tam erişim, kullanıcı yönetimi
- **Manager**: Raporlama ve ticket yönetimi
- **Employee**: Temel işlemler

### 🎫 **Ticket Sistemi**
- Müşteri bilgileri
- Öncelik seviyeleri
- Durum takibi
- Teknik atama

### 📊 **Raporlama**
- Ticket istatistikleri
- Kullanıcı performansı
- Öncelik dağılımı
- Zaman bazlı analizler

## 🌍 Çok Dilli Destek

| Dil | Kod | Durum |
|-----|-----|-------|
| 🇹🇷 Türkçe | tr | ✅ Tam |
| 🇺🇸 English | en | ✅ Tam |
| 🇩🇪 Deutsch | de | ✅ Tam |
| 🇫🇷 Français | fr | ✅ Tam |
| 🇪🇸 Español | es | ✅ Tam |
| 🇮🇹 Italiano | it | ✅ Tam |
| 🇵🇹 Português | pt | ✅ Tam |
| 🇳🇱 Nederlands | nl | ✅ Tam |

## 🔐 Lisans Sistemi

### 🎁 **Trial Lisans**
- **Süre**: 1 yıl (365 gün)
- **Özellikler**: Tam erişim
- **Aktivasyon**: Otomatik
- **Grace Period**: 7 gün ek süre

### 💳 **Satın Alma**
- **Website**: [akkistech.com/optiviera](https://akkistech.com/optiviera)
- **İletişim**: support@akkistech.com
- **Ödeme**: Güvenli ödeme sistemi

## 🛠️ Geliştirme

### 📋 **Gereksinimler**
- .NET 6.0 SDK
- Node.js 16+
- Git

### 🚀 **Geliştirme Ortamı**
```bash
# Repository'yi klonla
git clone https://github.com/akkistech/optiviera.git
cd optiviera

# Backend bağımlılıkları
cd Optiviera
dotnet restore
dotnet ef database update

# Frontend bağımlılıkları
cd ../electron
npm install

# Geliştirme modunda çalıştır
npm run dev
```

### 🏗️ **Build**
```bash
# ASP.NET Core build
dotnet publish -c Release -r win-x64 --self-contained true

# Electron build
npm run build
```

## 📁 Proje Yapısı

```
Optiviera/
├── Optiviera/                 # ASP.NET Core uygulaması
│   ├── Controllers/            # MVC Controllers
│   ├── Models/               # Veri modelleri
│   ├── Views/                # Razor Views
│   ├── Services/             # İş mantığı servisleri
│   ├── Data/                 # Entity Framework
│   └── Resources/            # Çok dilli kaynaklar
├── electron/                 # Electron desktop app
│   ├── main.js              # Ana process
│   ├── preload.js           # Preload script
│   └── package.json         # Node.js bağımlılıkları
├── hosting/                  # Web hosting dosyaları
│   ├── index.html           # Ana sayfa
│   └── downloads/           # İndirilebilir dosyalar
└── build/                   # Build çıktıları
    ├── win-x64/            # Windows build
    └── osx-arm64/          # macOS build
```

## 🤝 Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Commit yapın (`git commit -m 'Add amazing feature'`)
4. Push yapın (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

## 📞 İletişim

- **Website**: [akkistech.com](https://akkistech.com)
- **E-posta**: support@akkistech.com
- **GitHub**: [github.com/akkistech/optiviera](https://github.com/akkistech/optiviera)

## 📄 Lisans

Bu proje Akkis Technologies (AkkisTech) tarafından lisanslanmıştır.

**Copyright © 2025 Akkis Technologies (AkkisTech) - Kerim Akkis**

### Lisans Koşulları
- ✅ **Trial**: 1 yıl ücretsiz kullanım
- ✅ **Ticari Kullanım**: Lisans satın alındıktan sonra
- ✅ **Dağıtım**: Lisanslı kullanıcılar için
- ❌ **Reverse Engineering**: Yasak
- ❌ **Kaynak Kod Dağıtımı**: Yasak

---

**Optiviera ERP** - Küçük işletmeler için modern ERP çözümü 🚀