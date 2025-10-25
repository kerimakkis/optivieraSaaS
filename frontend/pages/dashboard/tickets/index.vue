<template>
  <div class="tickets-page">
    <div class="page-header">
      <div class="header-left">
        <NuxtLink to="/dashboard">
          <img src="/logo.png" alt="Optiviera" class="page-logo" />
        </NuxtLink>
        <div class="header-text">
          <h1 class="page-title">Tickets</h1>
          <p class="page-subtitle">Manage and track all service tickets</p>
        </div>
      </div>
      <NuxtLink to="/dashboard/tickets/new">
        <Button
          label="Create New Ticket"
          icon="pi pi-plus"
          class="create-btn"
        />
      </NuxtLink>
    </div>

    <!-- Filters Card -->
    <Card class="filters-card">
      <template #content>
        <div class="filters-row">
          <div class="filter-group">
            <label>Search</label>
            <InputText
              v-model="filters.search"
              placeholder="Search tickets..."
              class="filter-input"
            >
              <template #prefix>
                <i class="pi pi-search"></i>
              </template>
            </InputText>
          </div>

          <div class="filter-group">
            <label>Status</label>
            <Dropdown
              v-model="filters.status"
              :options="statusOptions"
              optionLabel="label"
              optionValue="value"
              placeholder="All Statuses"
              class="filter-input"
              :showClear="true"
            />
          </div>

          <div class="filter-group">
            <label>Priority</label>
            <Dropdown
              v-model="filters.priority"
              :options="priorityOptions"
              optionLabel="label"
              optionValue="value"
              placeholder="All Priorities"
              class="filter-input"
              :showClear="true"
            />
          </div>

          <div class="filter-group">
            <label>Archived</label>
            <Dropdown
              v-model="filters.archived"
              :options="archivedOptions"
              optionLabel="label"
              optionValue="value"
              placeholder="Active Only"
              class="filter-input"
            />
          </div>
        </div>
      </template>
    </Card>

    <!-- Tickets Table -->
    <Card class="table-card">
      <template #content>
        <DataTable
          :value="tickets"
          :loading="loading"
          stripedRows
          :paginator="true"
          :rows="10"
          :rowsPerPageOptions="[5, 10, 20, 50]"
          paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
          currentPageReportTemplate="Showing {first} to {last} of {totalRecords} tickets"
          :globalFilterFields="['title', 'description', 'customerFirstName', 'customerLastName']"
          class="tickets-table"
          @row-click="onRowClick"
        >
          <template #empty>
            <div class="empty-state">
              <i class="pi pi-ticket empty-icon"></i>
              <p>No tickets found</p>
            </div>
          </template>

          <Column field="id" header="ID" :sortable="true" style="width: 80px">
            <template #body="{ data }">
              <span class="ticket-id">#{{ data.id }}</span>
            </template>
          </Column>

          <Column field="title" header="Title" :sortable="true">
            <template #body="{ data }">
              <div class="ticket-title-cell">
                <strong>{{ data.title }}</strong>
                <small>{{ data.customerFirstName }} {{ data.customerLastName }}</small>
              </div>
            </template>
          </Column>

          <Column field="status" header="Status" :sortable="true" style="width: 150px">
            <template #body="{ data }">
              <Tag :value="getStatusLabel(data.status)" :severity="getStatusSeverity(data.status)" />
            </template>
          </Column>

          <Column field="priority" header="Priority" :sortable="true" style="width: 120px">
            <template #body="{ data }">
              <Tag
                v-if="data.priority"
                :value="data.priority.name"
                :severity="getPrioritySeverity(data.priority.name)"
              />
              <span v-else class="text-muted">-</span>
            </template>
          </Column>

          <Column field="technicianName" header="Technician" :sortable="true" style="width: 180px">
            <template #body="{ data }">
              <div v-if="data.technicianName" class="user-cell">
                <i class="pi pi-user"></i>
                {{ data.technicianName }}
              </div>
              <span v-else class="text-muted">Unassigned</span>
            </template>
          </Column>

          <Column field="schedule" header="Scheduled" :sortable="true" style="width: 180px">
            <template #body="{ data }">
              <div v-if="data.schedule" class="date-cell">
                <i class="pi pi-calendar"></i>
                {{ formatDate(data.schedule) }}
              </div>
              <span v-else class="text-muted">Not scheduled</span>
            </template>
          </Column>

          <Column field="created" header="Created" :sortable="true" style="width: 180px">
            <template #body="{ data }">
              <div class="date-cell">
                <i class="pi pi-clock"></i>
                {{ formatDate(data.created) }}
              </div>
            </template>
          </Column>

          <Column style="width: 100px">
            <template #body="{ data }">
              <Button
                icon="pi pi-arrow-right"
                text
                rounded
                @click.stop="viewTicket(data.id)"
                v-tooltip.top="'View Details'"
              />
            </template>
          </Column>
        </DataTable>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import type { Ticket } from '~/types'

definePageMeta({
  layout: 'dashboard',
  middleware: ['auth']
})

const loading = ref(false)
const tickets = ref<Ticket[]>([])

const filters = ref({
  search: '',
  status: null,
  priority: null,
  archived: false
})

