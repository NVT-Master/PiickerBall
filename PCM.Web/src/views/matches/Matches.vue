<template>
  <div class="matches-page">
    <!-- Page Header -->
    <div class="page-header d-flex justify-content-between align-items-center">
      <div>
        <h1>Quản lý trận đấu</h1>
        <nav aria-label="breadcrumb">
          <ol class="breadcrumb">
            <li class="breadcrumb-item"><router-link to="/dashboard">Dashboard</router-link></li>
            <li class="breadcrumb-item active">Trận đấu</li>
          </ol>
        </nav>
      </div>
      <button class="btn btn-primary" @click="openCreateModal">
        <i class="bi bi-plus-lg me-2"></i>
        Ghi nhận trận đấu
      </button>
    </div>

    <!-- Stats Cards -->
    <div class="row g-3 mb-4">
      <div class="col-md-3">
        <div class="stat-card bg-primary">
          <div class="stat-value">{{ totalMatches }}</div>
          <div class="stat-label">Tổng trận đấu</div>
          <i class="bi bi-controller stat-icon"></i>
        </div>
      </div>
      <div class="col-md-3">
        <div class="stat-card bg-success">
          <div class="stat-value">{{ rankedMatches }}</div>
          <div class="stat-label">Trận xếp hạng</div>
          <i class="bi bi-trophy stat-icon"></i>
        </div>
      </div>
      <div class="col-md-3">
        <div class="stat-card bg-info">
          <div class="stat-value">{{ singleMatches }}</div>
          <div class="stat-label">Trận đơn</div>
          <i class="bi bi-person stat-icon"></i>
        </div>
      </div>
      <div class="col-md-3">
        <div class="stat-card bg-warning">
          <div class="stat-value">{{ doubleMatches }}</div>
          <div class="stat-label">Trận đôi</div>
          <i class="bi bi-people stat-icon"></i>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="card mb-4">
      <div class="card-body">
        <div class="row g-3">
          <div class="col-md-3">
            <select class="form-select" v-model="filters.gameMode" @change="loadMatches">
              <option value="">Tất cả thể thức</option>
              <option value="0">Đơn</option>
              <option value="1">Đôi</option>
            </select>
          </div>
          <div class="col-md-3">
            <select class="form-select" v-model="filters.isRanked" @change="loadMatches">
              <option value="">Tất cả loại</option>
              <option value="true">Xếp hạng</option>
              <option value="false">Giao hữu</option>
            </select>
          </div>
          <div class="col-md-3">
            <input 
              type="date" 
              class="form-control"
              v-model="filters.date"
              @change="loadMatches"
            >
          </div>
          <div class="col-md-3">
            <button class="btn btn-outline-secondary w-100" @click="resetFilters">
              <i class="bi bi-x-circle me-2"></i>
              Xóa bộ lọc
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="isLoading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>

    <!-- Matches Table -->
    <div v-else class="card">
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead>
              <tr>
                <th>ID</th>
                <th>Thời gian</th>
                <th>Thể thức</th>
                <th>Team A</th>
                <th>Kết quả</th>
                <th>Team B</th>
                <th>Loại</th>
                <th class="text-end">Thao tác</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="match in matches" :key="match.id">
                <td>#{{ match.id }}</td>
                <td>{{ formatDate(match.playedAt) }}</td>
                <td>
                  <span class="badge" :class="match.gameMode === 0 ? 'bg-secondary' : 'bg-primary'">
                    {{ match.gameMode === 0 ? 'Đơn' : 'Đôi' }}
                  </span>
                </td>
                <td>
                  <div class="d-flex align-items-center gap-2">
                    <div 
                      v-for="player in match.team1Players" 
                      :key="player.id"
                      class="avatar avatar-sm"
                      :title="player.fullName"
                    >
                      {{ getInitials(player.fullName) }}
                    </div>
                  </div>
                </td>
                <td>
                  <div class="d-flex align-items-center gap-2 fw-bold">
                    <span :class="match.winningSide === 1 ? 'text-success' : ''">
                      {{ match.team1Score }}
                    </span>
                    <span class="text-muted">-</span>
                    <span :class="match.winningSide === 2 ? 'text-success' : ''">
                      {{ match.team2Score }}
                    </span>
                  </div>
                </td>
                <td>
                  <div class="d-flex align-items-center gap-2">
                    <div 
                      v-for="player in match.team2Players" 
                      :key="player.id"
                      class="avatar avatar-sm"
                      :title="player.fullName"
                    >
                      {{ getInitials(player.fullName) }}
                    </div>
                  </div>
                </td>
                <td>
                  <span class="badge" :class="match.isRanked ? 'bg-warning text-dark' : 'bg-light text-dark'">
                    {{ match.isRanked ? 'Xếp hạng' : 'Giao hữu' }}
                  </span>
                </td>
                <td class="text-end">
                  <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" @click="viewMatch(match)">
                      <i class="bi bi-eye"></i>
                    </button>
                    <button 
                      v-if="authStore.isAdmin"
                      class="btn btn-outline-danger" 
                      @click="handleDelete(match.id)"
                    >
                      <i class="bi bi-trash"></i>
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Empty State -->
        <div v-if="matches.length === 0" class="empty-state">
          <i class="bi bi-controller"></i>
          <h5>Chưa có trận đấu nào</h5>
          <p>Hãy ghi nhận trận đấu đầu tiên!</p>
        </div>
      </div>

      <!-- Pagination -->
      <div v-if="matches.length > 0" class="card-footer">
        <div class="d-flex justify-content-between align-items-center">
          <span class="text-muted">
            Hiển thị {{ matches.length }} / {{ totalMatches }} trận
          </span>
          <nav>
            <ul class="pagination pagination-sm mb-0">
              <li class="page-item" :class="{ disabled: pagination.page === 1 }">
                <button class="page-link" @click="changePage(pagination.page - 1)">
                  <i class="bi bi-chevron-left"></i>
                </button>
              </li>
              <li 
                v-for="p in displayedPages" 
                :key="p"
                class="page-item"
                :class="{ active: p === pagination.page }"
              >
                <button class="page-link" @click="changePage(p)">{{ p }}</button>
              </li>
              <li class="page-item" :class="{ disabled: pagination.page === pagination.totalPages }">
                <button class="page-link" @click="changePage(pagination.page + 1)">
                  <i class="bi bi-chevron-right"></i>
                </button>
              </li>
            </ul>
          </nav>
        </div>
      </div>
    </div>

    <!-- Create Match Modal -->
    <div class="modal fade" id="createMatchModal" tabindex="-1">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Ghi nhận trận đấu</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <form @submit.prevent="handleCreate">
            <div class="modal-body">
              <div class="row g-3">
                <!-- Game Mode -->
                <div class="col-md-6">
                  <label class="form-label">Thể thức <span class="text-danger">*</span></label>
                  <div class="btn-group w-100">
                    <input type="radio" class="btn-check" id="mode-single" v-model="form.gameMode" :value="0">
                    <label class="btn btn-outline-primary" for="mode-single">
                      <i class="bi bi-person me-2"></i>Đơn
                    </label>
                    <input type="radio" class="btn-check" id="mode-double" v-model="form.gameMode" :value="1">
                    <label class="btn btn-outline-primary" for="mode-double">
                      <i class="bi bi-people me-2"></i>Đôi
                    </label>
                  </div>
                </div>

                <!-- Match Format -->
                <div class="col-md-6">
                  <label class="form-label">Định dạng</label>
                  <select class="form-select" v-model.number="form.matchFormat">
                    <option :value="1">Best of 1</option>
                    <option :value="3">Best of 3</option>
                    <option :value="5">Best of 5</option>
                  </select>
                </div>

                <!-- Team 1 -->
                <div class="col-md-6">
                  <label class="form-label">Team A <span class="text-danger">*</span></label>
                  <select 
                    class="form-select mb-2" 
                    v-model.number="form.team1Player1"
                    required
                  >
                    <option value="">-- Chọn người chơi 1 --</option>
                    <option 
                      v-for="member in availableMembers" 
                      :key="member.id"
                      :value="member.id"
                      :disabled="isPlayerSelected(member.id, 'team1Player1')"
                    >
                      {{ member.fullName }} (ELO: {{ member.eloRating || 1000 }})
                    </option>
                  </select>
                  <select 
                    v-if="form.gameMode === 1"
                    class="form-select" 
                    v-model.number="form.team1Player2"
                    required
                  >
                    <option value="">-- Chọn người chơi 2 --</option>
                    <option 
                      v-for="member in availableMembers" 
                      :key="member.id"
                      :value="member.id"
                      :disabled="isPlayerSelected(member.id, 'team1Player2')"
                    >
                      {{ member.fullName }} (ELO: {{ member.eloRating || 1000 }})
                    </option>
                  </select>
                </div>

                <!-- Team 2 -->
                <div class="col-md-6">
                  <label class="form-label">Team B <span class="text-danger">*</span></label>
                  <select 
                    class="form-select mb-2" 
                    v-model.number="form.team2Player1"
                    required
                  >
                    <option value="">-- Chọn người chơi 1 --</option>
                    <option 
                      v-for="member in availableMembers" 
                      :key="member.id"
                      :value="member.id"
                      :disabled="isPlayerSelected(member.id, 'team2Player1')"
                    >
                      {{ member.fullName }} (ELO: {{ member.eloRating || 1000 }})
                    </option>
                  </select>
                  <select 
                    v-if="form.gameMode === 1"
                    class="form-select" 
                    v-model.number="form.team2Player2"
                    required
                  >
                    <option value="">-- Chọn người chơi 2 --</option>
                    <option 
                      v-for="member in availableMembers" 
                      :key="member.id"
                      :value="member.id"
                      :disabled="isPlayerSelected(member.id, 'team2Player2')"
                    >
                      {{ member.fullName }} (ELO: {{ member.eloRating || 1000 }})
                    </option>
                  </select>
                </div>

                <!-- Score -->
                <div class="col-12">
                  <label class="form-label">Kết quả <span class="text-danger">*</span></label>
                  <div class="row align-items-center">
                    <div class="col">
                      <input 
                        type="number" 
                        class="form-control form-control-lg text-center"
                        v-model.number="form.team1Score"
                        min="0"
                        required
                      >
                      <small class="text-muted">Team A</small>
                    </div>
                    <div class="col-auto">
                      <span class="display-6 text-muted">-</span>
                    </div>
                    <div class="col">
                      <input 
                        type="number" 
                        class="form-control form-control-lg text-center"
                        v-model.number="form.team2Score"
                        min="0"
                        required
                      >
                      <small class="text-muted">Team B</small>
                    </div>
                  </div>
                </div>

                <!-- Winner -->
                <div class="col-md-6">
                  <label class="form-label">Bên thắng <span class="text-danger">*</span></label>
                  <select class="form-select" v-model.number="form.winningSide" required>
                    <option value="">-- Chọn --</option>
                    <option :value="1">Team A thắng</option>
                    <option :value="2">Team B thắng</option>
                  </select>
                </div>

                <!-- Is Ranked -->
                <div class="col-md-6">
                  <label class="form-label">Loại trận</label>
                  <div class="form-check mt-2">
                    <input 
                      type="checkbox" 
                      class="form-check-input" 
                      id="isRanked"
                      v-model="form.isRanked"
                    >
                    <label class="form-check-label" for="isRanked">
                      Trận xếp hạng (ảnh hưởng ELO)
                    </label>
                  </div>
                </div>

                <!-- Played At -->
                <div class="col-md-6">
                  <label class="form-label">Thời gian thi đấu</label>
                  <input 
                    type="datetime-local" 
                    class="form-control"
                    v-model="form.playedAt"
                  >
                </div>

                <!-- Challenge -->
                <div class="col-md-6">
                  <label class="form-label">Gắn với kèo (nếu có)</label>
                  <select class="form-select" v-model="form.challengeId">
                    <option value="">-- Không --</option>
                    <option 
                      v-for="challenge in openChallenges" 
                      :key="challenge.id"
                      :value="challenge.id"
                    >
                      #{{ challenge.id }} - {{ challenge.title || 'Kèo ' + challenge.id }}
                    </option>
                  </select>
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                Hủy
              </button>
              <button type="submit" class="btn btn-primary" :disabled="isLoading">
                <span v-if="isLoading" class="spinner-border spinner-border-sm me-2"></span>
                Lưu trận đấu
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
import { useMatchStore } from '@/stores/match.store'
import { useMemberStore } from '@/stores/member.store'
import { useChallengeStore } from '@/stores/challenge.store'
import { useAuthStore } from '@/stores/auth.store'
import dayjs from 'dayjs'
import { Modal } from 'bootstrap'

