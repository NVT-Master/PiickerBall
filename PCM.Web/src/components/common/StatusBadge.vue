<template>
  <span class="badge" :class="badgeClass">
    <i v-if="icon" :class="['bi', icon, 'me-1']"></i>
    {{ text }}
  </span>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  status: {
    type: String,
    required: true
  },
  type: {
    type: String,
    default: 'booking', // booking, challenge, payment
    validator: (v) => ['booking', 'challenge', 'payment', 'member'].includes(v)
  }
})

const statusConfig = {
  booking: {
    pending: { class: 'bg-warning', text: 'Chờ xác nhận', icon: 'bi-clock' },
    confirmed: { class: 'bg-success', text: 'Đã xác nhận', icon: 'bi-check' },
    cancelled: { class: 'bg-danger', text: 'Đã hủy', icon: 'bi-x' },
    completed: { class: 'bg-info', text: 'Hoàn thành', icon: 'bi-check-circle' }
  },
  challenge: {
    open: { class: 'bg-info', text: 'Đang mở', icon: 'bi-broadcast' },
    full: { class: 'bg-primary', text: 'Đủ người', icon: 'bi-people-fill' },
    ongoing: { class: 'bg-warning', text: 'Đang đấu', icon: 'bi-play-fill' },
    completed: { class: 'bg-success', text: 'Hoàn thành', icon: 'bi-trophy' },
    cancelled: { class: 'bg-danger', text: 'Đã hủy', icon: 'bi-x-circle' }
  },
  payment: {
    pending: { class: 'bg-warning', text: 'Chờ thanh toán', icon: 'bi-clock' },
    paid: { class: 'bg-success', text: 'Đã thanh toán', icon: 'bi-check' },
    refunded: { class: 'bg-secondary', text: 'Đã hoàn tiền', icon: 'bi-arrow-counterclockwise' }
  },
  member: {
    active: { class: 'bg-success', text: 'Đang hoạt động', icon: 'bi-check-circle' },
    inactive: { class: 'bg-secondary', text: 'Tạm ngừng', icon: 'bi-pause-circle' },
    banned: { class: 'bg-danger', text: 'Bị cấm', icon: 'bi-slash-circle' }
  }
}

const config = computed(() => {
  const typeConfig = statusConfig[props.type] || {}
  return typeConfig[props.status?.toLowerCase()] || {
    class: 'bg-secondary',
    text: props.status,
    icon: null
  }
})

const badgeClass = computed(() => config.value.class)
const text = computed(() => config.value.text)
const icon = computed(() => config.value.icon)
</script>
