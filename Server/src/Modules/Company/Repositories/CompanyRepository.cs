using System.Data;
using Infrastructure.DbContext;
using Infrastructure.StoredProcedureMapperModule;
using Microsoft.Data.SqlClient;
using Shared.Helpers;
using Shared.Extensions;
using Company.Entities;
using SqlMapper.Models;

namespace Company.Repositories;

public class CompanyRepository : StoredProcedureMapperModule, ICompanyRepository
{
    public CompanyRepository(DatabaseConnection dbConnection)
        : base(dbConnection, "TanTam")
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

    public async Task<int> CreateBrancheAsync(CreateBranchesRequest request, int companyId)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@BranchName", request.Name },
                { "@BranchAddress", request.Address },
                { "@RegionId", request.RegionId },
                { "@IsOnboarding", request.IsOnboarding },
                { "@Latitude", request.Latitude },
                { "@Longitude", request.Longitude },
                { "@CompanyId", companyId },
                { "@Alias", TextHelper.NormalizeText(request.Name,"-") },
                { "@Code", TextHelper.NormalizeText(request.Name,"_").ToUpper() }
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
            return await ExecuteStoredProcedureListAsync<DepartmentCreatedResult>("Ins_CompanyDepartment_CreateInAllBranchId", parameters, outputParameters);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateDepartmentInAllBranchesAsync Exception Name {name}, CompanyId {companyId} ", ex);
            return new List<DepartmentCreatedResult>();
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

            return await ExecuteStoredProcedureListAsync<DepartmentCreatedResult>("Ins_CompanyDepartment_CreateMultipleInAllBranches", parameters, outputParameters);

        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateDepartmentsInAllBranchesOptimizedAsync Exception CompanyId {companyId}, DepartmentNames {string.Join(", ", departmentNames)} ", ex);
            return new List<DepartmentCreatedResult>();
        }
    }

    public async Task<int> UpdateCompanyStepAsync(int companyId, int code)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@CompanyId", companyId },
                { "@Code", code },
            };

            var outputParameters = new Dictionary<string, object>
            {
                { "@OutResult", 0 }
            };

            await ExecuteStoredProcedureAsync<int>("Ins_ComPany_UpdateSetupStep", parameters, outputParameters);
            return outputParameters.GetSafeInt32("@OutResult");
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"UpdateCompanyStepAsync Exception code {code}, CompanyId {companyId} ", ex);
            return 0;
        }
    }

    public async Task<List<Ins_Business_GetList_Result>> BusinessGetListAsync()
    {
        var result = new List<Ins_Business_GetList_Result>();
        try
        {
            return await ExecuteStoredProcedureListAsync<Ins_Business_GetList_Result>("Ins_Account_Login");
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("BusinessGetListAsync Exception.", ex);
        }
        return result;
    }

    public async Task<int> UpdateInfoWhenSinup(UpdateInfoWhenSinupRequest request)
    {
        try
        {
           
            var parameters = new Dictionary<string, object>
            {
                { "@AccountId", request.AccountId },
                { "@CompanyId", request.CompanyId },
                { "@CompanyName", request.CompanyName },
                { "@ComPany_Latitude", request.CompanyLatitude },
                { "@ComPany_Longitude", request.CompanyLongitude },
                { "@ComPany_Number_Employee", request.CompanyNumberEmploye },
                { "@ComPany_Address", request.CompanyAddress},
                { "@Email", request.Email },
                { "@HearAbout", request.HearAbout},
                { "@UsePurpose", request.UsePurpose }
            };

            await ExecuteStoredProcedureAsync<int>("Ins_Company_UpdateInfoWhenSinup", parameters);
            return 1;
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateSetupStepAsync Exception CompanyId {request.CompanyId},AccountId {request.AccountId} ", ex);
            return 0;
        }
    }    
}