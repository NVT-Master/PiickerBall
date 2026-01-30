<template>
  <div class="bookings-page">
    <div class="page-header d-flex justify-content-between align-items-center">
      <div>
        <h1>Đặt sân</h1>
        <p class="text-muted mb-0">Xem lịch và đặt sân Pickleball</p>
      </div>
    </div>

    <div class="row g-4">
      <!-- Sidebar: Court Selection & Info -->
      <div class="col-lg-3">
        <div class="card">
          <div class="card-header">
            <i class="bi bi-grid-3x3 me-2"></i>Chọn sân
          </div>
          <div class="card-body p-0">
            <div class="list-group list-group-flush">
              <button
                v-for="court in courts"
                :key="court.id"
                class="list-group-item list-group-item-action d-flex justify-content-between align-items-center"
                :class="{ active: selectedCourtId === court.id }"
                @click="selectCourt(court.id)"
              >
                <span>{{ court.name }}</span>
                <span class="badge" :class="court.isActive ? 'bg-success' : 'bg-secondary'">
                  {{ court.isActive ? 'Mở' : 'Đóng' }}
                </span>
              </button>
            </div>
          </div>
        </div>

        <!-- Legend -->
        <div class="card mt-3">
          <div class="card-header">
            <i class="bi bi-info-circle me-2"></i>Chú thích
          </div>
          <div class="card-body">
            <div class="d-flex align-items-center mb-2">
              <div class="legend-dot bg-success me-2"></div>
              <span>Còn trống</span>
            </div>
            <div class="d-flex align-items-center mb-2">
              <div class="legend-dot bg-danger me-2"></div>
              <span>Đã đặt</span>
            </div>
            <div class="d-flex align-items-center mb-2">
              <div class="legend-dot bg-warning me-2"></div>
              <span>Đang chờ xác nhận</span>
            </div>
            <div class="d-flex align-items-center">
              <div class="legend-dot bg-primary me-2"></div>
              <span>Đã chọn</span>
            </div>
          </div>
        </div>

        <!-- My Bookings -->
        <div class="card mt-3">
          <div class="card-header d-flex justify-content-between align-items-center">
            <span><i class="bi bi-calendar-check me-2"></i>Booking của tôi</span>
          </div>
          <div class="card-body p-0">
            <div class="list-group list-group-flush" style="max-height: 300px; overflow-y: auto;">
              <div 
                v-for="booking in myBookings" 
                :key="booking.id"
                class="list-group-item"
              >
                <div class="d-flex justify-content-between">
                  <strong>{{ booking.courtName }}</strong>
                  <span 
                    class="badge"
                    :class="getStatusBadgeClass(booking.status)"
                  >
                    {{ getStatusText(booking.status) }}
                  </span>
                </div>
                <small class="text-muted">
                  {{ formatDate(booking.date) }} | {{ booking.startTime }} - {{ booking.endTime }}
                </small>
                <button 
                  v-if="booking.status === 0"
                  class="btn btn-link btn-sm text-danger p-0 mt-1"
                  @click="cancelBooking(booking.id)"
                >
                  Hủy booking
                </button>
              </div>
              <div v-if="myBookings.length === 0" class="list-group-item text-center text-muted py-3">
                Bạn chưa có booking nào
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Main: Calendar View -->
      <div class="col-lg-9">
        <div class="card">
          <div class="card-header d-flex justify-content-between align-items-center">
            <div class="d-flex align-items-center gap-3">
              <button class="btn btn-outline-secondary btn-sm" @click="prevWeek">
                <i class="bi bi-chevron-left"></i>
              </button>
              <h5 class="mb-0">{{ weekRangeText }}</h5>
              <button class="btn btn-outline-secondary btn-sm" @click="nextWeek">
                <i class="bi bi-chevron-right"></i>
              </button>
            </div>
            <button class="btn btn-outline-primary btn-sm" @click="goToToday">
              Hôm nay
            </button>
          </div>
          <div class="card-body p-0">
            <LoadingSpinner v-if="isLoading" />
            
            <div v-else class="calendar-wrapper">
              <div class="calendar-grid">
                <!-- Header: Days of week -->
                <div class="calendar-header">
                  <div class="time-column"></div>
                  <div 
                    v-for="day in weekDays" 
                    :key="day.date"
                    class="day-column"
                    :class="{ 'today': isToday(day.date) }"
                  >
                    <div class="day-name">{{ day.dayName }}</div>
                    <div class="day-date">{{ day.dateStr }}</div>
                  </div>
                </div>

                <!-- Body: Time slots -->
                <div class="calendar-body">
                  <div v-for="hour in timeSlots" :key="hour" class="time-row">
                    <div class="time-label">{{ hour }}:00</div>
                    <div 
                      v-for="day in weekDays" 
                      :key="`${day.date}-${hour}`"
                      class="slot-cell"
                      :class="getSlotClass(day.date, hour)"
                      @click="handleSlotClick(day.date, hour)"
                    >
                      <span v-if="getBookingInfo(day.date, hour)" class="slot-info">
                        {{ getBookingInfo(day.date, hour) }}
                      </span>
                      <button
                        v-if="isAdmin && bookedSlots[day.date]?.[hour]"
                        class="btn btn-sm btn-danger slot-delete-btn"
                        @click.stop="deleteBookingSlot(day.date, hour)"
                        title="Xóa booking này"
                      >
                        <i class="bi bi-trash"></i>
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Booking Modal -->
    <BaseModal
      v-if="showBookingModal"
      title="Đặt sân"
      :loading="isSubmitting"
      @close="showBookingModal = false"
      @confirm="confirmBooking"
      confirm-text="Xác nhận đặt"
    >
      <div class="booking-summary">
        <div class="mb-3">
          <label class="text-muted small">Sân</label>
          <p class="mb-0 fw-bold">{{ selectedCourtName }}</p>
        </div>
        <div class="mb-3">
          <label class="text-muted small">Ngày</label>
          <p class="mb-0 fw-bold">{{ formatDate(bookingForm.date) }}</p>
        </div>
        <div class="row">
          <div class="col-6 mb-3">
            <label class="form-label">Giờ bắt đầu</label>
            <select class="form-select" v-model="bookingForm.startTime">
              <option v-for="h in availableStartHours" :key="h" :value="`${h}:00`">
                {{ h }}:00
              </option>
            </select>
          </div>
          <div class="col-6 mb-3">
            <label class="form-label">Giờ kết thúc</label>
            <select class="form-select" v-model="bookingForm.endTime">
              <option v-for="h in availableEndHours" :key="h" :value="`${h}:00`">
                {{ h }}:00
              </option>
            </select>
          </div>
        </div>
        <div class="mb-3">
          <label class="form-label">Ghi chú</label>
          <textarea
            class="form-control"
            v-model="bookingForm.note"
            rows="2"
            placeholder="Ghi chú thêm (tùy chọn)"
          ></textarea>
        </div>
        <div class="alert alert-info mb-0">
          <i class="bi bi-info-circle me-2"></i>
          Ước tính: <strong>{{ estimatedPrice }}</strong>
        </div>
      </div>
    </BaseModal>

    <!-- Delete Confirmation Modal -->
    <BaseModal
      v-if="showDeleteModal"
      title="Xác nhận xóa"
      :loading="isSubmitting"
      @close="showDeleteModal = false"
      @confirm="confirmDelete"
      confirm-text="Xóa"
      confirm-class="btn-danger"
    >
      <div class="text-center py-3">
        <i class="bi bi-exclamation-triangle-fill text-warning" style="font-size: 3rem;"></i>
        <h5 class="mt-3">Bạn có chắc muốn xóa booking này?</h5>
        <p class="text-muted mb-0" v-if="bookingToDelete">
          Booking của <strong>{{ bookingToDelete.memberName || 'thành viên' }}</strong>
        </p>
        <p class="text-danger mt-2 mb-0">
          <small><i class="bi bi-info-circle me-1"></i>Hành động này không thể hoàn tác!</small>
        </p>
      </div>
    </BaseModal>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useCourtStore } from '@/stores/court.store'
