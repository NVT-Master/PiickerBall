<template>
  <div class="error-page d-flex align-items-center justify-content-center" style="min-height: 100vh;">
    <div class="text-center">
      <div class="display-1 text-warning mb-4">
        <i class="bi bi-question-circle"></i>
      </div>
      <h1 class="display-4 mb-3">404</h1>
      <h2 class="mb-4">Không tìm thấy trang</h2>
      <p class="text-muted mb-4">
        Trang bạn đang tìm kiếm không tồn tại<br>
        hoặc đã được di chuyển đến địa chỉ khác.
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
