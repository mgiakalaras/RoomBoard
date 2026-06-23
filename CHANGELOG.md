# Changelog

## v0.1.0 - Starter project

- Created ASP.NET Core Razor Pages project.
- Added public kiosk view.
- Added teacher/admin schedule view.
- Added create booking form.
- Added demo in-memory data service.
- Added room, teacher, class group, lesson period and booking models.
- Added initial dark RoomBoard visual theme.


## v0.2.0 - Basic management and multi-period bookings

### Added
- Admin screen for adding shared rooms.
- Admin screen for adding teachers.
- Admin screen for editing school lesson-period start/end times.
- Option to add extra lesson periods.
- Booking form now supports a start and end lesson period.
- Multi-period bookings for cases such as 2-hour or 3-hour simulation exams.

### Changed
- Default 7th period now ends at 14:00 in the seeded demo schedule.
- Kiosk period strip now follows the configured lesson periods instead of assuming exactly 7 periods.
- Schedule grid shows bookings that span multiple periods.

### Fixed
- Razor Greek suffix parsing issues from v0.1.0 are included in this patch.


## v0.2.1 - Deactivate rooms and teachers

### Added
- Deactivate action for rooms.
- Deactivate action for teachers.
- Confirmation prompt before deactivation.

### Changed
- Rooms and teachers now behave as soft-deleted records when removed from the active lists.
- Existing bookings remain safe because the underlying records are not hard-deleted.


## v0.3.0 - SQLite persistence and reactivation

### Added
- SQLite database using Entity Framework Core.
- Automatic database creation on first run in `App_Data/roomboard.db`.
- Database seeding for starter rooms, teachers, classes, lesson periods and demo bookings.
- Inactive rooms section in `/Admin/Rooms`.
- Inactive teachers section in `/Admin/Teachers`.
- Reactivate action for rooms.
- Reactivate action for teachers.

### Changed
- Replaced the in-memory service registration with a scoped SQLite-backed service.
- Rooms and teachers remain soft-deleted when deactivated, preserving existing booking references.
- New room/teacher creation detects inactive duplicates and suggests reactivation instead of creating duplicates.


## v0.4.0 - Excel bulk import

### Added
- Admin page `/Admin/Import` for bulk import from `.xlsx`.
- Bulk teacher import.
- Bulk room import.
- Duplicate handling during import.
- Import result summary with added, skipped and failed rows.
- No extra NuGet dependency for Excel reading; the app includes a small internal `.xlsx` reader for simple tabular imports.

### Planned next
- School unit settings.
- School logo upload.
- Application logo / branding settings.

## v0.4.1 - SimpleXlsxReader compile fix

### Fixed
- Fixed CS0019 in `SimpleXlsxReader.cs` where null-coalescing mixed `List<string>` and `string[]`.

## v0.5.0 - Timeline UI redesign

### Changed
- Reworked the application shell into a Timeline / Swimlanes / Right Drawer layout.
- Replaced the old dashboard-style daily schedule with a room timeline.
- Replaced the public kiosk card layout with a room timeline.
- Moved navigation into a right-side action drawer.
- Added a fixed left time rail.
- Updated admin pages to fit the new visual direction.

### Notes
- This patch focuses on UI/layout only.
- No database schema change is required.

## v0.6.0 - Class group management

### Added
- New admin page for class groups / school sections.
- Add class groups manually.
- Deactivate and reactivate class groups.
- Class group import from Excel.
- Navigation links for class groups in the Timeline drawer and quick actions.

### Notes
- No database schema change is required because `ClassGroup` already existed in the database model.
- Existing bookings remain safe when a class group is deactivated.

## v0.6.1 - Timeline readability tuning

### Changed
- Reduced typography size inside the timeline board.
- Darkened the timeline board and page background.
- Increased contrast for timeline lanes, period headers, room labels and booking blocks.
- Improved table readability in admin pages.

## v0.6.2 - Dynamic rail and analog clock

### Changed
- The left time rail now follows the configured school lesson periods.
- The active lesson period is calculated dynamically in the browser.
- When the current time is after the final lesson period, the rail shows "Εκτός διδακτικού ωραρίου".
- When the current time is between lesson periods, the rail shows "Διάλειμμα / μεταξύ ωρών".
- The public kiosk header clock is now analog with a small digital time below it.
- Improved kiosk header spacing so title/subtitle lines do not feel stuck together.

## v0.7.0 - Booking preview and weekly print

### Added
- Daily booking preview on the new booking page.
- Clear safety notice explaining the double-booking guard.
- Weekly printable booking schedule page.
- Print button for weekly room bookings.
- Navigation link for weekly print.

### Changed
- Conflict messages now show which room/period/teacher caused the booking conflict.

### Notes
- The server-side overlap guard already existed and remains the source of truth.
- Weekly print is optimized for A4 landscape.

