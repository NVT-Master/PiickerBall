<template>
  <div class="profile-page">
    <div class="page-header">
      <h1>Hồ sơ cá nhân</h1>
      <nav aria-label="breadcrumb">
        <ol class="breadcrumb">
          <li class="breadcrumb-item"><router-link to="/dashboard">Dashboard</router-link></li>
          <li class="breadcrumb-item active">Hồ sơ</li>
        </ol>
      </nav>
    </div>

    <div class="row g-4">
      <!-- Profile Card -->
      <div class="col-lg-4">
        <div class="card">
          <div class="card-body text-center">
            <div class="avatar avatar-lg mx-auto mb-3" style="width: 100px; height: 100px; font-size: 2.5rem;">
              {{ authStore.userInitials }}
            </div>
            <h4>{{ authStore.userFullName }}</h4>
            <p class="text-muted">{{ authStore.user?.email }}</p>
            <div class="d-flex justify-content-center gap-2 flex-wrap">
              <span 
                v-for="role in authStore.roles" 
                :key="role"
                class="badge bg-primary"
              >
                {{ role }}
              </span>
            </div>
          </div>
        </div>

        <!-- Stats Card -->
        <div class="card mt-4">
          <div class="card-header">
            <h6 class="mb-0">Thống kê</h6>
          </div>
          <div class="card-body">
            <div class="d-flex justify-content-between mb-3">
              <span class="text-muted">ELO Rating</span>
              <span class="fw-bold text-primary">{{ stats.eloRating || 1000 }}</span>
            </div>
            <div class="d-flex justify-content-between mb-3">
              <span class="text-muted">Số trận</span>
              <span class="fw-bold">{{ stats.totalMatches || 0 }}</span>
            </div>
            <div class="d-flex justify-content-between mb-3">
              <span class="text-muted">Thắng</span>
              <span class="fw-bold text-success">{{ stats.wins || 0 }}</span>
            </div>
            <div class="d-flex justify-content-between mb-3">
              <span class="text-muted">Thua</span>
              <span class="fw-bold text-danger">{{ stats.losses || 0 }}</span>
            </div>
            <div class="d-flex justify-content-between">
              <span class="text-muted">Tỷ lệ thắng</span>
              <span class="fw-bold">{{ winRate }}%</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Profile Form -->
      <div class="col-lg-8">
        <!-- Update Profile -->
        <div class="card mb-4">
          <div class="card-header">
            <h5 class="mb-0">Thông tin cá nhân</h5>
          </div>
          <div class="card-body">
            <form @submit.prevent="handleUpdateProfile">
              <div class="row g-3">
                <div class="col-md-6">
                  <label class="form-label">Họ và tên</label>
                  <input 
                    type="text" 
                    class="form-control"
                    v-model="profileForm.fullName"
                    required
                  >
                </div>
                <div class="col-md-6">
                  <label class="form-label">Email</label>
                  <input 
                    type="email" 
                    class="form-control"
                    :value="authStore.user?.email"
                    disabled
                  >
                </div>
                <div class="col-md-6">
                  <label class="form-label">Số điện thoại</label>
                  <input 
                    type="tel" 
                    class="form-control"
                    v-model="profileForm.phone"
                  >
                </div>
                <div class="col-md-6">
                  <label class="form-label">Ngày sinh</label>
                  <input 
                    type="date" 
                    class="form-control"
                    v-model="profileForm.dateOfBirth"
                  >
                </div>
                <div class="col-12">
                  <label class="form-label">Địa chỉ</label>
                  <input 
                    type="text" 
                    class="form-control"
                    v-model="profileForm.address"
                  >
                </div>
                <div class="col-12">
                  <label class="form-label">Giới thiệu</label>
                  <textarea 
                    class="form-control" 
                    rows="3"
                    v-model="profileForm.bio"
                    placeholder="Vài lời giới thiệu về bản thân..."
                  ></textarea>
                </div>
              </div>
              <div class="mt-4">
                <button type="submit" class="btn btn-primary" :disabled="isLoading">
                  <span v-if="isLoading" class="spinner-border spinner-border-sm me-2"></span>
                  Lưu thay đổi
                </button>
              </div>
            </form>
          </div>
        </div>

        <!-- Change Password -->
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Đổi mật khẩu</h5>
          </div>
          <div class="card-body">
            <form @submit.prevent="handleChangePassword">
              <div class="row g-3">
                <div class="col-md-12">
                  <label class="form-label">Mật khẩu hiện tại</label>
                  <input 
                    type="password" 
                    class="form-control"
                    v-model="passwordForm.currentPassword"
                    required
                  >
                </div>
                <div class="col-md-6">
                  <label class="form-label">Mật khẩu mới</label>
                  <input 
                    type="password" 
                    class="form-control"
                    v-model="passwordForm.newPassword"
                    required
                    minlength="6"
                  >
                </div>
                <div class="col-md-6">
                  <label class="form-label">Xác nhận mật khẩu mới</label>
                  <input 
                    type="password" 
                    class="form-control"
                    v-model="passwordForm.confirmPassword"
                    required
                  >
                </div>
              </div>
              <div class="mt-4">
                <button type="submit" class="btn btn-warning" :disabled="isLoading">
                  <span v-if="isLoading" class="spinner-border spinner-border-sm me-2"></span>
                  Đổi mật khẩu
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth.store'
import { membersApi } from '@/api'
import { useToast } from 'vue-toastification'

const authStore = useAuthStore()
const toast = useToast()

// State
const isLoading = ref(false)
const stats = ref({})

const profileForm = reactive({
  fullName: '',
  phone: '',
  dateOfBirth: '',
  address: '',
  bio: ''
})

const passwordForm = reactive({
  currentPassword: '',
  newPassword: '',
  confirmPassword: ''
})

// Computed
const winRate = computed(() => {
  const total = stats.value.totalMatches || 0
  const wins = stats.value.wins || 0
  if (total === 0) return 0
  return Math.round((wins / total) * 100)
})

// Methods
async function loadProfile() {
  try {
    const response = await membersApi.getById(authStore.user?.id || 'me')
    const data = response.data
    
    Object.assign(profileForm, {
      fullName: data.fullName || '',
      phone: data.phone || '',
      dateOfBirth: data.dateOfBirth?.split('T')[0] || '',
      address: data.address || '',
      bio: data.bio || ''
    })
    
    stats.value = {
      eloRating: data.eloRating,
      totalMatches: data.totalMatches,
      wins: data.wins,
      losses: data.losses
    }
  } catch (err) {
    console.error('Failed to load profile:', err)
  }
}

async function handleUpdateProfile() {
  isLoading.value = true
  
  try {
    await membersApi.updateMyProfile(profileForm)
    toast.success('Cập nhật thành công!')
    
    // Update auth store
    if (authStore.user) {
      authStore.user.fullName = profileForm.fullName
    }
  } catch (err) {
    toast.error(err.response?.data?.message || 'Không thể cập nhật')
  } finally {
    isLoading.value = false
  }
}

async function handleChangePassword() {
  if (passwordForm.newPassword !== passwordForm.confirmPassword) {
    toast.error('Mật khẩu xác nhận không khớp!')
    return
  }
  
  isLoading.value = true
  
  try {
    await authStore.changePassword({
      currentPassword: passwordForm.currentPassword,
      newPassword: passwordForm.newPassword
    })
    
    // Reset form
    passwordForm.currentPassword = ''
    passwordForm.newPassword = ''
    passwordForm.confirmPassword = ''
  } finally {
    isLoading.value = false
  }
}

// Lifecycle
onMounted(() => {
  loadProfile()
})
</script>
