namespace DSMS.Data
{
    public enum SystemEventType : int
    {
        None = 0,
        SignIn = 1,
        SignOut = 2,
        OffCampus = 4,
        OnCampus = 8,
        CalledIn = 16,
        Enroll = 25,
        Withdraw = 26,
        Suspended = 27,
        AbsenceLetterSent = 28,
        ExcusedAbsence = 50,
        UnexcusedAbsence = 51,
        IncompleteDay = 53
    }

    public enum CalendarEventType : int
    {
        FirstDayOfSchool = 1,
        LastDayOfSchool = 10,
        NoSchool = 100
    }

    public enum FineReasonType : int
    {
        Other = 0,
        UnexcusedAbsence = 1,
        IncompleteDay = 10
    }
}