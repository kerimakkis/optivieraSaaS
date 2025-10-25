<template>
  <div class="reports-page">
    <div class="page-header">
      <div class="header-left">
        <NuxtLink to="/dashboard">
          <img src="/logo.png" alt="Optiviera" class="page-logo" />
        </NuxtLink>
        <div class="header-text">
          <h1 class="page-title">Reports & Analytics</h1>
          <p class="page-subtitle">View detailed reports and analytics</p>
        </div>
      </div>
    </div>

    <!-- Report Cards -->
    <div class="report-cards">
      <Card class="report-card">
        <template #title>
          <i class="pi pi-chart-line"></i>
          Ticket Statistics
        </template>
        <template #content>
          <div class="report-content">
            <p>View comprehensive ticket statistics and trends</p>
            <Button
              label="View Statistics"
              icon="pi pi-eye"
              @click="viewStatistics"
              class="report-btn"
            />
          </div>
        </template>
      </Card>

      <Card class="report-card">
        <template #title>
          <i class="pi pi-users"></i>
          User Performance
        </template>
        <template #content>
          <div class="report-content">
            <p>Analyze user performance and productivity metrics</p>
            <Button
              label="View Performance"
              icon="pi pi-eye"
              @click="viewPerformance"
              class="report-btn"
            />
          </div>
        </template>
      </Card>

      <Card class="report-card">
        <template #title>
          <i class="pi pi-calendar"></i>
          Time Reports
        </template>
        <template #content>
          <div class="report-content">
            <p>Generate time-based reports and schedules</p>
            <Button
              label="View Time Reports"
              icon="pi pi-eye"
              @click="viewTimeReports"
              class="report-btn"
            />
          </div>
        </template>
      </Card>
    </div>

    <!-- Quick Stats -->
    <Card class="stats-card">
      <template #title>Quick Statistics</template>
      <template #content>
        <div class="stats-grid">
          <div class="stat-item">
            <div class="stat-icon">
              <i class="pi pi-ticket"></i>
            </div>
            <div class="stat-info">
              <h3>{{ quickStats.totalTickets }}</h3>
              <p>Total Tickets</p>
            </div>
          </div>
          <div class="stat-item">
            <div class="stat-icon">
              <i class="pi pi-check-circle"></i>
            </div>
            <div class="stat-info">
              <h3>{{ quickStats.completedTickets }}</h3>
              <p>Completed</p>
            </div>
          </div>
          <div class="stat-item">
            <div class="stat-icon">
              <i class="pi pi-clock"></i>
            </div>
            <div class="stat-info">
              <h3>{{ quickStats.avgResolutionTime }}</h3>
              <p>Avg Resolution (hours)</p>
            </div>
          </div>
          <div class="stat-item">
            <div class="stat-icon">
              <i class="pi pi-star"></i>
            </div>
            <div class="stat-info">
              <h3>{{ quickStats.satisfactionRate }}%</h3>
              <p>Satisfaction Rate</p>
            </div>
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

// Mock quick stats
const quickStats = ref({
  totalTickets: 0,
  completedTickets: 0,
  avgResolutionTime: 0,
  satisfactionRate: 0
})

const viewStatistics = () => {
  // TODO: Implement statistics view
  console.log('Viewing statistics...')
}

const viewPerformance = () => {
  // TODO: Implement performance view
  console.log('Viewing performance...')
}

const viewTimeReports = () => {
  // TODO: Implement time reports view
  console.log('Viewing time reports...')
}

onMounted(() => {
  // TODO: Fetch real statistics
  quickStats.value = {
    totalTickets: 0,
    completedTickets: 0,
    avgResolutionTime: 0,
    satisfactionRate: 0
  }
})
</script>

<style scoped>
.reports-page {
  animation: fadeIn 0.3s ease-in;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

.page-header {
  margin-bottom: 2rem;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.page-logo {
  height: 60px;
  width: auto;
  filter: drop-shadow(0 2px 8px rgba(24, 154, 180, 0.3));
}

.page-title {
  font-size: 2rem;
  font-weight: 700;
  color: var(--optiviera-navy);
  margin: 0 0 0.5rem 0;
}

.page-subtitle {
  font-size: 1.1rem;
  color: var(--text-color-secondary);
  margin: 0;
}

/* Report Cards */
.report-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
  margin-bottom: 2rem;
}

.report-card {
  border-left: 4px solid var(--optiviera-blue);
  transition: transform 0.2s, box-shadow 0.2s;
}

.report-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
}

.report-content {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.report-btn {
  align-self: flex-start;
}

/* Stats Card */
.stats-card {
  border-left: 4px solid var(--optiviera-green);
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 2rem;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background: var(--surface-50);
  border-radius: 8px;
}

.stat-icon {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, var(--optiviera-blue), var(--optiviera-green));
  color: white;
  font-size: 1.5rem;
}

.stat-info h3 {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--optiviera-navy);
  margin: 0 0 0.25rem 0;
}

.stat-info p {
  font-size: 0.9rem;
  color: var(--text-color-secondary);
  margin: 0;
}

/* Responsive */
@media (max-width: 768px) {
  .page-logo {
    height: 40px;
  }

  .page-title {
    font-size: 1.5rem;
  }

  .page-subtitle {
    font-size: 1rem;
  }

  .report-cards {
    grid-template-columns: 1fr;
  }

  .stats-grid {
    grid-template-columns: 1fr;
  }
}
</style>
