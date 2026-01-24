/**
 * Challenges API Service
 * Xử lý các API liên quan đến kèo đấu
 */
import axios from './axios'

const CHALLENGES_ENDPOINT = '/challenges'

export const challengesApi = {
  /**
   * Lấy danh sách challenges (có phân trang)
   * @param {Object} params - { page, pageSize, status, type, creatorId }
   */
  getAll(params = {}) {
    return axios.get(CHALLENGES_ENDPOINT, { params })
  },

  /**
   * Lấy thông tin một challenge
   * @param {number|string} id
   */
  getById(id) {
    return axios.get(`${CHALLENGES_ENDPOINT}/${id}`)
  },

  /**
   * Tạo challenge mới
   * @param {Object} challengeData
   */
  create(challengeData) {
    return axios.post(CHALLENGES_ENDPOINT, challengeData)
  },

  /**
   * Cập nhật challenge
   * @param {number|string} id
   * @param {Object} challengeData
   */
  update(id, challengeData) {
    return axios.put(`${CHALLENGES_ENDPOINT}/${id}`, challengeData)
  },

  /**
   * Xóa / Hủy challenge
   * @param {number|string} id
   */
  delete(id) {
    return axios.delete(`${CHALLENGES_ENDPOINT}/${id}`)
  },

  /**
   * Tham gia challenge
   * @param {number|string} id
   */
  join(id) {
    return axios.post(`${CHALLENGES_ENDPOINT}/${id}/join`)
  },

  /**
   * Rời khỏi challenge
   * @param {number|string} id
   */
  leave(id) {
    return axios.post(`${CHALLENGES_ENDPOINT}/${id}/leave`)
  },

  /**
   * Auto chia team
   * @param {number|string} id
   */
  autoDivideTeams(id) {
    return axios.post(`${CHALLENGES_ENDPOINT}/${id}/auto-divide-teams`)
  },

  /**
   * Bắt đầu challenge
   * @param {number|string} id
   */
  start(id) {
    return axios.post(`${CHALLENGES_ENDPOINT}/${id}/start`)
  },

  /**
   * Kết thúc challenge và xác định kết quả
   * @param {number|string} id
   * @param {Object} resultData - { winningSide }
   */
  complete(id, resultData) {
    return axios.post(`${CHALLENGES_ENDPOINT}/${id}/complete`, resultData)
  },

  /**
   * Lấy danh sách participants của challenge
   * @param {number|string} id
   */
  getParticipants(id) {
    return axios.get(`${CHALLENGES_ENDPOINT}/${id}/participants`)
  },

  /**
   * Lấy challenges đang mở
   */
  getOpenChallenges() {
    return axios.get(`${CHALLENGES_ENDPOINT}/open`)
  },

  /**
   * Lấy challenges của tôi
   * @param {Object} params - { page, pageSize, status }
   */
  getMyChallenges(params = {}) {
    return axios.get(`${CHALLENGES_ENDPOINT}/my-challenges`, { params })
  },

  /**
   * Đếm số challenges đang mở
   */
  getOpenCount() {
    return axios.get(`${CHALLENGES_ENDPOINT}/open-count`)
  }
}

export default challengesApi
