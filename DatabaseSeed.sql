-- ========================================
-- AVANS MAALTIJDRESERVERING DATABASE SEED
-- ========================================
-- Run this script against the main database (AvansMaaltijdreservering)
-- Users are created through API endpoints only!

PRINT 'Starting database seeding...';

-- CLEANUP (respect foreign keys)
DELETE FROM Packages; 
DELETE FROM Products; 
DELETE FROM Canteens; 

PRINT 'Existing data cleared';

-- 1. INSERT CANTEENS
-- Location enum: 0=BREDA_LA, 1=BREDA_LD, 2=BREDA_HA, 3=DENBOSCH, 4=TILBURG  
-- City enum: 0=BREDA, 1=TILBURG, 2=DENBOSCH
INSERT INTO Canteens (Location, City, ServesWarmMeals) VALUES
(0, 0, 1), -- Breda LA Building, serves warm meals
(1, 0, 0), -- Breda LD Building, no warm meals  
(2, 0, 1), -- Breda HA Building, serves warm meals
(3, 2, 1), -- Den Bosch Building, serves warm meals
(4, 1, 0); -- Tilburg Building, no warm meals

-- 2. INSERT PRODUCTS
INSERT INTO Products (Name, ContainsAlcohol, PhotoUrl) VALUES
('Turkey Club Sandwich', 0, 'https://example.com/turkey.jpg'),
('Fresh Orange Juice', 0, 'https://example.com/juice.jpg'),
('Grilled Salmon Fillet', 0, 'https://example.com/salmon.jpg'),
('Red Wine (House)', 1, 'https://example.com/wine.jpg'),
('Pasta Bolognese', 0, 'https://example.com/pasta.jpg'),
('Caesar Salad', 0, 'https://example.com/salad.jpg'),
('Craft Beer', 1, 'https://example.com/beer.jpg'),
('Vegetarian Wrap', 0, 'https://example.com/wrap.jpg'),
('Hot Coffee', 0, 'https://example.com/coffee.jpg'),
('Chocolate Cake', 0, 'https://example.com/cake.jpg');

-- 3. INSERT PACKAGES
DECLARE @CanteenLA INT = (SELECT TOP 1 Id FROM Canteens WHERE Location = 0 AND City = 0);
DECLARE @CanteenLD INT = (SELECT TOP 1 Id FROM Canteens WHERE Location = 1 AND City = 0);
DECLARE @CanteenHA INT = (SELECT TOP 1 Id FROM Canteens WHERE Location = 2 AND City = 0);

-- Is18Plus is now calculated from products (removed from database)
INSERT INTO Packages (Name, City, CanteenLocation, PickupTime, LatestPickupTime, Price, MealType, CanteenId) VALUES
('Healthy Lunch Special', 0, 0, '2025-08-28T12:30:00', '2025-08-28T14:00:00', 5.95, 0, @CanteenLA),
('Deluxe Evening Package', 0, 0, '2025-08-29T18:00:00', '2025-08-29T19:30:00', 15.95, 1, @CanteenLA),
('Hot Pasta Special', 0, 1, '2025-08-29T17:30:00', '2025-08-29T19:00:00', 8.50, 1, @CanteenLD),
('Fresh Wrap & Juice', 0, 2, '2025-08-28T11:30:00', '2025-08-28T13:00:00', 4.50, 0, @CanteenHA);

-- Link products to packages (many-to-many relationship)
DECLARE @Package1 INT = (SELECT Id FROM Packages WHERE Name = 'Healthy Lunch Special');
DECLARE @Package2 INT = (SELECT Id FROM Packages WHERE Name = 'Deluxe Evening Package');
DECLARE @Package3 INT = (SELECT Id FROM Packages WHERE Name = 'Hot Pasta Special');
DECLARE @Package4 INT = (SELECT Id FROM Packages WHERE Name = 'Fresh Wrap & Juice');

-- Get actual Product IDs (not hardcoded)
DECLARE @TurkeySandwich INT = (SELECT Id FROM Products WHERE Name = 'Turkey Club Sandwich');
DECLARE @OrangeJuice INT = (SELECT Id FROM Products WHERE Name = 'Fresh Orange Juice');
DECLARE @Salmon INT = (SELECT Id FROM Products WHERE Name = 'Grilled Salmon Fillet');
DECLARE @RedWine INT = (SELECT Id FROM Products WHERE Name = 'Red Wine (House)');
DECLARE @Pasta INT = (SELECT Id FROM Products WHERE Name = 'Pasta Bolognese');
DECLARE @Coffee INT = (SELECT Id FROM Products WHERE Name = 'Hot Coffee');
DECLARE @Wrap INT = (SELECT Id FROM Products WHERE Name = 'Vegetarian Wrap');

INSERT INTO PackageProducts (PackagesId, ProductsId) VALUES
(@Package1, @TurkeySandwich), (@Package1, @OrangeJuice), -- Healthy lunch
(@Package2, @Salmon), (@Package2, @RedWine), -- Deluxe evening (18+)
(@Package3, @Pasta), (@Package3, @Coffee), -- Hot pasta
(@Package4, @Wrap), (@Package4, @OrangeJuice); -- Fresh wrap

PRINT 'Package-Product relationships created';

-- 4. CANTEEN EMPLOYEES
-- CanteenEmployees are created through API when registering employee users
-- This maintains proper linking between Identity users and employee records

-- 5. VERIFICATION
DECLARE @CanteenCount INT = (SELECT COUNT(*) FROM Canteens);
DECLARE @ProductCount INT = (SELECT COUNT(*) FROM Products);
DECLARE @PackageCount INT = (SELECT COUNT(*) FROM Packages);

PRINT '=== DATABASE SEEDED SUCCESSFULLY ===';
PRINT 'Canteens: ' + CAST(@CanteenCount AS VARCHAR) + ' (Expected: 5)';
PRINT 'Products: ' + CAST(@ProductCount AS VARCHAR) + ' (Expected: 10)';
PRINT 'Packages: ' + CAST(@PackageCount AS VARCHAR) + ' (Expected: 4)';

