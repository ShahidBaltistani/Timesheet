using Microsoft.Exchange.WebServices.Data;
using System;
using System.Configuration;
using System.Security;

namespace computan.exchange.web.services
{
    public class ExchangeCredentialsCustom : IExchangeCredentials
    {
        private ExchangeCredentialsCustom ExchangeCredentials = null;

        public IExchangeCredentials GetExchangeCredentials(string Email, string Password)
        {
            try
            {
                if (string.IsNullOrEmpty(Email))
                {
                    throw new ArgumentException("Email address is required to connect to MS Exchange.");
                }

                if (string.IsNullOrEmpty(Password))
                {
                    throw new ArgumentException("Password is required to connect to MS Exchange.");
                }

                if (ExchangeCredentials == null)
                {
                    ExchangeCredentials = new ExchangeCredentialsCustom
                    {

                        // Fetch Email Address.
                        EmailAddress = Email,

                        // Fetch Password.
                        Password = new SecureString()
                    };
                    foreach (char c in Password)
                    {
                        ExchangeCredentials.Password.AppendChar(c);
                    }
                    ExchangeCredentials.Password.MakeReadOnly();

                    // Fetch Domain.
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Domain"]))
                    {
                        ExchangeCredentials.Domain = ConfigurationManager.AppSettings["Domain"];
                    }
                    else
                    {
                        ExchangeCredentials.Domain = "computan";
                    }

                    // Fetch Auto Discover Url.
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AutodiscoverUrl"]))
                    {
                        ExchangeCredentials.AutodiscoverUrl = new Uri(ConfigurationManager.AppSettings["AutodiscoverUrl"].ToString());
                    }
                }
                return ExchangeCredentials;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ExchangeVersion Version
        {
            get
            {
                // If Application Setting for Exchange Version is missing, then by default send Exchange2010_SP1
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["ExchangeVersion"]))
                {
                    return ExchangeVersion.Exchange2010_SP1;
                }

                // Return appropriate version of the exchange set by the user.
                switch (ConfigurationManager.AppSettings["ExchangeVersion"].ToString())
                {
                    case "Exchange2007_SP1":
                        return ExchangeVersion.Exchange2007_SP1;
                    case "Exchange2010":
                        return ExchangeVersion.Exchange2010;
                    case "Exchange2010_SP1":
                        return ExchangeVersion.Exchange2010_SP1;
                    case "Exchange2010_SP2":
                        return ExchangeVersion.Exchange2010_SP2;
                    case "Exchange2013":
                        return ExchangeVersion.Exchange2013;
                    case "Exchange2013_SP1":
                        return ExchangeVersion.Exchange2013_SP1;
                    default:
                        return ExchangeVersion.Exchange2010_SP1;
                }
            }
        }

        public string EmailAddress
        {
            get;
            private set;
        }

        public SecureString Password
        {
            get;
            private set;
        }

        public string Domain
        {
            get;
            private set;
        }

        public Uri AutodiscoverUrl
        {
            get;
            set;
        }
    }
}
