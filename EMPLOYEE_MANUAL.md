# 👨‍🍳 Avans Meal Rescue - Canteen Employee Manual

## 🎯 Mission: Reduce Food Waste, Help Students
As a canteen employee, you're on the frontline of fighting food waste while helping students access affordable, quality meals!

---

## 🚀 Getting Started

### 1. Account Registration
1. Visit the Avans Meal Rescue website
2. Click **"Register"** in the top navigation
3. Select **"I'm a Canteen Employee"**
4. Fill in your details:
   - **Full Name**: Your real name
   - **Email**: Your work email address  
   - **Password**: Choose a strong password
   - **Employee Number**: Your official Avans employee number
   - **Your Canteen Location**: Select from:
     - Breda - LA Building
     - Breda - LD Building  
     - Breda - HA Building
     - Den Bosch Building
     - Tilburg Building
5. Click **"Create Account & Start Saving Food!"**
6. Success message appears - you can now log in!

### 2. Logging In
1. Click **"Login"** in the top menu
2. Enter your **email address** and **password**  
3. Check "Remember me" for convenience
4. Click **"Login"** - redirects to Employee Dashboard

---

## 🏢 Employee Dashboard Overview

### Two Main Sections

#### 📍 Your Canteen Packages
- **Only your location**: Packages you've created for your canteen
- **Full management**: Edit, delete, and monitor your packages
- **Reservation status**: See which packages are reserved and by whom
- **Quick actions**: Create, modify, and manage no-shows

#### 🏛️ All Canteens Overview
- **System-wide view**: See packages from all Avans canteens
- **Monitoring tool**: Track overall system activity
- **Reference data**: Learn from other locations' strategies
- **Read-only**: You can view but not modify other canteens' packages

---

## 📦 Package Management

### Creating a New Package

#### 1. Click "Create Package"
- Large button in dashboard header
- Or use navigation menu

#### 2. Package Information
- **Package Name**: Descriptive title (3-100 characters)
  - ✅ Good: "Fresh Evening Bread Selection"
  - ✅ Good: "Warm Dinner Deal - Pasta & Salad"
  - ❌ Avoid: "Package 1", "Food"
- **Price**: Set attractive discount (€0.01 - €999.99)
  - 💡 Tip: 50-70% off regular price works well

#### 3. Location & Timing
- **City**: Automatically matches your canteen location
- **Canteen Location**: Pre-set to your assigned location
- **Pickup Time**: When students can start collecting
  - ⚠️ **Rule**: Maximum 2 days ahead
  - 💡 **Best practice**: Same day or next day
- **Latest Pickup Time**: Final deadline for collection
  - 💡 **Recommended**: 1-2 hours after pickup time

#### 4. Meal Category Selection
Choose from:
- 🍞 **Bread & Bakery**: Fresh bread, pastries, baked goods
- 🍽️ **Warm Evening Meal**: Hot dinners, prepared meals
- 🥤 **Beverages**: Drinks, juices, coffee
- 🍿 **Snacks**: Light bites, chips, cookies  
- 🥪 **Lunch**: Sandwiches, salads, lunch items
- 🥐 **Breakfast**: Morning items, cereals, yogurts

#### ⚠️ US_09: Warm Meal Location Restrictions
**Important Business Rule**: Warm evening meals can only be created at locations with kitchen facilities:
- ✅ **Can serve warm meals**: Breda LA Building, Den Bosch Building, Tilburg Building
- ❌ **Cannot serve warm meals**: Breda LD Building, Breda HA Building

The system will warn you if you try to create warm meals at restricted locations.

#### 5. Product Selection (US_06)
- **Purpose**: Give students examples of what they might receive
- **Select products** that could be in this package type
- **Important**: These are examples only - actual contents may vary
- **Auto 18+ detection**: If you select any alcoholic products, package automatically becomes 18+

#### 6. Review and Create
- Check all details carefully
- Click **"Create Package"**
- Package becomes immediately available to students!

### 📝 Editing Existing Packages

#### When You Can Edit:
- ✅ **Before reservation**: Full editing allowed
- ❌ **After reservation**: Cannot modify (US_03 business rule)

#### Editing Process:
1. Find package in "Your Canteen Packages" section
2. Click **"Edit"** button
3. Modify any details (same form as creation)
4. **Save changes**

#### ⚠️ Time Restrictions:
- **2-day limit**: Cannot plan more than 2 days ahead
- **Future dates only**: Pickup time must be in the future
- **Logical timing**: Latest pickup must be after pickup time

### 🗑️ Deleting Packages

#### When You Can Delete:
- ✅ **Unreserved packages**: No student has claimed it
- ❌ **Reserved packages**: Cannot delete (US_03 business rule)

#### Deletion Process:
1. Click **"Delete"** button in package row
2. **Confirm deletion** in popup dialog
3. Package is permanently removed from system