## v0.7.1 - Student kiosk and compact menu

### Added
- New standalone student kiosk page at `/Kiosk/Students`.
- Student kiosk shows only class group, room, subject/purpose and teacher.
- Student kiosk auto-refreshes every 60 seconds.
- Student kiosk optionally shows a compact next-period strip.

### Changed
- Reduced clock digits in the left rail.
- Grouped the right drawer menu into compact sections so it no longer looks like a long scroll/papyrus.
- Added student kiosk links to the drawer and admin quick actions.

## v0.8.0 - School settings, branding, About and Help

### Added
- New school unit settings page at `/Admin/SchoolSettings`.
- School name, type, school year, address, phone and email fields.
- School logo upload to `wwwroot/uploads`.
- School logo/name shown in public and student kiosk views.
- New About page.
- New Help page.

### Changed
- Student kiosk title changed from a casual phrase to `Οδηγός τμημάτων`.
- Student kiosk now shows a clear current/next period badge.
- Drawer menu includes settings/support entries.

### Notes
- A small schema bootstrap creates the `SchoolSettings` table automatically if an existing SQLite database does not have it yet.

## v0.8.1 - Drawer overflow fix

### Fixed
- Fixed right action drawer overflowing outside its visual panel when the Support/About/Help group is visible.
- Added internal scrolling for the drawer.
- Compact drawer spacing for smaller screens.
- Made drawer section links slightly smaller so the menu stays grouped and tidy.

## v0.8.2 - Sticky header and responsive scaling

### Changed
- Made the public kiosk header a full-width sticky band inside the content area.
- Improved spacing so all timeline content sits clearly below the header.
- Added scaling rules for 1366x768 and similar lower-resolution displays.
- Collapsed the right drawer into grouped blocks below the page content on narrower screens.
- Reduced rail, header, clock, timeline and booking block sizing on smaller displays.

## v0.8.3 - True full-width kiosk header

### Fixed
- Reworked the public kiosk header into a true full-width fixed top band.
- The left time rail, central timeline and right drawer now sit below the header as a three-column layout.
- Removed the old Timeline UI note from the drawer.
- Reduced analog clock number size further.
- Improved 1366x768 and 1280-class kiosk scaling.

## v0.8.4 - Compact full-width kiosk header

### Fixed
- Reduced the full-width public kiosk header height.
- Reduced public kiosk title size, logo size and analog clock size.
- Prevented the header from covering the timeline content.
- Tightened kiosk layout for 1366x768 and similar displays.
- Kept the three-column kiosk layout under the header.

## v0.8.5 - Remove drawer title

### Changed
- Removed the RoomBoard / Actions / School title block from the right drawer.
- Reduced top padding in the right drawer.
- The drawer now starts directly with grouped actions, saving vertical space.

## v0.8.6 - Responsive safe layout

### Fixed
- Removed the right drawer title block directly from `_Layout.cshtml`.
- Made the public kiosk header smaller and safer.
- Added a clean responsive safe mode below 1180px width.
- Prevented the right drawer from becoming a broken floating block on narrow windows.
- Kept the three-column public kiosk layout only when there is enough horizontal space.
- Further reduced analog clock and rail sizing.

## v0.8.7 - Horizontal public header identity

### Changed
- Reworked the public kiosk header identity into a horizontal layout.
- School logo now sits on the far left with school/title text beside it.
- Reduced public kiosk title size to avoid clipping.
- Kept the analog clock unchanged.
- Adjusted header height and content offsets accordingly.

## v0.8.8 - Final public header fit

### Changed
- Slightly increased the public kiosk header height so the content no longer feels clipped.
- Kept the horizontal header layout.
- Kept the analog clock size but made its dial numbers readable again.
- Adjusted content offsets below the header.

## v0.8.9 - Compact titles across pages

### Changed
- Reduced large page titles across admin, help, about and settings pages.
- Reduced section/card title sizes for a more consistent compact layout.
- Reduced admin summary/stat typography slightly.
- Kept the public kiosk header layout unchanged.

## v0.8.10 - Admin header spacing

### Changed
- Reduced admin page title size a little more.
- Added clearer vertical spacing between eyebrow, title and description.
- Improved line-height for page descriptions so they do not look glued to the title.

## v0.8.11 - Remove drawer Timeline UI note

### Changed
- Removed the Timeline UI explanatory note from the right sidebar.
- Added CSS fallback to keep it hidden if an older layout still contains the block.

## v0.8.12 - Real lower public panels

### Changed
- Removed indicative/dummy behavior from the public lower panels.
- "Επόμενη ώρα" now uses the actual next configured lesson period and today's real bookings.
- "Διαθέσιμες τώρα" now appears only during an actual current lesson period.
- Outside teaching hours, the availability panel clearly says that availability is not shown.
- Next movement cards now also show the teacher name.

