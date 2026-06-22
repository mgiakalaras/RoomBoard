using RoomBoard.Web.Models;

namespace RoomBoard.Web.Services;

public sealed class InMemoryRoomBoardService : IRoomBoardService
{
    private readonly object _lock = new();
    private KioskSpotlightSettings _kioskSpotlightSettings = new()
    {
        Id = 1,
        IsManualEnabled = false,
        Label = "Image of the day",
        Title = "Μια εικόνα, μια σκέψη",
        Text = "Κάθε μέρα μπορεί να ξεκινά με μια μικρή αφορμή για παρατήρηση.",
        ImagePath = "/img/kiosk/spotlight-cosmos.svg",
        Credit = "RoomBoard visual spotlight"
    };
    private readonly List<Room> _rooms = new();
    private readonly List<Teacher> _teachers = new();
    private readonly List<ClassGroup> _classGroups = new();
    private readonly List<LessonPeriod> _periods = new();
    private readonly List<Booking> _bookings = new();
    private int _nextBookingId = 1;
    private SchoolSettings _schoolSettings = new()
    {
        Id = 1,
        SchoolName = "Πυθαγόρειο ΓΕΛ Σάμου",
        SchoolType = "Γενικό Λύκειο",
        SchoolYear = "2025-2026"
    };

    public InMemoryRoomBoardService()
    {
        SeedReferenceData();
        SeedDemoBookings(DateOnly.FromDateTime(DateTime.Today));
    }


    public SchoolSettings GetSchoolSettings()
    {
        lock (_lock)
        {
            return new SchoolSettings
            {
                Id = _schoolSettings.Id,
                SchoolName = _schoolSettings.SchoolName,
                SchoolType = _schoolSettings.SchoolType,
                SchoolYear = _schoolSettings.SchoolYear,
                Address = _schoolSettings.Address,
                Phone = _schoolSettings.Phone,
                Email = _schoolSettings.Email,
                LogoPath = _schoolSettings.LogoPath,
                UpdatedAt = _schoolSettings.UpdatedAt
            };
        }
    }

    public (bool Success, string Message) SaveSchoolSettings(SchoolSettingsInput input, string? logoPath)
    {
        lock (_lock)
        {
            var schoolName = input.SchoolName.Trim();
            if (string.IsNullOrWhiteSpace(schoolName))
            {
                return (false, "Συμπληρώστε όνομα σχολικής μονάδας.");
            }

            _schoolSettings.SchoolName = schoolName;
            _schoolSettings.SchoolType = string.IsNullOrWhiteSpace(input.SchoolType) ? "Σχολική μονάδα" : input.SchoolType.Trim();
            _schoolSettings.SchoolYear = string.IsNullOrWhiteSpace(input.SchoolYear) ? DateTime.Today.Year + "-" + (DateTime.Today.Year + 1) : input.SchoolYear.Trim();
            _schoolSettings.Address = string.IsNullOrWhiteSpace(input.Address) ? null : input.Address.Trim();
            _schoolSettings.Phone = string.IsNullOrWhiteSpace(input.Phone) ? null : input.Phone.Trim();
            _schoolSettings.Email = string.IsNullOrWhiteSpace(input.Email) ? null : input.Email.Trim();

            if (!string.IsNullOrWhiteSpace(logoPath))
            {
                _schoolSettings.LogoPath = logoPath;
            }

            _schoolSettings.UpdatedAt = DateTime.Now;
            return (true, "Οι ρυθμίσεις σχολικής μονάδας αποθηκεύτηκαν.");
        }
    }


    public KioskSpotlightSettings GetKioskSpotlightSettings()
    {
        lock (_lock)
        {
            return new KioskSpotlightSettings
            {
                Id = _kioskSpotlightSettings.Id,
                IsManualEnabled = _kioskSpotlightSettings.IsManualEnabled,
                Label = _kioskSpotlightSettings.Label,
                Title = _kioskSpotlightSettings.Title,
                Text = _kioskSpotlightSettings.Text,
                ImagePath = _kioskSpotlightSettings.ImagePath,
                Credit = _kioskSpotlightSettings.Credit,
                UpdatedAt = _kioskSpotlightSettings.UpdatedAt
            };
        }
    }

