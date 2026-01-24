# PCM Web - Hệ thống Quản lý CLB Pickleball "Vợt Thủ Phố Núi"

## 📌 Giới thiệu

Frontend Vue.js 3 cho hệ thống quản lý CLB Pickleball. Hỗ trợ quản lý thành viên, đặt sân, kèo đấu, ghi nhận trận đấu và quản lý quỹ.

## 🛠️ Công nghệ sử dụng

- **Vue.js 3** với Composition API (`<script setup>`)
- **Vue Router 4** - Routing với guards bảo vệ theo role
- **Pinia** - State management
- **Axios** - HTTP client với interceptors
- **Bootstrap 5** - CSS framework
- **Bootstrap Icons** - Icon library
- **vue-toastification** - Toast notifications
- **dayjs** - Date formatting
- **Chart.js** - Biểu đồ thống kê

## 📁 Cấu trúc dự án

```
PCM.Web/
├── public/
│   └── favicon.svg
├── src/
│   ├── api/                    # API layer
│   │   ├── axios.js            # Axios instance với interceptors
│   │   ├── auth.api.js         # Auth endpoints
│   │   ├── members.api.js      # Members CRUD
│   │   ├── courts.api.js       # Courts CRUD
│   │   ├── bookings.api.js     # Bookings management
│   │   ├── challenges.api.js   # Challenges (kèo đấu)
│   │   ├── matches.api.js      # Match recording
│   │   ├── transactions.api.js # Treasury management
│   │   ├── news.api.js         # News/announcements
│   │   └── index.js            # Export all APIs
│   │
│   ├── assets/
│   │   └── styles/
│   │       └── main.scss       # Global styles
│   │
│   ├── components/
│   │   └── common/
│   │       ├── LoadingSpinner.vue
│   │       ├── Pagination.vue
│   │       ├── Modal.vue
│   │       ├── EmptyState.vue
│   │       ├── StatCard.vue
│   │       ├── StatusBadge.vue
│   │       └── DataTable.vue
│   │
│   ├── config/
│   │   └── app.config.js       # App constants & configs
│   │
│   ├── layouts/
│   │   └── MainLayout.vue      # Main app layout
│   │
│   ├── router/
│   │   └── index.js            # Vue Router with guards
│   │
│   ├── stores/                 # Pinia stores
│   │   ├── auth.store.js
│   │   ├── member.store.js
│   │   ├── court.store.js
│   │   ├── booking.store.js
│   │   ├── challenge.store.js
│   │   ├── match.store.js
│   │   ├── transaction.store.js
│   │   └── news.store.js
│   │
│   ├── views/
│   │   ├── auth/
│   │   │   ├── Login.vue
│   │   │   ├── Register.vue
│   │   │   └── ForgotPassword.vue
│   │   │
│   │   ├── Dashboard.vue
│   │   │
│   │   ├── members/
│   │   │   ├── Members.vue
│   │   │   ├── MemberDetail.vue
│   │   │   └── MyProfile.vue
│   │   │
│   │   ├── news/
│   │   │   ├── News.vue
│   │   │   └── NewsDetail.vue
│   │   │
│   │   ├── courts/
│   │   │   └── Courts.vue
│   │   │
│   │   ├── bookings/
│   │   │   └── Bookings.vue
│   │   │
│   │   ├── challenges/
│   │   │   ├── Challenges.vue
│   │   │   └── ChallengeDetail.vue
│   │   │
│   │   ├── matches/
│   │   │   └── Matches.vue
│   │   │
│   │   ├── transactions/
│   │   │   └── Transactions.vue
│   │   │
│   │   ├── profile/
│   │   │   └── Profile.vue
│   │   │
│   │   └── errors/
│   │       ├── Forbidden.vue
│   │       └── NotFound.vue
│   │
│   ├── App.vue
│   └── main.js
│
├── index.html
├── package.json
└── vite.config.js
```

## 🚀 Cài đặt và chạy

### 1. Cài đặt dependencies

```bash
cd PCM.Web
npm install
```

### 2. Cấu hình API URL

Sửa file `vite.config.js` để cấu hình proxy đến API backend:

```javascript
server: {
  port: 3000,
  proxy: {
    '/api': {
      target: 'https://localhost:7001', // URL của API backend
      changeOrigin: true,
      secure: false
    }
  }
}
```

### 3. Chạy development server

```bash
npm run dev
```

Truy cập: http://localhost:3000

### 4. Build production

```bash
npm run build
```

Output nằm trong thư mục `dist/`

## 👥 Roles và quyền hạn

| Role | Quyền |
|------|-------|
| **Admin** | Toàn quyền: CRUD members, courts, news, transactions |
| **Treasurer** | Quản lý quỹ CLB (thu/chi) |
| **Referee** | Ghi nhận kết quả trận đấu |
| **Member** | Đặt sân, tham gia kèo, xem thông tin |

## 🔐 Authentication

- JWT Token được lưu trong `localStorage`
- Axios interceptor tự động attach token vào headers
- Tự động logout khi token hết hạn (401)
- Route guards kiểm tra role trước khi cho phép truy cập

## 📱 Responsive Design

- Mobile-first approach
- Sidebar thu gọn trên mobile
- Cards và tables responsive
- Touch-friendly UI

## 🎨 Theming

Sửa biến CSS trong `src/assets/styles/main.scss`:

```scss
// Primary colors
$primary: #2563eb;
$secondary: #64748b;

// Custom variables
$sidebar-width: 260px;
$header-height: 70px;
```

## 📝 API Endpoints

Frontend gọi đến các endpoints sau:

```
POST   /api/auth/login
POST   /api/auth/register
GET    /api/members
GET    /api/members/{id}
PUT    /api/members/{id}
GET    /api/courts
GET    /api/courts/available-slots
POST   /api/bookings
GET    /api/bookings
GET    /api/challenges
POST   /api/challenges
POST   /api/challenges/{id}/join
POST   /api/matches
GET    /api/transactions
POST   /api/transactions
GET    /api/news
```

## 🔧 Development

### Thêm trang mới

1. Tạo component trong `src/views/`
2. Thêm route trong `src/router/index.js`
3. Thêm API functions trong `src/api/`
4. Tạo store nếu cần trong `src/stores/`

### Convention

- Components: PascalCase (`MemberDetail.vue`)
- Composables: camelCase với prefix `use` (`useAuth`)
- Stores: camelCase với suffix `.store` (`auth.store.js`)
- APIs: camelCase với suffix `.api` (`members.api.js`)

## 📄 License

Private - CLB Pickleball Vợt Thủ Phố Núi
