/**
 * Matches API Service
 * Xử lý các API liên quan đến trận đấu
 */
import axios from './axios'

const MATCHES_ENDPOINT = '/matches'

export const matchesApi = {
  /**
   * Lấy danh sách matches (có phân trang)
   * @param {Object} params - { page, pageSize, gameMode, isRanked, memberId, challengeId }
   */
  getAll(params = {}) {
    return axios.get(MATCHES_ENDPOINT, { params })
  },

  /**
   * Lấy thông tin một match
   * @param {number|string} id
   */
  getById(id) {
    return axios.get(`${MATCHES_ENDPOINT}/${id}`)
  },

  /**
   * Ghi nhận trận đấu mới
   * @param {Object} matchData
   * {
   *   gameMode: 0|1, // Single/Double
   *   matchFormat: 1|3|5, // Best of
   *   courtId: number,
   *   isRanked: boolean,
   *   challengeId?: number,
   *   team1Players: [memberId],
   *   team2Players: [memberId],
   *   team1Score: number,
   *   team2Score: number,
   *   winningSide: 1|2,
   *   playedAt: datetime
   * }
   */
  create(matchData) {
    return axios.post(MATCHES_ENDPOINT, matchData)
  },

  /**
   * Cập nhật match
   * @param {number|string} id
   * @param {Object} matchData
   */
  update(id, matchData) {
    return axios.put(`${MATCHES_ENDPOINT}/${id}`, matchData)
  },

  /**
   * Xóa match (Admin/Referee)
   * @param {number|string} id
   */
  delete(id) {
    return axios.delete(`${MATCHES_ENDPOINT}/${id}`)
  },

  /**
   * Lấy matches gần đây
   * @param {number} limit
   */
  getRecent(limit = 10) {
    return axios.get(`${MATCHES_ENDPOINT}/recent`, { params: { limit } })
  },

  /**
   * Lấy thống kê matches
   */
  getStatistics() {
    return axios.get(`${MATCHES_ENDPOINT}/statistics`)
  }
}

export default matchesApi
