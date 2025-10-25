<template>
  <div class="users-page">
    <div class="page-header">
      <div class="header-left">
        <NuxtLink to="/dashboard">
          <img src="/logo.png" alt="Optiviera" class="page-logo" />
        </NuxtLink>
        <div class="header-text">
          <h1 class="page-title">Users</h1>
          <p class="page-subtitle">Manage team members and permissions</p>
        </div>
      </div>
      <Button
        label="Add New User"
        icon="pi pi-user-plus"
        @click="openAddDialog"
        class="add-btn"
      />
    </div>

    <!-- Filters Card -->
    <Card class="filters-card">
      <template #content>
        <div class="filters-row">
          <div class="filter-group">
            <label>Search</label>
            <InputText
              v-model="filters.search"
              placeholder="Search users..."
              class="filter-input"
            >
              <template #prefix>
                <i class="pi pi-search"></i>
              </template>
            </InputText>
          </div>

          <div class="filter-group">
            <label>Role</label>
            <Dropdown
              v-model="filters.role"
              :options="roleOptions"
              placeholder="All Roles"
              class="filter-input"
              :showClear="true"
            />
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
        </div>
      </template>
    </Card>

    <!-- Users Table -->
    <Card class="table-card">
      <template #content>
        <DataTable
          :value="users"
          :loading="loading"
          stripedRows
          :paginator="true"
          :rows="10"
          :rowsPerPageOptions="[5, 10, 20, 50]"
          paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
          currentPageReportTemplate="Showing {first} to {last} of {totalRecords} users"
          :globalFilterFields="['firstName', 'lastName', 'email']"
          class="users-table"
        >
          <template #empty>
            <div class="empty-state">
              <i class="pi pi-users empty-icon"></i>
              <p>No users found</p>
            </div>
          </template>

          <Column field="fullName" header="Name" :sortable="true">
            <template #body="{ data }">
              <div class="user-cell">
                <div class="user-avatar">
                  {{ getInitials(data.firstName, data.lastName) }}
                </div>
                <div class="user-info">
                  <strong>{{ data.fullName }}</strong>
                  <small>{{ data.email }}</small>
                </div>
              </div>
            </template>
          </Column>

          <Column field="roles" header="Roles" :sortable="true" style="width: 250px">
            <template #body="{ data }">
              <div class="roles-cell">
                <Tag
                  v-for="role in data.roles"
                  :key="role"
                  :value="role"
                  :severity="getRoleSeverity(role)"
                  class="role-tag"
                />
              </div>
            </template>
          </Column>

          <Column field="createdAt" header="Joined" :sortable="true" style="width: 180px">
            <template #body="{ data }">
              <div class="date-cell">
                <i class="pi pi-calendar"></i>
                {{ formatDate(data.createdAt) }}
              </div>
            </template>
          </Column>

          <Column field="status" header="Status" :sortable="true" style="width: 120px">
            <template #body="{ data }">
              <Tag
                :value="data.isActive ? 'Active' : 'Inactive'"
                :severity="data.isActive ? 'success' : 'danger'"
              />
            </template>
          </Column>

          <Column style="width: 150px">
            <template #body="{ data }">
              <div class="action-buttons">
                <Button
                  icon="pi pi-pencil"
                  text
                  rounded
                  severity="info"
                  @click="editUser(data)"
                  v-tooltip.top="'Edit User'"
                />
                <Button
                  icon="pi pi-trash"
                  text
                  rounded
                  severity="danger"
                  @click="confirmDelete(data)"
                  v-tooltip.top="'Delete User'"
                  :disabled="data.id === authStore.user?.id"
                />
              </div>
            </template>
          </Column>
        </DataTable>
      </template>
    </Card>

    <!-- Add/Edit User Dialog -->
    <Dialog
      v-model:visible="showAddDialog"
      :header="editingUser ? 'Edit User' : 'Add New User'"
      :modal="true"
      :closable="true"
      class="user-dialog"
    >
      <div class="dialog-content">
        <div class="form-grid">
          <div class="form-field">
            <label for="firstName" class="required">First Name</label>
            <InputText
              id="firstName"
              v-model="userForm.firstName"
              placeholder="First name"
              :class="{ 'p-invalid': formSubmitted && !userForm.firstName }"
            />
            <small v-if="formSubmitted && !userForm.firstName" class="p-error">Required</small>
          </div>

          <div class="form-field">
            <label for="lastName" class="required">Last Name</label>
            <InputText
              id="lastName"
              v-model="userForm.lastName"
              placeholder="Last name"
              :class="{ 'p-invalid': formSubmitted && !userForm.lastName }"
            />
            <small v-if="formSubmitted && !userForm.lastName" class="p-error">Required</small>
          </div>

          <div class="form-field full-width">
            <label for="email" class="required">Email</label>
            <InputText
              id="email"
              v-model="userForm.email"
              placeholder="email@example.com"
              type="email"
              :class="{ 'p-invalid': formSubmitted && !userForm.email }"
            />
            <small v-if="formSubmitted && !userForm.email" class="p-error">Required</small>
          </div>

          <div v-if="!editingUser" class="form-field full-width">
            <label for="password" class="required">Password</label>
            <Password
              id="password"
              v-model="userForm.password"
              placeholder="Enter password"
              toggleMask
              :class="{ 'p-invalid': formSubmitted && !userForm.password }"
            />
            <small v-if="formSubmitted && !userForm.password" class="p-error">Required</small>
          </div>

          <div class="form-field full-width">
            <label class="required">Roles</label>
            <div class="roles-checkboxes">
              <div v-for="role in availableRoles" :key="role" class="role-checkbox">
                <Checkbox
                  v-model="userForm.roles"
                  :inputId="`role-${role}`"
                  :value="role"
                />
                <label :for="`role-${role}`">{{ role }}</label>
              </div>
            </div>
            <small v-if="formSubmitted && userForm.roles.length === 0" class="p-error">
              At least one role is required
            </small>
          </div>

          <div v-if="editingUser" class="form-field full-width">
            <div class="checkbox-wrapper">
              <Checkbox v-model="userForm.isActive" :binary="true" inputId="isActive" />
              <label for="isActive">Active user</label>
            </div>
          </div>
        </div>
      </div>

      <template #footer>
        <Button
          label="Cancel"
          severity="secondary"
          outlined
          @click="closeDialog"
        />
        <Button
          :label="editingUser ? 'Update' : 'Create'"
          @click="handleSubmit"
          :loading="saving"
          class="submit-btn"
        />
      </template>
    </Dialog>

    <!-- Delete Confirmation Dialog -->
    <Dialog
      v-model:visible="showDeleteDialog"
      header="Confirm Delete"
      :modal="true"
      class="delete-dialog"
    >
      <div class="dialog-content">
        <i class="pi pi-exclamation-triangle warning-icon"></i>
        <p>Are you sure you want to delete <strong>{{ userToDelete?.fullName }}</strong>?</p>
        <p class="warning-text">This action cannot be undone.</p>
      </div>

      <template #footer>
        <Button
          label="Cancel"
          severity="secondary"
          outlined
          @click="showDeleteDialog = false"
        />
        <Button
          label="Delete"
          severity="danger"
          @click="handleDelete"
          :loading="deleting"
        />
      </template>
    </Dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '~/stores/auth'

