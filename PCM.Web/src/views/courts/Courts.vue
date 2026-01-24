<template>
  <div class="courts-page">
    <div class="page-header d-flex justify-content-between align-items-center">
      <div>
        <h1>Quản lý sân</h1>
        <p class="text-muted mb-0">Quản lý danh sách sân Pickleball</p>
      </div>
      <button class="btn btn-primary" @click="openCreateModal">
        <i class="bi bi-plus-lg me-2"></i>Thêm sân mới
      </button>
    </div>

    <!-- Courts Grid -->
    <div class="row g-4">
      <div v-if="courtStore.isLoading" class="col-12">
        <LoadingSpinner />
      </div>

      <template v-else>
        <div 
          v-for="court in courtStore.courts" 
          :key="court.id"
          class="col-md-6 col-xl-4"
        >
          <div class="card court-card h-100">
            <div class="card-body">
              <div class="d-flex justify-content-between align-items-start mb-3">
                <div>
                  <h5 class="card-title mb-1">{{ court.name }}</h5>
                  <span 
                    class="badge"
                    :class="court.isActive ? 'bg-success' : 'bg-secondary'"
                  >
                    {{ court.isActive ? 'Đang hoạt động' : 'Tạm ngưng' }}
                  </span>
                </div>
                <div class="dropdown">
                  <button 
                    class="btn btn-link text-muted p-0"
                    data-bs-toggle="dropdown"
                  >
                    <i class="bi bi-three-dots-vertical"></i>
                  </button>
                  <ul class="dropdown-menu dropdown-menu-end">
                    <li>
                      <a class="dropdown-item" href="#" @click.prevent="openEditModal(court)">
                        <i class="bi bi-pencil me-2"></i>Chỉnh sửa
                      </a>
                    </li>
                    <li>
                      <a class="dropdown-item" href="#" @click.prevent="toggleStatus(court)">
                        <i class="bi me-2" :class="court.isActive ? 'bi-pause-circle' : 'bi-play-circle'"></i>
                        {{ court.isActive ? 'Tạm ngưng' : 'Kích hoạt' }}
                      </a>
                    </li>
                    <li><hr class="dropdown-divider"></li>
                    <li>
                      <a class="dropdown-item text-danger" href="#" @click.prevent="confirmDelete(court)">
                        <i class="bi bi-trash me-2"></i>Xóa
                      </a>
                    </li>
                  </ul>
                </div>
              </div>

              <div class="mb-3">
                <div class="d-flex align-items-center text-muted mb-2">
                  <i class="bi bi-geo-alt me-2"></i>
                  <span>{{ court.location || 'Chưa cập nhật địa chỉ' }}</span>
                </div>
                <div class="d-flex align-items-center text-muted">
                  <i class="bi bi-cash me-2"></i>
                  <span>{{ formatMoney(court.pricePerHour) }}/giờ</span>
                </div>
              </div>

              <p class="card-text text-muted small">
                {{ court.description || 'Không có mô tả' }}
              </p>
            </div>
            <div class="card-footer bg-transparent">
              <router-link 
                :to="`/bookings?courtId=${court.id}`"
                class="btn btn-outline-primary btn-sm w-100"
              >
                <i class="bi bi-calendar-check me-2"></i>Xem lịch đặt
              </router-link>
            </div>
          </div>
        </div>

        <div v-if="courtStore.courts.length === 0" class="col-12">
          <div class="text-center py-5">
            <i class="bi bi-grid-3x3 text-muted" style="font-size: 4rem;"></i>
            <h4 class="mt-3">Chưa có sân nào</h4>
            <p class="text-muted">Bắt đầu bằng cách thêm sân mới</p>
            <button class="btn btn-primary" @click="openCreateModal">
              <i class="bi bi-plus-lg me-2"></i>Thêm sân mới
            </button>
          </div>
        </div>
      </template>
    </div>

    <!-- Create/Edit Modal -->
    <BaseModal
      v-if="showModal"
      :title="editingCourt ? 'Cập nhật sân' : 'Thêm sân mới'"
      :loading="isSubmitting"
      @close="closeModal"
      @confirm="handleSubmit"
    >
      <form @submit.prevent="handleSubmit">
        <div class="mb-3">
          <label class="form-label">Tên sân <span class="text-danger">*</span></label>
          <input
            type="text"
            class="form-control"
            :class="{ 'is-invalid': errors.name }"
            v-model="form.name"
            placeholder="VD: Sân A, Sân 1..."
          />
          <div class="invalid-feedback">{{ errors.name }}</div>
        </div>

        <div class="mb-3">
          <label class="form-label">Địa chỉ</label>
          <input
            type="text"
            class="form-control"
            v-model="form.location"
            placeholder="Nhập địa chỉ sân"
          />
        </div>

        <div class="mb-3">
          <label class="form-label">Giá thuê (VNĐ/giờ) <span class="text-danger">*</span></label>
          <div class="input-group">
            <input
              type="number"
              class="form-control"
              :class="{ 'is-invalid': errors.pricePerHour }"
              v-model.number="form.pricePerHour"
              placeholder="VD: 100000"
              min="0"
            />
            <span class="input-group-text">₫/giờ</span>
            <div class="invalid-feedback">{{ errors.pricePerHour }}</div>
          </div>
        </div>

        <div class="mb-3">
          <label class="form-label">Mô tả</label>
          <textarea
            class="form-control"
            v-model="form.description"
            rows="3"
            placeholder="Mô tả thêm về sân..."
          ></textarea>
        </div>

        <div class="form-check form-switch">
          <input
            type="checkbox"
            class="form-check-input"
            id="isActive"
            v-model="form.isActive"
          />
          <label class="form-check-label" for="isActive">
            Sân đang hoạt động
          </label>
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
      <p>Bạn có chắc chắn muốn xóa sân <strong>{{ deletingCourt?.name }}</strong>?</p>
      <div class="alert alert-warning">
        <i class="bi bi-exclamation-triangle me-2"></i>
        Lưu ý: Các booking liên quan đến sân này sẽ bị ảnh hưởng!
      </div>
    </BaseModal>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useCourtStore } from '@/stores/court.store'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import BaseModal from '@/components/common/BaseModal.vue'

