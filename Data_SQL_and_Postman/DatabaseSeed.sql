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

-- 2. INSERT ALL 32 PRODUCTS FROM IMAGE URLS
INSERT INTO Products (Name, ContainsAlcohol, PhotoUrl) VALUES
-- Breakfast & Bakery Items
('Apple Pie', 0, 'https://i.ibb.co/ccB7yXmF/apple-pie.png'),
('Apple Slices', 0, 'https://i.ibb.co/hFWT0Tpn/apple-slices.png'),
('Bagel', 0, 'https://i.ibb.co/Fb99ym5q/bagel.png'),
('Baguette', 0, 'https://i.ibb.co/84MVYM7S/baguette.png'),
('Berliner Bol', 0, 'https://i.ibb.co/TM8BFFpC/berliner-bol.png'),
('Bread Carreiro', 0, 'https://i.ibb.co/nNFVgJNV/bread-carrero.png'),
('Chocolate Cake', 0, 'https://i.ibb.co/cWDSYQt/chocolate-cake.png'),
('Chocolate Cookie', 0, 'https://i.ibb.co/0ycSC34y/chocolate-cookie.png'),
('Croissant with Jam', 0, 'https://i.ibb.co/dwZzkk5D/croissant-jam.png'),
('Pancakes', 0, 'https://i.ibb.co/4RQQn2vt/pancakes.png'),
('Pretzel', 0, 'https://i.ibb.co/QFMdyPHW/pretzel.png'),
('Scrambled Eggs', 0, 'https://i.ibb.co/YBpLFDND/scrambled-eggs.png'),
('Toast with Jam', 0, 'https://i.ibb.co/VcLR9tpW/toast-jam.png'),
('White Bread', 0, 'https://i.ibb.co/Cs6Btdxz/white-bread.png'),
('Yogurt with Muesli', 0, 'https://i.ibb.co/GvcSWNSt/yoghurt-with-cruesli.png'),

-- Main Meals & Lunch Items
('Caesar Salad', 0, 'https://i.ibb.co/WQtjcTg/ceasar-salad.png'),
('Chicken Soup', 0, 'https://i.ibb.co/Pvh6CqQ3/chicken-soup.png'),
('Club Sandwich', 0, 'https://i.ibb.co/dsf9pN27/club-sandwich.png'),
('Grilled Salmon', 0, 'https://i.ibb.co/PzNrvWGB/grilled-salmon.png'),
('Pasta Bolognese', 0, 'https://i.ibb.co/vNpRW2s/pasta-bolognese.png'),
('Steak with Mashed Potato', 0, 'https://i.ibb.co/yn2M7SJD/steak-mashedpotato.png'),
('Thai Green Curry', 0, 'https://i.ibb.co/v6pT3zy6/thai-green-curry.png'),
('Tomato Soup', 0, 'https://i.ibb.co/ks7WqYRv/tomato-soup.png'),

-- Snacks
('Chips Natural', 0, 'https://i.ibb.co/DH79MCBZ/chips-naturel.png'),

-- Beverages (Non-Alcoholic)
('Black Coffee', 0, 'https://i.ibb.co/HT7WrrjY/coffee-black.png'),
('Blueberry Smoothie', 0, 'https://i.ibb.co/0jPw0mgc/smoothie-blueberry.png'),
('Cola', 0, 'https://i.ibb.co/Vc72Wxqb/cola.png'),
('Fanta', 0, 'https://i.ibb.co/ZCP2Ywx/fanta.png'),
('Orange Juice', 0, 'https://i.ibb.co/cS95Ldt3/orange-juice.png'),
('Tea', 0, 'https://i.ibb.co/twHsf6L2/tea.png'),

-- Alcoholic Beverages (18+)
('Beer', 1, 'https://i.ibb.co/Y4wLQBXy/beer.png'),
('Red Wine', 1, 'https://i.ibb.co/QFLt1QkY/red-wine.png');

