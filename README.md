# RoomBoard

**RoomBoard** is a small ASP.NET Core Razor Pages web app for managing and displaying shared school room bookings.

The first version is intentionally simple:

- Public kiosk view for entrance / teachers' office.
- Teacher/admin day schedule view.
- Create booking form.
- Greek UI.
- School lesson periods instead of arbitrary clock countdowns.
- Demo in-memory data so the project opens and runs immediately.

## Requirements

- Visual Studio 2022
- .NET 8 SDK

## How to run

1. Open `RoomBoard.sln` in Visual Studio.
2. Set `RoomBoard.Web` as startup project if needed.
3. Run with the `https` or `http` profile.
4. Open:
   - `/` for public kiosk view
   - `/Admin` for the teacher/admin schedule
   - `/Admin/Create` for a new booking

## Current architecture

```text
src/RoomBoard.Web
├── Models
├── Services
├── Pages
│   ├── Index.cshtml                 Public kiosk view
│   └── Admin
│       ├── Index.cshtml             Day schedule
│       ├── Create.cshtml            New booking form
│       └── Rooms.cshtml             Demo room list
└── wwwroot/css/roomboard.css        Main theme
```

## Important design decision

The app is based on **school lesson periods**: 1η, 2η, 3η ώρα etc.

The following were intentionally left out of the initial scope:

- QR codes per room.
- Tablet mode outside each room.
- Countdown such as "room frees in 12 minutes".

These are not needed for the current school use case.

## Next planned steps

1. Add real database with SQLite + Entity Framework Core.
2. Add proper CRUD for rooms, teachers, class groups and lesson periods.
3. Add login roles: Admin, Teacher, View-only.
4. Add public/private display settings.
5. Add import/export later if useful.


## v0.2.0 notes

This version adds the first real management screens while still using the in-memory demo service:

- Add shared rooms from the admin area.
- Add teachers from the admin area.
- Edit school lesson-period times, including schools that finish at 14:00 instead of 14:10.
- Add extra lesson periods if a school needs more than seven periods.
- Create bookings that span multiple consecutive lesson periods, for example 1st–3rd period for a simulation exam.
- Conflict detection now checks overlapping period ranges, not just a single period.

Data is still in memory and will reset when the application restarts. The next major technical step is SQLite / Entity Framework persistence.


## v0.2.1 notes

Rooms and teachers can now be deactivated from the admin screens. This behaves like a soft delete:

- The item disappears from active admin lists and new booking dropdowns.
- Existing/historical bookings are not broken.
- This is suitable for teachers leaving the school or rooms going out of service.

Hard deletion is intentionally avoided at this stage so that existing booking references remain safe.


## v0.3.0 notes

This version switches RoomBoard from in-memory demo data to a real SQLite database.

- The database is created automatically on first run at `src/RoomBoard.Web/App_Data/roomboard.db` when running from Visual Studio.
- Rooms, teachers, lesson periods and bookings now persist after application restart.
- Inactive rooms and inactive teachers are visible in their admin pages.
- Inactive rooms/teachers can be reactivated.
- Existing bookings remain safe because deactivation remains a soft-delete operation.

On first run, Visual Studio will restore the EF Core SQLite NuGet packages. No manual migration command is required in this version because the app uses `Database.EnsureCreated()` for the starter database.


## v0.4.0 notes

This version adds bulk import from `.xlsx` files for teachers and rooms.

Supported teacher columns:
- `FullName` or `Ονοματεπώνυμο` (required)
- `Specialty` or `Ειδικότητα` (optional)

Supported room columns:
- `Name` or `Αίθουσα` (required)
- `Location` or `Τοποθεσία` (optional)

The importer reads a sheet named `Teachers` / `Καθηγητές` or `Rooms` / `Αίθουσες` when present. If the expected sheet is not found, it reads the first sheet. Existing active records are skipped, and inactive duplicates are also skipped so they can be reactivated manually from the admin screen.

## v0.5.0 notes

This patch changes the UI direction to the selected Timeline / Swimlanes / Right Drawer concept.

Main UI changes:
- Left fixed time rail instead of the old classic header/sidebar pattern.
- Right action drawer for navigation.
- Public kiosk becomes a timeline board.
- Admin daily schedule becomes a timeline board.
- Bookings appear as blocks spanning one or more lesson periods.
- Admin subpages are adapted to the new shell, without the old left sidebar.

