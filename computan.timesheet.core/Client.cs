using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class Client : BaseEntity
    {
        public long id { get; set; }

        [DisplayName("Parent Client")] public long? parentid { get; set; }

        [Required(ErrorMessage = "Client Name Is Required.")]
        [MaxLength(255)]
        [DisplayName("Client Name")]
        public string name { get; set; }

        [DisplayName("Client Type")] public long? clienttypeid { get; set; }

        [DisplayName("Max Billable hours")] public double? maxbillablehours { get; set; }

        [MaxLength(255)]
        [DisplayName("Primary Email")]
        public string email { get; set; }

        [MaxLength(1000)]
        [DisplayName("Address")]
        public string address { get; set; }

        [MaxLength(100)][DisplayName("City")] public string city { get; set; }

        [DisplayName("State")] public long? stateid { get; set; }

        [DisplayName("Country")] public long? countryid { get; set; }

        [MaxLength(20)][DisplayName("Zip")] public string zip { get; set; }

        [MaxLength(20)][DisplayName("Phone")] public string phone { get; set; }

        [MaxLength(20)]
        [DisplayName("Mobile")]
        public string mobile { get; set; }

        [MaxLength(255)]
        [DisplayName("Website")]
        public string website { get; set; }

        [DisplayName("Customer Source")] public long customersourceid { get; set; }

        [DisplayName("Account Manager")] public string accountmanager { get; set; }

        [DisplayName("Active?")] public bool isactive { get; set; }

        [DisplayName("Pm Tool")][Url] public string pmplateformlink { get; set; }

        [MaxLength(128)]
        [DisplayName("Username")]
        public string usersid { get; set; }

        [ForeignKey("parentid")] public virtual Client ParentClient { get; set; }

        [ForeignKey("accountmanager")] public virtual ApplicationUser User { get; set; }

        [ForeignKey("parentid")] public ICollection<Client> SubClients { get; set; }

        [ForeignKey("clienttypeid")] public ClientType ClientType { get; set; }

        [ForeignKey("customersourceid")] public CustomerSource CustomerSource { get; set; }

        [DisplayName("Warning?")] public bool iswarning { get; set; }

        [DisplayName("Warning Text")] public string warningtext { get; set; }

        public ICollection<ClientContact> ClientContactCollection { get; set; }
        public ICollection<Project> ProjectCollection { get; set; }
    }
}