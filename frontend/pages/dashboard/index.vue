<template>
  <div class="dashboard-page">
    <div class="page-header">
      <div class="page-header-content">
        <NuxtLink to="/dashboard" class="page-logo-link">
          <img src="/logo.png" alt="Optiviera" class="page-logo" />
        </NuxtLink>
        <div class="page-header-text">
          <h1 class="page-title">Welcome to Optiviera ERP</h1>
          <h2 class="page-subtitle">WOM Module Dashboard</h2>
          <p class="page-description">Welcome back, {{ authStore.user?.fullName }}!</p>
        </div>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="stats-grid">
      <Card class="stat-card stat-card-primary">
        <template #content>
          <div class="stat-content">
            <div class="stat-icon">
              <i class="pi pi-ticket"></i>
            </div>
            <div class="stat-info">
              <h3 class="stat-value">{{ reportData.totalTickets }}</h3>
              <p class="stat-label">Total Tickets</p>
            </div>
          </div>
        </template>
      </Card>

      <Card class="stat-card stat-card-success">
        <template #content>
          <div class="stat-content">
            <div class="stat-icon">
              <i class="pi pi-check-circle"></i>
            </div>
            <div class="stat-info">
              <h3 class="stat-value">{{ reportData.completedTickets }}</h3>
              <p class="stat-label">Completed</p>
            </div>
          </div>
        </template>
      </Card>

      <Card class="stat-card stat-card-warning">
        <template #content>
          <div class="stat-content">
            <div class="stat-icon">
              <i class="pi pi-clock"></i>
            </div>
            <div class="stat-info">
              <h3 class="stat-value">{{ reportData.inProgressTickets }}</h3>
              <p class="stat-label">In Progress</p>
            </div>
          </div>
        </template>
      </Card>

      <Card class="stat-card stat-card-info">
        <template #content>
          <div class="stat-content">
            <div class="stat-icon">
              <i class="pi pi-calendar"></i>
            </div>
            <div class="stat-info">
              <h3 class="stat-value">{{ reportData.recentTickets }}</h3>
              <p class="stat-label">Last 30 Days</p>
            </div>
          </div>
        </template>
      </Card>
    </div>

    <!-- Module Cards -->
    <div class="module-cards">
      <Card class="module-card">
        <template #content>
          <div class="module-content">
            <div class="module-icon">
              <i class="pi pi-ticket"></i>
            </div>
            <div class="module-info">
              <h3>Ticket Management</h3>
              <p>Create, track and manage service tickets efficiently</p>
              <NuxtLink to="/dashboard/tickets">
                <Button
                  label="Go to Tickets"
                  icon="pi pi-arrow-right"
                  class="module-btn"
                />
              </NuxtLink>
            </div>
          </div>
        </template>
      </Card>

      <Card class="module-card">
        <template #content>
          <div class="module-content">
            <div class="module-icon">
              <i class="pi pi-users"></i>
            </div>
            <div class="module-info">
              <h3>User Management</h3>
              <p>Manage team members and their permissions</p>
              <NuxtLink v-if="authStore.isAdmin" to="/dashboard/users">
                <Button
                  label="Go to User Management"
                  icon="pi pi-arrow-right"
                  severity="info"
                  class="module-btn"
                />
              </NuxtLink>
              <Button
                v-else
                label="Admin Required"
                icon="pi pi-lock"
                severity="secondary"
                disabled
                class="module-btn"
              />
            </div>
          </div>
        </template>
      </Card>

      <Card class="module-card">
        <template #content>
          <div class="module-content">
            <div class="module-icon">
              <i class="pi pi-chart-bar"></i>
            </div>
            <div class="module-info">
              <h3>Reports & Analytics</h3>
              <p>View detailed reports and analytics</p>
              <NuxtLink v-if="authStore.isAdmin || authStore.isManager" to="/dashboard/reports">
                <Button
                  label="Go to Reports"
                  icon="pi pi-arrow-right"
                  severity="success"
                  class="module-btn"
                />
              </NuxtLink>
              <Button
                v-else
                label="Manager Required"
                icon="pi pi-lock"
                severity="secondary"
                disabled
                class="module-btn"
              />
            </div>
          </div>
        </template>
      </Card>
    </div>

    <!-- Account Information -->
    <Card class="account-card">
      <template #title>Account Information</template>
      <template #content>
        <div class="info-list">
          <div class="info-item">
            <span class="info-label">Name:</span>
            <span class="info-value">{{ authStore.user?.fullName }}</span>
          </div>
          <div class="info-item">
            <span class="info-label">Email:</span>
            <span class="info-value">{{ authStore.user?.email }}</span>
          </div>
          <div class="info-item">
            <span class="info-label">Role:</span>
            <span class="info-value">
              <span class="role-badge">{{ authStore.user?.roles?.join(', ') || 'N/A' }}</span>
            </span>
          </div>
          <div class="info-item">
            <span class="info-label">Tenant ID:</span>
            <span class="info-value">{{ authStore.user?.tenantId }}</span>
          </div>
        </div>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '~/stores/auth'