-- 3. INSERT COMPREHENSIVE PACKAGE SELECTION
DECLARE @CanteenLA INT = (SELECT TOP 1 Id FROM Canteens WHERE Location = 0 AND City = 0);
DECLARE @CanteenLD INT = (SELECT TOP 1 Id FROM Canteens WHERE Location = 1 AND City = 0);
DECLARE @CanteenHA INT = (SELECT TOP 1 Id FROM Canteens WHERE Location = 2 AND City = 0);
DECLARE @CanteenDenBosch INT = (SELECT TOP 1 Id FROM Canteens WHERE Location = 3 AND City = 2);
DECLARE @CanteenTilburg INT = (SELECT TOP 1 Id FROM Canteens WHERE Location = 4 AND City = 1);

-- Generate future-proof dates: tomorrow and day after tomorrow
DECLARE @Tomorrow DATETIME = DATEADD(day, 1, CAST(GETDATE() AS DATE));
DECLARE @DayAfter DATETIME = DATEADD(day, 2, CAST(GETDATE() AS DATE));

-- MealType enum: 0=Bread, 1=WarmEveningMeal, 2=Drink, 3=Snack, 4=Lunch, 5=Breakfast
INSERT INTO Packages (Name, City, CanteenLocation, PickupTime, LatestPickupTime, Price, MealType, CanteenId) VALUES
-- Breda LA Building (serves warm meals) - 8 packages
('Fresh Bakery Selection', 0, 0, DATEADD(hour, 8, DATEADD(minute, 0, @Tomorrow)), DATEADD(hour, 10, @Tomorrow), 4.50, 0, @CanteenLA),
('Evening Steak Dinner', 0, 0, DATEADD(hour, 18, @Tomorrow), DATEADD(hour, 20, @Tomorrow), 12.95, 1, @CanteenLA),
('Cookie & Coffee Break', 0, 0, DATEADD(hour, 14, @Tomorrow), DATEADD(hour, 16, @Tomorrow), 6.75, 3, @CanteenLA),
('Beer & Wine Tasting', 0, 0, DATEADD(hour, 17, @Tomorrow), DATEADD(hour, 19, @Tomorrow), 15.50, 2, @CanteenLA),
('Caesar Salad Lunch', 0, 0, DATEADD(hour, 12, @DayAfter), DATEADD(hour, 14, @DayAfter), 7.25, 4, @CanteenLA),
('Pancake Breakfast', 0, 0, DATEADD(hour, 7, DATEADD(minute, 30, @DayAfter)), DATEADD(hour, 9, DATEADD(minute, 30, @DayAfter)), 5.95, 5, @CanteenLA),
('Chocolate Cake Special', 0, 0, DATEADD(hour, 15, DATEADD(minute, 30, @DayAfter)), DATEADD(hour, 17, DATEADD(minute, 30, @DayAfter)), 4.25, 3, @CanteenLA),
('Wine & Dine Experience', 0, 0, DATEADD(hour, 19, @DayAfter), DATEADD(hour, 21, @DayAfter), 18.75, 1, @CanteenLA),

-- Breda LD Building (no warm meals) - 6 packages
('Daily Bread Mix', 0, 1, DATEADD(hour, 9, @Tomorrow), DATEADD(hour, 11, @Tomorrow), 3.25, 0, @CanteenLD),
('Smoothie & Juice Bar', 0, 1, DATEADD(hour, 13, @Tomorrow), DATEADD(hour, 15, @Tomorrow), 4.50, 2, @CanteenLD),
('Pretzel & Chips Combo', 0, 1, DATEADD(hour, 16, @Tomorrow), DATEADD(hour, 18, @Tomorrow), 3.75, 3, @CanteenLD),
('Club Sandwich Lunch', 0, 1, DATEADD(hour, 11, DATEADD(minute, 30, @DayAfter)), DATEADD(hour, 13, DATEADD(minute, 30, @DayAfter)), 5.50, 4, @CanteenLD),
('Bagel Breakfast Deal', 0, 1, DATEADD(hour, 8, DATEADD(minute, 15, @DayAfter)), DATEADD(hour, 10, DATEADD(minute, 15, @DayAfter)), 4.95, 5, @CanteenLD),
('Apple Pie Delight', 0, 1, DATEADD(hour, 14, DATEADD(minute, 30, @DayAfter)), DATEADD(hour, 16, DATEADD(minute, 30, @DayAfter)), 3.50, 3, @CanteenLD),

