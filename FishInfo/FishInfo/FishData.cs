using Harmony;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FishInfo
{
    internal class FishData
    {
        internal List<string> CaughtIn;
        internal List<TimePair> CatchingTimes;
        internal Weather weather;
        internal bool IsCrabPot;
        internal Season season;

        public FishData()
        {
            CaughtIn = new List<string>();
            CatchingTimes = new List<TimePair>();
            weather = Weather.None;
            IsCrabPot = false;
            season = Season.None;
        }

        internal void AddLocation(string location)
        {
            string ToAdd = Regex.Replace(char.ToUpper(location[0]) + location.Substring(1), "([A-Z0-9]+)", " $1").Trim();
            if (!CaughtIn.Contains(ToAdd))
            {
                CaughtIn.Add(ToAdd);
            }
        }
        internal void AddTimes(int StartTime, int EndTime)
        {
            TimePair times = new TimePair(StartTime, EndTime);
            if (!CatchingTimes.Contains(times))
            {
                CatchingTimes.Add(times);
            }
        }
        internal void AddWeather(Weather weather)
        {
            if(!this.weather.HasFlag(weather))
            this.weather |= weather;
        }
        internal void AddSeason(Season season)
        {
            if (!this.season.HasFlag(season))
                this.season |= season;
        }
        internal void SetCrabPot(bool IsCrabPot)
        {
            this.IsCrabPot = IsCrabPot;
        }

        private string CalcEachTimeString(int time)
        {
            if (time == 1200)
            {
                return "midday";
            }
            else if (time == 2400)
            {
                return "midnight";
            }
            else if (time < 1200)
            {
                return FormatTime(time) + "am";
            }
            else if (time < 2400)
            {
                return FormatTime(time - 1200) + "pm";
            }
            else
            {
                return FormatTime(time - 2400) + "am";
            }
        }

        private string FormatTime(int time)
        {
            string sTime = time.ToString();

            if (sTime.Length == 3)
            {
                sTime = sTime.Insert(1, ":");
            }
            else
            {
                sTime = sTime.Insert(2, ":");
            }

            return sTime;
        }

        private string CalcTimeString()
        {
            if (CatchingTimes.Count == 1 && CatchingTimes[0].StartTime == 600 && CatchingTimes[0].EndTime == 2600)
            {
                return "All day!";
            }
            List<string> strings = new List<string>();
            foreach (TimePair times in CatchingTimes)
            {
                strings.Add($"{CalcEachTimeString(times.StartTime)} - {CalcEachTimeString(times.EndTime)}");
            }
            return strings.Join();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine();
            if (IsCrabPot)
            {
                sb.AppendLine($"Crab Pot in: {CaughtIn.Join()}");
            }
            else
            {
                if (CaughtIn.Count == 0)
                {
                    CaughtIn.Add("Unknown - Mines or event");
                }
                sb.AppendLine($"Caught in: {CaughtIn.Join()}");
                if (season != Season.None)
                {
                    sb.AppendLine($"Season: {season.ToString().Replace("_", " ")}");
                }
                sb.AppendLine($"Time: {CalcTimeString()}");
                sb.AppendLine($"Weather: {weather.ToString()}");
            }
            return sb.ToString();
        }
    }
}
