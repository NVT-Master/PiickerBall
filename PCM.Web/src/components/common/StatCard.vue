<template>
  <div class="stat-card card h-100">
    <div class="card-body">
      <div class="d-flex justify-content-between align-items-start">
        <div>
          <p class="text-muted mb-1">{{ title }}</p>
          <h3 class="mb-0" :class="valueClass">
            {{ formattedValue }}
          </h3>
          <small v-if="subtitle" class="text-muted">{{ subtitle }}</small>
        </div>
        <div 
          class="stat-icon rounded-circle d-flex align-items-center justify-content-center"
          :class="iconBgClass"
        >
          <i :class="['bi', icon, 'fs-4', iconColorClass]"></i>
        </div>
      </div>
      
      <!-- Trend -->
      <div v-if="trend !== null" class="mt-2">
        <span 
          class="badge" 
          :class="trend >= 0 ? 'bg-success-subtle text-success' : 'bg-danger-subtle text-danger'"
        >
          <i class="bi" :class="trend >= 0 ? 'bi-arrow-up' : 'bi-arrow-down'"></i>
          {{ Math.abs(trend) }}%
        </span>
        <small class="text-muted ms-1">so với tháng trước</small>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  title: {
    type: String,
    required: true
  },
  value: {
    type: [Number, String],
    required: true
  },
  icon: {
    type: String,
    default: 'bi-bar-chart'
  },
  color: {
    type: String,
    default: 'primary'
  },
  subtitle: {
    type: String,
    default: ''
  },
  trend: {
    type: Number,
    default: null
  },
  isCurrency: {
    type: Boolean,
    default: false
  }
})

const formattedValue = computed(() => {
  if (props.isCurrency) {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(props.value)
  }
  return props.value
})

const valueClass = computed(() => `text-${props.color}`)

const iconBgClass = computed(() => `bg-${props.color}-subtle`)

const iconColorClass = computed(() => `text-${props.color}`)
</script>

<style scoped>
.stat-card {
  transition: transform 0.2s;
}

.stat-card:hover {
  transform: translateY(-2px);
}

.stat-icon {
  width: 48px;
  height: 48px;
}
</style>
