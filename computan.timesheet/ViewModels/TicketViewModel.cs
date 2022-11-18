using computan.timesheet.core;
using System.Collections.Generic;
using System.Linq;

namespace computan.timesheet.Models
{
    public class TicketViewModel : BaseViewModel
    {
        public IQueryable<Ticket> tickets { get; set; }
        public List<TicketItem> ticketitems { get; set; }
        public IQueryable<Team> teams { get; set; }

        public SentItemLog SentItemLog { get; set; }
        public IQueryable<TicketUserFlagged> flaggeditems { get; set; }

        public bool IsFlagged(long? ticketid)
        {
            bool flag = false;
            if (flaggeditems == null)
            {
                return flag;
            }

            if (ticketid == null)
            {
                return flag;
            }

            foreach (TicketUserFlagged item in flaggeditems)
            {
                if (item.ticketid == ticketid)
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }
    }
}