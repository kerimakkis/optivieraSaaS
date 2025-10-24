<template>
  <div class="page-container">
    <h1>Dashboard</h1>
    <div class="dashboard-grid">
      <Card>
        <template #title>Welcome, {{ authStore.user?.fullName }}</template>
        <template #content>
          <p>Your tenant: {{ authStore.user?.tenantId }}</p>
          <p>Roles: {{ authStore.user?.roles.join(', ') }}</p>
        </template>
      </Card>

      <Card>
        <template #title>Quick Actions</template>
        <template #content>
          <div class="actions">
            <Button label="New Ticket" icon="pi pi-plus" @click="navigateTo('/dashboard/tickets/new')" />
            <Button label="View Tickets" icon="pi pi-list" severity="secondary" @click="navigateTo('/dashboard/tickets')" />
          </div>
        </template>
      </Card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '~/stores/auth'

definePageMeta({
  layout: 'dashboard',
  middleware: ['auth']
})

const authStore = useAuthStore()
</script>

<style scoped>
.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
  margin-top: 2rem;
}

.actions {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}
</style>
