using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace computan.timesheet.Models
{
    public class GraphMessage
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string context { get; set; }

        [JsonProperty(PropertyName = "@odata.nextLink")]
        public string nextLink { get; set; }

        [JsonProperty(PropertyName = "@odata.deltaLink")]
        public string deltaLink { get; set; }

        public IList<message> value { get; set; }
    }

    public class InternetMessageHeader
    {
        public string name { get; set; }
        public string value { get; set; }
    }


    public class GraphAttachment
    {
        [JsonProperty(PropertyName = "@odata.type")]
        public string odatatype { get; set; }

        public string id { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public string name { get; set; }
        public string contentType { get; set; }
        public int size { get; set; }
        public bool isInline { get; set; }
        public string contentId { get; set; }
        public object contentLocation { get; set; }
        public string contentBytes { get; set; }
    }

    public class GraphAttachments
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string odatacontext { get; set; }

        public IList<GraphAttachment> value { get; set; }
    }

    public class message
    {
        [JsonProperty(PropertyName = "@odata.type")]
        public string type { get; set; }

        [JsonProperty(PropertyName = "@odata.etag")]
        public string etag { get; set; }

        public DateTime createdDateTime { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public string changeKey { get; set; }
        public IList<object> categories { get; set; }
        public DateTime receivedDateTime { get; set; }
        public DateTime sentDateTime { get; set; }
        public bool hasAttachments { get; set; }
        public string internetMessageId { get; set; }
        public string subject { get; set; }
        public string bodyPreview { get; set; }
        public string importance { get; set; }
        public string parentFolderId { get; set; }
        public string conversationId { get; set; }
        public object isDeliveryReceiptRequested { get; set; }
        public bool isReadReceiptRequested { get; set; }
        public bool isRead { get; set; }
        public IList<InternetMessageHeader> internetMessageHeaders { get; set; }
        public bool isDraft { get; set; }
        public string webLink { get; set; }
        public string inferenceClassification { get; set; }
        public string id { get; set; }
        public Body body { get; set; }
        public Sender sender { get; set; }
        public From from { get; set; }
        public IList<ToRecipient> toRecipients { get; set; }
        public IList<CcRecipient> ccRecipients { get; set; }
        public IList<bccRecipients> bccRecipients { get; set; }
        public IList<replyTo> replyTo { get; set; }
        public Flag flag { get; set; }

        public IList<GraphAttachment> GraphAttachment { get; set; }
    }

    public class Body
    {
        public string contentType { get; set; }
        public string content { get; set; }
    }

    public class EmailAddress
    {
        public string name { get; set; }
        public string address { get; set; }
    }

    public class Sender
    {
        public EmailAddress emailAddress { get; set; }
    }

    public class From
    {
        public EmailAddress emailAddress { get; set; }
    }

    public class ToRecipient
    {
        public EmailAddress emailAddress { get; set; }
    }

    public class CcRecipient
    {
        public EmailAddress emailAddress { get; set; }
    }

    public class bccRecipients
    {
        public EmailAddress emailAddress { get; set; }
    }

    public class replyTo
    {
        public EmailAddress emailAddress { get; set; }
    }

    public class Flag
    {
        public string flagStatus { get; set; }
    }
}