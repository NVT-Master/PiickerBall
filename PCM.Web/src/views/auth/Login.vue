<template>
  <div class="auth-page">
    <div class="auth-container">
      <div class="auth-card">
        <!-- Logo & Title -->
        <div class="text-center mb-4">
          <div class="auth-logo mb-3">🏸</div>
          <h1 class="h3 fw-bold text-dark">Đăng nhập</h1>
          <p class="text-muted">Chào mừng đến với PCM - Vợt Thủ Phố Núi</p>
        </div>

        <!-- Session Expired Message -->
        <div class="alert alert-warning d-flex align-items-center" v-if="sessionExpired">
          <i class="bi bi-exclamation-triangle me-2"></i>
          Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.
        </div>

        <!-- Login Form -->
        <form @submit.prevent="handleLogin">
          <div class="mb-3">
            <label class="form-label">Email <span class="text-danger">*</span></label>
            <div class="input-group">
              <span class="input-group-text">
                <i class="bi bi-envelope"></i>
              </span>
              <input
                type="email"
                class="form-control"
                :class="{ 'is-invalid': errors.email }"
                v-model="form.email"
                placeholder="Nhập email của bạn"
                @blur="validateEmail"
                autofocus
              />
              <div class="invalid-feedback">{{ errors.email }}</div>
            </div>
          </div>

          <div class="mb-3">
            <label class="form-label">Mật khẩu <span class="text-danger">*</span></label>
            <div class="input-group">
              <span class="input-group-text">
                <i class="bi bi-lock"></i>
              </span>
              <input
                :type="showPassword ? 'text' : 'password'"
                class="form-control"
                :class="{ 'is-invalid': errors.password }"
                v-model="form.password"
                placeholder="Nhập mật khẩu"
                @blur="validatePassword"
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
          </div>

          <div class="d-flex justify-content-between align-items-center mb-4">
            <div class="form-check">
              <input type="checkbox" class="form-check-input" id="rememberMe" v-model="rememberMe">
              <label class="form-check-label" for="rememberMe">Ghi nhớ đăng nhập</label>
            </div>
            <router-link to="/forgot-password" class="text-primary text-decoration-none small">
              Quên mật khẩu?
            </router-link>
          </div>

          <button 
            type="submit" 
            class="btn btn-primary w-100 py-2"
            :disabled="authStore.isLoading"
          >
            <span v-if="authStore.isLoading" class="spinner-border spinner-border-sm me-2"></span>
            <span v-if="authStore.isLoading">Đang đăng nhập...</span>
            <span v-else>Đăng nhập</span>
          </button>
        </form>

        <!-- Divider -->
        <div class="divider my-4">
          <span>hoặc</span>
        </div>

        <!-- Register Link -->
        <p class="text-center mb-0">
          Chưa có tài khoản?
          <router-link to="/register" class="text-primary fw-semibold text-decoration-none">
            Đăng ký ngay
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
import { ref, reactive, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'

const route = useRoute()
const authStore = useAuthStore()

const form = reactive({
  email: '',
  password: ''
})

const errors = reactive({
  email: '',
  password: ''
})

const showPassword = ref(false)
const rememberMe = ref(false)

const sessionExpired = computed(() => route.query.expired === 'true')

onMounted(() => {
  // Load saved email if remember me was checked
  const savedEmail = localStorage.getItem('pcm_remember_email')
  if (savedEmail) {
    form.email = savedEmail
    rememberMe.value = true
  }
})

function validateEmail() {
  errors.email = ''
  if (!form.email) {
    errors.email = 'Vui lòng nhập email'
    return false
  }
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!emailRegex.test(form.email)) {
    errors.email = 'Email không hợp lệ'
    return false
  }
  return true
}

function validatePassword() {
  errors.password = ''
  if (!form.password) {
    errors.password = 'Vui lòng nhập mật khẩu'
    return false
  }
  if (form.password.length < 6) {
    errors.password = 'Mật khẩu phải có ít nhất 6 ký tự'
    return false
  }
  return true
}

function validate() {
  const isEmailValid = validateEmail()
  const isPasswordValid = validatePassword()
  return isEmailValid && isPasswordValid
}

async function handleLogin() {
  if (!validate()) return

  // Save email if remember me is checked
  if (rememberMe.value) {
    localStorage.setItem('pcm_remember_email', form.email)
  } else {
    localStorage.removeItem('pcm_remember_email')
  }

  await authStore.login({
    email: form.email,
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
  max-width: 420px;
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

.input-group-text {
  background: white;
  border-right: none;
}

.input-group .form-control {
  border-left: none;
}

.input-group .form-control:focus {
  border-color: #ced4da;
  box-shadow: none;
}

.input-group:focus-within .input-group-text,
.input-group:focus-within .form-control,
.input-group:focus-within .btn-outline-secondary {
  border-color: var(--pcm-primary);
}

.btn-primary {
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  border: none;
}

.btn-primary:hover {
  background: linear-gradient(135deg, #059669 0%, #047857 100%);
}
</style>
