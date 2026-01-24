<template>
  <div class="transactions-page">
    <!-- Page Header -->
    <div class="page-header d-flex justify-content-between align-items-center">
      <div>
        <h1>Quản lý Quỹ CLB</h1>
        <nav aria-label="breadcrumb">
          <ol class="breadcrumb">
            <li class="breadcrumb-item"><router-link to="/dashboard">Dashboard</router-link></li>
            <li class="breadcrumb-item active">Quỹ CLB</li>
          </ol>
        </nav>
      </div>
      <div class="d-flex gap-2">
        <button class="btn btn-outline-primary" @click="showCategoryModal = true">
          <i class="bi bi-tags me-2"></i>
          Danh mục
        </button>
        <button class="btn btn-primary" @click="openCreateModal">
          <i class="bi bi-plus-lg me-2"></i>
          Tạo giao dịch
        </button>
      </div>
    </div>

    <!-- Summary Cards -->
    <div class="row g-3 mb-4">
      <div class="col-md-4">
        <div class="stat-card bg-success">
          <div class="stat-value">{{ formatCurrency(summary.totalIncome) }}</div>
          <div class="stat-label">Tổng thu</div>
          <i class="bi bi-arrow-down-circle stat-icon"></i>
        </div>
      </div>
      <div class="col-md-4">
        <div class="stat-card bg-danger">
          <div class="stat-value">{{ formatCurrency(summary.totalExpense) }}</div>
          <div class="stat-label">Tổng chi</div>
          <i class="bi bi-arrow-up-circle stat-icon"></i>
        </div>
      </div>
      <div class="col-md-4">
        <div 
          class="stat-card"
          :class="summary.balance >= 0 ? 'bg-primary' : 'bg-warning'"
        >
          <div class="stat-value">{{ formatCurrency(summary.balance) }}</div>
          <div class="stat-label">
            Số dư hiện tại
            <span v-if="summary.balance < 0" class="badge bg-danger ms-2">
              <i class="bi bi-exclamation-triangle"></i> Thâm hụt
            </span>
          </div>
          <i class="bi bi-wallet2 stat-icon"></i>
        </div>
      </div>
    </div>

    <!-- Warning Alert -->
    <div v-if="summary.balance < 0" class="alert alert-danger d-flex align-items-center mb-4">
      <i class="bi bi-exclamation-triangle-fill me-3 fs-4"></i>
      <div>
        <strong>Cảnh báo!</strong> Quỹ CLB đang thâm hụt {{ formatCurrency(Math.abs(summary.balance)) }}. 
        Vui lòng kiểm tra và bổ sung nguồn thu.
      </div>
    </div>

    <!-- Filters -->
    <div class="card mb-4">
      <div class="card-body">
        <div class="row g-3">
          <div class="col-md-2">
            <select class="form-select" v-model="filters.type" @change="loadTransactions">
              <option value="">Tất cả loại</option>
              <option value="0">Thu</option>
              <option value="1">Chi</option>
            </select>
          </div>
          <div class="col-md-3">
            <select class="form-select" v-model="filters.categoryId" @change="loadTransactions">
              <option value="">Tất cả danh mục</option>
              <option v-for="cat in categories" :key="cat.id" :value="cat.id">
                {{ cat.name }}
              </option>
            </select>
          </div>
          <div class="col-md-2">
            <input 
              type="date" 
              class="form-control"
              v-model="filters.fromDate"
              @change="loadTransactions"
              placeholder="Từ ngày"
            >
          </div>
          <div class="col-md-2">
            <input 
              type="date" 
              class="form-control"
              v-model="filters.toDate"
              @change="loadTransactions"
              placeholder="Đến ngày"
            >
          </div>
          <div class="col-md-3">
            <div class="btn-group w-100">
              <button class="btn btn-outline-secondary" @click="resetFilters">
                <i class="bi bi-x-circle me-1"></i>
                Xóa lọc
              </button>
              <button class="btn btn-outline-success" @click="exportExcel">
                <i class="bi bi-file-excel me-1"></i>
                Xuất Excel
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="isLoading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>

    <!-- Transactions Table -->
    <div v-else class="card">
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead>
              <tr>
                <th>ID</th>
                <th>Ngày</th>
                <th>Loại</th>
                <th>Danh mục</th>
                <th>Mô tả</th>
                <th class="text-end">Số tiền</th>
                <th class="text-end">Thao tác</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="trans in transactions" :key="trans.id">
                <td>#{{ trans.id }}</td>
                <td>{{ formatDate(trans.transactionDate) }}</td>
                <td>
                  <span 
                    class="badge"
                    :class="trans.type === 0 ? 'bg-success' : 'bg-danger'"
                  >
                    {{ trans.type === 0 ? 'Thu' : 'Chi' }}
                  </span>
                </td>
                <td>{{ trans.category?.name || 'N/A' }}</td>
                <td>{{ trans.description }}</td>
                <td class="text-end fw-bold" :class="trans.type === 0 ? 'text-success' : 'text-danger'">
                  {{ trans.type === 0 ? '+' : '-' }}{{ formatCurrency(trans.amount) }}
                </td>
                <td class="text-end">
                  <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" @click="editTransaction(trans)">
                      <i class="bi bi-pencil"></i>
                    </button>
                    <button 
                      class="btn btn-outline-danger" 
                      @click="handleDelete(trans.id)"
                    >
                      <i class="bi bi-trash"></i>
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Empty State -->
        <div v-if="transactions.length === 0" class="empty-state">
          <i class="bi bi-cash-stack"></i>
          <h5>Chưa có giao dịch nào</h5>
          <p>Hãy tạo giao dịch đầu tiên!</p>
        </div>
      </div>

      <!-- Pagination -->
      <div v-if="transactions.length > 0" class="card-footer">
        <div class="d-flex justify-content-between align-items-center">
          <span class="text-muted">
            Hiển thị {{ transactions.length }} / {{ totalTransactions }} giao dịch
          </span>
          <nav>
            <ul class="pagination pagination-sm mb-0">
              <li class="page-item" :class="{ disabled: pagination.page === 1 }">
                <button class="page-link" @click="changePage(pagination.page - 1)">
                  <i class="bi bi-chevron-left"></i>
                </button>
              </li>
              <li 
                v-for="p in displayedPages" 
                :key="p"
                class="page-item"
                :class="{ active: p === pagination.page }"
              >
                <button class="page-link" @click="changePage(p)">{{ p }}</button>
              </li>
              <li class="page-item" :class="{ disabled: pagination.page === pagination.totalPages }">
                <button class="page-link" @click="changePage(pagination.page + 1)">
                  <i class="bi bi-chevron-right"></i>
                </button>
              </li>
            </ul>
          </nav>
        </div>
      </div>
    </div>

    <!-- Create/Edit Transaction Modal -->
    <div class="modal fade" id="transactionModal" tabindex="-1">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              {{ editingTransaction ? 'Sửa giao dịch' : 'Tạo giao dịch mới' }}
            </h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <form @submit.prevent="handleSave">
            <div class="modal-body">
              <!-- Type -->
              <div class="mb-3">
                <label class="form-label">Loại giao dịch <span class="text-danger">*</span></label>
                <div class="btn-group w-100">
                  <input type="radio" class="btn-check" id="type-income" v-model.number="form.type" :value="0">
                  <label class="btn btn-outline-success" for="type-income">
                    <i class="bi bi-arrow-down-circle me-2"></i>Thu
                  </label>
                  <input type="radio" class="btn-check" id="type-expense" v-model.number="form.type" :value="1">
                  <label class="btn btn-outline-danger" for="type-expense">
                    <i class="bi bi-arrow-up-circle me-2"></i>Chi
                  </label>
                </div>
              </div>

              <!-- Amount -->
              <div class="mb-3">
                <label class="form-label">Số tiền <span class="text-danger">*</span></label>
                <div class="input-group">
                  <input 
                    type="number" 
                    class="form-control"
                    v-model.number="form.amount"
                    min="0"
                    step="1000"
                    required
                  >
                  <span class="input-group-text">VNĐ</span>
                </div>
              </div>

              <!-- Category -->
              <div class="mb-3">
                <label class="form-label">Danh mục <span class="text-danger">*</span></label>
                <select class="form-select" v-model.number="form.categoryId" required>
                  <option value="">-- Chọn danh mục --</option>
                  <option 
                    v-for="cat in filteredCategories" 
                    :key="cat.id" 
                    :value="cat.id"
                  >
                    {{ cat.name }}
                  </option>
                </select>
              </div>

              <!-- Description -->
              <div class="mb-3">
                <label class="form-label">Mô tả</label>
                <textarea 
                  class="form-control" 
                  rows="3"
                  v-model="form.description"
                  placeholder="Ghi chú về giao dịch..."
                ></textarea>
              </div>

              <!-- Date -->
              <div class="mb-3">
                <label class="form-label">Ngày giao dịch</label>
                <input 
                  type="date" 
                  class="form-control"
                  v-model="form.transactionDate"
                >
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                Hủy
              </button>
              <button type="submit" class="btn btn-primary" :disabled="isLoading">
                <span v-if="isLoading" class="spinner-border spinner-border-sm me-2"></span>
                {{ editingTransaction ? 'Cập nhật' : 'Tạo mới' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Category Management Modal -->
    <div class="modal fade" id="categoryModal" tabindex="-1">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Quản lý danh mục</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <div class="modal-body">
            <!-- Add new category -->
            <div class="mb-4">
              <h6>Thêm danh mục mới</h6>
              <div class="row g-2">
                <div class="col-5">
                  <input 
                    type="text" 
                    class="form-control"
                    v-model="newCategory.name"
                    placeholder="Tên danh mục"
                  >
                </div>
                <div class="col-4">
                  <select class="form-select" v-model.number="newCategory.type">
                    <option :value="0">Thu</option>
                    <option :value="1">Chi</option>
                  </select>
                </div>
                <div class="col-3">
                  <button 
                    class="btn btn-primary w-100"
                    @click="handleCreateCategory"
                    :disabled="!newCategory.name"
                  >
                    Thêm
                  </button>
                </div>
              </div>
            </div>

            <!-- Existing categories -->
            <h6>Danh mục hiện có</h6>
            <div class="list-group">
              <div 
                v-for="cat in categories" 
                :key="cat.id"
                class="list-group-item d-flex justify-content-between align-items-center"
              >
                <div>
                  <span 
                    class="badge me-2"
                    :class="cat.type === 0 ? 'bg-success' : 'bg-danger'"
                  >
                    {{ cat.type === 0 ? 'Thu' : 'Chi' }}
                  </span>
                  {{ cat.name }}
                </div>
                <button 
                  class="btn btn-sm btn-outline-danger"
                  @click="handleDeleteCategory(cat.id)"
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
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { useTransactionStore } from '@/stores/transaction.store'
import dayjs from 'dayjs'
import { Modal } from 'bootstrap'

const transactionStore = useTransactionStore()

// State
const transactionModal = ref(null)
const categoryModal = ref(null)
const showCategoryModal = ref(false)
const editingTransaction = ref(null)

const filters = reactive({
  type: '',
  categoryId: '',
  fromDate: '',
  toDate: ''
})

const form = reactive({
  type: 0,
  amount: 0,
  categoryId: '',
  description: '',
  transactionDate: dayjs().format('YYYY-MM-DD')
})

const newCategory = reactive({
  name: '',
  type: 0
})

// Computed
const isLoading = computed(() => transactionStore.isLoading)
const transactions = computed(() => transactionStore.transactions)
const categories = computed(() => transactionStore.categories)
const summary = computed(() => transactionStore.summary)
const pagination = computed(() => transactionStore.pagination)
const totalTransactions = computed(() => pagination.value.totalItems)

const filteredCategories = computed(() => {
  return categories.value.filter(c => c.type === form.type)
})

const displayedPages = computed(() => {
  const total = pagination.value.totalPages
  const current = pagination.value.page
  const pages = []
  
  for (let i = Math.max(1, current - 2); i <= Math.min(total, current + 2); i++) {
    pages.push(i)
  }
  
  return pages
})

// Watch
watch(showCategoryModal, (val) => {
  if (val) {
    if (!categoryModal.value) {
      categoryModal.value = new Modal(document.getElementById('categoryModal'))
    }
    categoryModal.value.show()
  }
})

watch(() => form.type, () => {
  // Reset category when type changes
  form.categoryId = ''
})

// Methods
function formatCurrency(value) {
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(value || 0)
}

function formatDate(date) {
  return dayjs(date).format('DD/MM/YYYY')
}

function openCreateModal() {
  editingTransaction.value = null
  Object.assign(form, {
    type: 0,
    amount: 0,
    categoryId: '',
    description: '',
    transactionDate: dayjs().format('YYYY-MM-DD')
  })
  
  if (!transactionModal.value) {
    transactionModal.value = new Modal(document.getElementById('transactionModal'))
  }
  transactionModal.value.show()
}

function editTransaction(trans) {
  editingTransaction.value = trans
  Object.assign(form, {
    type: trans.type,
    amount: trans.amount,
    categoryId: trans.categoryId,
    description: trans.description,
    transactionDate: dayjs(trans.transactionDate).format('YYYY-MM-DD')
  })
  
  if (!transactionModal.value) {
    transactionModal.value = new Modal(document.getElementById('transactionModal'))
  }
  transactionModal.value.show()
}

async function handleSave() {
  try {
    if (editingTransaction.value) {
      await transactionStore.updateTransaction(editingTransaction.value.id, form)
    } else {
      await transactionStore.createTransaction(form)
    }
    transactionModal.value.hide()
    await loadTransactions()
  } catch (err) {
    console.error('Failed to save transaction:', err)
  }
}

async function handleDelete(id) {
  if (confirm('Bạn có chắc muốn xóa giao dịch này?')) {
    await transactionStore.deleteTransaction(id)
  }
}

async function handleCreateCategory() {
  if (!newCategory.name) return
  
  try {
    await transactionStore.createCategory(newCategory)
    newCategory.name = ''
    newCategory.type = 0
  } catch (err) {
    console.error('Failed to create category:', err)
  }
}

async function handleDeleteCategory(id) {
  if (confirm('Bạn có chắc muốn xóa danh mục này?')) {
    await transactionStore.deleteCategory(id)
  }
}

async function loadTransactions() {
  const params = {}
  if (filters.type !== '') params.type = filters.type
  if (filters.categoryId) params.categoryId = filters.categoryId
  if (filters.fromDate) params.fromDate = filters.fromDate
  if (filters.toDate) params.toDate = filters.toDate
  
  await transactionStore.fetchTransactions(params)
}

function resetFilters() {
  filters.type = ''
  filters.categoryId = ''
  filters.fromDate = ''
  filters.toDate = ''
  loadTransactions()
}

async function exportExcel() {
  // TODO: Implement export
  alert('Chức năng đang phát triển')
}

function changePage(page) {
  if (page < 1 || page > pagination.value.totalPages) return
  transactionStore.setPage(page)
  loadTransactions()
}

// Lifecycle
onMounted(async () => {
  await Promise.all([
    loadTransactions(),
    transactionStore.fetchSummary(),
    transactionStore.fetchCategories()
  ])
})
</script>
