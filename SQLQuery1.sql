-- Tạo User mẫu
IF NOT EXISTS (
    SELECT 1
    FROM [User]
    WHERE Username = 'khach1'
)
BEGIN
    INSERT INTO [User] (Username, Password, UserRole)
    VALUES ('khach1', '123456', 'C');
END


-- Tạo Category mẫu
IF NOT EXISTS (
    SELECT 1
    FROM Category
    WHERE CategoryName = N'Điện thoại'
)
BEGIN
    INSERT INTO Category (CategoryName)
    VALUES (N'Điện thoại');
END


-- Tạo Customer mẫu
IF NOT EXISTS (
    SELECT 1
    FROM Customer
    WHERE Username = 'khach1'
)
BEGIN
    INSERT INTO Customer
    (
        CustomerName,
        CustomerPhone,
        CustomerEmail,
        CustomerAddress,
        Username
    )
    VALUES
    (
        N'Nguyễn Văn An',
        '0901234567',
        'an@gmail.com',
        N'Quận 10, TP.HCM',
        'khach1'
    );
END


-- Tạo Product mẫu
IF NOT EXISTS (
    SELECT 1
    FROM Product
    WHERE ProductName = N'iPhone 15'
)
BEGIN
    INSERT INTO Product
    (
        CategoryID,
        ProductName,
        ProductDecription,
        ProductPrice,
        ProductImage
    )
    SELECT
        CategoryID,
        N'iPhone 15',
        N'Điện thoại mẫu để kiểm tra hệ thống',
        20000000,
        NULL
    FROM Category
    WHERE CategoryName = N'Điện thoại';
END


-- Tạo Order mẫu
IF NOT EXISTS (
    SELECT 1
    FROM [Order] o
    INNER JOIN Customer c ON o.CustomerID = c.CustomerID
    WHERE c.Username = 'khach1'
)
BEGIN
    INSERT INTO [Order]
    (
        CustomerID,
        OrderDate,
        TotalAmount,
        PaymentStatus,
        AddressDelivery
    )
    SELECT
        CustomerID,
        CAST(GETDATE() AS date),
        20000000,
        N'Đã thanh toán',
        N'Quận 10, TP.HCM'
    FROM Customer
    WHERE Username = 'khach1';
END


-- Tạo OrderDetail mẫu
IF NOT EXISTS (
    SELECT 1
    FROM OrderDetail od
    INNER JOIN [Order] o ON od.OrderID = o.OrderID
    INNER JOIN Customer c ON o.CustomerID = c.CustomerID
    WHERE c.Username = 'khach1'
)
BEGIN
    INSERT INTO OrderDetail
    (
        ProductID,
        OrderID,
        Quantity,
        UnitPrice
    )
    SELECT
        p.ProductID,
        o.OrderID,
        1,
        20000000
    FROM Product p
    CROSS JOIN [Order] o
    INNER JOIN Customer c ON o.CustomerID = c.CustomerID
    WHERE p.ProductName = N'iPhone 15'
      AND c.Username = 'khach1';
END


-- Kiểm tra kết quả
SELECT * FROM [User];
SELECT * FROM Category;
SELECT * FROM Customer;
SELECT * FROM Product;
SELECT * FROM [Order];
SELECT * FROM OrderDetail;