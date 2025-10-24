<template>
  <div class="auth-page">
    <Card class="auth-card">
      <template #header>
        <div class="auth-header">
          <h2>Create Your Account</h2>
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
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 1rem;
}

.auth-card {
  width: 100%;
  max-width: 500px;
}

.auth-header {
  text-align: center;
  padding: 2rem 2rem 0;
}

.auth-header h2 {
  margin: 0;
  color: #333;
}

.field {
  margin-bottom: 1.5rem;
}

.field-group {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.field label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #333;
}

.error-message {
  padding: 0.75rem;
  margin-bottom: 1rem;
  background-color: #fee;
  color: #c33;
  border-radius: 4px;
  border-left: 4px solid #c33;
}

.auth-footer {
  margin-top: 1.5rem;
  text-align: center;
  padding-top: 1rem;
  border-top: 1px solid #e0e0e0;
}

.auth-footer p {
  margin: 0;
  color: #666;
}

.auth-footer a {
  color: #667eea;
  text-decoration: none;
  font-weight: 600;
}

.auth-footer a:hover {
  text-decoration: underline;
}
</style>
