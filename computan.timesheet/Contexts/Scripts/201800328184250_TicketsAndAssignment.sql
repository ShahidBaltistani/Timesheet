alter table tickets
add fromEmail nvarchar(500) null;
update Tickets
set fromEmail = '';



UPDATE t
SET fromEmail = ti.[from]
FROM Tickets t
JOIN TicketItems ti ON t.id = 
(select top 1 ticketid from TicketItems where t.id = TicketItems.ticketid)
and t.fromEmail = ''

--------------------------------------------------------------------------

insert into [timesheetdb].[dbo].[TicketStatus] values('Assigned',1,'2017-03-20 00:00:00.000','2018-03-20 00:00:00.000','127.0.0.1','d4f5505c-6cd7-4b42-8027-71a0535e5d4f')

alter table tickets
add projectid bigint null;
update Tickets
set projectid = 0;

alter table tickets
add skillid bigint null;
update Tickets
set skillid = 0;

update  [timesheetdb].[dbo].[Tickets]
set statusid = 3
where statusid = 4;
 
update TicketStatus
set name = 'On Hold'
where id = 4

update TicketStatus
set name = 'QA'
where id = 5

update TicketStatus
set name = 'Done'
where id = 3

insert into [timesheetdb].[dbo].[TicketStatus] values('In Review',1,'2017-03-20 00:00:00.000','2018-03-20 00:00:00.000','127.0.0.1','d4f5505c-6cd7-4b42-8027-71a0535e5d4f')

update Tickets
set projectid=ti.projectid,skillid=ti.skillid
from Tickets t, TicketItems ti
where t.id=ti.ticketid and ti.projectid>0 and ti.skillid>0





-------------2018-03-26-------------------------------------------------------
alter table Tickets
add startdate datetime null

alter table Tickets
add enddate datetime null



------------2018-03-27-------------------------------------------------------------------
USE [timesheetdb]

SET QUOTED_IDENTIFIER ON
GO

drop table [dbo].[TicketTeamLogs];

CREATE TABLE [dbo].[TicketTeamLogs](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[ticketid] [bigint] NOT NULL,
	[teamid] [bigint] NOT NULL,
	[assignedbyusersid] [nvarchar](128) NULL,
	[assignedon] [datetime] NOT NULL,
	[statusid] [int] NOT NULL,
	[statusupdatedbyusersid] [nvarchar](128) NULL,
	[statusupdatedon] [datetime] NOT NULL,
	[displayorder] [bigint] NULL,
 CONSTRAINT [PK_dbo.TicketTeamLogs] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TicketTeamLogs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TicketTeamLogs_dbo.Tickets_ticketid] FOREIGN KEY([ticketid])
REFERENCES [dbo].[Tickets] ([id])
GO

ALTER TABLE [dbo].[TicketTeamLogs] CHECK CONSTRAINT [FK_dbo.TicketTeamLogs_dbo.Tickets_ticketid]
GO

ALTER TABLE [dbo].[TicketTeamLogs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TicketTeamLogs_dbo.Teams_teamid] FOREIGN KEY([teamid])
REFERENCES [dbo].[Teams] ([id])
GO

ALTER TABLE [dbo].[TicketTeamLogs] CHECK CONSTRAINT [FK_dbo.TicketTeamLogs_dbo.Teams_teamid]
GO

ALTER TABLE [dbo].[TicketTeamLogs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TicketTeamLogs_dbo.TicketStatus_statusid] FOREIGN KEY([statusid])
REFERENCES [dbo].[TicketStatus] ([id])
GO

ALTER TABLE [dbo].[TicketTeamLogs] CHECK CONSTRAINT [FK_dbo.TicketTeamLogs_dbo.TicketStatus_statusid]
GO

ALTER TABLE [dbo].[TicketTeamLogs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TicketTeamLogs_dbo.Users_assignedbyusersid] FOREIGN KEY([assignedbyusersid])
REFERENCES [dbo].[Users] ([UsersId])
GO

ALTER TABLE [dbo].[TicketTeamLogs] CHECK CONSTRAINT [FK_dbo.TicketTeamLogs_dbo.Users_assignedbyusersid]
GO
