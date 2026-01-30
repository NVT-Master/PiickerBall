<template>
  <div class="dashboard">
    <!-- Page Header -->
    <div class="page-header">
      <h1>Dashboard</h1>
      <p class="text-muted mb-0">Chào mừng trở lại, {{ authStore.userFullName }}!</p>
    </div>

    <!-- Loading State -->
    <LoadingSpinner v-if="isLoading" />

    <template v-else>
      <!-- Stats Cards -->
      <div class="row g-4 mb-4">
        <!-- Treasury Balance (Admin/Treasurer only) -->
        <div class="col-sm-6 col-xl-3" v-if="authStore.canViewTreasury">
          <div class="stat-card" :class="treasurySummary.balance < 0 ? 'bg-danger' : 'bg-primary'">
            <div class="stat-icon">
              <i class="bi bi-wallet2"></i>
            </div>
            <div class="stat-value">{{ formatMoney(treasurySummary.balance) }}</div>
            <div class="stat-label">
              Số dư quỹ
              <span v-if="treasurySummary.balance < 0" class="ms-1">
                <i class="bi bi-exclamation-triangle-fill"></i> Cảnh báo!
              </span>
            </div>
          </div>
        </div>

        <!-- Open Challenges -->
        <div class="col-sm-6 col-xl-3">
          <div class="stat-card bg-secondary">
            <div class="stat-icon">
              <i class="bi bi-trophy"></i>
            </div>
            <div class="stat-value">{{ openChallengesCount }}</div>
            <div class="stat-label">Kèo đang mở</div>
          </div>
        </div>

        <!-- Total Members -->
        <div class="col-sm-6 col-xl-3">
          <div class="stat-card bg-success">
            <div class="stat-icon">
              <i class="bi bi-people"></i>
            </div>
            <div class="stat-value">{{ totalMembers }}</div>
            <div class="stat-label">Thành viên</div>
          </div>
        </div>

        <!-- Today's Bookings -->
        <div class="col-sm-6 col-xl-3">
          <div class="stat-card bg-info">
            <div class="stat-icon">
              <i class="bi bi-calendar-check"></i>
            </div>
            <div class="stat-value">{{ todayBookings }}</div>
            <div class="stat-label">Lượt đặt sân hôm nay</div>
          </div>
        </div>
      </div>

      <div class="row g-4">
        <!-- Top Ranking -->
        <div class="col-lg-6">
          <div class="card h-100">
            <div class="card-header d-flex justify-content-between align-items-center">
              <span><i class="bi bi-bar-chart me-2"></i>Bảng xếp hạng Top 5</span>
              <router-link to="/members" class="btn btn-sm btn-outline-primary">
                Xem tất cả
              </router-link>
            </div>
            <div class="card-body p-0">
              <div class="list-group list-group-flush">
                <div 
                  v-for="(member, index) in topRanking" 
                  :key="member.id"
                  class="list-group-item d-flex align-items-center py-3"
                >
                  <div class="rank-badge me-3" :class="getRankClass(index + 1)">
                    {{ index + 1 }}
                  </div>
                  <div class="avatar me-3">
                    {{ getInitials(member.fullName) }}
                  </div>
                  <div class="flex-grow-1">
                    <h6 class="mb-0">{{ member.fullName }}</h6>
                    <small class="text-muted">{{ member.wins }}W - {{ member.losses }}L</small>
                  </div>
                  <div class="text-end">
                    <span class="badge bg-primary fs-6">{{ member.rankingPoints }} pts</span>
                  </div>
                </div>
                <div v-if="topRanking.length === 0" class="list-group-item text-center py-4 text-muted">
                  Chưa có dữ liệu xếp hạng
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Open Challenges -->
        <div class="col-lg-6">
          <div class="card h-100">
            <div class="card-header d-flex justify-content-between align-items-center">
              <span><i class="bi bi-trophy me-2"></i>Kèo đang mở</span>
              <router-link to="/challenges" class="btn btn-sm btn-outline-primary">
                Xem tất cả
              </router-link>
            </div>
            <div class="card-body">
              <div class="row g-3">
                <div 
                  v-for="challenge in openChallenges.slice(0, 4)" 
                  :key="challenge.id"
                  class="col-sm-6"
                >
                  <router-link 
                    :to="`/challenges/${challenge.id}`"
                    class="text-decoration-none"
                  >
                    <div class="challenge-mini-card">
                      <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="badge" :class="challenge.type === 0 ? 'bg-secondary' : 'bg-primary'">
                          {{ challenge.type === 0 ? 'Đơn' : 'Đôi' }}
                        </span>
                        <span class="text-warning fw-bold">
                          <i class="bi bi-coin me-1"></i>
                          {{ formatMoney(challenge.prizePool) }}
                        </span>
                      </div>
                      <h6 class="mb-1 text-dark">{{ challenge.title || 'Kèo #' + challenge.id }}</h6>
                      <small class="text-muted">
                        <i class="bi bi-people me-1"></i>
                        {{ challenge.currentParticipants || 0 }}/{{ challenge.maxParticipants }}
                      </small>
                    </div>
                  </router-link>
                </div>
                <div v-if="openChallenges.length === 0" class="col-12 text-center py-4 text-muted">
                  Không có kèo nào đang mở
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Pinned News -->
        <div class="col-lg-8">
          <div class="card">
            <div class="card-header d-flex justify-content-between align-items-center">
              <span><i class="bi bi-newspaper me-2"></i>Tin tức & Thông báo</span>
              <router-link to="/news" class="btn btn-sm btn-outline-primary">
                Xem tất cả
              </router-link>
            </div>
            <div class="card-body">
              <div class="list-group list-group-flush">
                <router-link 
                  v-for="news in pinnedNews" 
                  :key="news.id"
                  :to="`/news/${news.id}`"
                  class="list-group-item list-group-item-action border-0 px-0"
                >
                  <div class="d-flex align-items-start">
                    <div class="news-icon me-3">
                      <i class="bi bi-pin-angle-fill text-danger" v-if="news.isPinned"></i>
                      <i class="bi bi-newspaper" v-else></i>
                    </div>
                    <div class="flex-grow-1">
                      <h6 class="mb-1">{{ news.title }}</h6>
                      <p class="text-muted mb-1 small line-clamp-2">{{ news.summary || news.content }}</p>
                      <small class="text-muted">
                        <i class="bi bi-clock me-1"></i>
                        {{ formatDate(news.createdAt) }}
                      </small>
                    </div>
                  </div>
                </router-link>
                <div v-if="pinnedNews.length === 0" class="text-center py-4 text-muted">
                  Chưa có tin tức
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Quick Actions -->
        <div class="col-lg-4">
          <div class="card">
            <div class="card-header">
              <i class="bi bi-lightning me-2"></i>Thao tác nhanh
            </div>
            <div class="card-body">
              <div class="d-grid gap-2">
                <router-link to="/bookings" class="btn btn-outline-primary">
                  <i class="bi bi-calendar-plus me-2"></i>Đặt sân
                </router-link>
                <router-link to="/challenges" class="btn btn-outline-success">
                  <i class="bi bi-trophy me-2"></i>Tham gia kèo
                </router-link>
                <router-link to="/my-profile" class="btn btn-outline-secondary">
                  <i class="bi bi-person me-2"></i>Cập nhật profile
                </router-link>
                <router-link 
                  v-if="authStore.canManageMatches" 
                  to="/matches" 
                  class="btn btn-outline-info"
                >
                  <i class="bi bi-controller me-2"></i>Ghi nhận trận đấu
                </router-link>
              </div>
            </div>
          </div>

          <!-- Treasury Summary (Admin/Treasurer) -->
          <div class="card mt-4" v-if="authStore.canViewTreasury">
            <div class="card-header">
              <i class="bi bi-graph-up me-2"></i>Tổng kết quỹ
            </div>
            <div class="card-body">
              <div class="d-flex justify-content-between mb-2">
                <span class="text-muted">Thu:</span>
                <span class="text-success fw-bold">+{{ formatMoney(treasurySummary.totalIncome) }}</span>
              </div>
              <div class="d-flex justify-content-between mb-2">
                <span class="text-muted">Chi:</span>
                <span class="text-danger fw-bold">-{{ formatMoney(treasurySummary.totalExpense) }}</span>
              </div>
              <hr>
              <div class="d-flex justify-content-between">
                <span class="fw-bold">Còn lại:</span>
                <span 
                  class="fw-bold"
                  :class="treasurySummary.balance >= 0 ? 'text-success' : 'text-danger'"
                >
                  {{ formatMoney(treasurySummary.balance) }}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth.store'