const courtStore = useCourtStore()

const showModal = ref(false)
const showDeleteModal = ref(false)
const editingCourt = ref(null)
const deletingCourt = ref(null)
const isSubmitting = ref(false)

const form = reactive({
  name: '',
  location: '',
  pricePerHour: 100000,
  description: '',
  isActive: true
})

const errors = reactive({
  name: '',
  pricePerHour: ''
})

onMounted(() => {
  courtStore.fetchCourts()
})

function formatMoney(amount) {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(amount || 0)
}

function openCreateModal() {
  editingCourt.value = null
  resetForm()
  showModal.value = true
}

function openEditModal(court) {
  editingCourt.value = court
  form.name = court.name
  form.location = court.location || ''
  form.pricePerHour = court.pricePerHour || 100000
  form.description = court.description || ''
  form.isActive = court.isActive !== false
  showModal.value = true
}

function closeModal() {
  showModal.value = false
  editingCourt.value = null
  resetForm()
}

function resetForm() {
  form.name = ''
  form.location = ''
  form.pricePerHour = 100000
  form.description = ''
  form.isActive = true
  errors.name = ''
  errors.pricePerHour = ''
}

function validate() {
  let isValid = true
  errors.name = ''
  errors.pricePerHour = ''

  if (!form.name.trim()) {
    errors.name = 'Vui lòng nhập tên sân'
    isValid = false
  }

  if (!form.pricePerHour || form.pricePerHour < 0) {
    errors.pricePerHour = 'Vui lòng nhập giá thuê hợp lệ'
    isValid = false
  }

  return isValid
}

async function handleSubmit() {
  if (!validate()) return

  isSubmitting.value = true
  try {
    const data = { ...form }

    if (editingCourt.value) {
      await courtStore.updateCourt(editingCourt.value.id, data)
    } else {
      await courtStore.createCourt(data)
    }

    closeModal()
  } finally {
    isSubmitting.value = false
  }
}

async function toggleStatus(court) {
  await courtStore.updateCourt(court.id, {
    ...court,
    isActive: !court.isActive
  })
}

function confirmDelete(court) {
  deletingCourt.value = court
  showDeleteModal.value = true
}

async function handleDelete() {
  if (!deletingCourt.value) return

  isSubmitting.value = true
  try {
    await courtStore.deleteCourt(deletingCourt.value.id)
    showDeleteModal.value = false
    deletingCourt.value = null
  } finally {
    isSubmitting.value = false
  }
}
</script>

<style scoped>
.court-card {
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.court-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
}
</style>
