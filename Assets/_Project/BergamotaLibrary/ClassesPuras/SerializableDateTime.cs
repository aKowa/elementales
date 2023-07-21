using System;

// Uma versao serializavel do DateTime
// Para criar um DateTime com todas as variaveis:
// new DateTime(data.year, data.month, data.day, data.hour, data.minute, data.second);

namespace BergamotaLibrary
{
    [System.Serializable]
    public class SerializableDateTime
    {
        public int second;
        public int minute;
        public int hour;

        public int day;
        public int month;
        public int year;

        public SerializableDateTime(DateTime dateTime)
        {
            second = dateTime.Second;
            minute = dateTime.Minute;
            hour = dateTime.Hour;
            day = dateTime.Day;
            month = dateTime.Month;
            year = dateTime.Year;
        }

        public static DateTime NewDateTime(SerializableDateTime data)
        {
            return new DateTime(data.year, data.month, data.day, data.hour, data.minute, data.second);
        }
    }
}