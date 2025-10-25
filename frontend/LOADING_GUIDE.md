# Global Loading System Guide

## Overview
Bu sistem, tüm sayfa geçişlerinde ve API çağrılarında tutarlı bir loading deneyimi sağlar.

## Components

### 1. GlobalLoading.vue
- Ana loading component'i
- Tüm uygulamada global olarak kullanılır
- Animated Optiviera logo ile loading gösterir

### 2. useLoading() Composable
```typescript
const { isLoading, loadingText, start, finish, setText } = useLoading()
```

**Methods:**
- `start(text?: string)` - Loading'i başlatır
- `finish()` - Loading'i bitirir
- `setText(text: string)` - Loading mesajını değiştirir

### 3. useApiLoading() Composable
API çağrıları için özel loading wrapper'ı:

```typescript
const { withLoading, withAsyncLoading } = useApiLoading()

// Otomatik finish
const result = await withLoading(async () => {
  return await $fetch('/api/data')
}, 'Loading data...')

// Manuel finish
const result = await withAsyncLoading(async () => {
  return await $fetch('/api/data')
}, 'Loading data...')
// finish() çağrılmalı
```

## Usage Examples

### Sayfa Geçişlerinde
Otomatik olarak çalışır. Plugin'de tanımlı mesajlar:
- `/` → "Loading Home..."
- `/login` → "Signing in..."
- `/dashboard` → "Loading Dashboard..."
- `/dashboard/tickets` → "Loading Tickets..."

### API Çağrılarında
```typescript
// Store'da kullanım
async fetchData() {
  const { withLoading } = useApiLoading()
  
  const result = await withLoading(async () => {
    return await $fetch('/api/data')
  }, 'Fetching data...')
  
  return result
}
```

### Manuel Kontrol
```typescript
const { start, finish, setText } = useLoading()

// Başlat
start('Processing...')

// Mesaj değiştir
setText('Almost done...')

// Bitir
finish()
```

## Features

- ✅ Global loading state
- ✅ Route-specific messages
- ✅ API call wrappers
- ✅ Animated Optiviera logo
- ✅ Smooth transitions
- ✅ TypeScript support
- ✅ Auto-cleanup

## Customization

### Loading Messages
`plugins/loading.client.ts` dosyasında route mesajlarını özelleştirebilirsiniz:

```typescript
const routeMessages: Record<string, string> = {
  '/your-route': 'Your custom message...'
}
```

### Styling
`components/GlobalLoading.vue` dosyasında CSS'i düzenleyebilirsiniz.