import { useBookingStore } from '@/stores/booking.store'
import { useAuthStore } from '@/stores/auth.store'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import BaseModal from '@/components/common/BaseModal.vue'
import dayjs from 'dayjs'

const route = useRoute()
const courtStore = useCourtStore()
const bookingStore = useBookingStore()
const authStore = useAuthStore()

const isAdmin = computed(() => authStore.isAdmin)

const isLoading = ref(false)
const isSubmitting = ref(false)
const showBookingModal = ref(false)
const showDeleteModal = ref(false)
const bookingToDelete = ref(null)

const selectedCourtId = ref(null)
const currentWeekStart = ref(dayjs().startOf('week').add(1, 'day')) // Monday
const bookedSlots = ref({}) // { 'YYYY-MM-DD': { hour: booking } }
const myBookings = ref([])

const courts = computed(() => courtStore.courts.filter(c => c.isActive))
const selectedCourtName = computed(() => {
  const court = courts.value.find(c => c.id === selectedCourtId.value)
  return court?.name || ''
})

const bookingForm = reactive({
  date: '',
  startTime: '',
  endTime: '',
  note: ''
})

// Time slots (6:00 - 22:00)
const timeSlots = computed(() => {
  const slots = []
  for (let h = 6; h <= 21; h++) {
    slots.push(h)
  }
  return slots
})

