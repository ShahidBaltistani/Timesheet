using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace computan.timesheet.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "RoleName")]
        public string Name { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Official Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required")]
        [DisplayName("First Name")]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required")]
        [DisplayName("Last Name")]
        [MaxLength(100)]
        public string LastName { get; set; }

        [DisplayName("Skype Id")]
        [MaxLength(100)]
        public string Skypeid { get; set; }

        public string Address { get; set; }

        [DisplayName("City Name")]
        [MaxLength(100)]
        public string City { get; set; }

        [DisplayName("State Name")] public long? StateId { get; set; }

        [DisplayName("Country Name")] public long? CountryId { get; set; }

        [DisplayName("Credential Level")] public long Levelid { get; set; }

        [DisplayName("Zip")][MaxLength(20)] public string Zip { get; set; }

        [DisplayName("First Phone Number")]
        [MaxLength(20)]
        [Required]
        public string Phone { get; set; }

        [DisplayName("Second Phone Number")]
        [MaxLength(20)]
        public string Mobile { get; set; }

        [DisplayName("Designation")]
        [MaxLength(100)]
        public string Designation { get; set; }

        [DisplayName("Profile Image")]
        [MaxLength(255)]
        public string ProfileImage { get; set; }

        [DisplayName("Status")] public bool isactive { get; set; }

        public List<string> userskills { get; set; }
        public IEnumerable<SelectListItem> skills { get; set; }
        public IEnumerable<SelectListItem> RolesList { get; set; }
        public IEnumerable<SelectListItem> TeamLeads { get; set; }

        //Later Changes
        [DisplayName("Date of Birth")]
        public string DateOfBirth { get; set; }

        [DisplayName("National Identification Number")]
        public string NationalIdentificationNumber { get; set; }

        [DisplayName("Personal Email Address")]
        public string PersonalEmailAddress { get; set; }

        [DisplayName("Person Name & Relation in Case of Emergency")]
        public string PersonNameEmergency { get; set; }

        [DisplayName("Emergency Phone Number")]
        public string EmergencyPhoneNumber { get; set; }

        [DisplayName("Spouse Name")] public string SpouseName { get; set; }

        [DisplayName("Spouse Date of Birth")] public string SpouseDateOfBirth { get; set; }

        [DisplayName("Children Names with DateOfBirth")]
        public string ChildrenNames { get; set; }

        [DisplayName("Date of Joining")]
        public string DateOfJoining { get; set; }

        [DisplayName("Total Working Experience")]
        public string Experience { get; set; }

        [DisplayName("Account Number")] public string AccountNumber { get; set; }

        [DisplayName("Meezan Bank Branch Name")]
        public string BranchName { get; set; }

        [DisplayName("Reported To")] public string RepotedId { get; set; }

        public bool IsPkHoliday { get; set; }
        public bool IsRemoteJob { get; set; }
        [DisplayName("Project Manager")]
        public string ProjectManager { get; set; }
        [DisplayName("Team Lead")]
        public string TeamLead { get; set; }
        [DisplayName("Shift Timings PKT")]
        public string ShiftTimePK { get; set; }

        [DisplayName("Shift Timings EST")]
        public string ShiftTimeEST { get; set; }

        //Document Fields

        [DisplayName("Upload Latest Resume")]
        public string LatestResume { get; set; }

        [DisplayName("Upload Last Degree")]
        public string LastDegree { get; set; }

        [DisplayName("Upload CNIC Front Side")]
        public string CNIC_Front { get; set; }

        [DisplayName("Upload CNIC Back Side")]
        public string CNIC_Back { get; set; }

        [DisplayName("Upload Experience Letter")]
        public string ExperienceLetter { get; set; }

        public string OldEmail { get; set; }

    }
}