<template>
  <div class="members-page">
    <div class="page-header d-flex justify-content-between align-items-center">
      <div>
        <h1>Thành viên</h1>
        <p class="text-muted mb-0">Danh sách thành viên CLB</p>
      </div>
      <button 
        v-if="authStore.isAdmin"
        class="btn btn-primary"
        @click="openCreateModal"
      >
        <i class="bi bi-plus-lg me-2"></i>Thêm thành viên
      </button>
    </div>

    <!-- Data Table -->
    <div class="card">
      <div class="card-body">
        <DataTable
          :columns="columns"
          :data="memberStore.members"
          :loading="memberStore.isLoading"
          :current-page="memberStore.pagination.page"
          :total-items="memberStore.pagination.totalItems"
          :page-size="memberStore.pagination.pageSize"
          @search="handleSearch"
          @page-change="handlePageChange"
          @page-size-change="handlePageSizeChange"
        >
          <!-- Custom cell: Avatar + Name -->
          <template #cell-fullName="{ row }">
            <div class="d-flex align-items-center">
              <div class="avatar avatar-sm me-2">
                {{ getInitials(row.fullName) }}
              </div>
              <div>
                <span class="fw-medium">{{ row.fullName }}</span>
                <br>
                <small class="text-muted">{{ row.email }}</small>
              </div>
            </div>
          </template>

          <!-- Custom cell: Stats -->
          <template #cell-stats="{ row }">
            <span class="text-success">{{ row.wins || 0 }}W</span>
            <span class="mx-1">-</span>
            <span class="text-danger">{{ row.losses || 0 }}L</span>
          </template>

          <!-- Custom cell: Ranking -->
          <template #cell-rankingPoints="{ row }">
            <span class="badge bg-primary">{{ row.rankingPoints || 0 }} pts</span>
          </template>

          <!-- Custom cell: Status -->
          <template #cell-isActive="{ row }">
            <span 
              class="badge"
              :class="row.isActive !== false ? 'bg-success' : 'bg-secondary'"
            >
              {{ row.isActive !== false ? 'Hoạt động' : 'Ngưng' }}
            </span>
          </template>

          <!-- Row Actions -->
          <template #rowActions="{ row }">
            <div class="btn-group btn-group-sm">
              <router-link 
                :to="`/members/${row.id}`"
                class="btn btn-outline-primary"
                title="Xem chi tiết"
              >
                <i class="bi bi-eye"></i>
              </router-link>
              <button 
                v-if="authStore.isAdmin || row.id === authStore.user?.id"
                class="btn btn-outline-secondary"
                @click="openEditModal(row)"
                title="Sửa"
              >
                <i class="bi bi-pencil"></i>
              </button>
              <button 
                v-if="authStore.isAdmin && row.id !== authStore.user?.id"
                class="btn btn-outline-danger"
                @click="confirmDelete(row)"
                title="Xóa"
              >
                <i class="bi bi-trash"></i>
              </button>
            </div>
          </template>
        </DataTable>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <BaseModal
      v-if="showModal"
      :title="editingMember ? 'Cập nhật thành viên' : 'Thêm thành viên mới'"
      :loading="isSubmitting"
      @close="closeModal"
      @confirm="handleSubmit"
      size="lg"
    >
      <form @submit.prevent="handleSubmit">
        <div class="row">
          <div class="col-md-6 mb-3">
            <label class="form-label">Họ tên <span class="text-danger">*</span></label>
            <input
              type="text"
              class="form-control"
              :class="{ 'is-invalid': errors.fullName }"
              v-model="form.fullName"
              placeholder="Nhập họ tên"
            />
            <div class="invalid-feedback">{{ errors.fullName }}</div>
          </div>

          <div class="col-md-6 mb-3">
            <label class="form-label">Email <span class="text-danger">*</span></label>
            <input
              type="email"
              class="form-control"
              :class="{ 'is-invalid': errors.email }"
              v-model="form.email"
              placeholder="Nhập email"
              :disabled="!!editingMember"
            />
            <div class="invalid-feedback">{{ errors.email }}</div>
          </div>

          <div class="col-md-6 mb-3">
            <label class="form-label">Số điện thoại</label>
            <input
              type="tel"
              class="form-control"
              v-model="form.phone"
              placeholder="0xxx xxx xxx"
            />
          </div>

          <div class="col-md-6 mb-3">
            <label class="form-label">Ngày sinh</label>
            <input
              type="date"
              class="form-control"
              v-model="form.dateOfBirth"
            />
          </div>

          <div class="col-md-6 mb-3" v-if="!editingMember">
            <label class="form-label">Mật khẩu <span class="text-danger">*</span></label>
            <input
              type="password"
              class="form-control"
              :class="{ 'is-invalid': errors.password }"
              v-model="form.password"
              placeholder="Nhập mật khẩu"
            />
            <div class="invalid-feedback">{{ errors.password }}</div>
          </div>

          <div class="col-md-6 mb-3" v-if="authStore.isAdmin">
            <label class="form-label">Trạng thái</label>
            <select class="form-select" v-model="form.isActive">
              <option :value="true">Hoạt động</option>
              <option :value="false">Ngưng hoạt động</option>
            </select>
          </div>

          <div class="col-12 mb-3">
            <label class="form-label">Địa chỉ</label>
            <textarea
              class="form-control"
              v-model="form.address"
              rows="2"
              placeholder="Nhập địa chỉ"
            ></textarea>
          </div>
        </div>
      </form>
    </BaseModal>

    <!-- Delete Confirmation Modal -->
    <BaseModal
      v-if="showDeleteModal"
      title="Xác nhận xóa"
      :loading="isSubmitting"
      @close="showDeleteModal = false"
      @confirm="handleDelete"
      confirm-text="Xóa"
    >
      <p>Bạn có chắc chắn muốn xóa thành viên <strong>{{ deletingMember?.fullName }}</strong>?</p>
      <p class="text-danger mb-0">
        <i class="bi bi-exclamation-triangle me-1"></i>
        Hành động này không thể hoàn tác!
      </p>
    </BaseModal>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth.store'
