# StoredProcedureMapperModule

Module xử lý việc thực thi stored procedure và quản lý kết nối database.

## Cấu trúc

```csharp
public class StoredProcedureMapperModule
{
    protected readonly DatabaseConnection _dbConnection;
    protected readonly string _connectionName;
    private const int DEFAULT_COMMAND_TIMEOUT = 30; // 30 giây

    public StoredProcedureMapperModule(DatabaseConnection dbConnection = null, string connectionName = null)
    {
        _dbConnection = dbConnection;
        _connectionName = connectionName;
    }
}
```

## Cách sử dụng

### 1. Khởi tạo Repository

```csharp
public class YourRepository : StoredProcedureMapperModule
{
    public YourRepository(DatabaseConnection dbConnection)
        : base(dbConnection, "TanCa")  // Tên connection string
    {
    }
}
```

### 2. Thực thi Stored Procedure

#### 2.1. Trả về một entity
```csharp
// Không có output parameters
var loginResult = await ExecuteStoredProcedureAsync<LoginResultEntities>("Ins_Account_Login", parameters);

// Có output parameters
var outputParams = new Dictionary<string, object>
{
    { "@AccountId", 0 },
    { "@Status", 0 }
};
var result = await ExecuteStoredProcedureAsync<int>("Ins_Account_Register", parameters, outputParams);
var accountId = outputParams.GetSafeInt32("@AccountId");
var status = outputParams.GetSafeInt32("@Status");
```

#### 2.2. Trả về danh sách entity
```csharp
var companies = await ExecuteStoredProcedureListAsync<CompanyAccountMapEntities>("Ins_Account_GetAllCompany", parameters);
```

#### 2.3. Trả về DataTable
```csharp
var dataTable = await ExecuteStoredProcedureAsync<DataTable>("Ins_Account_GetAll", parameters);
```

#### 2.4. Trả về bool
```csharp
var success = await ExecuteStoredProcedureAsync<bool>("Ins_Account_CheckStatus", parameters, outputParams);
```

#### 2.5. Trả về int
```csharp
var result = await ExecuteStoredProcedureAsync<int>("Ins_Account_Register", parameters, outputParams);
```

### 3. Cấu hình Timeout

Mặc định timeout là 30 giây. Bạn có thể tùy chỉnh timeout cho từng stored procedure:

```csharp
// Sử dụng timeout mặc định (30 giây)
var result = await ExecuteStoredProcedureAsync<int>("Ins_Account_Register", parameters, outputParams);

// Tùy chỉnh timeout (60 giây)
var result = await ExecuteStoredProcedureAsync<int>("Ins_Account_Register", parameters, outputParams, 60);

// Tùy chỉnh timeout (5 phút)
var result = await ExecuteStoredProcedureAsync<int>("Ins_Account_Register", parameters, outputParams, 300);
```

### 4. Xử lý lỗi Timeout

```csharp
try
{
    var result = await ExecuteStoredProcedureAsync<int>("Ins_Account_Register", parameters, outputParams);
}
catch (TimeoutException ex)
{
    // Xử lý lỗi timeout
    LoggerHelper.Error($"Operation timed out: {ex.Message}");
    // Có thể thử lại hoặc thông báo cho user
}
```

### 5. Sử dụng Nullable Types

Module hỗ trợ các kiểu dữ liệu nullable:

```csharp
public class EmployeeEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? Age { get; set; }           // Nullable int
    public bool? IsActive { get; set; }     // Nullable bool
    public DateTime? LastLogin { get; set; } // Nullable DateTime
    public decimal? Salary { get; set; }     // Nullable decimal
}
```

Khi cột trong database là NULL:
- Với non-nullable types: sẽ giữ giá trị mặc định (0, false, DateTime.MinValue)
- Với nullable types: sẽ được set là null

### 6. Lưu ý quan trọng

1. **Tên property trong entity phải khớp với tên cột trong kết quả stored procedure**
   ```csharp
   // Nếu stored procedure trả về cột "EmployeeId"
   public int EmployeeId { get; set; }  // Đúng
   public int Id { get; set; }          // Sai
   ```

2. **Kiểu dữ liệu của property phải tương thích với kiểu dữ liệu của cột**
   ```csharp
   // Nếu cột là INT
   public int Id { get; set; }          // Đúng
   public string Id { get; set; }       // Sai
   ```

3. **Xử lý lỗi**
   ```csharp
   try
   {
       var result = await ExecuteStoredProcedureAsync<YourEntity>("YourStoredProcedure", parameters);
   }
   catch (TimeoutException ex)
   {
       // Xử lý lỗi timeout
   }
   catch (Exception ex)
   {
       // Xử lý các lỗi khác
   }
   ```

4. **Performance**
   - Module sử dụng cache cho properties để tăng hiệu suất
   - Sử dụng HashSet để tối ưu việc tìm kiếm tên cột
   - Tự động chuyển đổi kiểu dữ liệu phù hợp

### 7. Ví dụ thực tế

Xem các repository trong project để tham khảo cách sử dụng:
- `AuthRepository.cs`
- `CompanyRepository.cs`
- `EmployeeRepository.cs` 