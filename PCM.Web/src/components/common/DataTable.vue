<template>
  <div class="data-table-wrapper">
    <!-- Table Header with search and actions -->
    <div class="d-flex flex-wrap justify-content-between align-items-center mb-3 gap-2" v-if="!hideToolbar">
      <div class="d-flex align-items-center gap-2">
        <!-- Search -->
        <div class="input-group" style="width: 280px;" v-if="searchable">
          <span class="input-group-text bg-white">
            <i class="bi bi-search"></i>
          </span>
          <input
            type="text"
            class="form-control"
            :placeholder="searchPlaceholder"
            v-model="searchQuery"
            @input="debouncedSearch"
          />
          <button 
            class="btn btn-outline-secondary" 
            v-if="searchQuery"
            @click="clearSearch"
          >
            <i class="bi bi-x"></i>
          </button>
        </div>

        <!-- Page size selector -->
        <select 
          class="form-select" 
          style="width: auto;"
          v-model="localPageSize"
          @change="handlePageSizeChange"
        >
          <option v-for="size in pageSizeOptions" :key="size" :value="size">
            {{ size }} / trang
          </option>
        </select>
      </div>

      <div>
        <slot name="actions"></slot>
      </div>
    </div>

    <!-- Table -->
    <div class="table-responsive position-relative">
      <LoadingSpinner v-if="loading" overlay />
      
      <table class="table table-hover" :class="tableClass">
        <thead>
          <tr>
            <th v-if="selectable" style="width: 40px;">
              <input 
                type="checkbox" 
                class="form-check-input"
                :checked="isAllSelected"
                @change="toggleSelectAll"
              />
            </th>
            <th 
              v-for="col in columns" 
              :key="col.key"
              :style="{ width: col.width }"
              :class="{ 'sortable': col.sortable }"
              @click="col.sortable && handleSort(col.key)"
            >
              {{ col.label }}
              <i 
                v-if="col.sortable" 
                class="bi ms-1"
                :class="{
                  'bi-chevron-expand': sortKey !== col.key,
                  'bi-chevron-up': sortKey === col.key && sortOrder === 'asc',
                  'bi-chevron-down': sortKey === col.key && sortOrder === 'desc'
                }"
              ></i>
            </th>
            <th v-if="$slots.rowActions" style="width: 100px;">Thao tác</th>
          </tr>
        </thead>
        <tbody>
          <tr v-if="data.length === 0">
            <td :colspan="totalColumns" class="text-center py-5">
              <div class="empty-state">
                <i class="bi bi-inbox"></i>
                <h5>{{ emptyText }}</h5>
                <p class="text-muted mb-0">{{ emptySubText }}</p>
              </div>
            </td>
          </tr>
          <tr 
            v-for="(row, index) in data" 
            :key="row.id || index"
            :class="{ 'table-active': isSelected(row) }"
          >
            <td v-if="selectable">
              <input 
                type="checkbox" 
                class="form-check-input"
                :checked="isSelected(row)"
                @change="toggleSelect(row)"
              />
            </td>
            <td v-for="col in columns" :key="col.key">
              <slot :name="`cell-${col.key}`" :row="row" :value="getValue(row, col.key)">
                {{ formatValue(row, col) }}
              </slot>
            </td>
            <td v-if="$slots.rowActions">
              <slot name="rowActions" :row="row" :index="index"></slot>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Pagination -->
    <div class="d-flex flex-wrap justify-content-between align-items-center mt-3 gap-2">
      <div class="text-muted small">
        Hiển thị {{ startItem }}-{{ endItem }} / {{ totalItems }} kết quả
      </div>
      <Pagination
        :current-page="currentPage"
        :total-pages="totalPages"
        @page-change="handlePageChange"
      />
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch } from 'vue'
import LoadingSpinner from './LoadingSpinner.vue'
import Pagination from './Pagination.vue'
import config from '@/config/app.config'