---

## 👥 Student Reservation Management

### Understanding Package Status

#### 🔵 Available (Blue Badge)
- No student has reserved it yet
- Can be edited or deleted
- Visible to all eligible students

#### 🟢 Reserved (Green Badge)  
- Student has claimed this package
- Shows student name and number
- Cannot be modified or deleted
- Student must pick up within time window

#### 🔴 Expired (Red/Gray)
- Latest pickup time has passed
- May need no-show processing if student didn't collect

### 📅 Daily Package Monitoring

#### Morning Routine:
1. **Check today's packages**: Look for yellow-highlighted rows
2. **Prepare reserved items**: Get packages ready for pickup
3. **Monitor pickup times**: Be available during pickup windows

#### During Service Hours:
1. **Handle pickups**: Verify student ID and process payment
2. **Check reservations**: Confirm package contents match expectations
3. **Update status**: Mental note of successful pickups

#### End of Day:
1. **Identify no-shows**: Check expired reservations
2. **Process no-shows**: Use "No-Show" button for missed pickups
3. **Plan tomorrow**: Consider creating new packages based on available inventory

---

## ⚠️ No-Show Management (US_10)

### What is a No-Show?
- **Student reserved** a package
- **Didn't collect** by latest pickup time
- **Food goes to waste** - exactly what we're trying to prevent!

### Processing No-Shows

#### When to Register:
- ✅ **After latest pickup time** has passed
- ✅ **Student never showed up** to collect
- ✅ **No communication** from student

#### How to Process:
1. **Find the expired reservation** in your canteen packages
2. **Click "No-Show" button** (appears for expired reserved packages)  
3. **Confirm action** - this is permanent
4. **System automatically**:
   - Increments student's no-show count
   - Releases package back to available pool
   - Logs the incident for tracking

#### Student Impact:
- **1 No-Show**: Warning shown to student
- **2+ No-Shows**: Student account blocked from making new reservations
- **Appeals**: Students must contact student services to reactivate

### 💡 Best Practices:
- **Give students grace period**: Wait a few minutes past deadline
- **Try calling**: If you have student contact info
- **Document patterns**: Note frequent no-show students
- **Communicate**: Inform student services of chronic issues

---

## 📊 Business Rules & Compliance

### US_03: Package Modification Rules
- ✅ **Before reservation**: Full CRUD operations allowed
- ❌ **After reservation**: No modifications allowed
- ✅ **Maximum 2 days ahead**: Cannot plan packages beyond this
- ✅ **Own canteen only**: Can only manage your location's packages

### US_04: Age Restrictions (Automatic)
- **System handles**: Auto-detects alcohol in product selection
- **18+ marking**: Automatically applied to packages with alcohol
- **Student blocking**: System prevents minors from reserving 18+ packages

### US_07: First-Come-First-Served
- **Reservation race**: Students compete for popular packages
- **System handles**: Automatic locking during reservation process
- **Fair access**: No preferential treatment or holds

### US_09: Warm Meal Location Validation
- **Kitchen facilities required**: Only equipped locations can serve warm meals
- **System enforcement**: Form validation prevents invalid combinations
- **Override**: No manual override - business rule is absolute

### US_10: No-Show Enforcement
- **Employee discretion**: You decide when to register no-shows
- **Not automatic**: System doesn't auto-process missed pickups
- **Final decision**: Your judgment on grace periods and special circumstances

---

## 📈 Package Strategy & Best Practices

### Timing Strategies

#### ⏰ Optimal Posting Times:
- **Morning packages**: Post around 7-8 AM for breakfast items
- **Lunch packages**: Post around 10-11 AM for lunch deals
- **Evening packages**: Post around 2-3 PM for dinner items

#### ⏰ Pickup Time Planning:
- **End of service**: 30 minutes before you close service
- **Peak hours**: Avoid during busy regular service times
- **Student schedules**: Consider class times and breaks

### Pricing Psychology

#### 💰 Sweet Spot Pricing:
- **50-70% discount**: Students feel they're getting great value
- **Round numbers**: €2.50, €3.00, €5.00 are easy to process
- **Value perception**: €4.99 feels cheaper than €5.00

#### 💰 Package Sizing:
- **Individual portions**: Most popular with students
- **Sharing size**: Mark clearly if intended for multiple people
- **Family packs**: Less popular but good for specific audiences

### Content Strategy

#### 🍽️ Popular Package Types:
1. **Bread packages**: Always in demand, high turnover
2. **Warm meals**: Premium pricing, target dinner crowd
3. **Snack boxes**: Great for study sessions
4. **Breakfast deals**: Popular with early classes
5. **Beverage bundles**: Good margins, easy to package

