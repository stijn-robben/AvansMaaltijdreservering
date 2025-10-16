-- ========================================
-- CLEAR ALL USERS AND USER DATA - AZURE VERSION
-- ========================================
--
-- INSTRUCTIES:
-- 1. Verbind eerst met de IDENTITY database in Azure
-- 2. Voer DEEL 1 uit
-- 3. Verander connectie naar de APPLICATION database in Azure
-- 4. Voer DEEL 2 uit
--
-- ========================================

-- ========================================
-- DEEL 1: Voer uit in IDENTITY DATABASE
-- ========================================

PRINT 'Clearing Identity Database...';

DELETE FROM AspNetUserRoles;
DELETE FROM AspNetUserClaims;
DELETE FROM AspNetUserLogins;
DELETE FROM AspNetUserTokens;
DELETE FROM AspNetUsers;

PRINT 'Cleared all Identity users';
PRINT '';


  SELECT COUNT(*) FROM AspNetUsers;

-- ========================================
-- DEEL 2: Voer uit in APPLICATION DATABASE
-- ========================================

PRINT 'Clearing Main Database user data...';

-- Clear reservations (packages with ReservedByStudentId)
UPDATE Packages SET ReservedByStudentId = NULL WHERE ReservedByStudentId IS NOT NULL;
PRINT 'Cleared package reservations';

-- Clear students
DELETE FROM Students;
PRINT 'Cleared Students';

-- Clear employees
DELETE FROM CanteenEmployees;
PRINT 'Cleared CanteenEmployees';
  SELECT COUNT(*) FROM Students;
  SELECT COUNT(*) FROM CanteenEmployees;
  SELECT COUNT(*) FROM Packages WHERE ReservedByStudentId IS NOT NULL;

PRINT '';
PRINT '==================';
PRINT 'ALL USERS CLEARED!';
PRINT '==================';
