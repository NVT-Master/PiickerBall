<template>
  <div class="auth-page">
    <div class="auth-container">
      <div class="auth-card">
        <!-- Logo & Title -->
        <div class="text-center mb-4">
          <div class="auth-logo mb-3">🏸</div>
          <h1 class="h3 fw-bold text-dark">Đăng ký tài khoản</h1>
          <p class="text-muted">Tham gia CLB Pickleball Vợt Thủ Phố Núi</p>
        </div>

        <!-- Register Form -->
        <form @submit.prevent="handleRegister">
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
              <label class="form-label">Số điện thoại</label>
              <input
                type="tel"
                class="form-control"
                :class="{ 'is-invalid': errors.phone }"
                v-model="form.phone"
                placeholder="0xxx xxx xxx"
              />
              <div class="invalid-feedback">{{ errors.phone }}</div>
            </div>
          </div>

          <div class="mb-3">
            <label class="form-label">Email <span class="text-danger">*</span></label>
            <input
              type="email"
              class="form-control"
              :class="{ 'is-invalid': errors.email }"
              v-model="form.email"
              placeholder="Nhập email của bạn"
            />
            <div class="invalid-feedback">{{ errors.email }}</div>
          </div>

          <div class="mb-3">
            <label class="form-label">Mật khẩu <span class="text-danger">*</span></label>
            <div class="input-group">
              <input
                :type="showPassword ? 'text' : 'password'"
                class="form-control"
                :class="{ 'is-invalid': errors.password }"
                v-model="form.password"
                placeholder="Nhập mật khẩu (ít nhất 6 ký tự)"
              />
              <button 
                type="button" 
                class="btn btn-outline-secondary"
                @click="showPassword = !showPassword"
              >
                <i class="bi" :class="showPassword ? 'bi-eye-slash' : 'bi-eye'"></i>
              </button>
              <div class="invalid-feedback">{{ errors.password }}</div>
            </div>
            <!-- Password strength indicator -->
            <div class="password-strength mt-2" v-if="form.password">
              <div class="progress" style="height: 4px;">
                <div 
                  class="progress-bar" 
                  :class="passwordStrengthClass"
                  :style="{ width: passwordStrength.percentage + '%' }"
                ></div>
              </div>
              <small :class="'text-' + passwordStrength.color">{{ passwordStrength.text }}</small>
            </div>
          </div>

          <div class="mb-4">
            <label class="form-label">Xác nhận mật khẩu <span class="text-danger">*</span></label>
            <input
              :type="showPassword ? 'text' : 'password'"
              class="form-control"
              :class="{ 'is-invalid': errors.confirmPassword }"
              v-model="form.confirmPassword"
              placeholder="Nhập lại mật khẩu"
            />
            <div class="invalid-feedback">{{ errors.confirmPassword }}</div>
          </div>

          <div class="form-check mb-4">
            <input 
              type="checkbox" 
              class="form-check-input" 
              :class="{ 'is-invalid': errors.agreeTerms }"
              id="agreeTerms" 
              v-model="form.agreeTerms"
            >
            <label class="form-check-label" for="agreeTerms">
              Tôi đồng ý với <a href="#" class="text-primary">Điều khoản sử dụng</a> 
              và <a href="#" class="text-primary">Chính sách bảo mật</a>
            </label>
            <div class="invalid-feedback">{{ errors.agreeTerms }}</div>
          </div>

          <button 
            type="submit" 
            class="btn btn-primary w-100 py-2"
            :disabled="authStore.isLoading"
          >
            <span v-if="authStore.isLoading" class="spinner-border spinner-border-sm me-2"></span>
            <span v-if="authStore.isLoading">Đang đăng ký...</span>
            <span v-else>Đăng ký</span>
          </button>
        </form>

        <!-- Divider -->
        <div class="divider my-4">
          <span>hoặc</span>
        </div>

        <!-- Login Link -->
        <p class="text-center mb-0">
          Đã có tài khoản?
          <router-link to="/login" class="text-primary fw-semibold text-decoration-none">
            Đăng nhập ngay
          </router-link>
        </p>
      </div>

      <!-- Footer -->
      <p class="text-center text-muted mt-4 small">
        © 2026 PCM - Vợt Thủ Phố Núi. All rights reserved.
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'
import { useAuthStore } from '@/stores/auth.store'

const authStore = useAuthStore()

const form = reactive({
  fullName: '',
  email: '',
  phone: '',
  password: '',
  confirmPassword: '',
  agreeTerms: false
})

const errors = reactive({
  fullName: '',
  email: '',
  phone: '',
  password: '',
  confirmPassword: '',
  agreeTerms: ''
})

const showPassword = ref(false)

// Password strength calculation
const passwordStrength = computed(() => {
  const password = form.password
  let strength = 0
  
  if (password.length >= 6) strength++
  if (password.length >= 10) strength++
  if (/[A-Z]/.test(password)) strength++
  if (/[0-9]/.test(password)) strength++
  if (/[^A-Za-z0-9]/.test(password)) strength++
  
  const levels = [
    { percentage: 20, text: 'Rất yếu', color: 'danger' },
    { percentage: 40, text: 'Yếu', color: 'warning' },
    { percentage: 60, text: 'Trung bình', color: 'info' },
    { percentage: 80, text: 'Mạnh', color: 'primary' },
    { percentage: 100, text: 'Rất mạnh', color: 'success' }
  ]
  
  return levels[Math.min(strength, 4)]
})

const passwordStrengthClass = computed(() => `bg-${passwordStrength.value.color}`)

function validate() {
  let isValid = true
  
  // Reset errors
  Object.keys(errors).forEach(key => errors[key] = '')

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

  if (form.phone && !/^(0[0-9]{9,10})$/.test(form.phone.replace(/\s/g, ''))) {
    errors.phone = 'Số điện thoại không hợp lệ'
    isValid = false
  }

  if (!form.password) {
    errors.password = 'Vui lòng nhập mật khẩu'
    isValid = false
  } else if (form.password.length < 6) {
    errors.password = 'Mật khẩu phải có ít nhất 6 ký tự'
    isValid = false
  }

  if (!form.confirmPassword) {
    errors.confirmPassword = 'Vui lòng xác nhận mật khẩu'
    isValid = false
  } else if (form.password !== form.confirmPassword) {
    errors.confirmPassword = 'Mật khẩu xác nhận không khớp'
    isValid = false
  }

  if (!form.agreeTerms) {
    errors.agreeTerms = 'Bạn cần đồng ý với điều khoản sử dụng'
    isValid = false
  }

  return isValid
}

async function handleRegister() {
  if (!validate()) return

  await authStore.register({
    fullName: form.fullName,
    email: form.email,
    phone: form.phone,
    password: form.password
  })
}
</script>

<style scoped>
.auth-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  padding: 1rem;
}

.auth-container {
  width: 100%;
  max-width: 500px;
}

.auth-card {
  background: white;
  border-radius: 1rem;
  padding: 2rem;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
}

.auth-logo {
  font-size: 4rem;
  line-height: 1;
}

.divider {
  display: flex;
  align-items: center;
  text-align: center;
  color: #94a3b8;
  font-size: 0.875rem;
}

.divider::before,
.divider::after {
  content: '';
  flex: 1;
  border-bottom: 1px solid #e2e8f0;
}

.divider span {
  padding: 0 1rem;
}

.btn-primary {
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  border: none;
}

.btn-primary:hover {
  background: linear-gradient(135deg, #059669 0%, #047857 100%);
}

.password-strength {
  font-size: 0.75rem;
}
</style>
