<template>
  <div class="challenges-page">
    <!-- Page Header -->
    <div class="page-header d-flex justify-content-between align-items-center">
      <div>
        <h1>Sàn đấu</h1>
        <nav aria-label="breadcrumb">
          <ol class="breadcrumb">
            <li class="breadcrumb-item"><router-link to="/dashboard">Dashboard</router-link></li>
            <li class="breadcrumb-item active">Sàn đấu</li>
          </ol>
        </nav>
      </div>
      <button class="btn btn-primary" @click="openCreateModal">
        <i class="bi bi-plus-lg me-2"></i>
        Tạo kèo mới
      </button>
    </div>

    <!-- Filter Tabs -->
    <ul class="nav nav-pills mb-4">
      <li class="nav-item">
        <button 
          class="nav-link" 
          :class="{ active: filter === 'all' }"
          @click="filter = 'all'"
        >
          Tất cả
        </button>
      </li>
      <li class="nav-item">
        <button 
          class="nav-link" 
          :class="{ active: filter === 'open' }"
          @click="filter = 'open'"
        >
          <i class="bi bi-unlock me-1"></i>
          Đang mở
          <span class="badge bg-success ms-1">{{ openCount }}</span>
        </button>
      </li>
      <li class="nav-item">
        <button 
          class="nav-link" 
          :class="{ active: filter === 'in_progress' }"
          @click="filter = 'in_progress'"
        >
          <i class="bi bi-play-circle me-1"></i>
          Đang diễn ra
        </button>
      </li>
      <li class="nav-item">
        <button 
          class="nav-link" 
          :class="{ active: filter === 'completed' }"
          @click="filter = 'completed'"
        >
          <i class="bi bi-check-circle me-1"></i>
          Đã kết thúc
        </button>
      </li>
    </ul>

    <!-- Loading -->
    <div v-if="isLoading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>

    <!-- Challenge Cards -->
    <div v-else-if="filteredChallenges.length > 0" class="row g-4">
      <div 
        v-for="challenge in filteredChallenges" 
        :key="challenge.id"
        class="col-md-6 col-lg-4"
      >
        <div class="challenge-card card h-100">
          <!-- Header -->
          <div 
            class="challenge-header"
            :class="challenge.type === 0 ? 'type-single' : 'type-double'"
          >
            <div class="d-flex justify-content-between align-items-start">
              <span class="badge bg-white text-dark">
                {{ challenge.type === 0 ? 'Đơn' : 'Đôi' }}
              </span>
              <span class="badge" :class="getStatusBadgeClass(challenge.status)">
                {{ getStatusText(challenge.status) }}
              </span>
            </div>
            <div class="prize-pool mt-3">
              {{ formatCurrency(challenge.prizePool) }}
            </div>
            <small class="opacity-75">Tổng giải thưởng</small>
          </div>

          <!-- Body -->
          <div class="card-body">
            <h5 class="card-title">{{ challenge.title || 'Kèo #' + challenge.id }}</h5>
            
            <div class="d-flex gap-3 text-muted small mb-3">
              <span>
                <i class="bi bi-cash me-1"></i>
                Phí: {{ formatCurrency(challenge.entryFee) }}
              </span>
              <span>
                <i class="bi bi-people me-1"></i>
                {{ challenge.currentParticipants || 0 }}/{{ challenge.maxParticipants }}
              </span>
            </div>

            <div class="mb-3">
              <div class="progress" style="height: 8px;">
                <div 
                  class="progress-bar bg-primary"
                  :style="{ width: getParticipantProgress(challenge) + '%' }"
                ></div>
              </div>
              <small class="text-muted">
                {{ challenge.currentParticipants || 0 }} người tham gia
              </small>
            </div>

            <div class="text-muted small">
              <i class="bi bi-clock me-1"></i>
              {{ formatDate(challenge.scheduledAt || challenge.createdAt) }}
            </div>
          </div>

          <!-- Footer -->
          <div class="card-footer bg-transparent border-top-0">
            <div class="d-flex gap-2">
              <router-link 
                :to="`/challenges/${challenge.id}`"
                class="btn btn-outline-primary btn-sm flex-grow-1"
              >
                Chi tiết
              </router-link>
              <button 
                v-if="challenge.status === 0 && !isJoined(challenge)"
                class="btn btn-primary btn-sm flex-grow-1"
                @click="handleJoin(challenge.id)"
                :disabled="isLoading"
              >
                Tham gia
              </button>
              <button 
                v-else-if="challenge.status === 0 && isJoined(challenge)"
                class="btn btn-outline-danger btn-sm flex-grow-1"
                @click="handleLeave(challenge.id)"
                :disabled="isLoading"
              >
                Rời khỏi
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else class="empty-state">
      <i class="bi bi-trophy"></i>
      <h5>Chưa có kèo đấu nào</h5>
      <p>Hãy tạo kèo mới để bắt đầu!</p>
      <button class="btn btn-primary" @click="openCreateModal">
        <i class="bi bi-plus-lg me-2"></i>
        Tạo kèo mới
      </button>
    </div>

    <!-- Create Challenge Modal -->
    <div class="modal fade" id="createChallengeModal" tabindex="-1">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Tạo kèo mới</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <form @submit.prevent="handleCreate">
            <div class="modal-body">
              <!-- Title -->
              <div class="mb-3">
                <label class="form-label">Tên kèo</label>
                <input 
                  type="text" 
                  class="form-control"
                  v-model="createForm.title"
                  placeholder="VD: Kèo chiều thứ 7"
                >
              </div>

              <!-- Type -->
              <div class="mb-3">
                <label class="form-label">Thể thức <span class="text-danger">*</span></label>
                <div class="btn-group w-100">
                  <input 
                    type="radio" 
                    class="btn-check" 
                    id="type-single"
                    v-model="createForm.type"
                    :value="0"
                  >
                  <label class="btn btn-outline-primary" for="type-single">
                    <i class="bi bi-person me-2"></i>
                    Đơn
                  </label>

                  <input 
                    type="radio" 
                    class="btn-check" 
                    id="type-double"
                    v-model="createForm.type"
                    :value="1"
                  >
                  <label class="btn btn-outline-primary" for="type-double">
                    <i class="bi bi-people me-2"></i>
                    Đôi
                  </label>
                </div>
              </div>

              <!-- Entry Fee -->
              <div class="mb-3">
                <label class="form-label">Phí tham gia <span class="text-danger">*</span></label>
                <div class="input-group">
                  <input 
                    type="number" 
                    class="form-control"
                    v-model.number="createForm.entryFee"
                    min="0"
                    step="10000"
                    required
                  >
                  <span class="input-group-text">VNĐ</span>
                </div>
              </div>

              <!-- Max Participants -->
              <div class="mb-3">
                <label class="form-label">Số người tối đa <span class="text-danger">*</span></label>
                <select class="form-select" v-model.number="createForm.maxParticipants" required>
                  <option :value="4">4 người</option>
                  <option :value="8">8 người</option>
                  <option :value="12">12 người</option>
                  <option :value="16">16 người</option>
                </select>
              </div>

              <!-- Scheduled At -->
              <div class="mb-3">
                <label class="form-label">Thời gian dự kiến</label>
                <input 
                  type="datetime-local" 
                  class="form-control"
                  v-model="createForm.scheduledAt"
                >
              </div>

              <!-- Description -->
              <div class="mb-3">
                <label class="form-label">Mô tả</label>
                <textarea 
                  class="form-control" 
                  rows="3"
                  v-model="createForm.description"
                  placeholder="Ghi chú thêm về kèo..."
                ></textarea>
              </div>

              <!-- Prize Preview -->
              <div class="alert alert-info">
                <strong>Giải thưởng dự kiến:</strong>
                {{ formatCurrency(createForm.entryFee * createForm.maxParticipants) }}
                <br>
                <small class="text-muted">
                  ({{ createForm.maxParticipants }} người x {{ formatCurrency(createForm.entryFee) }})
                </small>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                Hủy
              </button>
              <button type="submit" class="btn btn-primary" :disabled="isLoading">
                <span v-if="isLoading" class="spinner-border spinner-border-sm me-2"></span>
                Tạo kèo
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
import { useChallengeStore } from '@/stores/challenge.store'
import { useAuthStore } from '@/stores/auth.store'
import dayjs from 'dayjs'
import { Modal } from 'bootstrap'

