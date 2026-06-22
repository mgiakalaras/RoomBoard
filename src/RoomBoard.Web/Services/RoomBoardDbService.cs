using Microsoft.EntityFrameworkCore;
using RoomBoard.Web.Data;
using RoomBoard.Web.Models;

namespace RoomBoard.Web.Services;

public sealed class RoomBoardDbService : IRoomBoardService
{
    private readonly RoomBoardDbContext _db;

    public RoomBoardDbService(RoomBoardDbContext db)
    {
        _db = db;
    }


    public SchoolSettings GetSchoolSettings()
    {
        var settings = _db.SchoolSettings.AsNoTracking().FirstOrDefault(s => s.Id == 1);
        return settings ?? new SchoolSettings
        {
            Id = 1,
            SchoolName = "Πυθαγόρειο ΓΕΛ Σάμου",
            SchoolType = "Γενικό Λύκειο",
            SchoolYear = "2025-2026"
        };
    }

    public (bool Success, string Message) SaveSchoolSettings(SchoolSettingsInput input, string? logoPath)
    {
        var schoolName = input.SchoolName.Trim();
        if (string.IsNullOrWhiteSpace(schoolName))
        {
            return (false, "Συμπληρώστε όνομα σχολικής μονάδας.");
        }

        var settings = _db.SchoolSettings.FirstOrDefault(s => s.Id == 1);
        if (settings is null)
        {
            settings = new SchoolSettings { Id = 1 };
            _db.SchoolSettings.Add(settings);
        }

        settings.SchoolName = schoolName;
        settings.SchoolType = string.IsNullOrWhiteSpace(input.SchoolType) ? "Σχολική μονάδα" : input.SchoolType.Trim();
        settings.SchoolYear = string.IsNullOrWhiteSpace(input.SchoolYear) ? DateTime.Today.Year + "-" + (DateTime.Today.Year + 1) : input.SchoolYear.Trim();
        settings.Address = string.IsNullOrWhiteSpace(input.Address) ? null : input.Address.Trim();
        settings.Phone = string.IsNullOrWhiteSpace(input.Phone) ? null : input.Phone.Trim();
        settings.Email = string.IsNullOrWhiteSpace(input.Email) ? null : input.Email.Trim();

        if (!string.IsNullOrWhiteSpace(logoPath))
        {
            settings.LogoPath = logoPath;
        }

        settings.UpdatedAt = DateTime.Now;
        _db.SaveChanges();
        return (true, "Οι ρυθμίσεις σχολικής μονάδας αποθηκεύτηκαν.");
    }


    public KioskSpotlightSettings GetKioskSpotlightSettings()
    {
        var settings = _db.KioskSpotlightSettings.AsNoTracking().FirstOrDefault(s => s.Id == 1);
        return settings ?? new KioskSpotlightSettings
        {
            Id = 1,
            IsManualEnabled = false,
            Label = "Image of the day",
            Title = "Μια εικόνα, μια σκέψη",
            Text = "Κάθε μέρα μπορεί να ξεκινά με μια μικρή αφορμή για παρατήρηση.",
            ImagePath = "/img/kiosk/spotlight-cosmos.svg",
            Credit = "RoomBoard visual spotlight"
        };
    }

