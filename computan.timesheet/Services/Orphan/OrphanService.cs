using computan.timesheet.Contexts;
using computan.timesheet.Extensions;
using computan.timesheet.Models.OrphanTickets;
using System;
using System.Collections.Generic;

namespace computan.timesheet.Services.Orphan
{
    public class OrphanService
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        //public int CalculateOrphanAge(int ticketstatus, DateTime lastActivityDate)
        //{
        //    long age = db.ConversationStatus.Where(x => x.id == ticketstatus).Select(x => x.OrphanAge).FirstOrDefault();
        //    int orphanAge = Convert.ToInt32((DateTime.Now - lastActivityDate.AddDays(age)).Days);
        //    return orphanAge;
        //}

        private string tableRow(OrphanTicketViewModel ticket) //, int orphanAge
        {
            return @"
                    <td style='line-height: 24px; font-size: 16px; margin: 0; padding: 12px; border: 1px solid #e2e8f0;' align='left' valign='top'>" + ticket.StatusName + @"</td>
                    <td style='line-height: 24px; font-size: 16px; margin: 0; padding: 12px; border: 1px solid #e2e8f0;' align='left' valign='top'>" + ticket.Age + @"</td>
                    <td style='line-height: 24px; font-size: 16px; margin: 0; padding: 12px; border: 1px solid #e2e8f0;' align='left' valign='top'><a href=" + ViewExtensions.BaseUrl + @"/tickets/ticketitem/" + ticket.id + @">" + ticket.topic + @"</a></td>
                    <td style='line-height: 24px; font-size: 16px; margin: 0; padding: 12px; border: 1px solid #e2e8f0;' align='left' valign='top'>
                        <table class='btn btn-info btn-sm' role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-radius: 6px; border-collapse: separate !important;'>
                            <tbody>
                                <tr>
                                    <td style='line-height: 24px; font-size: 16px; border-radius: 6px; margin: 0;' align='center' bgcolor='#0dcaf0'>
                                        <a href=" + ViewExtensions.BaseUrl + @"/Orphan/SuppressTicket/" + ticket.id + @"/?isExternal=true" + " " + @"target='_blank' style='color: #ffffff; font-size: 14px; font-family: Helvetica, Arial, sans-serif; text-decoration: none; border-radius: 3px; line-height: 17.5px; display: block; font-weight: normal; white-space: nowrap; background-color: #0dcaf0; padding: 4px 8px; border: 1px solid #0dcaf0;'>Suppress</a>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                    <td style='line-height: 24px; font-size: 16px; margin: 0; padding: 12px; border: 1px solid #e2e8f0;' align='left' valign='top'>
                        <table class='btn btn-info btn-sm' role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-radius: 6px; border-collapse: separate !important;'>
                            <tbody>
                                <tr>
                                    <td style='line-height: 24px; font-size: 16px; border-radius: 6px; margin: 0;' align='center' bgcolor='#d9534f'>
                                        <a href=" + ViewExtensions.BaseUrl + @"/Tickets/ChnageTicketStatus/" + ticket.id + "/?status=" + 8 + "&isExternal=true" + " " + @"target='_blank' style='color: #ffffff; font-size: 14px; font-family: Helvetica, Arial, sans-serif; text-decoration: none; border-radius: 3px; line-height: 17.5px; display: block; font-weight: normal; white-space: nowrap; background-color: #d9534f; padding: 4px 8px; border: 1px solid #d9534f;'>Trash</a>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>";
        }

        public string EmailFooterHTML()
        {
            return @"</table>
                 </div>
                    <table class='s-5 w-full' role='presentation' border='0' cellpadding='0' cellspacing='0' style='width: 100%;' width='100%'>
                        <tbody>
                            <tr>
                                <td style='line-height: 20px; font-size: 20px; width: 100%; height: 20px; margin: 0;' align='left' width='100%' height='20'>
                                    &#160;
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <table class='hr' role='presentation' border='0' cellpadding='0' cellspacing='0' style='width: 100%;'>
                        <tbody>
                            <tr>
                                <td style='line-height: 24px; font-size: 16px; border-top-width: 1px; border-top-color: #e2e8f0; border-top-style: solid; height: 1px; width: 100%; margin: 0;' align='left'>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <table class='s-5 w-full' role='presentation' border='0' cellpadding='0' cellspacing='0' style='width: 100%;' width='100%'>
                        <tbody>
                            <tr>
                                <td style='line-height: 20px; font-size: 20px; width: 100%; height: 20px; margin: 0;' align='left' width='100%' height='20'>
                                    &#160;
                                </td>
                            </tr>
                        </tbody>
                    </table>
                        </body>";
        }