-- Breda HA Building (serves warm meals) - 7 packages
('Eggs & Toast Breakfast', 0, 2, DATEADD(hour, 7, @Tomorrow), DATEADD(hour, 9, @Tomorrow), 6.75, 5, @CanteenHA),
('Salmon & Soup Lunch', 0, 2, DATEADD(hour, 12, DATEADD(minute, 30, @Tomorrow)), DATEADD(hour, 14, DATEADD(minute, 30, @Tomorrow)), 9.50, 4, @CanteenHA),
('Thai Curry Dinner', 0, 2, DATEADD(hour, 18, DATEADD(minute, 30, @Tomorrow)), DATEADD(hour, 20, DATEADD(minute, 30, @Tomorrow)), 16.95, 1, @CanteenHA),
('Bread & Croissant Box', 0, 2, DATEADD(hour, 10, @DayAfter), DATEADD(hour, 12, @DayAfter), 4.25, 0, @CanteenHA),
('Coffee & Tea Selection', 0, 2, DATEADD(hour, 15, @DayAfter), DATEADD(hour, 17, @DayAfter), 5.75, 2, @CanteenHA),
('Mixed Snack Platter', 0, 2, DATEADD(hour, 13, DATEADD(minute, 45, @DayAfter)), DATEADD(hour, 15, DATEADD(minute, 45, @DayAfter)), 6.25, 3, @CanteenHA),
('Premium Wine Selection', 0, 2, DATEADD(hour, 19, DATEADD(minute, 30, @DayAfter)), DATEADD(hour, 21, DATEADD(minute, 30, @DayAfter)), 22.50, 1, @CanteenHA),

-- Den Bosch Building (serves warm meals) - 6 packages
('Yogurt & Croissant Breakfast', 2, 3, DATEADD(hour, 7, DATEADD(minute, 45, @Tomorrow)), DATEADD(hour, 9, DATEADD(minute, 45, @Tomorrow)), 5.50, 5, @CanteenDenBosch),
('Pasta Bolognese Lunch', 2, 3, DATEADD(hour, 12, @Tomorrow), DATEADD(hour, 14, @Tomorrow), 8.25, 4, @CanteenDenBosch),
('Chicken Soup Dinner', 2, 3, DATEADD(hour, 17, DATEADD(minute, 45, @Tomorrow)), DATEADD(hour, 19, DATEADD(minute, 45, @Tomorrow)), 14.75, 1, @CanteenDenBosch),
('Bread Variety Pack', 2, 3, DATEADD(hour, 9, DATEADD(minute, 30, @DayAfter)), DATEADD(hour, 11, DATEADD(minute, 30, @DayAfter)), 3.95, 0, @CanteenDenBosch),
('Cola & Fanta Mix', 2, 3, DATEADD(hour, 16, @DayAfter), DATEADD(hour, 18, @DayAfter), 8.50, 2, @CanteenDenBosch),
('Beer & Wine Evening', 2, 3, DATEADD(hour, 18, @DayAfter), DATEADD(hour, 20, @DayAfter), 19.95, 1, @CanteenDenBosch),

-- Tilburg Building (no warm meals) - 5 packages
('Morning Coffee & Toast', 1, 4, DATEADD(hour, 8, @Tomorrow), DATEADD(hour, 10, @Tomorrow), 4.75, 5, @CanteenTilburg),
('Sandwich & Juice Lunch', 1, 4, DATEADD(hour, 11, DATEADD(minute, 45, @Tomorrow)), DATEADD(hour, 13, DATEADD(minute, 45, @Tomorrow)), 6.50, 4, @CanteenTilburg),
('Baguette & Bread Selection', 1, 4, DATEADD(hour, 14, @Tomorrow), DATEADD(hour, 16, @Tomorrow), 3.75, 0, @CanteenTilburg),
('Refreshing Drink Mix', 1, 4, DATEADD(hour, 13, DATEADD(minute, 30, @DayAfter)), DATEADD(hour, 15, DATEADD(minute, 30, @DayAfter)), 4.95, 2, @CanteenTilburg),
('Evening Snack Pack', 1, 4, DATEADD(hour, 17, @DayAfter), DATEADD(hour, 19, @DayAfter), 5.25, 3, @CanteenTilburg);