const challengeStore = useChallengeStore()
const authStore = useAuthStore()

// State
const filter = ref('all')
const createModal = ref(null)
const createForm = reactive({
  title: '',
  type: 1, // Default: Double
  entryFee: 50000,
  maxParticipants: 8,
  scheduledAt: '',
  description: ''
})

// Computed
const isLoading = computed(() => challengeStore.isLoading)
const challenges = computed(() => challengeStore.challenges)
const openCount = computed(() => challengeStore.openCount)

const filteredChallenges = computed(() => {
  if (filter.value === 'all') return challenges.value
  
  const statusMap = {
    'open': 0,
    'in_progress': 2,
    'completed': 3
  }
  
  return challenges.value.filter(c => c.status === statusMap[filter.value])
})

// Methods
function formatCurrency(value) {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(value || 0)
}

function formatDate(date) {
  return dayjs(date).format('DD/MM/YYYY HH:mm')
}

function getStatusText(status) {
  const statusTexts = {
    0: 'Đang mở',
    1: 'Đã đủ',
    2: 'Đang diễn ra',
    3: 'Đã kết thúc',
    4: 'Đã hủy'
  }
  return statusTexts[status] || 'Không xác định'
}

function getStatusBadgeClass(status) {
  const classes = {
    0: 'bg-success',
    1: 'bg-warning',
    2: 'bg-primary',
    3: 'bg-secondary',
    4: 'bg-danger'
  }
  return classes[status] || 'bg-secondary'
}

