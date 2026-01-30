<template>
  <div class="main-layout" :class="{ 'sidebar-collapsed': sidebarCollapsed }">
    <!-- Sidebar -->
    <aside class="sidebar" :class="{ show: !sidebarCollapsed }">
      <div class="sidebar-header">
        <router-link to="/dashboard" class="logo">
          <span class="logo-icon">🏸</span>
          <span class="logo-text">PCM</span>
        </router-link>
      </div>

      <nav class="sidebar-nav">
        <!-- Main Navigation -->
        <div class="nav-section">Menu chính</div>
        
        <ul class="nav flex-column">
          <li class="nav-item">
            <router-link to="/dashboard" class="nav-link">
              <i class="bi bi-speedometer2"></i>
              <span>Dashboard</span>
            </router-link>
          </li>
          
          <li class="nav-item">
            <router-link to="/members" class="nav-link">
              <i class="bi bi-people"></i>
              <span>Thành viên</span>
            </router-link>
          </li>

          <li class="nav-item">
            <router-link to="/news" class="nav-link">
              <i class="bi bi-newspaper"></i>
              <span>Tin tức</span>
            </router-link>
          </li>

          <li class="nav-item">
            <router-link to="/bookings" class="nav-link">
              <i class="bi bi-calendar-check"></i>
              <span>Đặt sân</span>
            </router-link>
          </li>

          <li class="nav-item">
            <router-link to="/challenges" class="nav-link">
              <i class="bi bi-trophy"></i>
              <span>Sàn đấu</span>
            </router-link>
          </li>
        </ul>

        <!-- Admin/Management Section -->
        <template v-if="authStore.isAdmin || authStore.isReferee || authStore.isTreasurer">
          <div class="nav-section mt-4">Quản lý</div>
          
          <ul class="nav flex-column">
            <li class="nav-item" v-if="authStore.isAdmin">
              <router-link to="/courts" class="nav-link">
                <i class="bi bi-grid-3x3"></i>
                <span>Quản lý sân</span>
              </router-link>
            </li>

            <li class="nav-item" v-if="authStore.canManageMatches">
              <router-link to="/matches" class="nav-link">
                <i class="bi bi-controller"></i>
                <span>Trận đấu</span>
              </router-link>
            </li>

            <li class="nav-item" v-if="authStore.canViewTreasury">
              <router-link to="/transactions" class="nav-link">
                <i class="bi bi-wallet2"></i>
                <span>Quản lý quỹ</span>
              </router-link>
            </li>
          </ul>
        </template>
      </nav>

      <!-- Sidebar Footer -->
      <div class="sidebar-footer">
        <small class="text-muted">v1.0.0</small>
      </div>
    </aside>

    <!-- Main Content -->
    <main class="main-content" :class="{ 'sidebar-hidden': sidebarCollapsed }">
      <!-- Navbar -->
      <nav class="navbar-main navbar navbar-expand px-3">
        <!-- Toggle button for all screens -->
        <button 
          class="btn btn-link text-dark me-2"
          @click="toggleSidebar"
          :title="sidebarCollapsed ? 'Mở menu' : 'Ẩn menu'"
        >
          <i class="bi bi-list fs-4"></i>
        </button>

        <div class="d-none d-md-block">
          <nav aria-label="breadcrumb">
            <ol class="breadcrumb mb-0">
              <li class="breadcrumb-item">
                <router-link to="/dashboard">
                  <i class="bi bi-house"></i>
                </router-link>
              </li>
              <li class="breadcrumb-item active" v-if="currentRoute.meta?.title">
                {{ currentRoute.meta.title }}
              </li>
            </ol>
          </nav>
        </div>

        <div class="ms-auto d-flex align-items-center gap-3">
          <!-- Notifications -->
          <div class="dropdown" ref="notificationDropdown">
            <button 
              class="btn btn-link text-dark position-relative"
              @click="toggleNotificationDropdown"
            >
              <i class="bi bi-bell fs-5"></i>
              <span 
                v-if="notificationCount > 0"
                class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger"
              >
                {{ notificationCount }}
              </span>
            </button>
            <div class="dropdown-menu dropdown-menu-end" :class="{ show: showNotificationDropdown }" style="width: 300px;">
              <h6 class="dropdown-header">Thông báo</h6>
              <div class="px-3 py-2 text-muted small">
                Không có thông báo mới
              </div>
            </div>
          </div>

          <!-- User Dropdown -->
          <div class="dropdown" ref="userDropdown">
            <button 
              class="btn btn-link text-dark d-flex align-items-center gap-2"
              @click="toggleUserDropdown"
            >
              <div class="avatar avatar-sm">
                {{ authStore.userInitials }}
              </div>
              <span class="d-none d-md-inline">{{ authStore.userFullName }}</span>
              <i class="bi bi-chevron-down small"></i>
            </button>
            <ul class="dropdown-menu dropdown-menu-end" :class="{ show: showUserDropdown }">
              <li>
                <router-link to="/my-profile" class="dropdown-item" @click="closeAllDropdowns">
                  <i class="bi bi-person me-2"></i>
                  Thông tin cá nhân
                </router-link>
              </li>
              <li>
                <a href="#" class="dropdown-item" @click.prevent="openChangePassword">
                  <i class="bi bi-key me-2"></i>
                  Đổi mật khẩu
                </a>
              </li>
              <li><hr class="dropdown-divider"></li>
              <li>
                <span class="dropdown-item text-muted small">
                  <i class="bi bi-shield-check me-2"></i>
                  {{ authStore.roles.join(', ') || 'Member' }}
                </span>
              </li>
              <li><hr class="dropdown-divider"></li>
              <li>
                <a href="#" class="dropdown-item text-danger" @click.prevent="handleLogout">
                  <i class="bi bi-box-arrow-right me-2"></i>
                  Đăng xuất
                </a>
              </li>
            </ul>
          </div>
        </div>
      </nav>

      <!-- Page Content -->
      <div class="page-content">
        <router-view v-slot="{ Component }">
          <transition name="fade" mode="out-in">
            <component :is="Component" />
          </transition>
        </router-view>
      </div>
    </main>

    <!-- Change Password Modal -->
    <ChangePasswordModal 
      v-if="showChangePassword"
      @close="showChangePassword = false"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'
