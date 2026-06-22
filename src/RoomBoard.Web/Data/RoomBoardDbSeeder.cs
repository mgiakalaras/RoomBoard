using Microsoft.EntityFrameworkCore;
using RoomBoard.Web.Models;

namespace RoomBoard.Web.Data;

public static class RoomBoardDbSeeder
{
    public static void Seed(RoomBoardDbContext db)
    {
        EnsureSchoolSettingsTable(db);
        EnsureKioskSpotlightSettingsTable(db);

        if (!db.SchoolSettings.Any())
        {
            db.SchoolSettings.Add(new SchoolSettings
            {
                Id = 1,
                SchoolName = "Πυθαγόρειο ΓΕΛ Σάμου",
                SchoolType = "Γενικό Λύκειο",
                SchoolYear = "2025-2026",
                UpdatedAt = DateTime.Now
            });
            db.SaveChanges();
        }

        if (!db.KioskSpotlightSettings.Any())
        {
            db.KioskSpotlightSettings.Add(new KioskSpotlightSettings
            {
                Id = 1,
                IsManualEnabled = false,
                Label = "Image of the day",
                Title = "Μια εικόνα, μια σκέψη",
                Text = "Κάθε μέρα μπορεί να ξεκινά με μια μικρή αφορμή για παρατήρηση.",
                ImagePath = "/img/kiosk/spotlight-cosmos.svg",
                Credit = "RoomBoard visual spotlight",
                UpdatedAt = DateTime.Now
            });
            db.SaveChanges();
        }

        if (!db.Rooms.Any())
        {
            db.Rooms.AddRange(
                new Room { Id = 1, Name = "Αίθουσα Προβολών", Location = "Κεντρικό κτήριο · 1ος όροφος", DisplayOrder = 1, IsActive = true },
                new Room { Id = 2, Name = "Εργαστήριο Πληροφορικής 1", Location = "Ισόγειο · Η/Υ", DisplayOrder = 2, IsActive = true },
                new Room { Id = 3, Name = "Βιβλιοθήκη", Location = "Κεντρικό κτήριο · Ισόγειο", DisplayOrder = 3, IsActive = true },
                new Room { Id = 4, Name = "Αίθουσα Πολλαπλών Χρήσεων", Location = "Νέο κτήριο · Ισόγειο", DisplayOrder = 4, IsActive = true }
            );
        }

        if (!db.Teachers.Any())
        {
            db.Teachers.AddRange(
                new Teacher { Id = 1, FullName = "Γιακαλάρας Μάριος", Specialty = "ΠΕ86", IsActive = true },
                new Teacher { Id = 2, FullName = "Παπαδοπούλου Μαρία", Specialty = "ΠΕ02", IsActive = true },
                new Teacher { Id = 3, FullName = "Βασιλείου Νίκος", Specialty = "ΠΕ02", IsActive = true },
                new Teacher { Id = 4, FullName = "Κωνσταντίνου Ελένη", Specialty = "ΠΕ04", IsActive = true }
            );
        }

        if (!db.ClassGroups.Any())
        {
            db.ClassGroups.AddRange(
                new ClassGroup { Id = 1, Name = "Α1", IsActive = true },
                new ClassGroup { Id = 2, Name = "Α2", IsActive = true },
                new ClassGroup { Id = 3, Name = "Β1", IsActive = true },
                new ClassGroup { Id = 4, Name = "Β2", IsActive = true },
                new ClassGroup { Id = 5, Name = "Γ Οικ.-Πληρ.", IsActive = true }
            );
        }

        if (!db.LessonPeriods.Any())
        {
            db.LessonPeriods.AddRange(
                new LessonPeriod { Id = 1, Number = 1, StartTime = new TimeOnly(8, 15), EndTime = new TimeOnly(9, 0) },
                new LessonPeriod { Id = 2, Number = 2, StartTime = new TimeOnly(9, 5), EndTime = new TimeOnly(9, 50) },
                new LessonPeriod { Id = 3, Number = 3, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(10, 45) },
                new LessonPeriod { Id = 4, Number = 4, StartTime = new TimeOnly(10, 55), EndTime = new TimeOnly(11, 40) },
                new LessonPeriod { Id = 5, Number = 5, StartTime = new TimeOnly(11, 50), EndTime = new TimeOnly(12, 35) },
                new LessonPeriod { Id = 6, Number = 6, StartTime = new TimeOnly(12, 40), EndTime = new TimeOnly(13, 25) },
                new LessonPeriod { Id = 7, Number = 7, StartTime = new TimeOnly(13, 30), EndTime = new TimeOnly(14, 0) }
            );
        }

        db.SaveChanges();

        if (!db.Bookings.Any())
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            db.Bookings.AddRange(
                CreateBooking(today, 2, 1, 1, 1, 2, "Πληροφορική"),
                CreateBooking(today, 2, 2, 2, 1, 3, "Εργασία"),
                CreateBooking(today, 1, 3, 4, 2, 4, "Ιστορία"),
                CreateBooking(today, 2, 4, 4, 1, 5, "Πληροφορική"),
                CreateBooking(today, 3, 4, 5, 3, 1, "Φιλαναγνωσία")
            );
            db.SaveChanges();
        }
    }

    private static void EnsureSchoolSettingsTable(RoomBoardDbContext db)
    {
        db.Database.ExecuteSqlRaw(@"
CREATE TABLE IF NOT EXISTS SchoolSettings (
    Id INTEGER NOT NULL CONSTRAINT PK_SchoolSettings PRIMARY KEY,
    SchoolName TEXT NOT NULL,
    SchoolType TEXT NOT NULL,
    SchoolYear TEXT NOT NULL,
    Address TEXT NULL,
    Phone TEXT NULL,
    Email TEXT NULL,
    LogoPath TEXT NULL,
    UpdatedAt TEXT NOT NULL
);");
    }


    private static void EnsureKioskSpotlightSettingsTable(RoomBoardDbContext db)
    {
        db.Database.ExecuteSqlRaw(@"
CREATE TABLE IF NOT EXISTS KioskSpotlightSettings (
    Id INTEGER NOT NULL CONSTRAINT PK_KioskSpotlightSettings PRIMARY KEY,
    IsManualEnabled INTEGER NOT NULL,
    Label TEXT NOT NULL,
    Title TEXT NOT NULL,
    Text TEXT NOT NULL,
    ImagePath TEXT NULL,
    Credit TEXT NULL,
    UpdatedAt TEXT NOT NULL
);");
    }

    private static Booking CreateBooking(DateOnly date, int roomId, int startPeriodId, int endPeriodId, int teacherId, int classGroupId, string subject)
    {
        return new Booking
        {
            Date = date,
            RoomId = roomId,
            StartLessonPeriodId = startPeriodId,
            EndLessonPeriodId = endPeriodId,
            TeacherId = teacherId,
            ClassGroupId = classGroupId,
            SubjectOrPurpose = subject,
            CreatedAt = DateTime.Now
        };
    }
}
