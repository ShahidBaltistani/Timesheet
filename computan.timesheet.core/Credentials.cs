using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class Credentials : BaseEntity
    {
        public long id { get; set; }

        [DisplayName("Title")] public string title { get; set; }

        [DisplayName("Credential Type")]
        [Required(ErrorMessage = "Credential type is required.")]
        public long crendentialtypeid { get; set; }

        [DisplayName("Credential Level")]
        [Required(ErrorMessage = "Credntial Level is required.")]
        public long credentiallevelid { get; set; }

        [DisplayName("Credential Project")] public long? projectid { get; set; }

        [DisplayName("Credential Category")] public long credentialcategoryid { get; set; }

        [DisplayName("URL")] public string url { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [DisplayName("User Name")]
        public string username { get; set; }

        [DisplayName("Password")]
        [Required(ErrorMessage = "Password is required.")]
        public string password { get; set; }

        [DisplayName("Port")] public string port { get; set; }

        [DisplayName("Comments")] public string comments { get; set; }

        [DisplayName("Host/IP")] public string host { get; set; }

        [DisplayName("Network Domain")] public string networkdomain { get; set; }

        [ForeignKey("crendentialtypeid")] public CredentialType CredentialType { get; set; }

        [DisplayName("Hash")] public string passwordhash { get; set; }

        [DisplayName("Salt")] public string passwordsalt { get; set; }

        [ForeignKey("projectid")] public Project Project { get; set; }

        [ForeignKey("credentiallevelid")] public CredentialLevel CredentialLevel { get; set; }

        [ForeignKey("credentialcategoryid")] public CredentialCategory CredentialCategory { get; set; }

        [NotMapped] public List<CredentialSkills> ck { get; set; }

        public byte[] crendentialfile { get; set; }

        [DisplayName("File Name")] public string filename { get; set; }

        [DisplayName("Linked Credential")] public string linkedCredential { get; set; }

        [DisplayName("Status")] public bool isactive { get; set; }
    }
}