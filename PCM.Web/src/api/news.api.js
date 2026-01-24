/**
 * News API Service
 * Xử lý các API liên quan đến tin tức
 */
import axios from './axios'

const NEWS_ENDPOINT = '/news'

export const newsApi = {
  /**
   * Lấy danh sách tin tức (có phân trang)
   * @param {Object} params - { page, pageSize, isPinned, search }
   */
  getAll(params = {}) {
    return axios.get(NEWS_ENDPOINT, { params })
  },

  /**
   * Lấy tin ghim (pinned)
   */
  getPinned() {
    return axios.get(NEWS_ENDPOINT, { params: { isPinned: true } })
  },

  /**
   * Lấy thông tin một tin tức
   * @param {number|string} id
   */
  getById(id) {
    return axios.get(`${NEWS_ENDPOINT}/${id}`)
  },

  /**
   * Tạo tin tức mới
   * @param {Object} newsData - { title, content, isPinned, imageUrl }
   */
  create(newsData) {
    return axios.post(NEWS_ENDPOINT, newsData)
  },

  /**
   * Cập nhật tin tức
   * @param {number|string} id
   * @param {Object} newsData
   */
  update(id, newsData) {
    return axios.put(`${NEWS_ENDPOINT}/${id}`, newsData)
  },

  /**
   * Xóa tin tức
   * @param {number|string} id
   */
  delete(id) {
    return axios.delete(`${NEWS_ENDPOINT}/${id}`)
  },

  /**
   * Toggle ghim tin
   * @param {number|string} id
   */
  togglePin(id) {
    return axios.patch(`${NEWS_ENDPOINT}/${id}/toggle-pin`)
  }
}

export default newsApi
