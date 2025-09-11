# ShopFlow Role-Based Access Control (RBAC) System

## Tổng quan

Hệ thống phân quyền role-based cho ShopFlow E-commerce với các tính năng:

- **Authentication**: Xác thực người dùng qua JWT Token
- **Authorization**: Phân quyền dựa trên Role và Permission
- **Role-based Navigation**: Chuyển hướng trang dựa trên role
- **Security**: Bảo mật API endpoints và trang web

## Cấu trúc Role và Permission

### Roles Hierarchy

```
ADMIN (Super Admin)
├── MODERATOR (Content Moderator)
├── VENDOR_STAFF (Vendor Management)
├── WAREHOUSE_STAFF (Logistics)
└── CUSTOMER (End User)
    └── GUEST (Anonymous)
```

### Roles Chi tiết

#### 1. ADMIN - System Administrator

**Trang mặc định**: `/admin/dashboard`
**Quyền**: Toàn quyền hệ thống

- Quản lý users, roles, permissions
- Quản lý products, orders, vendors
- Xem reports và analytics
- Cấu hình hệ thống

#### 2. MODERATOR - Content Moderator

**Trang mặc định**: `/moderator/dashboard`
**Quyền**: Kiểm duyệt nội dung

- Duyệt và kiểm duyệt reviews
- Xử lý reports từ users
- Quản lý content và moderation
- Xem thông tin products và orders

#### 3. VENDOR_STAFF - Vendor Staff

**Trang mặc định**: `/vendor/dashboard`
**Quyền**: Quản lý vendor

- Quản lý products của vendor
- Xử lý orders cho vendor
- Quản lý inventory
- Xem customer information

#### 4. WAREHOUSE_STAFF - Warehouse Staff

**Trang mặc định**: `/warehouse/dashboard`
**Quyền**: Quản lý kho

- Quản lý inventory và stock
- Xử lý shipments
- Quản lý warehouse operations
- Xem và xử lý orders

#### 5. CUSTOMER - Regular Customer

**Trang mặc định**: `/customer/dashboard`
**Quyền**: Mua sắm

- Đặt hàng và xem orders
- Quản lý profile cá nhân
- Viết reviews và đánh giá
- Quản lý cart và wishlist

#### 6. GUEST - Anonymous User

**Trang mặc định**: `/guest/catalog`
**Quyền**: Xem công khai

- Xem products catalog
- Truy cập public pages
- Đăng ký và đăng nhập

## API Authentication & Authorization

### 1. Login Flow

```http
POST /api/v1/users/login
Content-Type: application/json

{
  "email": "admin@shopflow.com",
  "password": "password123"
}
```

**Response:**

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64_refresh_token",
  "expiresAt": "2024-01-01T12:00:00Z",
  "user": {
    "id": 1,
    "email": "admin@shopflow.com",
    "fullName": "System Administrator",
    "roles": ["ADMIN"],
    "primaryRole": "ADMIN"
  }
}
```

### 2. JWT Token Claims

```json
{
  "sub": "1", // User ID
  "email": "admin@shopflow.com",
  "role": ["ADMIN"], // User roles
  "permission": [
    // User permissions
    "ADMIN_FULL_ACCESS",
    "MANAGE_USERS",
    "VIEW_PRODUCTS"
  ],
  "exp": 1641024000,
  "iss": "ShopFlow",
  "aud": "ShopFlow"
}
```

### 3. Role-based Page Routing

```http
GET /api/v1/navigation/default-page
Authorization: Bearer {token}
```

**Response:**

```json
{
  "defaultPage": "/admin/dashboard",
  "primaryRole": "ADMIN"
}
```

### 4. Dashboard Configuration

```http
GET /api/v1/navigation/dashboard-config
Authorization: Bearer {token}
```

**Response:**

```json
{
  "title": "Admin Dashboard",
  "welcomeMessage": "Welcome to ShopFlow Admin Panel",
  "menuItems": [
    {
      "name": "Users",
      "url": "/admin/users",
      "icon": "users",
      "requiredPermissions": ["MANAGE_USERS"]
    }
  ],
  "quickActions": [
    {
      "name": "Add User",
      "url": "/admin/users/create",
      "description": "Create a new user account",
      "icon": "user-plus"
    }
  ]
}
```

## Frontend Implementation Examples

### 1. Login Component (React/Vue)

```javascript
// Login.js
async function handleLogin(email, password) {
  try {
    const response = await fetch("/api/v1/users/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password }),
    });

    const data = await response.json();

    // Store token
    localStorage.setItem("accessToken", data.accessToken);
    localStorage.setItem("user", JSON.stringify(data.user));

    // Redirect to role-based page
    const defaultPage = await getDefaultPage();
    window.location.href = defaultPage;
  } catch (error) {
    console.error("Login failed:", error);
  }
}

