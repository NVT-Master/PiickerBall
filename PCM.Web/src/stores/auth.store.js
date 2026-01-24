/**
 * Auth Store (Pinia)
 * Quản lý trạng thái xác thực người dùng
 */
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi } from '@/api'
import config from '@/config/app.config'
import router from '@/router'
import { useToast } from 'vue-toastification'

export const useAuthStore = defineStore('auth', () => {
  const toast = useToast()
  
  // ==================== STATE ====================
  const user = ref(null)
  const token = ref(null)
  const roles = ref([])
  const isLoading = ref(false)
  const error = ref(null)

  // ==================== GETTERS ====================
  const isAuthenticated = computed(() => !!token.value)
  
  const isAdmin = computed(() => roles.value.includes(config.ROLES.ADMIN))
  const isTreasurer = computed(() => roles.value.includes(config.ROLES.TREASURER))
  const isReferee = computed(() => roles.value.includes(config.ROLES.REFEREE))
  const isMember = computed(() => roles.value.includes(config.ROLES.MEMBER))
  
  // Admin hoặc Treasurer có thể xem quỹ
  const canViewTreasury = computed(() => isAdmin.value || isTreasurer.value)
  
  // Admin hoặc Referee có thể ghi nhận trận đấu
  const canManageMatches = computed(() => isAdmin.value || isReferee.value)
  
  const userFullName = computed(() => user.value?.fullName || user.value?.email || 'User')
  const userInitials = computed(() => {
    const name = userFullName.value
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2)
  })

  // ==================== ACTIONS ====================
  
  /**
   * Khởi tạo auth state từ localStorage
   */
  function initializeAuth() {
    const storedToken = localStorage.getItem(config.STORAGE_KEYS.TOKEN)
    const storedUser = localStorage.getItem(config.STORAGE_KEYS.USER)
    const storedRoles = localStorage.getItem(config.STORAGE_KEYS.ROLES)

    if (storedToken && storedUser) {
      token.value = storedToken
      user.value = JSON.parse(storedUser)
      roles.value = storedRoles ? JSON.parse(storedRoles) : []
    }
  }

  /**
   * Đăng nhập
   * @param {Object} credentials - { email, password }
   */
  async function login(credentials) {
    isLoading.value = true
    error.value = null

    try {
      const response = await authApi.login(credentials)
      const data = response.data

      // Lưu vào state
      token.value = data.token
      user.value = data.user || { email: credentials.email }
      roles.value = data.roles || []

      // Lưu vào localStorage
      localStorage.setItem(config.STORAGE_KEYS.TOKEN, data.token)
      localStorage.setItem(config.STORAGE_KEYS.USER, JSON.stringify(user.value))
      localStorage.setItem(config.STORAGE_KEYS.ROLES, JSON.stringify(roles.value))

      toast.success('Đăng nhập thành công!')
      
      // Redirect về trang trước đó hoặc dashboard
      const redirectPath = router.currentRoute.value.query.redirect || '/dashboard'
      router.push(redirectPath)

      return { success: true }
    } catch (err) {
      const message = err.response?.data?.message || 'Đăng nhập thất bại'
      error.value = message
      toast.error(message)
      return { success: false, error: message }
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Đăng ký
   * @param {Object} userData
   */
  async function register(userData) {
    isLoading.value = true
    error.value = null

    try {
      await authApi.register(userData)
      toast.success('Đăng ký thành công! Vui lòng đăng nhập.')
      router.push({ name: 'Login' })
      return { success: true }
    } catch (err) {
      const message = err.response?.data?.message || 'Đăng ký thất bại'
      error.value = message
      toast.error(message)
      return { success: false, error: message }
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Đăng xuất
   */
  async function logout() {
    try {
      // Gọi API logout (optional - để invalidate token phía server)
      await authApi.logout().catch(() => {})
    } finally {
      // Clear state
      token.value = null
      user.value = null
      roles.value = []
      error.value = null

      // Clear localStorage
      localStorage.removeItem(config.STORAGE_KEYS.TOKEN)
      localStorage.removeItem(config.STORAGE_KEYS.REFRESH_TOKEN)
      localStorage.removeItem(config.STORAGE_KEYS.USER)
      localStorage.removeItem(config.STORAGE_KEYS.ROLES)

      toast.info('Đã đăng xuất')
      router.push({ name: 'Login' })
    }
  }

  /**
   * Lấy thông tin user hiện tại từ server
   */
  async function fetchCurrentUser() {
    if (!token.value) return

    try {
      const response = await authApi.getCurrentUser()
      user.value = response.data.user
      roles.value = response.data.roles || []
      
      localStorage.setItem(config.STORAGE_KEYS.USER, JSON.stringify(user.value))
      localStorage.setItem(config.STORAGE_KEYS.ROLES, JSON.stringify(roles.value))
    } catch (err) {
      console.error('Failed to fetch current user:', err)
    }
  }

  /**
   * Đổi mật khẩu
   */
  async function changePassword(passwords) {
    isLoading.value = true
    error.value = null

    try {
      await authApi.changePassword(passwords)
      toast.success('Đổi mật khẩu thành công!')
      return { success: true }
    } catch (err) {
      const message = err.response?.data?.message || 'Đổi mật khẩu thất bại'
      error.value = message
      toast.error(message)
      return { success: false, error: message }
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Kiểm tra user có role nào đó không
   * @param {string|string[]} requiredRoles
   */
  function hasRole(requiredRoles) {
    if (!requiredRoles) return true
    
    const roleArray = Array.isArray(requiredRoles) ? requiredRoles : [requiredRoles]
    return roleArray.some(role => roles.value.includes(role))
  }

  /**
   * Kiểm tra user có tất cả các roles không
   * @param {string[]} requiredRoles
   */
  function hasAllRoles(requiredRoles) {
    return requiredRoles.every(role => roles.value.includes(role))
  }

  return {
    // State
    user,
    token,
    roles,
    isLoading,
    error,
    
    // Getters
    isAuthenticated,
    isAdmin,
    isTreasurer,
    isReferee,
    isMember,
    canViewTreasury,
    canManageMatches,
    userFullName,
    userInitials,
    
    // Actions
    initializeAuth,
    login,
    register,
    logout,
    fetchCurrentUser,
    changePassword,
    hasRole,
    hasAllRoles
  }
})
