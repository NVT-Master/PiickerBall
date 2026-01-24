/**
 * Match Store (Pinia)
 * Quản lý trạng thái trận đấu
 */
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { matchesApi } from '@/api'
import { useToast } from 'vue-toastification'

export const useMatchStore = defineStore('match', () => {
  const toast = useToast()

  // ==================== STATE ====================
  const matches = ref([])
  const recentMatches = ref([])
  const currentMatch = ref(null)
  const statistics = ref(null)
  const pagination = ref({
    page: 1,
    pageSize: 10,
    totalItems: 0,
    totalPages: 0
  })
  const isLoading = ref(false)
  const error = ref(null)

  // ==================== GETTERS ====================
  const totalMatches = computed(() => pagination.value.totalItems)

  // ==================== ACTIONS ====================

  async function fetchMatches(params = {}) {
    isLoading.value = true
    error.value = null

    try {
      const response = await matchesApi.getAll({
        page: pagination.value.page,
        pageSize: pagination.value.pageSize,
        ...params
      })

      const data = response.data
      matches.value = data.items || data

      if (data.pagination) {
        pagination.value = { ...pagination.value, ...data.pagination }
      }

      return matches.value
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể tải danh sách trận đấu'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function fetchRecentMatches(limit = 10) {
    try {
      const response = await matchesApi.getRecent(limit)
      recentMatches.value = response.data
      return recentMatches.value
    } catch (err) {
      console.error('Failed to fetch recent matches:', err)
      return []
    }
  }

  async function fetchMatchById(id) {
    isLoading.value = true
    error.value = null

    try {
      const response = await matchesApi.getById(id)
      currentMatch.value = response.data
      return currentMatch.value
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể tải thông tin trận đấu'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function createMatch(matchData) {
    isLoading.value = true
    error.value = null

    try {
      const response = await matchesApi.create(matchData)
      matches.value.unshift(response.data)
      toast.success('Ghi nhận trận đấu thành công!')
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể ghi nhận trận đấu'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function updateMatch(id, matchData) {
    isLoading.value = true
    error.value = null

    try {
      const response = await matchesApi.update(id, matchData)
      
      const index = matches.value.findIndex(m => m.id === id)
      if (index !== -1) {
        matches.value[index] = response.data
      }

      toast.success('Cập nhật trận đấu thành công!')
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể cập nhật trận đấu'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function deleteMatch(id) {
    isLoading.value = true

    try {
      await matchesApi.delete(id)
      matches.value = matches.value.filter(m => m.id !== id)
      toast.success('Xóa trận đấu thành công!')
    } catch (err) {
      toast.error(err.response?.data?.message || 'Không thể xóa trận đấu')
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function fetchStatistics() {
    try {
      const response = await matchesApi.getStatistics()
      statistics.value = response.data
      return statistics.value
    } catch (err) {
      console.error('Failed to fetch match statistics:', err)
      return null
    }
  }

  function setPage(page) {
    pagination.value.page = page
  }

  function $reset() {
    matches.value = []
    recentMatches.value = []
    currentMatch.value = null
    statistics.value = null
    pagination.value = { page: 1, pageSize: 10, totalItems: 0, totalPages: 0 }
    isLoading.value = false
    error.value = null
  }

  return {
    matches,
    recentMatches,
    currentMatch,
    statistics,
    pagination,
    isLoading,
    error,
    totalMatches,
    fetchMatches,
    fetchRecentMatches,
    fetchMatchById,
    createMatch,
    updateMatch,
    deleteMatch,
    fetchStatistics,
    setPage,
    $reset
  }
})
