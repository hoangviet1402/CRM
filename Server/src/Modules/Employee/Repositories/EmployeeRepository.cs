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

    public async Task<int> EmployeeRegister(string fullname,string employeesCode, string phone, string email, string password, int companyId, int role)
    {
        var connection = _context.Database.GetDbConnection();
        var response = 0;

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Employee_Register";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@FullName", SqlDbType.NVarChar, 100) { Value = fullname });
            command.Parameters.Add(new SqlParameter("@EmployeesCode", SqlDbType.NVarChar, 20) { Value = employeesCode });
            command.Parameters.Add(new SqlParameter("@Phone", SqlDbType.NVarChar, 20) { Value = phone });
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 100) { Value = email });
            command.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 256) { Value = AESHelper.HashPassword(password) });
            command.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.Int) { Value = companyId });
            command.Parameters.Add(new SqlParameter("@Role", SqlDbType.Int) { Value = role });
            command.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Direction = ParameterDirection.Output });

            await command.ExecuteNonQueryAsync();
            response = Convert.ToInt32(command.Parameters["@EmployeesId"].Value);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }

        return response;
    }


    public async Task<EmployeeEntity?> GetEmployeeById(int id, int companyId)
    {
        var connection = _context.Database.GetDbConnection();
        
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Employee_GetById";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = id });
            command.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.Int) { Value = companyId });

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