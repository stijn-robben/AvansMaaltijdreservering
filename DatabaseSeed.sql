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

-- Reset identity seeds to ensure consistent IDs
DBCC CHECKIDENT ('Packages', RESEED, 0);
DBCC CHECKIDENT ('Products', RESEED, 0);
DBCC CHECKIDENT ('Canteens', RESEED, 0); 

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
('Chocolate Cake', 0, 'https://example.com/cake.jpg'),
('Chicken Tikka Masala', 0, 'https://example.com/tikka.jpg'),
('Greek Salad', 0, 'https://example.com/greek.jpg'),
('Fish & Chips', 0, 'https://example.com/fish.jpg'),
('Margherita Pizza', 0, 'https://example.com/pizza.jpg'),
('Thai Green Curry', 0, 'https://example.com/curry.jpg'),
('Prosecco', 1, 'https://example.com/prosecco.jpg'),
('Energy Drink', 0, 'https://example.com/energy.jpg'),
('Mushroom Risotto', 0, 'https://example.com/risotto.jpg'),
('BBQ Ribs', 0, 'https://example.com/ribs.jpg'),
('Whiskey', 1, 'https://example.com/whiskey.jpg');

-- 3. INSERT PACKAGES
DECLARE @CanteenLA INT = (SELECT TOP 1 Id FROM Canteens WHERE Location = 0 AND City = 0);
DECLARE @CanteenLD INT = (SELECT TOP 1 Id FROM Canteens WHERE Location = 1 AND City = 0);
DECLARE @CanteenHA INT = (SELECT TOP 1 Id FROM Canteens WHERE Location = 2 AND City = 0);

-- Is18Plus is now calculated from products (removed from database)
-- Generate future-proof dates: tomorrow and day after tomorrow
DECLARE @Tomorrow DATETIME = DATEADD(day, 1, CAST(GETDATE() AS DATE));
DECLARE @DayAfter DATETIME = DATEADD(day, 2, CAST(GETDATE() AS DATE));

INSERT INTO Packages (Name, City, CanteenLocation, PickupTime, LatestPickupTime, Price, MealType, CanteenId) VALUES
('Healthy Lunch Special', 0, 0, DATEADD(hour, 12, DATEADD(minute, 30, @Tomorrow)), DATEADD(hour, 14, @Tomorrow), 5.95, 0, @CanteenLA),
('Deluxe Evening Package', 0, 0, DATEADD(hour, 18, @DayAfter), DATEADD(hour, 19, DATEADD(minute, 30, @DayAfter)), 15.95, 1, @CanteenLA),
('Hot Pasta Special', 0, 1, DATEADD(hour, 17, DATEADD(minute, 30, @DayAfter)), DATEADD(hour, 19, @DayAfter), 8.50, 1, @CanteenLD),
('Fresh Wrap & Juice', 0, 2, DATEADD(hour, 11, DATEADD(minute, 30, @Tomorrow)), DATEADD(hour, 13, @Tomorrow), 4.50, 0, @CanteenHA),
('Spicy Curry Night', 0, 0, DATEADD(hour, 19, @DayAfter), DATEADD(hour, 20, DATEADD(minute, 30, @DayAfter)), 12.95, 1, @CanteenLA),
('Fish Friday Special', 0, 1, DATEADD(hour, 12, @DayAfter), DATEADD(hour, 14, @DayAfter), 9.95, 0, @CanteenLD),
('Weekend Pizza Party', 0, 2, DATEADD(hour, 17, @DayAfter), DATEADD(hour, 19, @DayAfter), 11.50, 1, @CanteenHA),
('Mediterranean Mix', 0, 0, DATEADD(hour, 11, DATEADD(minute, 45, @Tomorrow)), DATEADD(hour, 13, DATEADD(minute, 15, @Tomorrow)), 7.95, 0, @CanteenLA),
('BBQ Bonanza', 0, 1, DATEADD(hour, 18, DATEADD(minute, 30, @Tomorrow)), DATEADD(hour, 20, @Tomorrow), 16.95, 1, @CanteenLD),
('Whiskey Tasting Dinner', 0, 2, DATEADD(hour, 19, DATEADD(minute, 30, @DayAfter)), DATEADD(hour, 21, @DayAfter), 24.95, 1, @CanteenHA);

-- Link products to packages (many-to-many relationship)
DECLARE @Package1 INT = (SELECT Id FROM Packages WHERE Name = 'Healthy Lunch Special');
DECLARE @Package2 INT = (SELECT Id FROM Packages WHERE Name = 'Deluxe Evening Package');
DECLARE @Package3 INT = (SELECT Id FROM Packages WHERE Name = 'Hot Pasta Special');
DECLARE @Package4 INT = (SELECT Id FROM Packages WHERE Name = 'Fresh Wrap & Juice');
DECLARE @Package5 INT = (SELECT Id FROM Packages WHERE Name = 'Spicy Curry Night');
DECLARE @Package6 INT = (SELECT Id FROM Packages WHERE Name = 'Fish Friday Special');
DECLARE @Package7 INT = (SELECT Id FROM Packages WHERE Name = 'Weekend Pizza Party');
DECLARE @Package8 INT = (SELECT Id FROM Packages WHERE Name = 'Mediterranean Mix');
DECLARE @Package9 INT = (SELECT Id FROM Packages WHERE Name = 'BBQ Bonanza');
DECLARE @Package10 INT = (SELECT Id FROM Packages WHERE Name = 'Whiskey Tasting Dinner');

