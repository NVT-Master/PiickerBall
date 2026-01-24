/**
 * Courts API Service
 * Xử lý các API liên quan đến sân
 */
import axios from './axios'

const COURTS_ENDPOINT = '/courts'

export const courtsApi = {
  /**
   * Lấy danh sách sân (có phân trang)
   * @param {Object} params - { page, pageSize, search, isActive }
   */
  getAll(params = {}) {
    return axios.get(COURTS_ENDPOINT, { params })
  },

  /**
   * Lấy thông tin một sân
   * @param {number|string} id
   */
  getById(id) {
    return axios.get(`${COURTS_ENDPOINT}/${id}`)
  },

  /**
   * Tạo sân mới (Admin)
   * @param {Object} courtData - { name, location, description, pricePerHour, isActive }
   */
  create(courtData) {
    return axios.post(COURTS_ENDPOINT, courtData)
  },

  /**
   * Cập nhật thông tin sân (Admin)
   * @param {number|string} id
   * @param {Object} courtData
   */
  update(id, courtData) {
    return axios.put(`${COURTS_ENDPOINT}/${id}`, courtData)
  },

  /**
   * Xóa sân (Admin)
   * @param {number|string} id
   */
  delete(id) {
    return axios.delete(`${COURTS_ENDPOINT}/${id}`)
  },

  /**
   * Lấy danh sách sân đang hoạt động (cho dropdown)
   */
  getActiveList() {
    return axios.get(`${COURTS_ENDPOINT}/active`)
  },

  /**
   * Lấy lịch đặt sân theo ngày
   * @param {number|string} courtId
   * @param {string} date - YYYY-MM-DD
   */
  getSchedule(courtId, date) {
    return axios.get(`${COURTS_ENDPOINT}/${courtId}/schedule`, { params: { date } })
  }
}

export default courtsApi
