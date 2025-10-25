<template>
  <div class="dashboard-layout">
    <!-- Top Navbar -->
    <header class="dashboard-header">
      <div class="header-content">
        <div class="header-left">
          <Button
            icon="pi pi-bars"
            text
            rounded
            @click="mobileMenuOpen = !mobileMenuOpen"
            class="mobile-menu-btn"
            aria-label="Toggle Menu"
          />
          <NuxtLink to="/dashboard" class="logo-link">
            <img src="/logo.png" alt="Optiviera" class="header-logo" />
            <h1 class="header-title">Optiviera</h1>
          </NuxtLink>
        </div>

        <nav class="header-nav" :class="{ 'mobile-open': mobileMenuOpen }">
          <NuxtLink to="/dashboard" class="nav-link" @click="mobileMenuOpen = false">
            <i class="pi pi-home"></i>
            <span>Dashboard</span>
          </NuxtLink>
          <NuxtLink to="/dashboard/tickets" class="nav-link" @click="mobileMenuOpen = false">
            <i class="pi pi-ticket"></i>
            <span>Tickets</span>
          </NuxtLink>
          <NuxtLink v-if="authStore.isAdmin" to="/dashboard/users" class="nav-link" @click="mobileMenuOpen = false">
            <i class="pi pi-users"></i>
            <span>Users</span>
          </NuxtLink>
          <NuxtLink v-if="authStore.isAdmin || authStore.isManager" to="/dashboard/reports" class="nav-link" @click="mobileMenuOpen = false">
            <i class="pi pi-chart-bar"></i>
            <span>Reports</span>
          </NuxtLink>
        </nav>

        <div class="header-right">
          <div class="user-info">
            <span class="user-name">{{ authStore.user?.fullName }}</span>
            <span class="user-role">{{ authStore.user?.roles?.[0] }}</span>
          </div>
          <Button
            icon="pi pi-sign-out"
            severity="secondary"
            text
            rounded
            @click="handleLogout"
            v-tooltip.bottom="'Logout'"
            class="logout-btn"
          />
        </div>
      </div>
    </header>

    <!-- Mobile Menu Overlay -->
    <div
      v-if="mobileMenuOpen"
      class="mobile-overlay"
      @click="mobileMenuOpen = false"
    ></div>

    <!-- Main Content -->
    <main class="dashboard-main">
      <div class="dashboard-container">
        <slot />
      </div>
    </main>

    <!-- Footer -->
    <footer class="dashboard-footer">
      <p>&copy; 2025 Optiviera SaaS. All rights reserved.</p>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '~/stores/auth'

const authStore = useAuthStore()
const router = useRouter()
const mobileMenuOpen = ref(false)

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}

// Close mobile menu on route change
watch(() => router.currentRoute.value.path, () => {
  mobileMenuOpen.value = false
})

// Close mobile menu on escape key
onMounted(() => {
  const handleEscape = (e: KeyboardEvent) => {
    if (e.key === 'Escape') {
      mobileMenuOpen.value = false
    }
  }
  window.addEventListener('keydown', handleEscape)
  onUnmounted(() => {
    window.removeEventListener('keydown', handleEscape)
  })
})
</script>

<style scoped>
.dashboard-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  background-color: var(--surface-ground);
}

/* Header */
.dashboard-header {
  background: linear-gradient(135deg, var(--optiviera-navy) 0%, var(--optiviera-blue) 100%);
  color: white;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  position: sticky;
  top: 0;
  z-index: 1000;
}

.header-content {
  max-width: 1400px;
  margin: 0 auto;
  padding: 1rem 2rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 2rem;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.logo-link {
  display: flex;
  align-items: center;
  gap: 1rem;
  text-decoration: none;
  transition: all 0.2s;
}

.logo-link:hover {
  opacity: 0.9;
  transform: scale(1.02);
}

.header-logo {
  height: 40px;
  width: auto;
}

.header-title {
  font-size: 1.5rem;
  font-weight: 700;
  margin: 0;
  color: white;
}

.header-nav {
  display: flex;
  gap: 0.5rem;
  flex: 1;
  justify-content: center;
}

.nav-link {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.25rem;
  color: rgba(255, 255, 255, 0.9);
  text-decoration: none;
  border-radius: 8px;
  font-weight: 500;
  transition: all 0.2s;
}

.nav-link:hover {
  background-color: rgba(255, 255, 255, 0.1);
  color: white;
}

.nav-link.router-link-active {
  background-color: rgba(255, 255, 255, 0.2);
  color: white;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.user-info {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
}

.user-name {
  font-weight: 600;
  font-size: 0.95rem;
}

.user-role {
  font-size: 0.8rem;
  opacity: 0.8;
}

/* Main Content */
.dashboard-main {
  flex: 1;
  padding: 2rem 0;
}

.dashboard-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 2rem;
}

/* Footer */
.dashboard-footer {
  background-color: var(--optiviera-navy);
  color: rgba(255, 255, 255, 0.8);
  padding: 1.5rem 2rem;
  text-align: center;
  margin-top: auto;
}

.dashboard-footer p {
  margin: 0;
  font-size: 0.9rem;
}

/* Mobile Menu Button */
.mobile-menu-btn {
  display: none;
  color: white !important;
}

/* Mobile Overlay */
.mobile-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  z-index: 999;
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

/* Responsive */
@media (max-width: 768px) {
  .mobile-menu-btn {
    display: inline-flex !important;
  }

  .header-content {
    padding: 1rem;
  }

  .header-left {
    gap: 0.75rem;
  }

  .header-logo {
    height: 32px;
  }

  .header-title {
    font-size: 1.25rem;
  }

  .header-nav {
    position: fixed;
    top: 0;
    left: -280px;
    height: 100vh;
    width: 280px;
    background: linear-gradient(180deg, var(--optiviera-navy) 0%, var(--optiviera-blue) 100%);
    flex-direction: column;
    justify-content: flex-start;
    align-items: stretch;
    padding: 5rem 1rem 1rem;
    box-shadow: 2px 0 10px rgba(0, 0, 0, 0.3);
    transition: left 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    z-index: 1000;
    gap: 0.25rem;
  }

  .header-nav.mobile-open {
    left: 0;
  }

  .nav-link {
    padding: 1rem 1.25rem;
    justify-content: flex-start;
    width: 100%;
  }

  .nav-link i {
    font-size: 1.25rem;
  }

  .nav-link span {
    font-size: 1rem;
  }

  .user-info {
    display: none;
  }

  .dashboard-container {
    padding: 0 1rem;
  }
}
</style>
