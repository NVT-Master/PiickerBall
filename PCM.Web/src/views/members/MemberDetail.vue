<template>
  <div class="member-detail">
    <LoadingSpinner v-if="isLoading" />

    <template v-else-if="member">
      <!-- Header -->
      <div class="page-header d-flex justify-content-between align-items-start mb-4">
        <div class="d-flex align-items-center">
          <router-link to="/members" class="btn btn-link text-muted p-0 me-3">
            <i class="bi bi-arrow-left fs-4"></i>
          </router-link>
          <div>
            <h1 class="mb-1">{{ member.fullName }}</h1>
            <p class="text-muted mb-0">{{ member.email }}</p>
          </div>
        </div>
        <div v-if="canEdit">
          <button class="btn btn-primary" @click="showEditModal = true">
            <i class="bi bi-pencil me-2"></i>Chỉnh sửa
          </button>
        </div>
      </div>

      <div class="row g-4">
        <!-- Profile Card -->
        <div class="col-lg-4">
          <div class="card">
            <div class="card-body text-center">
              <div class="avatar avatar-lg mx-auto mb-3" style="width: 100px; height: 100px; font-size: 2.5rem;">
                {{ getInitials(member.fullName) }}
              </div>
              <h4 class="mb-1">{{ member.fullName }}</h4>
              <p class="text-muted mb-3">{{ member.email }}</p>
              
              <div class="d-flex justify-content-center gap-2 mb-3">
                <span 
                  v-for="role in (member.roles || ['Member'])" 
                  :key="role"
                  class="badge"
                  :class="getRoleBadgeClass(role)"
                >
                  {{ role }}
                </span>
              </div>

              <span 
                class="badge"
                :class="member.isActive !== false ? 'bg-success' : 'bg-secondary'"
              >
                {{ member.isActive !== false ? 'Đang hoạt động' : 'Ngưng hoạt động' }}
              </span>
            </div>
          </div>

          <!-- Stats Card -->
          <div class="card mt-4">
            <div class="card-header">
              <i class="bi bi-bar-chart me-2"></i>Thống kê
            </div>
            <div class="card-body">
              <div class="stat-row">
                <span>Điểm xếp hạng</span>
                <span class="badge bg-primary fs-6">{{ member.rankingPoints || 0 }}</span>
              </div>
              <hr>
              <div class="stat-row">
                <span>Tổng trận</span>
                <span>{{ (member.wins || 0) + (member.losses || 0) }}</span>
              </div>
              <div class="stat-row">
                <span class="text-success">Thắng</span>
                <span class="text-success fw-bold">{{ member.wins || 0 }}</span>
              </div>
              <div class="stat-row">
                <span class="text-danger">Thua</span>
                <span class="text-danger fw-bold">{{ member.losses || 0 }}</span>
              </div>
              <hr>
              <div class="stat-row">
                <span>Tỷ lệ thắng</span>
                <span class="fw-bold">{{ winRate }}%</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Details -->
        <div class="col-lg-8">
          <!-- Info Card -->
          <div class="card">
            <div class="card-header">
              <i class="bi bi-person me-2"></i>Thông tin cá nhân
            </div>
            <div class="card-body">
              <div class="row g-3">
                <div class="col-sm-6">
                  <label class="text-muted small">Số điện thoại</label>
                  <p class="mb-0">{{ member.phone || 'Chưa cập nhật' }}</p>
                </div>
                <div class="col-sm-6">
                  <label class="text-muted small">Ngày sinh</label>
                  <p class="mb-0">{{ member.dateOfBirth ? formatDate(member.dateOfBirth) : 'Chưa cập nhật' }}</p>
                </div>
                <div class="col-12">
                  <label class="text-muted small">Địa chỉ</label>
                  <p class="mb-0">{{ member.address || 'Chưa cập nhật' }}</p>
                </div>
                <div class="col-sm-6">
                  <label class="text-muted small">Ngày tham gia</label>
                  <p class="mb-0">{{ formatDate(member.createdAt) }}</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Match History -->
          <div class="card mt-4">
            <div class="card-header d-flex justify-content-between align-items-center">
              <span><i class="bi bi-clock-history me-2"></i>Lịch sử trận đấu gần đây</span>
            </div>
            <div class="card-body p-0">
              <div class="list-group list-group-flush">
                <div 
                  v-for="match in matchHistory" 
                  :key="match.id"
                  class="list-group-item"
                >
                  <div class="d-flex justify-content-between align-items-center">
                    <div>
                      <span class="badge me-2" :class="match.isWin ? 'bg-success' : 'bg-danger'">
                        {{ match.isWin ? 'THẮNG' : 'THUA' }}
                      </span>
                      <span>vs {{ match.opponent }}</span>
                    </div>
                    <div class="text-end">
                      <span class="fw-bold">{{ match.score }}</span>
                      <br>
                      <small class="text-muted">{{ formatDate(match.playedAt) }}</small>
                    </div>
                  </div>
                </div>
                <div v-if="matchHistory.length === 0" class="list-group-item text-center py-4 text-muted">
                  Chưa có lịch sử trận đấu
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>

    <template v-else>
      <div class="text-center py-5">
        <i class="bi bi-person-x text-muted" style="font-size: 4rem;"></i>
        <h4 class="mt-3">Không tìm thấy thành viên</h4>
        <router-link to="/members" class="btn btn-primary mt-3">
          Quay lại danh sách
        </router-link>
      </div>
    </template>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'
import { useMemberStore } from '@/stores/member.store'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import dayjs from 'dayjs'

const route = useRoute()
const authStore = useAuthStore()
const memberStore = useMemberStore()

const isLoading = ref(true)
const member = ref(null)
const matchHistory = ref([])
const showEditModal = ref(false)

const canEdit = computed(() => {
  return authStore.isAdmin || member.value?.id === authStore.user?.id
})

const winRate = computed(() => {
  if (!member.value) return 0
  const total = (member.value.wins || 0) + (member.value.losses || 0)
  if (total === 0) return 0
  return Math.round((member.value.wins / total) * 100)
})

onMounted(async () => {
  const memberId = route.params.id
  
  try {
    member.value = await memberStore.fetchMemberById(memberId)
    // Fetch match history
    // matchHistory.value = await membersApi.getMatchHistory(memberId)
    
    // Mock data for demo
    matchHistory.value = [
      { id: 1, opponent: 'Nguyễn Văn A', score: '21-15', isWin: true, playedAt: '2026-01-20' },
      { id: 2, opponent: 'Trần Thị B', score: '18-21', isWin: false, playedAt: '2026-01-18' },
      { id: 3, opponent: 'Lê Văn C', score: '21-19', isWin: true, playedAt: '2026-01-15' }
    ]
  } catch (error) {
    console.error('Failed to load member:', error)
  } finally {
    isLoading.value = false
  }
})

function getInitials(name) {
  if (!name) return '?'
  return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
}

function formatDate(date) {
  return dayjs(date).format('DD/MM/YYYY')
}

function getRoleBadgeClass(role) {
  const classes = {
    Admin: 'bg-danger',
    Treasurer: 'bg-warning text-dark',
    Referee: 'bg-info',
    Member: 'bg-secondary'
  }
  return classes[role] || 'bg-secondary'
}
</script>

<style scoped>
.stat-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.5rem 0;
}
</style>