definePageMeta({
  layout: 'dashboard',
  middleware: ['auth']
})

const authStore = useAuthStore()

// Mock report data (replace with API call)
const reportData = ref({
  totalTickets: 0,
  completedTickets: 0,
  inProgressTickets: 0,
  recentTickets: 0
})

// Fetch report data
const fetchReportData = async () => {
  try {
    // TODO: Implement API call to fetch report data
    // const config = useRuntimeConfig()
    // const data = await $fetch(`${config.public.apiBase}/reports/dashboard`, {
    //   headers: { Authorization: `Bearer ${authStore.token}` }
    // })
    // reportData.value = data

    // Mock data for now
    reportData.value = {
      totalTickets: 0,
      completedTickets: 0,
      inProgressTickets: 0,
      recentTickets: 0
    }
  } catch (error) {
    console.error('Failed to fetch report data:', error)
  }
}

onMounted(() => {
  fetchReportData()
})
</script>

<style scoped>
.dashboard-page {
  animation: fadeIn 0.3s ease-in;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

.page-header {
  margin-bottom: 2rem;
}

.page-header-content {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.page-logo {
  height: 60px;
  width: auto;
  filter: drop-shadow(0 2px 8px rgba(24, 154, 180, 0.3));
}

.page-header-text {
  flex: 1;
}

.page-title {
  font-size: 2.5rem;
  font-weight: 700;
  color: var(--optiviera-navy);
  margin: 0 0 0.5rem 0;
}

.page-subtitle {
  font-size: 1.5rem;
  font-weight: 600;
  color: var(--optiviera-blue);
  margin: 0 0 0.5rem 0;
}

.page-description {
  font-size: 1.1rem;
  color: var(--text-color-secondary);
  margin: 0;
}

/* Stats Grid */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.stat-card {
  border-left: 4px solid var(--optiviera-blue);
  transition: transform 0.2s, box-shadow 0.2s;
}

.stat-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
}

.stat-card-primary { border-left-color: var(--optiviera-blue); }
.stat-card-success { border-left-color: var(--optiviera-green); }
.stat-card-warning { border-left-color: #f59e0b; }
.stat-card-info { border-left-color: var(--optiviera-navy); }

.stat-content {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.stat-icon {
  width: 60px;
  height: 60px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, var(--optiviera-blue), var(--optiviera-green));
  color: white;
  font-size: 1.75rem;
}

.stat-card-success .stat-icon {
  background: linear-gradient(135deg, var(--optiviera-green), #75E6DA);
}

.stat-card-warning .stat-icon {
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
}

.stat-card-info .stat-icon {
  background: linear-gradient(135deg, var(--optiviera-navy), var(--optiviera-blue));
}

.stat-info {
  flex: 1;
}

.stat-value {
  font-size: 2rem;
  font-weight: 700;
  color: var(--optiviera-navy);
  margin: 0 0 0.25rem 0;
}

.stat-label {
  font-size: 0.95rem;
  color: var(--text-color-secondary);
  margin: 0;
}

/* Module Cards */
.module-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: 2rem;
  margin-bottom: 2rem;
}

.module-card {
  height: fit-content;
  border-left: 4px solid var(--optiviera-blue);
  transition: transform 0.2s, box-shadow 0.2s;
}

.module-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
}

.module-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  gap: 1.5rem;
  padding: 1rem;
}

.module-icon {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, var(--optiviera-blue), var(--optiviera-green));
  color: white;
  font-size: 2rem;
}

.module-info h3 {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--optiviera-navy);
  margin: 0 0 0.5rem 0;
}

.module-info p {
  color: var(--text-color-secondary);
  line-height: 1.6;
  margin: 0 0 1rem 0;
}

.module-btn {
  width: 100%;
  justify-content: center;
}

/* Account Card */
.account-card {
  border-left: 4px solid var(--optiviera-green);
}

/* Info List */
.info-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.info-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 0;
  border-bottom: 1px solid var(--surface-border);
}

.info-item:last-child {
  border-bottom: none;
}

.info-label {
  font-weight: 600;
  color: var(--text-color-secondary);
}

.info-value {
  font-weight: 500;
  color: var(--optiviera-navy);
}

.role-badge {
  background: linear-gradient(135deg, var(--optiviera-blue), var(--optiviera-green));
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
  font-size: 0.85rem;
  font-weight: 600;
}

/* Quick Actions */
.quick-actions {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.action-link {
  text-decoration: none;
  width: 100%;
}

.action-btn {
  width: 100%;
  justify-content: flex-start;
}

/* Responsive */
@media (max-width: 768px) {
  .page-header-content {
    gap: 1rem;
  }

  .page-logo {
    height: 40px;
  }

  .page-title {
    font-size: 1.8rem;
  }

  .page-subtitle {
    font-size: 1.2rem;
  }

  .page-description {
    font-size: 1rem;
  }

  .stats-grid {
    grid-template-columns: 1fr;
  }

  .module-cards {
    grid-template-columns: 1fr;
  }
}
</style>
