using System.Data;
using Infrastructure.DbContext;
using Infrastructure.StoredProcedureMapperModule;
using Microsoft.Data.SqlClient;
using Shared.Helpers;
using Shared.Extensions;
using Company.Entities;

namespace Company.Repositories;

public class CompanyRepository : StoredProcedureMapperModule, ICompanyRepository
{
    public CompanyRepository(DatabaseConnection dbConnection) 
        : base(dbConnection, "TanCa")
    {
    }

    public async Task<int> CreateCompanyAsync(string fullName, string address)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@FullName", fullName },
                { "@Addess", address }
            };

            var outputParameters = new Dictionary<string, object>
            {
                { "@OutResult", 0 }
            };

            return await ExecuteStoredProcedureAsync<int>("Ins_Company_Create", parameters, outputParameters);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("CreateCompany Exception.", ex);
            return 0;
        }
    }

    public async Task<IEnumerable<EmployeeCompany>> GetEmployeeCompanyAsync(int employeeId)
    {
        var result = new List<EmployeeCompany>();

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@EmployeeID", employeeId }
            };

            var dataTable = await ExecuteStoredProcedureAsync<DataTable>("Ins_Company_GetEmployeeID", parameters);

            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new EmployeeCompany
                    {
                        Id = row.GetSafeInt32("Id"),
                        EmployeesAccountId = row.GetSafeInt32("EmployeesAccountId"),
                        CompanyId = row.GetSafeInt32("CompanyId"),
                        FullName = row.GetSafeString("FullName"),
                        Alias = row.GetSafeString("Alias"),
                        Prefix = row.GetSafeString("Prefix"),
                        IsActive = row.GetSafeBoolean("IsActive")
                    });
                }
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"GetEmployeeCompany Exception.", ex);
        }

        return result;
    }

    public async Task<int> CreateBranchAsync(string name, int companyId)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@Name", name },
                { "@CompanyId", companyId }
            };

            var outputParameters = new Dictionary<string, object>
            {
                { "@OutResult", 0 }
            };

            await ExecuteStoredProcedureAsync<int>("Ins_CompanyBranch_Create", parameters, outputParameters);
            return outputParameters.GetSafeInt32("@OutResult");
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("CreateBranch Exception.", ex);
            return 0;
        }
    }

    public async Task<int> CreateDepartmentAsync(string name, int branchId, int companyId)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@Name", name },
                { "@BranchId", branchId },
                { "@CompanyId", companyId }
            };

            var outputParameters = new Dictionary<string, object>
            {
                { "@OutResult", 0 }
            };

            await ExecuteStoredProcedureAsync<int>("Ins_CompanyDepartment_Create", parameters, outputParameters);
            return outputParameters.GetSafeInt32("@OutResult");
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("CreateDepartment Exception.", ex);
            return 0;
        }
    }

    public async Task<int> CreatePositionAsync(string name, int departmentId, int companyId)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@Name", name },
                { "@DepartmentId", departmentId },
                { "@CompanyId", companyId }
            };

            var outputParameters = new Dictionary<string, object>
            {
                { "@OutResult", 0 }
            };

            await ExecuteStoredProcedureAsync<int>("Ins_CompanyPosition_Create", parameters, outputParameters);
            return outputParameters.GetSafeInt32("@OutResult");
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("CreatePosition Exception.", ex);
            return 0;
        }
    }

    public async Task<List<DepartmentCreatedResult>> CreateDepartmentInAllBranchesAsync(string name, int companyId)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@Name", name },
                { "@CompanyId", companyId }
            };

            var outputParameters = new Dictionary<string, object>
            {
                { "@OutResult", 0 }
            };

            var dataTable = await ExecuteStoredProcedureAsync<DataTable>("Ins_CompanyDepartment_CreateInAllBranchId", parameters, outputParameters);
            var result = new List<DepartmentCreatedResult>();

            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new DepartmentCreatedResult
                    {
                        DepartmentId = row.GetSafeInt32("Id"),
                        Name = row.GetSafeString("Name"),
                        CompanyId = row.GetSafeInt32("CompanyId")
                    });
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateDepartmentInAllBranchesAsync Exception Name {name}, CompanyId {companyId} ", ex);
            return new List<DepartmentCreatedResult>();
        }
    }

    public async Task<List<int>> CreateBranchesOptimizedAsync(int companyId, List<string> branchNames)
    {
        try
        {
            // Nếu chỉ có 1 item, sử dụng stored procedure cũ
            if (branchNames.Count == 1)
            {
                var branchId = await CreateBranchAsync(branchNames[0], companyId);
                return branchId > 0 ? new List<int> { branchId } : new List<int>();
            }

            // Nếu có nhiều item, sử dụng stored procedure mới
            var parameters = new Dictionary<string, object>
            {
                { "@CompanyId", companyId },
                { "@BranchNames", string.Join(",", branchNames) }
            };

            var outputParameters = new Dictionary<string, object>
            {
                { "@OutResult", 0 }
            };

            await ExecuteStoredProcedureAsync<int>("Ins_CompanyBranch_CreateMultiple", parameters, outputParameters);
            
            // Lấy danh sách ID đã tạo
            var dataTable = await ExecuteStoredProcedureAsync<DataTable>("Sel_CompanyBranch_GetIds", 
                new Dictionary<string, object> { { "@CompanyId", companyId } });
                
            var result = new List<int>();
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(row.GetSafeInt32("Id"));
                }
            }
                
            return result;
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateBranchesOptimizedAsync Exception CompanyId {companyId}, BranchNames {string.Join(", ", branchNames)} ", ex);
            return new List<int>();
        }
    }

    public async Task<List<DepartmentCreatedResult>> CreateDepartmentsInAllBranchesOptimizedAsync(int companyId, List<string> departmentNames)
    {
        try
        {
            // Nếu chỉ có 1 item, sử dụng stored procedure cũ
            if (departmentNames.Count == 1)
            {
                var departments = await CreateDepartmentInAllBranchesAsync(departmentNames[0], companyId);
                return departments;
            }

            // Nếu có nhiều item, sử dụng stored procedure mới
            var parameters = new Dictionary<string, object>
            {
                { "@CompanyId", companyId },
                { "@DepartmentNames", string.Join(",", departmentNames) }
            };

            var outputParameters = new Dictionary<string, object>
            {
                { "@OutResult", 0 }
            };

            await ExecuteStoredProcedureAsync<int>("Ins_CompanyDepartment_CreateMultipleInAllBranches", parameters, outputParameters);
            
            // Lấy danh sách phòng ban đã tạo
            var dataTable = await ExecuteStoredProcedureAsync<DataTable>("Sel_CompanyDepartment_GetCreated", 
                new Dictionary<string, object> { { "@CompanyId", companyId } });
                
            var result = new List<DepartmentCreatedResult>();
            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new DepartmentCreatedResult
                    {
                        DepartmentId = row.GetSafeInt32("DepartmentId"),
                        Name = row.GetSafeString("Name"),
                        CompanyId = row.GetSafeInt32("CompanyId")
                    });
                }
            }
            
            return result;
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateDepartmentsInAllBranchesOptimizedAsync Exception CompanyId {companyId}, DepartmentNames {string.Join(", ", departmentNames)} ", ex);
            return new List<DepartmentCreatedResult>();
        }
    }
}