using Infrastructure.DbContext;
using Infrastructure.StoredProcedureMapperModule;

namespace Education.Repositories;

public class EducationRepository : IEducationRepository
{
    private readonly DatabaseConnection _dbConnection;
    private readonly StoredProcedureMapperModule _storedProcedureMapper;

    public EducationRepository(DatabaseConnection dbConnection)
    {
        _dbConnection = dbConnection;
        _storedProcedureMapper = new StoredProcedureMapperModule(dbConnection);
    }
} 