const matchStore = useMatchStore()
const memberStore = useMemberStore()
const challengeStore = useChallengeStore()
const authStore = useAuthStore()

// State
const createModal = ref(null)
const filters = reactive({
  gameMode: '',
  isRanked: '',
  date: ''
})

const form = reactive({
  gameMode: 1,
  matchFormat: 1,
  team1Player1: '',
  team1Player2: '',
  team2Player1: '',
  team2Player2: '',
  team1Score: 0,
  team2Score: 0,
  winningSide: '',
  isRanked: true,
  playedAt: '',
  challengeId: ''
})

// Computed
const isLoading = computed(() => matchStore.isLoading)
const matches = computed(() => matchStore.matches)
const pagination = computed(() => matchStore.pagination)
const totalMatches = computed(() => pagination.value.totalItems)

const rankedMatches = computed(() => matches.value.filter(m => m.isRanked).length)
const singleMatches = computed(() => matches.value.filter(m => m.gameMode === 0).length)
const doubleMatches = computed(() => matches.value.filter(m => m.gameMode === 1).length)

const availableMembers = computed(() => memberStore.members)
const openChallenges = computed(() => challengeStore.openChallenges)

const displayedPages = computed(() => {
  const total = pagination.value.totalPages
  const current = pagination.value.page
  const pages = []
  
  for (let i = Math.max(1, current - 2); i <= Math.min(total, current + 2); i++) {
    pages.push(i)
  }
  
  return pages
})

