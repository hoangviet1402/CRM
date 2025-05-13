# SQL Mapper

Ứng dụng .NET Core để tự động tạo các lớp C# từ kết quả của stored procedure trong SQL Server.

## Tính năng

- Kết nối đến SQL Server thông qua chuỗi kết nối
- Tự động tạo định nghĩa lớp C# dựa trên kết quả của stored procedure
- Hỗ trợ ánh xạ tất cả các kiểu dữ liệu SQL sang C# tương ứng
- Các API endpoint để tạo lớp
- Lưu các lớp đã tạo vào thư mục Models

## Bắt đầu

### Yêu cầu

- .NET 8 SDK hoặc mới hơn
- Cài đặt SQL Server

### Cấu hình

1. Cập nhật chuỗi kết nối trong `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TênServer;Database=TênDatabase;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Thay thế `TênServer` và `TênDatabase` với thông tin SQL Server và cơ sở dữ liệu của bạn.

### Chạy ứng dụng

```bash
cd SqlMapper
dotnet run
```

Ứng dụng sẽ khởi động và giao diện Swagger UI sẽ có sẵn tại:
- https://localhost:7xxx/swagger (trong đó xxx là số cổng)

## Cách sử dụng

### Sử dụng API

#### Tạo và xem định nghĩa lớp

```
GET /api/ModelGenerator/generate?procedureName=TênStoredProcedure
```

API này sẽ trả về định nghĩa lớp C# dựa trên kết quả của stored procedure. Tên lớp sẽ được tạo tự động từ tên stored procedure + "_Result". Nếu bạn muốn đặt tên lớp cụ thể, bạn có thể thêm tham số `className`:

```
GET /api/ModelGenerator/generate?procedureName=TênStoredProcedure&className=TênLớp
```

#### Tạo và lưu tệp lớp

```
POST /api/ModelGenerator/generate?procedureName=TênStoredProcedure
```

API này sẽ tạo lớp C# và lưu vào thư mục Models. Tên lớp sẽ được tạo tự động từ tên stored procedure + "_Result". Nếu bạn muốn đặt tên lớp cụ thể, bạn có thể thêm tham số `className`:

```
POST /api/ModelGenerator/generate?procedureName=TênStoredProcedure&className=TênLớp
```

### Sử dụng Service trực tiếp

Bạn cũng có thể tiêm và sử dụng `IModelGeneratorService` trong các controller hoặc service khác:

```csharp
// Tạo định nghĩa lớp
string classDefinition = await _modelGeneratorService.GenerateClassFromStoredProcedureAsync(
    "TênStoredProcedure", 
    "TênLớp");

// Tạo và lưu vào tệp
string filePath = await _modelGeneratorService.GenerateAndSaveClassAsync(
    "TênStoredProcedure", 
    "TênLớp");
```

## Ví dụ

Nếu bạn có một stored procedure tên là `GetEmployees` trả về dữ liệu nhân viên, bạn có thể tạo một lớp cho nó:

```
GET /api/ModelGenerator/generate?procedureName=GetEmployees&className=Employee
```

Điều này sẽ tạo ra một lớp như sau:

```csharp
using System;
using System.Collections.Generic;

namespace SqlMapper.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? HireDate { get; set; }
        public decimal? Salary { get; set; }
        // ... các thuộc tính khác dựa trên các cột kết quả của stored procedure
    }
}
```

## Giấy phép

Dự án này được cấp phép theo Giấy phép MIT. 