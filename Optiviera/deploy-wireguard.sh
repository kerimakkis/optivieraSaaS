#!/bin/bash

# Optiviera ERP v1.0.0 - WireGuard VPN Deployment (Otomatik)
# Date: 14 Ekim 2025

SSH_PASS="Duka1429!"
SSH_USER="kerim"
SSH_HOST="192.168.178.20"
REMOTE_PATH="/mnt/data/volumes/websites/akkistech/html/optiviera/"
LOCAL_PATH="/Users/kerimakkis/Projects/Optiviera/hosting/"

echo "╔═══════════════════════════════════════════════════════╗"
echo "║   OPTIVIERA v1.0.0 - WIREGUARD DEPLOYMENT            ║"
echo "╚═══════════════════════════════════════════════════════╝"
echo ""

# VPN bağlantı kontrolü
echo "🔍 WireGuard VPN bağlantısı kontrol ediliyor..."
if ping -c 1 -W 2 $SSH_HOST > /dev/null 2>&1; then
    echo "✅ VPN bağlantısı aktif ($SSH_HOST)"
else
    echo "❌ VPN bağlantısı yok! WireGuard'ı başlatın."
    exit 1
fi

echo ""
echo "📦 Deployment bilgileri:"
echo "   • Kaynak: $LOCAL_PATH"
echo "   • Hedef: $SSH_USER@$SSH_HOST:$REMOTE_PATH"
echo "   • Boyut: $(du -sh $LOCAL_PATH | cut -f1)"
echo ""

# rsync ile dosya transferi (otomatik şifre)
echo "🚀 Deployment başlatılıyor..."
echo ""

sshpass -p "$SSH_PASS" rsync -avh --progress --delete \
  -e "ssh -o StrictHostKeyChecking=no" \
  $LOCAL_PATH \
  $SSH_USER@$SSH_HOST:$REMOTE_PATH

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Dosya transferi tamamlandı!"
    echo ""
    echo "🔄 Web container yeniden başlatılıyor..."
    
    sshpass -p "$SSH_PASS" ssh -o StrictHostKeyChecking=no \
      $SSH_USER@$SSH_HOST 'docker restart web-akkishost'
    
    if [ $? -eq 0 ]; then
        echo ""
        echo "╔═══════════════════════════════════════════════════════╗"
        echo "║          ✅ DEPLOYMENT BAŞARILI! ✅                  ║"
        echo "╚═══════════════════════════════════════════════════════╝"
        echo ""
        echo "🌐 Website: https://akkistech.com/optiviera/"
        echo ""
        echo "📋 Test Adımları:"
        echo "   1. Ana sayfa: https://akkistech.com/optiviera/"
        echo "   2. Download linkleri çalışıyor mu?"
        echo "   3. Dosya boyutları doğru mu?"
        echo ""
        echo "📦 Yüklenen Paketler:"
        echo "   • Windows x64: 114 MB"
        echo "   • macOS Intel: 148 MB"
        echo "   • macOS ARM64: 143 MB"
        echo "   • Linux AppImage: 151 MB"
        echo "   • Linux Deb: 98 MB"
        echo ""
    else
        echo "❌ Container restart başarısız!"
        exit 1
    fi
else
    echo "❌ Dosya transferi başarısız!"
    exit 1
fi
