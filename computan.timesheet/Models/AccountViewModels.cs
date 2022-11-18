using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace computan.timesheet.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required][Display(Name = "Email")] public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public string Token { get; set; }
        public bool RememberMe { get; set; }
        public string Email { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required] public string Provider { get; set; }

        [Required][Display(Name = "Code")] public string Code { get; set; }

        public string ReturnUrl { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }
    }

    public class ForgotViewModel
    {
        [Required][Display(Name = "Email")] public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")] public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Official Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password",
            ErrorMessage = "The password and confirmation password do not match.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Confirm password is required")]
        public string ConfirmPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required.")]
        [DisplayName("First Name")]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required.")]
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

        [DisplayName("User Team")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select the team for new user.")]
        public long teamId { get; set; }

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

        public List<long> skills { get; set; }

        [DisplayName("Report To")] public string Reported { get; set; }

        //Miss Nadia Changes
        [DisplayName("Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [DisplayName("National Identification Number")]
        public string NationalIdentificationNumber { get; set; }

        [DisplayName("Personal Email Address")]
        public string PersonalEmailAddress { get; set; }

        [DisplayName("Person Name & Relation in Case of Emergency")]
        public string PersonNameEmergency { get; set; } //Incase of Emergency

        [DisplayName("Emergency Phone Number")]
        public string EmergencyPhoneNumber { get; set; }

        [DisplayName("Spouse Name")] public string SpouseName { get; set; }

        [DisplayName("Spouse Date of Birth")] public DateTime? SpouseDateOfBirth { get; set; }

        [DisplayName("Children Names with Date of Birth")]
        public string ChildrenNames { get; set; }

        [DisplayName("Date of Joining")]
        public DateTime? DateOfJoining { get; set; }
        [DisplayName("Total Working Experience")]
        public string Experience { get; set; }

        [DisplayName("Account Number")] public string AccountNumber { get; set; }

        [DisplayName("Meezan Bank Branch Name")]
        public string BranchName { get; set; }

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
        public string ReportedId { get; set; }
        public string TeamLeadId { get; set; }
        public string ProjectManagerId { get; set; }
        public bool Shift { get; set; }

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

    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password",
            ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}