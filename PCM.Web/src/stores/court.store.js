/**
 * Court Store (Pinia)
 * Quản lý trạng thái sân
 */
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { courtsApi } from '@/api'
import { useToast } from 'vue-toastification'

export const useCourtStore = defineStore('court', () => {
  const toast = useToast()

  // ==================== STATE ====================
  const courts = ref([])
  const activeCourts = ref([])
  const currentCourt = ref(null)
  const isLoading = ref(false)
  const error = ref(null)

  // ==================== GETTERS ====================
  const totalCourts = computed(() => courts.value.length)
  const activeCount = computed(() => activeCourts.value.length)

  // ==================== ACTIONS ====================

  async function fetchCourts(params = {}) {
    isLoading.value = true
    error.value = null

    try {
      const response = await courtsApi.getAll(params)
      courts.value = response.data.items || response.data
      return courts.value
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể tải danh sách sân'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function fetchActiveCourts() {
    try {
      const response = await courtsApi.getActiveList()
      activeCourts.value = response.data
      return activeCourts.value
    } catch (err) {
      console.error('Failed to fetch active courts:', err)
      // Fallback: filter từ danh sách đã có
      activeCourts.value = courts.value.filter(c => c.isActive)
      return activeCourts.value
    }
  }

  async function fetchCourtById(id) {
    isLoading.value = true
    error.value = null

    try {
      const response = await courtsApi.getById(id)
      currentCourt.value = response.data
      return currentCourt.value
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể tải thông tin sân'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function createCourt(courtData) {
    isLoading.value = true
    error.value = null

    try {
      const response = await courtsApi.create(courtData)
      courts.value.unshift(response.data)
      if (response.data.isActive) {
        activeCourts.value.push(response.data)
      }
      toast.success('Tạo sân thành công!')
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể tạo sân'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function updateCourt(id, courtData) {
    isLoading.value = true
    error.value = null

    try {
      const response = await courtsApi.update(id, courtData)
      
      const index = courts.value.findIndex(c => c.id === id)
      if (index !== -1) {
        courts.value[index] = response.data
      }
      
      // Cập nhật active courts
      await fetchActiveCourts()

      toast.success('Cập nhật sân thành công!')
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể cập nhật sân'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function deleteCourt(id) {
    isLoading.value = true
    error.value = null

    try {
      await courtsApi.delete(id)
      courts.value = courts.value.filter(c => c.id !== id)
      activeCourts.value = activeCourts.value.filter(c => c.id !== id)
      toast.success('Xóa sân thành công!')
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể xóa sân'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  function $reset() {
    courts.value = []
    activeCourts.value = []
    currentCourt.value = null
    isLoading.value = false
    error.value = null
  }

  return {
    courts,
    activeCourts,
    currentCourt,
    isLoading,
    error,
    totalCourts,
    activeCount,
    fetchCourts,
    fetchActiveCourts,
    fetchCourtById,
    createCourt,
    updateCourt,
    deleteCourt,
    $reset
  }
})
