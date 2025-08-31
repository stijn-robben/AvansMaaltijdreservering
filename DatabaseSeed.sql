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
('Apple Slices', 0, 'https://i.ibb.co/hFWT0Tpn/apple-slices.png'),
('Bagel', 0, 'https://i.ibb.co/Fb99ym5q/bagel.png'),
('Bread Carrero', 0, 'https://i.ibb.co/nNFVgJNV/bread-carrero.png'),
('Caesar Salad', 0, 'https://i.ibb.co/WQtjcTg/ceasar-salad.png'),
('Chicken Soup', 0, 'https://i.ibb.co/Pvh6CqQ3/chicken-soup.png'),
('Chips Naturel', 0, 'https://i.ibb.co/DH79MCBZ/chips-naturel.png'),
('Chocolate Cake', 0, 'https://i.ibb.co/cWDSYQt/chocolate-cake.png'),
('Club Sandwich', 0, 'https://i.ibb.co/dsf9pN27/club-sandwich.png'),
('Coffee Black', 0, 'https://i.ibb.co/HT7WrrjY/coffee-black.png'),
('Croissant with Jam', 0, 'https://i.ibb.co/dwZzkk5D/croissant-jam.png'),
('Grilled Salmon', 0, 'https://i.ibb.co/PzNrvWGB/grilled-salmon.png'),
('Orange Juice', 0, 'https://i.ibb.co/cS95Ldt3/orange-juice.png'),
('Pancakes', 0, 'https://i.ibb.co/4RQQn2vt/pancakes.png'),
('Pasta Bolognese', 0, 'https://i.ibb.co/vNpRW2s/pasta-bolognese.png'),
('Pretzel', 0, 'https://i.ibb.co/QFMdyPHW/pretzel.png'),
('Scrambled Eggs', 0, 'https://i.ibb.co/YBpLFDND/scrambled-eggs.png'),
('Blueberry Smoothie', 0, 'https://i.ibb.co/0jPw0mgc/smoothie-blueberry.png'),
('Steak with Mashed Potato', 0, 'https://i.ibb.co/yn2M7SJD/steak-mashedpotato.png'),
('Thai Green Curry', 0, 'https://i.ibb.co/v6pT3zy6/thai-green-curry.png'),
('Toast with Jam', 0, 'https://i.ibb.co/VcLR9tpW/toast-jam.png'),
('Tomato Soup', 0, 'https://i.ibb.co/ks7WqYRv/tomato-soup.png');

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
('Breakfast Special', 0, 2, DATEADD(hour, 8, @Tomorrow), DATEADD(hour, 10, @Tomorrow), 6.95, 5, @CanteenHA);

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
DECLARE @Package10 INT = (SELECT Id FROM Packages WHERE Name = 'Breakfast Special');

-- Get actual Product IDs (not hardcoded)
DECLARE @AppleSlices INT = (SELECT Id FROM Products WHERE Name = 'Apple Slices');
DECLARE @Bagel INT = (SELECT Id FROM Products WHERE Name = 'Bagel');
DECLARE @BreadCarrero INT = (SELECT Id FROM Products WHERE Name = 'Bread Carrero');
DECLARE @CaesarSalad INT = (SELECT Id FROM Products WHERE Name = 'Caesar Salad');
DECLARE @ChickenSoup INT = (SELECT Id FROM Products WHERE Name = 'Chicken Soup');
DECLARE @ChipsNaturel INT = (SELECT Id FROM Products WHERE Name = 'Chips Naturel');
DECLARE @ChocolateCake INT = (SELECT Id FROM Products WHERE Name = 'Chocolate Cake');
DECLARE @ClubSandwich INT = (SELECT Id FROM Products WHERE Name = 'Club Sandwich');
DECLARE @CoffeeBlack INT = (SELECT Id FROM Products WHERE Name = 'Coffee Black');
DECLARE @CroissantJam INT = (SELECT Id FROM Products WHERE Name = 'Croissant with Jam');
DECLARE @GrilledSalmon INT = (SELECT Id FROM Products WHERE Name = 'Grilled Salmon');
DECLARE @OrangeJuice INT = (SELECT Id FROM Products WHERE Name = 'Orange Juice');
DECLARE @Pancakes INT = (SELECT Id FROM Products WHERE Name = 'Pancakes');
DECLARE @PastaBolognese INT = (SELECT Id FROM Products WHERE Name = 'Pasta Bolognese');
DECLARE @Pretzel INT = (SELECT Id FROM Products WHERE Name = 'Pretzel');
DECLARE @ScrambledEggs INT = (SELECT Id FROM Products WHERE Name = 'Scrambled Eggs');
DECLARE @BlueberrySmoothie INT = (SELECT Id FROM Products WHERE Name = 'Blueberry Smoothie');
DECLARE @SteakMashedPotato INT = (SELECT Id FROM Products WHERE Name = 'Steak with Mashed Potato');
DECLARE @ThaiGreenCurry INT = (SELECT Id FROM Products WHERE Name = 'Thai Green Curry');
DECLARE @ToastJam INT = (SELECT Id FROM Products WHERE Name = 'Toast with Jam');
DECLARE @TomatoSoup INT = (SELECT Id FROM Products WHERE Name = 'Tomato Soup');

INSERT INTO PackageProducts (PackagesId, ProductsId) VALUES
-- Package 1: Healthy Lunch (No alcohol)
(@Package1, @ClubSandwich), (@Package1, @OrangeJuice), (@Package1, @CaesarSalad),
-- Package 2: Deluxe Evening (No alcohol)
(@Package2, @GrilledSalmon), (@Package2, @SteakMashedPotato), (@Package2, @ChocolateCake),
-- Package 3: Hot Pasta (No alcohol) 
(@Package3, @PastaBolognese), (@Package3, @CoffeeBlack),
-- Package 4: Fresh Wrap (No alcohol)
(@Package4, @ToastJam), (@Package4, @OrangeJuice), (@Package4, @AppleSlices),
-- Package 5: Spicy Curry (No alcohol)
(@Package5, @ThaiGreenCurry), (@Package5, @ChickenSoup), (@Package5, @BlueberrySmoothie),
-- Package 6: Fish Friday (No alcohol)
(@Package6, @GrilledSalmon), (@Package6, @CoffeeBlack),
-- Package 7: Pizza Party (No alcohol)
(@Package7, @Pretzel), (@Package7, @ChipsNaturel),
-- Package 8: Mediterranean (No alcohol)
(@Package8, @CaesarSalad), (@Package8, @OrangeJuice), (@Package8, @TomatoSoup),
-- Package 9: BBQ Bonanza (No alcohol)
(@Package9, @SteakMashedPotato), (@Package9, @ChocolateCake), (@Package9, @ChipsNaturel),
-- Package 10: Breakfast Special (No alcohol)
(@Package10, @Pancakes), (@Package10, @ScrambledEggs), (@Package10, @CroissantJam), (@Package10, @Bagel);

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
PRINT 'Products: ' + CAST(@ProductCount AS VARCHAR) + ' (Expected: 21)';
PRINT 'Packages: ' + CAST(@PackageCount AS VARCHAR) + ' (Expected: 10)';

