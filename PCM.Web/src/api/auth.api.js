/**
 * Auth API Service
 * Xử lý các API liên quan đến xác thực
 */
import axios from './axios'

const AUTH_ENDPOINT = '/auth'

export const authApi = {
  /**
   * Đăng nhập
   * @param {Object} credentials - { email, password }
   * @returns {Promise} - { token, user, roles }
   */
  login(credentials) {
    return axios.post(`${AUTH_ENDPOINT}/login`, credentials)
  },

  /**
   * Đăng ký tài khoản mới
   * @param {Object} userData - { email, password, fullName, phone, ... }
   */
  register(userData) {
    return axios.post(`${AUTH_ENDPOINT}/register`, userData)
  },

  /**
   * Lấy thông tin user hiện tại
   */
  getCurrentUser() {
    return axios.get(`${AUTH_ENDPOINT}/me`)
  },

  /**
   * Đổi mật khẩu
   * @param {Object} passwords - { currentPassword, newPassword }
   */
  changePassword(passwords) {
    return axios.post(`${AUTH_ENDPOINT}/change-password`, passwords)
  },

  /**
   * Quên mật khẩu - Gửi email reset
   * @param {string} email
   */
  forgotPassword(email) {
    return axios.post(`${AUTH_ENDPOINT}/forgot-password`, { email })
  },

  /**
   * Reset mật khẩu
   * @param {Object} data - { token, email, newPassword }
   */
  resetPassword(data) {
    return axios.post(`${AUTH_ENDPOINT}/reset-password`, data)
  },

  /**
   * Đăng xuất
   */
  logout() {
    return axios.post(`${AUTH_ENDPOINT}/logout`)
  },

  /**
   * Refresh token (nếu backend hỗ trợ)
   * @param {string} refreshToken
   */
  refreshToken(refreshToken) {
    return axios.post(`${AUTH_ENDPOINT}/refresh`, { refreshToken })
  }
}

export default authApi