// Methods
function formatDate(date) {
  return dayjs(date).format('DD/MM/YYYY HH:mm')
}

function getInitials(name) {
  if (!name) return '?'
  return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
}

function isPlayerSelected(memberId, excludeField) {
  const fields = ['team1Player1', 'team1Player2', 'team2Player1', 'team2Player2']
  return fields
    .filter(f => f !== excludeField)
    .some(f => form[f] === memberId)
}

function openCreateModal() {
  if (!createModal.value) {
    createModal.value = new Modal(document.getElementById('createMatchModal'))
  }
  
  // Reset form
  Object.assign(form, {
    gameMode: 1,
    matchFormat: 1,
    team1Player1: '',
    team1Player2: '',
    team2Player1: '',
    team2Player2: '',
    team1Score: 0,
    team2Score: 0,
    winningSide: '',
    isRanked: true,
    playedAt: dayjs().format('YYYY-MM-DDTHH:mm'),
    challengeId: ''
  })
  
  createModal.value.show()
}

async function handleCreate() {
  const matchData = {
    gameMode: form.gameMode,
    matchFormat: form.matchFormat,
    team1Players: form.gameMode === 0 
      ? [form.team1Player1]
      : [form.team1Player1, form.team1Player2],
    team2Players: form.gameMode === 0 
      ? [form.team2Player1]
      : [form.team2Player1, form.team2Player2],
    team1Score: form.team1Score,
    team2Score: form.team2Score,
    winningSide: form.winningSide,
    isRanked: form.isRanked,
    playedAt: form.playedAt || new Date().toISOString(),
    challengeId: form.challengeId || null
  }

  try {
    await matchStore.createMatch(matchData)
    createModal.value.hide()
    await loadMatches()
  } catch (err) {
    console.error('Failed to create match:', err)
  }
}

async function handleDelete(id) {
  if (confirm('Bạn có chắc muốn xóa trận đấu này?')) {
    await matchStore.deleteMatch(id)
  }
}

function viewMatch(match) {
  // TODO: Implement match detail modal/page
  console.log('View match:', match)
}

async function loadMatches() {
  const params = {}
  if (filters.gameMode !== '') params.gameMode = filters.gameMode
  if (filters.isRanked !== '') params.isRanked = filters.isRanked
  if (filters.date) params.date = filters.date
  
  await matchStore.fetchMatches(params)
}

function resetFilters() {
  filters.gameMode = ''
  filters.isRanked = ''
  filters.date = ''
  loadMatches()
}

function changePage(page) {
  if (page < 1 || page > pagination.value.totalPages) return
  matchStore.setPage(page)
  loadMatches()
}

// Lifecycle
onMounted(async () => {
  await Promise.all([
    loadMatches(),
    memberStore.fetchMembers(),
    challengeStore.fetchOpenChallenges()
  ])
})
</script>
