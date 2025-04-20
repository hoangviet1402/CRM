using EmployeeModule.DTOs;
using EmployeeModule.Entities;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using Shared.Result;
using Shared.Helpers;
using Shared.Enums;

namespace EmployeeModule.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResult<CreateEmployeeResponse>> CreateEmployeeSimple(string fullName, string employeeCode, string email, string phone)
    {
        var connection = _context.Database.GetDbConnection();
        var response = new ApiResult<CreateEmployeeResponse>() { 
            Code = ResponseCodeEnum.SystemMaintenance.Value(),
            Data = new CreateEmployeeResponse()
        };    
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "sp_InsertEmployee";
            command.CommandType = CommandType.StoredProcedure;

            // Thêm input parameters
            command.Parameters.Add(new SqlParameter("@FullName", SqlDbType.NVarChar, 100) { Value = fullName });
            command.Parameters.Add(new SqlParameter("@EmployeeCode", SqlDbType.VarChar, 50) { Value = employeeCode });
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = email });
            command.Parameters.Add(new SqlParameter("@Phone", SqlDbType.NVarChar, 20) { Value = phone });

            // Thêm output parameter để nhận Id
            var outputIdParam = new SqlParameter
            {
                ParameterName = "@EmployeeId",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(outputIdParam);
           
            // Thực thi stored procedure
            await command.ExecuteNonQueryAsync();

            // Lấy Id được trả về
            response.Data = new CreateEmployeeResponse()
            {
                EmployeeId = (int)outputIdParam.Value
            };
            response.Code = ResponseCodeEnum.Success.Value();
            response.Message = "Tạo nhân viên thành công";
            return response;
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"CreateEmployeeSimple Exception.", ex);
            response.Data = new CreateEmployeeResponse()
            {
                EmployeeId = 0
            };
            response.Code = ResponseCodeEnum.DatabaseError.Value();
            response.Message = "Tạo nhân viên thất bại";            
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }

        return response;
    }

    public async Task<EmployeeEntity?> GetEmployeeById(int id)
    {
        var connection = _context.Database.GetDbConnection();
        
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "sp_GetEmployeeById";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = id });

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new EmployeeEntity
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FullName = reader.GetString(reader.GetOrdinal("FullName")),
                    EmployeeCode = reader.GetString(reader.GetOrdinal("EmployeeCode")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Phone = reader.GetString(reader.GetOrdinal("Phone"))
                };
            }
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }

        return null;
    }
} 