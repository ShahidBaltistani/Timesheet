using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace computan.timesheet.core
{
    public class Project : BaseEntity
    {
        public long id { get; set; }

        [DisplayName("Parent Project")] public long? parentid { get; set; }

        [Required(ErrorMessage = "Client Name is Required.")]
        [DisplayName("Client")]
        public long? clientid { get; set; }

        [Required(ErrorMessage = "Project Name Is Required.")]
        [MaxLength(255)]
        [DisplayName("Project Name")]
        public string name { get; set; }

        [DisplayName("Project Manager")] public string projectmanager { get; set; }

        [DisplayName("Description")] public string description { get; set; }

        [DisplayName("Start Date")]
        [Column(TypeName = "DateTime2")]
        public DateTime? startdate { get; set; }

        [DisplayName("Completion Date")]
        [Column(TypeName = "DateTime")]
        public DateTime? completiondate { get; set; }

        [DisplayName("Status")] public bool isactive { get; set; }

        [DisplayName("Warning")] public bool iswarning { get; set; }

        [DisplayName("Warning Text")] public string warningtext { get; set; }

        [ForeignKey("parentid")] public virtual Project ParentProject { get; set; }

        [ForeignKey("parentid")] public ICollection<Project> SubProjects { get; set; }

        [ForeignKey("clientid")] public Client Client { get; set; }

        public ICollection<ProjectFiles> ProjectFiles { get; set; }
        public ICollection<ProjectNotes> ProjectNotes { get; set; }

        [NotMapped]
        public string getprojectname
        {
            get
            {
                string parentname = string.Empty;
                if (parentid != null && ParentProject != null)
                {
                    parentname = ParentProject.name + " -> " + name;
                }
                else
                {
                    parentname = name;
                }

                return parentname;
            }
        }

        public string getclientname
        {
            get
            {
                string clientname = string.Empty;
                if (clientid != null && Client.parentid != null && Client.ParentClient != null)
                {
                    clientname = Client.ParentClient.name + " -> " + Client.name;
                }
                else if (clientid != null && Client.parentid == null && Client.ParentClient == null)
                {
                    clientname = Client.name;
                }

                return clientname;
            }
        }

        public string getclienttype
        {
            get
            {
                string clientname = string.Empty;

                if (Client.clienttypeid == 1)
                {
                    clientname = "Retainer";
                }
                else if (Client.clienttypeid == 2)
                {
                    clientname = "Non-Retainer";
                }

                return clientname;
            }
        }

        public string getclientmaxhour
        {
            get
            {
                string clientmaxhour = string.Empty;

                if (!string.IsNullOrEmpty(Client.maxbillablehours.ToString()))
                {
                    clientmaxhour = Client.maxbillablehours.ToString();
                }
                else
                {
                    clientmaxhour = "Not Specified";
                }

                return clientmaxhour;
            }
        }

        [ForeignKey("projectmanager")] public virtual ApplicationUser User { get; set; }
    }
}