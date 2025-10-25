<template>
  <div class="ticket-detail-page">
    <div class="page-header">
      <div class="header-left">
        <NuxtLink to="/dashboard/tickets">
          <Button
            icon="pi pi-arrow-left"
            text
            rounded
            v-tooltip.right="'Back to Tickets'"
            class="back-btn"
          />
        </NuxtLink>
        <NuxtLink to="/dashboard">
          <img src="/logo.png" alt="Optiviera" class="page-logo" />
        </NuxtLink>
        <div>
          <h1 class="page-title">Ticket #{{ ticketId }}</h1>
          <p class="page-subtitle">View and manage ticket details</p>
        </div>
      </div>
      <div class="header-actions">
        <Button
          v-if="!editing"
          label="Edit"
          icon="pi pi-pencil"
          @click="editing = true"
          outlined
        />
        <Button
          v-if="editing"
          label="Cancel"
          icon="pi pi-times"
          severity="secondary"
          @click="cancelEdit"
          outlined
        />
        <Button
          v-if="editing"
          label="Save Changes"
          icon="pi pi-check"
          @click="handleSave"
          :loading="saving"
          class="save-btn"
        />
      </div>
    </div>

    <div class="ticket-content">
      <!-- Left Column -->
      <div class="left-column">
        <!-- Ticket Information -->
        <Card class="info-card">
          <template #title>
            <div class="card-title">
              <i class="pi pi-ticket"></i>
              Ticket Information
            </div>
          </template>
          <template #content>
            <div class="info-grid">
              <div class="info-item full-width">
                <label>Title</label>
                <InputText
                  v-if="editing"
                  v-model="ticketData.title"
                  placeholder="Ticket title"
                />
                <div v-else class="info-value">{{ ticketData.title || '-' }}</div>
              </div>

              <div class="info-item full-width">
                <label>Description</label>
                <Textarea
                  v-if="editing"
                  v-model="ticketData.description"
                  rows="4"
                  placeholder="Ticket description"
                />
                <div v-else class="info-value description">{{ ticketData.description || '-' }}</div>
              </div>

              <div class="info-item">
                <label>Status</label>
                <Dropdown
                  v-if="editing"
                  v-model="ticketData.status"
                  :options="statusOptions"
                  optionLabel="label"
                  optionValue="value"
                />
                <Tag
                  v-else
                  :value="getStatusLabel(ticketData.status)"
                  :severity="getStatusSeverity(ticketData.status)"
                />
              </div>

              <div class="info-item">
                <label>Priority</label>
                <Dropdown
                  v-if="editing"
                  v-model="ticketData.priorityId"
                  :options="priorities"
                  optionLabel="name"
                  optionValue="id"
                />
                <Tag
                  v-else-if="ticketData.priority"
                  :value="ticketData.priority.name"
                  :severity="getPrioritySeverity(ticketData.priority.name)"
                />
                <span v-else class="text-muted">-</span>
              </div>

              <div class="info-item">
                <label>Technician</label>
                <Dropdown
                  v-if="editing && (authStore.isAdmin || authStore.isManager)"
                  v-model="ticketData.technicianId"
                  :options="technicians"
                  optionLabel="fullName"
                  optionValue="id"
                  placeholder="Select Technician"
                  :showClear="true"
                />
                <div v-else-if="ticketData.technicianName" class="info-value">
                  <i class="pi pi-user"></i>
                  {{ ticketData.technicianName }}
                </div>
                <span v-else class="text-muted">Unassigned</span>
              </div>

              <div class="info-item">
                <label>Support</label>
                <Dropdown
                  v-if="editing && authStore.isAdmin"
                  v-model="ticketData.supportId"
                  :options="supportStaff"
                  optionLabel="fullName"
                  optionValue="id"
                  placeholder="Select Support"
                  :showClear="true"
                />
                <div v-else-if="ticketData.supportName" class="info-value">
                  <i class="pi pi-user"></i>
                  {{ ticketData.supportName }}
                </div>
                <span v-else class="text-muted">Unassigned</span>
              </div>

              <div class="info-item">
                <label>Scheduled</label>
                <Calendar
                  v-if="editing"
                  v-model="ticketData.schedule"
                  showTime
                  hourFormat="24"
                  placeholder="Select date and time"
                  dateFormat="mm/dd/yy"
                />
                <div v-else-if="ticketData.schedule" class="info-value">
                  <i class="pi pi-calendar"></i>
                  {{ formatDate(ticketData.schedule) }}
                </div>
                <span v-else class="text-muted">Not scheduled</span>
              </div>

              <div class="info-item">
                <label>Created</label>
                <div class="info-value">
                  <i class="pi pi-clock"></i>
                  {{ formatDate(ticketData.created) }}
                </div>
              </div>

              <div class="info-item" v-if="editing">
                <label>Archived</label>
                <div class="checkbox-wrapper">
                  <Checkbox v-model="ticketData.isArchived" :binary="true" inputId="archived" />
                  <label for="archived">Archive this ticket</label>
                </div>
              </div>
            </div>
          </template>
        </Card>

        <!-- Customer Information -->
        <Card class="info-card">
          <template #title>
            <div class="card-title">
              <i class="pi pi-user"></i>
              Customer Information
            </div>
          </template>
          <template #content>
            <div class="customer-info">
              <div class="info-row">
                <span class="info-label">Name:</span>
                <span class="info-value">
                  {{ ticketData.customerFirstName }} {{ ticketData.customerLastName }}
                </span>
              </div>
              <div class="info-row">
                <span class="info-label">Phone:</span>
                <span class="info-value">{{ ticketData.customerPhone }}</span>
              </div>
              <div class="info-row">
                <span class="info-label">Address:</span>
                <span class="info-value">
                  {{ ticketData.customerAddress }}<br>
                  {{ ticketData.customerCity }}, {{ ticketData.customerState }} {{ ticketData.customerZip }}
                </span>
              </div>
            </div>
          </template>
        </Card>
      </div>

      <!-- Right Column - Comments -->
      <div class="right-column">
        <Card class="comments-card">
          <template #title>
            <div class="card-title">
              <i class="pi pi-comments"></i>
              Comments
              <span class="comment-count">{{ comments.length }}</span>
            </div>
          </template>
          <template #content>
            <!-- Comments List -->
            <div class="comments-list">
              <div v-for="comment in comments" :key="comment.id" class="comment">
                <div class="comment-header">
                  <div class="comment-author">
                    <i class="pi pi-user"></i>
                    {{ comment.userName }}
                  </div>
                  <div class="comment-date">{{ formatDate(comment.created) }}</div>
                </div>
                <div class="comment-body">{{ comment.note }}</div>
              </div>

              <div v-if="comments.length === 0" class="no-comments">
                <i class="pi pi-comment"></i>
                <p>No comments yet</p>
              </div>
            </div>

            <!-- Add Comment Form -->
            <div class="add-comment">
              <Textarea
                v-model="newComment"
                rows="3"
                placeholder="Add a comment..."
                class="comment-input"
              />
              <Button
                label="Add Comment"
                icon="pi pi-send"
                @click="addComment"
                :loading="addingComment"
                :disabled="!newComment.trim()"
                class="add-comment-btn"
              />
            </div>
          </template>
        </Card>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '~/stores/auth'