## v0.8.13 - Horizontal clock card

### Changed
- Public page clock is now a horizontal rectangular card.
- Analog clock appears on the left and digital time/date/refresh info on the right.
- Student kiosk gets the same horizontal clock pattern.
- Admin pages keep only the small rail clock and do not show this clock card.

## v0.8.14 - Analog clock hand proportions

### Fixed
- Fixed oversized analog clock hands in the horizontal clock card.
- Hands now scale proportionally to the dial size.
- Second hand no longer protrudes outside the clock.
- Improved clock dial balance on public and student kiosk pages.

## v0.8.15 - Student kiosk compact layout and info panel

### Changed
- Fixed duplicated period text such as "Επόμενη: 1η ώρα ώρα".
- Made the student kiosk layout more compact so many class movements can fit on screen.
- Reduced movement card size and typography.
- Reduced oversized empty-state presentation.
- Added a rotating offline "Σαν σήμερα / Παγκόσμια ημέρα / Μικρή σκέψη" information panel.
- The info panel is offline and stable; future versions can optionally fetch external data.

## v0.8.16 - Student kiosk carousel

### Added
- Student kiosk movement area now works as a carousel.
- Each movement is shown as one clear slide: class group -> room, with subject and teacher.
- Carousel loops automatically until the page refreshes and the current/next period changes.
- Added slide counter and dots.

### Changed
- Improved pre-first-period, between-periods and after-hours messages.
- Fixed duplicated period wording in the student kiosk badge.

## v0.8.17 - Student info panel polish

### Changed
- Redesigned the student kiosk information panel so it no longer looks like text thrown inside a large empty card.
- Added an icon, better internal spacing, tighter typography and a more intentional card surface.
- Added CSS structure that can later support manual messages, offline fallback and cached online information.

## v0.8.18 - Student visual spotlight

### Changed
- Replaced the text-only student info panel with a visual "image of the day" style spotlight card.
- Added local SVG background images so the kiosk never depends on internet access.
- Each rotating info item now has label, title, text, image and credit.
- The visual card is ready for a future manual/online/cached content provider.

## v0.9.0 - Kiosk spotlight settings

### Added
- New admin page `/Admin/KioskSpotlight`.
- Manual kiosk spotlight mode with custom label, title, text, credit and image upload.
- Uploaded spotlight image support: PNG, JPG/JPEG, WEBP and SVG up to 4MB.
- Stored kiosk spotlight settings in SQLite.
- Automatic fallback to the offline visual spotlight when manual mode is disabled.
- Navigation link for Kiosk spotlight settings.

### Notes
- This closes the student kiosk visual/info panel workflow.
- Online image-of-the-day feeds can be added later with cache/fallback, without risking blank kiosk screens.

## v1.0.0 - Release/deploy cleanup

### Added
- Dockerfile for production container build.
- docker-compose.yml configured for Portainer.
- Default external port 7010 mapped to container port 8080.
- Persistent volumes for SQLite data and uploaded images.
- .dockerignore for clean Docker builds.
- Improved .gitignore for runtime database/upload cleanup.
- App_Data and uploads placeholder folders.
- `/health` endpoint.
- Documentation:
  - docs/DEPLOY_PORTAINER.md
  - docs/RELEASE_CHECKLIST.md

### Changed
- HTTPS redirection can now be disabled in Docker using `DISABLE_HTTPS_REDIRECTION=true`.

## v1.0.1 - Booking cancellation and production seed cleanup

### Added
- Soft cancellation for bookings.
- Cancel button on the admin timeline.
- Optional cancellation reason prompt.
- SQLite auto-upgrade for existing databases:
  - `IsCancelled`
  - `CancelledAt`
  - `CancellationReason`

### Changed
- Cancelled bookings are hidden from public view, student kiosk, weekly print and room availability.
- Cancelled bookings no longer participate in room conflict checks.
- Demo bookings are no longer seeded in Production by default.
- Demo bookings can still be enabled explicitly with `SEED_DEMO_BOOKINGS=true`.

### Docs
- Added Portainer volume reset instructions for clean first deploy.

## v1.0.2 - UI polishing, favicon and kiosk display controls

### Added
- Browser favicon.
- Web manifest.
- Square abstract RoomBoard app logo SVG.
- Student kiosk display controls:
  - A-
  - Reset
  - A+
- Kiosk movement display scale is saved in browser localStorage.

### Changed
- Student kiosk movement carousel default typography is smaller.
- Kiosk movement card typography can now be adjusted without touching the database.

### Notes
- This patch does not modify SQLite schema or existing data.

## v1.0.3 - Rail logo and home return navigation

