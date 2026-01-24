/**
 * Vue Router Configuration
 * Cấu hình routing với guards bảo vệ theo role
 */
import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth.store'
import config from '@/config/app.config'

// Lazy load components để tối ưu performance
const Login = () => import('@/views/auth/Login.vue')
const Register = () => import('@/views/auth/Register.vue')
const ForgotPassword = () => import('@/views/auth/ForgotPassword.vue')

const MainLayout = () => import('@/layouts/MainLayout.vue')
const Dashboard = () => import('@/views/Dashboard.vue')
const Members = () => import('@/views/members/Members.vue')
const MemberDetail = () => import('@/views/members/MemberDetail.vue')
const MyProfile = () => import('@/views/members/MyProfile.vue')

const News = () => import('@/views/news/News.vue')
const NewsDetail = () => import('@/views/news/NewsDetail.vue')

const Courts = () => import('@/views/courts/Courts.vue')
const Bookings = () => import('@/views/bookings/Bookings.vue')
const Challenges = () => import('@/views/challenges/Challenges.vue')
const ChallengeDetail = () => import('@/views/challenges/ChallengeDetail.vue')
const Matches = () => import('@/views/matches/Matches.vue')
const Transactions = () => import('@/views/transactions/Transactions.vue')

const Forbidden = () => import('@/views/errors/Forbidden.vue')
const NotFound = () => import('@/views/errors/NotFound.vue')

const { ROLES } = config

const routes = [
  // ==================== PUBLIC ROUTES ====================
  {
    path: '/login',
    name: 'Login',
    component: Login,
    meta: { 
      requiresAuth: false,
      title: 'Đăng nhập'
    }
  },
  {
    path: '/register',
    name: 'Register',
    component: Register,
    meta: { 
      requiresAuth: false,
      title: 'Đăng ký'
    }
  },
  {
    path: '/forgot-password',
    name: 'ForgotPassword',
    component: ForgotPassword,
    meta: { 
      requiresAuth: false,
      title: 'Quên mật khẩu'
    }
  },

  // ==================== PROTECTED ROUTES ====================
  {
    path: '/',
    component: MainLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        redirect: '/dashboard'
      },
      {
        path: 'dashboard',
        name: 'Dashboard',
        component: Dashboard,
        meta: { 
          title: 'Dashboard',
          icon: 'bi-speedometer2'
        }
      },
      
      // Members
      {
        path: 'members',
        name: 'Members',
        component: Members,
        meta: { 
          title: 'Thành viên',
          icon: 'bi-people'
        }
      },
      {
        path: 'members/:id',
        name: 'MemberDetail',
        component: MemberDetail,
        meta: { 
          title: 'Chi tiết thành viên'
        }
      },
      {
        path: 'my-profile',
        name: 'MyProfile',
        component: MyProfile,
        meta: { 
          title: 'Thông tin cá nhân'
        }
      },

      // News
      {
        path: 'news',
        name: 'News',
        component: News,
        meta: { 
          title: 'Tin tức',
          icon: 'bi-newspaper'
        }
      },
      {
        path: 'news/:id',
        name: 'NewsDetail',
        component: NewsDetail,
        meta: { 
          title: 'Chi tiết tin tức'
        }
      },

      // Courts - Admin Only
      {
        path: 'courts',
        name: 'Courts',
        component: Courts,
        meta: { 
          title: 'Quản lý sân',
          icon: 'bi-grid-3x3',
          roles: [ROLES.ADMIN]
        }
      },

      // Bookings
      {
        path: 'bookings',
        name: 'Bookings',
        component: Bookings,
        meta: { 
          title: 'Đặt sân',
          icon: 'bi-calendar-check'
        }
      },

      // Challenges
      {
        path: 'challenges',
        name: 'Challenges',
        component: Challenges,
        meta: { 
          title: 'Sàn đấu',
          icon: 'bi-trophy'
        }
      },
      {
        path: 'challenges/:id',
        name: 'ChallengeDetail',
        component: ChallengeDetail,
        meta: { 
          title: 'Chi tiết kèo'
        }
      },

      // Matches - Referee/Admin
      {
        path: 'matches',
        name: 'Matches',
        component: Matches,
        meta: { 
          title: 'Trận đấu',
          icon: 'bi-controller',
          roles: [ROLES.ADMIN, ROLES.REFEREE]
        }
      },

      // Transactions - Admin/Treasurer
      {
        path: 'transactions',
        name: 'Transactions',
        component: Transactions,
        meta: { 
          title: 'Quản lý quỹ',
          icon: 'bi-wallet2',
          roles: [ROLES.ADMIN, ROLES.TREASURER]
        }
      }
    ]
  },

  // ==================== ERROR ROUTES ====================
  {
    path: '/403',
    name: 'Forbidden',
    component: Forbidden,
    meta: { 
      title: 'Không có quyền truy cập'
    }
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: NotFound,
    meta: { 
      title: 'Không tìm thấy trang'
    }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition
    } else {
      return { top: 0 }
    }
  }
})

// ==================== NAVIGATION GUARDS ====================
router.beforeEach((to, from, next) => {
  // Cập nhật title
  document.title = to.meta.title 
    ? `${to.meta.title} | ${config.APP_NAME}` 
    : config.APP_NAME

  // Lấy auth store
  const authStore = useAuthStore()
  
  // Khởi tạo auth nếu chưa
  if (!authStore.isAuthenticated) {
    authStore.initializeAuth()
  }

  const requiresAuth = to.matched.some(record => record.meta.requiresAuth !== false)
  const requiredRoles = to.meta.roles

  // 1. Route yêu cầu auth nhưng chưa đăng nhập
  if (requiresAuth && !authStore.isAuthenticated) {
    next({
      name: 'Login',
      query: { redirect: to.fullPath }
    })
    return
  }

  // 2. Đã đăng nhập mà vào trang login/register
  if (authStore.isAuthenticated && ['Login', 'Register'].includes(to.name)) {
    next({ name: 'Dashboard' })
    return
  }

  // 3. Kiểm tra role
  if (requiredRoles && requiredRoles.length > 0) {
    const hasPermission = authStore.hasRole(requiredRoles)
    
    if (!hasPermission) {
      next({ name: 'Forbidden' })
      return
    }
  }

  // 4. OK - cho đi tiếp
  next()
})

// After each navigation
router.afterEach((to) => {
  // Có thể thêm analytics tracking ở đây
  // console.log('Navigated to:', to.path)
})

export default router
