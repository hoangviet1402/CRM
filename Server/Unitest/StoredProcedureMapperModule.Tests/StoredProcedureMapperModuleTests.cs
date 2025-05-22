using Xunit;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Moq;
using Infrastructure.StoredProcedureMapperModule;
using System.Threading.Tasks;

namespace StoredProcedureMapperModule.Tests
{
    public class StoredProcedureMapperModuleTests
    {
        private readonly Mock<IDbConnection> _mockConnection;
        private readonly Mock<IDbCommand> _mockCommand;
        private readonly Mock<IDataReader> _mockReader;
        private readonly StoredProcedureMapperModule _mapper;

        public StoredProcedureMapperModuleTests()
        {
            _mockConnection = new Mock<IDbConnection>();
            _mockCommand = new Mock<IDbCommand>();
            _mockReader = new Mock<IDataReader>();
            _mapper = new StoredProcedureMapperModule();

            // Setup mock command
            _mockCommand.Setup(c => c.CreateParameter()).Returns(new SqlParameter());
            _mockConnection.Setup(c => c.CreateCommand()).Returns(_mockCommand.Object);
        }

        [Fact]
        public async Task ExecuteStoredProcedureAsync_WithValidParameters_ShouldExecuteSuccessfully()
        {
            // Arrange
            var parameters = new Dictionary<string, object>
            {
                { "@param1", "value1" },
                { "@param2", 123 }
            };
            var outputParameters = new Dictionary<string, object>();

            _mockCommand.Setup(c => c.ExecuteNonQueryAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _mapper.ExecuteStoredProcedureAsync(_mockConnection.Object, "TestProcedure", parameters, outputParameters);

            // Assert
            Assert.True(result);
            _mockCommand.Verify(c => c.ExecuteNonQueryAsync(default), Times.Once);
        }

        [Fact]
        public async Task ExecuteStoredProcedureAsync_WithNullParameters_ShouldExecuteSuccessfully()
        {
            // Arrange
            var outputParameters = new Dictionary<string, object>();
            _mockCommand.Setup(c => c.ExecuteNonQueryAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _mapper.ExecuteStoredProcedureAsync(_mockConnection.Object, "TestProcedure", null, outputParameters);

            // Assert
            Assert.True(result);
            _mockCommand.Verify(c => c.ExecuteNonQueryAsync(default), Times.Once);
        }

        [Fact]
        public async Task ExecuteStoredProcedureAsync_WithException_ShouldReturnFalse()
        {
            // Arrange
            var outputParameters = new Dictionary<string, object>();
            _mockCommand.Setup(c => c.ExecuteNonQueryAsync(default)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _mapper.ExecuteStoredProcedureAsync(_mockConnection.Object, "TestProcedure", null, outputParameters);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ExecuteStoredProcedure_WithValidParameters_ShouldExecuteSuccessfully()
        {
            // Arrange
            var parameters = new Dictionary<string, object>
            {
                { "@param1", "value1" },
                { "@param2", 123 }
            };

            _mockCommand.Setup(c => c.ExecuteNonQuery()).Returns(1);

            // Act
            var result = _mapper.ExecuteStoredProcedure(_mockConnection.Object, "TestProcedure", parameters);

            // Assert
            Assert.True(result);
            _mockCommand.Verify(c => c.ExecuteNonQuery(), Times.Once);
        }

        [Fact]
        public void ExecuteStoredProcedure_WithNullParameters_ShouldExecuteSuccessfully()
        {
            // Arrange
            _mockCommand.Setup(c => c.ExecuteNonQuery()).Returns(1);

            // Act
            var result = _mapper.ExecuteStoredProcedure(_mockConnection.Object, "TestProcedure", null);

            // Assert
            Assert.True(result);
            _mockCommand.Verify(c => c.ExecuteNonQuery(), Times.Once);
        }

        [Fact]
        public void ExecuteStoredProcedure_WithException_ShouldReturnFalse()
        {
            // Arrange
            _mockCommand.Setup(c => c.ExecuteNonQuery()).Throws(new Exception("Test exception"));

            // Act
            var result = _mapper.ExecuteStoredProcedure(_mockConnection.Object, "TestProcedure", null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ExecuteStoredProcedureWithResult_WithValidParameters_ShouldReturnDataTable()
        {
            // Arrange
            var parameters = new Dictionary<string, object>
            {
                { "@param1", "value1" }
            };

            _mockCommand.Setup(c => c.ExecuteReader()).Returns(_mockReader.Object);
            _mockReader.SetupSequence(r => r.Read()).Returns(true).Returns(false);
            _mockReader.Setup(r => r.FieldCount).Returns(2);
            _mockReader.Setup(r => r.GetName(0)).Returns("Column1");
            _mockReader.Setup(r => r.GetName(1)).Returns("Column2");
            _mockReader.Setup(r => r.GetValue(0)).Returns("Value1");
            _mockReader.Setup(r => r.GetValue(1)).Returns("Value2");

            // Act
            var result = _mapper.ExecuteStoredProcedureWithResult(_mockConnection.Object, "TestProcedure", parameters);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Rows.Count);
            Assert.Equal(2, result.Columns.Count);
        }

        [Fact]
        public void ExecuteStoredProcedureWithResult_WithException_ShouldReturnNull()
        {
            // Arrange
            _mockCommand.Setup(c => c.ExecuteReader()).Throws(new Exception("Test exception"));

            // Act
            var result = _mapper.ExecuteStoredProcedureWithResult(_mockConnection.Object, "TestProcedure", null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ExecuteStoredProcedureWithResultAsync_WithValidParameters_ShouldReturnDataTable()
        {
            // Arrange
            var parameters = new Dictionary<string, object>
            {
                { "@param1", "value1" }
            };

            var mockSqlCommand = new Mock<SqlCommand>();
            var mockSqlDataReader = new Mock<SqlDataReader>();
            mockSqlCommand.Setup(c => c.ExecuteReaderAsync(CommandBehavior.Default)).ReturnsAsync(mockSqlDataReader.Object);
            _mockConnection.Setup(c => c.CreateCommand()).Returns(mockSqlCommand.Object);

            mockSqlDataReader.SetupSequence(r => r.Read()).Returns(true).Returns(false);
            mockSqlDataReader.Setup(r => r.FieldCount).Returns(2);
            mockSqlDataReader.Setup(r => r.GetName(0)).Returns("Column1");
            mockSqlDataReader.Setup(r => r.GetName(1)).Returns("Column2");
            mockSqlDataReader.Setup(r => r.GetValue(0)).Returns("Value1");
            mockSqlDataReader.Setup(r => r.GetValue(1)).Returns("Value2");

            // Act
            var result = await _mapper.ExecuteStoredProcedureWithResultAsync(_mockConnection.Object, "TestProcedure", parameters);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Rows.Count);
            Assert.Equal(2, result.Columns.Count);
        }

        [Fact]
        public async Task ExecuteStoredProcedureWithResultAsync_WithException_ShouldReturnNull()
        {
            // Arrange
            var mockSqlCommand = new Mock<SqlCommand>();
            mockSqlCommand.Setup(c => c.ExecuteReaderAsync(CommandBehavior.Default)).ThrowsAsync(new Exception("Test exception"));
            _mockConnection.Setup(c => c.CreateCommand()).Returns(mockSqlCommand.Object);

            // Act
            var result = await _mapper.ExecuteStoredProcedureWithResultAsync(_mockConnection.Object, "TestProcedure", null);

            // Assert
            Assert.Null(result);
        }
    }
} 