-- Link products to packages (many-to-many relationship) - ALL 32 PACKAGES
-- Get actual Product IDs for all 32 products
DECLARE @ApplePie INT = (SELECT Id FROM Products WHERE Name = 'Apple Pie');
DECLARE @AppleSlices INT = (SELECT Id FROM Products WHERE Name = 'Apple Slices');
DECLARE @Bagel INT = (SELECT Id FROM Products WHERE Name = 'Bagel');
DECLARE @Baguette INT = (SELECT Id FROM Products WHERE Name = 'Baguette');
DECLARE @BerlinerBol INT = (SELECT Id FROM Products WHERE Name = 'Berliner Bol');
DECLARE @BreadCarreiro INT = (SELECT Id FROM Products WHERE Name = 'Bread Carreiro');
DECLARE @ChocolateCake INT = (SELECT Id FROM Products WHERE Name = 'Chocolate Cake');
DECLARE @ChocolateCookie INT = (SELECT Id FROM Products WHERE Name = 'Chocolate Cookie');
DECLARE @CroissantJam INT = (SELECT Id FROM Products WHERE Name = 'Croissant with Jam');
DECLARE @Pancakes INT = (SELECT Id FROM Products WHERE Name = 'Pancakes');
DECLARE @Pretzel INT = (SELECT Id FROM Products WHERE Name = 'Pretzel');
DECLARE @ScrambledEggs INT = (SELECT Id FROM Products WHERE Name = 'Scrambled Eggs');
DECLARE @ToastJam INT = (SELECT Id FROM Products WHERE Name = 'Toast with Jam');
DECLARE @WhiteBread INT = (SELECT Id FROM Products WHERE Name = 'White Bread');
DECLARE @YogurtMuesli INT = (SELECT Id FROM Products WHERE Name = 'Yogurt with Muesli');
DECLARE @CaesarSalad INT = (SELECT Id FROM Products WHERE Name = 'Caesar Salad');
DECLARE @ChickenSoup INT = (SELECT Id FROM Products WHERE Name = 'Chicken Soup');
DECLARE @ClubSandwich INT = (SELECT Id FROM Products WHERE Name = 'Club Sandwich');
DECLARE @GrilledSalmon INT = (SELECT Id FROM Products WHERE Name = 'Grilled Salmon');
DECLARE @PastaBolognese INT = (SELECT Id FROM Products WHERE Name = 'Pasta Bolognese');
DECLARE @SteakMashedPotato INT = (SELECT Id FROM Products WHERE Name = 'Steak with Mashed Potato');
DECLARE @ThaiGreenCurry INT = (SELECT Id FROM Products WHERE Name = 'Thai Green Curry');
DECLARE @TomatoSoup INT = (SELECT Id FROM Products WHERE Name = 'Tomato Soup');
DECLARE @ChipsNatural INT = (SELECT Id FROM Products WHERE Name = 'Chips Natural');
DECLARE @BlackCoffee INT = (SELECT Id FROM Products WHERE Name = 'Black Coffee');
DECLARE @BlueberrySmoothie INT = (SELECT Id FROM Products WHERE Name = 'Blueberry Smoothie');
DECLARE @Cola INT = (SELECT Id FROM Products WHERE Name = 'Cola');
DECLARE @Fanta INT = (SELECT Id FROM Products WHERE Name = 'Fanta');
DECLARE @OrangeJuice INT = (SELECT Id FROM Products WHERE Name = 'Orange Juice');
DECLARE @Tea INT = (SELECT Id FROM Products WHERE Name = 'Tea');
DECLARE @Beer INT = (SELECT Id FROM Products WHERE Name = 'Beer');
DECLARE @RedWine INT = (SELECT Id FROM Products WHERE Name = 'Red Wine');

INSERT INTO PackageProducts (PackagesId, ProductsId) VALUES
-- BREDA LA BUILDING (8 packages)
-- Fresh Bakery Selection (Bread)
((SELECT Id FROM Packages WHERE Name = 'Fresh Bakery Selection'), @Baguette),
((SELECT Id FROM Packages WHERE Name = 'Fresh Bakery Selection'), @WhiteBread),
((SELECT Id FROM Packages WHERE Name = 'Fresh Bakery Selection'), @BerlinerBol),
((SELECT Id FROM Packages WHERE Name = 'Fresh Bakery Selection'), @CroissantJam),