    public (bool Success, string Message) SaveKioskSpotlightSettings(KioskSpotlightInput input, string? imagePath)
    {
        if (input.IsManualEnabled)
        {
            if (string.IsNullOrWhiteSpace(input.Title))
            {
                return (false, "Συμπληρώστε τίτλο για το χειροκίνητο spotlight.");
            }

            if (string.IsNullOrWhiteSpace(input.Text))
            {
                return (false, "Συμπληρώστε σύντομο κείμενο για το χειροκίνητο spotlight.");
            }
        }

        var settings = _db.KioskSpotlightSettings.FirstOrDefault(s => s.Id == 1);
        if (settings is null)
        {
            settings = new KioskSpotlightSettings { Id = 1 };
            _db.KioskSpotlightSettings.Add(settings);
        }

        settings.IsManualEnabled = input.IsManualEnabled;
        settings.Label = string.IsNullOrWhiteSpace(input.Label) ? "Image of the day" : input.Label.Trim();
        settings.Title = string.IsNullOrWhiteSpace(input.Title) ? "Μια εικόνα, μια σκέψη" : input.Title.Trim();
        settings.Text = string.IsNullOrWhiteSpace(input.Text) ? "Κάθε μέρα μπορεί να ξεκινά με μια μικρή αφορμή για παρατήρηση." : input.Text.Trim();
        settings.Credit = string.IsNullOrWhiteSpace(input.Credit) ? null : input.Credit.Trim();

        if (!string.IsNullOrWhiteSpace(imagePath))
        {
            settings.ImagePath = imagePath;
        }

        if (string.IsNullOrWhiteSpace(settings.ImagePath))
        {
            settings.ImagePath = "/img/kiosk/spotlight-cosmos.svg";
        }

        settings.UpdatedAt = DateTime.Now;
        _db.SaveChanges();

        return (true, input.IsManualEnabled
            ? "Το χειροκίνητο kiosk spotlight ενεργοποιήθηκε και αποθηκεύτηκε."
            : "Το χειροκίνητο kiosk spotlight απενεργοποιήθηκε. Θα εμφανίζεται το offline fallback.");
    }

    public IReadOnlyList<Room> GetRooms()
    {
        return _db.Rooms.AsNoTracking()
            .Where(r => r.IsActive)
            .OrderBy(r => r.DisplayOrder)
            .ThenBy(r => r.Name)
            .ToList();
    }

    public IReadOnlyList<Room> GetInactiveRooms()
    {
        return _db.Rooms.AsNoTracking()
            .Where(r => !r.IsActive)
            .OrderBy(r => r.DisplayOrder)
            .ThenBy(r => r.Name)
            .ToList();
    }

    public IReadOnlyList<Teacher> GetTeachers()
    {
        return _db.Teachers.AsNoTracking()
            .Where(t => t.IsActive)
            .OrderBy(t => t.FullName)
            .ToList();
    }

    public IReadOnlyList<Teacher> GetInactiveTeachers()
    {
        return _db.Teachers.AsNoTracking()
            .Where(t => !t.IsActive)
            .OrderBy(t => t.FullName)
            .ToList();
    }

    public IReadOnlyList<ClassGroup> GetClassGroups()
    {
        return _db.ClassGroups.AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToList();
    }

    public IReadOnlyList<ClassGroup> GetInactiveClassGroups()
    {
        return _db.ClassGroups.AsNoTracking()
            .Where(c => !c.IsActive)
            .OrderBy(c => c.Name)
            .ToList();
    }

    public IReadOnlyList<LessonPeriod> GetLessonPeriods()
    {
        return _db.LessonPeriods.AsNoTracking()
            .OrderBy(p => p.Number)
            .ToList();
    }

    public LessonPeriod? GetCurrentPeriod(DateTime now)
    {
        var currentTime = TimeOnly.FromDateTime(now);
        return _db.LessonPeriods.AsNoTracking()
            .OrderBy(p => p.Number)
            .FirstOrDefault(p => currentTime >= p.StartTime && currentTime <= p.EndTime);
    }

    public IReadOnlyList<RoomStatus> GetRoomStatuses(DateOnly date, int periodNumber)
    {
        var allPeriods = GetLessonPeriods();
        if (allPeriods.Count == 0)
        {
            return Array.Empty<RoomStatus>();
        }

        var currentPeriod = allPeriods.FirstOrDefault(p => p.Number == periodNumber) ?? allPeriods.First();
        var allBookings = GetBookingsForDate(date);

        return GetRooms().Select(room =>
        {
            var roomBookings = allBookings
                .Where(b => b.Room.Id == room.Id)
                .OrderBy(b => b.StartPeriod.Number)
                .ToList();

            return new RoomStatus
            {
                Room = room,
                CurrentBooking = roomBookings.FirstOrDefault(b => b.CoversPeriodNumber(currentPeriod.Number)),
                NextBooking = roomBookings.FirstOrDefault(b => b.StartPeriod.Number > currentPeriod.Number),
                BusyPeriodNumbers = roomBookings
                    .SelectMany(b => b.CoveredPeriodNumbers())
                    .Distinct()
                    .OrderBy(n => n)
                    .ToList()
            };
        }).ToList();
    }