// Generate week days
const weekDays = computed(() => {
  const days = []
  for (let i = 0; i < 7; i++) {
    const date = currentWeekStart.value.add(i, 'day')
    days.push({
      date: date.format('YYYY-MM-DD'),
      dayName: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'][date.day()],
      dateStr: date.format('DD/MM')
    })
  }
  return days
})

const weekRangeText = computed(() => {
  const start = currentWeekStart.value.format('DD/MM')
  const end = currentWeekStart.value.add(6, 'day').format('DD/MM/YYYY')
  return `${start} - ${end}`
})

const availableStartHours = computed(() => {
  return timeSlots.value.filter(h => h < 21)
})

const availableEndHours = computed(() => {
  const startHour = parseInt(bookingForm.startTime?.split(':')[0]) || 6
  return timeSlots.value.filter(h => h > startHour && h <= 22)
})

const estimatedPrice = computed(() => {
  const court = courts.value.find(c => c.id === selectedCourtId.value)
  if (!court) return '0 ₫'
  
  const startHour = parseInt(bookingForm.startTime?.split(':')[0]) || 0
  const endHour = parseInt(bookingForm.endTime?.split(':')[0]) || 0
  const hours = endHour - startHour
  
  const total = hours * (court.pricePerHour || 100000)
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(total)
})

onMounted(async () => {
  await courtStore.fetchCourts()
  
  // Check if courtId is in query params
  if (route.query.courtId) {
    selectedCourtId.value = parseInt(route.query.courtId)
  } else if (courts.value.length > 0) {
    selectedCourtId.value = courts.value[0].id
  }

  loadBookings()
  loadMyBookings()
})

watch(selectedCourtId, () => {
  loadBookings()
})

watch(currentWeekStart, () => {
  loadBookings()
})

async function loadBookings() {
  if (!selectedCourtId.value) return

  isLoading.value = true
  try {
    const startDate = currentWeekStart.value.format('YYYY-MM-DD')
    const endDate = currentWeekStart.value.add(6, 'day').format('YYYY-MM-DD')
    
    await bookingStore.fetchCalendar({
      courtId: selectedCourtId.value,
      startDate,
      endDate
    })

    // Process bookings into slot map
    bookedSlots.value = {}
    bookingStore.calendarData.forEach(booking => {
      // Parse date from startTime (ISO DateTime string)
      const startDateTime = dayjs(booking.startTime)
      const endDateTime = dayjs(booking.endTime)
      const date = startDateTime.format('YYYY-MM-DD')
      
      if (!bookedSlots.value[date]) {
        bookedSlots.value[date] = {}
      }
      
      const startHour = startDateTime.hour()
      const endHour = endDateTime.hour()
      
      // Mark all hours in this booking
      for (let h = startHour; h < endHour; h++) {
        bookedSlots.value[date][h] = {
          ...booking,
          memberName: booking.member?.fullName || 'Đã đặt'
        }
      }
    })
  } catch (error) {
    console.error('Failed to load bookings:', error)
    // Mock data for demo
    bookedSlots.value = {}
  } finally {
    isLoading.value = false
  }
}

async function loadMyBookings() {
  try {
    await bookingStore.fetchMyBookings()
    myBookings.value = bookingStore.myBookings.slice(0, 5)
  } catch (error) {
    console.error('Failed to load my bookings:', error)
    myBookings.value = []
  }
}

function selectCourt(courtId) {
  selectedCourtId.value = courtId
}

function prevWeek() {
  currentWeekStart.value = currentWeekStart.value.subtract(7, 'day')
}

function nextWeek() {
  currentWeekStart.value = currentWeekStart.value.add(7, 'day')
}

function goToToday() {
  currentWeekStart.value = dayjs().startOf('week').add(1, 'day')
}

function isToday(dateStr) {
  return dateStr === dayjs().format('YYYY-MM-DD')
}

function getSlotClass(date, hour) {
  const booking = bookedSlots.value[date]?.[hour]
  
  // Past slots
  const slotTime = dayjs(`${date} ${hour}:00`)
  if (slotTime.isBefore(dayjs())) {
    return 'slot-past'
  }
  
  if (booking) {
    if (booking.status === 0) return 'slot-pending'
    if (booking.status === 1) return 'slot-booked'
    if (booking.status === 2) return 'slot-cancelled'
    return 'slot-booked'
  }
  
  return 'slot-available'
}

function getBookingInfo(date, hour) {
  const booking = bookedSlots.value[date]?.[hour]
  if (!booking) return null
  
  // Only show info on first slot of booking
  const startDateTime = dayjs(booking.startTime)
  const startHour = startDateTime.hour()
  
  if (hour === startHour) {
    return booking.memberName || 'Đã đặt'
  }
  return null
}

