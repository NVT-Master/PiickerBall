<template>
  <div class="error-page d-flex align-items-center justify-content-center" style="min-height: 100vh;">
    <div class="text-center">
      <div class="display-1 text-danger mb-4">
        <i class="bi bi-shield-x"></i>
      </div>
      <h1 class="display-4 mb-3">403</h1>
      <h2 class="mb-4">Không có quyền truy cập</h2>
      <p class="text-muted mb-4">
        Bạn không có quyền truy cập trang này.<br>
        Vui lòng liên hệ Admin nếu bạn nghĩ đây là lỗi.
      </p>
      <div class="d-flex gap-3 justify-content-center">
        <router-link to="/dashboard" class="btn btn-primary">
          <i class="bi bi-house me-2"></i>
          Về Dashboard
        </router-link>
        <button class="btn btn-outline-secondary" @click="$router.back()">
          <i class="bi bi-arrow-left me-2"></i>
          Quay lại
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
// No logic needed
</script>

<style scoped>
.error-page {
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
}
</style>