-- Get actual Product IDs (not hardcoded)
DECLARE @TurkeySandwich INT = (SELECT Id FROM Products WHERE Name = 'Turkey Club Sandwich');
DECLARE @OrangeJuice INT = (SELECT Id FROM Products WHERE Name = 'Fresh Orange Juice');
DECLARE @Salmon INT = (SELECT Id FROM Products WHERE Name = 'Grilled Salmon Fillet');
DECLARE @RedWine INT = (SELECT Id FROM Products WHERE Name = 'Red Wine (House)');
DECLARE @Pasta INT = (SELECT Id FROM Products WHERE Name = 'Pasta Bolognese');
DECLARE @Coffee INT = (SELECT Id FROM Products WHERE Name = 'Hot Coffee');
DECLARE @Wrap INT = (SELECT Id FROM Products WHERE Name = 'Vegetarian Wrap');
DECLARE @Caesar INT = (SELECT Id FROM Products WHERE Name = 'Caesar Salad');
DECLARE @Tikka INT = (SELECT Id FROM Products WHERE Name = 'Chicken Tikka Masala');
DECLARE @Greek INT = (SELECT Id FROM Products WHERE Name = 'Greek Salad');
DECLARE @Fish INT = (SELECT Id FROM Products WHERE Name = 'Fish & Chips');
DECLARE @Pizza INT = (SELECT Id FROM Products WHERE Name = 'Margherita Pizza');
DECLARE @Curry INT = (SELECT Id FROM Products WHERE Name = 'Thai Green Curry');
DECLARE @Prosecco INT = (SELECT Id FROM Products WHERE Name = 'Prosecco');
DECLARE @Energy INT = (SELECT Id FROM Products WHERE Name = 'Energy Drink');
DECLARE @Risotto INT = (SELECT Id FROM Products WHERE Name = 'Mushroom Risotto');
DECLARE @Ribs INT = (SELECT Id FROM Products WHERE Name = 'BBQ Ribs');
DECLARE @Whiskey INT = (SELECT Id FROM Products WHERE Name = 'Whiskey');
DECLARE @Beer INT = (SELECT Id FROM Products WHERE Name = 'Craft Beer');
DECLARE @Cake INT = (SELECT Id FROM Products WHERE Name = 'Chocolate Cake');

INSERT INTO PackageProducts (PackagesId, ProductsId) VALUES
-- Package 1: Healthy Lunch (No alcohol)
(@Package1, @TurkeySandwich), (@Package1, @OrangeJuice), (@Package1, @Caesar),
-- Package 2: Deluxe Evening (18+ - Contains Red Wine)
(@Package2, @Salmon), (@Package2, @RedWine), (@Package2, @Cake),
-- Package 3: Hot Pasta (No alcohol) 
(@Package3, @Pasta), (@Package3, @Coffee),
-- Package 4: Fresh Wrap (No alcohol)
(@Package4, @Wrap), (@Package4, @OrangeJuice), (@Package4, @Greek),
-- Package 5: Spicy Curry (No alcohol)
(@Package5, @Tikka), (@Package5, @Curry), (@Package5, @Energy),
-- Package 6: Fish Friday (No alcohol)
(@Package6, @Fish), (@Package6, @Coffee),
-- Package 7: Pizza Party (18+ - Contains Prosecco)
(@Package7, @Pizza), (@Package7, @Prosecco),
-- Package 8: Mediterranean (No alcohol)
(@Package8, @Greek), (@Package8, @OrangeJuice), (@Package8, @Risotto),
-- Package 9: BBQ Bonanza (18+ - Contains Beer)
(@Package9, @Ribs), (@Package9, @Beer), (@Package9, @Cake),
-- Package 10: Whiskey Tasting (18+ - Contains Whiskey)
(@Package10, @Salmon), (@Package10, @Whiskey), (@Package10, @Cake);

PRINT 'Package-Product relationships created';

-- VERIFY PACKAGE IDs FOR TESTING
PRINT 'Package ID verification:';
SELECT 'Package ' + CAST(Id AS VARCHAR) + ': ' + Name + ' on ' + CONVERT(VARCHAR, PickupTime, 120) AS PackageInfo
FROM Packages 
WHERE Id IN (1, 2, 3, 4)
ORDER BY Id;

-- 4. CANTEEN EMPLOYEES
-- CanteenEmployees are created through API when registering employee users
-- This maintains proper linking between Identity users and employee records

-- 5. VERIFICATION
DECLARE @CanteenCount INT = (SELECT COUNT(*) FROM Canteens);
DECLARE @ProductCount INT = (SELECT COUNT(*) FROM Products);
DECLARE @PackageCount INT = (SELECT COUNT(*) FROM Packages);

PRINT '=== DATABASE SEEDED SUCCESSFULLY ===';
PRINT 'Canteens: ' + CAST(@CanteenCount AS VARCHAR) + ' (Expected: 5)';
PRINT 'Products: ' + CAST(@ProductCount AS VARCHAR) + ' (Expected: 20)';
PRINT 'Packages: ' + CAST(@PackageCount AS VARCHAR) + ' (Expected: 10)';

