<template>
  <div class="new-ticket-page">
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
          <h1 class="page-title">Create New Ticket</h1>
          <p class="page-subtitle">Fill in the details to create a new service ticket</p>
        </div>
      </div>
    </div>

    <form @submit.prevent="handleSubmit" class="ticket-form">
      <!-- Ticket Details Card -->
      <Card class="form-card">
        <template #title>
          <div class="card-title">
            <i class="pi pi-ticket"></i>
            Ticket Details
          </div>
        </template>
        <template #content>
          <div class="form-grid">
            <div class="form-field full-width">
              <label for="title" class="required">Title</label>
              <InputText
                id="title"
                v-model="form.title"
                placeholder="Brief description of the issue"
                :class="{ 'p-invalid': submitted && !form.title }"
              />
              <small v-if="submitted && !form.title" class="p-error">Title is required</small>
            </div>

            <div class="form-field full-width">
              <label for="description" class="required">Description</label>
              <Textarea
                id="description"
                v-model="form.description"
                rows="5"
                placeholder="Detailed description of the issue and any relevant information"
                :class="{ 'p-invalid': submitted && !form.description }"
              />
              <small v-if="submitted && !form.description" class="p-error">Description is required</small>
            </div>

            <div class="form-field">
              <label for="priority" class="required">Priority</label>
              <Dropdown
                id="priority"
                v-model="form.priorityId"
                :options="priorities"
                optionLabel="name"
                optionValue="id"
                placeholder="Select Priority"
                :class="{ 'p-invalid': submitted && !form.priorityId }"
              />
              <small v-if="submitted && !form.priorityId" class="p-error">Priority is required</small>
            </div>

            <div class="form-field">
              <label for="schedule">Scheduled Date</label>
              <Calendar
                id="schedule"
                v-model="form.schedule"
                showTime
                hourFormat="24"
                placeholder="Select date and time"
                dateFormat="mm/dd/yy"
              />
            </div>

            <div class="form-field" v-if="authStore.isAdmin || authStore.isManager">
              <label for="technician">Assign Technician</label>
              <Dropdown
                id="technician"
                v-model="form.technicianId"
                :options="technicians"
                optionLabel="fullName"
                optionValue="id"
                placeholder="Select Technician"
                :showClear="true"
              />
            </div>

            <div class="form-field" v-if="authStore.isAdmin">
              <label for="support">Assign Support</label>
              <Dropdown
                id="support"
                v-model="form.supportId"
                :options="supportStaff"
                optionLabel="fullName"
                optionValue="id"
                placeholder="Select Support"
                :showClear="true"
              />
            </div>
          </div>
        </template>
      </Card>

      <!-- Customer Information Card -->
      <Card class="form-card">
        <template #title>
          <div class="card-title">
            <i class="pi pi-user"></i>
            Customer Information
          </div>
        </template>
        <template #content>
          <div class="form-grid">
            <div class="form-field">
              <label for="firstName" class="required">First Name</label>
              <InputText
                id="firstName"
                v-model="form.customerFirstName"
                placeholder="Customer first name"
                :class="{ 'p-invalid': submitted && !form.customerFirstName }"
              />
              <small v-if="submitted && !form.customerFirstName" class="p-error">First name is required</small>
            </div>

            <div class="form-field">
              <label for="lastName" class="required">Last Name</label>
              <InputText
                id="lastName"
                v-model="form.customerLastName"
                placeholder="Customer last name"
                :class="{ 'p-invalid': submitted && !form.customerLastName }"
              />
              <small v-if="submitted && !form.customerLastName" class="p-error">Last name is required</small>
            </div>

            <div class="form-field">
              <label for="phone" class="required">Phone</label>
              <InputText
                id="phone"
                v-model="form.customerPhone"
                placeholder="(555) 123-4567"
                :class="{ 'p-invalid': submitted && !form.customerPhone }"
              />
              <small v-if="submitted && !form.customerPhone" class="p-error">Phone is required</small>
            </div>

            <div class="form-field full-width">
              <label for="address" class="required">Address</label>
              <InputText
                id="address"
                v-model="form.customerAddress"
                placeholder="Street address"
                :class="{ 'p-invalid': submitted && !form.customerAddress }"
              />
              <small v-if="submitted && !form.customerAddress" class="p-error">Address is required</small>
            </div>

            <div class="form-field">
              <label for="city" class="required">City</label>
              <InputText
                id="city"
                v-model="form.customerCity"
                placeholder="City"
                :class="{ 'p-invalid': submitted && !form.customerCity }"
              />
              <small v-if="submitted && !form.customerCity" class="p-error">City is required</small>
            </div>

            <div class="form-field">
              <label for="state" class="required">State</label>
              <InputText
                id="state"
                v-model="form.customerState"
                placeholder="State"
                :class="{ 'p-invalid': submitted && !form.customerState }"
              />
              <small v-if="submitted && !form.customerState" class="p-error">State is required</small>
            </div>

            <div class="form-field">
              <label for="zip" class="required">ZIP Code</label>
              <InputText
                id="zip"
                v-model="form.customerZip"
                placeholder="12345"
                :class="{ 'p-invalid': submitted && !form.customerZip }"
              />
              <small v-if="submitted && !form.customerZip" class="p-error">ZIP code is required</small>
            </div>
          </div>
        </template>
      </Card>

      <!-- Form Actions -->
      <div class="form-actions">
        <NuxtLink to="/dashboard/tickets">
          <Button
            label="Cancel"
            severity="secondary"
            outlined
            type="button"
          />
        </NuxtLink>
        <Button
          label="Create Ticket"
          icon="pi pi-check"
          type="submit"
          :loading="loading"
          class="submit-btn"
        />
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '~/stores/auth'
import type { CreateTicketRequest } from '~/types'

