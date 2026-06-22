# RoomBoard Roadmap

## Phase 1 - Starter baseline

- Public kiosk view.
- Teacher/admin day schedule.
- Demo bookings.
- Create booking form.
- Conflict check for same room / same date / same lesson period.

## Phase 2 - Database

- Add SQLite database.
- Add EF Core migrations.
- Store rooms, teachers, class groups, periods and bookings persistently.
- Seed default school periods.

## Phase 3 - Management screens

- CRUD for rooms.
- CRUD for teachers.
- CRUD for class groups.
- CRUD for lesson periods.
- Edit/delete bookings.

## Phase 4 - Users and roles

- Admin.
- Teacher.
- View-only.
- Teachers can manage their own bookings.
- Admin can manage everything.

## Phase 5 - Display polish

- Public/private display modes.
- School name/logo settings.
- Optional announcement strip.
- Fullscreen kiosk layout.

## Intentionally out of scope for now

- QR codes per room.
- Tablets outside rooms.
- Minute countdowns.


## Next planned step: School identity and branding

- School unit details:
  - School name
  - School type
  - Address / contact details
  - Active school year
- School logo upload
- Application logo / RoomBoard branding
- Display these settings in kiosk header, admin header, future print/PDF reports

## UI direction selected

Selected direction: Timeline / Swimlanes / Right Drawer.

Future UI refinements:
- Dynamic left rail based on configured lesson periods.
- Highlight the real current lesson period in the rail.
- Add school branding and school logo to the timeline header.
- Add theme switcher after the base layout stabilizes.

## Class groups

Class groups are now a core reference-data entity:
- Manual add
- Soft deactivate/reactivate
- Excel import

Future:
- Optional class group display order
- Optional grade/category grouping: Α Λυκείου, Β Λυκείου, Γ Λυκείου

## Booking workflow improvements

Completed:
- Daily booking preview before creating a booking.
- Weekly printable schedule.

Future:
- Booking edit/cancel workflow.
- Optional booking approval flow.
- Export weekly schedule to PDF after print styling stabilizes.

## Kiosk separation

Completed:
- Separate student kiosk page.
- Teacher/admin views remain timeline-based and management-oriented.
- Student kiosk shows only immediate student-facing movement information.

Future:
- Optional theme for student kiosk.
- Optional school logo / school name from School Settings.
- Optional filtering by current period or next period.

## School identity and documentation

Completed:
- School settings page.
- School logo upload.
- School logo/name in kiosk views.
- About page.
- Help page.

Future:
- Use school logo in printable weekly reports.
- Add application logo customization.
- Add exportable user manual.
