/**
 * News Store (Pinia)
 */
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { newsApi } from '@/api'
import { useToast } from 'vue-toastification'

export const useNewsStore = defineStore('news', () => {
  const toast = useToast()

  const newsList = ref([])
  const pinnedNews = ref([])
  const currentNews = ref(null)
  const pagination = ref({
    page: 1,
    pageSize: 10,
    totalItems: 0,
    totalPages: 0
  })
  const isLoading = ref(false)
  const error = ref(null)

  const totalNews = computed(() => pagination.value.totalItems)

  async function fetchNews(params = {}) {
    isLoading.value = true
    error.value = null

    try {
      const response = await newsApi.getAll({
        page: pagination.value.page,
        pageSize: pagination.value.pageSize,
        ...params
      })

      const data = response.data
      newsList.value = data.items || data

      if (data.pagination) {
        pagination.value = { ...pagination.value, ...data.pagination }
      }

      return newsList.value
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể tải tin tức'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function fetchPinnedNews() {
    try {
      const response = await newsApi.getPinned()
      pinnedNews.value = response.data.items || response.data
      return pinnedNews.value
    } catch (err) {
      console.error('Failed to fetch pinned news:', err)
      return []
    }
  }

  async function fetchNewsById(id) {
    isLoading.value = true
    error.value = null

    try {
      const response = await newsApi.getById(id)
      currentNews.value = response.data
      return currentNews.value
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể tải chi tiết tin tức'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function createNews(newsData) {
    isLoading.value = true
    error.value = null

    try {
      const response = await newsApi.create(newsData)
      newsList.value.unshift(response.data)
      toast.success('Tạo tin tức thành công!')
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể tạo tin tức'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function updateNews(id, newsData) {
    isLoading.value = true
    error.value = null

    try {
      const response = await newsApi.update(id, newsData)
      
      const index = newsList.value.findIndex(n => n.id === id)
      if (index !== -1) {
        newsList.value[index] = response.data
      }

      toast.success('Cập nhật tin tức thành công!')
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể cập nhật tin tức'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function deleteNews(id) {
    isLoading.value = true

    try {
      await newsApi.delete(id)
      newsList.value = newsList.value.filter(n => n.id !== id)
      toast.success('Xóa tin tức thành công!')
    } catch (err) {
      toast.error(err.response?.data?.message || 'Không thể xóa tin tức')
      throw err
    } finally {
      isLoading.value = false
    }
  }

  function setPage(page) {
    pagination.value.page = page
  }

  function $reset() {
    newsList.value = []
    pinnedNews.value = []
    currentNews.value = null
    pagination.value = { page: 1, pageSize: 10, totalItems: 0, totalPages: 0 }
    isLoading.value = false
    error.value = null
  }

  return {
    newsList,
    pinnedNews,
    currentNews,
    pagination,
    isLoading,
    error,
    totalNews,
    fetchNews,
    fetchPinnedNews,
    fetchNewsById,
    createNews,
    updateNews,
    deleteNews,
    setPage,
    $reset
  }
})
