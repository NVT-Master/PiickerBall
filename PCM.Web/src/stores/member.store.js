/**
 * Member Store (Pinia)
 * Quản lý trạng thái thành viên với caching
 */
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { membersApi } from '@/api'
import { useToast } from 'vue-toastification'

export const useMemberStore = defineStore('member', () => {
  const toast = useToast()

  // ==================== STATE ====================
  const members = ref([])
  const currentMember = ref(null)
  const topRanking = ref([])
  const pagination = ref({
    page: 1,
    pageSize: 10,
    totalItems: 0,
    totalPages: 0
  })
  const isLoading = ref(false)
  const error = ref(null)
  
  // Cache timestamp để biết khi nào cần refresh
  const lastFetched = ref(null)
  const CACHE_DURATION = 5 * 60 * 1000 // 5 phút

  // ==================== GETTERS ====================
  const totalMembers = computed(() => pagination.value.totalItems)
  
  const isCacheValid = computed(() => {
    if (!lastFetched.value) return false
    return Date.now() - lastFetched.value < CACHE_DURATION
  })

  // ==================== ACTIONS ====================

  /**
   * Lấy danh sách members
   */
  async function fetchMembers(params = {}, forceRefresh = false) {
    // Sử dụng cache nếu còn hợp lệ
    if (!forceRefresh && isCacheValid.value && members.value.length > 0) {
      return members.value
    }

    isLoading.value = true
    error.value = null

    try {
      const response = await membersApi.getAll({
        page: pagination.value.page,
        pageSize: pagination.value.pageSize,
        ...params
      })

      const data = response.data
      members.value = data.items || data
      
      if (data.pagination) {
        pagination.value = { ...pagination.value, ...data.pagination }
      } else if (data.totalItems !== undefined) {
        pagination.value.totalItems = data.totalItems
        pagination.value.totalPages = Math.ceil(data.totalItems / pagination.value.pageSize)
      }

      lastFetched.value = Date.now()
      return members.value
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể tải danh sách thành viên'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Lấy thông tin một member
   */
  async function fetchMemberById(id) {
    isLoading.value = true
    error.value = null

    try {
      const response = await membersApi.getById(id)
      currentMember.value = response.data
      return currentMember.value
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể tải thông tin thành viên'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Tạo member mới
   */
  async function createMember(memberData) {
    isLoading.value = true
    error.value = null

    try {
      const response = await membersApi.create(memberData)
      members.value.unshift(response.data)
      toast.success('Tạo thành viên thành công!')
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể tạo thành viên'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Cập nhật member
   */
  async function updateMember(id, memberData) {
    isLoading.value = true
    error.value = null

    try {
      const response = await membersApi.update(id, memberData)
      
      // Cập nhật trong danh sách
      const index = members.value.findIndex(m => m.id === id)
      if (index !== -1) {
        members.value[index] = response.data
      }
      
      // Cập nhật current member nếu đang xem
      if (currentMember.value?.id === id) {
        currentMember.value = response.data
      }

      toast.success('Cập nhật thành công!')
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể cập nhật thành viên'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Xóa member
   */
  async function deleteMember(id) {
    isLoading.value = true
    error.value = null

    try {
      await membersApi.delete(id)
      members.value = members.value.filter(m => m.id !== id)
      toast.success('Xóa thành viên thành công!')
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể xóa thành viên'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Lấy top ranking
   */
  async function fetchTopRanking(limit = 5) {
    try {
      const response = await membersApi.getTopRanking(limit)
      topRanking.value = response.data
      return topRanking.value
    } catch (err) {
      console.error('Failed to fetch top ranking:', err)
      return []
    }
  }

  /**
   * Set page
   */
  function setPage(page) {
    pagination.value.page = page
  }

  /**
   * Set page size
   */
  function setPageSize(size) {
    pagination.value.pageSize = size
    pagination.value.page = 1
  }

  /**
   * Clear cache
   */
  function clearCache() {
    lastFetched.value = null
  }

  /**
   * Reset store
   */
  function $reset() {
    members.value = []
    currentMember.value = null
    topRanking.value = []
    pagination.value = { page: 1, pageSize: 10, totalItems: 0, totalPages: 0 }
    isLoading.value = false
    error.value = null
    lastFetched.value = null
  }

  return {
    // State
    members,
    currentMember,
    topRanking,
    pagination,
    isLoading,
    error,
    
    // Getters
    totalMembers,
    isCacheValid,
    
    // Actions
    fetchMembers,
    fetchMemberById,
    createMember,
    updateMember,
    deleteMember,
    fetchTopRanking,
    setPage,
    setPageSize,
    clearCache,
    $reset
  }
})