definePageMeta({
  layout: 'dashboard',
  middleware: ['auth', 'admin']
})

const authStore = useAuthStore()
const loading = ref(false)
const saving = ref(false)
const deleting = ref(false)
const formSubmitted = ref(false)

const users = ref<any[]>([])
const showAddDialog = ref(false)
const showDeleteDialog = ref(false)
const editingUser = ref<any>(null)
const userToDelete = ref<any>(null)

const filters = ref({
  search: '',
  role: null,
  status: null
})

const roleOptions = ['Admin', 'Manager', 'Technician', 'Support']
const statusOptions = [
  { label: 'Active', value: true },
  { label: 'Inactive', value: false }
]

const availableRoles = ['Admin', 'Manager', 'Technician', 'Support']

const userForm = ref({
  firstName: '',
  lastName: '',
  email: '',
  password: '',
  roles: [] as string[],
  isActive: true
})

const getInitials = (firstName: string, lastName: string): string => {
  return `${firstName?.[0] || ''}${lastName?.[0] || ''}`.toUpperCase()
}

const getRoleSeverity = (role: string): string => {
  const severities: { [key: string]: string } = {
    'Admin': 'danger',
    'Manager': 'warn',
    'Technician': 'info',
    'Support': 'success'
  }
  return severities[role] || 'info'
}

