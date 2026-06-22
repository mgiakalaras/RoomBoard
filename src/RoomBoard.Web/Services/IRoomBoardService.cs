using RoomBoard.Web.Models;

namespace RoomBoard.Web.Services;

public interface IRoomBoardService
{
    IReadOnlyList<Room> GetRooms();
    IReadOnlyList<Room> GetInactiveRooms();
    IReadOnlyList<Teacher> GetTeachers();
    IReadOnlyList<Teacher> GetInactiveTeachers();
    IReadOnlyList<ClassGroup> GetClassGroups();
    IReadOnlyList<ClassGroup> GetInactiveClassGroups();
    IReadOnlyList<LessonPeriod> GetLessonPeriods();
    SchoolSettings GetSchoolSettings();
    (bool Success, string Message) SaveSchoolSettings(SchoolSettingsInput input, string? logoPath);
    KioskSpotlightSettings GetKioskSpotlightSettings();
    (bool Success, string Message) SaveKioskSpotlightSettings(KioskSpotlightInput input, string? imagePath);

    LessonPeriod? GetCurrentPeriod(DateTime now);
    IReadOnlyList<RoomStatus> GetRoomStatuses(DateOnly date, int periodNumber);
    IReadOnlyList<BookingDetails> GetBookingsForDate(DateOnly date);
    IReadOnlyList<BookingDetails> GetNextPeriodClassMovements(DateOnly date, int periodNumber);

    (bool Success, string Message) CreateBooking(CreateBookingInput input);
    (bool Success, string Message) CancelBooking(int bookingId, string? reason);

    (bool Success, string Message) AddRoom(AddRoomInput input);
    (bool Success, string Message) DeactivateRoom(int roomId);
    (bool Success, string Message) ReactivateRoom(int roomId);
    (bool Success, string Message) AddTeacher(AddTeacherInput input);
    (bool Success, string Message) DeactivateTeacher(int teacherId);
    (bool Success, string Message) ReactivateTeacher(int teacherId);
    (bool Success, string Message) AddClassGroup(AddClassGroupInput input);
    (bool Success, string Message) DeactivateClassGroup(int classGroupId);
    (bool Success, string Message) ReactivateClassGroup(int classGroupId);
    (bool Success, string Message) AddLessonPeriod(AddLessonPeriodInput input);
    (bool Success, string Message) UpdateLessonPeriods(IReadOnlyList<EditLessonPeriodInput> input);
}