function handleSlotClick(date, hour) {
  const slotTime = dayjs(`${date} ${hour}:00`)
  if (slotTime.isBefore(dayjs())) return // Past slot
  
  const booking = bookedSlots.value[date]?.[hour]
  if (booking) return // Already booked

  bookingForm.date = date
  bookingForm.startTime = `${hour.toString().padStart(2, '0')}:00`
  bookingForm.endTime = `${(hour + 1).toString().padStart(2, '0')}:00`
  bookingForm.note = ''
  
  showBookingModal.value = true
}

async function confirmBooking() {
  isSubmitting.value = true
  try {
    await bookingStore.createBooking({
      courtId: selectedCourtId.value,
      date: bookingForm.date,
      startTime: bookingForm.startTime,
      endTime: bookingForm.endTime,
      note: bookingForm.note
    })

    showBookingModal.value = false
    loadBookings()
    loadMyBookings()
  } finally {
    isSubmitting.value = false
  }
}

async function cancelBooking(bookingId) {
  if (!confirm('Bạn có chắc muốn hủy booking này?')) return
  
  await bookingStore.cancelBooking(bookingId)
  loadBookings()
  loadMyBookings()
}

async function deleteBookingSlot(date, hour) {
  const booking = bookedSlots.value[date]?.[hour]
  if (!booking) return
  
  bookingToDelete.value = booking
  showDeleteModal.value = true
}

async function confirmDelete() {
  if (!bookingToDelete.value) return
  
  isSubmitting.value = true
  try {
    await bookingStore.deleteBooking(bookingToDelete.value.id)
    showDeleteModal.value = false
    bookingToDelete.value = null
    loadBookings()
    loadMyBookings()
  } catch (error) {
    console.error('Failed to delete booking:', error)
  } finally {
    isSubmitting.value = false
  }
}

function formatDate(date) {
  return dayjs(date).format('DD/MM/YYYY')
}

function getStatusBadgeClass(status) {
  const classes = {
    0: 'bg-warning text-dark',
    1: 'bg-success',
    2: 'bg-secondary',
    3: 'bg-info'
  }
  return classes[status] || 'bg-secondary'
}

function getStatusText(status) {
  const texts = {
    0: 'Chờ xác nhận',
    1: 'Đã xác nhận',
    2: 'Đã hủy',
    3: 'Hoàn thành'
  }
  return texts[status] || 'Unknown'
}
</script>

<style scoped>
.legend-dot {
  width: 16px;
  height: 16px;
  border-radius: 4px;
}

.calendar-wrapper {
  overflow-x: auto;
}

.calendar-grid {
  min-width: 800px;
}

.slot-cell {
  position: relative;
}

.slot-delete-btn {
  position: absolute;
  top: 2px;
  right: 2px;
  padding: 2px 6px;
  font-size: 0.75rem;
  opacity: 0;
  transition: opacity 0.2s;
}

.slot-cell:hover .slot-delete-btn {
  opacity: 1;
}

.calendar-header {
  display: flex;
  border-bottom: 2px solid #e2e8f0;
}

.time-column {
  width: 60px;
  flex-shrink: 0;
}

.day-column {
  flex: 1;
  text-align: center;
  padding: 0.75rem 0.5rem;
  border-left: 1px solid #e2e8f0;
}

.day-column.today {
  background: #ecfdf5;
}

.day-name {
  font-weight: 600;
  color: #64748b;
  font-size: 0.875rem;
}

.day-date {
  font-size: 0.875rem;
}

.calendar-body {
  max-height: 500px;
  overflow-y: auto;
}

.time-row {
  display: flex;
  border-bottom: 1px solid #f1f5f9;
}

.time-label {
  width: 60px;
  flex-shrink: 0;
  padding: 0.5rem;
  font-size: 0.75rem;
  color: #64748b;
  text-align: right;
  border-right: 1px solid #e2e8f0;
}

.slot-cell {
  flex: 1;
  min-height: 40px;
  border-left: 1px solid #f1f5f9;
  cursor: pointer;
  transition: all 0.15s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.75rem;
}

.slot-available {
  background: #ecfdf5;
}

.slot-available:hover {
  background: #d1fae5;
}

.slot-booked {
  background: #fee2e2;
  cursor: not-allowed;
}

.slot-pending {
  background: #fef3c7;
  cursor: not-allowed;
}

.slot-past {
  background: #f1f5f9;
  cursor: not-allowed;
  color: #94a3b8;
}

.slot-info {
  padding: 2px 6px;
  background: rgba(0, 0, 0, 0.1);
  border-radius: 4px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: 90%;
}
</style>
