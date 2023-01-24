using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
//using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace BGLeopold
{
    public class EDTWeekData
    {
        #region static elements / ParseICSStringToEDTWeekData

        //private static string modulePattern = @"BEGIN:VEVENT(?<word>[A-Z0-9a-z àé°\p{P}]+)END:VEVENT";
        private static string imagePath = "EDtetalon.png";

        public static EDTWeekData ParseICSStringToEDTWeekData(string s)
        {
            List<string> modulesStrings = GetSubstringsBetweenPatterns(s, "BEGIN:VEVENT", "END:VEVENT");

            List<EDTModule> modules = new List<EDTModule>();
            foreach (string moduleStr in modulesStrings)
            {
                modules.Add(GenerateEDTModuleFromString(moduleStr));
            }

            List<EDTModule> monday = new List<EDTModule>(), tuesday = new List<EDTModule>(), wednesday = new List<EDTModule>(), thursday = new List<EDTModule>(), friday = new List<EDTModule>(), saturday = new List<EDTModule>();

            foreach (EDTModule module in modules)
            {
                Date d = module.startDate;
                DateTime date = new DateTime(d.year, d.month, d.day);
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        //en général il n'y a pas de cour le dimanche
                        Console.WriteLine("Debug1 pls");
                        break;
                    case DayOfWeek.Monday:
                        monday.Add(module);
                        break;
                    case DayOfWeek.Tuesday:
                        tuesday.Add(module);
                        break;
                    case DayOfWeek.Wednesday:
                        wednesday.Add(module);
                        break;
                    case DayOfWeek.Thursday:
                        thursday.Add(module);
                        break;
                    case DayOfWeek.Friday:
                        friday.Add(module);
                        break;
                    case DayOfWeek.Saturday:
                        saturday.Add(module);
                        break;
                    default:
                        break;
                }
            }
            monday.Sort(new ModuleComparer()); tuesday.Sort(new ModuleComparer()); wednesday.Sort(new ModuleComparer()); thursday.Sort(new ModuleComparer()); friday.Sort(new ModuleComparer()); saturday.Sort(new ModuleComparer());
            return new EDTWeekData(monday, tuesday, wednesday, thursday, friday, saturday);
        }

        private static EDTModule GenerateEDTModuleFromString(string s)
        {
            string name = GetSubstringsBetweenPatterns(s, "SUMMARY:", "\r\n").FirstOrDefault();
            name = name == null || name == string.Empty ? string.Empty : RemoveDoubleBackslash(name).Replace("\\n", "").Replace("\n", "");
            string salle = GetSubstringsBetweenPatterns(s, "LOCATION:", "\r\n").FirstOrDefault();
            salle = salle == null || salle == string.Empty ? string.Empty : RemoveDoubleBackslash(salle).Replace("\\n", "").Replace("\n", "");
            string info = GetSubstringsBetweenPatterns(s, "DESCRIPTION:", "\r\n").FirstOrDefault();
            info = info == null || info == string.Empty ? string.Empty : RemoveUselessDataInModuleInfo(info);

            string dateInfo = GetSubstringsBetweenPatterns(s, "DTSTART:", "\r\n").FirstOrDefault();
            Date begDate = GenerateDateFromICSString(dateInfo);

            dateInfo = GetSubstringsBetweenPatterns(s, "DTEND:", "\r\n").FirstOrDefault();
            Date endDate = GenerateDateFromICSString(dateInfo);

            return new EDTModule(name, begDate, endDate, salle, info);
        }

        private static string RemoveBegAndEndBackslashN(string s)
        {
            while(s.Substring(0, 1) == "\n")
            {
                s = s.Remove(0, 1);
            }
            while(s.Substring(s.Length - 1, 1) == "\n")
            {
                s = s.Remove(s.Length - 1, 1);
            }
            return s;
        }

        private static string RemoveDoubleBackslash(string s)
        {
            string res = new string(s.ToCharArray());
            for (int i = res.Length - 2; i >= 0; i--)
            {
                if (res[i] == '\\' && res[i + 1] == '\\')
                {
                    res = res.Remove(i + 1, 1);
                }
            }
            return res;
        }

        private static string RemoveUselessDataInModuleInfo(string s)
        {
            string res = s.Replace("\\n", "\n");

            const string begPattern = "(Exporté";
            int  begIndex = 0, index = 0;
            bool foundPattern = false;
            for (int i = 0; i < res.Length; i++)
            {
                if(res[i] == begPattern[index])
                {
                    if(index == 0)
                        begIndex = i;
                    if(index >= begPattern.Length - 1)
                    {
                        foundPattern = true;
                        break;
                    }
                    index++;
                }
                else
                {
                    index = 0;
                }
            }
            if(foundPattern)
                res = res.Remove(begIndex);
            return RemoveBegAndEndBackslashN(res);
        }

        private static Date GenerateDateFromICSString(string s)
        {
            int year = s.Substring(0, 4).ToInt();
            int month = s.Substring(4, 2).ToInt();
            int day = s.Substring(6, 2).ToInt();
            int hour = s.Substring(9, 2).ToInt() + 1;
            int min = s.Substring(11, 2).ToInt();
            int sec = s.Substring(13, 2).ToInt();

            return new Date(year, month, day, hour, min, sec);
        }

        #region GenerateStringsModules

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s">la chaine de char ou l'on extrait les données</param>
        /// <param name="startPattern">Le pattern de début d'extraction de données</param>
        /// <param name="endPattern">Le pattern de fin d'extraction de données</param>
        /// <returns>L'ensemble des string entre le début de pattern et le fin de pattern</returns>
        private static List<string> GetSubstringsBetweenPatterns(string s, string startPattern, string endPattern)
        {
            List<string> res = new List<string>();

            char c;
            bool detectModule = false;
            int indexBeg = 0, indexEnd = 0;
            StringBuilder currentModule = new StringBuilder();

            for (int i = 0; i < s.Length; i++)
            {
                c = s[i];
                if(detectModule)
                {
                    if (c == startPattern[indexBeg])
                    {
                        if (indexBeg >= startPattern.Length - 1)
                        {
                            currentModule.Clear();
                            indexBeg = 0;
                        }
                        else
                        {
                            indexBeg++;
                        }
                    }
                    else
                    {
                        indexBeg = 0;
                    }
                    if (c == endPattern[indexEnd])
                    {
                        if (indexEnd >= endPattern.Length - 1)
                        {
                            string tmp = currentModule.ToString();
                            tmp = tmp.Remove(tmp.Length - endPattern.Length + 1, endPattern.Length - 1);
                            res.Add(tmp);
                            currentModule.Clear();
                            detectModule = false;
                            indexEnd = 0;
                            continue;
                        }
                        else
                        {
                            indexEnd++;
                        }
                    }
                    else
                    {
                        indexEnd = 0;
                    }

                    currentModule.Append(c);
                }
                else
                {
                    if(c == startPattern[indexBeg])
                    {
                        if (indexBeg >= startPattern.Length - 1)
                        {
                            detectModule = true;
                            indexBeg = 0;
                        }
                        else
                        {
                            indexBeg++;
                        }
                    }
                    else
                    {
                        indexBeg = 0;
                    }
                }
            }
            return res;
        }

        #endregion

        private class ModuleComparer : IComparer<EDTModule>
        {
            public int Compare(EDTModule p1, EDTModule p2)//return 1 => p1 > p2, 0 => p1 == p2, -1 => p1 < p2
            {
                if (ReferenceEquals(p1, default(EDTModule)) && ReferenceEquals(p2,default(EDTModule)))
                {
                    return 0;
                }
                if (ReferenceEquals(p1, default(EDTModule)))
                {
                    return 1;
                }
                if (ReferenceEquals(p2, default(EDTModule)))
                {
                    return -1;
                }

                if (p1.startDate.year < p2.startDate.year)
                    return -1;
                if (p2.startDate.year < p1.startDate.year)
                    return 1;
                if (p1.startDate.month < p2.startDate.month)
                    return -1;
                if (p2.startDate.month < p1.startDate.month)
                    return 1;
                if (p1.startDate.day < p2.startDate.day)
                    return -1;
                if (p2.startDate.day < p1.startDate.day)
                    return 1;
                if (p1.startDate.hour < p2.startDate.hour)
                    return -1;
                if (p2.startDate.hour < p1.startDate.hour)
                    return 1;
                if (p1.startDate.min < p2.startDate.min)
                    return -1;
                if (p2.startDate.min < p1.startDate.min)
                    return 1;
                if (p1.startDate.sec < p2.startDate.sec)
                    return -1;
                if (p2.startDate.sec < p1.startDate.sec)
                    return 1;
                return 0;
            }
        }

        #endregion

        public List<EDTModule> monday { get; set; }
        public List<EDTModule> tuesday { get; set; }
        public List<EDTModule> wednesday { get; set; }
        public List<EDTModule> thursday { get; set; }
        public List<EDTModule> friday { get; set; }
        public List<EDTModule> saturday { get; set; }

        public EDTWeekData(List<EDTModule> monday, List<EDTModule> tuesday, List<EDTModule> wednesday, List<EDTModule> thursday, List<EDTModule> friday, List<EDTModule> saturday)
        {
            this.monday = monday; this.tuesday = tuesday; this.wednesday = wednesday; this.thursday = thursday; this.friday = friday; this.saturday = saturday;
        }

        public void Save(string filename)
        {
            string json = JsonConvert.SerializeObject(this);
            if (json != "{}")
                File.WriteAllText(filename, json);
        }

        public static EDTWeekData Load(string filename)
        {
            if (!File.Exists(filename))
                return null;

            string json = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject(json, typeof(EDTWeekData)) as EDTWeekData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>the path were the image is save</returns>
        public string GenerateImage()
        {
            EDTDrawer drawer = new EDTDrawer(imagePath);

            float startLine, endLine;
            DrawRectangleAndStringModuleDay(monday, 0f);
            DrawRectangleAndStringModuleDay(tuesday, 1f);
            DrawRectangleAndStringModuleDay(wednesday, 2f);
            DrawRectangleAndStringModuleDay(thursday, 3f);
            DrawRectangleAndStringModuleDay(friday, 4f);
            DrawRectangleAndStringModuleDay(saturday, 5);

            drawer.SaveImage("tmpImg.png");
            return "tmpImg.png";

            void DrawRectangleAndStringModuleDay(List<EDTModule> modules, in float column)
            {
                StringBuilder sb = new StringBuilder();
                foreach (EDTModule module in modules)
                {
                    startLine = (module.startDate.hour - 8) * 2 + (module.startDate.min / 30f);
                    endLine = (module.endDate.hour - 8) * 2 + (module.endDate.min / 30f);
                    drawer.DrawModuleRectangle(module.name, startLine, endLine, column);

                    sb.Clear();
                    sb.Append(module.name);
                    sb.Append("\n");
                    sb.Append(module.infos);
                    sb.Append("\n");
                    sb.Append(module.salle);
                    sb.Append("\n");
                    sb.Append(module.startDate.ToString(true, true));
                    sb.Append(" - ");
                    sb.Append(module.endDate.ToString(false, true));
                    string s = sb.ToString();
                    drawer.DrawString(module.name, sb.ToString(), startLine, endLine, column);
                }
            }
        }

        public override bool Equals(object obj) => obj is EDTWeekData ? ((EDTWeekData)obj) == this : false;
        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(EDTWeekData a, EDTWeekData b)
        {
            bool VerifyDay(List<EDTModule> dayA, List<EDTModule> dayB)
            {
                if (dayA.Count != dayB.Count)
                    return false;
                for (int i = 0; i < dayA.Count; i++)
                {
                    if (dayA[i] != dayB[i])
                        return false;
                }
                return true;
            }
            return VerifyDay(a.monday, b.monday) && VerifyDay(a.tuesday, b.tuesday) && VerifyDay(a.wednesday, b.wednesday) && VerifyDay(a.thursday, b.thursday) && VerifyDay(a.friday, b.friday) && VerifyDay(a.saturday, b.saturday);
        }

        public static bool operator !=(EDTWeekData a, EDTWeekData b) => !(a == b);

        public class EDTModule
        {
            public string name { get; set; }
            public Date startDate { get; set; }
            public Date endDate { get; set; }
            public string salle { get; set; }
            public string infos { get; set; }

            public EDTModule(string name, Date startDate, Date endDate, string salle, string infos)
            {
                this.name = name; this.startDate = startDate; this.endDate = endDate; this.salle = salle; this.infos = infos;
            }

            public EDTModule Clone() => new EDTModule(name, startDate, endDate, salle, infos);

            public override bool Equals(object obj) => obj is EDTModule ? ((EDTModule)obj) == this : false;
            public override int GetHashCode() => base.GetHashCode();

            public static bool operator ==(EDTModule a, EDTModule b)
            {
                return a.startDate == b.startDate && a.endDate == b.endDate && a.name == b.name && a.salle == b.salle && a.infos == b.infos;
            }
            public static bool operator !=(EDTModule a, EDTModule b) => !(a == b);
        }

        public class Date
        {
            public int year { get; set; }
            public int month { get; set; }
            public int day { get; set; }
            public int hour { get; set; }
            public int min { get; set; }
            public int sec { get; set; }

            public Date(in int year, in int month, in int day, in int hour, in int min, in int sec)
            {
                this.year = year; this.month = month; this.day = day; this.hour = hour; this.min = min; this.sec = sec;
            }

            public static string IntToString2Format(in int nb)
            {
                string res = nb.ToString();
                if (res.Length <= 1)
                    res = "0" + res;
                if (res.Length > 2)
                    throw new Exception("The number nb need to have 2 maximum digits!");
                return res;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder(IntToString2Format(day));
                sb.Append("/");
                sb.Append(IntToString2Format(month));
                sb.Append("/");
                sb.Append(year);
                sb.Append(" ");
                sb.Append(IntToString2Format(hour));
                sb.Append("h");
                sb.Append(IntToString2Format(min));
                return sb.ToString();
            }


            public string ToString(in bool date, in bool horraire)
            {
                StringBuilder sb = new StringBuilder();
                if(date)
                {
                    sb.Append(IntToString2Format(day));
                    sb.Append("/");
                    sb.Append(IntToString2Format(month));
                    sb.Append("/");
                    sb.Append(year);
                    sb.Append(" ");
                }
                if(horraire)
                {
                    sb.Append(IntToString2Format(hour));
                    sb.Append("h");
                    sb.Append(IntToString2Format(min));
                }
                return sb.ToString();
            }

            public Date Clone() => new Date(year, month, day, hour, min, sec);

            public override bool Equals(object obj) => obj is Date ? ((Date)obj) == this : false;

            public override int GetHashCode() => base.GetHashCode();

            public static bool operator ==(Date a, Date b)
            {
                return a.year == b.year && a.month == b.month && a.day == b.day && a.hour == b.hour && a.min == b.min && a.sec == b.sec;
            }
            public static bool operator !=(Date a, Date b) => !(a == b);
        }
    }
}
