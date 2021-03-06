﻿namespace logParser {
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System;
    using System.Linq;

    public class Log : ILog
    {
        public int? LogId { get; set; } = null;
        public DateTime DateTime { get; set; }
        public string Level { get; set; }
        public string Info { get; set; }
        private static char _delimeter='|';
        public static char Delimeter
        {
            get { return _delimeter; }
            set
            {
                if (value != '\0')
                    _delimeter = value;
            }
        }
        private static Regex _LogFormat;
        public static Regex LogFormat
        {
            get { return _LogFormat; }
            private set { _LogFormat = value; }
        }

        public Log(int? logId, string dateStr, string timeStr, string level, string info)
        {
            this.LogId = logId;
            this.Level = level;
            this.Info = info;
            string[] dateArr = dateStr.Split('/');
            DateTime time = new DateTime(DateTime.Now.Year, int.Parse(dateArr[0]), int.Parse(dateArr[1]));
            string[] timeArr = timeStr.Split(':');
            TimeSpan span = new TimeSpan(short.Parse(timeArr[0]), short.Parse(timeArr[1]), short.Parse(timeArr[2]));
            this.DateTime = time.Add(span);
        }

        public static bool IsLog(string line)
        {
            return _LogFormat.IsMatch(line);
        }

        public static void SetLevelsToConsider(HashSet<string> userGivenLevels)
        {
            string str = @"(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])\s(0[1-9]|1[012])[:](0[1-9]|[12345][0-9])[:](0[1-9]|[12345][0-9])\s(";
            foreach (string level in userGivenLevels)
            {
                str = str + level + "|";
            }
            LogFormat = new Regex(str.Remove(str.Length - 1, 1) + ")", RegexOptions.IgnoreCase);
        }

        public static Log GetLog(string formattedLogLine)
        {
            string[] lineSplit = formattedLogLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Log log = new Log(null,
                    Regex.Match(formattedLogLine, "(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])").ToString(),
                    Regex.Match(formattedLogLine, "(0[1-9]|1[012])[:](0[1-9]|[12345][0-9])[:](0[1-9]|[12345][0-9])").ToString(),
                    Regex.Match(formattedLogLine, "(INFO|WARN|DEBUG|TRACE|ERROR|EVENT)").ToString(),
                    string.Join(" ", Enumerable.Skip<string>(lineSplit, 3)));
            return log;
        }
        public override string ToString()
        {
            string str = "" + Delimeter + LogId + Delimeter + Level.ToString() + Delimeter + DateTime.ToString("dd MMM yyy") + Delimeter + DateTime.ToShortTimeString() + Delimeter + Info + Delimeter;
            return str;
        }
    }
}