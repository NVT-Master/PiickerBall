<template>
  <div class="news-page">
    <!-- Page Header -->
    <div class="page-header d-flex justify-content-between align-items-center">
      <div>
        <h1>Tin tức</h1>
        <nav aria-label="breadcrumb">
          <ol class="breadcrumb">
            <li class="breadcrumb-item"><router-link to="/dashboard">Dashboard</router-link></li>
            <li class="breadcrumb-item active">Tin tức</li>
          </ol>
        </nav>
      </div>
      <button v-if="authStore.isAdmin" class="btn btn-primary" @click="openCreateModal">
        <i class="bi bi-plus-lg me-2"></i>
        Đăng tin mới
      </button>
    </div>

    <!-- Loading -->
    <div v-if="isLoading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>

    <template v-else>
      <!-- Pinned News -->
      <div v-if="pinnedNews.length > 0" class="mb-4">
        <h5 class="mb-3">
          <i class="bi bi-pin-angle text-danger me-2"></i>
          Tin ghim
        </h5>
        <div class="row g-3">
          <div v-for="news in pinnedNews" :key="news.id" class="col-md-6">
            <div class="card border-danger h-100">
              <div class="card-body">
                <div class="d-flex justify-content-between">
                  <span class="badge bg-danger mb-2">
                    <i class="bi bi-pin me-1"></i> Ghim
                  </span>
                  <small class="text-muted">{{ formatDate(news.createdAt) }}</small>
                </div>
                <h5 class="card-title">{{ news.title }}</h5>
                <p class="card-text text-muted">{{ truncate(news.content, 100) }}</p>
                <router-link :to="`/news/${news.id}`" class="btn btn-outline-primary btn-sm">
                  Đọc tiếp
                </router-link>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- All News -->
      <div class="row g-4">
        <div v-for="news in regularNews" :key="news.id" class="col-md-6 col-lg-4">
          <div class="card h-100">
            <img 
              v-if="news.imageUrl" 
              :src="news.imageUrl" 
              class="card-img-top"
              :alt="news.title"
              style="height: 180px; object-fit: cover;"
            >
            <div v-else class="card-img-top bg-light d-flex align-items-center justify-content-center" style="height: 180px;">
              <i class="bi bi-newspaper text-muted" style="font-size: 3rem;"></i>
            </div>
            <div class="card-body">
              <small class="text-muted">{{ formatDate(news.createdAt) }}</small>
              <h5 class="card-title mt-1">{{ news.title }}</h5>
              <p class="card-text text-muted">{{ truncate(news.content, 80) }}</p>
            </div>
            <div class="card-footer bg-transparent border-0">
              <div class="d-flex justify-content-between align-items-center">
                <router-link :to="`/news/${news.id}`" class="btn btn-outline-primary btn-sm">
                  Đọc tiếp
                </router-link>
                <div v-if="authStore.isAdmin" class="btn-group btn-group-sm">
                  <button class="btn btn-outline-secondary" @click="togglePin(news)">
                    <i class="bi" :class="news.isPinned ? 'bi-pin-fill' : 'bi-pin'"></i>
                  </button>
                  <button class="btn btn-outline-primary" @click="editNews(news)">
                    <i class="bi bi-pencil"></i>
                  </button>
                  <button class="btn btn-outline-danger" @click="handleDelete(news.id)">
                    <i class="bi bi-trash"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="newsList.length === 0" class="empty-state">
        <i class="bi bi-newspaper"></i>
        <h5>Chưa có tin tức nào</h5>
        <p>Các tin tức của CLB sẽ hiển thị tại đây</p>
      </div>
    </template>

    <!-- Create/Edit News Modal -->
    <div class="modal fade" id="newsModal" tabindex="-1">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              {{ editingNews ? 'Sửa tin tức' : 'Đăng tin mới' }}
            </h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <form @submit.prevent="handleSave">
            <div class="modal-body">
              <!-- Title -->
              <div class="mb-3">
                <label class="form-label">Tiêu đề <span class="text-danger">*</span></label>
                <input 
                  type="text" 
                  class="form-control"
                  v-model="form.title"
                  placeholder="Nhập tiêu đề tin"
                  required
                >
              </div>

              <!-- Content -->
              <div class="mb-3">
                <label class="form-label">Nội dung <span class="text-danger">*</span></label>
                <textarea 
                  class="form-control" 
                  rows="10"
                  v-model="form.content"
                  placeholder="Nhập nội dung tin..."
                  required
                ></textarea>
              </div>

              <!-- Image URL -->
              <div class="mb-3">
                <label class="form-label">URL hình ảnh</label>
                <input 
                  type="url" 
                  class="form-control"
                  v-model="form.imageUrl"
                  placeholder="https://example.com/image.jpg"
                >
              </div>

              <!-- Is Pinned -->
              <div class="form-check">
                <input 
                  type="checkbox" 
                  class="form-check-input" 
                  id="isPinned"
                  v-model="form.isPinned"
                >
                <label class="form-check-label" for="isPinned">
                  Ghim tin này
                </label>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                Hủy
              </button>
              <button type="submit" class="btn btn-primary" :disabled="isLoading">
                <span v-if="isLoading" class="spinner-border spinner-border-sm me-2"></span>
                {{ editingNews ? 'Cập nhật' : 'Đăng tin' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth.store'
import { newsApi } from '@/api'
import { useToast } from 'vue-toastification'
import dayjs from 'dayjs'
import { Modal } from 'bootstrap'

const authStore = useAuthStore()
const toast = useToast()

// State
const newsList = ref([])
const isLoading = ref(false)
const newsModal = ref(null)
const editingNews = ref(null)

const form = reactive({
  title: '',
  content: '',
  imageUrl: '',
  isPinned: false
})

// Computed
const pinnedNews = computed(() => newsList.value.filter(n => n.isPinned))
const regularNews = computed(() => newsList.value.filter(n => !n.isPinned))

// Methods
function formatDate(date) {
  return dayjs(date).format('DD/MM/YYYY')
}

function truncate(text, length) {
  if (!text) return ''
  if (text.length <= length) return text
  return text.slice(0, length) + '...'
}

function openCreateModal() {
  editingNews.value = null
  Object.assign(form, {
    title: '',
    content: '',
    imageUrl: '',
    isPinned: false
  })
  
  if (!newsModal.value) {
    newsModal.value = new Modal(document.getElementById('newsModal'))
  }
  newsModal.value.show()
}

function editNews(news) {
  editingNews.value = news
  Object.assign(form, {
    title: news.title,
    content: news.content,
    imageUrl: news.imageUrl || '',
    isPinned: news.isPinned
  })
  
  if (!newsModal.value) {
    newsModal.value = new Modal(document.getElementById('newsModal'))
  }
  newsModal.value.show()
}

async function handleSave() {
  isLoading.value = true
  
  try {
    if (editingNews.value) {
      await newsApi.update(editingNews.value.id, form)
      toast.success('Cập nhật tin thành công!')
    } else {
      await newsApi.create(form)
      toast.success('Đăng tin thành công!')
    }
    
    newsModal.value.hide()
    await loadNews()
  } catch (err) {
    toast.error(err.response?.data?.message || 'Có lỗi xảy ra')
  } finally {
    isLoading.value = false
  }
}

async function handleDelete(id) {
  if (!confirm('Bạn có chắc muốn xóa tin này?')) return
  
  try {
    await newsApi.delete(id)
    newsList.value = newsList.value.filter(n => n.id !== id)
    toast.success('Xóa tin thành công!')
  } catch (err) {
    toast.error('Không thể xóa tin')
  }
}

async function togglePin(news) {
  try {
    await newsApi.togglePin(news.id)
    news.isPinned = !news.isPinned
    toast.success(news.isPinned ? 'Đã ghim tin' : 'Đã bỏ ghim')
  } catch (err) {
    toast.error('Không thể thay đổi trạng thái ghim')
  }
}

async function loadNews() {
  isLoading.value = true
  
  try {
    const response = await newsApi.getAll()
    newsList.value = response.data.items || response.data
  } catch (err) {
    toast.error('Không thể tải tin tức')
  } finally {
    isLoading.value = false
  }
}

// Lifecycle
onMounted(() => {
  loadNews()
})
</script>