    public IReadOnlyList<BookingDetails> GetBookingsForDate(DateOnly date)
    {
        var rooms = _db.Rooms.AsNoTracking().ToDictionary(r => r.Id);
        var teachers = _db.Teachers.AsNoTracking().ToDictionary(t => t.Id);
        var classGroups = _db.ClassGroups.AsNoTracking().ToDictionary(c => c.Id);
        var periods = _db.LessonPeriods.AsNoTracking().ToDictionary(p => p.Id);

        return _db.Bookings.AsNoTracking()
            .Where(b => b.Date == date)
            .AsEnumerable()
            .Where(b => rooms.ContainsKey(b.RoomId)
                        && teachers.ContainsKey(b.TeacherId)
                        && periods.ContainsKey(b.StartLessonPeriodId)
                        && periods.ContainsKey(b.EndLessonPeriodId))
            .Select(b => ToDetails(b, rooms, teachers, classGroups, periods))
            .OrderBy(b => b.StartPeriod.Number)
            .ThenBy(b => b.Room.DisplayOrder)
            .ToList();
    }

    public IReadOnlyList<BookingDetails> GetNextPeriodClassMovements(DateOnly date, int periodNumber)
    {
        var nextPeriod = GetLessonPeriods().FirstOrDefault(p => p.Number == periodNumber + 1);
        if (nextPeriod is null)
        {
            return Array.Empty<BookingDetails>();
        }

        return GetBookingsForDate(date)
            .Where(b => b.CoversPeriodNumber(nextPeriod.Number) && b.ClassGroup is not null)
            .OrderBy(b => b.ClassGroup!.Name)
            .ThenBy(b => b.Room.DisplayOrder)
            .ToList();
    }

    public (bool Success, string Message) CreateBooking(CreateBookingInput input)
    {
        var room = _db.Rooms.FirstOrDefault(r => r.Id == input.RoomId && r.IsActive);
        var startPeriod = _db.LessonPeriods.FirstOrDefault(p => p.Id == input.StartLessonPeriodId);
        var endPeriod = _db.LessonPeriods.FirstOrDefault(p => p.Id == input.EndLessonPeriodId);
        var teacher = _db.Teachers.FirstOrDefault(t => t.Id == input.TeacherId && t.IsActive);

        if (room is null || startPeriod is null || endPeriod is null || teacher is null)
        {
            return (false, "Η κράτηση δεν αποθηκεύτηκε. Ελέγξτε αίθουσα, ώρες και καθηγητή.");
        }

        if (endPeriod.Number < startPeriod.Number)
        {
            return (false, "Η ώρα λήξης πρέπει να είναι ίδια ή μεταγενέστερη από την ώρα έναρξης.");
        }

        if (input.ClassGroupId is not null && input.ClassGroupId != 0 && !_db.ClassGroups.Any(c => c.Id == input.ClassGroupId.Value && c.IsActive))
        {
            return (false, "Το τμήμα που επιλέχθηκε δεν υπάρχει.");
        }

        var periodById = _db.LessonPeriods.AsNoTracking().ToDictionary(p => p.Id);
        var existingBookings = _db.Bookings.AsNoTracking()
            .Where(b => b.Date == input.Date && b.RoomId == input.RoomId)
            .ToList();

        var conflictBooking = existingBookings.FirstOrDefault(b =>
        {
            if (!periodById.TryGetValue(b.StartLessonPeriodId, out var existingStart) ||
                !periodById.TryGetValue(b.EndLessonPeriodId, out var existingEnd))
            {
                return false;
            }

            return RangesOverlap(startPeriod.Number, endPeriod.Number, existingStart.Number, existingEnd.Number);
        });

        if (conflictBooking is not null)
        {
            var existingStart = periodById[conflictBooking.StartLessonPeriodId];
            var existingEnd = periodById[conflictBooking.EndLessonPeriodId];
            var existingTeacher = _db.Teachers.AsNoTracking()
                .FirstOrDefault(t => t.Id == conflictBooking.TeacherId)?.FullName ?? "άλλον/η καθηγητή/τρια";

            return (false, $"Σύγκρουση κράτησης: η αίθουσα {room.Name} είναι ήδη κλεισμένη στις {existingStart.Number}η - {existingEnd.Number}η ώρα από {existingTeacher}.");
        }

        _db.Bookings.Add(new Booking
        {
            Date = input.Date,
            RoomId = input.RoomId,
            StartLessonPeriodId = startPeriod.Id,
            EndLessonPeriodId = endPeriod.Id,
            TeacherId = input.TeacherId,
            ClassGroupId = input.ClassGroupId == 0 ? null : input.ClassGroupId,
            SubjectOrPurpose = input.SubjectOrPurpose.Trim(),
            Notes = string.IsNullOrWhiteSpace(input.Notes) ? null : input.Notes.Trim(),
            CreatedAt = DateTime.Now
        });

        _db.SaveChanges();
        return (true, "Η κράτηση αποθηκεύτηκε.");
    }