    public (bool Success, string Message) SaveKioskSpotlightSettings(KioskSpotlightInput input, string? imagePath)
    {
        lock (_lock)
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

            _kioskSpotlightSettings.IsManualEnabled = input.IsManualEnabled;
            _kioskSpotlightSettings.Label = string.IsNullOrWhiteSpace(input.Label) ? "Image of the day" : input.Label.Trim();
            _kioskSpotlightSettings.Title = string.IsNullOrWhiteSpace(input.Title) ? "Μια εικόνα, μια σκέψη" : input.Title.Trim();
            _kioskSpotlightSettings.Text = string.IsNullOrWhiteSpace(input.Text) ? "Κάθε μέρα μπορεί να ξεκινά με μια μικρή αφορμή για παρατήρηση." : input.Text.Trim();
            _kioskSpotlightSettings.Credit = string.IsNullOrWhiteSpace(input.Credit) ? null : input.Credit.Trim();

            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                _kioskSpotlightSettings.ImagePath = imagePath;
            }

            if (string.IsNullOrWhiteSpace(_kioskSpotlightSettings.ImagePath))
            {
                _kioskSpotlightSettings.ImagePath = "/img/kiosk/spotlight-cosmos.svg";
            }

            _kioskSpotlightSettings.UpdatedAt = DateTime.Now;
            return (true, input.IsManualEnabled
                ? "Το χειροκίνητο kiosk spotlight ενεργοποιήθηκε και αποθηκεύτηκε."
                : "Το χειροκίνητο kiosk spotlight απενεργοποιήθηκε. Θα εμφανίζεται το offline fallback.");
        }
    }

    public IReadOnlyList<Room> GetRooms()
    {
        lock (_lock)
        {
            return _rooms
                .Where(r => r.IsActive)
                .OrderBy(r => r.DisplayOrder)
                .ThenBy(r => r.Name)
                .ToList();
        }
    }

    public IReadOnlyList<Room> GetInactiveRooms()
    {
        lock (_lock)
        {
            return _rooms
                .Where(r => !r.IsActive)
                .OrderBy(r => r.DisplayOrder)
                .ThenBy(r => r.Name)
                .ToList();
        }
    }

    public IReadOnlyList<Teacher> GetTeachers()
    {
        lock (_lock)
        {
            return _teachers
                .Where(t => t.IsActive)
                .OrderBy(t => t.FullName)
                .ToList();
        }
    }

    public IReadOnlyList<Teacher> GetInactiveTeachers()
    {
        lock (_lock)
        {
            return _teachers
                .Where(t => !t.IsActive)
                .OrderBy(t => t.FullName)
                .ToList();
        }
    }

    public IReadOnlyList<ClassGroup> GetClassGroups()
    {
        lock (_lock)
        {
            return _classGroups
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToList();
        }
    }

    public IReadOnlyList<ClassGroup> GetInactiveClassGroups()
    {
        lock (_lock)
        {
            return _classGroups
                .Where(c => !c.IsActive)
                .OrderBy(c => c.Name)
                .ToList();
        }
    }

    public IReadOnlyList<LessonPeriod> GetLessonPeriods()
    {
        lock (_lock)
        {
            return _periods
                .OrderBy(p => p.Number)
                .ToList();
        }
    }

    public LessonPeriod? GetCurrentPeriod(DateTime now)
    {
        lock (_lock)
        {
            var currentTime = TimeOnly.FromDateTime(now);
            return _periods
                .OrderBy(p => p.Number)
                .FirstOrDefault(p => currentTime >= p.StartTime && currentTime <= p.EndTime);
        }
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
        lock (_lock)
        {
            return _bookings
                .Where(b => b.Date == date)
                .Select(ToDetails)
                .OrderBy(b => b.StartPeriod.Number)
                .ThenBy(b => b.Room.DisplayOrder)
                .ToList();
        }
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
        lock (_lock)
        {
            var room = _rooms.FirstOrDefault(r => r.Id == input.RoomId && r.IsActive);
            var startPeriod = _periods.FirstOrDefault(p => p.Id == input.StartLessonPeriodId);
            var endPeriod = _periods.FirstOrDefault(p => p.Id == input.EndLessonPeriodId);
            var teacher = _teachers.FirstOrDefault(t => t.Id == input.TeacherId && t.IsActive);

            if (room is null || startPeriod is null || endPeriod is null || teacher is null)
            {
                return (false, "Η κράτηση δεν αποθηκεύτηκε. Ελέγξτε αίθουσα, ώρες και καθηγητή.");
            }

            if (endPeriod.Number < startPeriod.Number)
            {
                return (false, "Η ώρα λήξης πρέπει να είναι ίδια ή μεταγενέστερη από την ώρα έναρξης.");
            }

            if (input.ClassGroupId is not null && input.ClassGroupId != 0 && !_classGroups.Any(c => c.Id == input.ClassGroupId.Value && c.IsActive))
            {
                return (false, "Το τμήμα που επιλέχθηκε δεν υπάρχει.");
            }

            var conflictBooking = _bookings.FirstOrDefault(b =>
            {
                if (b.Date != input.Date || b.RoomId != input.RoomId)
                {
                    return false;
                }

                var existingStart = _periods.First(p => p.Id == b.StartLessonPeriodId).Number;
                var existingEnd = _periods.First(p => p.Id == b.EndLessonPeriodId).Number;

                return RangesOverlap(startPeriod.Number, endPeriod.Number, existingStart, existingEnd);
            });

            if (conflictBooking is not null)
            {
                var existingStart = _periods.First(p => p.Id == conflictBooking.StartLessonPeriodId);
                var existingEnd = _periods.First(p => p.Id == conflictBooking.EndLessonPeriodId);
                var existingTeacher = _teachers.FirstOrDefault(t => t.Id == conflictBooking.TeacherId)?.FullName ?? "άλλον/η καθηγητή/τρια";

                return (false, $"Σύγκρουση κράτησης: η αίθουσα {room.Name} είναι ήδη κλεισμένη στις {existingStart.Number}η - {existingEnd.Number}η ώρα από {existingTeacher}.");
            }

            _bookings.Add(new Booking
            {
                Id = _nextBookingId++,
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

            return (true, "Η κράτηση αποθηκεύτηκε.");
        }
    }

    public (bool Success, string Message) AddRoom(AddRoomInput input)
    {
        lock (_lock)
        {
            var name = input.Name.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                return (false, "Συμπληρώστε όνομα αίθουσας.");
            }

            if (_rooms.Any(r => string.Equals(r.Name.Trim(), name, StringComparison.CurrentCultureIgnoreCase)))
            {
                return (false, "Υπάρχει ήδη αίθουσα με αυτό το όνομα.");
            }

            var nextId = _rooms.Count == 0 ? 1 : _rooms.Max(r => r.Id) + 1;
            var nextOrder = _rooms.Count == 0 ? 1 : _rooms.Max(r => r.DisplayOrder) + 1;

            _rooms.Add(new Room
            {
                Id = nextId,
                Name = name,
                Location = string.IsNullOrWhiteSpace(input.Location) ? "Χωρίς τοποθεσία" : input.Location.Trim(),
                DisplayOrder = nextOrder,
                IsActive = true
            });

            return (true, "Η αίθουσα προστέθηκε.");
        }
    }

    public (bool Success, string Message) DeactivateRoom(int roomId)
    {
        lock (_lock)
        {
            var room = _rooms.FirstOrDefault(r => r.Id == roomId);
            if (room is null)
            {
                return (false, "Η αίθουσα δεν βρέθηκε.");
            }

            if (!room.IsActive)
            {
                return (false, "Η αίθουσα είναι ήδη απενεργοποιημένη.");
            }

            room.IsActive = false;
            return (true, "Η αίθουσα απενεργοποιήθηκε και δεν θα εμφανίζεται πλέον στις νέες κρατήσεις.");
        }
    }

    public (bool Success, string Message) ReactivateRoom(int roomId)
    {
        lock (_lock)
        {
            var room = _rooms.FirstOrDefault(r => r.Id == roomId);
            if (room is null)
            {
                return (false, "Η αίθουσα δεν βρέθηκε.");
            }

            if (room.IsActive)
            {
                return (false, "Η αίθουσα είναι ήδη ενεργή.");
            }

            room.IsActive = true;
            return (true, "Η αίθουσα επανενεργοποιήθηκε.");
        }
    }

    public (bool Success, string Message) AddTeacher(AddTeacherInput input)
    {
        lock (_lock)
        {
            var fullName = input.FullName.Trim();
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return (false, "Συμπληρώστε ονοματεπώνυμο καθηγητή/τριας.");
            }

            if (_teachers.Any(t => string.Equals(t.FullName.Trim(), fullName, StringComparison.CurrentCultureIgnoreCase)))
            {
                return (false, "Υπάρχει ήδη καθηγητής/τρια με αυτό το ονοματεπώνυμο.");
            }

            var nextId = _teachers.Count == 0 ? 1 : _teachers.Max(t => t.Id) + 1;

            _teachers.Add(new Teacher
            {
                Id = nextId,
                FullName = fullName,
                Specialty = string.IsNullOrWhiteSpace(input.Specialty) ? null : input.Specialty.Trim(),
                IsActive = true
            });

            return (true, "Ο/Η καθηγητής/τρια προστέθηκε.");
        }
    }

    public (bool Success, string Message) DeactivateTeacher(int teacherId)
    {
        lock (_lock)
        {
            var teacher = _teachers.FirstOrDefault(t => t.Id == teacherId);
            if (teacher is null)
            {
                return (false, "Ο/Η καθηγητής/τρια δεν βρέθηκε.");
            }

            if (!teacher.IsActive)
            {
                return (false, "Ο/Η καθηγητής/τρια είναι ήδη απενεργοποιημένος/η.");
            }

            teacher.IsActive = false;
            return (true, "Ο/Η καθηγητής/τρια απενεργοποιήθηκε και δεν θα εμφανίζεται πλέον στις νέες κρατήσεις.");
        }
    }

    public (bool Success, string Message) ReactivateTeacher(int teacherId)
    {
        lock (_lock)
        {
            var teacher = _teachers.FirstOrDefault(t => t.Id == teacherId);
            if (teacher is null)
            {
                return (false, "Ο/Η καθηγητής/τρια δεν βρέθηκε.");
            }

            if (teacher.IsActive)
            {
                return (false, "Ο/Η καθηγητής/τρια είναι ήδη ενεργός/ή.");
            }

            teacher.IsActive = true;
            return (true, "Ο/Η καθηγητής/τρια επανενεργοποιήθηκε.");
        }
    }


    public (bool Success, string Message) AddClassGroup(AddClassGroupInput input)
    {
        lock (_lock)
        {
            var name = input.Name.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                return (false, "Συμπληρώστε όνομα τμήματος.");
            }

            var existingClassGroup = _classGroups
                .FirstOrDefault(c => string.Equals(c.Name.Trim(), name, StringComparison.CurrentCultureIgnoreCase));

            if (existingClassGroup is not null)
            {
                return existingClassGroup.IsActive
                    ? (false, "Υπάρχει ήδη ενεργό τμήμα με αυτό το όνομα.")
                    : (false, "Υπάρχει ήδη απενεργοποιημένο τμήμα με αυτό το όνομα. Κάντε επανενεργοποίηση αντί για νέα προσθήκη.");
            }

            var nextId = _classGroups.Count == 0 ? 1 : _classGroups.Max(c => c.Id) + 1;

            _classGroups.Add(new ClassGroup
            {
                Id = nextId,
                Name = name,
                IsActive = true
            });

            return (true, "Το τμήμα προστέθηκε.");
        }
    }

    public (bool Success, string Message) DeactivateClassGroup(int classGroupId)
    {
        lock (_lock)
        {
            var classGroup = _classGroups.FirstOrDefault(c => c.Id == classGroupId);
            if (classGroup is null)
            {
                return (false, "Το τμήμα δεν βρέθηκε.");
            }

            if (!classGroup.IsActive)
            {
                return (false, "Το τμήμα είναι ήδη απενεργοποιημένο.");
            }

            classGroup.IsActive = false;
            return (true, "Το τμήμα απενεργοποιήθηκε και δεν θα εμφανίζεται πλέον στις νέες κρατήσεις.");
        }
    }

    public (bool Success, string Message) ReactivateClassGroup(int classGroupId)
    {
        lock (_lock)
        {
            var classGroup = _classGroups.FirstOrDefault(c => c.Id == classGroupId);
            if (classGroup is null)
            {
                return (false, "Το τμήμα δεν βρέθηκε.");
            }

            if (classGroup.IsActive)
            {
                return (false, "Το τμήμα είναι ήδη ενεργό.");
            }

            classGroup.IsActive = true;
            return (true, "Το τμήμα επανενεργοποιήθηκε.");
        }
    }

    public (bool Success, string Message) AddLessonPeriod(AddLessonPeriodInput input)
    {
        lock (_lock)
        {
            if (input.StartTime >= input.EndTime)
            {
                return (false, "Η ώρα έναρξης πρέπει να είναι πριν από την ώρα λήξης.");
            }

            if (_periods.Any(p => p.Number == input.Number))
            {
                return (false, "Υπάρχει ήδη διδακτική ώρα με αυτόν τον αριθμό.");
            }

            var nextId = _periods.Count == 0 ? 1 : _periods.Max(p => p.Id) + 1;
            var candidate = new LessonPeriod
            {
                Id = nextId,
                Number = input.Number,
                StartTime = input.StartTime,
                EndTime = input.EndTime
            };

            var validation = ValidatePeriodSet(_periods.Concat(new[] { candidate }).ToList());
            if (!validation.Success)
            {
                return validation;
            }

            _periods.Add(candidate);
            return (true, "Η νέα διδακτική ώρα προστέθηκε.");
        }
    }

    public (bool Success, string Message) UpdateLessonPeriods(IReadOnlyList<EditLessonPeriodInput> input)
    {
        lock (_lock)
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

            if (updated.Any(p => !_periods.Any(existing => existing.Id == p.Id)))
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
                var existing = _periods.First(p => p.Id == period.Id);
                existing.Number = period.Number;
                existing.StartTime = period.StartTime;
                existing.EndTime = period.EndTime;
            }

            return (true, "Το ωράριο αποθηκεύτηκε.");
        }
    }

    private BookingDetails ToDetails(Booking booking)
    {
        var startPeriod = _periods.First(p => p.Id == booking.StartLessonPeriodId);
        var endPeriod = _periods.First(p => p.Id == booking.EndLessonPeriodId);

        return new BookingDetails
        {
            Id = booking.Id,
            Date = booking.Date,
            Room = _rooms.First(r => r.Id == booking.RoomId),
            StartPeriod = startPeriod,
            EndPeriod = endPeriod,
            Teacher = _teachers.First(t => t.Id == booking.TeacherId),
            ClassGroup = booking.ClassGroupId.HasValue
                ? _classGroups.FirstOrDefault(c => c.Id == booking.ClassGroupId.Value)
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

    private void SeedReferenceData()
    {
        _rooms.AddRange(new[]
        {
            new Room { Id = 1, Name = "Αίθουσα Προβολών", Location = "Κεντρικό κτήριο · 1ος όροφος", DisplayOrder = 1 },
            new Room { Id = 2, Name = "Εργαστήριο Πληροφορικής 1", Location = "Ισόγειο · Η/Υ", DisplayOrder = 2 },
            new Room { Id = 3, Name = "Βιβλιοθήκη", Location = "Κεντρικό κτήριο · Ισόγειο", DisplayOrder = 3 },
            new Room { Id = 4, Name = "Αίθουσα Πολλαπλών Χρήσεων", Location = "Νέο κτήριο · Ισόγειο", DisplayOrder = 4 }
        });

        _teachers.AddRange(new[]
        {
            new Teacher { Id = 1, FullName = "Γιακαλάρας Μάριος", Specialty = "ΠΕ86" },
            new Teacher { Id = 2, FullName = "Παπαδοπούλου Μαρία", Specialty = "ΠΕ02" },
            new Teacher { Id = 3, FullName = "Βασιλείου Νίκος", Specialty = "ΠΕ02" },
            new Teacher { Id = 4, FullName = "Κωνσταντίνου Ελένη", Specialty = "ΠΕ04" }
        });

        _classGroups.AddRange(new[]
        {
            new ClassGroup { Id = 1, Name = "Α1" },
            new ClassGroup { Id = 2, Name = "Α2" },
            new ClassGroup { Id = 3, Name = "Β1" },
            new ClassGroup { Id = 4, Name = "Β2" },
            new ClassGroup { Id = 5, Name = "Γ Οικ.-Πληρ." }
        });

        _periods.AddRange(new[]
        {
            new LessonPeriod { Id = 1, Number = 1, StartTime = new TimeOnly(8, 15), EndTime = new TimeOnly(9, 0) },
            new LessonPeriod { Id = 2, Number = 2, StartTime = new TimeOnly(9, 5), EndTime = new TimeOnly(9, 50) },
            new LessonPeriod { Id = 3, Number = 3, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(10, 45) },
            new LessonPeriod { Id = 4, Number = 4, StartTime = new TimeOnly(10, 55), EndTime = new TimeOnly(11, 40) },
            new LessonPeriod { Id = 5, Number = 5, StartTime = new TimeOnly(11, 50), EndTime = new TimeOnly(12, 35) },
            new LessonPeriod { Id = 6, Number = 6, StartTime = new TimeOnly(12, 40), EndTime = new TimeOnly(13, 25) },
            new LessonPeriod { Id = 7, Number = 7, StartTime = new TimeOnly(13, 30), EndTime = new TimeOnly(14, 0) }
        });
    }

    private void SeedDemoBookings(DateOnly today)
    {
        AddSeed(today, 2, 1, 1, 1, 2, "Πληροφορική");
        AddSeed(today, 2, 2, 2, 1, 3, "Εργασία");
        AddSeed(today, 1, 3, 4, 2, 4, "Ιστορία");
        AddSeed(today, 2, 4, 4, 1, 5, "Πληροφορική");
        AddSeed(today, 3, 4, 5, 3, 1, "Φιλαναγνωσία");
    }

    private void AddSeed(DateOnly date, int roomId, int startPeriodNumber, int endPeriodNumber, int teacherId, int classGroupId, string subject)
    {
        var startPeriod = _periods.First(p => p.Number == startPeriodNumber);
        var endPeriod = _periods.First(p => p.Number == endPeriodNumber);

        _bookings.Add(new Booking
        {
            Id = _nextBookingId++,
            Date = date,
            RoomId = roomId,
            StartLessonPeriodId = startPeriod.Id,
            EndLessonPeriodId = endPeriod.Id,
            TeacherId = teacherId,
            ClassGroupId = classGroupId,
            SubjectOrPurpose = subject,
            CreatedAt = DateTime.Now
        });
    }
}
