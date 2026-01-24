/**
 * Challenge Store (Pinia)
 * Quản lý trạng thái kèo đấu
 */
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { challengesApi } from '@/api'
import { useToast } from 'vue-toastification'
import config from '@/config/app.config'

export const useChallengeStore = defineStore('challenge', () => {
  const toast = useToast()

  // ==================== STATE ====================
  const challenges = ref([])
  const myChallenges = ref([])
  const openChallenges = ref([])
  const currentChallenge = ref(null)
  const participants = ref([])
  const openCount = ref(0)
  const pagination = ref({
    page: 1,
    pageSize: 10,
    totalItems: 0,
    totalPages: 0
  })
  const isLoading = ref(false)
  const error = ref(null)

  // ==================== GETTERS ====================
  const totalChallenges = computed(() => pagination.value.totalItems)
  
  const singleChallenges = computed(() => 
    challenges.value.filter(c => c.type === config.CHALLENGE_TYPE.SINGLE)
  )
  
  const doubleChallenges = computed(() => 
    challenges.value.filter(c => c.type === config.CHALLENGE_TYPE.DOUBLE)
  )

  // ==================== ACTIONS ====================

  async function fetchChallenges(params = {}) {
    isLoading.value = true
    error.value = null

    try {
      const response = await challengesApi.getAll({
        page: pagination.value.page,
        pageSize: pagination.value.pageSize,
        ...params
      })

      const data = response.data
      challenges.value = data.items || data

      if (data.pagination) {
        pagination.value = { ...pagination.value, ...data.pagination }
      }

      return challenges.value
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể tải danh sách kèo'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function fetchOpenChallenges() {
    try {
      const response = await challengesApi.getOpenChallenges()
      openChallenges.value = response.data
      return openChallenges.value
    } catch (err) {
      console.error('Failed to fetch open challenges:', err)
      throw err
    }
  }

  async function fetchOpenCount() {
    try {
      const response = await challengesApi.getOpenCount()
      openCount.value = response.data.count || response.data
      return openCount.value
    } catch (err) {
      console.error('Failed to fetch open count:', err)
      return 0
    }
  }

  async function fetchChallengeById(id) {
    isLoading.value = true
    error.value = null

    try {
      const response = await challengesApi.getById(id)
      currentChallenge.value = response.data
      return currentChallenge.value
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể tải thông tin kèo'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function fetchParticipants(challengeId) {
    try {
      const response = await challengesApi.getParticipants(challengeId)
      participants.value = response.data
      return participants.value
    } catch (err) {
      console.error('Failed to fetch participants:', err)
      throw err
    }
  }

  async function createChallenge(challengeData) {
    isLoading.value = true
    error.value = null

    try {
      const response = await challengesApi.create(challengeData)
      challenges.value.unshift(response.data)
      openChallenges.value.unshift(response.data)
      openCount.value++
      toast.success('Tạo kèo thành công!')
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể tạo kèo'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function joinChallenge(id) {
    isLoading.value = true

    try {
      const response = await challengesApi.join(id)
      
      // Cập nhật participants
      if (currentChallenge.value?.id === id) {
        await fetchParticipants(id)
      }

      toast.success('Tham gia kèo thành công!')
      return response.data
    } catch (err) {
      toast.error(err.response?.data?.message || 'Không thể tham gia kèo')
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function leaveChallenge(id) {
    isLoading.value = true

    try {
      await challengesApi.leave(id)
      
      if (currentChallenge.value?.id === id) {
        await fetchParticipants(id)
      }

      toast.success('Đã rời khỏi kèo!')
    } catch (err) {
      toast.error(err.response?.data?.message || 'Không thể rời kèo')
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function autoDivideTeams(id) {
    isLoading.value = true

    try {
      const response = await challengesApi.autoDivideTeams(id)
      
      if (currentChallenge.value?.id === id) {
        await fetchParticipants(id)
      }

      toast.success('Chia team thành công!')
      return response.data
    } catch (err) {
      toast.error(err.response?.data?.message || 'Không thể chia team')
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function startChallenge(id) {
    isLoading.value = true

    try {
      await challengesApi.start(id)
      
      // Cập nhật status
      const challenge = challenges.value.find(c => c.id === id)
      if (challenge) challenge.status = config.CHALLENGE_STATUS.IN_PROGRESS
      
      if (currentChallenge.value?.id === id) {
        currentChallenge.value.status = config.CHALLENGE_STATUS.IN_PROGRESS
      }

      // Xóa khỏi open challenges
      openChallenges.value = openChallenges.value.filter(c => c.id !== id)
      openCount.value = Math.max(0, openCount.value - 1)

      toast.success('Kèo đã bắt đầu!')
    } catch (err) {
      toast.error(err.response?.data?.message || 'Không thể bắt đầu kèo')
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function completeChallenge(id, resultData) {
    isLoading.value = true

    try {
      await challengesApi.complete(id, resultData)
      
      const challenge = challenges.value.find(c => c.id === id)
      if (challenge) challenge.status = config.CHALLENGE_STATUS.COMPLETED

      toast.success('Kết thúc kèo thành công!')
    } catch (err) {
      toast.error(err.response?.data?.message || 'Không thể kết thúc kèo')
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function deleteChallenge(id) {
    isLoading.value = true

    try {
      await challengesApi.delete(id)
      challenges.value = challenges.value.filter(c => c.id !== id)
      openChallenges.value = openChallenges.value.filter(c => c.id !== id)
      toast.success('Xóa kèo thành công!')
    } catch (err) {
      toast.error(err.response?.data?.message || 'Không thể xóa kèo')
      throw err
    } finally {
      isLoading.value = false
    }
  }

  function setPage(page) {
    pagination.value.page = page
  }

  function $reset() {
    challenges.value = []
    myChallenges.value = []
    openChallenges.value = []
    currentChallenge.value = null
    participants.value = []
    openCount.value = 0
    pagination.value = { page: 1, pageSize: 10, totalItems: 0, totalPages: 0 }
    isLoading.value = false
    error.value = null
  }

  return {
    challenges,
    myChallenges,
    openChallenges,
    currentChallenge,
    participants,
    openCount,
    pagination,
    isLoading,
    error,
    totalChallenges,
    singleChallenges,
    doubleChallenges,
    fetchChallenges,
    fetchOpenChallenges,
    fetchOpenCount,
    fetchChallengeById,
    fetchParticipants,
    createChallenge,
    joinChallenge,
    leaveChallenge,
    autoDivideTeams,
    startChallenge,
    completeChallenge,
    deleteChallenge,
    setPage,
    $reset
  }
})