import type { TicketDetail, UpdateTicketRequest } from '~/types'

definePageMeta({
  layout: 'dashboard',
  middleware: ['auth']
})

const route = useRoute()
const authStore = useAuthStore()

const ticketId = ref(route.params.id as string)
const editing = ref(false)
const saving = ref(false)
const addingComment = ref(false)

const ticketData = ref<any>({
  title: '',
  description: '',
  status: 1,
  priority: null,
  priorityId: 0,
  customerFirstName: '',
  customerLastName: '',
  customerPhone: '',
  customerAddress: '',
  customerCity: '',
  customerState: '',
  customerZip: '',
  created: new Date().toISOString(),
  schedule: null,
  isArchived: false,
  technicianName: null,
  technicianId: null,
  supportName: null,
  supportId: null
})

const originalData = ref<any>(null)
const comments = ref<any[]>([])
const newComment = ref('')

const statusOptions = [
  { label: 'Open', value: 1 },
  { label: 'In Progress', value: 2 },
  { label: 'Completed', value: 3 },
  { label: 'Closed', value: 4 }
]

const priorities = ref([
  { id: 1, name: 'Low' },
  { id: 2, name: 'Medium' },
  { id: 3, name: 'High' },
  { id: 4, name: 'Critical' }
])

const technicians = ref([])
const supportStaff = ref([])

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

const cancelEdit = () => {
  ticketData.value = { ...originalData.value }
  editing.value = false
}

const handleSave = async () => {
  saving.value = true

  try {
    // TODO: Implement API call to update ticket
    // const config = useRuntimeConfig()
    // const updateData: UpdateTicketRequest = {
    //   title: ticketData.value.title,
    //   description: ticketData.value.description,
    //   status: ticketData.value.status,
    //   priorityId: ticketData.value.priorityId,
    //   technicianId: ticketData.value.technicianId,
    //   supportId: ticketData.value.supportId,
    //   schedule: ticketData.value.schedule,
    //   isArchived: ticketData.value.isArchived
    // }
    //
    // await $fetch(`${config.public.apiBase}/tickets/${ticketId.value}`, {
    //   method: 'PUT',
    //   body: updateData,
    //   headers: { Authorization: `Bearer ${authStore.token}` }
    // })

    console.log('Ticket updated:', ticketData.value)
    originalData.value = { ...ticketData.value }
    editing.value = false
  } catch (error) {
    console.error('Failed to update ticket:', error)
  } finally {
    saving.value = false
  }
}

const addComment = async () => {
  if (!newComment.value.trim()) return

  addingComment.value = true

  try {
    // TODO: Implement API call to add comment
    // const config = useRuntimeConfig()
    // const comment = await $fetch(`${config.public.apiBase}/tickets/${ticketId.value}/comments`, {
    //   method: 'POST',
    //   body: { note: newComment.value },
    //   headers: { Authorization: `Bearer ${authStore.token}` }
    // })

    // comments.value.push(comment)

    console.log('Comment added:', newComment.value)
    newComment.value = ''
  } catch (error) {
    console.error('Failed to add comment:', error)
  } finally {
    addingComment.value = false
  }
}