async function getDefaultPage() {
  const token = localStorage.getItem("accessToken");
  const response = await fetch("/api/v1/navigation/default-page", {
    headers: { Authorization: `Bearer ${token}` },
  });
  const data = await response.json();
  return data.defaultPage;
}
```

### 2. Role-based Route Guard

```javascript
// RouteGuard.js
export function createRoleGuard(allowedRoles) {
  return async (to, from, next) => {
    const user = JSON.parse(localStorage.getItem("user") || "{}");
    const userRoles = user.roles || [];

    // Check if user has any of the allowed roles
    const hasAccess = allowedRoles.some((role) => userRoles.includes(role));

    if (hasAccess) {
      next();
    } else {
      // Check with server for real-time validation
      const canAccess = await checkPageAccess(to.path);
      if (canAccess) {
        next();
      } else {
        next("/unauthorized");
      }
    }
  };
}

async function checkPageAccess(pageUrl) {
  const token = localStorage.getItem("accessToken");
  const response = await fetch(
    `/api/v1/navigation/can-access?pageUrl=${pageUrl}`,
    { headers: { Authorization: `Bearer ${token}` } }
  );
  const data = await response.json();
  return data.canAccess;
}

// Usage in route configuration
const routes = [
  {
    path: "/admin",
    component: AdminDashboard,
    beforeEnter: createRoleGuard(["ADMIN"]),
  },
  {
    path: "/moderator",
    component: ModeratorDashboard,
    beforeEnter: createRoleGuard(["ADMIN", "MODERATOR"]),
  },
  {
    path: "/customer",
    component: CustomerDashboard,
    beforeEnter: createRoleGuard(["CUSTOMER"]),
  },
];
```

### 3. Dynamic Menu Component

```javascript
// DynamicMenu.js
import React, { useState, useEffect } from "react";

export function DynamicMenu() {
  const [dashboardConfig, setDashboardConfig] = useState(null);
  const [userPermissions, setUserPermissions] = useState([]);

  useEffect(() => {
    loadDashboardConfig();
    loadUserContext();
  }, []);

  async function loadDashboardConfig() {
    const token = localStorage.getItem("accessToken");
    const response = await fetch("/api/v1/navigation/dashboard-config", {
      headers: { Authorization: `Bearer ${token}` },
    });
    const config = await response.json();
    setDashboardConfig(config);
  }

  async function loadUserContext() {
    const token = localStorage.getItem("accessToken");
    const response = await fetch("/api/v1/navigation/user-context", {
      headers: { Authorization: `Bearer ${token}` },
    });
    const context = await response.json();
    setUserPermissions(context.permissions);
  }

  function hasPermission(requiredPermissions) {
    return requiredPermissions.some((permission) =>
      userPermissions.includes(permission)
    );
  }

  if (!dashboardConfig) return <div>Loading...</div>;

  return (
    <nav className="dashboard-menu">
      <h2>{dashboardConfig.title}</h2>
      <p>{dashboardConfig.welcomeMessage}</p>

      <ul className="menu-items">
        {dashboardConfig.menuItems
          .filter((item) => hasPermission(item.requiredPermissions))
          .map((item) => (
            <li key={item.url}>
              <a href={item.url}>
                <i className={`icon-${item.icon}`}></i>
                {item.name}
              </a>
            </li>
          ))}
      </ul>

      <div className="quick-actions">
        <h3>Quick Actions</h3>
        {dashboardConfig.quickActions.map((action) => (
          <button
            key={action.url}
            onClick={() => (window.location.href = action.url)}
          >
            <i className={`icon-${action.icon}`}></i>
            {action.name}
          </button>
        ))}
      </div>
    </nav>
  );
}
```

## Backend Implementation

### 1. Controller Authorization

```csharp
[ApiController]
[Route("api/v1/[controller]")]
[Authorize] // Require authentication for all actions
public class ProductsController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous] // Public endpoint
    public async Task<IActionResult> GetProducts() { }

    [HttpPost]
    [RequireRole(RoleCode.ADMIN, RoleCode.VENDOR_STAFF)]
    [RequirePermission(PermissionCode.CREATE_PRODUCTS)]
    public async Task<IActionResult> CreateProduct() { }

    [HttpPut("{id}")]
    [AdminOnly] // Only admin can update any product
    public async Task<IActionResult> UpdateProduct(long id) { }

    [HttpDelete("{id}")]
    [RequirePermission(PermissionCode.DELETE_PRODUCTS)]
    public async Task<IActionResult> DeleteProduct(long id) { }
}
```

### 2. Service Layer Authorization

```csharp
public class ProductService : IProductService
{
    private readonly IUserRoleService _userRoleService;
    private readonly ICurrentUserService _currentUserService;

