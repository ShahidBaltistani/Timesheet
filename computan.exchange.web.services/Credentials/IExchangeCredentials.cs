using Microsoft.Exchange.WebServices.Data;
using System;
using System.Security;

namespace computan.exchange.web.services
{
    public interface IExchangeCredentials
    {
        ExchangeVersion Version { get; }
        string EmailAddress { get; }
        SecureString Password { get; }
        string Domain { get; }
        Uri AutodiscoverUrl { get; set; }
    }
}
