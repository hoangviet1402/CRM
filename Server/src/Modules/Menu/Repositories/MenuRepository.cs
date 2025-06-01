using System.Data;
using Infrastructure.DbContext;
using Infrastructure.StoredProcedureMapperModule;
using Microsoft.Data.SqlClient;
using Shared.Helpers;
using Shared.Extensions;
using Menu.Entities;
// using SqlMapper.Models; // Uncomment if you have models for Menu

namespace Menu.Repositories;

public class MenuRepository : StoredProcedureMapperModule, IMenuRepository
{
    public MenuRepository(DatabaseConnection dbConnection)
        : base(dbConnection, "TanTam") // Đặt tên DB hoặc schema nếu cần
    {
    }
    // ...Bạn có thể bổ sung các hàm CRUD, gọi stored procedure, mapping dữ liệu tương tự CompanyRepository ở đây...
}
