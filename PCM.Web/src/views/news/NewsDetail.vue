<template>
  <div class="news-detail-page">
    <!-- Loading -->
    <div v-if="isLoading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>

    <template v-else-if="news">
      <!-- Breadcrumb -->
      <nav aria-label="breadcrumb" class="mb-4">
        <ol class="breadcrumb">
          <li class="breadcrumb-item"><router-link to="/dashboard">Dashboard</router-link></li>
          <li class="breadcrumb-item"><router-link to="/news">Tin tức</router-link></li>
          <li class="breadcrumb-item active">Chi tiết</li>
        </ol>
      </nav>

      <div class="row">
        <div class="col-lg-8 mx-auto">
          <article class="card">
            <!-- Image -->
            <img 
              v-if="news.imageUrl" 
              :src="news.imageUrl" 
              class="card-img-top"
              :alt="news.title"
              style="max-height: 400px; object-fit: cover;"
            >

            <div class="card-body">
              <!-- Meta -->
              <div class="d-flex align-items-center gap-3 mb-3">
                <span v-if="news.isPinned" class="badge bg-danger">
                  <i class="bi bi-pin me-1"></i>Ghim
                </span>
                <span class="text-muted">
                  <i class="bi bi-calendar me-1"></i>
                  {{ formatDate(news.createdAt) }}
                </span>
                <span class="text-muted">
                  <i class="bi bi-person me-1"></i>
                  {{ news.author?.fullName || 'Admin' }}
                </span>
              </div>

              <!-- Title -->
              <h1 class="card-title mb-4">{{ news.title }}</h1>

              <!-- Content -->
              <div class="card-text news-content" v-html="formatContent(news.content)"></div>
            </div>

            <!-- Footer Actions -->
            <div v-if="authStore.isAdmin" class="card-footer bg-transparent">
              <div class="d-flex justify-content-end gap-2">
                <button class="btn btn-outline-primary" @click="editNews">
                  <i class="bi bi-pencil me-2"></i>Sửa
                </button>
                <button class="btn btn-outline-danger" @click="handleDelete">
                  <i class="bi bi-trash me-2"></i>Xóa
                </button>
              </div>
            </div>
          </article>

          <!-- Back Button -->
          <div class="mt-4">
            <router-link to="/news" class="btn btn-outline-secondary">
              <i class="bi bi-arrow-left me-2"></i>
              Quay lại danh sách
            </router-link>
          </div>
        </div>
      </div>
    </template>

    <!-- Not Found -->
    <div v-else class="text-center py-5">
      <i class="bi bi-exclamation-circle display-1 text-muted"></i>
      <h3 class="mt-3">Không tìm thấy tin tức</h3>
      <router-link to="/news" class="btn btn-primary mt-3">
        Quay lại danh sách
      </router-link>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'
import { newsApi } from '@/api'
import { useToast } from 'vue-toastification'
import dayjs from 'dayjs'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const toast = useToast()

// State
const news = ref(null)
const isLoading = ref(false)

// Methods
function formatDate(date) {
  return dayjs(date).format('DD/MM/YYYY HH:mm')
}

function formatContent(content) {
  if (!content) return ''
  // Convert line breaks to paragraphs
  return content
    .split('\n\n')
    .map(p => `<p>${p.replace(/\n/g, '<br>')}</p>`)
    .join('')
}

function editNews() {
  // TODO: Implement inline edit or redirect to edit page
  alert('Chức năng đang phát triển')
}

async function handleDelete() {
  if (!confirm('Bạn có chắc muốn xóa tin này?')) return
  
  try {
    await newsApi.delete(route.params.id)
    toast.success('Xóa tin thành công!')
    router.push('/news')
  } catch (err) {
    toast.error('Không thể xóa tin')
  }
}

async function loadNews() {
  isLoading.value = true
  
  try {
    const response = await newsApi.getById(route.params.id)
    news.value = response.data
  } catch (err) {
    console.error('Failed to load news:', err)
  } finally {
    isLoading.value = false
  }
}

// Lifecycle
onMounted(() => {
  loadNews()
})
</script>

<style scoped>
.news-content {
  font-size: 1.1rem;
  line-height: 1.8;
}

.news-content p {
  margin-bottom: 1rem;
}
</style>
