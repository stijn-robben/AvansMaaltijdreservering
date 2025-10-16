-- ========================================
-- CLEAR ALL USERS AND USER DATA
-- ========================================

-- STEP 1: Clear Identity Database
USE sswpi-stijnrobben-2;
PRINT 'Clearing Identity Database...';

DELETE FROM AspNetUserRoles;
DELETE FROM AspNetUserClaims;
DELETE FROM AspNetUserLogins;
DELETE FROM AspNetUserTokens;
DELETE FROM AspNetUsers;
PRINT 'Cleared all Identity users';

-- STEP 2: Clear Main Database
USE AvansMaaltijdreservering;
PRINT 'Clearing Main Database user data...';

-- Clear reservations (packages with ReservedByStudentId)
UPDATE Packages SET ReservedByStudentId = NULL WHERE ReservedByStudentId IS NOT NULL;
PRINT 'Cleared package reservations';

-- Clear students
DELETE FROM Students;
PRINT 'Cleared Students';

DELETE FROM CanteenEmployees;
PRINT 'Cleared CanteenEmployees';

PRINT '';
PRINT '==================';

PRINT 'ALL USERS CLEARED!';
PRINT '==================';