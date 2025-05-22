using EmployeeModule.DTOs;
using EmployeeModule.Entities;
using Infrastructure.DbContext;
using Microsoft.Data.SqlClient;
using System.Data;
using Shared.Result;
using Shared.Helpers;
using Shared.Enums;
using Infrastructure.StoredProcedureMapperModule;

namespace EmployeeModule.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly DatabaseConnection _dbConnection;
    private readonly StoredProcedureMapperModule _storedProcedureMapper;

    public EmployeeRepository(DatabaseConnection dbConnection)
    {
        _dbConnection = dbConnection;
        _storedProcedureMapper = new StoredProcedureMapperModule();
    }

    public async Task<EmployeeCreate_ResultEntities> EmployeeRegister(string fullname, string employeesCode, string phone, string email, string password, int companyId, int role)
    {
        using var connection = _dbConnection.CreateConnection("Default");
        var response = new EmployeeCreate_ResultEntities();

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@FullName", fullname },
                { "@EmployeesCode", employeesCode },
                { "@Phone", phone },
                { "@Email", email },
                { "@Password", string.IsNullOrEmpty(password) ? "" : AESHelper.HashPassword(password) },
                { "@CompanyId", companyId },
                { "@Role", role }
            };

            var outputParameters = new Dictionary<string, object>();
            var success = await _storedProcedureMapper.ExecuteStoredProcedureAsync(connection, "Ins_Employee_Create", parameters, outputParameters);

            if (success)
            {
                response.EmployeeAccountId = outputParameters.GetSafeInt32("@EmployeeAccountId");
                response.IsNewUser = outputParameters.GetSafeInt32("@IsNewUser");
                response.NeedSetPassword = outputParameters.GetSafeInt32("@NeedSetPassword");
                response.NeedSetCompany = outputParameters.GetSafeInt32("@NeedSetCompany");
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"EmployeeRegister Exception.", ex);
            throw;
        }

        return response;
    }

    public async Task<EmployeeEntity?> GetEmployeeById(int id, int companyId)
    {
        using var connection = _dbConnection.CreateConnection("Default");
        EmployeeEntity? result = null;

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@EmployeeId", id },
                { "@CompanyId", companyId }
            };

            var dataTable = await _storedProcedureMapper.ExecuteStoredProcedureWithResultAsync(connection, "Ins_Employee_GetById", parameters);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                result = new EmployeeEntity
                {
                    Id = row.GetSafeInt32("Id"),
                    FullName = row.GetSafeString("FullName"),
                    EmployeeCode = row.GetSafeString("EmployeeCode"),
                    Email = row.GetSafeString("Email"),
                    Phone = row.GetSafeString("Phone")
                };
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"GetEmployeeById Exception.", ex);
            throw;
        }

        return result;
    }
} 