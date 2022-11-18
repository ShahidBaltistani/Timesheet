using HtmlAgilityPack;
using System;
using System.Web;

namespace computan.timesheet.Models
{
    public class AllTaskViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime assignedon { get; set; }
        public string status { get; set; }
        public int? statusid { get; set; }
        public long id { get; set; }
        public long taskid { get; set; }
        public long ticketid { get; set; }
        public string subject { get; set; }
        public string uniquebody { get; set; }
        public string team { get; set; }
        public long? displayorder { get; set; }

        public string shortbody
        {
            get
            {
                if (uniquebody != null)
                {
                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(uniquebody);
                    string InnerText = htmlDoc.DocumentNode.InnerText;

                    if (InnerText.Length > 150)
                    {
                        InnerText = InnerText.Substring(0, 150);
                    }

                    return HttpUtility.HtmlDecode(InnerText);
                }

                return uniquebody;
            }
        }

        public string FullName
        {
            get
            {
                string fullname = FirstName + " " + LastName;
                return fullname;
            }
        }
    }
}