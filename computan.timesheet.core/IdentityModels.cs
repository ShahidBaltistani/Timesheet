using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

namespace computan.timesheet.core
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            ClaimsIdentity userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        [DisplayName("First Name*")]
        [MaxLength(100)]
        //[Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }
        //[Required(ErrorMessage = "Last name is required.")]
        [DisplayName("Last Name*")]
        [MaxLength(100)]
        public string LastName { get; set; }
        [NotMapped]
        [DisplayName("Full Name")]
        public string FullName
        {
            get
            {
                string fullname = FirstName + " " + LastName;
                return fullname.Trim();
            }
        }
        [NotMapped]
        public string NameEmail
        {
            get
            {
                string name = FirstName + " " + LastName + " - " + Email;
                return name.Trim();
            }
        }

        [NotMapped]
        public string GetInitials
        {
            get
            {
                string initials = string.Empty;
                if (!string.IsNullOrEmpty(FirstName))
                {
                    initials = FirstName.Substring(0, 1);
                }

                if (!string.IsNullOrEmpty(LastName))
                {
                    initials += LastName.Substring(0, 1);
                }

                return initials;
            }
        }

        public string Address { get; set; }

        [DisplayName("City Name")]
        [MaxLength(100)]
        public string City { get; set; }

        [DisplayName("State Name")]
        public long? StateId { get; set; }

        [DisplayName("Country Name")]
        public long? CountryId { get; set; }

        [DisplayName("Zip")]
        [MaxLength(20)]
        public string Zip { get; set; }

        [DisplayName("First Phone Number")]
        [MaxLength(20)]
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

        [DisplayName("Credential Level")]
        public long Levelid { get; set; }

        [DisplayName("Skype id")]
        public string Skypeid { get; set; }

        [DisplayName("Status")]
        public bool isactive { get; set; }
        [DisplayName("Is Notify Manager")]
        public bool IsNotifyManagerOnTaskAssignment { get; set; }
        [DisplayName("Restrict to Enter time on Email Sending")]
        public bool IsRestrictEntertime { get; set; }

        [DisplayName("Exchange Username")]
        public string ExchangeUsername { get; set; }
        [DisplayName("Exchange Password")]
        public string ExchangePassword { get; set; }
        [DisplayName("Signature")]
        public string EmailReplySignature { get; set; }

        [DisplayName("Created Date")]
        public DateTime createdonutc { get; set; }

        [DisplayName("Last Updated Date")]
        public DateTime? updatedonutc { get; set; }

        [MaxLength(20)]
        [DisplayName("IP Used")]
        public string ipused { get; set; }

        [MaxLength]
        [DisplayName("Last Updated By")]
        public string userid { get; set; }

        [DisplayName("Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [DisplayName("National Identification Number")]
        public string NationalIdentificationNumber { get; set; }

        [DisplayName("Personal Email Address")]
        public string PersonalEmailAddress { get; set; }

        [DisplayName("Person Name & Relation in Case of Emergency")]
        public string PersonNameEmergency { get; set; }

        [DisplayName("Emergency Phone Number")]
        public string EmergencyPhoneNumber { get; set; }

        [DisplayName("Spouse Name")]
        public string SpouseName { get; set; } //If marrried

        [DisplayName("Spouse Date of Birth")]
        public DateTime? SpouseDateOfBirth { get; set; }

        [DisplayName("Children Names with Date of Birth")]
        public string ChildrenNames { get; set; }

        [DisplayName("Date of Joining")]
        public DateTime? DateOfJoining { get; set; }
        [DisplayName("Total Working Experience")]
        public string Experience { get; set; }

        [DisplayName("Account Number")]
        public string AccountNumber { get; set; }

        [DisplayName("Meezan Bank Branch Name")]
        public string BranchName { get; set; }

        public bool IsPkHoliday { get; set; }
        public bool IsRemoteJob { get; set; }
        [DisplayName("Project Manager")]
        public string ProjectManager { get; set; }
        [DisplayName("Team Lead")]
        public string TeamLead { get; set; }
        [DisplayName("Shift Timings PKT")]
        [MaxLength(25)]
        public string ShiftTimePK { get; set; }

        [DisplayName("Shift Timings EST")]
        [MaxLength(25)]
        public string ShiftTimeEST { get; set; }

        [NotMapped]
        [DisplayName("Reported To")]
        public string ReportedId { get; set; } 

        [NotMapped]
        [DisplayName("Date of Birth")]
        public string DateOfBirths { get; set; }

        [NotMapped]
        [DisplayName("Spouse Date of Birth")]
        public string SpouseDateOfBirths { get; set; }

        [NotMapped]
        [DisplayName("Date of Joining")]
        public string DateOfJoin { get; set; }


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

        //Google 2FA
        public bool IsAppAuthenticatorEnabled { get; set; }
        public string AppAuthenticatorSecretKey { get; set; }
        //Rocket 2FA
        public bool IsRocketAuthenticatorEnabled { get; set; }


    }
}