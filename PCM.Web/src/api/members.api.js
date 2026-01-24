/**
 * Members API Service
 * Xử lý các API liên quan đến thành viên
 */
import axios from './axios'

const MEMBERS_ENDPOINT = '/members'

export const membersApi = {
  /**
   * Lấy danh sách thành viên (có phân trang)
   * @param {Object} params - { page, pageSize, search, sortBy, sortOrder }
   */
  getAll(params = {}) {
    return axios.get(MEMBERS_ENDPOINT, { params })
  },

  /**
   * Lấy thông tin một thành viên
   * @param {number|string} id
   */
  getById(id) {
    return axios.get(`${MEMBERS_ENDPOINT}/${id}`)
  },

  /**
   * Tạo thành viên mới (Admin)
   * @param {Object} memberData
   */
  create(memberData) {
    return axios.post(MEMBERS_ENDPOINT, memberData)
  },

  /**
   * Cập nhật thông tin thành viên
   * @param {number|string} id
   * @param {Object} memberData
   */
  update(id, memberData) {
    return axios.put(`${MEMBERS_ENDPOINT}/${id}`, memberData)
  },

  /**
   * Xóa thành viên (Admin)
   * @param {number|string} id
   */
  delete(id) {
    return axios.delete(`${MEMBERS_ENDPOINT}/${id}`)
  },

  /**
   * Lấy top ranking
   * @param {number} limit - Số lượng top (default: 5)
   */
  getTopRanking(limit = 5) {
    return axios.get(`${MEMBERS_ENDPOINT}/top-ranking`, { params: { limit } })
  },

  /**
   * Lấy thống kê thành viên
   */
  getStatistics() {
    return axios.get(`${MEMBERS_ENDPOINT}/statistics`)
  },

  /**
   * Upload avatar
   * @param {number|string} id
   * @param {FormData} formData
   */
  uploadAvatar(id, formData) {
    return axios.post(`${MEMBERS_ENDPOINT}/${id}/avatar`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
  },

  /**
   * Cập nhật profile của chính mình
   * @param {Object} profileData
   */
  updateMyProfile(profileData) {
    return axios.put(`${MEMBERS_ENDPOINT}/me`, profileData)
  },

  /**
   * Lấy lịch sử trận đấu của member
   * @param {number|string} id
   * @param {Object} params - { page, pageSize }
   */
  getMatchHistory(id, params = {}) {
    return axios.get(`${MEMBERS_ENDPOINT}/${id}/matches`, { params })
  }
}

export default membersApi
