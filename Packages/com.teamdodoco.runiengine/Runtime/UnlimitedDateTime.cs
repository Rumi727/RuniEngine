#nullable enable
using System;

namespace RuniEngine
{
    public struct UnlimitedDateTime
    {
        public UnlimitedDateTime(int year, int month, int day) : this()
        {
            this.year = year;
            this.month = month;
            this.day = day;
        }

        public UnlimitedDateTime(int year, int month, int day, int hour, int minute, int second) : this(year, month, day)
        {
            this.hour = hour;
            this.minute = minute;
            this.second = second;
        }

        public UnlimitedDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond) : this(year, month, day, hour, minute, second) => this.millisecond = millisecond;

        public int year { get; set; }
        public int month { get; set; }
        public int day { get; set; }
        public int hour { get; set; }
        public int minute { get; set; }
        public int second { get; set; }
        public int millisecond { get; set; }

        public static explicit operator DateTime(UnlimitedDateTime dateTime) => new DateTime(dateTime.year, dateTime.month, dateTime.day, dateTime.hour, dateTime.minute, dateTime.second, dateTime.minute);
        public static implicit operator UnlimitedDateTime(DateTime dateTime) => new UnlimitedDateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Minute);

        public override readonly string ToString() => $"{year}-{month}-{day} {hour}:{minute}:{second}";
    }
}
