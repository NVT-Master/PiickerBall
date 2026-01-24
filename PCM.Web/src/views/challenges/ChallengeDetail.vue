<template>
  <div class="challenge-detail-page">
    <!-- Loading -->
    <div v-if="isLoading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>

    <template v-else-if="challenge">
      <!-- Page Header -->
      <div class="page-header">
        <nav aria-label="breadcrumb">
          <ol class="breadcrumb">
            <li class="breadcrumb-item"><router-link to="/dashboard">Dashboard</router-link></li>
            <li class="breadcrumb-item"><router-link to="/challenges">Sàn đấu</router-link></li>
            <li class="breadcrumb-item active">Chi tiết kèo</li>
          </ol>
        </nav>
        <div class="d-flex justify-content-between align-items-start">
          <div>
            <h1>{{ challenge.title || 'Kèo #' + challenge.id }}</h1>
            <span class="badge" :class="getStatusBadgeClass(challenge.status)">
              {{ getStatusText(challenge.status) }}
            </span>
          </div>
          <div class="d-flex gap-2" v-if="authStore.isAdmin">
            <button 
              v-if="challenge.status === 1"
              class="btn btn-warning"
              @click="handleAutoDivide"
            >
              <i class="bi bi-shuffle me-2"></i>
              Chia team
            </button>
            <button 
              v-if="challenge.status === 1"
              class="btn btn-success"
              @click="handleStart"
            >
              <i class="bi bi-play me-2"></i>
              Bắt đầu
            </button>
            <button 
              v-if="challenge.status === 0"
              class="btn btn-outline-danger"
              @click="handleDelete"
            >
              <i class="bi bi-trash me-2"></i>
              Xóa
            </button>
          </div>
        </div>
      </div>

      <div class="row g-4">
        <!-- Main Info -->
        <div class="col-lg-8">
          <!-- Prize Card -->
          <div 
            class="card mb-4"
            :class="challenge.type === 0 ? 'border-secondary' : 'border-primary'"
          >
            <div 
              class="card-header text-white"
              :style="{ 
                background: challenge.type === 0 
                  ? 'linear-gradient(135deg, #6366f1, #4f46e5)' 
                  : 'linear-gradient(135deg, #10b981, #059669)' 
              }"
            >
              <div class="d-flex justify-content-between align-items-center">
                <span>
                  <i class="bi bi-trophy me-2"></i>
                  Tổng giải thưởng
                </span>
                <span class="badge bg-white text-dark">
                  {{ challenge.type === 0 ? 'Đơn' : 'Đôi' }}
                </span>
              </div>
              <div class="display-5 fw-bold mt-2">
                {{ formatCurrency(challenge.prizePool) }}
              </div>
            </div>
            <div class="card-body">
              <div class="row text-center">
                <div class="col-4">
                  <div class="text-muted small">Phí tham gia</div>
                  <div class="fw-bold text-primary">{{ formatCurrency(challenge.entryFee) }}</div>
                </div>
                <div class="col-4">
                  <div class="text-muted small">Số người</div>
                  <div class="fw-bold">{{ participants.length }}/{{ challenge.maxParticipants }}</div>
                </div>
                <div class="col-4">
                  <div class="text-muted small">Thời gian</div>
                  <div class="fw-bold">{{ formatDate(challenge.scheduledAt) }}</div>
                </div>
              </div>
            </div>
          </div>

          <!-- Participants -->
          <div class="card">
            <div class="card-header d-flex justify-content-between align-items-center">
              <h5 class="mb-0">
                <i class="bi bi-people me-2"></i>
                Danh sách người tham gia
              </h5>
              <button 
                v-if="challenge.status === 0 && !isJoined"
                class="btn btn-primary btn-sm"
                @click="handleJoin"
              >
                <i class="bi bi-plus me-1"></i>
                Tham gia
              </button>
            </div>
            <div class="card-body">
              <div v-if="participants.length === 0" class="text-center text-muted py-4">
                Chưa có ai tham gia
              </div>
              
              <div v-else class="row g-3">
                <!-- Team A -->
                <div class="col-md-6">
                  <h6 class="text-primary mb-3">
                    <i class="bi bi-people-fill me-2"></i>
                    Team A
                  </h6>
                  <div 
                    v-for="p in teamA" 
                    :key="p.id"
                    class="d-flex align-items-center gap-3 p-2 rounded bg-light mb-2"
                  >
                    <div class="avatar">
                      {{ getInitials(p.member?.fullName) }}
                    </div>
                    <div>
                      <div class="fw-medium">{{ p.member?.fullName }}</div>
                      <small class="text-muted">ELO: {{ p.member?.eloRating || 1000 }}</small>
                    </div>
                  </div>
                </div>

                <!-- Team B -->
                <div class="col-md-6">
                  <h6 class="text-danger mb-3">
                    <i class="bi bi-people-fill me-2"></i>
                    Team B
                  </h6>
                  <div 
                    v-for="p in teamB" 
                    :key="p.id"
                    class="d-flex align-items-center gap-3 p-2 rounded bg-light mb-2"
                  >
                    <div class="avatar">
                      {{ getInitials(p.member?.fullName) }}
                    </div>
                    <div>
                      <div class="fw-medium">{{ p.member?.fullName }}</div>
                      <small class="text-muted">ELO: {{ p.member?.eloRating || 1000 }}</small>
                    </div>
                  </div>
                </div>

                <!-- Unassigned -->
                <div v-if="unassigned.length > 0" class="col-12">
                  <h6 class="text-muted mb-3">
                    <i class="bi bi-question-circle me-2"></i>
                    Chưa chia team
                  </h6>
                  <div class="d-flex flex-wrap gap-2">
                    <div 
                      v-for="p in unassigned" 
                      :key="p.id"
                      class="d-flex align-items-center gap-2 p-2 rounded border"
                    >
                      <div class="avatar avatar-sm">
                        {{ getInitials(p.member?.fullName) }}
                      </div>
                      <span>{{ p.member?.fullName }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Sidebar -->
        <div class="col-lg-4">
          <!-- Actions -->
          <div class="card mb-4">
            <div class="card-body">
              <div v-if="challenge.status === 0">
                <button 
                  v-if="!isJoined"
                  class="btn btn-primary w-100 mb-2"
                  @click="handleJoin"
                >
                  <i class="bi bi-person-plus me-2"></i>
                  Tham gia kèo này
                </button>
                <button 
                  v-else
                  class="btn btn-outline-danger w-100"
                  @click="handleLeave"
                >
                  <i class="bi bi-person-dash me-2"></i>
                  Rời khỏi kèo
                </button>
              </div>
              <div v-else-if="challenge.status === 2" class="text-center">
                <i class="bi bi-play-circle text-primary display-4"></i>
                <p class="mt-2 mb-0">Kèo đang diễn ra</p>
              </div>
              <div v-else-if="challenge.status === 3" class="text-center">
                <i class="bi bi-trophy text-warning display-4"></i>
                <p class="mt-2 mb-0">Kèo đã kết thúc</p>
                <div v-if="challenge.winningSide" class="mt-2">
                  <strong>Team chiến thắng:</strong> 
                  {{ challenge.winningSide === 1 ? 'Team A' : 'Team B' }}
                </div>
              </div>
            </div>
          </div>

          <!-- Info -->
          <div class="card">
            <div class="card-header">
              <h6 class="mb-0">Thông tin chi tiết</h6>
            </div>
            <div class="card-body">
              <table class="table table-borderless table-sm mb-0">
                <tr>
                  <td class="text-muted">ID</td>
                  <td class="text-end">#{{ challenge.id }}</td>
                </tr>
                <tr>
                  <td class="text-muted">Người tạo</td>
                  <td class="text-end">{{ challenge.creator?.fullName || 'N/A' }}</td>
                </tr>
                <tr>
                  <td class="text-muted">Ngày tạo</td>
                  <td class="text-end">{{ formatDate(challenge.createdAt) }}</td>
                </tr>
                <tr v-if="challenge.description">
                  <td colspan="2">
                    <div class="text-muted small">Mô tả:</div>
                    <div>{{ challenge.description }}</div>
                  </td>
                </tr>
              </table>
            </div>
          </div>
        </div>
      </div>

      <!-- Complete Modal (Admin) -->
      <div class="modal fade" id="completeModal" tabindex="-1">
        <div class="modal-dialog">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">Kết thúc kèo</h5>
              <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
              <p>Chọn team chiến thắng:</p>
              <div class="d-grid gap-2">
                <button 
                  class="btn btn-outline-primary btn-lg"
                  @click="handleComplete(1)"
                >
                  Team A thắng
                </button>
                <button 
                  class="btn btn-outline-danger btn-lg"
                  @click="handleComplete(2)"
                >
                  Team B thắng
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>

    <!-- Not Found -->
    <div v-else class="text-center py-5">
      <i class="bi bi-exclamation-circle display-1 text-muted"></i>
      <h3 class="mt-3">Không tìm thấy kèo</h3>
      <router-link to="/challenges" class="btn btn-primary mt-3">
        Quay lại danh sách
      </router-link>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useChallengeStore } from '@/stores/challenge.store'
