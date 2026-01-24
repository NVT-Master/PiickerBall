/**
 * Booking Store (Pinia)
 * Quản lý trạng thái đặt sân
 */
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { bookingsApi } from '@/api'
import { useToast } from 'vue-toastification'

export const useBookingStore = defineStore('booking', () => {
  const toast = useToast()

  // ==================== STATE ====================
  const bookings = ref([])
  const myBookings = ref([])
  const availableSlots = ref([])
  const calendarData = ref([])
  const currentBooking = ref(null)
  const pagination = ref({
    page: 1,
    pageSize: 10,
    totalItems: 0,
    totalPages: 0
  })
  const isLoading = ref(false)
  const error = ref(null)

  // ==================== GETTERS ====================
  const totalBookings = computed(() => pagination.value.totalItems)
  const pendingBookings = computed(() => bookings.value.filter(b => b.status === 0))

  // ==================== ACTIONS ====================

  async function fetchBookings(params = {}) {
    isLoading.value = true
    error.value = null

    try {
      const response = await bookingsApi.getAll({
        page: pagination.value.page,
        pageSize: pagination.value.pageSize,
        ...params
      })

      const data = response.data
      bookings.value = data.items || data

      if (data.pagination) {
        pagination.value = { ...pagination.value, ...data.pagination }
      }

      return bookings.value
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể tải danh sách booking'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function fetchMyBookings(params = {}) {
    isLoading.value = true

    try {
      const response = await bookingsApi.getMyBookings(params)
      myBookings.value = response.data.items || response.data
      return myBookings.value
    } catch (err) {
      toast.error('Không thể tải booking của bạn')
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function fetchAvailableSlots(params) {
    isLoading.value = true

    try {
      const response = await bookingsApi.getAvailableSlots(params)
      availableSlots.value = response.data
      return availableSlots.value
    } catch (err) {
      toast.error('Không thể tải slot trống')
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function fetchCalendar(params) {
    isLoading.value = true

    try {
      const response = await bookingsApi.getCalendar(params)
      calendarData.value = response.data
      return calendarData.value
    } catch (err) {
      console.error('Failed to fetch calendar:', err)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function createBooking(bookingData) {
    isLoading.value = true
    error.value = null

    try {
      const response = await bookingsApi.create(bookingData)
      bookings.value.unshift(response.data)
      toast.success('Đặt sân thành công!')
      return response.data
    } catch (err) {
      error.value = err.response?.data?.message || 'Không thể đặt sân'
      toast.error(error.value)
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function cancelBooking(id) {
    isLoading.value = true

    try {
      await bookingsApi.cancel(id)
      
      // Cập nhật status trong list
      const booking = bookings.value.find(b => b.id === id)
      if (booking) booking.status = 2 // Cancelled
      
      const myBooking = myBookings.value.find(b => b.id === id)
      if (myBooking) myBooking.status = 2

      toast.success('Hủy booking thành công!')
    } catch (err) {
      toast.error(err.response?.data?.message || 'Không thể hủy booking')
      throw err
    } finally {
      isLoading.value = false
    }
  }

  async function confirmBooking(id) {
    isLoading.value = true

    try {
      await bookingsApi.confirm(id)
      
      const booking = bookings.value.find(b => b.id === id)
      if (booking) booking.status = 1 // Confirmed

      toast.success('Xác nhận booking thành công!')
    } catch (err) {
      toast.error(err.response?.data?.message || 'Không thể xác nhận booking')
      throw err
    } finally {
      isLoading.value = false
    }
  }

  function setPage(page) {
    pagination.value.page = page
  }

  function $reset() {
    bookings.value = []
    myBookings.value = []
    availableSlots.value = []
    calendarData.value = []
    currentBooking.value = null
    pagination.value = { page: 1, pageSize: 10, totalItems: 0, totalPages: 0 }
    isLoading.value = false
    error.value = null
  }

  return {
    bookings,
    myBookings,
    availableSlots,
    calendarData,
    currentBooking,
    pagination,
    isLoading,
    error,
    totalBookings,
    pendingBookings,
    fetchBookings,
    fetchMyBookings,
    fetchAvailableSlots,
    fetchCalendar,
    createBooking,
    cancelBooking,
    confirmBooking,
    setPage,
    $reset
  }
})