This is a visual/layout patch. It does not change the database schema.

## v0.6.0 notes

This patch adds first-class class group management.

Included:
- New `/Admin/ClassGroups` page.
- Add class groups manually.
- Deactivate/reactivate class groups.
- Class groups remain safe for historical bookings through soft-delete behavior.
- Excel import now supports class groups.

Supported class group import columns:
- `Name` or `Τμήμα`

## v0.6.2 notes

This patch improves the Timeline UI behavior:
- The left rail now uses the configured lesson periods instead of static hard-coded hours.
- The active period updates live.
- Outside school teaching hours are clearly shown in the rail.
- The public kiosk clock is now analog.
- Header spacing was adjusted for better readability.

## v0.7.0 notes

This patch adds booking preview and weekly print.

Included:
- Daily preview on `/Admin/Create`.
- Server-side room/date/period overlap guard remains active.
- More informative conflict messages.
- Weekly print page at `/Admin/WeeklyPrint`.
- A4 landscape print styling for the weekly schedule.

## v0.7.1 notes

This patch separates teacher/admin views from the student-facing kiosk.

New page:
- `/Kiosk/Students`

Purpose:
- Shows only the information students need:
  - Class group
  - Room
  - Subject/purpose
  - Teacher
- Avoids admin/teacher information such as free rooms, management timeline, imports, settings or weekly print.

## v0.8.0 notes

This patch adds school branding and basic documentation pages.

Included:
- `/Admin/SchoolSettings` for school unit details.
- School logo upload.
- `/About` page.
- `/Help` page.
- Student kiosk refinement with a clearer title and current/next period badge.

Logo uploads are stored under:

```text
src/RoomBoard.Web/wwwroot/uploads/
```

The app automatically creates the `SchoolSettings` table for existing SQLite databases.

## v0.8.12 notes

The public view lower panels now avoid indicative/dummy output:
- Next period movements are calculated from the actual next configured lesson period.
- Available rooms are shown only during a real current teaching period.
- Outside teaching hours, the panels display clear status messages instead of fake availability.

## v0.8.13 notes

Clock card refinement:
- Public/home and student kiosk clock cards now use a horizontal rectangular layout.
- Analog clock is placed on the left.
- Digital time, date and auto-refresh hint are placed on the right.
- Admin pages are not affected.

## v0.8.15 notes

Student kiosk refinements:
- Kiosk movement cards are smaller and more scalable.
- Empty state no longer consumes the entire screen.
- Added an offline rotating information panel for "Σαν σήμερα", global-day-style notes and useful school/learning thoughts.
- This avoids relying on external internet services for the kiosk display.

## v0.8.16 notes

Student kiosk carousel:
- Shows one class movement at a time.
- Loops through movements every 8 seconds.
- Auto-refresh remains every 60 seconds so the displayed period changes automatically.
- Before the first lesson, between lessons, and after school hours, the kiosk shows clear messages.

## v0.8.17 notes

Student info panel polish:
- The kiosk information panel is now visually integrated.
- The design is ready for a future content strategy:
  - manual school messages,
  - offline fallback facts,
  - optional cached online "Σαν σήμερα" / global days provider.

## v0.8.18 notes

Student kiosk visual spotlight:
- The old text-only information panel is replaced by an image-style spotlight card.
- Images are local SVG assets under `wwwroot/img/kiosk/`, so the kiosk remains reliable offline.
- Future content strategy can add:
  - manual school messages,
  - uploaded custom images,
  - cached online image-of-the-day/global-day feeds.

## v0.9.0 notes

Kiosk spotlight can now be managed from `/Admin/KioskSpotlight`.

Supported:
- Enable/disable manual spotlight.
- Set label, title, text and credit.
- Upload custom image.
- Fall back to local offline spotlight content when manual mode is off.

This is the recommended stable approach before adding any optional online image-of-the-day provider.

## v1.0.0 deployment notes

RoomBoard now includes Docker/Portainer deployment files.

Default deployment port:

```text
http://SERVER_IP:7010
```

Docker mapping:

```text
7010:8080
```

Included deployment files:

```text
Dockerfile
docker-compose.yml
.dockerignore
docs/DEPLOY_PORTAINER.md
docs/RELEASE_CHECKLIST.md
```

Persistent Docker volumes:

```text
roomboard_data
roomboard_uploads
```

These preserve the SQLite database and uploaded school/kiosk images.

Health endpoint:

```text
/health
```