import { useAuthStore } from '@/stores/auth.store'
import dayjs from 'dayjs'

const route = useRoute()
const router = useRouter()
const challengeStore = useChallengeStore()
const authStore = useAuthStore()

// Computed
const isLoading = computed(() => challengeStore.isLoading)
const challenge = computed(() => challengeStore.currentChallenge)
const participants = computed(() => challengeStore.participants)

const teamA = computed(() => participants.value.filter(p => p.side === 1))
const teamB = computed(() => participants.value.filter(p => p.side === 2))
const unassigned = computed(() => participants.value.filter(p => !p.side || p.side === 0))

const isJoined = computed(() => {
  const userId = authStore.user?.id
  return participants.value.some(p => p.memberId === userId)
})

// Methods
function formatCurrency(value) {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(value || 0)
}

function formatDate(date) {
  if (!date) return 'Chưa xác định'
  return dayjs(date).format('DD/MM/YYYY HH:mm')
}

function getStatusText(status) {
  const statusTexts = {
    0: 'Đang mở',
    1: 'Đã đủ người',
    2: 'Đang diễn ra',
    3: 'Đã kết thúc',
    4: 'Đã hủy'
  }
  return statusTexts[status] || 'Không xác định'
}

function getStatusBadgeClass(status) {
  const classes = {
    0: 'bg-success',
    1: 'bg-warning text-dark',
    2: 'bg-primary',
    3: 'bg-secondary',
    4: 'bg-danger'
  }
  return classes[status] || 'bg-secondary'
}

function getInitials(name) {
  if (!name) return '?'
  return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
}

async function handleJoin() {
  await challengeStore.joinChallenge(route.params.id)
}

async function handleLeave() {
  await challengeStore.leaveChallenge(route.params.id)
}

async function handleAutoDivide() {
  await challengeStore.autoDivideTeams(route.params.id)
}

async function handleStart() {
  await challengeStore.startChallenge(route.params.id)
}

async function handleComplete(winningSide) {
  await challengeStore.completeChallenge(route.params.id, { winningSide })
}

async function handleDelete() {
  if (confirm('Bạn có chắc muốn xóa kèo này?')) {
    await challengeStore.deleteChallenge(route.params.id)
    router.push('/challenges')
  }
}

// Lifecycle
onMounted(async () => {
  const id = route.params.id
  await Promise.all([
    challengeStore.fetchChallengeById(id),
    challengeStore.fetchParticipants(id)
  ])
})
</script>