import { useMemberStore } from '@/stores/member.store'
import DataTable from '@/components/common/DataTable.vue'
import BaseModal from '@/components/common/BaseModal.vue'

const authStore = useAuthStore()
const memberStore = useMemberStore()

// Table columns
const columns = [
  { key: 'fullName', label: 'Thành viên', sortable: true },
  { key: 'phone', label: 'SĐT' },
  { key: 'stats', label: 'Thắng/Thua' },
  { key: 'rankingPoints', label: 'Điểm xếp hạng', sortable: true },
  { key: 'isActive', label: 'Trạng thái' }
]

// Modal state
const showModal = ref(false)
const showDeleteModal = ref(false)
const editingMember = ref(null)
const deletingMember = ref(null)
const isSubmitting = ref(false)

// Form
const form = reactive({
  fullName: '',
  email: '',
  phone: '',
  dateOfBirth: '',
  address: '',
  password: '',
  isActive: true
})

const errors = reactive({
  fullName: '',
  email: '',
  password: ''
})

onMounted(() => {
  memberStore.fetchMembers()
})

function getInitials(name) {
  if (!name) return '?'
  return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
}

function handleSearch(query) {
  memberStore.setPage(1)
  memberStore.fetchMembers({ search: query })
}

function handlePageChange(page) {
  memberStore.setPage(page)
  memberStore.fetchMembers()
}

function handlePageSizeChange(size) {
  memberStore.setPageSize(size)
  memberStore.fetchMembers()
}

function openCreateModal() {
  editingMember.value = null
  resetForm()
  showModal.value = true
}

function openEditModal(member) {
  editingMember.value = member
  form.fullName = member.fullName
  form.email = member.email
  form.phone = member.phone || ''
  form.dateOfBirth = member.dateOfBirth?.split('T')[0] || ''
  form.address = member.address || ''
  form.isActive = member.isActive !== false
  showModal.value = true
}

function closeModal() {
  showModal.value = false
  editingMember.value = null
  resetForm()
}

function resetForm() {
  form.fullName = ''
  form.email = ''
  form.phone = ''
  form.dateOfBirth = ''
  form.address = ''
  form.password = ''
  form.isActive = true
  errors.fullName = ''
  errors.email = ''
  errors.password = ''
}

function validate() {
  let isValid = true
  errors.fullName = ''
  errors.email = ''
  errors.password = ''

  if (!form.fullName.trim()) {
    errors.fullName = 'Vui lòng nhập họ tên'
    isValid = false
  }

  if (!form.email) {
    errors.email = 'Vui lòng nhập email'
    isValid = false
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) {
    errors.email = 'Email không hợp lệ'
    isValid = false
  }

  if (!editingMember.value && !form.password) {
    errors.password = 'Vui lòng nhập mật khẩu'
    isValid = false
  }

  return isValid
}

async function handleSubmit() {
  if (!validate()) return

  isSubmitting.value = true
  try {
    const data = {
      fullName: form.fullName,
      email: form.email,
      phone: form.phone,
      dateOfBirth: form.dateOfBirth || null,
      address: form.address,
      isActive: form.isActive
    }

    if (!editingMember.value) {
      data.password = form.password
      await memberStore.createMember(data)
    } else {
      await memberStore.updateMember(editingMember.value.id, data)
    }

    closeModal()
  } finally {
    isSubmitting.value = false
  }
}

function confirmDelete(member) {
  deletingMember.value = member
  showDeleteModal.value = true
}

async function handleDelete() {
  if (!deletingMember.value) return

  isSubmitting.value = true
  try {
    await memberStore.deleteMember(deletingMember.value.id)
    showDeleteModal.value = false
    deletingMember.value = null
  } finally {
    isSubmitting.value = false
  }
}
</script>