-- Evening Steak Dinner (Warm meal - no alcohol)
((SELECT Id FROM Packages WHERE Name = 'Evening Steak Dinner'), @SteakMashedPotato),
((SELECT Id FROM Packages WHERE Name = 'Evening Steak Dinner'), @TomatoSoup),
((SELECT Id FROM Packages WHERE Name = 'Evening Steak Dinner'), @Tea),
((SELECT Id FROM Packages WHERE Name = 'Evening Steak Dinner'), @WhiteBread),

-- Cookie & Coffee Break (Snacks)
((SELECT Id FROM Packages WHERE Name = 'Cookie & Coffee Break'), @ChocolateCookie),
((SELECT Id FROM Packages WHERE Name = 'Cookie & Coffee Break'), @BlackCoffee),
((SELECT Id FROM Packages WHERE Name = 'Cookie & Coffee Break'), @ApplePie),
((SELECT Id FROM Packages WHERE Name = 'Cookie & Coffee Break'), @BerlinerBol),

-- Beer & Wine Tasting (Drinks - 18+ due to alcohol)
((SELECT Id FROM Packages WHERE Name = 'Beer & Wine Tasting'), @Beer),
((SELECT Id FROM Packages WHERE Name = 'Beer & Wine Tasting'), @RedWine),
((SELECT Id FROM Packages WHERE Name = 'Beer & Wine Tasting'), @Cola),

-- Caesar Salad Lunch (Lunch)
((SELECT Id FROM Packages WHERE Name = 'Caesar Salad Lunch'), @CaesarSalad),
((SELECT Id FROM Packages WHERE Name = 'Caesar Salad Lunch'), @Baguette),
((SELECT Id FROM Packages WHERE Name = 'Caesar Salad Lunch'), @OrangeJuice),
((SELECT Id FROM Packages WHERE Name = 'Caesar Salad Lunch'), @AppleSlices),

-- Pancake Breakfast (Breakfast)
((SELECT Id FROM Packages WHERE Name = 'Pancake Breakfast'), @Pancakes),
((SELECT Id FROM Packages WHERE Name = 'Pancake Breakfast'), @YogurtMuesli),
((SELECT Id FROM Packages WHERE Name = 'Pancake Breakfast'), @BlackCoffee),
((SELECT Id FROM Packages WHERE Name = 'Pancake Breakfast'), @AppleSlices),

-- Chocolate Cake Special (Snacks)
((SELECT Id FROM Packages WHERE Name = 'Chocolate Cake Special'), @ChocolateCake),
((SELECT Id FROM Packages WHERE Name = 'Chocolate Cake Special'), @Tea),
((SELECT Id FROM Packages WHERE Name = 'Chocolate Cake Special'), @ChocolateCookie),

-- Wine & Dine Experience (Warm meal - 18+ due to wine)
((SELECT Id FROM Packages WHERE Name = 'Wine & Dine Experience'), @RedWine),
((SELECT Id FROM Packages WHERE Name = 'Wine & Dine Experience'), @Beer),
((SELECT Id FROM Packages WHERE Name = 'Wine & Dine Experience'), @GrilledSalmon),
((SELECT Id FROM Packages WHERE Name = 'Wine & Dine Experience'), @ChocolateCake),

-- BREDA LD BUILDING (6 packages)
-- Daily Bread Mix (Bread)
((SELECT Id FROM Packages WHERE Name = 'Daily Bread Mix'), @Baguette),
((SELECT Id FROM Packages WHERE Name = 'Daily Bread Mix'), @WhiteBread),
((SELECT Id FROM Packages WHERE Name = 'Daily Bread Mix'), @BreadCarreiro),

-- Smoothie & Juice Bar (Drinks - no alcohol)
((SELECT Id FROM Packages WHERE Name = 'Smoothie & Juice Bar'), @BlueberrySmoothie),
((SELECT Id FROM Packages WHERE Name = 'Smoothie & Juice Bar'), @OrangeJuice),
((SELECT Id FROM Packages WHERE Name = 'Smoothie & Juice Bar'), @Tea),

