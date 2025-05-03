using Infrastructure.DbContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shared.Helpers;

namespace Company.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Entities.Company>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Entities.Company?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Entities.Company> CreateAsync(Entities.Company company)
    {
        throw new NotImplementedException();
    }

    public async Task<Entities.Company> UpdateAsync(Entities.Company company)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<int> CreateCompanyAsync(string fullName, string address)
    {
        var connection = _context.Database.GetDbConnection();
        var companyId = 0;

        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Company_Create";
            command.CommandType = System.Data.CommandType.StoredProcedure;

            // Add parameters
            command.Parameters.Add(new SqlParameter("@FullName", System.Data.SqlDbType.NVarChar, 100) { Value = fullName });
            command.Parameters.Add(new SqlParameter("@Addess", System.Data.SqlDbType.NVarChar, 250) { Value = address });
            command.Parameters.Add(new SqlParameter("@CompanyId", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output });

            // Execute
            await command.ExecuteNonQueryAsync();
            companyId = Convert.ToInt32(command.Parameters["@CompanyId"].Value);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("CreateCompany Exception.", ex);
            throw;
        }
        finally
        {
            if (connection.State == System.Data.ConnectionState.Open)
                await connection.CloseAsync();
        }

        return companyId;
    }
} 