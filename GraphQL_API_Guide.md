# Avans Meal Rescue - GraphQL API Guide

## Overview
This GraphQL endpoint is designed for the AvansOne mobile app team. It provides efficient data fetching with the ability to request exactly the data you need.

## Endpoint
```
GET/POST /graphql
```

## Development Tools
- **GraphQL IDE**: Available at `/graphql` in development mode
- **Schema Explorer**: Browse all available queries and mutations

## Sample Queries

### Get Available Packages
```graphql
query GetPackages($city: City, $mealType: MealType) {
  packages(city: $city, mealType: $mealType) {
    id
    name
    city
    mealType
    price
    pickupTime
    lastPickupTime
    isAdultOnly
    reservedById
    products {
      id
      name
      containsAlcohol
      photo
    }
  }
}
```

### Get Package Details
```graphql
query GetPackage($id: Int!) {
  package(id: $id) {
    id
    name
    city
    canteenLocation
    mealType
    price
    pickupTime
    lastPickupTime
    isAdultOnly
    reservedById
    products {
      id
      name
      containsAlcohol
      photo
    }
  }
}
```

### Search Packages
```graphql
query SearchPackages($searchTerm: String!) {
  searchPackages(searchTerm: $searchTerm) {
    id
    name
    city
    mealType
    price
    pickupTime
    products {
      name
    }
  }
}
```

### Get Products
```graphql
query GetProducts {
  products {
    id
    name
    containsAlcohol
    photo
  }
}
```

## Sample Mutations

### Make Reservation
```graphql
mutation MakeReservation($packageId: Int!) {
  makeReservation(packageId: $packageId) {
    success
    message
    packageId
  }
}
```

### Cancel Reservation
```graphql
mutation CancelReservation($packageId: Int!) {
  cancelReservation(packageId: $packageId) {
    success
    message
    packageId
  }
}
```

### Register Student
```graphql
mutation RegisterStudent($input: StudentInput!) {
  registerStudent(input: $input) {
    success
    message
    student {
      id
      name
      email
      studyCity
    }
  }
}
```

## Filtering & Sorting

### Filter by City
```graphql
query PackagesByCity {
  packages(city: BREDA) {
    id
    name
    city
  }
}
```

### Filter by Meal Type
```graphql
query WarmMeals {
  packages(mealType: WARM_EVENING_MEAL) {
    id
    name
    mealType
  }
}
```

### Only Available Packages
```graphql
query AvailablePackages {
  packages(isAvailable: true) {
    id
    name
    reservedById
  }
}
```

## Enums Available

### City
- `BREDA`
- `TILBURG` 
- `DENBOSCH`

### MealType
- `BREAD`
- `WARM_EVENING_MEAL`
- `DRINK`

### CanteenLocation
- `BREDA_LA_BUILDING`
- `BREDA_LD_BUILDING` 
- `BREDA_HA_BUILDING`
- `DENBOSCH_BUILDING`
- `TILBURG_BUILDING`

## Error Handling
All mutations return a result object with:
- `success`: Boolean indicating if operation succeeded
- `message`: Human-readable success/error message
- Additional fields depending on the operation

## Authentication
Authentication is handled automatically through the existing Identity system. Make sure users are logged in before making reservation-related mutations.

## Mobile App Integration Tips

1. **Efficient Queries**: Request only the fields you need
2. **Batch Operations**: Use fragments for reusable field sets
3. **Real-time Updates**: Consider implementing subscriptions for live updates
4. **Caching**: GraphQL responses are perfect for client-side caching
5. **Error Handling**: Always check the `success` field in mutation responses

## Example Mobile App Flow

```typescript
// 1. Get available packages for student's city
const packages = await graphql(`
  query GetAvailablePackages($city: City!) {
    packages(city: $city, isAvailable: true) {
      id
      name
      price
      pickupTime
      products { name }
    }
  }
`, { city: studentCity });

// 2. Make reservation
const result = await graphql(`
  mutation MakeReservation($packageId: Int!) {
    makeReservation(packageId: $packageId) {
      success
      message
    }
  }
`, { packageId: selectedPackageId });

if (result.makeReservation.success) {
  // Show success message
} else {
  // Handle error
}
```