-- Pretzel & Chips Combo (Snacks)
((SELECT Id FROM Packages WHERE Name = 'Pretzel & Chips Combo'), @Pretzel),
((SELECT Id FROM Packages WHERE Name = 'Pretzel & Chips Combo'), @ChipsNatural),
((SELECT Id FROM Packages WHERE Name = 'Pretzel & Chips Combo'), @Cola),

-- Club Sandwich Lunch (Lunch)
((SELECT Id FROM Packages WHERE Name = 'Club Sandwich Lunch'), @ClubSandwich),
((SELECT Id FROM Packages WHERE Name = 'Club Sandwich Lunch'), @ChipsNatural),
((SELECT Id FROM Packages WHERE Name = 'Club Sandwich Lunch'), @Fanta),

-- Bagel Breakfast Deal (Breakfast)
((SELECT Id FROM Packages WHERE Name = 'Bagel Breakfast Deal'), @Bagel),
((SELECT Id FROM Packages WHERE Name = 'Bagel Breakfast Deal'), @ScrambledEggs),
((SELECT Id FROM Packages WHERE Name = 'Bagel Breakfast Deal'), @BlackCoffee),

-- Apple Pie Delight (Snacks)
((SELECT Id FROM Packages WHERE Name = 'Apple Pie Delight'), @ApplePie),
((SELECT Id FROM Packages WHERE Name = 'Apple Pie Delight'), @AppleSlices),
((SELECT Id FROM Packages WHERE Name = 'Apple Pie Delight'), @Tea),

-- BREDA HA BUILDING (7 packages)
-- Eggs & Toast Breakfast (Breakfast)
((SELECT Id FROM Packages WHERE Name = 'Eggs & Toast Breakfast'), @ScrambledEggs),
((SELECT Id FROM Packages WHERE Name = 'Eggs & Toast Breakfast'), @ToastJam),
((SELECT Id FROM Packages WHERE Name = 'Eggs & Toast Breakfast'), @BlackCoffee),
((SELECT Id FROM Packages WHERE Name = 'Eggs & Toast Breakfast'), @OrangeJuice),

-- Salmon & Soup Lunch (Lunch)
((SELECT Id FROM Packages WHERE Name = 'Salmon & Soup Lunch'), @GrilledSalmon),
((SELECT Id FROM Packages WHERE Name = 'Salmon & Soup Lunch'), @TomatoSoup),
((SELECT Id FROM Packages WHERE Name = 'Salmon & Soup Lunch'), @WhiteBread),
((SELECT Id FROM Packages WHERE Name = 'Salmon & Soup Lunch'), @Tea),

-- Thai Curry Dinner (Warm meal - no alcohol)
((SELECT Id FROM Packages WHERE Name = 'Thai Curry Dinner'), @ThaiGreenCurry),
((SELECT Id FROM Packages WHERE Name = 'Thai Curry Dinner'), @Baguette),
((SELECT Id FROM Packages WHERE Name = 'Thai Curry Dinner'), @Tea),
((SELECT Id FROM Packages WHERE Name = 'Thai Curry Dinner'), @AppleSlices),

-- Bread & Croissant Box (Bread)
((SELECT Id FROM Packages WHERE Name = 'Bread & Croissant Box'), @Baguette),
((SELECT Id FROM Packages WHERE Name = 'Bread & Croissant Box'), @CroissantJam),
((SELECT Id FROM Packages WHERE Name = 'Bread & Croissant Box'), @BerlinerBol),

-- Coffee & Tea Selection (Drinks - no alcohol)
((SELECT Id FROM Packages WHERE Name = 'Coffee & Tea Selection'), @BlackCoffee),
((SELECT Id FROM Packages WHERE Name = 'Coffee & Tea Selection'), @Tea),
((SELECT Id FROM Packages WHERE Name = 'Coffee & Tea Selection'), @BlueberrySmoothie),

-- Mixed Snack Platter (Snacks)
((SELECT Id FROM Packages WHERE Name = 'Mixed Snack Platter'), @ChocolateCookie),
((SELECT Id FROM Packages WHERE Name = 'Mixed Snack Platter'), @Pretzel),
((SELECT Id FROM Packages WHERE Name = 'Mixed Snack Platter'), @ChipsNatural),
((SELECT Id FROM Packages WHERE Name = 'Mixed Snack Platter'), @ApplePie),

