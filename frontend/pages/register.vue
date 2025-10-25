<template>
  <div class="auth-page">
    <Card class="auth-card">
      <template #header>
        <div class="auth-header">
          <NuxtLink to="/dashboard" class="logo-link">
            <img src="/logo.png" alt="Optiviera Logo" class="auth-logo" />
          </NuxtLink>
          <h2>Create Your Account</h2>
          <p class="auth-subtitle">Start managing your tickets today</p>
        </div>
      </template>
      <template #content>
        <form @submit.prevent="handleRegister">
          <div class="field">
            <label for="companyName">Company Name</label>
            <InputText
              id="companyName"
              v-model="formData.companyName"
              placeholder="Your Company"
              required
              fluid
            />
          </div>

          <div class="field-group">
            <div class="field">
              <label for="firstName">First Name</label>
              <InputText
                id="firstName"
                v-model="formData.firstName"
                placeholder="John"
                required
                fluid
              />
            </div>

            <div class="field">
              <label for="lastName">Last Name</label>
              <InputText
                id="lastName"
                v-model="formData.lastName"
                placeholder="Doe"
                required
                fluid
              />
            </div>
          </div>

          <div class="field">
            <label for="email">Email</label>
            <InputText
              id="email"
              v-model="formData.email"
              type="email"
              placeholder="john@company.com"
              required
              fluid
            />
          </div>

          <div class="field">
            <label for="password">Password</label>
            <Password
              id="password"
              v-model="formData.password"
              placeholder="Minimum 6 characters"
              toggleMask
              required
              fluid
            />
          </div>

          <div v-if="authStore.error" class="error-message">
            {{ authStore.error }}
          </div>

          <Button
            type="submit"
            label="Create Account"
            icon="pi pi-user-plus"
            :loading="authStore.loading"
            fluid
          />
        </form>

        <div class="auth-footer">
          <p>Already have an account? <NuxtLink to="/login">Login here</NuxtLink></p>
        </div>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '~/stores/auth'

definePageMeta({
  layout: 'default',
  middleware: ['guest']
})

const authStore = useAuthStore()
const router = useRouter()

const formData = ref({
  companyName: '',
  email: '',
  password: '',
  firstName: '',
  lastName: ''
})

const handleRegister = async () => {
  try {
    await authStore.register(formData.value)
    router.push('/dashboard')
  } catch (error) {
    // Error is already set in the store
  }
}
</script>

<style scoped>
.auth-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #05445E 0%, #189AB4 50%, #75E6DA 100%);
  padding: 1rem;
  position: relative;
  overflow: hidden;
}

.auth-page::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1440 320"><path fill="%2375E6DA" fill-opacity="0.1" d="M0,96L48,112C96,128,192,160,288,186.7C384,213,480,235,576,213.3C672,192,768,128,864,128C960,128,1056,192,1152,197.3C1248,203,1344,149,1392,122.7L1440,96L1440,320L1392,320C1344,320,1248,320,1152,320C1056,320,960,320,864,320C768,320,672,320,576,320C480,320,384,320,288,320C192,320,96,320,48,320L0,320Z"></path></svg>') no-repeat bottom;
  background-size: cover;
}

.auth-card {
  width: 100%;
  max-width: 500px;
  position: relative;
  z-index: 1;
  box-shadow: 0 20px 60px rgba(5, 68, 94, 0.3);
}

.auth-header {
  text-align: center;
  padding: 2rem 2rem 1rem;
}

.auth-header .logo-link {
  display: inline-block;
  transition: all 0.3s;
  margin-bottom: 1rem;
}

.auth-header .logo-link:hover {
  transform: scale(1.05);
  opacity: 0.9;
}

.auth-logo {
  max-width: 180px;
  height: auto;
}

.auth-header h2 {
  margin: 0 0 0.5rem 0;
  color: #05445E;
  font-size: 1.75rem;
  font-weight: 700;
}

.auth-subtitle {
  margin: 0;
  color: #6c757d;
  font-size: 0.95rem;
}

.field {
  margin-bottom: 1.5rem;
}

.field-group {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.field label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #05445E;
}

.error-message {
  padding: 0.75rem;
  margin-bottom: 1rem;
  background-color: #fee2e2;
  color: #dc2626;
  border-radius: 6px;
  border-left: 4px solid #dc2626;
  font-size: 0.9rem;
}

.auth-footer {
  margin-top: 1.5rem;
  text-align: center;
  padding-top: 1rem;
  border-top: 1px solid #e5e7eb;
}

.auth-footer p {
  margin: 0;
  color: #6c757d;
  font-size: 0.95rem;
}

.auth-footer a {
  color: #189AB4;
  text-decoration: none;
  font-weight: 600;
  transition: color 0.2s;
}

.auth-footer a:hover {
  color: #05445E;
  text-decoration: underline;
}

/* Responsive */
@media (max-width: 768px) {
  .auth-card {
    max-width: 100%;
  }

  .auth-header {
    padding: 1.5rem 1.5rem 1rem;
  }

  .auth-logo {
    max-width: 140px;
  }

  .auth-header h2 {
    font-size: 1.5rem;
  }

  .field-group {
    grid-template-columns: 1fr;
    gap: 0;
    margin-bottom: 0;
  }

  .field-group .field {
    margin-bottom: 1.5rem;
  }
}
</style>
