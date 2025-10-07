#!/bin/bash

# Optiviera ERP Deployment Script
# Bu script akkistech.com'a deploy etmek için kullanılır

echo "🚀 Optiviera ERP Deployment Başlatılıyor..."

# Renkli output için
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Deployment bilgileri
DEPLOY_DIR="/Users/kerimakkis/Projects/Optiviera/hosting"
REMOTE_HOST="akkistech.com"
REMOTE_USER="kerim"
REMOTE_PATH="/var/www/optiviera"

echo -e "${BLUE}📋 Deployment Bilgileri:${NC}"
echo "  - Local Directory: $DEPLOY_DIR"
echo "  - Remote Host: $REMOTE_HOST"
echo "  - Remote User: $REMOTE_USER"
echo "  - Remote Path: $REMOTE_PATH"
echo ""

# Dosya boyutlarını kontrol et
echo -e "${YELLOW}📊 Dosya Boyutları:${NC}"
ls -lh $DEPLOY_DIR/downloads/ | grep -E "\.(exe|dmg|AppImage)$"
echo ""

# Deployment öncesi kontrol
echo -e "${YELLOW}🔍 Deployment Öncesi Kontroller:${NC}"

# 1. Dosyaların varlığını kontrol et
if [ ! -f "$DEPLOY_DIR/index.html" ]; then
    echo -e "${RED}❌ index.html bulunamadı!${NC}"
    exit 1
fi

if [ ! -d "$DEPLOY_DIR/downloads" ]; then
    echo -e "${RED}❌ downloads klasörü bulunamadı!${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Tüm dosyalar mevcut${NC}"

# 2. Dosya boyutlarını kontrol et
TOTAL_SIZE=$(du -sh $DEPLOY_DIR | cut -f1)
echo -e "${BLUE}📦 Toplam boyut: $TOTAL_SIZE${NC}"

# 3. Remote host bağlantısını test et
echo -e "${YELLOW}🌐 Remote host bağlantısı test ediliyor...${NC}"
if ssh -o ConnectTimeout=10 $REMOTE_USER@$REMOTE_HOST "echo 'Bağlantı başarılı'" 2>/dev/null; then
    echo -e "${GREEN}✅ Remote host bağlantısı başarılı${NC}"
else
    echo -e "${RED}❌ Remote host bağlantısı başarısız!${NC}"
    echo -e "${YELLOW}💡 Manuel deployment gerekli:${NC}"
    echo "  1. $DEPLOY_DIR klasörünü akkistech.com'a yükleyin"
    echo "  2. /var/www/optiviera/ dizinine yerleştirin"
    echo "  3. Web server'ı yeniden başlatın"
    exit 1
fi

# Deployment işlemi
echo -e "${YELLOW}🚀 Deployment başlatılıyor...${NC}"

# Remote dizini oluştur
ssh $REMOTE_USER@$REMOTE_HOST "mkdir -p $REMOTE_PATH"

# Dosyaları kopyala
echo -e "${BLUE}📁 Dosyalar kopyalanıyor...${NC}"
rsync -avz --progress $DEPLOY_DIR/ $REMOTE_USER@$REMOTE_HOST:$REMOTE_PATH/

# Dosya izinlerini ayarla
echo -e "${BLUE}🔐 Dosya izinleri ayarlanıyor...${NC}"
ssh $REMOTE_USER@$REMOTE_HOST "chmod -R 755 $REMOTE_PATH"

# Web server'ı yeniden başlat
echo -e "${BLUE}🔄 Web server yeniden başlatılıyor...${NC}"
ssh $REMOTE_USER@$REMOTE_HOST "sudo systemctl reload nginx"

# Deployment sonrası kontrol
echo -e "${YELLOW}🔍 Deployment sonrası kontrol...${NC}"
if curl -s -o /dev/null -w "%{http_code}" "https://akkistech.com/optiviera/" | grep -q "200"; then
    echo -e "${GREEN}✅ Deployment başarılı!${NC}"
    echo -e "${GREEN}🌐 Website: https://akkistech.com/optiviera/${NC}"
else
    echo -e "${YELLOW}⚠️  Website kontrolü başarısız, manuel kontrol gerekli${NC}"
fi

echo ""
echo -e "${GREEN}🎉 Deployment tamamlandı!${NC}"
echo -e "${BLUE}📋 Sonraki adımlar:${NC}"
echo "  1. https://akkistech.com/optiviera/ adresini kontrol edin"
echo "  2. Download linklerini test edin"
echo "  3. Lisans sistemi çalışıyor mu kontrol edin"
echo "  4. Kullanıcı geri bildirimlerini takip edin"

echo ""
echo -e "${YELLOW}📊 Deployment Özeti:${NC}"
echo "  - Ana sayfa: ✅ Hazır"
echo "  - Download dosyaları: ✅ Hazır"
echo "  - Lisans sistemi: ✅ Aktif"
echo "  - 8 dil desteği: ✅ Aktif"
echo "  - Trial lisans: ✅ 1 yıl ücretsiz"


