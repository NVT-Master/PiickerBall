<template>
  <div class="empty-state text-center py-5">
    <i :class="['bi', icon, 'display-1 text-muted mb-3']"></i>
    <h5>{{ title }}</h5>
    <p class="text-muted">{{ description }}</p>
    <slot name="action"></slot>
  </div>
</template>

<script setup>
defineProps({
  icon: {
    type: String,
    default: 'bi-inbox'
  },
  title: {
    type: String,
    default: 'Không có dữ liệu'
  },
  description: {
    type: String,
    default: ''
  }
})
</script>