function getParticipantProgress(challenge) {
  const current = challenge.currentParticipants || 0
  const max = challenge.maxParticipants || 1
  return (current / max) * 100
}

function isJoined(challenge) {
  // Check if current user is in participants
  const userId = authStore.user?.id
  return challenge.participants?.some(p => p.memberId === userId)
}

function openCreateModal() {
  if (!createModal.value) {
    createModal.value = new Modal(document.getElementById('createChallengeModal'))
  }
  // Reset form
  Object.assign(createForm, {
    title: '',
    type: 1,
    entryFee: 50000,
    maxParticipants: 8,
    scheduledAt: '',
    description: ''
  })
  createModal.value.show()
}

async function handleCreate() {
  try {
    await challengeStore.createChallenge(createForm)
    createModal.value.hide()
    await loadData()
  } catch (err) {
    console.error('Failed to create challenge:', err)
  }
}

async function handleJoin(id) {
  try {
    await challengeStore.joinChallenge(id)
    await loadData()
  } catch (err) {
    console.error('Failed to join challenge:', err)
  }
}

async function handleLeave(id) {
  try {
    await challengeStore.leaveChallenge(id)
    await loadData()
  } catch (err) {
    console.error('Failed to leave challenge:', err)
  }
}

async function loadData() {
  await Promise.all([
    challengeStore.fetchChallenges(),
    challengeStore.fetchOpenCount()
  ])
}

// Lifecycle
onMounted(() => {
  loadData()
})
</script>

<style scoped>
.challenge-card {
  border: none;
  border-radius: 1rem;
  overflow: hidden;
  transition: transform 0.2s, box-shadow 0.2s;
}

.challenge-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.15);
}

.challenge-header {
  padding: 1.5rem;
  color: white;
}

.challenge-header.type-single {
  background: linear-gradient(135deg, #6366f1, #4f46e5);
}

.challenge-header.type-double {
  background: linear-gradient(135deg, #10b981, #059669);
}

.prize-pool {
  font-size: 1.75rem;
  font-weight: 700;
}

.nav-pills .nav-link {
  color: #64748b;
}

.nav-pills .nav-link.active {
  background-color: #10b981;
}
</style>