        public string MailTableHeaders(string teamName)
        {
            return @"<div class='space-y-3'>
                    <h5 class='text-yellow-700' style='color: #997404; padding-top: 0; padding-bottom: 0; font-weight: 500; vertical-align: baseline; font-size: 20px; line-height: 24px; margin: 0;' align='left'>" + teamName + @"</h5>
                    <table class='s-3 w-full' role='presentation' border='0' cellpadding='0' cellspacing='0' style='width: 100%;' width='100%'>
                        <tbody>
                            <tr>
                                <td style='line-height: 12px; font-size: 12px; width: 100%; height: 12px; margin: 0;' align='left' width='100%' height='12'>
                                    &#160;
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <table class='table table-striped table-bordered' border='0' cellpadding='0' cellspacing='0' style='width: 100%; max-width: 100%; border: 1px solid #e2e8f0;'>
                        <thead>
                            <tr>
                                <th style='line-height: 24px; font-size: 16px; margin: 0; padding: 12px; border-color: #e2e8f0; border-style: solid; border-width: 1px 1px 2px;' align='left' valign='top'>Status</th>
                                <th style='line-height: 24px; font-size: 16px; margin: 0; padding: 12px; border-color: #e2e8f0; border-style: solid; border-width: 1px 1px 2px;' align='left' valign='top'>Age</th>
                                <th style='line-height: 24px; font-size: 16px; margin: 0; padding: 12px; border-color: #e2e8f0; border-style: solid; border-width: 1px 1px 2px;' align='left' valign='top'>Ticket</th>
                                <th style='line-height: 24px; font-size: 16px; margin: 0; padding: 12px; border-color: #e2e8f0; border-style: solid; border-width: 1px 1px 2px;' align='left' valign='top'>Suppress</th>
                                <th style='line-height: 24px; font-size: 16px; margin: 0; padding: 12px; border-color: #e2e8f0; border-style: solid; border-width: 1px 1px 2px;' align='left' valign='top'>Trash</th>
                            </tr>
                        </thead>";
        }

