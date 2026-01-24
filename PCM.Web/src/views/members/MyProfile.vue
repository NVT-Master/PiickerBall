<template>
  <div class="my-profile">
    <div class="page-header">
      <h1>Thông tin cá nhân</h1>
      <p class="text-muted mb-0">Quản lý thông tin tài khoản của bạn</p>
    </div>

    <div class="row g-4">
      <div class="col-lg-4">
        <!-- Profile Summary Card -->
        <div class="card">
          <div class="card-body text-center">
            <div class="avatar avatar-lg mx-auto mb-3" style="width: 100px; height: 100px; font-size: 2.5rem;">
              {{ authStore.userInitials }}
            </div>
            <h4 class="mb-1">{{ authStore.userFullName }}</h4>
            <p class="text-muted mb-3">{{ authStore.user?.email }}</p>
            
            <div class="d-flex justify-content-center gap-2">
              <span 
                v-for="role in authStore.roles" 
                :key="role"
                class="badge"
                :class="getRoleBadgeClass(role)"
              >
                {{ role }}
              </span>
            </div>
          </div>
        </div>

        <!-- Stats -->
        <div class="card mt-4">
          <div class="card-header">
            <i class="bi bi-bar-chart me-2"></i>Thống kê của bạn
          </div>
          <div class="card-body">
            <div class="row text-center g-3">
              <div class="col-4">
                <div class="fs-4 fw-bold text-primary">{{ stats.rankingPoints }}</div>
                <small class="text-muted">Điểm XH</small>
              </div>
              <div class="col-4">
                <div class="fs-4 fw-bold text-success">{{ stats.wins }}</div>
                <small class="text-muted">Thắng</small>
              </div>
              <div class="col-4">
                <div class="fs-4 fw-bold text-danger">{{ stats.losses }}</div>
                <small class="text-muted">Thua</small>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-8">
        <!-- Edit Profile Form -->
        <div class="card">
          <div class="card-header">
            <i class="bi bi-pencil me-2"></i>Cập nhật thông tin
          </div>
          <div class="card-body">
            <form @submit.prevent="handleUpdate">
              <div class="row g-3">
                <div class="col-md-6">
                  <label class="form-label">Họ tên <span class="text-danger">*</span></label>
                  <input
                    type="text"
                    class="form-control"
                    :class="{ 'is-invalid': errors.fullName }"
                    v-model="form.fullName"
                  />
                  <div class="invalid-feedback">{{ errors.fullName }}</div>
                </div>

                <div class="col-md-6">
                  <label class="form-label">Email</label>
                  <input
                    type="email"
                    class="form-control"
                    :value="authStore.user?.email"
                    disabled
                  />
                  <small class="text-muted">Email không thể thay đổi</small>
                </div>

                <div class="col-md-6">
                  <label class="form-label">Số điện thoại</label>
                  <input
                    type="tel"
                    class="form-control"
                    v-model="form.phone"
                    placeholder="0xxx xxx xxx"
                  />
                </div>

                <div class="col-md-6">
                  <label class="form-label">Ngày sinh</label>
                  <input
                    type="date"
                    class="form-control"
                    v-model="form.dateOfBirth"
                  />
                </div>

                <div class="col-12">
                  <label class="form-label">Địa chỉ</label>
                  <textarea
                    class="form-control"
                    v-model="form.address"
                    rows="2"
                    placeholder="Nhập địa chỉ"
                  ></textarea>
                </div>
              </div>

              <div class="mt-4">
                <button type="submit" class="btn btn-primary" :disabled="isLoading">
                  <span v-if="isLoading" class="spinner-border spinner-border-sm me-2"></span>
                  Lưu thay đổi
                </button>
                <button type="button" class="btn btn-outline-secondary ms-2" @click="resetForm">
                  Hủy
                </button>
              </div>
            </form>
          </div>
        </div>

        <!-- Security Section -->
        <div class="card mt-4">
          <div class="card-header">
            <i class="bi bi-shield-lock me-2"></i>Bảo mật
          </div>
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-center">
              <div>
                <h6 class="mb-1">Đổi mật khẩu</h6>
                <p class="text-muted mb-0 small">Thay đổi mật khẩu đăng nhập của bạn</p>
              </div>
              <button class="btn btn-outline-primary" @click="showChangePassword = true">
                Đổi mật khẩu
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Change Password Modal -->
    <ChangePasswordModal 
      v-if="showChangePassword"
      @close="showChangePassword = false"
    />
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth.store'
import { membersApi } from '@/api'
import { useToast } from 'vue-toastification'
import ChangePasswordModal from '@/components/common/ChangePasswordModal.vue'

const authStore = useAuthStore()
const toast = useToast()

const isLoading = ref(false)
const showChangePassword = ref(false)

const form = reactive({
  fullName: '',
  phone: '',
  dateOfBirth: '',
  address: ''
})

const errors = reactive({
  fullName: ''
})

const stats = reactive({
  rankingPoints: 0,
  wins: 0,
  losses: 0
})

onMounted(() => {
  // Load current user data
  if (authStore.user) {
    form.fullName = authStore.user.fullName || ''
    form.phone = authStore.user.phone || ''
    form.dateOfBirth = authStore.user.dateOfBirth?.split('T')[0] || ''
    form.address = authStore.user.address || ''
    
    stats.rankingPoints = authStore.user.rankingPoints || 0
    stats.wins = authStore.user.wins || 0
    stats.losses = authStore.user.losses || 0
  }
})

function resetForm() {
  if (authStore.user) {
    form.fullName = authStore.user.fullName || ''
    form.phone = authStore.user.phone || ''
    form.dateOfBirth = authStore.user.dateOfBirth?.split('T')[0] || ''
    form.address = authStore.user.address || ''
  }
  errors.fullName = ''
}

function validate() {
  errors.fullName = ''
  
  if (!form.fullName.trim()) {
    errors.fullName = 'Vui lòng nhập họ tên'
    return false
  }
  
  return true
}

async function handleUpdate() {
  if (!validate()) return

  isLoading.value = true
  try {
    await membersApi.updateMyProfile({
      fullName: form.fullName,
      phone: form.phone,
      dateOfBirth: form.dateOfBirth || null,
      address: form.address
    })

    // Update local state
    if (authStore.user) {
      authStore.user.fullName = form.fullName
      authStore.user.phone = form.phone
      authStore.user.dateOfBirth = form.dateOfBirth
      authStore.user.address = form.address
      
      // Update localStorage
      localStorage.setItem('pcm_user', JSON.stringify(authStore.user))
    }

    toast.success('Cập nhật thành công!')
  } catch (error) {
    toast.error(error.response?.data?.message || 'Cập nhật thất bại')
  } finally {
    isLoading.value = false
  }
}

function getRoleBadgeClass(role) {
  const classes = {
    Admin: 'bg-danger',
    Treasurer: 'bg-warning text-dark',
    Referee: 'bg-info',
    Member: 'bg-secondary'
  }
  return classes[role] || 'bg-secondary'
}
</script>
