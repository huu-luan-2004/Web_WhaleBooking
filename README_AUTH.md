# WhaleBooking Authentication Setup

## Cấu hình Firebase

### 1. Tạo project Firebase
1. Truy cập [Firebase Console](https://console.firebase.google.com/)
2. Tạo project mới hoặc chọn project có sẵn
3. Kích hoạt Authentication
4. Cấu hình các phương thức đăng nhập:
   - Email/Password
   - Google
   - Phone (SMS)

### 2. Lấy Firebase Config
1. Trong Firebase Console, vào Project Settings
2. Tại tab "General", cuộn xuống "Your apps"
3. Chọn "Web app" và copy config object
4. Cập nhật config trong các file sau:

**File: `wwwroot/js/whalebooking.js`**
```javascript
firebaseConfig: {
    apiKey: "your-api-key",
    authDomain: "your-project.firebaseapp.com",
    projectId: "your-project-id",
    storageBucket: "your-project.appspot.com",
    messagingSenderId: "123456789",
    appId: "your-app-id"
}
```

**File: `Views/Auth/Login.cshtml`**
```javascript
const firebaseConfig = {
    // Copy từ Firebase Console
};
```

**File: `Views/Auth/Dashboard.cshtml`**
```javascript
const firebaseConfig = {
    // Copy từ Firebase Console
};
```

### 3. Cấu hình Authentication Methods

#### Email/Password
1. Trong Firebase Console → Authentication → Sign-in method
2. Bật "Email/Password"

#### Google Sign-In
1. Bật "Google" trong Sign-in method
2. Thêm domain của website vào "Authorized domains"

#### Phone Authentication
1. Bật "Phone" trong Sign-in method
2. Cấu hình Cloud Messaging nếu cần

## Backend API Requirements

Backend cần implement các API endpoints sau:

### 1. Login với Firebase Token
```
POST /api/auth/login
Header: Content-Type: application/json
Body: { "idToken": "<firebase_id_token>" }

Response:
{
  "success": true,
  "message": "Đăng nhập thành công",
  "data": {
    "user": { "id": 12, "email": "...", "hoTen": "...", "vaiTro": "KhachHang" },
    "roles": ["KhachHang"],
    "permissions": [...],
    "token": "<jwt_backend>"
  }
}
```

### 2. Lấy thông tin user hiện tại
```
GET /api/user/me
Header: Authorization: Bearer <jwt_backend>

Response:
{
  "success": true,
  "data": {
    "id": 12,
    "email": "user@example.com",
    "hoTen": "Nguyễn Văn A",
    "vaiTro": "KhachHang"
  }
}
```

### 3. Cấp quyền (Admin only)
```
PATCH /api/user/{id}/role
Header: Authorization: Bearer <jwt_admin>
Body: { "vaiTro": "ChuCoSo" }

Response:
{
  "success": true,
  "message": "Cấp quyền thành công"
}
```

## URL Configuration

### Development
- Web: `http://localhost:5000/api`

### Android Emulator
- API: `http://10.0.2.2:5000/api`

### Production
Cập nhật URL trong `whalebooking.js`:
```javascript
getApiUrl: function() {
    return 'https://your-api-domain.com/api';
}
```

## User Roles

### KhachHang (Customer)
- Tìm kiếm tour
- Đặt lịch
- Xem lịch sử booking

### ChuCoSo (Owner)
- Tất cả tính năng KhachHang
- Thêm/quản lý tour
- Xem thống kê
- Quản lý booking

### Admin
- Tất cả tính năng
- Quản lý người dùng
- Cấp quyền
- Cài đặt hệ thống

## Security Notes

1. **Firebase Security Rules**: Cấu hình rules phù hợp
2. **CORS**: Cấu hình CORS cho domain của bạn
3. **JWT Token**: Set thời gian hết hạn phù hợp
4. **HTTPS**: Sử dụng HTTPS trong production

## Testing

### Test Firebase Authentication
1. Truy cập `/Auth/Login`
2. Thử đăng nhập bằng email/password
3. Thử đăng nhập bằng Google
4. Thử đăng nhập bằng phone

### Test Role Management
1. Đăng nhập với tài khoản Admin
2. Truy cập Dashboard
3. Thử cấp quyền ChuCoSo cho user khác
4. Kiểm tra user info

## Troubleshooting

### Common Issues

1. **Firebase Config Error**
   - Kiểm tra API key và project ID
   - Đảm bảo domain được authorize

2. **CORS Error**
   - Thêm domain vào Firebase authorized domains
   - Cấu hình CORS trên backend

3. **401 Unauthorized**
   - Kiểm tra JWT token
   - Verify token trên backend

4. **Phone Auth Error**
   - Kiểm tra cấu hình SMS
   - Verify phone number format

### Debug Tips

1. Mở Developer Tools → Console để xem lỗi
2. Kiểm tra Network tab để xem API calls
3. Verify Firebase token tại [jwt.io](https://jwt.io)