definePageMeta({
  layout: 'dashboard',
  middleware: ['auth']
})

const authStore = useAuthStore()
const router = useRouter()
const loading = ref(false)
const submitted = ref(false)

const form = ref<CreateTicketRequest>({
  title: '',
  description: '',
  customerFirstName: '',
  customerLastName: '',
  customerPhone: '',
  customerAddress: '',
  customerCity: '',
  customerState: '',
  customerZip: '',
  priorityId: 0,
  technicianId: undefined,
  supportId: undefined,
  schedule: undefined
})

const priorities = ref([
  { id: 1, name: 'Low' },
  { id: 2, name: 'Medium' },
  { id: 3, name: 'High' },
  { id: 4, name: 'Critical' }
])

const technicians = ref([])
const supportStaff = ref([])

const handleSubmit = async () => {
  submitted.value = true

  // Validate required fields
  if (
    !form.value.title ||
    !form.value.description ||
    !form.value.priorityId ||
    !form.value.customerFirstName ||
    !form.value.customerLastName ||
    !form.value.customerPhone ||
    !form.value.customerAddress ||
    !form.value.customerCity ||
    !form.value.customerState ||
    !form.value.customerZip
  ) {
    return
  }

  loading.value = true

  try {
    // TODO: Implement API call to create ticket
    // const config = useRuntimeConfig()
    // const response = await $fetch(`${config.public.apiBase}/tickets`, {
    //   method: 'POST',
    //   body: form.value,
    //   headers: {
    //     Authorization: `Bearer ${authStore.token}`
    //   }
    // })

    // Show success message
    console.log('Ticket created successfully:', form.value)

    // Navigate back to tickets list
    router.push('/dashboard/tickets')
  } catch (error: any) {
    console.error('Failed to create ticket:', error)
    // TODO: Show error toast
  } finally {
    loading.value = false
  }
}

const fetchUsers = async () => {
  try {
    // TODO: Implement API call to fetch technicians and support staff
    // const config = useRuntimeConfig()
    // const users = await $fetch(`${config.public.apiBase}/users`, {
    //   headers: { Authorization: `Bearer ${authStore.token}` }
    // })

    // technicians.value = users.filter(u => u.roles.includes('Technician'))
    // supportStaff.value = users.filter(u => u.roles.includes('Support'))

    technicians.value = []
    supportStaff.value = []
  } catch (error) {
    console.error('Failed to fetch users:', error)
  }
}

onMounted(() => {
  if (authStore.isAdmin || authStore.isManager) {
    fetchUsers()
  }
})
</script>

<style scoped>
.new-ticket-page {
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
  gap: 1rem;
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

/* Form */
.ticket-form {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.form-card {
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

.form-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
}

.form-field {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-field.full-width {
  grid-column: 1 / -1;
}

.form-field label {
  font-weight: 600;
  color: var(--optiviera-navy);
  font-size: 0.95rem;
}

.form-field label.required::after {
  content: ' *';
  color: #ef4444;
}

.form-field :deep(.p-inputtext),
.form-field :deep(.p-dropdown),
.form-field :deep(.p-calendar),
.form-field :deep(.p-textarea) {
  width: 100%;
}

.form-field :deep(.p-inputtext:focus),
.form-field :deep(.p-dropdown:focus),
.form-field :deep(.p-calendar:focus-within) {
  border-color: var(--optiviera-blue);
  box-shadow: 0 0 0 0.2rem rgba(24, 154, 180, 0.2);
}

.form-field small.p-error {
  font-size: 0.85rem;
  margin-top: 0.25rem;
}

/* Form Actions */
.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  padding: 2rem 0 1rem;
  border-top: 1px solid var(--surface-border);
}

.submit-btn {
  background: linear-gradient(135deg, var(--optiviera-blue), var(--optiviera-green));
  border: none;
  font-weight: 600;
  padding: 0.75rem 2rem;
  box-shadow: 0 4px 12px rgba(24, 154, 180, 0.3);
  transition: all 0.3s;
}

.submit-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(24, 154, 180, 0.4);
}

/* Responsive */
@media (max-width: 768px) {
  .page-logo {
    height: 35px;
  }

  .page-title {
    font-size: 1.5rem;
  }

  .page-subtitle {
    font-size: 1rem;
  }

  .form-grid {
    grid-template-columns: 1fr;
  }

  .form-actions {
    flex-direction: column-reverse;
  }

  .form-actions button {
    width: 100%;
  }
}
</style>
