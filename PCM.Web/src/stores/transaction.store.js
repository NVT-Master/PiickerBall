/**
 * Transaction Store (Pinia)
 * Quản lý trạng thái giao dịch quỹ
 */
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { transactionsApi } from '@/api'
import { useToast } from 'vue-toastification'

export const useTransactionStore = defineStore('transaction', () => {
  const toast = useToast()

  // ==================== STATE ====================
  const transactions = ref([])
  const categories = ref([])
  const summary = ref({
    totalIncome: 0,
    totalExpense: 0,
    balance: 0
  })
  const monthlyReport = ref(null)
  const pagination = ref({
    page: 1,
    pageSize: 10,
    totalItems: 0,
    totalPages: 0
  })
  const isLoading = ref(false)
  const error = ref(null)

  // ==================== GETTERS ====================
  const totalTransactions = computed(() => pagination.value.totalItems)
  
  const balance = computed(() => summary.value.balance)
  
  const isBalanceNegative = computed(() => summary.value.balance < 0)
  
  const incomeCategories = computed(() => 
    categories.value.filter(c => c.type === 0) // Income
  )
  
  const expenseCategories = computed(() => 
    categories.value.filter(c => c.type === 1) // Expense
  )

  // ==================== ACTIONS ====================

  async function fetchTransactions(params = {}) {
    isLoading.value = true
    error.value = null

    try {
      const response = await transactionsApi.getAll({
        page: pagination.value.page,
        pageSize: pagination.value.pageSize,
        ...params
      })

      const data = response.data
      transactions.value = data.items || data

      if (data.pagination) {
        pagination.value = { ...pagination.value, ...data.pagination }
      }

      return transactions.value
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể tải danh sách giao dịch'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function fetchSummary() {
    try {
      const response = await transactionsApi.getSummary()
      summary.value = response.data
      return summary.value
    } catch (err) {
      console.error('Failed to fetch summary:', err)
      return summary.value
    }
  }

  async function fetchCategories() {
    try {
      const response = await transactionsApi.getCategories()
      categories.value = response.data
      return categories.value
    } catch (err) {
      console.error('Failed to fetch categories:', err)
      return []
    }
  }

  async function createTransaction(transactionData) {
    isLoading.value = true
    error.value = null

    try {
      const response = await transactionsApi.create(transactionData)
      transactions.value.unshift(response.data)
      
      // Refresh summary
      await fetchSummary()
      
      toast.success('Tạo giao dịch thành công!')
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể tạo giao dịch'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function updateTransaction(id, transactionData) {
    isLoading.value = true
    error.value = null

    try {
      const response = await transactionsApi.update(id, transactionData)
      
      const index = transactions.value.findIndex(t => t.id === id)
      if (index !== -1) {
        transactions.value[index] = response.data
      }
      
      await fetchSummary()

      toast.success('Cập nhật giao dịch thành công!')
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể cập nhật giao dịch'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function deleteTransaction(id) {
    isLoading.value = true

    try {
      await transactionsApi.delete(id)
      transactions.value = transactions.value.filter(t => t.id !== id)
      
      await fetchSummary()
      
      toast.success('Xóa giao dịch thành công!')
    } catch (err) {
      toast.error(err.response?.data?.message || 'Không thể xóa giao dịch')
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function fetchMonthlyReport(params) {
    try {
      const response = await transactionsApi.getMonthlyReport(params)
      monthlyReport.value = response.data
      return monthlyReport.value
    } catch (err) {
      console.error('Failed to fetch monthly report:', err)
      return null
    }
  }

  // ==================== CATEGORY ACTIONS ====================

  async function createCategory(categoryData) {
    isLoading.value = true

    try {
      const response = await transactionsApi.createCategory(categoryData)
      categories.value.push(response.data)
      toast.success('Tạo danh mục thành công!')
      return response.data
    } catch (err) {
      toast.error(err.response?.data?.message || 'Không thể tạo danh mục')
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function updateCategory(id, categoryData) {
    isLoading.value = true

    try {
      const response = await transactionsApi.updateCategory(id, categoryData)
      
      const index = categories.value.findIndex(c => c.id === id)
      if (index !== -1) {
        categories.value[index] = response.data
      }

      toast.success('Cập nhật danh mục thành công!')
      return response.data
    } catch (err) {
      toast.error(err.response?.data?.message || 'Không thể cập nhật danh mục')
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function deleteCategory(id) {
    isLoading.value = true

    try {
      await transactionsApi.deleteCategory(id)
      categories.value = categories.value.filter(c => c.id !== id)
      toast.success('Xóa danh mục thành công!')
    } catch (err) {
      toast.error(err.response?.data?.message || 'Không thể xóa danh mục')
      throw err
    } finally {
      isLoading.value = false
    }
  }

  function setPage(page) {
    pagination.value.page = page
  }

  function $reset() {
    transactions.value = []
    categories.value = []
    summary.value = { totalIncome: 0, totalExpense: 0, balance: 0 }
    monthlyReport.value = null
    pagination.value = { page: 1, pageSize: 10, totalItems: 0, totalPages: 0 }
    isLoading.value = false
    error.value = null
  }

  return {
    transactions,
    categories,
    summary,
    monthlyReport,
    pagination,
    isLoading,
    error,
    totalTransactions,
    balance,
    isBalanceNegative,
    incomeCategories,
    expenseCategories,
    fetchTransactions,
    fetchSummary,
    fetchCategories,
    createTransaction,
    updateTransaction,
    deleteTransaction,
    fetchMonthlyReport,
    createCategory,
    updateCategory,
    deleteCategory,
    setPage,
    $reset
  }
})
