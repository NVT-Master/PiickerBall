<template>
  <nav aria-label="Pagination" v-if="totalPages > 1">
    <ul class="pagination mb-0" :class="sizeClass">
      <!-- Previous -->
      <li class="page-item" :class="{ disabled: currentPage === 1 }">
        <a 
          class="page-link" 
          href="#" 
          @click.prevent="changePage(currentPage - 1)"
        >
          <i class="bi bi-chevron-left"></i>
        </a>
      </li>

      <!-- First page -->
      <li class="page-item" :class="{ active: currentPage === 1 }" v-if="showFirst">
        <a class="page-link" href="#" @click.prevent="changePage(1)">1</a>
      </li>

      <!-- Ellipsis before -->
      <li class="page-item disabled" v-if="showFirstEllipsis">
        <span class="page-link">...</span>
      </li>

      <!-- Page numbers -->
      <li 
        v-for="page in visiblePages" 
        :key="page"
        class="page-item" 
        :class="{ active: page === currentPage }"
      >
        <a class="page-link" href="#" @click.prevent="changePage(page)">{{ page }}</a>
      </li>

      <!-- Ellipsis after -->
      <li class="page-item disabled" v-if="showLastEllipsis">
        <span class="page-link">...</span>
      </li>

      <!-- Last page -->
      <li class="page-item" :class="{ active: currentPage === totalPages }" v-if="showLast">
        <a class="page-link" href="#" @click.prevent="changePage(totalPages)">{{ totalPages }}</a>
      </li>

      <!-- Next -->
      <li class="page-item" :class="{ disabled: currentPage === totalPages }">
        <a 
          class="page-link" 
          href="#" 
          @click.prevent="changePage(currentPage + 1)"
        >
          <i class="bi bi-chevron-right"></i>
        </a>
      </li>
    </ul>
  </nav>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  currentPage: {
    type: Number,
    required: true
  },
  totalPages: {
    type: Number,
    required: true
  },
  maxVisible: {
    type: Number,
    default: 5
  },
  size: {
    type: String,
    default: '', // '', 'sm', 'lg'
    validator: (v) => ['', 'sm', 'lg'].includes(v)
  }
})

const emit = defineEmits(['page-change'])

const sizeClass = computed(() => {
  return props.size ? `pagination-${props.size}` : ''
})

const visiblePages = computed(() => {
  const pages = []
  const half = Math.floor(props.maxVisible / 2)
  
  let start = Math.max(2, props.currentPage - half)
  let end = Math.min(props.totalPages - 1, props.currentPage + half)
  
  // Adjust if near start or end
  if (props.currentPage <= half + 1) {
    end = Math.min(props.totalPages - 1, props.maxVisible)
  }
  if (props.currentPage >= props.totalPages - half) {
    start = Math.max(2, props.totalPages - props.maxVisible + 1)
  }
  
  for (let i = start; i <= end; i++) {
    pages.push(i)
  }
  
  return pages
})

const showFirst = computed(() => props.totalPages > 1)
const showLast = computed(() => props.totalPages > 1 && props.totalPages > props.maxVisible)
const showFirstEllipsis = computed(() => visiblePages.value[0] > 2)
const showLastEllipsis = computed(() => {
  const lastVisible = visiblePages.value[visiblePages.value.length - 1]
  return lastVisible < props.totalPages - 1
})

function changePage(page) {
  if (page >= 1 && page <= props.totalPages && page !== props.currentPage) {
    emit('page-change', page)
  }
}
</script>
