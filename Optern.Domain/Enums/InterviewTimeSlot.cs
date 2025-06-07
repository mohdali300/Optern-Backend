using System.ComponentModel;

namespace Optern.Domain.Enums
{
    public enum InterviewTimeSlot
    {
        [Description("08:00 AM")]
        EightAM,

        [Description("10:00 AM")]
        TenAM,

        [Description("12:00 PM")]
        TwelvePM,

        [Description("02:00 PM")]
        TwoPM,

        [Description("06:00 PM")]
        SixPM,

        [Description("10:00 PM")]
        TenPM 
    }
}
