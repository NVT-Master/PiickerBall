<template>
  <div id="app">
    <router-view />
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth.store'

const authStore = useAuthStore()

onMounted(() => {
  // Khởi tạo auth state từ localStorage khi app load
  authStore.initializeAuth()
})
</script>

<style>
#app {
  min-height: 100vh;
}
</style>
