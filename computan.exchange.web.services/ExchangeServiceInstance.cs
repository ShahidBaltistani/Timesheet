using Microsoft.Exchange.WebServices.Data;
using System;
using System.Net;

namespace computan.exchange.web.services
{
    public static class ExchangeServiceInstance
    {
        static ExchangeServiceInstance()
        {
            CertificateCallback.Initialize();
        }

        // The following is a basic redirection validation callback method. It 
        // inspects the redirection URL and only allows the Service object to 
        // follow the redirection link if the URL is using HTTPS. 
        //
        // This redirection URL validation callback provides sufficient security
        // for development and testing of your application. However, it may not
        // provide sufficient security for your deployed application. You should
        // always make sure that the URL validation callback method that you use
        // meets the security requirements of your organization.
        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;

            Uri redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }

            return result;
        }

        public static ExchangeService ConnectToService(IExchangeCredentials exchangeCredentials)
        {
            ExchangeService service = new ExchangeService(exchangeCredentials.Version);
            // Set Credentials
            if (!string.IsNullOrEmpty(exchangeCredentials.Domain))
            {
                service.Credentials = new NetworkCredential(exchangeCredentials.EmailAddress, exchangeCredentials.Password, exchangeCredentials.Domain);
            }
            else
            {
                service.Credentials = new NetworkCredential(exchangeCredentials.EmailAddress, exchangeCredentials.Password);
            }

            if (exchangeCredentials.AutodiscoverUrl == null)
            {
                service.AutodiscoverUrl(exchangeCredentials.EmailAddress, RedirectionUrlValidationCallback);
                exchangeCredentials.AutodiscoverUrl = service.Url;
            }
            else
            {
                service.Url = exchangeCredentials.AutodiscoverUrl;
            }

            return service;
        }

    }
}
