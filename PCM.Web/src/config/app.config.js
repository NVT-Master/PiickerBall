// Cấu hình ứng dụng
export default {
  // API Configuration
  API_BASE_URL: import.meta.env.VITE_API_URL || '/api',
  
  // App Info
  APP_NAME: 'PCM - Vợt Thủ Phố Núi',
  APP_VERSION: '1.0.0',
  
  // Pagination
  DEFAULT_PAGE_SIZE: 10,
  PAGE_SIZE_OPTIONS: [10, 25, 50, 100],
  
  // Local Storage Keys
  STORAGE_KEYS: {
    TOKEN: 'pcm_token',
    REFRESH_TOKEN: 'pcm_refresh_token',
    USER: 'pcm_user',
    ROLES: 'pcm_roles'
  },
  
  // Roles
  ROLES: {
    ADMIN: 'Admin',
    TREASURER: 'Treasurer',
    REFEREE: 'Referee',
    MEMBER: 'Member'
  },
  
  // Booking
  BOOKING_STATUS: {
    PENDING: 0,
    CONFIRMED: 1,
    CANCELLED: 2,
    COMPLETED: 3
  },
  
  // Challenge
  CHALLENGE_STATUS: {
    OPEN: 0,
    FULL: 1,
    IN_PROGRESS: 2,
    COMPLETED: 3,
    CANCELLED: 4
  },
  
  CHALLENGE_TYPE: {
    SINGLE: 0,
    DOUBLE: 1
  },
  
  // Match
  GAME_MODE: {
    SINGLE: 0,
    DOUBLE: 1
  },
  
  MATCH_FORMAT: {
    BEST_OF_1: 1,
    BEST_OF_3: 3,
    BEST_OF_5: 5
  },
  
  // Treasury warning threshold
  TREASURY_WARNING_THRESHOLD: 0
}