    public async Task<Product> CreateProductAsync(CreateProductRequest request)
    {
        var currentUserId = _currentUserService.UserId;

        // Check if user has permission
        var hasPermission = await _userRoleService.HasPermissionAsync(
            currentUserId, PermissionCode.CREATE_PRODUCTS);

        if (!hasPermission)
        {
            throw new UnauthorizedAccessException("Insufficient permissions");
        }

        // Business logic here...
    }
}
```

## Security Best Practices

### 1. JWT Token Security

- Tokens expire sau 1 giờ
- Refresh tokens để gia hạn
- Store tokens securely (httpOnly cookies preferred)
- Validate tokens trên mọi protected endpoints

### 2. Role Validation

- Server-side validation cho mọi requests
- Real-time role checks cho sensitive operations
- Audit logging cho role changes

### 3. Page Access Control

- Frontend route guards
- Backend API authorization
- Real-time permission checks
- Graceful fallbacks cho unauthorized access

### 4. Error Handling

```javascript
// Centralized error handling
axios.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Token expired or invalid
      localStorage.removeItem("accessToken");
      window.location.href = "/login";
    } else if (error.response?.status === 403) {
      // Insufficient permissions
      window.location.href = "/unauthorized";
    }
    return Promise.reject(error);
  }
);
```

## Testing Scenarios

### 1. Role Assignment Testing

```csharp
[Test]
public async Task Login_WithAdminRole_ShouldRedirectToAdminDashboard()
{
    // Arrange
    var adminUser = CreateUserWithRole(RoleCode.ADMIN);

    // Act
    var response = await LoginAsync(adminUser.Email, "password");

    // Assert
    Assert.Equal("/admin/dashboard", response.User.PrimaryRole);
}
```

### 2. Permission Testing

```csharp
[Test]
public async Task CreateProduct_WithoutPermission_ShouldReturnForbidden()
{
    // Arrange
    var customerUser = CreateUserWithRole(RoleCode.CUSTOMER);
    var token = GenerateTokenForUser(customerUser);

    // Act
    var response = await PostAsync("/api/v1/products", token);

    // Assert
    Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
}
```

## Deployment Configuration

### 1. Environment Variables

```bash
# JWT Configuration
JWT_SECRET_KEY=your-super-secret-key-here
JWT_ISSUER=ShopFlow
JWT_AUDIENCE=ShopFlow
JWT_EXPIRATION_MINUTES=60

# Database
DB_CONNECTION_STRING=your-database-connection-string

# Redis (for distributed caching)
REDIS_CONNECTION_STRING=your-redis-connection-string
```

### 2. Production Security

- Use HTTPS everywhere
- Secure JWT secret keys
- Enable CORS properly
- Implement rate limiting
- Set up monitoring and alerting

## Migration và Maintenance

### 1. Adding New Roles

1. Thêm role code vào `RoleCode.cs`
2. Thêm permissions vào `PermissionCode.cs`
3. Cập nhật `PageRoutingService`
4. Tạo database migration
5. Update frontend route guards

### 2. Permission Changes

1. Modify `PermissionCode` constants
2. Update role-permission mappings
3. Update API authorization attributes
4. Test all affected endpoints

Hệ thống này cung cấp bảo mật toàn diện với khả năng mở rộng và bảo trì dễ dàng cho ứng dụng ShopFlow E-commerce.