    public (bool Success, string Message) AddRoom(AddRoomInput input)
    {
        var name = input.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return (false, "Συμπληρώστε όνομα αίθουσας.");
        }

        var existingRoom = _db.Rooms.AsEnumerable()
            .FirstOrDefault(r => string.Equals(r.Name.Trim(), name, StringComparison.CurrentCultureIgnoreCase));

        if (existingRoom is not null)
        {
            return existingRoom.IsActive
                ? (false, "Υπάρχει ήδη ενεργή αίθουσα με αυτό το όνομα.")
                : (false, "Υπάρχει ήδη απενεργοποιημένη αίθουσα με αυτό το όνομα. Κάντε επανενεργοποίηση αντί για νέα προσθήκη.");
        }

        var nextOrder = _db.Rooms.Any() ? _db.Rooms.Max(r => r.DisplayOrder) + 1 : 1;

        _db.Rooms.Add(new Room
        {
            Name = name,
            Location = string.IsNullOrWhiteSpace(input.Location) ? "Χωρίς τοποθεσία" : input.Location.Trim(),
            DisplayOrder = nextOrder,
            IsActive = true
        });

        _db.SaveChanges();
        return (true, "Η αίθουσα προστέθηκε.");
    }

    public (bool Success, string Message) DeactivateRoom(int roomId)
    {
        var room = _db.Rooms.FirstOrDefault(r => r.Id == roomId);
        if (room is null)
        {
            return (false, "Η αίθουσα δεν βρέθηκε.");
        }

        if (!room.IsActive)
        {
            return (false, "Η αίθουσα είναι ήδη απενεργοποιημένη.");
        }

        room.IsActive = false;
        _db.SaveChanges();
        return (true, "Η αίθουσα απενεργοποιήθηκε και δεν θα εμφανίζεται πλέον στις νέες κρατήσεις.");
    }

    public (bool Success, string Message) ReactivateRoom(int roomId)
    {
        var room = _db.Rooms.FirstOrDefault(r => r.Id == roomId);
        if (room is null)
        {
            return (false, "Η αίθουσα δεν βρέθηκε.");
        }

        if (room.IsActive)
        {
            return (false, "Η αίθουσα είναι ήδη ενεργή.");
        }

        room.IsActive = true;
        _db.SaveChanges();
        return (true, "Η αίθουσα επανενεργοποιήθηκε.");
    }

    public (bool Success, string Message) AddTeacher(AddTeacherInput input)
    {
        var fullName = input.FullName.Trim();
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return (false, "Συμπληρώστε ονοματεπώνυμο καθηγητή/τριας.");
        }

        var existingTeacher = _db.Teachers.AsEnumerable()
            .FirstOrDefault(t => string.Equals(t.FullName.Trim(), fullName, StringComparison.CurrentCultureIgnoreCase));

        if (existingTeacher is not null)
        {
            return existingTeacher.IsActive
                ? (false, "Υπάρχει ήδη ενεργός/ή καθηγητής/τρια με αυτό το ονοματεπώνυμο.")
                : (false, "Υπάρχει ήδη απενεργοποιημένος/η καθηγητής/τρια με αυτό το ονοματεπώνυμο. Κάντε επανενεργοποίηση αντί για νέα προσθήκη.");
        }

        _db.Teachers.Add(new Teacher
        {
            FullName = fullName,
            Specialty = string.IsNullOrWhiteSpace(input.Specialty) ? null : input.Specialty.Trim(),
            IsActive = true
        });

        _db.SaveChanges();
        return (true, "Ο/Η καθηγητής/τρια προστέθηκε.");
    }

    public (bool Success, string Message) DeactivateTeacher(int teacherId)
    {
        var teacher = _db.Teachers.FirstOrDefault(t => t.Id == teacherId);
        if (teacher is null)
        {
            return (false, "Ο/Η καθηγητής/τρια δεν βρέθηκε.");
        }

        if (!teacher.IsActive)
        {
            return (false, "Ο/Η καθηγητής/τρια είναι ήδη απενεργοποιημένος/η.");
        }

        teacher.IsActive = false;
        _db.SaveChanges();
        return (true, "Ο/Η καθηγητής/τρια απενεργοποιήθηκε και δεν θα εμφανίζεται πλέον στις νέες κρατήσεις.");
    }

    public (bool Success, string Message) ReactivateTeacher(int teacherId)
    {
        var teacher = _db.Teachers.FirstOrDefault(t => t.Id == teacherId);
        if (teacher is null)
        {
            return (false, "Ο/Η καθηγητής/τρια δεν βρέθηκε.");
        }

        if (teacher.IsActive)
        {
            return (false, "Ο/Η καθηγητής/τρια είναι ήδη ενεργός/ή.");
        }

        teacher.IsActive = true;
        _db.SaveChanges();
        return (true, "Ο/Η καθηγητής/τρια επανενεργοποιήθηκε.");
    }


    public (bool Success, string Message) AddClassGroup(AddClassGroupInput input)
    {
        var name = input.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return (false, "Συμπληρώστε όνομα τμήματος.");
        }

        var existingClassGroup = _db.ClassGroups.AsEnumerable()
            .FirstOrDefault(c => string.Equals(c.Name.Trim(), name, StringComparison.CurrentCultureIgnoreCase));

        if (existingClassGroup is not null)
        {
            return existingClassGroup.IsActive
                ? (false, "Υπάρχει ήδη ενεργό τμήμα με αυτό το όνομα.")
                : (false, "Υπάρχει ήδη απενεργοποιημένο τμήμα με αυτό το όνομα. Κάντε επανενεργοποίηση αντί για νέα προσθήκη.");
        }

        _db.ClassGroups.Add(new ClassGroup
        {
            Name = name,
            IsActive = true
        });

        _db.SaveChanges();
        return (true, "Το τμήμα προστέθηκε.");
    }

    public (bool Success, string Message) DeactivateClassGroup(int classGroupId)
    {
        var classGroup = _db.ClassGroups.FirstOrDefault(c => c.Id == classGroupId);
        if (classGroup is null)
        {
            return (false, "Το τμήμα δεν βρέθηκε.");
        }

        if (!classGroup.IsActive)
        {
            return (false, "Το τμήμα είναι ήδη απενεργοποιημένο.");
        }

        classGroup.IsActive = false;
        _db.SaveChanges();
        return (true, "Το τμήμα απενεργοποιήθηκε και δεν θα εμφανίζεται πλέον στις νέες κρατήσεις.");
    }

    public (bool Success, string Message) ReactivateClassGroup(int classGroupId)
    {
        var classGroup = _db.ClassGroups.FirstOrDefault(c => c.Id == classGroupId);
        if (classGroup is null)
        {
            return (false, "Το τμήμα δεν βρέθηκε.");
        }

        if (classGroup.IsActive)
        {
            return (false, "Το τμήμα είναι ήδη ενεργό.");
        }

        classGroup.IsActive = true;
        _db.SaveChanges();
        return (true, "Το τμήμα επανενεργοποιήθηκε.");
    }

    public (bool Success, string Message) AddLessonPeriod(AddLessonPeriodInput input)
    {
        if (input.StartTime >= input.EndTime)
        {
            return (false, "Η ώρα έναρξης πρέπει να είναι πριν από την ώρα λήξης.");
        }

        if (_db.LessonPeriods.Any(p => p.Number == input.Number))
        {
            return (false, "Υπάρχει ήδη διδακτική ώρα με αυτόν τον αριθμό.");
        }

        var candidate = new LessonPeriod
        {
            Number = input.Number,
            StartTime = input.StartTime,
            EndTime = input.EndTime
        };

        var validation = ValidatePeriodSet(_db.LessonPeriods.AsNoTracking().ToList().Concat(new[] { candidate }).ToList());
        if (!validation.Success)
        {
            return validation;
        }

        _db.LessonPeriods.Add(candidate);
        _db.SaveChanges();
        return (true, "Η νέα διδακτική ώρα προστέθηκε.");
    }

    public (bool Success, string Message) UpdateLessonPeriods(IReadOnlyList<EditLessonPeriodInput> input)
    {
        if (input.Count == 0)
        {
            return (false, "Δεν βρέθηκαν ώρες για αποθήκευση.");
        }

        var updated = input
            .Select(p => new LessonPeriod
            {
                Id = p.Id,
                Number = p.Number,
                StartTime = p.StartTime,
                EndTime = p.EndTime
            })
            .ToList();

        var currentIds = _db.LessonPeriods.Select(p => p.Id).ToHashSet();
        if (updated.Any(p => !currentIds.Contains(p.Id)))
        {
            return (false, "Βρέθηκε διδακτική ώρα που δεν υπάρχει στο τρέχον ωράριο.");
        }

        var validation = ValidatePeriodSet(updated);
        if (!validation.Success)
        {
            return validation;
        }

        foreach (var period in updated)
        {
            var existing = _db.LessonPeriods.First(p => p.Id == period.Id);
            existing.Number = period.Number;
            existing.StartTime = period.StartTime;
            existing.EndTime = period.EndTime;
        }

        _db.SaveChanges();
        return (true, "Το ωράριο αποθηκεύτηκε.");
    }

    private static BookingDetails ToDetails(
        Booking booking,
        IReadOnlyDictionary<int, Room> rooms,
        IReadOnlyDictionary<int, Teacher> teachers,
        IReadOnlyDictionary<int, ClassGroup> classGroups,
        IReadOnlyDictionary<int, LessonPeriod> periods)
    {
        return new BookingDetails
        {
            Id = booking.Id,
            Date = booking.Date,
            Room = rooms[booking.RoomId],
            StartPeriod = periods[booking.StartLessonPeriodId],
            EndPeriod = periods[booking.EndLessonPeriodId],
            Teacher = teachers[booking.TeacherId],
            ClassGroup = booking.ClassGroupId.HasValue && classGroups.TryGetValue(booking.ClassGroupId.Value, out var classGroup)
                ? classGroup
                : null,
            SubjectOrPurpose = booking.SubjectOrPurpose,
            Notes = booking.Notes
        };
    }

    private static bool RangesOverlap(int startA, int endA, int startB, int endB)
    {
        return startA <= endB && startB <= endA;
    }

    private static (bool Success, string Message) ValidatePeriodSet(IReadOnlyList<LessonPeriod> periods)
    {
        if (periods.Any(p => p.Number <= 0))
        {
            return (false, "Οι αριθμοί των διδακτικών ωρών πρέπει να είναι θετικοί.");
        }

        if (periods.Any(p => p.StartTime >= p.EndTime))
        {
            return (false, "Κάθε διδακτική ώρα πρέπει να έχει ώρα έναρξης πριν από την ώρα λήξης.");
        }

        if (periods.Select(p => p.Number).Distinct().Count() != periods.Count)
        {
            return (false, "Δεν επιτρέπονται δύο διδακτικές ώρες με τον ίδιο αριθμό.");
        }

        var ordered = periods.OrderBy(p => p.Number).ToList();
        for (var i = 1; i < ordered.Count; i++)
        {
            if (ordered[i - 1].EndTime > ordered[i].StartTime)
            {
                return (false, "Οι διδακτικές ώρες δεν πρέπει να επικαλύπτονται χρονικά.");
            }
        }

        return (true, "OK");
    }
}
