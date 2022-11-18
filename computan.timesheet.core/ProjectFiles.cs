using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.IO;

namespace computan.timesheet.core
{
    public class ProjectFiles : BaseEntity
    {
        public long id { get; set; }
        public long projectid { get; set; }
        public string filename { get; set; }

        public long? filetypeid { get; set; }

        [ForeignKey("projectid")] public Project Project { get; set; }

        [ForeignKey("filetypeid")] public FileType FileType { get; set; }

        [NotMapped]
        public string FilePath
        {
            get
            {
                //AppRunMode mode;
                //string filepath = HttpContext.Current.Request.Url.Host;
                //if (Enum.TryParse(ConfigurationManager.AppSettings["AppRunMode"], out mode))
                //{
                //    switch (mode)
                //    {
                //        case AppRunMode.Debug:
                //            filepath = ConfigurationManager.AppSettings["DebugAppUrl"];
                //            break;
                //        case AppRunMode.Live:
                //            filepath = ConfigurationManager.AppSettings["LiveAppUrl"];
                //            break;
                //        default:
                //            filepath = ConfigurationManager.AppSettings["LiveAppUrl"];
                //            break;
                //    }
                //}
                //else
                //{
                //    filepath = ConfigurationManager.AppSettings["LiveAppUrl"];
                //}
                string filepath = ConfigurationManager.AppSettings["projectfilepath"];
                filepath += Path.GetFileNameWithoutExtension(filename);
                //filepath += "_" + ConfigurationManager.AppSettings["SliderSmallImageThumbValue"];
                filepath += Path.GetExtension(filename);

                return filepath;
            }
        }
    }
}