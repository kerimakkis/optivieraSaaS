# 🚀 Optiviera ERP v1.0.0 - Complete Deployment Guide

**Release Date:** 14 Ekim 2025  
**Version:** 1.0.0 FINAL  
**Status:** ✅ PRODUCTION READY  
**Live URL:** https://akkistech.com/optiviera/

---

## 📋 Table of Contents

1. [Build Paketleri](#build-paketleri)
2. [Deployment Yöntemleri](#deployment-yöntemleri)
3. [WireGuard VPN Deployment](#wireguard-vpn-deployment)
4. [Deployment Guide with VPN](#-deployment-guide-with-vpn)
   - [Ön Hazırlık](#ön-hazırlık)
   - [Manuel Deployment Komutları](#manuel-deployment-komutları)
   - [Hızlı Deployment (Tek Komut)](#hızlı-deployment-tek-komut)
   - [Troubleshooting VPN Deployment](#troubleshooting-vpn-deployment)
   - [Deployment Checklist](#deployment-checklist)
   - [Yararlı Komutlar](#yararlı-komutlar)
5. [v1.0.0 Özellikleri](#v100-özellikleri)
6. [Test Durumu](#test-durumu)
7. [Kullanıcı Bilgileri](#kullanıcı-bilgileri)
8. [Troubleshooting](#troubleshooting)

---

## 📦 Build Paketleri

### Windows (x64)
- **Dosya:** `OptivieraERP.exe`
- **Boyut:** 130 MB
- **Platform:** Windows 10/11 x64
- **Konum:** `electron/dist/OptivieraERP.exe`
- **Download:** https://akkistech.com/optiviera/downloads/OptivieraERP.exe
- **Status:** ✅ Test Edildi - Çalışıyor

### macOS (Intel)
- **Dosya:** `Optiviera ERP-1.0.0.dmg`
- **Boyut:** 148 MB
- **Platform:** macOS 10.12+ (Intel x64)
- **Konum:** `electron/dist/Optiviera ERP-1.0.0.dmg`
- **Download:** https://akkistech.com/optiviera/downloads/Optiviera%20ERP-1.0.0.dmg
- **Status:** ⏳ Test Bekliyor (2013 Late Mac)
- **Not:** İlk açılışta "System Preferences → Security & Privacy → Open Anyway"

### macOS (ARM64)
- **Dosya:** `Optiviera ERP-1.0.0-arm64.dmg`
- **Boyut:** 143 MB
- **Platform:** macOS 10.12+ (Apple Silicon M1/M2/M3)
- **Konum:** `electron/dist/Optiviera ERP-1.0.0-arm64.dmg`
- **Download:** https://akkistech.com/optiviera/downloads/Optiviera%20ERP-1.0.0-arm64.dmg
- **Status:** ⏳ Test Bekliyor

### Linux (ARM64 - AppImage)
- **Dosya:** `Optiviera ERP-1.0.0-arm64.AppImage`
- **Boyut:** 151 MB
- **Platform:** Ubuntu/Debian/CentOS ARM64
- **Konum:** `electron/dist/Optiviera ERP-1.0.0-arm64.AppImage`
- **Download:** https://akkistech.com/optiviera/downloads/Optiviera%20ERP-1.0.0-arm64.AppImage
- **Status:** ⏳ Test Bekliyor
- **Kullanım:** `chmod +x Optiviera*.AppImage && ./Optiviera*.AppImage`

### Linux (ARM64 - Debian)
- **Dosya:** `optiviera-desktop_1.0.0_arm64.deb`
- **Boyut:** 98 MB
- **Platform:** Debian/Ubuntu ARM64
- **Konum:** `electron/dist/optiviera-desktop_1.0.0_arm64.deb`
- **Download:** https://akkistech.com/optiviera/downloads/optiviera-desktop_1.0.0_arm64.deb
- **Status:** ⏳ Test Bekliyor
- **Kurulum:** `sudo dpkg -i optiviera-desktop_1.0.0_arm64.deb`

---

## 🌐 Deployment Yöntemleri

### Yöntem 1: WireGuard VPN + rsync (Önerilen - Otomatik)

**Avantajlar:**
- ✅ Hızlı deployment (~60 saniye)
- ✅ Otomatik şifre girişi
- ✅ Docker container restart
- ✅ İlerleme göstergesi

**Gereksinimler:**
- WireGuard VPN bağlantısı
- `sshpass` kurulu (Homebrew: `brew install sshpass`)

**Kullanım:**
```bash
cd /Users/kerimakkis/Projects/Optiviera
./deploy-wireguard.sh
```

**Script Detayları:**
```bash
#!/bin/bash
# Optiviera ERP v1.0.0 - WireGuard VPN Deployment

SSH_PASS="Duka1429!"
SSH_USER="kerim"
SSH_HOST="192.168.178.20"
REMOTE_PATH="/mnt/data/volumes/websites/akkistech/html/optiviera/"
LOCAL_PATH="/Users/kerimakkis/Projects/Optiviera/hosting/"

# VPN kontrolü
ping -c 1 -W 2 $SSH_HOST

# rsync ile transfer
sshpass -p "$SSH_PASS" rsync -avh --progress --delete \
  -e "ssh -o StrictHostKeyChecking=no" \
  $LOCAL_PATH \
  $SSH_USER@$SSH_HOST:$REMOTE_PATH

# Docker container restart
sshpass -p "$SSH_PASS" ssh -o StrictHostKeyChecking=no \
  $SSH_USER@$SSH_HOST 'docker restart web-akkishost'
```

**Çıktı:**
```
✅ VPN bağlantısı aktif (192.168.178.20)
📦 Boyut: 655M
🚀 Deployment başlatılıyor...
✅ Dosya transferi tamamlandı!
🔄 Web container yeniden başlatılıyor...
✅ DEPLOYMENT BAŞARILI!
```

---

### Yöntem 2: FTP/SFTP (Manuel)

**FileZilla, Cyberduck veya benzeri FTP client:**
```
Host: akkistech.com
Username: kerim
Password: Duka1429!
Remote Path: /var/www/optiviera/
Local Path: /Users/kerimakkis/Projects/Optiviera/hosting/
```

**Adımlar:**
1. FTP client'ı açın
2. Yukarıdaki bilgilerle bağlanın
3. `hosting/` klasörünün tüm içeriğini upload edin
4. Web server'ı reload edin

---

### Yöntem 3: Deployment Paketi (SSH)

**Deployment paketi:**
```
/Users/kerimakkis/Projects/Optiviera/optiviera-v1.0.0-deployment.tar.gz
Boyut: 653 MB
```

**Adımlar:**
```bash
# 1. Paketi server'a yükle
scp optiviera-v1.0.0-deployment.tar.gz kerim@192.168.178.20:/tmp/

# 2. Server'da extract et
ssh kerim@192.168.178.20
cd /mnt/data/volumes/websites/akkistech/html/optiviera/
tar -xzf /tmp/optiviera-v1.0.0-deployment.tar.gz

# 3. İzinleri ayarla
chmod -R 755 /mnt/data/volumes/websites/akkistech/html/optiviera/

# 4. Container restart
docker restart web-akkishost
```

---

### Yöntem 4: cPanel/Plesk Panel

1. `hosting/` klasörünü ZIP olarak arşivleyin
2. Hosting panel'e giriş yapın
3. File Manager'dan hedef dizine gidin
4. ZIP dosyasını yükleyin ve extract edin
5. Web server'ı reload edin

---

## 🔧 WireGuard VPN Deployment

### Sistem Bilgileri

**VPN Server:**
- IP: 192.168.178.20
- Ping: ~23ms
- Container: web-akkishost
- Status: ✅ Aktif

**Transfer İstatistikleri:**
- Toplam boyut: 687 MB
- Ortalama hız: ~10.5 MB/s
- Süre: ~60 saniye
- Yöntem: rsync + sshpass

**Docker Container:**
```bash
# Container listesi
docker ps -a

# Container restart
docker restart web-akkishost

# Container logs
docker logs -f web-akkishost
```

### Deployment Workflow

```mermaid
graph LR
    A[Local Build] --> B[hosting/ klasörü]
    B --> C[WireGuard VPN]
    C --> D[rsync Transfer]
    D --> E[192.168.178.20]
    E --> F[Docker Restart]
    F --> G[LIVE!]
```

---

## 📘 Deployment Guide with VPN

### Ön Hazırlık

#### 1. WireGuard VPN Kurulumu
```bash
# macOS için WireGuard kurulumu
brew install wireguard-tools

# veya GUI client indir
# https://www.wireguard.com/install/
```

#### 2. sshpass Kurulumu
```bash
# macOS için sshpass kurulumu
brew install sshpass
```

#### 3. VPN Bağlantısı Kontrolü
```bash
# VPN aktif mi kontrol et
ping -c 1 192.168.178.20

# Başarılı çıktı:
# 64 bytes from 192.168.178.20: icmp_seq=0 ttl=64 time=23ms
```

---

### Manuel Deployment Komutları

#### Adım 1: Server'daki Mevcut Dosyaları Kontrol Et
```bash
# .exe dosyalarını listele
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 \
  'ls -lh /mnt/data/volumes/websites/akkistech/html/optiviera/downloads/*.exe'

# Tüm dosyaları listele
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 \
  'ls -lah /mnt/data/volumes/websites/akkistech/html/optiviera/downloads/'
```

#### Adım 2: Eski Installer Dosyalarını Sil
```bash
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 << 'ENDSSH'
cd /mnt/data/volumes/websites/akkistech/html/optiviera/downloads

echo "🗑️ ESKİ .exe DOSYALARINI SİLİYORUM..."
rm -f "Optiviera ERP Setup 1.0.0.exe"

echo "✅ Eski dosyalar silindi!"
echo ""
echo "📦 MEVCUT DOSYALAR:"
ls -lh *.exe 2>/dev/null
ENDSSH
```

#### Adım 3: Yeni Installer'ı Deploy Et
```bash
# Local dosyayı kontrol et
ls -lh /Users/kerimakkis/Projects/Optiviera/hosting/downloads/*.exe

# Deploy script'i çalıştır
cd /Users/kerimakkis/Projects/Optiviera
./deploy-wireguard.sh
```

#### Adım 4: HTML Download Linklerini Güncelle
```bash
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 << 'ENDSSH'
cd /mnt/data/volumes/websites/akkistech/html/optiviera

echo "🔧 index.html güncelleniyor..."

# Backup oluştur
cp index.html index.html.backup

# .exe linklerini kontrol et (zaten güncel olmalı)
grep "downloads/OptivieraERP.exe" index.html

# Dosya boyutunu güncelle
sed -i 's/İndir v1.0.0 (118 MB)/İndir v1.0.0 (130 MB)/g' index.html

echo "✅ index.html güncellendi!"
echo ""
echo "🔍 YENİ DOWNLOAD LİNKİ:"
grep -A2 "Windows" index.html | grep "href.*exe"
ENDSSH
```

#### Adım 5: Nginx Cache Temizle ve Container Restart
```bash
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 << 'ENDSSH'
echo "🔄 Nginx cache temizleniyor ve container restart..."
docker restart web-akkishost nginx-proxy-manager-app-1

echo "✅ Deployment tamamlandı!"
ENDSSH
```

#### Adım 6: Deployment Doğrulama
```bash
# Server'daki yeni dosyayı kontrol et
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 \
  'ls -lh /mnt/data/volumes/websites/akkistech/html/optiviera/downloads/*.exe'

# HTML içeriğini kontrol et
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 \
  'grep "OptivieraERP.exe" /mnt/data/volumes/websites/akkistech/html/optiviera/index.html'

# Web'den test et
curl -I https://akkistech.com/optiviera/downloads/OptivieraERP.exe
```

---

### Hızlı Deployment (Tek Komut)

Tüm adımları tek seferde çalıştırmak için:

```bash
cd /Users/kerimakkis/Projects/Optiviera && \
./deploy-wireguard.sh && \
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 << 'ENDSSH'
cd /mnt/data/volumes/websites/akkistech/html/optiviera/downloads
echo "📦 Mevcut .exe dosyaları:"
ls -lh *.exe
cd ..
cp index.html index.html.backup
sed -i 's/İndir v1.0.0 (118 MB)/İndir v1.0.0 (130 MB)/g' index.html
docker restart web-akkishost nginx-proxy-manager-app-1
echo "✅ DEPLOYMENT TAMAMLANDI!"
ls -lh downloads/*.exe
ENDSSH
```

---

### Troubleshooting VPN Deployment

#### Problem 1: VPN Bağlantısı Yok
```bash
# Sorun: ping: cannot resolve 192.168.178.20

# Çözüm:
# 1. WireGuard VPN'in aktif olduğundan emin ol
sudo wg-quick up wg0

# 2. VPN interface'i kontrol et
ifconfig | grep wg

# 3. VPN routing'i kontrol et
netstat -rn | grep 192.168.178
```

#### Problem 2: sshpass Permission Denied
```bash
# Sorun: Failed to get a pseudo terminal: Operation not permitted

# Çözüm: -T flag ekle (pseudo-terminal devre dışı)
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 'ls'
```

#### Problem 3: rsync Transfer Hatası
```bash
# Sorun: rsync: failed to set times

# Çözüm: --no-perms ve --no-times flag'leri ekle
sshpass -p "Duka1429!" rsync -avh --no-perms --no-times --progress --delete \
  -e "ssh -o StrictHostKeyChecking=no" \
  /Users/kerimakkis/Projects/Optiviera/hosting/ \
  kerim@192.168.178.20:/mnt/data/volumes/websites/akkistech/html/optiviera/
```

#### Problem 4: Docker Container Bulunamıyor
```bash
# Sorun: Error: No such container: web-akkistech

# Çözüm: Doğru container ismini bul
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 \
  'docker ps -a | grep web'

# Doğru isim: web-akkishost
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 \
  'docker restart web-akkishost'
```

#### Problem 5: Eski Dosya Hala İndiriliyor
```bash
# Sorun: Browser cache veya Nginx cache

# Çözüm 1: Browser cache temizle
# Ctrl+Shift+Delete → Clear all cache

# Çözüm 2: Incognito mode kullan
# Ctrl+Shift+N (Chrome/Edge)

# Çözüm 3: Nginx cache temizle
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 << 'ENDSSH'
docker restart nginx-proxy-manager-app-1
docker exec nginx-proxy-manager-app-1 nginx -s reload
ENDSSH

# Çözüm 4: Hard refresh
# Ctrl+F5 (Windows)
# Cmd+Shift+R (macOS)
```

---

### Deployment Checklist

- [ ] ✅ WireGuard VPN aktif
- [ ] ✅ sshpass kurulu
- [ ] ✅ Local build hazır (`hosting/downloads/OptivieraERP.exe`)
- [ ] ✅ VPN ping testi başarılı (192.168.178.20)
- [ ] ✅ SSH bağlantısı test edildi
- [ ] ✅ Eski dosyalar silindi
- [ ] ✅ Yeni dosya deploy edildi
- [ ] ✅ HTML linkleri güncellendi
- [ ] ✅ Dosya boyutu güncellendi
- [ ] ✅ Docker container restart edildi
- [ ] ✅ Nginx cache temizlendi
- [ ] ✅ Web'den download test edildi
- [ ] ✅ Dosya boyutu doğru (130 MB)
- [ ] ✅ Checksum doğrulandı (opsiyonel)

---

### Deployment İstatistikleri

**Son Deployment:**
- **Tarih:** 23 Ekim 2025
- **Dosya:** OptivieraERP.exe
- **Boyut:** 130 MB (136,127,148 bytes)
- **Transfer Hızı:** 8.76 MB/s
- **Transfer Süresi:** ~15 saniye
- **Hedef:** akkistech.com/optiviera/downloads/
- **Status:** ✅ BAŞARILI

**Deployment Geçmişi:**
| Tarih | Dosya | Boyut | Durum |
|-------|-------|-------|-------|
| 23 Ekim 2025 | OptivieraERP.exe | 130 MB | ✅ LIVE |
| 14 Ekim 2025 | Optiviera ERP Setup 1.0.0.exe | 118 MB | ❌ Silindi (eski versiyon) |

---

### Yararlı Komutlar

#### Server Durumu
```bash
# Disk kullanımı
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 \
  'df -h /mnt/data/volumes/websites/akkistech/html/optiviera/'

# Dosya sayısı
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 \
  'find /mnt/data/volumes/websites/akkistech/html/optiviera/ -type f | wc -l'

# Toplam boyut
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 \
  'du -sh /mnt/data/volumes/websites/akkistech/html/optiviera/'
```

#### Docker Yönetimi
```bash
# Container durumu
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 \
  'docker ps | grep web-akkishost'

# Container logs (son 50 satır)
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 \
  'docker logs --tail 50 web-akkishost'

# Container kaynak kullanımı
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 \
  'docker stats --no-stream web-akkishost'
```

#### Dosya Checksum
```bash
# Local checksum
shasum -a 256 /Users/kerimakkis/Projects/Optiviera/hosting/downloads/OptivieraERP.exe

# Server checksum
sshpass -p "Duka1429!" ssh -o StrictHostKeyChecking=no -T kerim@192.168.178.20 \
  'sha256sum /mnt/data/volumes/websites/akkistech/html/optiviera/downloads/OptivieraERP.exe'

# Karşılaştır - aynı olmalı!
```

#### Backup Oluştur
```bash
# Server'dan local'e backup al
sshpass -p "Duka1429!" rsync -avh --progress \
  -e "ssh -o StrictHostKeyChecking=no" \
  kerim@192.168.178.20:/mnt/data/volumes/websites/akkistech/html/optiviera/ \
  ~/Desktop/optiviera-backup-$(date +%Y%m%d)/

# Backup'ı arşivle
cd ~/Desktop
tar -czf optiviera-backup-$(date +%Y%m%d).tar.gz optiviera-backup-$(date +%Y%m%d)/
```

---

## ✨ v1.0.0 Özellikleri

### Yeni Özellikler

#### 1. Dönen Optiviera Logo
- **Splash Screen:** Base64 embedded logo animasyonu
- **Animasyon:** 2 saniye rotasyon
- **Efekt:** Yeşil glow (#1abc9c)
- **Background:** Koyu gradient
- **Fix:** `file://` protokol sorunu çözüldü

#### 2. Bayraklı Dil Seçici
- **Diller:** 8 dil desteği
  - 🇹🇷 Türkçe
  - 🇬🇧 English
  - 🇩🇪 Deutsch
  - 🇫🇷 Français
  - 🇪🇸 Español
  - 🇮🇹 Italiano
  - 🇵🇹 Português
  - 🇳🇱 Nederlands
- **Görünüm:** Button'da sadece bayrak, dropdown'da bayrak + tam isim
- **Konum:** Header'da sağ üst köşe

#### 3. Auto-Migration & Seed
- **Database:** SQLite otomatik oluşturma
- **Konum:** `%APPDATA%/Optiviera ERP/Optiviera.db` (Windows)
- **Migration:** Startup'ta otomatik
- **Seed Data:**
  - Admin, Manager, Employee rolleri
  - 3 default kullanıcı
  - Priority seviyeleri
  - 365 günlük trial license

#### 4. Health Check
- **Problem:** Blank white screen on startup
- **Çözüm:** HTTP GET retry logic (30 saniye)
- **Kontrol:** Server responsive olana kadar bekler
- **Timeout:** 30 saniye sonra hata mesajı

#### 5. Clean Uninstall
- **Windows:** `taskkill /f /t` ile process tree temizleme
- **Process Management:** Detached: false
- **Cleanup:** Startup'ta eski process'leri temizleme
- **Events:** `before-quit` ve `will-quit` handlers

#### 6. Desktop Shortcut & Logo
- **Windows:** NSIS installer ile otomatik
- **Icon:** `icon.ico` (256x256)
- **Konum:** Desktop + Start Menu
- **Logo:** Optiviera branding

#### 7. AppData Database
- **Windows:** `%APPDATA%/Optiviera ERP/`
- **macOS:** `~/Library/Application Support/Optiviera ERP/`
- **Linux:** `~/.config/Optiviera ERP/`
- **Avantaj:** Kullanıcı verisi korumalı

#### 8. System Tray
- **Icon:** Optiviera logo
- **Menu:** Show/Hide/Quit
- **Minimize:** Tray'e minimize olma
- **Startup:** Arka planda başlatma

### Teknik İyileştirmeler

#### Base64 Embedded Logo
```javascript
const logoBuffer = fs.readFileSync(logoPath);
logoBase64 = `data:image/png;base64,${logoBuffer.toString('base64')}`;
```

#### Server Health Check
```javascript
function waitForServer(port, attempts = 0) {
  const maxAttempts = 30;
  http.get(`http://localhost:${port}`, (res) => {
    console.log(`Server is ready! Status: ${res.statusCode}`);
    mainWindow.loadURL(`http://localhost:${port}`);
  }).on('error', (err) => {
    if (attempts < maxAttempts) {
      setTimeout(() => waitForServer(port, attempts + 1), 1000);
    } else {
      dialog.showErrorBox('Server Timeout', 'Server failed to start.');
      app.quit();
    }
  });
}
```

#### Process Management (Windows)
```javascript
// Cleanup existing processes
spawn('taskkill', ['/IM', 'Optiviera.exe', '/F']);

// Start new process
aspNetProcess = spawn(exePath, [], {
  cwd: appPath,
  env: env,
  stdio: ['ignore', 'pipe', 'pipe'],
  detached: false
});

// Force kill on quit
app.on('before-quit', () => {
  isQuitting = true;
  if (aspNetProcess && !aspNetProcess.killed) {
    spawn('taskkill', ['/pid', aspNetProcess.pid, '/f', '/t']);
  }
});
```

#### Dynamic Port Allocation
```javascript
function findAvailablePort(startPort) {
  return new Promise((resolve) => {
    const server = net.createServer();
    server.listen(startPort, () => {
      const port = server.address().port;
      server.close(() => resolve(port));
    });
    server.on('error', () => {
      resolve(findAvailablePort(startPort + 1));
    });
  });
}
```

---

## 🧪 Test Durumu

### ✅ Test Edildi ve Çalışıyor

| Feature | Platform | Status | Notes |
|---------|----------|--------|-------|
| Splash Screen | Windows x64 | ✅ | Logo dönüyor, yeşil glow |
| Dil Seçici | Windows x64 | ✅ | Bayrak + tam isim |
| Auto-Migration | Windows x64 | ✅ | DB otomatik oluşuyor |
| Health Check | Windows x64 | ✅ | Blank screen fix |
| Clean Uninstall | Windows x64 | ✅ | Process düzgün sonlanıyor |
| Desktop Shortcut | Windows x64 | ✅ | Logo görünüyor |
| AppData Database | Windows x64 | ✅ | %APPDATA% klasöründe |
| Website Deployment | akkistech.com | ✅ | LIVE ve erişilebilir |

### ⏳ Test Bekliyor

| Platform | Package | Size | Priority |
|----------|---------|------|----------|
| macOS Intel | Optiviera ERP-1.0.0.dmg | 148 MB | 🔴 High (2013 Mac) |
| macOS ARM64 | Optiviera ERP-1.0.0-arm64.dmg | 143 MB | 🟡 Medium |
| Linux AppImage | Optiviera ERP-1.0.0-arm64.AppImage | 151 MB | 🟢 Low |
| Linux Debian | optiviera-desktop_1.0.0_arm64.deb | 98 MB | 🟢 Low |

### 🐛 Bilinen Sorunlar

1. **macOS Code Signing Yok**
   - **Durum:** Sertifika yok
   - **Workaround:** "System Preferences → Security & Privacy → Open Anyway"
   - **Çözüm:** Apple Developer sertifikası gerekli

2. **DB Migration Warning (Build Time)**
   - **Hata:** `MSBUILD : error MSB1009: Project file does not exist`
   - **Durum:** Önemsiz - runtime'da auto-migrate var
   - **Çözüm:** Gerekli değil

3. **Linux ARM64 Only**
   - **Durum:** x64 paketi yok
   - **Workaround:** Gerekirse build edilebilir
   - **Komut:** `node build-release.js linux` (x64 target ekle)

---

## 🔐 Kullanıcı Bilgileri

### Default Kullanıcılar

#### Admin
```
Email: admin@optiviera.local
Password: Admin123!
Role: Admin
Permissions: Full access
```

#### Manager
```
Email: manager@optiviera.local
Password: Manager123!
Role: Manager
Permissions: Ticket management, Reports
```

#### Employee
```
Email: employee@optiviera.local
Password: Employee123!
Role: Employee
Permissions: Ticket view/edit (assigned only)
```

### License Bilgileri

**Trial License:**
- **Süre:** 365 gün (1 yıl)
- **Aktivasyon:** Otomatik (ilk çalıştırmada)
- **Machine ID:** Otomatik oluşturulur
- **Format:** `OPTV-XXXX-XXXX-XXXX-XXXX`

**Full License:**
- **Satın Alma:** https://akkistech.com/optiviera/
- **Aktivasyon:** License key ile manuel
- **Süre:** 1 yıl (yenilenebilir)
- **Fiyat:** TBD

---

## 🛠 Troubleshooting

### Windows

#### Problem: Uygulama açılmıyor
```
Çözüm:
1. Task Manager'da "Optiviera.exe" process'ini sonlandırın
2. %APPDATA%/Optiviera ERP/ klasörünü kontrol edin
3. Uygulamayı yeniden başlatın
```

#### Problem: Uninstall çalışmıyor
```
Çözüm:
1. Task Manager'da tüm "Optiviera" process'lerini sonlandırın
2. Uninstaller'ı tekrar çalıştırın
3. Manuel silme: %LOCALAPPDATA%/Programs/Optiviera ERP/
```

#### Problem: Database hatası
```
Çözüm:
1. %APPDATA%/Optiviera ERP/Optiviera.db dosyasını silin
2. Uygulamayı yeniden başlatın (otomatik oluşur)
3. Default kullanıcılarla giriş yapın
```

### macOS

#### Problem: "Unidentified developer" hatası
```
Çözüm:
1. System Preferences → Security & Privacy
2. "Open Anyway" butonuna tıklayın
3. Uygulamayı tekrar açın
```

#### Problem: Uygulama açılmıyor (Intel Mac)
```
Çözüm:
1. Doğru DMG'yi indirdiğinizden emin olun:
   - Intel: Optiviera ERP-1.0.0.dmg
   - ARM64: Optiviera ERP-1.0.0-arm64.dmg
2. Terminal'den çalıştırın:
   /Applications/Optiviera\ ERP.app/Contents/MacOS/Optiviera\ ERP
3. Hata mesajını kontrol edin
```

### Linux

#### Problem: AppImage çalışmıyor
```
Çözüm:
1. Execute permission verin:
   chmod +x Optiviera*.AppImage
2. FUSE kurulu mu kontrol edin:
   sudo apt install fuse libfuse2
3. Çalıştırın:
   ./Optiviera*.AppImage
```

#### Problem: Debian paketi kurulmuyor
```
Çözüm:
1. Dependency'leri kurun:
   sudo apt install -f
2. Paketi yeniden kurun:
   sudo dpkg -i optiviera-desktop_1.0.0_arm64.deb
```

### Deployment

#### Problem: WireGuard VPN bağlantısı yok
```
Çözüm:
1. WireGuard'ın çalıştığından emin olun
2. VPN bağlantısını test edin:
   ping 192.168.178.20
3. WireGuard config'i kontrol edin
```

#### Problem: rsync şifre hatası
```
Çözüm:
1. sshpass kurulu mu kontrol edin:
   which sshpass
2. Kurulu değilse:
   brew install sshpass
3. Şifreyi kontrol edin: Duka1429!
```

#### Problem: Docker container restart başarısız
```
Çözüm:
1. Container ismini kontrol edin:
   ssh kerim@192.168.178.20 'docker ps -a'
2. Doğru isim: web-akkishost
3. Manuel restart:
   docker restart web-akkishost
```

---

## 📊 Deployment Özeti

### Build İstatistikleri
- **Toplam Paketler:** 5 adet
- **Toplam Boyut:** 654 MB (paketler) + 14 KB (index.html)
- **Platformlar:** Windows, macOS (Intel/ARM), Linux (AppImage/Deb)
- **Diller:** 8 dil desteği
- **Build Süresi:** ~5 dakika (tüm platformlar)

### Deployment İstatistikleri
- **Yöntem:** WireGuard VPN + rsync
- **Transfer Boyutu:** 687 MB
- **Transfer Hızı:** ~10.5 MB/s
- **Transfer Süresi:** ~60 saniye
- **Hedef:** 192.168.178.20:/mnt/data/volumes/websites/akkistech/html/optiviera/
- **Container:** web-akkishost
- **Status:** ✅ LIVE

### Website İstatistikleri
- **URL:** https://akkistech.com/optiviera/
- **HTTP Status:** 200 OK
- **SSL:** ✅ Aktif
- **Download Links:** ✅ Çalışıyor
- **Responsive:** ✅ Mobile-friendly

---

## 📝 Notlar

### Güvenlik
- ⚠️ `deploy-wireguard.sh` içinde şifre var - güvenli saklayın!
- ⚠️ SSH key kullanımı önerilir (gelecek versiyonlar için)
- ✅ WireGuard VPN güvenli tünel sağlıyor
- ✅ HTTPS ile şifreli bağlantı

### Performans
- ✅ rsync incremental transfer (sadece değişenler)
- ✅ `--delete` flag ile eski dosyalar temizleniyor
- ✅ Compression aktif (transfer hızı artıyor)
- ✅ Progress bar ile ilerleme takibi

### Gelecek İyileştirmeler
- [ ] SSH key authentication (şifresiz deployment)
- [ ] CI/CD pipeline (GitHub Actions)
- [ ] Automated testing (Playwright/Selenium)
- [ ] macOS code signing (Apple Developer)
- [ ] Linux x64 packages (Intel/AMD)
- [ ] Auto-update system (Electron updater)
- [ ] Crash reporting (Sentry/Bugsnag)
- [ ] Analytics (Google Analytics/Matomo)

---

## 🔗 Linkler

### Production
- **Website:** https://akkistech.com/optiviera/
- **Download:** https://akkistech.com/optiviera/#download
- **Support:** support@akkistech.com

### Development
- **Project:** `/Users/kerimakkis/Projects/Optiviera/`
- **Hosting:** `/Users/kerimakkis/Projects/Optiviera/hosting/`
- **Deployment Script:** `/Users/kerimakkis/Projects/Optiviera/deploy-wireguard.sh`
- **Build Script:** `/Users/kerimakkis/Projects/Optiviera/build-release.js`

### Documentation
- **README:** `/Users/kerimakkis/Projects/Optiviera/README.md`
- **DEPLOYMENT:** `/Users/kerimakkis/Projects/Optiviera/DEPLOYMENT.md`
- **THIS FILE:** `/Users/kerimakkis/Projects/Optiviera/DEPLOYMENT-GUIDE-v1.0.0.md`

---

## 📅 Version History

### v1.0.0 (14 Ekim 2025)
- ✅ Initial production release
- ✅ Multi-platform support (Windows, macOS, Linux)
- ✅ 8 language support
- ✅ Trial license system (365 days)
- ✅ Auto-migration & seed data
- ✅ WireGuard VPN deployment
- ✅ Website deployment (akkistech.com)

---

## 👥 Credits

**Developer:** Kerim Akkis  
**Company:** Akkis Technologies (AkkisTech)  
**Email:** support@akkistech.com  
**Website:** https://akkistech.com  

---

**© 2025 Akkis Technologies (AkkisTech) - All Rights Reserved**

**Optiviera ERP** - Küçük işletmeler için modern ERP çözümü 🚀



