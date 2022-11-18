using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace computan.timesheet.core
{
    public class Country : BaseEntity
    {
        public long id { get; set; }

        //[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        [MaxLength(2, ErrorMessage = "Please enter 2 character code.")]
        [DisplayName("Code 2")]
        public string iso { get; set; }

        //[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        [MaxLength(3, ErrorMessage = "Please enter 3 character code.")]
        [DisplayName("Code 3")]
        public string iso3 { get; set; }

        [MaxLength(80)]
        [DisplayName("Country Name")]
        [Required(ErrorMessage = "Country Name Is Required.")]
        public string name { get; set; }

        [MaxLength(80)]
        [DisplayName("Nice Country Name")]
        [Required(ErrorMessage = "Nice Country Name Is Required.")]
        public string nicename { get; set; }

        [DisplayName("Country Code")] public short? numcode { get; set; }

        [DisplayName("Country Phone Code")] public short? phonecode { get; set; }
    }
}