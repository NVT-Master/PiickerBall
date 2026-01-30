/**
 * Bookings API Service
 * Xử lý các API liên quan đến đặt sân
 */
import axios from './axios'

const BOOKINGS_ENDPOINT = '/bookings'

export const bookingsApi = {
  /**
   * Lấy danh sách booking (có phân trang)
   * @param {Object} params - { page, pageSize, courtId, date, status, memberId }
   */
  getAll(params = {}) {
    return axios.get(BOOKINGS_ENDPOINT, { params })
  },

  /**
   * Lấy thông tin một booking
   * @param {number|string} id
   */
  getById(id) {
    return axios.get(`${BOOKINGS_ENDPOINT}/${id}`)
  },

  /**
   * Tạo booking mới
   * @param {Object} bookingData - { courtId, date, startTime, endTime, note }
   */
  create(bookingData) {
    return axios.post(BOOKINGS_ENDPOINT, bookingData)
  },

  /**
   * Cập nhật booking
   * @param {number|string} id
   * @param {Object} bookingData
   */
  update(id, bookingData) {
    return axios.put(`${BOOKINGS_ENDPOINT}/${id}`, bookingData)
  },

  /**
   * Hủy booking
   * @param {number|string} id
   */
  cancel(id) {
    return axios.put(`${BOOKINGS_ENDPOINT}/${id}/cancel`)
  },

  /**
   * Xóa booking (Admin only)
   * @param {number|string} id
   */
  delete(id) {
    return axios.delete(`${BOOKINGS_ENDPOINT}/${id}`)
  },

  /**
   * Xác nhận booking (Admin)
   * @param {number|string} id
   */
  confirm(id) {
    return axios.put(`${BOOKINGS_ENDPOINT}/${id}/confirm`)
  },

  /**
   * Lấy các slot trống
   * @param {Object} params - { courtId, date }
   */
  getAvailableSlots(params) {
    return axios.get(`${BOOKINGS_ENDPOINT}/available-slots`, { params })
  },

  /**
   * Lấy booking của tôi
   * @param {Object} params - { page, pageSize, status }
   */
  getMyBookings(params = {}) {
    return axios.get(`${BOOKINGS_ENDPOINT}/my-bookings`, { params })
  },

  /**
   * Lấy lịch đặt sân theo tuần
   * @param {Object} params - { courtId, startDate, endDate }
   */
  getCalendar(params) {
    return axios.get(`${BOOKINGS_ENDPOINT}/calendar`, { params })
  }
}

export default bookingsApi
