-- Tạo stored procedure GetEmployees để thử nghiệm ứng dụng
USE tanca;
GO

-- Kiểm tra xem stored procedure đã tồn tại chưa
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetEmployees')
    DROP PROCEDURE GetEmployees;
GO

-- Tạo stored procedure
CREATE PROCEDURE GetEmployees
AS
BEGIN
    -- Tạo bảng tạm để giả lập kết quả
    SELECT 
        1 AS EmployeeId,
        N'Nguyễn' AS FirstName,
        N'Văn A' AS LastName,
        'employee1@example.com' AS Email,
        GETDATE() AS HireDate,
        5000000.00 AS Salary,
        1 AS DepartmentId,
        N'Phòng IT' AS DepartmentName,
        N'Lập trình viên' AS Position,
        1 AS IsActive
    UNION ALL
    SELECT 
        2 AS EmployeeId,
        N'Trần' AS FirstName,
        N'Thị B' AS LastName,
        'employee2@example.com' AS Email,
        GETDATE() - 30 AS HireDate,
        6000000.00 AS Salary,
        2 AS DepartmentId,
        N'Phòng Nhân sự' AS DepartmentName,
        N'Quản lý nhân sự' AS Position,
        1 AS IsActive;
END;
GO

-- Tạo stored procedure GetEmployeeById
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetEmployeeById')
    DROP PROCEDURE GetEmployeeById;
GO

-- Stored procedure với tham số
CREATE PROCEDURE GetEmployeeById
    @EmployeeId INT
AS
BEGIN
    -- Tạo bảng tạm để giả lập kết quả
    IF @EmployeeId = 1
    BEGIN
        SELECT 
            1 AS EmployeeId,
            N'Nguyễn' AS FirstName,
            N'Văn A' AS LastName,
            'employee1@example.com' AS Email,
            GETDATE() AS HireDate,
            5000000.00 AS Salary,
            1 AS DepartmentId,
            N'Phòng IT' AS DepartmentName,
            N'Lập trình viên' AS Position,
            1 AS IsActive;
    END
    ELSE
    BEGIN
        SELECT 
            2 AS EmployeeId,
            N'Trần' AS FirstName,
            N'Thị B' AS LastName,
            'employee2@example.com' AS Email,
            GETDATE() - 30 AS HireDate,
            6000000.00 AS Salary,
            2 AS DepartmentId,
            N'Phòng Nhân sự' AS DepartmentName,
            N'Quản lý nhân sự' AS Position,
            1 AS IsActive;
    END
END;
GO

PRINT N'Các stored procedure đã được tạo thành công!';
GO 