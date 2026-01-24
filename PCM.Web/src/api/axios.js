/**
 * Axios Instance Configuration
 * Cấu hình interceptor cho request/response
 * Tự động gắn JWT token và xử lý lỗi 401
 */
import axios from 'axios'
import config from '@/config/app.config'
import router from '@/router'

// Tạo axios instance
const axiosInstance = axios.create({
  baseURL: config.API_BASE_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// ==================== REQUEST INTERCEPTOR ====================
axiosInstance.interceptors.request.use(
  (requestConfig) => {
    // Lấy token từ localStorage
    const token = localStorage.getItem(config.STORAGE_KEYS.TOKEN)
    
    // Nếu có token, gắn vào header Authorization
    if (token) {
      requestConfig.headers.Authorization = `Bearer ${token}`
    }
    
    // Log request (development only)
    if (import.meta.env.DEV) {
      console.log(`[API Request] ${requestConfig.method?.toUpperCase()} ${requestConfig.url}`)
    }
    
    return requestConfig
  },
  (error) => {
    console.error('[API Request Error]', error)
    return Promise.reject(error)
  }
)

// ==================== RESPONSE INTERCEPTOR ====================
axiosInstance.interceptors.response.use(
  (response) => {
    // Log response (development only)
    if (import.meta.env.DEV) {
      console.log(`[API Response] ${response.status} ${response.config.url}`)
    }
    
    return response
  },
  async (error) => {
    const originalRequest = error.config
    
    // Xử lý lỗi 401 Unauthorized
    if (error.response?.status === 401) {
      // Đánh dấu đã retry để tránh infinite loop
      if (!originalRequest._retry) {
        originalRequest._retry = true
        
        // ==================== REFRESH TOKEN LOGIC ====================
        // TODO: Uncomment và implement khi backend hỗ trợ refresh token
        /*
        const refreshToken = localStorage.getItem(config.STORAGE_KEYS.REFRESH_TOKEN)
        
        if (refreshToken) {
          try {
            // Gọi API refresh token
            const response = await axios.post(`${config.API_BASE_URL}/auth/refresh`, {
              refreshToken: refreshToken
            })
            
            const { token, newRefreshToken } = response.data
            
            // Lưu token mới
            localStorage.setItem(config.STORAGE_KEYS.TOKEN, token)
            localStorage.setItem(config.STORAGE_KEYS.REFRESH_TOKEN, newRefreshToken)
            
            // Cập nhật header và retry request
            originalRequest.headers.Authorization = `Bearer ${token}`
            return axiosInstance(originalRequest)
          } catch (refreshError) {
            // Refresh token cũng hết hạn -> logout
            console.error('[Refresh Token Failed]', refreshError)
          }
        }
        */
        // ==================== END REFRESH TOKEN ====================
        
        // Không có refresh token hoặc refresh failed -> Logout
        handleLogout()
      }
    }
    
    // Xử lý lỗi 403 Forbidden
    if (error.response?.status === 403) {
      router.push({ name: 'Forbidden' })
    }
    
    // Xử lý lỗi 404 Not Found
    if (error.response?.status === 404) {
      console.error('[API] Resource not found:', error.config.url)
    }
    
    // Xử lý lỗi 500 Server Error
    if (error.response?.status >= 500) {
      console.error('[API] Server error:', error.response.data)
    }
    
    // Log error (development only)
    if (import.meta.env.DEV) {
      console.error('[API Response Error]', {
        status: error.response?.status,
        url: error.config?.url,
        message: error.response?.data?.message || error.message
      })
    }
    
    return Promise.reject(error)
  }
)

/**
 * Xử lý logout khi token hết hạn
 */
function handleLogout() {
  // Xóa tất cả thông tin auth từ localStorage
  localStorage.removeItem(config.STORAGE_KEYS.TOKEN)
  localStorage.removeItem(config.STORAGE_KEYS.REFRESH_TOKEN)
  localStorage.removeItem(config.STORAGE_KEYS.USER)
  localStorage.removeItem(config.STORAGE_KEYS.ROLES)
  
  // Redirect về trang login
  router.push({ 
    name: 'Login',
    query: { 
      redirect: router.currentRoute.value.fullPath,
      expired: 'true'
    }
  })
}

export default axiosInstance
