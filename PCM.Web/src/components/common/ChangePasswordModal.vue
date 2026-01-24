<template>
  <BaseModal 
    title="Đổi mật khẩu" 
    @close="$emit('close')"
    @confirm="handleSubmit"
    :loading="isLoading"
    confirm-text="Đổi mật khẩu"
  >
    <form @submit.prevent="handleSubmit">
      <div class="mb-3">
        <label class="form-label">Mật khẩu hiện tại <span class="text-danger">*</span></label>
        <input
          type="password"
          class="form-control"
          :class="{ 'is-invalid': errors.currentPassword }"
          v-model="form.currentPassword"
          placeholder="Nhập mật khẩu hiện tại"
        />
        <div class="invalid-feedback">{{ errors.currentPassword }}</div>
      </div>

      <div class="mb-3">
        <label class="form-label">Mật khẩu mới <span class="text-danger">*</span></label>
        <input
          type="password"
          class="form-control"
          :class="{ 'is-invalid': errors.newPassword }"
          v-model="form.newPassword"
          placeholder="Nhập mật khẩu mới"
        />
        <div class="invalid-feedback">{{ errors.newPassword }}</div>
      </div>

      <div class="mb-3">
        <label class="form-label">Xác nhận mật khẩu mới <span class="text-danger">*</span></label>
        <input
          type="password"
          class="form-control"
          :class="{ 'is-invalid': errors.confirmPassword }"
          v-model="form.confirmPassword"
          placeholder="Nhập lại mật khẩu mới"
        />
        <div class="invalid-feedback">{{ errors.confirmPassword }}</div>
      </div>
    </form>
  </BaseModal>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { useAuthStore } from '@/stores/auth.store'
import BaseModal from './BaseModal.vue'

const emit = defineEmits(['close'])

const authStore = useAuthStore()
const isLoading = ref(false)

const form = reactive({
  currentPassword: '',
  newPassword: '',
  confirmPassword: ''
})

const errors = reactive({
  currentPassword: '',
  newPassword: '',
  confirmPassword: ''
})

function validate() {
  let isValid = true
  
  // Reset errors
  errors.currentPassword = ''
  errors.newPassword = ''
  errors.confirmPassword = ''

  if (!form.currentPassword) {
    errors.currentPassword = 'Vui lòng nhập mật khẩu hiện tại'
    isValid = false
  }

  if (!form.newPassword) {
    errors.newPassword = 'Vui lòng nhập mật khẩu mới'
    isValid = false
  } else if (form.newPassword.length < 6) {
    errors.newPassword = 'Mật khẩu phải có ít nhất 6 ký tự'
    isValid = false
  }

  if (!form.confirmPassword) {
    errors.confirmPassword = 'Vui lòng xác nhận mật khẩu mới'
    isValid = false
  } else if (form.newPassword !== form.confirmPassword) {
    errors.confirmPassword = 'Mật khẩu xác nhận không khớp'
    isValid = false
  }

  return isValid
}

async function handleSubmit() {
  if (!validate()) return

  isLoading.value = true
  try {
    const result = await authStore.changePassword({
      currentPassword: form.currentPassword,
      newPassword: form.newPassword
    })

    if (result.success) {
      emit('close')
    }
  } finally {
    isLoading.value = false
  }
}
</script>
