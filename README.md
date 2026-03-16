# HotelApi

REST API for managing hotel room reservations, built with ASP.NET Core and SQLite.

## Tech Stack

- ASP.NET Core 10
- Entity Framework Core 10
- SQLite
- Scalar (API documentation)

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Run locally
```bash
git clone <your-repo-url>
cd HotelApi
dotnet restore
dotnet ef database update
dotnet run
```

API will be available at `http://localhost:5138`
Interactive API docs at `http://localhost:5138/scalar/v1`

## Project Structure
```
HotelApi/
├── Controllers/
│   ├── RoomsController.cs
│   └── ReservationsController.cs
├── Data/
│   └── AppDbContext.cs
├── Dto/
│   ├── CreateReservationDto.cs
│   └── UpdateReservationDto.cs
├── Models/
│   ├── Room.cs
│   └── Reservation.cs
└── Migrations/
```

## API Endpoints

### Rooms

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/rooms` | Get all rooms with availability status |
| GET | `/api/rooms/{id}` | Get room by ID with current reservation |

### Reservations

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/reservations` | Get all reservations |
| GET | `/api/reservations/{id}` | Get reservation by ID |
| POST | `/api/reservations` | Create a new reservation |
| PUT | `/api/reservations/{id}` | Update an existing reservation |
| DELETE | `/api/reservations/{id}` | Cancel a reservation |

### POST /api/reservations — request body
```json
{
  "roomId": 1,
  "guestName": "John Smith",
  "guestEmail": "john@example.com",
  "checkIn": "2026-04-01T14:00:00",
  "checkOut": "2026-04-05T11:00:00"
}
```

## Business Rules

- One reservation per room at a time — enforced at both application level and database level via a unique index on `RoomId`
- Check-out must be after check-in
- Attempting to book an already reserved room returns `409 Conflict`

## Database

SQLite database (`hotel.db`) is created automatically on first run via EF Core migrations. The database is seeded with 5 rooms on startup:

| Room | Type | Price/night |
|------|------|-------------|
| 101 | Single | $80 |
| 102 | Single | $80 |
| 201 | Double | $120 |
| 202 | Double | $120 |
| 301 | Suite | $250 |

## Notes

- SQLite is used for simplicity — can be replaced with PostgreSQL or SQL Server by changing the connection string and EF Core provider
- CORS is configured to allow requests from `http://localhost:3000`