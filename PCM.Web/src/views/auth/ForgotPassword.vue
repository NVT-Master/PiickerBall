<template>
  <div class="auth-page">
    <div class="auth-container">
      <div class="auth-card">
        <div class="text-center mb-4">
          <div class="auth-logo mb-3">🔑</div>
          <h1 class="h3 fw-bold text-dark">Quên mật khẩu</h1>
          <p class="text-muted">Nhập email để nhận link đặt lại mật khẩu</p>
        </div>

        <div v-if="!emailSent">
          <form @submit.prevent="handleSubmit">
            <div class="mb-4">
              <label class="form-label">Email <span class="text-danger">*</span></label>
              <input
                type="email"
                class="form-control"
                :class="{ 'is-invalid': error }"
                v-model="email"
                placeholder="Nhập email đã đăng ký"
                autofocus
              />
              <div class="invalid-feedback">{{ error }}</div>
            </div>

            <button 
              type="submit" 
              class="btn btn-primary w-100 py-2"
              :disabled="isLoading"
            >
              <span v-if="isLoading" class="spinner-border spinner-border-sm me-2"></span>
              Gửi link đặt lại mật khẩu
            </button>
          </form>
        </div>

        <div v-else class="text-center">
          <div class="mb-4">
            <i class="bi bi-envelope-check text-success" style="font-size: 4rem;"></i>
          </div>
          <h5 class="mb-3">Đã gửi email!</h5>
          <p class="text-muted">
            Chúng tôi đã gửi link đặt lại mật khẩu đến <strong>{{ email }}</strong>. 
            Vui lòng kiểm tra hộp thư của bạn.
          </p>
          <button class="btn btn-outline-primary" @click="emailSent = false">
            Gửi lại
          </button>
        </div>

        <div class="text-center mt-4">
          <router-link to="/login" class="text-primary text-decoration-none">
            <i class="bi bi-arrow-left me-1"></i>
            Quay lại đăng nhập
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { authApi } from '@/api'
import { useToast } from 'vue-toastification'

const toast = useToast()

const email = ref('')
const error = ref('')
const isLoading = ref(false)
const emailSent = ref(false)

async function handleSubmit() {
  error.value = ''
  
  if (!email.value) {
    error.value = 'Vui lòng nhập email'
    return
  }
  
  if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value)) {
    error.value = 'Email không hợp lệ'
    return
  }

  isLoading.value = true
  try {
    await authApi.forgotPassword(email.value)
    emailSent.value = true
    toast.success('Đã gửi email đặt lại mật khẩu!')
  } catch (err) {
    error.value = err.response?.data?.message || 'Có lỗi xảy ra, vui lòng thử lại'
  } finally {
    isLoading.value = false
  }
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

.btn-primary {
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  border: none;
}
</style>