-- Premium Wine Selection (Warm meal - 18+ due to wine)
((SELECT Id FROM Packages WHERE Name = 'Premium Wine Selection'), @RedWine),
((SELECT Id FROM Packages WHERE Name = 'Premium Wine Selection'), @Beer),
((SELECT Id FROM Packages WHERE Name = 'Premium Wine Selection'), @ChocolateCake),
((SELECT Id FROM Packages WHERE Name = 'Premium Wine Selection'), @GrilledSalmon),
((SELECT Id FROM Packages WHERE Name = 'Premium Wine Selection'), @Baguette),

-- DEN BOSCH BUILDING (6 packages)
-- Yogurt & Croissant Breakfast (Breakfast)
((SELECT Id FROM Packages WHERE Name = 'Yogurt & Croissant Breakfast'), @YogurtMuesli),
((SELECT Id FROM Packages WHERE Name = 'Yogurt & Croissant Breakfast'), @CroissantJam),
((SELECT Id FROM Packages WHERE Name = 'Yogurt & Croissant Breakfast'), @BlackCoffee),

-- Pasta Bolognese Lunch (Lunch)
((SELECT Id FROM Packages WHERE Name = 'Pasta Bolognese Lunch'), @PastaBolognese),
((SELECT Id FROM Packages WHERE Name = 'Pasta Bolognese Lunch'), @WhiteBread),
((SELECT Id FROM Packages WHERE Name = 'Pasta Bolognese Lunch'), @Cola),
((SELECT Id FROM Packages WHERE Name = 'Pasta Bolognese Lunch'), @AppleSlices),

-- Chicken Soup Dinner (Warm meal - no alcohol)
((SELECT Id FROM Packages WHERE Name = 'Chicken Soup Dinner'), @ChickenSoup),
((SELECT Id FROM Packages WHERE Name = 'Chicken Soup Dinner'), @Baguette),
((SELECT Id FROM Packages WHERE Name = 'Chicken Soup Dinner'), @Tea),

-- Bread Variety Pack (Bread)
((SELECT Id FROM Packages WHERE Name = 'Bread Variety Pack'), @Baguette),
((SELECT Id FROM Packages WHERE Name = 'Bread Variety Pack'), @WhiteBread),
((SELECT Id FROM Packages WHERE Name = 'Bread Variety Pack'), @BreadCarreiro),
((SELECT Id FROM Packages WHERE Name = 'Bread Variety Pack'), @BerlinerBol),

-- Cola & Fanta Mix (Drinks - 18+ due to alcohol)
((SELECT Id FROM Packages WHERE Name = 'Cola & Fanta Mix'), @RedWine),
((SELECT Id FROM Packages WHERE Name = 'Cola & Fanta Mix'), @Beer),
((SELECT Id FROM Packages WHERE Name = 'Cola & Fanta Mix'), @Cola),
((SELECT Id FROM Packages WHERE Name = 'Cola & Fanta Mix'), @Fanta),

-- Beer & Wine Evening (Warm meal - 18+ due to wine)
((SELECT Id FROM Packages WHERE Name = 'Beer & Wine Evening'), @RedWine),
((SELECT Id FROM Packages WHERE Name = 'Beer & Wine Evening'), @Beer),
((SELECT Id FROM Packages WHERE Name = 'Beer & Wine Evening'), @ChocolateCake),
((SELECT Id FROM Packages WHERE Name = 'Beer & Wine Evening'), @GrilledSalmon),

-- TILBURG BUILDING (5 packages)
-- Morning Coffee & Toast (Breakfast)
((SELECT Id FROM Packages WHERE Name = 'Morning Coffee & Toast'), @BlackCoffee),
((SELECT Id FROM Packages WHERE Name = 'Morning Coffee & Toast'), @ToastJam),
((SELECT Id FROM Packages WHERE Name = 'Morning Coffee & Toast'), @ScrambledEggs),
((SELECT Id FROM Packages WHERE Name = 'Morning Coffee & Toast'), @OrangeJuice),

-- Sandwich & Juice Lunch (Lunch)
((SELECT Id FROM Packages WHERE Name = 'Sandwich & Juice Lunch'), @ClubSandwich),
((SELECT Id FROM Packages WHERE Name = 'Sandwich & Juice Lunch'), @ChipsNatural),
((SELECT Id FROM Packages WHERE Name = 'Sandwich & Juice Lunch'), @OrangeJuice),

