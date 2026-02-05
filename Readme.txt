HOTEL RESERVATION API – UPUTE ZA KORIŠTENJE

(Admin → User demonstracija + autorizacija)

Ovaj dokument opisuje kako se demonstrira cijeli sustav:

JWT autentifikacija

Role-based autorizacija (Admin / User)

Svi controlleri: Auth, Admin, Facilities, RoomTypes, Rooms, TimeSlots, Reservations

Sve se testira kroz Swagger UI.

0) Pokretanje aplikacije

Pokrenuti API (F5 u Visual Studio ili dotnet run)

Otvoriti Swagger:

https://localhost:PORT/swagger

U Swaggeru koristiti gumb "Authorize" za unos JWT tokena.

1) AUTH – Registracija i Login
1A) Registracija korisnika

Controller: AuthController
Endpoint: POST /api/auth/register

Primjer body-ja:

{
"email": "user1@hotel.local
",
"password": "Test123!",
"fullName": "User One"
}

Rezultat:

200 OK → "Registered"

Korisnik dobije rolu User

Admin se najčešće kreira kroz IdentitySeeder.

1B) Login (Admin)

Endpoint: POST /api/auth/login

{
"email": "admin@hotel.local
",
"password": "Admin123!"
}

Response:

{ "token": "JWT_TOKEN" }

Token kopirati i zalijepiti u Swagger → Authorize:

Bearer JWT_TOKEN

1C) Login (User)

Endpoint: POST /api/auth/login

{
"email": "user1@hotel.local
",
"password": "Test123!"
}

Ponovno kopirati token.

2) Provjera role autorizacije

Controller: AdminController
Endpoint: GET /api/admin/secret
Authorize: Admin

Admin token → 200 OK

User token → 403 Forbidden

Time se dokazuje razlika između Admin i User role.

3) Javni endpointi (bez tokena)

Ove endpointove može koristiti svatko.

Facilities

GET /api/facilities

RoomTypes

GET /api/roomtypes

Rooms

GET /api/rooms

TimeSlots – svi

GET /api/timeslots/all

TimeSlots – dostupni

GET /api/timeslots/available?roomId=1&from=2026-02-01T00:00:00&to=2026-03-01T00:00:00

Rezultat: samo slobodni termini.

4) ADMIN FLOW – upravljanje sustavom

Sve se radi s Admin tokenom.

4A) Kreiranje sobe

Controller: RoomsController
POST /api/rooms
Authorize: Admin

{
"facilityId": 1,
"roomTypeId": 1,
"number": "101",
"floor": 1
}

Rezultat: 201 Created

4B) Kreiranje termina

Controller: TimeSlotsController
POST /api/timeslots
Authorize: Admin

{
"roomId": 1,
"start": "2026-02-10T14:00:00",
"end": "2026-02-10T16:00:00"
}

Validacije:

End mora biti veći od Start

Nema preklapanja termina

Soba mora postojati

5) USER FLOW – rezervacije

Sve se radi s User tokenom.

5A) Pregled slobodnih termina

GET /api/timeslots/available

Odabrati neki timeSlotId.

5B) Rezervacija termina

Controller: ReservationsController
POST /api/reservations
Authorize: User

{
"timeSlotId": 2
}

Rezultat:

200 OK

vraća podatke o rezervaciji

Ako je zauzet → 400 BadRequest

5C) Provjera dostupnosti

Ponovno pozvati:

GET /api/timeslots/available

Rezervirani termin više se ne prikazuje.

5D) Moje rezervacije

Controller: ReservationsController
GET /api/reservations/mine
Authorize: User

Rezultat:

lista vlastitih rezervacija

s vremenom i brojem sobe

Bez tokena → 401 Unauthorized

5E) Otkazivanje rezervacije

Controller: ReservationsController
DELETE /api/reservations/{id}

User → samo svoju

Admin → sve

Rezultat: 204 NoContent

Nakon toga slot je opet slobodan.

6) Dijagnostika

Controller: TimeSlotsController
GET /api/timeslots/diagnostics

Prikazuje podatke o bazi i konekciji.