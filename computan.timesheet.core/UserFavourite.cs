using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class UserFavourite
    {
        public long id { get; set; }
        public int userfavouritetypeid { get; set; }
        public long userfavouriteid { get; set; }
        public string userid { get; set; }

        [ForeignKey("userfavouritetypeid")] public UserFavouriteType UserFavouriteType { get; set; }
    }
}