#### 🍽️ Product Selection Tips:
- **Visual appeal**: Choose photogenic items for examples
- **Variety**: Mix of different food types increases interest
- **Quality indicators**: Include premium items to justify price
- **Allergen awareness**: Consider common dietary restrictions

---

## 🔧 Troubleshooting Common Issues

### Package Creation Problems

#### "Cannot create warm meals at this location"
- **Cause**: Your canteen doesn't have warm meal facilities (US_09)
- **Solution**: Choose different meal type or contact IT if this is incorrect

#### "Pickup time must be within 2 days"
- **Cause**: You're trying to plan too far ahead (US_03)
- **Solution**: Choose earlier pickup date (today or tomorrow)

#### "Cannot modify package with existing reservation"
- **Cause**: Student has already reserved this package (US_03)
- **Solution**: Create new package instead, or wait until after pickup

### Student Pickup Issues

#### Student claims they reserved but can't find it
1. **Check spelling**: Verify student name and number
2. **Check date**: Ensure they're looking at correct pickup date
3. **System lookup**: Search your canteen packages for their reservation
4. **Escalate**: Contact IT support if technical issue suspected

#### Student arrives outside pickup window
1. **Grace period**: Use discretion for reasonable delays
2. **Expired reservation**: Explain no-show policy
3. **Alternative**: Offer to sell at regular price if available
4. **Document**: Consider registering as no-show if significantly late

### Technical Problems

#### Dashboard not loading
1. **Refresh page**: Simple fix for many issues
2. **Clear browser cache**: Solves persistent display problems
3. **Try different browser**: Chrome, Firefox, Safari, Edge
4. **Check internet**: Ensure stable connection

#### Can't see packages from other canteens
- **Normal behavior**: This view is for monitoring only
- **System wide issues**: Contact IT if ALL canteens show empty

---

## 📞 Support & Escalation

### When to Contact IT Support:
- ✅ **Website technical issues**: Pages not loading, errors
- ✅ **Data inconsistencies**: Numbers don't add up
- ✅ **Authentication problems**: Can't log in with correct credentials
- ✅ **System bugs**: Unexpected behavior or error messages

### When to Contact Management:
- ✅ **Policy questions**: Unclear business rules
- ✅ **Student complaints**: Escalated issues about service
- ✅ **Inventory coordination**: Large-scale food waste opportunities
- ✅ **Training needs**: Staff onboarding for system

### When to Contact Student Services:
- ✅ **Blocked student appeals**: No-show account reactivations
- ✅ **Student behavior issues**: Abuse of system or aggressive behavior
- ✅ **Special circumstances**: Medical/personal situations affecting pickups

---

## 📊 Success Metrics & Impact

### Track Your Impact

#### 📈 Key Performance Indicators:
- **Packages created**: How many rescue opportunities you've provided
- **Reservation rate**: Percentage of packages that get claimed
- **Pickup success rate**: Percentage of reservations that convert to actual pickups
- **No-show rate**: Monitor and work to minimize

#### 📈 Food Waste Reduction:
- **Pounds saved**: Estimate weight of food rescued from waste
- **Cost savings**: For both institution and students
- **Environmental impact**: CO2 reduction from preventing food waste

### Continuous Improvement

#### 🔍 Analysis Questions:
- Which meal types are most popular?
- What pricing strategies work best?
- When do students prefer to pick up packages?
- Which products generate most interest?

#### 🔍 Optimization Strategies:
- **A/B test pricing**: Try different price points for similar packages
- **Timing experiments**: Vary posting and pickup times
- **Content mixing**: Combine different product types
- **Feedback collection**: Ask students informally about preferences

---

## 🎯 Quick Reference Guide

### Daily Workflow:
1. **Morning**: Check dashboard, prepare today's pickups
2. **During service**: Handle pickups, verify student IDs
3. **Plan ahead**: Create tomorrow's packages based on inventory
4. **End of day**: Process any no-shows, review success

### Package Creation Checklist:
- ✅ Descriptive, appetizing name
- ✅ Competitive pricing (50-70% discount)
- ✅ Realistic pickup time window
- ✅ Appropriate meal type for location
- ✅ Representative product examples
- ✅ Double-check all details before saving

### Emergency Procedures:
- **System down**: Use backup order tracking method
- **Student dispute**: Remain calm, document issue, escalate if needed
- **Large food surplus**: Contact management for bulk package creation
- **Staff shortage**: Prioritize existing reservations over new package creation

### Business Rule Reminders:
- 📅 **2 days maximum** planning ahead
- 🏢 **Own canteen only** for modifications
- 🔞 **18+ auto-detection** for alcohol products
- 🍽️ **Warm meal location restrictions** enforced
- ⚠️ **No-show discretion** - you decide when to register

---

**🌟 You're making a real difference in reducing food waste while helping students! Every package you create is a step toward a more sustainable campus! 🌟**