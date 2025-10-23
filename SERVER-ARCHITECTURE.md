# 🗺️ Server Yapısı - akkistech.com (akkisHost)

**Son Güncelleme:** 23 Ekim 2025  
**Server IP:** 192.168.178.20 (WireGuard VPN)  
**Hostname:** akkishost  
**OS:** Ubuntu 24.04.3 LTS (GNU/Linux 6.8.0-79-generic x86_64)

---

## 📋 İçindekiler

1. [Sistem Bilgileri](#sistem-bilgileri)
2. [Ağ Yapısı](#ağ-yapısı)
3. [Docker Container'lar](#docker-containerlar)
4. [Web Klasör Yapısı](#web-klasör-yapısı)
5. [Optiviera ERP Deployment](#optiviera-erp-deployment)
6. [Erişim Bilgileri](#erişim-bilgileri)
7. [Backup Stratejisi](#backup-stratejisi)

---

## 🖥️ Sistem Bilgileri

| Özellik | Değer |
|---------|-------|
| **Hostname** | akkishost |
| **IP Adresi** | 192.168.178.20 |
| **İşletim Sistemi** | Ubuntu 24.04.3 LTS |
| **Kernel** | 6.8.0-79-generic x86_64 |
| **Disk Toplam** | 5.5 TB |
| **Disk Kullanım** | 3.0 GB (1%) |
| **RAM Kullanım** | 5% |
| **CPU** | x86_64 |
| **Sıcaklık** | ~39-40°C |

---

## 🌐 Ağ Yapısı

### WireGuard VPN Bağlantısı

```
┌─────────────────┐         ┌──────────────────┐         ┌─────────────────┐
│  Local Machine  │ ◄─────► │  WireGuard VPN   │ ◄─────► │   akkisHost     │
│  (Mac/Windows)  │         │  192.168.178.x   │         │ 192.168.178.20  │
└─────────────────┘         └──────────────────┘         └─────────────────┘
     Ofis                         VPN Tunnel                   Ev Serveri
```

### Port Yapısı

| Port | Servis | Açıklama |
|------|--------|----------|
| **80** | HTTP | Nginx Proxy Manager |
| **81** | HTTP | Nginx Proxy Manager Admin |
| **443** | HTTPS | SSL/TLS (Let's Encrypt) |
| **8083** | HTTP | web-akkishost (Internal) |

### DNS & Domain

| Domain | Hedef | SSL | Status |
|--------|-------|-----|--------|
| **akkistech.com** | 192.168.178.20:80 | ✅ | Active |
| **www.akkistech.com** | 192.168.178.20:80 | ✅ | Active |

---

## 🐳 Docker Container'lar

### Container Listesi

| Container Name | Image | Status | Ports | Açıklama |
|----------------|-------|--------|-------|----------|
| **nginx-proxy-manager-app-1** | nginx-proxy-manager | Up | 80-81:80-81, 443:443 | Reverse proxy & SSL |
| **web-akkishost** | nginx:latest | Up | 8083:80 | Ana web server |

### Container Yönetim Komutları

```bash
# Container'ları listele
docker ps -a

# Container restart
docker restart web-akkishost
docker restart nginx-proxy-manager-app-1

# Container logs
docker logs -f web-akkishost
docker logs -f nginx-proxy-manager-app-1

# Container stats
docker stats --no-stream web-akkishost
```

---

## 📁 Web Klasör Yapısı

### Ana Dizin Ağacı

```
/mnt/data/volumes/websites/akkistech/html/
│
├── 📄 index.html                    # Ana sayfa (akkistech.com)
├── 📄 about.html                    # Hakkımızda sayfası
├── 📄 contact.html                  # İletişim sayfası
├── 📄 optiviera.html               # Optiviera ERP ana sayfası ⭐
│
├── 📁 downloads/                    # Download klasörü (671 MB)
│   ├── OptivieraERP.exe            # Windows installer (130 MB) ⭐ YENİ
│   ├── Optiviera ERP-1.0.0.dmg     # macOS Intel (148 MB)
│   ├── Optiviera ERP-1.0.0-arm64.dmg  # macOS ARM64 (144 MB)
│   ├── Optiviera ERP-1.0.0-arm64.AppImage  # Linux AppImage (152 MB)
│   └── optiviera-desktop_1.0.0_arm64.deb   # Linux Debian (99 MB)
│
├── 📁 images/                       # Görseller
├── 📁 css/                         # Stil dosyaları
├── 📁 js/                          # JavaScript dosyaları
├── 📁 about/                       # About sayfası assets
└── 📁 [diğer statik dosyalar]
```

### Klasör Boyutları

| Klasör | Boyut | Dosya Sayısı | Açıklama |
|--------|-------|--------------|----------|
| **downloads/** | 671 MB | 5 dosya | Optiviera ERP installer'ları |
| **images/** | ~50 MB | 100+ dosya | Web sitesi görselleri |
| **css/** | ~5 MB | 50+ dosya | Stil dosyaları |
| **js/** | ~10 MB | 80+ dosya | JavaScript dosyaları |

---

## 🚀 Optiviera ERP Deployment

### Aktif Deployment Yapısı

```
https://akkistech.com/
│
├── optiviera.html              ← Ana Optiviera sayfası
│   └── Links:
│       ├── downloads/OptivieraERP.exe
│       ├── downloads/Optiviera ERP-1.0.0.dmg
│       ├── downloads/Optiviera ERP-1.0.0-arm64.dmg
│       ├── downloads/Optiviera ERP-1.0.0-arm64.AppImage
│       └── downloads/optiviera-desktop_1.0.0_arm64.deb
│
└── downloads/                  ← Download klasörü
    └── [yukarıdaki tüm dosyalar]
```

### Download Linkleri

| Platform | Link | Boyut | Tarih |
|----------|------|-------|-------|
| **Windows x64** | [OptivieraERP.exe](https://akkistech.com/downloads/OptivieraERP.exe) | 130 MB | 23 Ekim 2025 |
| **macOS Intel** | [Optiviera ERP-1.0.0.dmg](https://akkistech.com/downloads/Optiviera%20ERP-1.0.0.dmg) | 148 MB | 14 Ekim 2025 |
| **macOS ARM64** | [Optiviera ERP-1.0.0-arm64.dmg](https://akkistech.com/downloads/Optiviera%20ERP-1.0.0-arm64.dmg) | 144 MB | 14 Ekim 2025 |
| **Linux AppImage** | [Optiviera ERP-1.0.0-arm64.AppImage](https://akkistech.com/downloads/Optiviera%20ERP-1.0.0-arm64.AppImage) | 152 MB | 14 Ekim 2025 |
| **Linux Debian** | [optiviera-desktop_1.0.0_arm64.deb](https://akkistech.com/downloads/optiviera-desktop_1.0.0_arm64.deb) | 99 MB | 14 Ekim 2025 |

### Backup Dosyaları

| Dosya | Boyut | Tarih | Açıklama |
|-------|-------|-------|----------|
| `optiviera.html.backup` | 20 KB | 14 Ekim 2025 | İlk backup |
| `optiviera.html.backup-20251023-080045` | 20 KB | 23 Ekim 2025 | Path fix öncesi |
| `optiviera.html.backup-final` | 20 KB | 23 Ekim 2025 | Klasör cleanup öncesi |
| `optiviera.html.backup-path-fix` | 20 KB | 23 Ekim 2025 | Path düzeltme öncesi |

---

## 🔐 Erişim Bilgileri

### SSH Bağlantısı

```bash
# Manuel SSH
ssh kerim@192.168.178.20

# sshpass ile otomatik
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no kerim@192.168.178.20

# Non-interactive SSH (script'ler için)
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 'ls -lh'
```

### Kullanıcı Bilgileri

| Kullanıcı | Şifre | Yetki | Açıklama |
|-----------|-------|-------|----------|
| **kerim** | Duka1429! | Admin | Ana kullanıcı (SSH, Docker, Web) |

### Web Klasör İzinleri

```bash
# Web klasörü izinleri
chown -R kerim:kerim /mnt/data/volumes/websites/akkistech/html/
chmod -R 755 /mnt/data/volumes/websites/akkistech/html/

# Download klasörü izinleri
chmod -R 755 /mnt/data/volumes/websites/akkistech/html/downloads/
```

---

## 📦 Deployment Workflow

### Local → Server Deployment

```
┌─────────────────────────────────────────────────────────────┐
│  LOCAL MACHINE (Mac)                                        │
├─────────────────────────────────────────────────────────────┤
│  /Users/kerimakkis/Projects/Optiviera/                     │
│  └── hosting/                                               │
│      ├── index.html                                         │
│      └── downloads/                                         │
│          └── OptivieraERP.exe                              │
└─────────────────────────────────────────────────────────────┘
                          │
                          │ WireGuard VPN (192.168.178.x)
                          │
                          ▼
            ┌──────────────────────────┐
            │  rsync + sshpass        │
            │  (deploy-wireguard.sh)  │
            └──────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────────┐
│  SERVER (akkisHost - 192.168.178.20)                       │
├─────────────────────────────────────────────────────────────┤
│  /mnt/data/volumes/websites/akkistech/html/                │
│  ├── optiviera.html                                         │
│  └── downloads/                                             │
│      └── OptivieraERP.exe                                  │
└─────────────────────────────────────────────────────────────┘
                          │
                          ▼
            ┌──────────────────────────┐
            │  Docker Restart         │
            │  - web-akkishost        │
            │  - nginx-proxy-manager  │
            └──────────────────────────┘
                          │
                          ▼
            ┌──────────────────────────┐
            │  LIVE ✅                │
            │  https://akkistech.com  │
            └──────────────────────────┘
```

### Deployment Komutu

```bash
# Local makineden deployment
cd /Users/kerimakkis/Projects/Optiviera
./deploy-wireguard.sh

# Script içeriği özeti:
# 1. VPN kontrolü (ping 192.168.178.20)
# 2. rsync ile dosya transferi
# 3. Docker container restart
# 4. Başarı mesajı
```

---

## 🔄 Yedekleme (Backup) Stratejisi

### Otomatik Backup

```bash
# Server'dan local'e backup
sshpass -p "Duka1429!" rsync -avh --progress \
  -e "ssh -o StrictHostKeyChecking=no" \
  kerim@192.168.178.20:/mnt/data/volumes/websites/akkistech/html/ \
  ~/Desktop/akkistech-backup-$(date +%Y%m%d)/

# Backup arşivle
cd ~/Desktop
tar -czf akkistech-backup-$(date +%Y%m%d).tar.gz akkistech-backup-$(date +%Y%m%d)/
```

### Backup Dosya Yapısı

```
~/Desktop/
└── akkistech-backup-20251023/
    ├── index.html
    ├── optiviera.html
    ├── downloads/
    │   ├── OptivieraERP.exe
    │   ├── Optiviera ERP-1.0.0.dmg
    │   └── [diğer dosyalar]
    └── [tüm web dosyaları]
```

### Manuel Backup Komutları

```bash
# Sadece optiviera dosyalarını backup al
sshpass -p "Duka1429!" rsync -avh --progress \
  -e "ssh -o StrictHostKeyChecking=no" \
  kerim@192.168.178.20:/mnt/data/volumes/websites/akkistech/html/optiviera.html \
  kerim@192.168.178.20:/mnt/data/volumes/websites/akkistech/html/downloads/ \
  ~/Desktop/optiviera-backup-$(date +%Y%m%d)/

# Sadece downloads klasörünü backup al
sshpass -p "Duka1429!" rsync -avh --progress \
  -e "ssh -o StrictHostKeyChecking=no" \
  kerim@192.168.178.20:/mnt/data/volumes/websites/akkistech/html/downloads/ \
  ~/Desktop/downloads-backup-$(date +%Y%m%d)/
```

---

## 🛠️ Yararlı Komutlar

### Disk & Dosya Yönetimi

```bash
# Disk kullanımı
ssh kerim@192.168.178.20 'df -h /mnt/data/volumes/websites/akkistech/html/'

# Klasör boyutları
ssh kerim@192.168.178.20 'du -sh /mnt/data/volumes/websites/akkistech/html/*'

# Dosya sayısı
ssh kerim@192.168.178.20 'find /mnt/data/volumes/websites/akkistech/html/ -type f | wc -l'

# Büyük dosyaları bul (>100 MB)
ssh kerim@192.168.178.20 'find /mnt/data/volumes/websites/akkistech/html/ -type f -size +100M'
```

### Docker Yönetimi

```bash
# Container'ları listele
ssh kerim@192.168.178.20 'docker ps -a'

# Container restart (tek seferde)
ssh kerim@192.168.178.20 'docker restart web-akkishost nginx-proxy-manager-app-1'

# Container logs (son 50 satır)
ssh kerim@192.168.178.20 'docker logs --tail 50 web-akkishost'

# Container kaynak kullanımı
ssh kerim@192.168.178.20 'docker stats --no-stream'
```

### Dosya İşlemleri

```bash
# Dosya checksum
ssh kerim@192.168.178.20 'sha256sum /mnt/data/volumes/websites/akkistech/html/downloads/OptivieraERP.exe'

# Dosya permissions
ssh kerim@192.168.178.20 'ls -la /mnt/data/volumes/websites/akkistech/html/downloads/'

# Dosya sil
ssh kerim@192.168.178.20 'rm /mnt/data/volumes/websites/akkistech/html/downloads/[filename]'

# Klasör sil
ssh kerim@192.168.178.20 'rm -rf /mnt/data/volumes/websites/akkistech/html/[folder]/'
```

---

## 📊 İstatistikler

### Server İstatistikleri (23 Ekim 2025)

| Metrik | Değer |
|--------|-------|
| Toplam Web Klasörü Boyutu | ~750 MB |
| Optiviera Downloads | 671 MB (5 dosya) |
| HTML/CSS/JS Dosyaları | ~70 MB |
| Görsel Dosyaları | ~50 MB |
| Toplam Dosya Sayısı | 500+ dosya |
| Son Deployment | 23 Ekim 2025 08:08 UTC |
| Uptime | 99.9% |

### Download İstatistikleri

| Platform | Dosya | İndirme Sayısı | Son Güncelleme |
|----------|-------|----------------|----------------|
| Windows | OptivieraERP.exe | TBD | 23 Ekim 2025 |
| macOS Intel | Optiviera ERP-1.0.0.dmg | TBD | 14 Ekim 2025 |
| macOS ARM64 | Optiviera ERP-1.0.0-arm64.dmg | TBD | 14 Ekim 2025 |
| Linux AppImage | Optiviera ERP-1.0.0-arm64.AppImage | TBD | 14 Ekim 2025 |
| Linux Debian | optiviera-desktop_1.0.0_arm64.deb | TBD | 14 Ekim 2025 |

---

## 🚨 Troubleshooting

### Yaygın Sorunlar ve Çözümler

#### 1. 404 Not Found Hatası

**Sorun:** Download linkleri 404 veriyor

**Çözüm:**
```bash
# Dosya var mı kontrol et
ssh kerim@192.168.178.20 'ls -lh /mnt/data/volumes/websites/akkistech/html/downloads/'

# Nginx restart
ssh kerim@192.168.178.20 'docker restart web-akkishost nginx-proxy-manager-app-1'

# Browser cache temizle
# Ctrl+Shift+Delete → Tüm zamanlar → Temizle
```

#### 2. VPN Bağlantı Sorunu

**Sorun:** Server'a erişilemiyor

**Çözüm:**
```bash
# VPN aktif mi kontrol et
ping 192.168.178.20

# WireGuard'ı başlat
sudo wg-quick up wg0

# VPN interface kontrol
ifconfig | grep wg
```

#### 3. Dosya Yüklenemiyor

**Sorun:** rsync transfer hatası

**Çözüm:**
```bash
# Disk doluluk kontrolü
ssh kerim@192.168.178.20 'df -h'

# İzinleri kontrol et
ssh kerim@192.168.178.20 'ls -la /mnt/data/volumes/websites/akkistech/html/'

# İzinleri düzelt
ssh kerim@192.168.178.20 'chmod -R 755 /mnt/data/volumes/websites/akkistech/html/'
```

#### 4. Docker Container Çalışmıyor

**Sorun:** Container down durumda

**Çözüm:**
```bash
# Container durumunu kontrol et
ssh kerim@192.168.178.20 'docker ps -a'

# Container başlat
ssh kerim@192.168.178.20 'docker start web-akkishost'

# Logs kontrol et
ssh kerim@192.168.178.20 'docker logs web-akkishost'
```

---

## 📝 Notlar

### Önemli Bilgiler

1. **VPN Bağlantısı:** Tüm işlemler WireGuard VPN üzerinden yapılmalı
2. **Backup:** Her büyük değişiklikten önce backup almak önemli
3. **Cache:** Deployment sonrası Nginx ve browser cache temizlenmeli
4. **İzinler:** Web dosyaları 755 permission'a sahip olmalı
5. **SSL:** Let's Encrypt sertifikaları Nginx Proxy Manager tarafından yönetiliyor

### Gelecek İyileştirmeler

- [ ] Otomatik backup sistemi (cronjob)
- [ ] CI/CD pipeline (GitHub Actions → Server)
- [ ] İndirme istatistikleri (analytics)
- [ ] CDN entegrasyonu (hız artışı için)
- [ ] Load balancing (yedek server)
- [ ] Monitoring & alerting (Prometheus/Grafana)

---

## 🔗 İlgili Dökümanlar

- **Deployment Guide:** [DEPLOYMENT-GUIDE-v1.0.0.md](./DEPLOYMENT-GUIDE-v1.0.0.md)
- **Deployment Script:** [deploy-wireguard.sh](./deploy-wireguard.sh)
- **Build Script:** [build-release.js](./build-release.js)
- **README:** [README.md](./README.md)

---

**Son Güncelleme:** 23 Ekim 2025  
**Güncelleyen:** Kerim Akkis  
**Versiyon:** 1.0.0  

**© 2025 Akkis Technologies (AkkisTech) - All Rights Reserved**