const fetchTicket = async () => {
  try {
    // TODO: Implement API call to fetch ticket
    // const config = useRuntimeConfig()
    // ticketData.value = await $fetch(`${config.public.apiBase}/tickets/${ticketId.value}`, {
    //   headers: { Authorization: `Bearer ${authStore.token}` }
    // })

    // Mock data for now
    originalData.value = { ...ticketData.value }
  } catch (error) {
    console.error('Failed to fetch ticket:', error)
  }
}

const fetchUsers = async () => {
  try {
    // TODO: Implement API call
    technicians.value = []
    supportStaff.value = []
  } catch (error) {
    console.error('Failed to fetch users:', error)
  }
}

onMounted(() => {
  fetchTicket()
  if (authStore.isAdmin || authStore.isManager) {
    fetchUsers()
  }
})
</script>

<style scoped>
.ticket-detail-page {
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
  display: flex;
  align-items: center;
  gap: 1rem;
  flex: 1;
}

.back-btn {
  color: var(--optiviera-blue);
}

.back-btn:hover {
  background-color: var(--optiviera-light);
}

.page-logo {
  height: 50px;
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

.header-actions {
  display: flex;
  gap: 0.75rem;
}

.save-btn {
  background: linear-gradient(135deg, var(--optiviera-blue), var(--optiviera-green));
  border: none;
  font-weight: 600;
}

/* Content Layout */
.ticket-content {
  display: grid;
  grid-template-columns: 1fr 450px;
  gap: 2rem;
}

.left-column,
.right-column {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

/* Cards */
.info-card,
.comments-card {
  border-left: 4px solid var(--optiviera-blue);
}

.card-title {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  color: var(--optiviera-navy);
  font-size: 1.25rem;
}

.card-title i {
  color: var(--optiviera-blue);
}

.comment-count {
  margin-left: auto;
  background: var(--optiviera-blue);
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 12px;
  font-size: 0.9rem;
  font-weight: 600;
}

/* Info Grid */
.info-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1.5rem;
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.info-item.full-width {
  grid-column: 1 / -1;
}

.info-item label {
  font-weight: 600;
  color: var(--optiviera-navy);
  font-size: 0.9rem;
}

.info-value {
  color: var(--text-color);
  padding: 0.5rem 0;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.info-value i {
  color: var(--optiviera-blue);
}

.info-value.description {
  white-space: pre-wrap;
  line-height: 1.6;
}

.checkbox-wrapper {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.text-muted {
  color: var(--text-color-secondary);
  font-style: italic;
}

/* Customer Info */
.customer-info {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.info-row {
  display: flex;
  gap: 1rem;
  padding: 0.75rem 0;
  border-bottom: 1px solid var(--surface-border);
}

.info-row:last-child {
  border-bottom: none;
}

.info-label {
  font-weight: 600;
  color: var(--text-color-secondary);
  min-width: 80px;
}

.info-row .info-value {
  flex: 1;
  padding: 0;
}

/* Comments */
.comments-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  max-height: 400px;
  overflow-y: auto;
  margin-bottom: 1.5rem;
  padding-right: 0.5rem;
}

.comment {
  background: var(--surface-50);
  border-left: 3px solid var(--optiviera-blue);
  border-radius: 8px;
  padding: 1rem;
}

.comment-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.75rem;
}

.comment-author {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: 600;
  color: var(--optiviera-navy);
}

.comment-author i {
  color: var(--optiviera-blue);
}

.comment-date {
  font-size: 0.85rem;
  color: var(--text-color-secondary);
}

.comment-body {
  color: var(--text-color);
  line-height: 1.6;
  white-space: pre-wrap;
}

.no-comments {
  text-align: center;
  padding: 3rem 1rem;
  color: var(--text-color-secondary);
}

.no-comments i {
  font-size: 3rem;
  color: var(--optiviera-blue);
  opacity: 0.3;
  margin-bottom: 1rem;
}

/* Add Comment */
.add-comment {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  padding-top: 1rem;
  border-top: 2px solid var(--surface-border);
}

.comment-input {
  width: 100%;
}

.add-comment-btn {
  align-self: flex-end;
  background: linear-gradient(135deg, var(--optiviera-blue), var(--optiviera-green));
  border: none;
  font-weight: 600;
}

/* Responsive */
@media (max-width: 1024px) {
  .ticket-content {
    grid-template-columns: 1fr;
  }

  .right-column {
    order: 2;
  }
}

@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
  }

  .page-logo {
    height: 35px;
  }

  .header-actions {
    width: 100%;
  }

  .header-actions button {
    flex: 1;
  }

  .page-title {
    font-size: 1.5rem;
  }

  .page-subtitle {
    font-size: 1rem;
  }

  .info-grid {
    grid-template-columns: 1fr;
  }
}
</style>
