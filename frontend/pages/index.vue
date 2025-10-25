<template>
  <div class="landing-page">
    <div class="landing-container">
      <div class="hero">
        <NuxtLink to="/dashboard" class="hero-logo-link">
          <img src="/logo.png" alt="Optiviera Logo" class="hero-logo" />
        </NuxtLink>
        <h1>Welcome to Optiviera</h1>
        <p>Modern IT Helpdesk & Ticket Management Platform</p>
        <p class="hero-description">
          Streamline your IT support operations with our powerful,
          multi-tenant SaaS solution designed for modern businesses.
        </p>
        <div class="cta-buttons">
          <Button
            label="Login"
            icon="pi pi-sign-in"
            @click="navigateTo('/login')"
            class="cta-btn cta-btn-primary"
          />
          <Button
            label="Get Started"
            icon="pi pi-user-plus"
            @click="navigateTo('/register')"
            class="cta-btn cta-btn-secondary"
            outlined
          />
          <Button
            label="Test Loading"
            icon="pi pi-spin pi-spinner"
            @click="testLoading"
            class="cta-btn cta-btn-test"
            outlined
          />
        </div>
      </div>

      <div class="features">
        <div class="feature-card">
          <i class="pi pi-ticket"></i>
          <h3>Ticket Management</h3>
          <p>Track and manage customer support tickets efficiently</p>
        </div>
        <div class="feature-card">
          <i class="pi pi-users"></i>
          <h3>Team Collaboration</h3>
          <p>Work together with your team in real-time</p>
        </div>
        <div class="feature-card">
          <i class="pi pi-chart-line"></i>
          <h3>Analytics & Reports</h3>
          <p>Get insights into your support operations</p>
        </div>
      </div>
    </div>

    <footer class="landing-footer">
      <p>&copy; 2025 Optiviera SaaS. All rights reserved.</p>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '~/stores/auth'

definePageMeta({
  layout: 'default'
})

const authStore = useAuthStore()
const { start, finish } = useLoading()

// Redirect to dashboard if already logged in
onMounted(() => {
  if (authStore.isAuthenticated) {
    navigateTo('/dashboard')
  }
})

const testLoading = () => {
  start('Testing loading animation...')
  setTimeout(() => {
    finish()
  }, 3000)
}
</script>

<style scoped>
.landing-page {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  background: linear-gradient(135deg, var(--optiviera-navy) 0%, var(--optiviera-blue) 50%, var(--optiviera-green) 100%);
}

.landing-container {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  gap: 4rem;
}

.hero {
  text-align: center;
  color: white;
  max-width: 800px;
  animation: fadeInUp 0.6s ease-out;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.hero-logo-link {
  display: inline-block;
  transition: all 0.3s;
  margin-bottom: 2rem;
}

.hero-logo-link:hover {
  transform: scale(1.05);
}

.hero-logo {
  height: 120px;
  width: auto;
  filter: drop-shadow(0 4px 12px rgba(0, 0, 0, 0.2));
}

.hero h1 {
  font-size: 3.5rem;
  margin-bottom: 1rem;
  font-weight: 700;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.hero p {
  font-size: 1.5rem;
  margin-bottom: 1rem;
  opacity: 0.95;
}

.hero-description {
  font-size: 1.1rem;
  opacity: 0.85;
  margin-bottom: 2.5rem;
  line-height: 1.6;
}

.cta-buttons {
  display: flex;
  gap: 1.5rem;
  justify-content: center;
  flex-wrap: wrap;
}

.cta-btn {
  padding: 1rem 2.5rem;
  font-size: 1.1rem;
  font-weight: 600;
  border-radius: 8px;
  transition: all 0.3s;
}

.cta-btn-primary {
  background: white;
  color: var(--optiviera-navy);
  border: none;
  box-shadow: 0 4px 12px rgba(255, 255, 255, 0.3);
}

.cta-btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(255, 255, 255, 0.4);
}

.cta-btn-secondary {
  border: 2px solid white;
  color: white;
}

.cta-btn-secondary:hover {
  background: white;
  color: var(--optiviera-navy);
  transform: translateY(-2px);
}

.cta-btn-test {
  border: 2px solid #f59e0b;
  color: #f59e0b;
}

.cta-btn-test:hover {
  background: #f59e0b;
  color: white;
  transform: translateY(-2px);
}

/* Features */
.features {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
  max-width: 1200px;
  width: 100%;
  animation: fadeInUp 0.6s ease-out 0.2s backwards;
}

.feature-card {
  background: rgba(255, 255, 255, 0.95);
  padding: 2.5rem 2rem;
  border-radius: 16px;
  text-align: center;
  transition: all 0.3s;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.feature-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
}

.feature-card i {
  font-size: 3rem;
  color: var(--optiviera-blue);
  margin-bottom: 1rem;
  display: block;
}

.feature-card h3 {
  font-size: 1.5rem;
  color: var(--optiviera-navy);
  margin-bottom: 1rem;
  font-weight: 700;
}

.feature-card p {
  color: var(--text-color-secondary);
  line-height: 1.6;
  margin: 0;
}

/* Footer */
.landing-footer {
  background-color: rgba(5, 68, 94, 0.9);
  color: rgba(255, 255, 255, 0.8);
  padding: 1.5rem 2rem;
  text-align: center;
}

.landing-footer p {
  margin: 0;
  font-size: 0.9rem;
}

/* Responsive */
@media (max-width: 768px) {
  .hero h1 {
    font-size: 2rem;
  }

  .hero p {
    font-size: 1.2rem;
  }

  .hero-description {
    font-size: 1rem;
  }

  .hero-logo-link {
    margin-bottom: 1.5rem;
  }

  .hero-logo {
    height: 80px;
  }

  .cta-buttons {
    flex-direction: column;
    width: 100%;
  }

  .cta-btn {
    width: 100%;
  }

  .features {
    grid-template-columns: 1fr;
  }
}
</style>
