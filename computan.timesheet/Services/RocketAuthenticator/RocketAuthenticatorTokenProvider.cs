using Base32;
using computan.timesheet.Contexts;
using computan.timesheet.core;
using computan.timesheet.Models;
using computan.timesheet.Models.OrphanTickets;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using OtpSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.SqlServer.Utilities;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace computan.timesheet.Services.RocketAuth
{
    //
    // Summary:
    //     TokenProvider that generates tokens from the user's security stamp and notifies
    //     a user via their phone number
    //
    // Type parameters:
    //   TUser:
    public class RocketAuthenticatorTokenProvider<TUser, TKey> : TotpSecurityStampBasedTokenProvider<TUser, TKey> where TUser : class, IUser<TKey> where TKey : IEquatable<TKey>
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        private string _body;

        //
        // Summary:
        //     Message contents which should contain a format string which the token will be
        //     the only argument
        public string MessageFormat
        {
            get
            {
                return _body ?? "{0}";
            }
            set
            {
                _body = value;
            }
        }

        public override Task NotifyAsync(string token, UserManager<TUser, TKey> manager, TUser user)
        {
            string username ="@"+ user.UserName.Split('@')[0];
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            OrphanWebhookViewModel t = new OrphanWebhookViewModel()
            {
                text = string.Format(CultureInfo.CurrentCulture, MessageFormat, new object[1] { token }),
                channel = username
            };

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            var myContent = JsonConvert.SerializeObject(t);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/Json");
            var responseTask = client.PostAsync(ConfigurationManager.AppSettings["TwoFA"].ToString(), byteContent);
            responseTask.Wait();
            return Task.FromResult(0);
        }
        public override async Task<bool> IsValidProviderForUserAsync(UserManager<TUser, TKey> manager, TUser user)
        {
            var users = await manager.GetEmailAsync(user.Id).WithCurrentCulture();
            var data = db.Users.Where(x=>x.Email == users).FirstOrDefault();
            return data.IsRocketAuthenticatorEnabled;
        }


    }
    // Summary:
    //     TokenProvider that generates tokens from the user's security stamp and notifies
    //     a user via their phone number
    //
    // Type parameters:
    //   TUser:
    public class RocketAuthenticatorTokenProvider<TUser> : RocketAuthenticatorTokenProvider<TUser, string> where TUser : class, IUser<string>
    {
    }

}