### Changed
- Replaced the left rail letter logo with the square RoomBoard app logo.
- Removed the `RoomBoard` text under the left rail clock.
- Added a prominent return link to `Προβολή καθηγητών` on non-home shared-layout pages.

### Notes
- This patch only changes UI/static assets.
- It does not modify the SQLite schema or existing data.

## v1.0.4 - Selected logo and safer booking cancellation UI

### Changed
- Applied selected logo concept #3 to the app icon and browser favicon.
- Improved readability of booking blocks in the admin timeline.
- Booking blocks now show class group, subject, teacher and period more clearly.
- Cancellation confirmation now displays the full booking summary before proceeding.

### Notes
- This patch only changes UI/static assets.
- It does not modify the SQLite schema or existing data.

## v1.0.5 - Admin booking readability and logo clipping fix

### Fixed
- Fixed selected app logo being clipped in the narrow left rail.
- Fixed admin timeline booking text being clipped/unreadable.
- Improved one-period booking layout.

### Changed
- Admin timeline rows are slightly taller so real Greek booking text can fit.
- Booking cancellation still shows full booking summary before cancelling.

### Notes
- This patch only changes UI/static assets.
- It does not modify the SQLite schema or existing data.

## v1.0.6 - Actual admin booking readability fix

### Fixed
- Fixed admin timeline readability rules not applying because the real booking blocks were still plain `.timeline-booking`.
- Added structured booking markup with class group, subject, teacher and period.
- Added direct `.admin-board .timeline-booking` CSS fallback so the issue cannot recur.
- Replaced app icon/favicon with a larger RB monogram for better rail display.

### Notes
- This patch only changes UI/static assets.
- It does not modify the SQLite schema or existing data.

## v1.0.7 - One-period booking layout and reliable cancel confirmation

### Fixed
- Fixed one-period booking cards being too cramped in the admin timeline.
- Fixed cancellation confirmation not appearing reliably.
- Moved cancellation confirmation to a direct page-level submit listener.

### Changed
- One-period bookings now prioritize class group and subject.
- One-period cancel action is compact so it does not consume the text area.

### Notes
- This patch only changes UI/static behavior.
- It does not modify the SQLite schema or existing data.

## v1.0.8 - Icon-only cancel action

### Changed
- Replaced the text `Ακύρωση` button in admin timeline bookings with a compact `×` icon button.
- Kept tooltip and aria-label for clarity/accessibility.
- Full cancellation confirmation remains active.

### Notes
- This patch only changes UI.
- It does not modify the SQLite schema or existing data.

## v1.0.9 - Server-side cancellation confirmation page

### Added
- New `/Admin/CancelBooking` confirmation page.
- Full booking details before final cancellation.
- Optional cancellation reason on the confirmation page.

### Changed
- Admin timeline cancel action now opens the confirmation page instead of relying on browser JavaScript confirm.
- Cancel icon remains compact.

### Notes
- This patch changes UI/service methods only.
- It does not modify the SQLite schema or existing data.

## v1.0.10 - Build fix for server-side cancellation page

### Fixed
- Fixed compile error in `RoomBoardDbService.GetBookingById()`.
- `Booking` does not have navigation properties such as `Room`; the method now loads the booking by ID and uses the existing `ToDetails(...)` mapping.

### Notes
- This patch only fixes build/runtime code.
- It does not modify the SQLite schema or existing data.

## v1.0.11 - Build fix for ToDetails lookup arguments

### Fixed
- Fixed compile error CS7036 in `RoomBoardDbService.GetBookingById()`.
- `ToDetails(...)` now receives the required rooms, teachers, class groups and lesson periods dictionaries.

### Notes
- This patch only fixes build code.
- It does not modify the SQLite schema or existing data.

## v1.0.12 - Graphic cancel icon

### Changed
- Replaced the font-rendered `×` cancel icon with a static CSS-drawn cross.
- The cancel icon is now visually centered and stable in small booking cards.

### Notes
- This patch only changes UI.
- It does not modify the SQLite schema or existing data.

## v1.0.13 - Prints center and daily print

### Added
- New `/Admin/Prints` page as a central print menu.
- New `/Admin/DailyPrint` page for daily room-booking printouts.
- Daily print date selector and print button.

### Changed
- Main menu link changed from `Εβδομαδιαία εκτύπωση` to `Εκτυπώσεις`.
- Weekly print now links back to the prints center and includes a daily print shortcut.

### Notes
- This patch only changes UI/pages.
- It does not modify the SQLite schema or existing data.

## v1.0.14 - Timeline print layout with school header and app footer

### Changed
- Daily print now uses a timeline board similar to the admin schedule.
- Weekly print now uses one timeline board per day.
- Print headers include school unit details from School Settings.
- Print footers include the RoomBoard application logo.

### Notes
- This patch only changes print UI/pages.
- It does not modify the SQLite schema or existing data.