const formatDate = (dateStr: string): string => {
  const date = new Date(dateStr)
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

const openAddDialog = () => {
  showAddDialog.value = true
}

const editUser = (user: any) => {
  editingUser.value = user
  userForm.value = {
    firstName: user.firstName,
    lastName: user.lastName,
    email: user.email,
    password: '',
    roles: [...user.roles],
    isActive: user.isActive ?? true
  }
  showAddDialog.value = true
}

const confirmDelete = (user: any) => {
  userToDelete.value = user
  showDeleteDialog.value = true
}

const closeDialog = () => {
  showAddDialog.value = false
  editingUser.value = null
  formSubmitted.value = false
  userForm.value = {
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    roles: [],
    isActive: true
  }
}

const handleSubmit = async () => {
  formSubmitted.value = true

  // Validate
  if (
    !userForm.value.firstName ||
    !userForm.value.lastName ||
    !userForm.value.email ||
    (!editingUser.value && !userForm.value.password) ||
    userForm.value.roles.length === 0
  ) {
    return
  }

  saving.value = true

  try {
    // TODO: Implement API call
    if (editingUser.value) {
      // Update existing user
      console.log('Updating user:', userForm.value)
    } else {
      // Create new user
      console.log('Creating user:', userForm.value)
    }

    closeDialog()
    await fetchUsers()
  } catch (error) {
    console.error('Failed to save user:', error)
  } finally {
    saving.value = false
  }
}

const handleDelete = async () => {
  deleting.value = true

  try {
    // TODO: Implement API call to delete user
    console.log('Deleting user:', userToDelete.value)

    showDeleteDialog.value = false
    userToDelete.value = null
    await fetchUsers()
  } catch (error) {
    console.error('Failed to delete user:', error)
  } finally {
    deleting.value = false
  }
}

const fetchUsers = async () => {
  loading.value = true

  try {
    // TODO: Implement API call to fetch users
    // const config = useRuntimeConfig()
    // users.value = await $fetch(`${config.public.apiBase}/users`, {
    //   headers: { Authorization: `Bearer ${authStore.token}` }
    // })

    // Mock data for now
    users.value = [
      {
        id: authStore.user?.id,
        firstName: authStore.user?.firstName,
        lastName: authStore.user?.lastName,
        fullName: authStore.user?.fullName,
        email: authStore.user?.email,
        roles: authStore.user?.roles,
        createdAt: authStore.user?.createdAt,
        isActive: true
      }
    ]
  } catch (error) {
    console.error('Failed to fetch users:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchUsers()
})
</script>

<style scoped>
.users-page {
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

.add-btn {
  background: linear-gradient(135deg, var(--optiviera-blue), var(--optiviera-green));
  border: none;
  font-weight: 600;
  padding: 0.75rem 1.5rem;
  box-shadow: 0 4px 12px rgba(24, 154, 180, 0.3);
  transition: all 0.3s;
}

.add-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(24, 154, 180, 0.4);
}

/* Filters */
.filters-card {
  margin-bottom: 2rem;
  border-left: 4px solid var(--optiviera-blue);
}

.filters-row {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
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

/* Table */
.table-card {
  border-left: 4px solid var(--optiviera-green);
}

.users-table {
  font-size: 0.95rem;
}

.users-table :deep(.p-datatable-thead) {
  background: linear-gradient(135deg, var(--optiviera-navy), var(--optiviera-blue));
}

.users-table :deep(.p-datatable-thead > tr > th) {
  color: white;
  font-weight: 600;
  padding: 1rem;
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

.user-cell {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.user-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: linear-gradient(135deg, var(--optiviera-blue), var(--optiviera-green));
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 0.9rem;
}

.user-info {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.user-info strong {
  color: var(--optiviera-navy);
}

.user-info small {
  color: var(--text-color-secondary);
  font-size: 0.85rem;
}

.roles-cell {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.role-tag {
  font-size: 0.85rem;
}

.date-cell {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--text-color);
}

.date-cell i {
  color: var(--optiviera-blue);
}

.action-buttons {
  display: flex;
  gap: 0.25rem;
}

/* Dialog */
.user-dialog,
.delete-dialog {
  width: 500px;
  max-width: 90vw;
}

.dialog-content {
  padding: 1.5rem 0;
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
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
.form-field :deep(.p-password) {
  width: 100%;
}

.roles-checkboxes {
  display: flex;
  flex-wrap: wrap;
  gap: 1rem;
  padding: 0.75rem;
  background: var(--surface-50);
  border-radius: 8px;
}

.role-checkbox {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.checkbox-wrapper {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.submit-btn {
  background: linear-gradient(135deg, var(--optiviera-blue), var(--optiviera-green));
  border: none;
  font-weight: 600;
}

/* Delete Dialog */
.delete-dialog .dialog-content {
  text-align: center;
}

.warning-icon {
  font-size: 4rem;
  color: #f59e0b;
  margin-bottom: 1rem;
}

.warning-text {
  color: var(--text-color-secondary);
  font-size: 0.9rem;
  margin-top: 0.5rem;
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

  .add-btn {
    width: 100%;
  }

  .filters-row {
    grid-template-columns: 1fr;
  }

  .form-grid {
    grid-template-columns: 1fr;
  }

  .user-dialog,
  .delete-dialog {
    width: 95vw;
  }
}
</style>