import { useMemberStore } from '@/stores/member.store'
import { useChallengeStore } from '@/stores/challenge.store'
import { useTransactionStore } from '@/stores/transaction.store'
import { useNewsStore } from '@/stores/news.store'
import axios from '@/api/axios'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import dayjs from 'dayjs'

const authStore = useAuthStore()
const memberStore = useMemberStore()
const challengeStore = useChallengeStore()
const transactionStore = useTransactionStore()
const newsStore = useNewsStore()

const isLoading = ref(true)
const topRanking = ref([])
const openChallenges = ref([])
const pinnedNews = ref([])
const treasurySummary = ref({ totalIncome: 0, totalExpense: 0, balance: 0 })
const openChallengesCount = ref(0)
const totalMembers = ref(0)
const todayBookings = ref(0)

onMounted(async () => {
  try {
    // Fetch data in parallel
    const promises = [
      memberStore.fetchTopRanking(5),
      challengeStore.fetchOpenChallenges(),
      newsStore.fetchPinnedNews(),
      axios.get('/dashboard/statistics')
    ]

    const results = await Promise.allSettled(promises)

    // Assign results
    if (results[0].status === 'fulfilled') {
      topRanking.value = results[0].value || []
    }
    if (results[1].status === 'fulfilled') {
      openChallenges.value = results[1].value || []
    }
    if (results[2].status === 'fulfilled') {
      pinnedNews.value = results[2].value || []
    }
    
    // Dashboard statistics từ API mới
    if (results[3].status === 'fulfilled') {
      const stats = results[3].value?.data || results[3].value
      totalMembers.value = stats?.totalMembers || 0
      todayBookings.value = stats?.todayBookings || 0
      openChallengesCount.value = stats?.openChallenges || 0
      if (stats?.treasury) {
        treasurySummary.value = stats.treasury
      }
    }

  } catch (error) {
    console.error('Failed to load dashboard data:', error)
  } finally {
    isLoading.value = false
  }
})

function formatMoney(amount) {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(amount || 0)
}

function formatDate(date) {
  return dayjs(date).format('DD/MM/YYYY HH:mm')
}

function getInitials(name) {
  if (!name) return '?'
  return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
}

function getRankClass(rank) {
  if (rank === 1) return 'rank-gold'
  if (rank === 2) return 'rank-silver'
  if (rank === 3) return 'rank-bronze'
  return ''
}
</script>

<style scoped>
.rank-badge {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  background: #e2e8f0;
  color: #64748b;
}

.rank-gold {
  background: linear-gradient(135deg, #fbbf24, #f59e0b);
  color: white;
}

.rank-silver {
  background: linear-gradient(135deg, #94a3b8, #64748b);
  color: white;
}

.rank-bronze {
  background: linear-gradient(135deg, #d97706, #b45309);
  color: white;
}

.challenge-mini-card {
  padding: 1rem;
  border-radius: 0.5rem;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  transition: all 0.2s ease;
}

.challenge-mini-card:hover {
  background: white;
  border-color: var(--pcm-primary);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.news-icon {
  width: 40px;
  height: 40px;
  border-radius: 0.5rem;
  background: #f1f5f9;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.25rem;
}

.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
