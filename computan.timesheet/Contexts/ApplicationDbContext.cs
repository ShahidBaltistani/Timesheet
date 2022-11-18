using computan.timesheet.core;
using computan.timesheet.core.OrphanTickets;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace computan.timesheet.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        static ApplicationDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            Database.SetInitializer<ApplicationDbContext>(null);
        }

        public ApplicationDbContext()
            : base("DefaultConnection", false)
        {
        }

        // Lookups
        public DbSet<Country> Country { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<FlagStatus> FlagStatus { get; set; }
        public DbSet<TicketStatus> TicketStatus { get; set; }
        public DbSet<ConversationStatus> ConversationStatus { get; set; }

        public DbSet<TicketPriorty> TicketPriorty { get; set; }

        // Entities
        public DbSet<Client> Client { get; set; }
        public DbSet<ClientContact> ClientContact { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<Skill> Skill { get; set; }
        public DbSet<Team> Team { get; set; }
        public DbSet<TeamMember> TeamMember { get; set; }
        public DbSet<TicketItemAttachment> TicketItemAttachment { get; set; }

        public DbSet<TimeEntryLogs> TimeEntryLogs { get; set; }

        // Tickets
        public virtual DbSet<Ticket> Ticket { get; set; }
        public DbSet<TicketItem> TicketItem { get; set; }
        public virtual DbSet<TicketItemLog> TicketItemLog { get; set; }
        public DbSet<TicketTimeLog> TicketTimeLog { get; set; }
        public DbSet<TicketEstimateTimeLog> TicketEstimateTimeLog { get; set; }
        public DbSet<TicketTeamLogs> TicketTeamLogs { get; set; }
        public DbSet<Billing> Billing { get; set; }
        public DbSet<MyExceptions> MyExceptions { get; set; }
        public DbSet<UserSkills> UserSkills { get; set; }
        public DbSet<CredentialSkills> CredentialSkills { get; set; }
        public DbSet<TeamWorkTimeLog> TeamWorkTimeLog { get; set; }

        public DbSet<TicketReplay> TicketReplay { get; set; }

        public DbSet<Credentials> Credentials { get; set; }

        public DbSet<CredentialCategory> CredentialCategories { get; set; }

        public DbSet<CredentialLevel> CredentialLevels { get; set; }

        public DbSet<CredentialType> CredentialTypes { get; set; }

        public DbSet<TicketType> TicketTypes { get; set; }

        // Ticket Comment/Files
        public DbSet<TicketComment> TicketComment { get; set; }
        public DbSet<TicketCommentUserRead> TicketCommentUserRead { get; set; }
        public DbSet<TicketFile> TicketFile { get; set; }
        public DbSet<TicketUserFlagged> TicketUserFlagged { get; set; }

        // Rules Tables.
        public DbSet<RuleConditionType> RuleConditionType { get; set; }
        public DbSet<RuleActionType> RuleActionType { get; set; }
        public DbSet<RuleExceptionType> RuleExceptionType { get; set; }

        public DbSet<Rule> Rule { get; set; }
        public DbSet<RuleCondition> RuleCondition { get; set; }
        public DbSet<RuleAction> RuleAction { get; set; }
        public DbSet<RuleException> RuleException { get; set; }

        // Billing Entities
        public DbSet<ClientType> ClientType { get; set; }
        public DbSet<BillingCycleType> BillingCyleType { get; set; }
        public DbSet<ClientBillingInvoice> ClientBillingInvoice { get; set; }
        public DbSet<ClientBillingCycle> ClientBillingCycle { get; set; }
        public DbSet<BillingNotificationType> BillingNotificationType { get; set; }
        public DbSet<BillingNotification> BillingNotification { get; set; }
        public DbSet<NotificationLimitForBilling> NotificationLimitForBilling { get; set; }
        public DbSet<EmailTemplate> EmailTemplate { get; set; }


        // Projecy Files
        public DbSet<ProjectFiles> ProjectFiles { get; set; }

        // Project Notes

        public DbSet<ProjectNotes> ProjectNotes { get; set; }

        // User Favourites
        public DbSet<UserFavouriteType> UserFavouriteType { get; set; }
        public DbSet<UserFavourite> UserFavourite { get; set; }

        //Contacts

        public DbSet<Contact> Contact { get; set; }
        public DbSet<ContactCompany> ContactCompany { get; set; }

        // Notification Model
        public DbSet<NotificationEntity> NotificationEntity { get; set; }
        public DbSet<NotificationEntityAction> NotificationAction { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<NotificationUsers> NotificationUsers { get; set; }
        public DbSet<UserBrowserinfo> UserBrowserinfo { get; set; }

        // Ticket Log Types
        public DbSet<ActionType> ActionTypes { get; set; }
        public DbSet<TicketLogs> TicketLogs { get; set; }

        public DbSet<TicketMergeLog> TicketMergeLog { get; set; }

        //Customer Source
        public DbSet<CustomerSource> CustomerSource { get; set; }
        public DbSet<SentItemLog> SentItemLog { get; set; }

        public DbSet<NextLink> NextLink { get; set; }

        // Integration freedcamp
        public DbSet<Integration> integration { get; set; }
        public DbSet<FreedcampProject> FreedcampProject { get; set; }
        public DbSet<FreedCampTask> freedCampTask { get; set; }
        public DbSet<FreedcampComment> freedcampComment { get; set; }
        public DbSet<JobQueue> jobQueue { get; set; }

        //Orphaned Tickets
        public DbSet<SubscribeTeam> SubscribeTeams { get; set; }
        public DbSet<SuppressTicket> SuppressTickets { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<IdentityRole>().ToTable("Roles").Property(p => p.Id).HasColumnName("RolesId");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims").Property(p => p.Id)
                .HasColumnName("UserClaimsId");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<ApplicationUser>().ToTable("Users").Property(p => p.Id).HasColumnName("UsersId");
        }
    }
}