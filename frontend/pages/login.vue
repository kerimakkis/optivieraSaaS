<template>
  <div class="auth-page">
    <Card class="auth-card">
      <template #header>
        <div class="auth-header">
          <h2>Login to Optiviera</h2>
        </div>
      </template>
      <template #content>
        <form @submit.prevent="handleLogin">
          <div class="field">
            <label for="email">Email</label>
            <InputText
              id="email"
              v-model="credentials.email"
              type="email"
              placeholder="admin@optiviera.com"
              required
              fluid
            />
          </div>

          <div class="field">
            <label for="password">Password</label>
            <Password
              id="password"
              v-model="credentials.password"
              placeholder="Enter your password"
              :feedback="false"
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
            label="Login"
            icon="pi pi-sign-in"
            :loading="authStore.loading"
            fluid
          />
        </form>

        <div class="auth-footer">
          <p>Don't have an account? <NuxtLink to="/register">Register here</NuxtLink></p>
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

const credentials = ref({
  email: '',
  password: ''
})

const handleLogin = async () => {
  try {
    await authStore.login(credentials.value)
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
  max-width: 450px;
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