const statusOptions = [
  { label: 'Open', value: 1 },
  { label: 'In Progress', value: 2 },
  { label: 'Completed', value: 3 },
  { label: 'Closed', value: 4 }
]

const priorityOptions = [
  { label: 'Low', value: 1 },
  { label: 'Medium', value: 2 },
  { label: 'High', value: 3 },
  { label: 'Critical', value: 4 }
]

const archivedOptions = [
  { label: 'Active Only', value: false },
  { label: 'Archived Only', value: true },
  { label: 'All', value: null }
]

const getStatusLabel = (status: number): string => {
  const labels: { [key: number]: string } = {
    1: 'Open',
    2: 'In Progress',
    3: 'Completed',
    4: 'Closed'
  }
  return labels[status] || 'Unknown'
}

const getStatusSeverity = (status: number): string => {
  const severities: { [key: number]: string } = {
    1: 'info',
    2: 'warn',
    3: 'success',
    4: 'secondary'
  }
  return severities[status] || 'info'
}

const getPrioritySeverity = (priority: string): string => {
  const severities: { [key: string]: string } = {
    'Low': 'info',
    'Medium': 'warn',
    'High': 'danger',
    'Critical': 'danger'
  }
  return severities[priority] || 'info'
}

const formatDate = (dateStr: string): string => {
  const date = new Date(dateStr)
  return date.toLocaleString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const router = useRouter()

const onRowClick = (event: any) => {
  viewTicket(event.data.id)
}

const viewTicket = (id: number) => {
  router.push(`/dashboard/tickets/${id}`)
}

const fetchTickets = async () => {
  loading.value = true
  try {
    // TODO: Implement API call to fetch tickets
    // const config = useRuntimeConfig()
    // const authStore = useAuthStore()
    // tickets.value = await $fetch(`${config.public.apiBase}/tickets`, {
    //   headers: { Authorization: `Bearer ${authStore.token}` }
    // })

    // Mock data for now
    tickets.value = []
  } catch (error) {
    console.error('Failed to fetch tickets:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchTickets()
})
</script>

<style scoped>
.tickets-page {
  animation: fadeIn 0.3s ease-in;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 2rem;
  gap: 2rem;
}

.header-left {
  flex: 1;
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.page-logo {
  height: 60px;
  width: auto;
  filter: drop-shadow(0 2px 8px rgba(24, 154, 180, 0.3));
}

.header-text {
  flex: 1;
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

.create-btn {
  background: linear-gradient(135deg, var(--optiviera-blue), var(--optiviera-green));
  border: none;
  font-weight: 600;
  padding: 0.75rem 1.5rem;
  box-shadow: 0 4px 12px rgba(24, 154, 180, 0.3);
  transition: all 0.3s;
}

.create-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(24, 154, 180, 0.4);
}

/* Filters Card */
.filters-card {
  margin-bottom: 2rem;
  border-left: 4px solid var(--optiviera-blue);
}

.filters-row {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1.5rem;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.filter-group label {
  font-weight: 600;
  font-size: 0.9rem;
  color: var(--optiviera-navy);
}

.filter-input {
  width: 100%;
}

/* Table Card */
.table-card {
  border-left: 4px solid var(--optiviera-green);
}

.tickets-table {
  font-size: 0.95rem;
}

.tickets-table :deep(.p-datatable-thead) {
  background: linear-gradient(135deg, var(--optiviera-navy), var(--optiviera-blue));
}

.tickets-table :deep(.p-datatable-thead > tr > th) {
  color: white;
  font-weight: 600;
  padding: 1rem;
}

.tickets-table :deep(.p-datatable-tbody > tr) {
  cursor: pointer;
  transition: all 0.2s;
}

.tickets-table :deep(.p-datatable-tbody > tr:hover) {
  background-color: var(--optiviera-light) !important;
}

.empty-state {
  text-align: center;
  padding: 3rem 1rem;
  color: var(--text-color-secondary);
}

.empty-icon {
  font-size: 4rem;
  color: var(--optiviera-blue);
  opacity: 0.3;
  margin-bottom: 1rem;
}

.ticket-id {
  font-weight: 700;
  color: var(--optiviera-blue);
  font-size: 0.9rem;
}

.ticket-title-cell {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.ticket-title-cell strong {
  color: var(--optiviera-navy);
}

.ticket-title-cell small {
  color: var(--text-color-secondary);
  font-size: 0.85rem;
}

.user-cell,
.date-cell {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--text-color);
}

.user-cell i,
.date-cell i {
  color: var(--optiviera-blue);
}

.text-muted {
  color: var(--text-color-secondary);
  font-style: italic;
}

/* Responsive */
@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    gap: 1rem;
  }

  .header-left {
    gap: 1rem;
    width: 100%;
  }

  .page-logo {
    height: 40px;
  }

  .page-title {
    font-size: 1.5rem;
  }

  .page-subtitle {
    font-size: 1rem;
  }

  .create-btn {
    width: 100%;
  }

  .filters-row {
    grid-template-columns: 1fr;
  }

  .tickets-table {
    font-size: 0.85rem;
  }
}
</style>