-- Baguette & Bread Selection (Bread)
((SELECT Id FROM Packages WHERE Name = 'Baguette & Bread Selection'), @Baguette),
((SELECT Id FROM Packages WHERE Name = 'Baguette & Bread Selection'), @WhiteBread),
((SELECT Id FROM Packages WHERE Name = 'Baguette & Bread Selection'), @BerlinerBol),

-- Refreshing Drink Mix (Drinks - no alcohol)
((SELECT Id FROM Packages WHERE Name = 'Refreshing Drink Mix'), @Cola),
((SELECT Id FROM Packages WHERE Name = 'Refreshing Drink Mix'), @Fanta),
((SELECT Id FROM Packages WHERE Name = 'Refreshing Drink Mix'), @BlueberrySmoothie),

-- Evening Snack Pack (Snacks)
((SELECT Id FROM Packages WHERE Name = 'Evening Snack Pack'), @ChocolateCookie),
((SELECT Id FROM Packages WHERE Name = 'Evening Snack Pack'), @Pretzel),
((SELECT Id FROM Packages WHERE Name = 'Evening Snack Pack'), @ChipsNatural),
((SELECT Id FROM Packages WHERE Name = 'Evening Snack Pack'), @Cola);

PRINT 'Package-Product relationships created';

-- VERIFY PACKAGE IDs FOR TESTING
PRINT 'Package ID verification:';
SELECT 'Package ' + CAST(Id AS VARCHAR) + ': ' + Name + ' (' + 
       CASE 
         WHEN EXISTS (SELECT 1 FROM PackageProducts pp JOIN Products p ON pp.ProductsId = p.Id 
                     WHERE pp.PackagesId = pkg.Id AND p.ContainsAlcohol = 1) 
         THEN '18+' 
         ELSE 'All Ages' 
       END + ')' AS PackageInfo,
       'Pickup: ' + CONVERT(VARCHAR, PickupTime, 120) AS PickupInfo
FROM Packages pkg
ORDER BY Id;

-- 4. CANTEEN EMPLOYEES
-- CanteenEmployees are created through API when registering employee users
-- This maintains proper linking between Identity users and employee records

-- 5. VERIFICATION
DECLARE @CanteenCount INT = (SELECT COUNT(*) FROM Canteens);
DECLARE @ProductCount INT = (SELECT COUNT(*) FROM Products);
DECLARE @PackageCount INT = (SELECT COUNT(*) FROM Packages);
DECLARE @AlcoholPackages INT = (SELECT COUNT(DISTINCT pp.PackagesId) 
                               FROM PackageProducts pp 
                               JOIN Products p ON pp.ProductsId = p.Id 
                               WHERE p.ContainsAlcohol = 1);

PRINT '=== DATABASE SEEDED SUCCESSFULLY ===';
PRINT 'Canteens: ' + CAST(@CanteenCount AS VARCHAR) + ' (Expected: 5)';
PRINT 'Products: ' + CAST(@ProductCount AS VARCHAR) + ' (Expected: 32)';
PRINT 'Packages: ' + CAST(@PackageCount AS VARCHAR) + ' (Expected: 32)';
PRINT '18+ Packages: ' + CAST(@AlcoholPackages AS VARCHAR) + ' (Contains alcohol)';
PRINT '';
PRINT 'PACKAGE DISTRIBUTION:';
PRINT 'Breda LA: 8 packages | Breda LD: 6 packages | Breda HA: 7 packages';
PRINT 'Den Bosch: 6 packages | Tilburg: 5 packages';
PRINT 'REALISTIC PRODUCTS: Breakfast items, main meals, snacks, and beverages';
PRINT '';

-- Show which packages are 18+ for testing
SELECT 'Package ' + CAST(pkg.Id AS VARCHAR) + ' (' + pkg.Name + ') is 18+ due to: ' +
       STRING_AGG(p.Name, ', ') AS AlcoholProducts
FROM Packages pkg
JOIN PackageProducts pp ON pkg.Id = pp.PackagesId
JOIN Products p ON pp.ProductsId = p.Id
WHERE p.ContainsAlcohol = 1
GROUP BY pkg.Id, pkg.Name;