import ChangePasswordModal from '@/components/common/ChangePasswordModal.vue'

const route = useRoute()
const authStore = useAuthStore()

// Default to showing sidebar (collapsed = false means sidebar is visible)
const sidebarCollapsed = ref(localStorage.getItem('sidebarCollapsed') === 'true')
const showChangePassword = ref(false)
const notificationCount = ref(0)

// Dropdown states
const showUserDropdown = ref(false)
const showNotificationDropdown = ref(false)
const userDropdown = ref(null)
const notificationDropdown = ref(null)

const currentRoute = computed(() => route)

function toggleSidebar() {
  sidebarCollapsed.value = !sidebarCollapsed.value
  localStorage.setItem('sidebarCollapsed', sidebarCollapsed.value.toString())
}

function toggleUserDropdown() {
  showUserDropdown.value = !showUserDropdown.value
  showNotificationDropdown.value = false
}

function toggleNotificationDropdown() {
  showNotificationDropdown.value = !showNotificationDropdown.value
  showUserDropdown.value = false
}

function closeAllDropdowns() {
  showUserDropdown.value = false
  showNotificationDropdown.value = false
}

function openChangePassword() {
  closeAllDropdowns()
  showChangePassword.value = true
}

function handleClickOutside(event) {
  if (userDropdown.value && !userDropdown.value.contains(event.target)) {
    showUserDropdown.value = false
  }
  if (notificationDropdown.value && !notificationDropdown.value.contains(event.target)) {
    showNotificationDropdown.value = false
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})

function handleLogout() {
  closeAllDropdowns()
  authStore.logout()
}
</script>

<style scoped>
.main-layout {
  min-height: 100vh;
}

.sidebar {
  display: flex;
  flex-direction: column;
}

.sidebar-nav {
  flex: 1;
  overflow-y: auto;
}
</style>
