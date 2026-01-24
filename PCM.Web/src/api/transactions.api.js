/**
 * Transactions API Service
 * Xử lý các API liên quan đến giao dịch quỹ
 */
import axios from './axios'

const TRANSACTIONS_ENDPOINT = '/transactions'
const CATEGORIES_ENDPOINT = '/transaction-categories'

export const transactionsApi = {
  // ==================== TRANSACTIONS ====================
  
  /**
   * Lấy danh sách giao dịch (có phân trang)
   * @param {Object} params - { page, pageSize, type, categoryId, fromDate, toDate }
   */
  getAll(params = {}) {
    return axios.get(TRANSACTIONS_ENDPOINT, { params })
  },

  /**
   * Lấy thông tin một giao dịch
   * @param {number|string} id
   */
  getById(id) {
    return axios.get(`${TRANSACTIONS_ENDPOINT}/${id}`)
  },

  /**
   * Tạo giao dịch mới
   * @param {Object} transactionData
   * {
   *   type: 0|1, // Income/Expense
   *   amount: number,
   *   categoryId: number,
   *   description: string,
   *   transactionDate: datetime
   * }
   */
  create(transactionData) {
    return axios.post(TRANSACTIONS_ENDPOINT, transactionData)
  },

  /**
   * Cập nhật giao dịch
   * @param {number|string} id
   * @param {Object} transactionData
   */
  update(id, transactionData) {
    return axios.put(`${TRANSACTIONS_ENDPOINT}/${id}`, transactionData)
  },

  /**
   * Xóa giao dịch
   * @param {number|string} id
   */
  delete(id) {
    return axios.delete(`${TRANSACTIONS_ENDPOINT}/${id}`)
  },

  /**
   * Lấy tổng kết quỹ (Summary)
   * @returns { totalIncome, totalExpense, balance }
   */
  getSummary() {
    return axios.get(`${TRANSACTIONS_ENDPOINT}/summary`)
  },

  /**
   * Lấy báo cáo theo tháng
   * @param {Object} params - { year, month }
   */
  getMonthlyReport(params) {
    return axios.get(`${TRANSACTIONS_ENDPOINT}/monthly-report`, { params })
  },

  /**
   * Lấy báo cáo theo năm
   * @param {number} year
   */
  getYearlyReport(year) {
    return axios.get(`${TRANSACTIONS_ENDPOINT}/yearly-report`, { params: { year } })
  },

  /**
   * Xuất báo cáo Excel
   * @param {Object} params - { fromDate, toDate }
   */
  exportExcel(params) {
    return axios.get(`${TRANSACTIONS_ENDPOINT}/export`, { 
      params,
      responseType: 'blob'
    })
  },

  // ==================== CATEGORIES ====================
  
  /**
   * Lấy danh sách danh mục
   */
  getCategories() {
    return axios.get(CATEGORIES_ENDPOINT)
  },

  /**
   * Lấy thông tin một danh mục
   * @param {number|string} id
   */
  getCategoryById(id) {
    return axios.get(`${CATEGORIES_ENDPOINT}/${id}`)
  },

  /**
   * Tạo danh mục mới (Admin)
   * @param {Object} categoryData - { name, type, description }
   */
  createCategory(categoryData) {
    return axios.post(CATEGORIES_ENDPOINT, categoryData)
  },

  /**
   * Cập nhật danh mục (Admin)
   * @param {number|string} id
   * @param {Object} categoryData
   */
  updateCategory(id, categoryData) {
    return axios.put(`${CATEGORIES_ENDPOINT}/${id}`, categoryData)
  },

  /**
   * Xóa danh mục (Admin)
   * @param {number|string} id
   */
  deleteCategory(id) {
    return axios.delete(`${CATEGORIES_ENDPOINT}/${id}`)
  }
}

export default transactionsApi