const props = defineProps({
  columns: {
    type: Array,
    required: true
    // [{ key: 'name', label: 'Tên', sortable: true, width: '200px', format: fn }]
  },
  data: {
    type: Array,
    default: () => []
  },
  loading: Boolean,
  searchable: {
    type: Boolean,
    default: true
  },
  searchPlaceholder: {
    type: String,
    default: 'Tìm kiếm...'
  },
  selectable: Boolean,
  striped: Boolean,
  bordered: Boolean,
  small: Boolean,
  hideToolbar: Boolean,
  emptyText: {
    type: String,
    default: 'Không có dữ liệu'
  },
  emptySubText: {
    type: String,
    default: 'Dữ liệu sẽ hiển thị ở đây khi có'
  },
  // Pagination
  currentPage: {
    type: Number,
    default: 1
  },
  totalItems: {
    type: Number,
    default: 0
  },
  pageSize: {
    type: Number,
    default: 10
  }
})

const emit = defineEmits([
  'search',
  'sort',
  'page-change',
  'page-size-change',
  'selection-change'
])

const searchQuery = ref('')
const sortKey = ref('')
const sortOrder = ref('asc')
const selectedRows = ref([])
const localPageSize = ref(props.pageSize)
const pageSizeOptions = config.PAGE_SIZE_OPTIONS

let searchTimeout = null

const tableClass = computed(() => ({
  'table-striped': props.striped,
  'table-bordered': props.bordered,
  'table-sm': props.small
}))

const totalColumns = computed(() => {
  let count = props.columns.length
  if (props.selectable) count++
  if (props.$slots?.rowActions) count++
  return count
})

const totalPages = computed(() => 
  Math.ceil(props.totalItems / localPageSize.value)
)

const startItem = computed(() => 
  props.totalItems === 0 ? 0 : (props.currentPage - 1) * localPageSize.value + 1
)

const endItem = computed(() => 
  Math.min(props.currentPage * localPageSize.value, props.totalItems)
)

const isAllSelected = computed(() => 
  props.data.length > 0 && selectedRows.value.length === props.data.length
)

function getValue(row, key) {
  return key.split('.').reduce((obj, k) => obj?.[k], row)
}

function formatValue(row, col) {
  const value = getValue(row, col.key)
  if (col.format) {
    return col.format(value, row)
  }
  return value ?? '-'
}

function debouncedSearch() {
  clearTimeout(searchTimeout)
  searchTimeout = setTimeout(() => {
    emit('search', searchQuery.value)
  }, 300)
}

function clearSearch() {
  searchQuery.value = ''
  emit('search', '')
}

function handleSort(key) {
  if (sortKey.value === key) {
    sortOrder.value = sortOrder.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortKey.value = key
    sortOrder.value = 'asc'
  }
  emit('sort', { key: sortKey.value, order: sortOrder.value })
}

function handlePageChange(page) {
  emit('page-change', page)
}

function handlePageSizeChange() {
  emit('page-size-change', localPageSize.value)
}

function isSelected(row) {
  return selectedRows.value.some(r => r.id === row.id)
}

function toggleSelect(row) {
  const index = selectedRows.value.findIndex(r => r.id === row.id)
  if (index === -1) {
    selectedRows.value.push(row)
  } else {
    selectedRows.value.splice(index, 1)
  }
  emit('selection-change', selectedRows.value)
}

function toggleSelectAll() {
  if (isAllSelected.value) {
    selectedRows.value = []
  } else {
    selectedRows.value = [...props.data]
  }
  emit('selection-change', selectedRows.value)
}

// Watch pageSize prop changes
watch(() => props.pageSize, (newVal) => {
  localPageSize.value = newVal
})
</script>

<style scoped>
.sortable {
  cursor: pointer;
  user-select: none;
}

.sortable:hover {
  background-color: #f8fafc;
}

.empty-state {
  padding: 2rem;
}

.empty-state i {
  font-size: 3rem;
  color: #cbd5e1;
}
</style>