        public string MailHeadHTML()
        {
            return @"
                     <style type='text/css'>
                          body,table,td{font-family:Helvetica,Arial,sans-serif !important}.ExternalClass{width:100%}.ExternalClass,.ExternalClass p,.ExternalClass span,.ExternalClass font,.ExternalClass td,.ExternalClass div{line-height:150%}a{text-decoration:none}*{color:inherit}a[x-apple-data-detectors],u+#body a,#MessageViewBody a{color:inherit;text-decoration:none;font-size:inherit;font-family:inherit;font-weight:inherit;line-height:inherit}img{-ms-interpolation-mode:bicubic}table:not([class^=s-]){font-family:Helvetica,Arial,sans-serif;mso-table-lspace:0pt;mso-table-rspace:0pt;border-spacing:0px;border-collapse:collapse}table:not([class^=s-]) td{border-spacing:0px;border-collapse:collapse}@media screen and (max-width: 600px){.w-full,.w-full>tbody>tr>td{width:100% !important}*[class*=s-lg-]>tbody>tr>td{font-size:0 !important;line-height:0 !important;height:0 !important}.s-2>tbody>tr>td{font-size:8px !important;line-height:8px !important;height:8px !important}.s-3>tbody>tr>td{font-size:12px !important;line-height:12px !important;height:12px !important}.s-5>tbody>tr>td{font-size:20px !important;line-height:20px !important;height:20px !important}.s-10>tbody>tr>td{font-size:40px !important;line-height:40px !important;height:40px !important}}
                    </style>
                    <body class='bg-light' style='outline: 0; width: 100%; min-width: 100%; height: 100%; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; font-family: Helvetica, Arial, sans-serif; line-height: 24px; font-weight: normal; font-size: 16px; -moz-box-sizing: border-box; -webkit-box-sizing: border-box; box-sizing: border-box; color: #000000; margin: 0; padding: 0; border-width: 0;' bgcolor='#f7fafc'>
                        <table class='bg-light body' valign='top' role='presentation' border='0' cellpadding='0' cellspacing='0' style='outline: 0; width: 100%; min-width: 100%; height: 100%; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; font-family: Helvetica, Arial, sans-serif; line-height: 24px; font-weight: normal; font-size: 16px; -moz-box-sizing: border-box; -webkit-box-sizing: border-box; box-sizing: border-box; color: #000000; margin: 0; padding: 0; border-width: 0;' bgcolor='#f7fafc'>
                            <tbody>
                                <tr>
                                    <td valign='top' style='line-height: 24px; font-size: 16px; margin: 0;' align='left' bgcolor='#f7fafc'>
                                        <table class='container' role='presentation' border='0' cellpadding='0' cellspacing='0' style='width: 100%;'>
                                            <tbody>
                                                <tr>
                                                    <td align='center' style='line-height: 24px; font-size: 16px; margin: 0; padding: 0 16px;'>
                                                        <table align='center' role='presentation' border='0' cellpadding='0' cellspacing='0' style='width: 100%; max-width: 600px; margin: 0 auto;'>
                                                            <tbody>
                                                                <tr>
                                                                    <td style='line-height: 24px; font-size: 16px; margin: 0;' align='left'>
                                                                        <table class='s-10 w-full' role='presentation' border='0' cellpadding='0' cellspacing='0' style='width: 100%;' width='100%'>
                                                                            <tbody>
                                                                                <tr>
                                                                                    <td style='line-height: 40px; font-size: 40px; width: 100%; height: 40px; margin: 0;' align='left' width='100%' height='40'>
                                                                                        &#160;
                                                                                    </td>
                                                                                </tr>
                                                                            </tbody>
                                                                        </table>
                                                                        <table class='card' role='presentation' border='0' cellpadding='0' cellspacing='0' style='border-radius: 6px; border-collapse: separate !important; width: 100%; overflow: hidden; border: 1px solid #e2e8f0;' bgcolor='#ffffff'>
                                                                            <tbody>
                                                                                <tr>
                                                                                    <td style='line-height: 24px; font-size: 16px; width: 100%; margin: 0;' align='left' bgcolor='#ffffff'>
                                                                                        <table class='card-body' role='presentation' border='0' cellpadding='0' cellspacing='0' style='width: 100%;'>
                                                                                            <tbody>
                                                                                                <tr>
                                                                                                    <td style='line-height: 24px; font-size: 16px; width: 100%; margin: 0; padding: 20px;' align='left'>
                                                                                                        <h1 class='h3' style='padding-top: 0; padding-bottom: 0; font-weight: 500; vertical-align: baseline; font-size: 28px; line-height: 33.6px; margin: 0;' align='left'>Orphaned Tickets Report</h1>
                                                                                                        <table class='s-2 w-full' role='presentation' border='0' cellpadding='0' cellspacing='0' style='width: 100%;' width='100%'>
                                                                                                            <tbody>
                                                                                                                <tr>
                                                                                                                    <td style='line-height: 8px; font-size: 8px; width: 100%; height: 8px; margin: 0;' align='left' width='100%' height='8'>
                                                                                                                        &#160;
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                            </tbody>
                                                                                                        </table>
                                                                                                        <table class='s-5 w-full' role='presentation' border='0' cellpadding='0' cellspacing='0' style='width: 100%;' width='100%'>
                                                                                                            <tbody>
                                                                                                                <tr>
                                                                                                                    <td style='line-height: 20px; font-size: 20px; width: 100%; height: 20px; margin: 0;' align='left' width='100%' height='20'>
                                                                                                                        &#160;
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                            </tbody>
                                                                                                        </table>
                                                                                                        <table class='hr' role='presentation' border='0' cellpadding='0' cellspacing='0' style='width: 100%;'>
                                                                                                            <tbody>
                                                                                                                <tr>
                                                                                                                    <td style='line-height: 24px; font-size: 16px; border-top-width: 1px; border-top-color: #e2e8f0; border-top-style: solid; height: 1px; width: 100%; margin: 0;' align='left'>
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                            </tbody>
                                                                                                        </table>
                                                                                                        <table class='s-5 w-full' role='presentation' border='0' cellpadding='0' cellspacing='0' style='width: 100%;' width='100%'>
                                                                                                            <tbody>
                                                                                                                <tr>
                                                                                                                    <td style='line-height: 20px; font-size: 20px; width: 100%; height: 20px; margin: 0;' align='left' width='100%' height='20'>
                                                                                                                        &#160;
                                                                                                                    </td>
                                                                                                                </tr>
                                                                                                            </tbody>
                                                                                                        </table>";
        }

        public string OrphanMailBody(List<OrphanTicketViewModel> ticketList)
        {
            int count = 0;
            //int orphanAge = 0;
            string mailbody = string.Empty;
            foreach (OrphanTicketViewModel ticket in ticketList)
            {
                //switch (ticket.StatusName)
                //{
                //    case "New Task":
                //        orphanAge = CalculateOrphanAge((int)TicketsStatus.NewTask, ticket.LastActivityDate);
                //        break;

                //    case "In Progress":
                //        orphanAge = CalculateOrphanAge((int)TicketsStatus.InProgress, ticket.LastActivityDate);
                //        break;

                //    case "On Hold":
                //        orphanAge = CalculateOrphanAge((int)TicketsStatus.OnHold, ticket.LastActivityDate);
                //        break;

                //    case "QC":
                //        orphanAge = CalculateOrphanAge((int)TicketsStatus.QC, ticket.LastActivityDate);
                //        break;

                //    case "Assigned":
                //        orphanAge = CalculateOrphanAge((int)TicketsStatus.Assigned, ticket.LastActivityDate);
                //        break;

                //    case "In Review":
                //        orphanAge = CalculateOrphanAge((int)TicketsStatus.InReview, ticket.LastActivityDate);
                //        break;

                //    default:
                //        break;
                //}
                string status = string.Join("", ticket.StatusName.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                if (count % 2 == 0)
                {
                    mailbody += @"<tr>" + tableRow(ticket) + @"</tr>";
                }
                else
                {
                    mailbody += @"<tr bgcolor='#f2f2f2'>" + tableRow(ticket) + @"</tr>";
                }
                count++;
            }
            return mailbody;
        }
    }
}