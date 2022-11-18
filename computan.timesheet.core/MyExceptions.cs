using System;

namespace computan.timesheet.core
{
    public class MyExceptions
    {
        public long id { get; set; }
        public DateTime exceptiondate { get; set; }
        public string controller { get; set; }
        public string action { get; set; }
        public string exception_message { get; set; }
        public string exception_source { get; set; }
        public string exception_stracktrace { get; set; }
        public string exception_targetsite { get; set; }
        public string ipused { get; set; }
        public string userid { get; set; }
    }
}