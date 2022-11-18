using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace computan.exchange.web.services
{
    public static class ExchangeManager
    {
        public static ICollection<Conversation> GetConversations(ExchangeService service)
        {
            try
            {
                FolderId inbox = new FolderId(WellKnownFolderName.Inbox);
                ConversationIndexedItemView iv = new ConversationIndexedItemView(10);
                iv.OrderBy.Add(ConversationSchema.LastDeliveryTime, SortDirection.Ascending);

                ICollection<Conversation> items = service.FindConversation(iv, inbox);
                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Item FindConversationItem(ExchangeService service, ItemId id)
        {
            // Create a search collection that contains your search conditions.
            List<SearchFilter> searchFilterCollection = new List<SearchFilter>
            {
                new SearchFilter.IsEqualTo(ItemSchema.Id, id.UniqueId)
            };

            // Create the search filter with a logical operator and your search parameters.
            SearchFilter searchFilter = new SearchFilter.SearchFilterCollection(LogicalOperator.Or, searchFilterCollection.ToArray());

            // Limit the view to 5 items.
            ItemView view = new ItemView(5);

            // Limit the property set to the property ID for the base property set, and the subject and item class for the additional properties to retrieve.
            PropertySet properties = new PropertySet(BasePropertySet.IdOnly,
                                                  ItemSchema.ConversationId,
                                                  ItemSchema.DateTimeCreated,
                                                  ItemSchema.DateTimeReceived,
                                                  ItemSchema.DateTimeSent,
                                                  ItemSchema.DisplayCc,
                                                  ItemSchema.DisplayTo,
                                                  ItemSchema.HasAttachments,
                                                  ItemSchema.Id,
                                                  ItemSchema.Importance,
                                                  ItemSchema.InReplyTo,
                                                  ItemSchema.IsAssociated,
                                                  ItemSchema.IsDraft,
                                                  ItemSchema.IsFromMe,
                                                  ItemSchema.IsReminderSet,
                                                  ItemSchema.IsResend,
                                                  ItemSchema.IsSubmitted,
                                                  ItemSchema.IsUnmodified,
                                                  ItemSchema.LastModifiedName,
                                                  ItemSchema.LastModifiedTime,
                                                  ItemSchema.ParentFolderId,
                                                  ItemSchema.Sensitivity,
                                                  ItemSchema.Size,
                                                  ItemSchema.Subject);
            view.PropertySet = properties;

            // Setting the traversal to shallow will return all non-soft-deleted items in the specified folder.
            view.Traversal = ItemTraversal.Shallow;

            // Send the request to search the Inbox and get the results.
            FindItemsResults<Item> findResults = service.FindItems(WellKnownFolderName.Inbox, searchFilter, view);

            return (findResults != null ? findResults.ToList()[0] : null);
        }

        public static List<EmailMessage> BatchGetEmailItems(ExchangeService service, List<ItemId> itemIds)
        {

            // Create a property set that limits the properties returned by the Bind method to only those that are required.
            PropertySet propSet = new PropertySet(BasePropertySet.IdOnly,
                                                    EmailMessageSchema.Attachments,
                                                    EmailMessageSchema.BccRecipients,
                                                    EmailMessageSchema.CcRecipients,
                                                    EmailMessageSchema.Body,
                                                    EmailMessageSchema.ConversationId,
                                                    EmailMessageSchema.ConversationIndex,
                                                    EmailMessageSchema.ConversationTopic,
                                                    EmailMessageSchema.DateTimeCreated,
                                                    EmailMessageSchema.DateTimeReceived,
                                                    EmailMessageSchema.DateTimeSent,
                                                    EmailMessageSchema.DisplayCc,
                                                    EmailMessageSchema.DisplayTo,
                                                    EmailMessageSchema.From,
                                                    EmailMessageSchema.HasAttachments,
                                                    EmailMessageSchema.Importance,
                                                    EmailMessageSchema.InReplyTo,
                                                    EmailMessageSchema.InternetMessageHeaders,
                                                    EmailMessageSchema.InternetMessageId,
                                                    EmailMessageSchema.LastModifiedName,
                                                    EmailMessageSchema.LastModifiedTime,
                                                    EmailMessageSchema.MimeContent,
                                                    EmailMessageSchema.ReplyTo,
                                                    EmailMessageSchema.Sensitivity,
                                                    EmailMessageSchema.Size,
                                                    EmailMessageSchema.Subject,
                                                    EmailMessageSchema.ToRecipients,
                                                    EmailMessageSchema.UniqueBody);

            // Get the items from the server.
            // This method call results in a GetItem call to EWS.
            ServiceResponseCollection<GetItemResponse> response = service.BindToItems(itemIds, propSet);

            // Instantiate a collection of EmailMessage objects to populate from the values that are returned by the Exchange server.
            List<EmailMessage> messageItems = new List<EmailMessage>();


            foreach (GetItemResponse getItemResponse in response)
            {
                try
                {
                    Item item = getItemResponse.Item;
                    EmailMessage message = (EmailMessage)item;
                    messageItems.Add(message);
                    // Print out confirmation and the last eight characters of the item ID.
                    Console.WriteLine("Found item {0}.", message.Id.ToString().Substring(144));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception while getting a message: {0}", ex.Message);
                }
            }

            // Check for success of the BindToItems method call.
            if (response.OverallResult == ServiceResult.Success)
            {
                Console.WriteLine("All email messages retrieved successfully.");
                Console.WriteLine("\r\n");
            }

            return messageItems;
        }

        public static List<Item> BatchGetItems(ExchangeService service, List<ItemId> itemIds)
        {

            // Create a property set that limits the properties returned by the Bind method to only those that are required.
            PropertySet propSet = new PropertySet(BasePropertySet.IdOnly,
                                                    EmailMessageSchema.Attachments,
                                                    EmailMessageSchema.BccRecipients,
                                                    EmailMessageSchema.Body,
                                                    EmailMessageSchema.ConversationId,
                                                    EmailMessageSchema.ConversationIndex,
                                                    EmailMessageSchema.ConversationTopic,
                                                    EmailMessageSchema.DateTimeCreated,
                                                    EmailMessageSchema.DateTimeReceived,
                                                    EmailMessageSchema.DateTimeSent,
                                                    EmailMessageSchema.DisplayCc,
                                                    EmailMessageSchema.DisplayTo,
                                                    EmailMessageSchema.From,
                                                    EmailMessageSchema.HasAttachments,
                                                    EmailMessageSchema.Importance,
                                                    EmailMessageSchema.InReplyTo,
                                                    EmailMessageSchema.InternetMessageHeaders,
                                                    EmailMessageSchema.InternetMessageId,
                                                    EmailMessageSchema.LastModifiedName,
                                                    EmailMessageSchema.LastModifiedTime,
                                                    EmailMessageSchema.MimeContent,
                                                    EmailMessageSchema.ReplyTo,
                                                    EmailMessageSchema.Sensitivity,
                                                    EmailMessageSchema.Size,
                                                    EmailMessageSchema.Subject,
                                                    EmailMessageSchema.ToRecipients,
                                                    EmailMessageSchema.UniqueBody);

            // Get the items from the server.
            // This method call results in a GetItem call to EWS.
            ServiceResponseCollection<GetItemResponse> response = service.BindToItems(itemIds, propSet);

            // Instantiate a collection of EmailMessage objects to populate from the values that are returned by the Exchange server.
            List<Item> messageItems = new List<Item>();


            foreach (GetItemResponse getItemResponse in response)
            {
                try
                {
                    messageItems.Add(getItemResponse.Item);
                    // Print out confirmation and the last eight characters of the item ID.
                    Console.WriteLine("Found item {0}.", getItemResponse.Item.Id.ToString().Substring(144));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception while getting a message: {0}", ex.Message);
                }
            }

            // Check for success of the BindToItems method call.
            if (response.OverallResult == ServiceResult.Success)
            {
                Console.WriteLine("All email messages retrieved successfully.");
                Console.WriteLine("\r\n");
            }

            return messageItems;
        }

        public static void DeleteEmail(ExchangeService service, WellKnownFolderName ParentFolder, string SearchCriteria)
        {
            // Find items in the specified folder (ParentFolder) that match the specified search criteria (SearchCriteria).
            // This example assumes that the search response returns exactly 3 results.
            FindItemsResults<Item> results = service.FindItems(ParentFolder, SearchCriteria, new ItemView(3));

            if (results.TotalCount != 3)
            {
                Console.WriteLine();
                Console.WriteLine("Found " + results.TotalCount.ToString() + " items in the " + ParentFolder + " folder that match the specified search criteria (" + SearchCriteria + ").");
                Console.WriteLine();
                Console.WriteLine("For this example to work as designed, update the search criteria so that exactly 3 items in the " + ParentFolder + " folder match the specified search criteria.");
                return;
            }
            else
            {
                Console.WriteLine("Found exactly 3 items in the " + ParentFolder + " folder that match the search criteria (" + SearchCriteria + ").");
                Console.WriteLine();
            }

            // In all versions of Exchange, there are 3 methods of deleting an item:  1) hard delete, 2) move to deleted items, and 3) soft delete.
            // The result of each method, however, differs depending on the (major) version of Exchange Server for the mailbox where the item exists.

            // Perform deletions against an Exchange Server 2007 mailbox. 
            if (service.ServerInfo.MajorVersion == 12)
            {
                Console.WriteLine("Major server version is Exchange Server 2007.");
                Console.WriteLine();

                // Using the search results, hard delete the first item.
                // In Exchange 2007, an item that is deleted by using DeleteMode.HardDelete is permanently deleted from the store. 
                results.Items[0].Delete(DeleteMode.HardDelete);

                // Using the search results, move the second item to the Deleted Items folder.
                results.Items[1].Delete(DeleteMode.MoveToDeletedItems);

                // Using the search results, soft delete the third item.
                // In Exchange 2007, an item that is deleted by using DeleteMode.SoftDelete continues to exist in the same folder,
                // but can only be found by performing a FindItem operation with a soft-delete traversal.
                // The item cannot be moved, copied, or restored by using Exchange Web Services.
                results.Items[2].Delete(DeleteMode.SoftDelete);

                Console.WriteLine("3 items that matched the search criteria (" + SearchCriteria + ") were deleted from the " + ParentFolder + " folder.");
            }

            // Perform deletions against an Exchange Server 2010 mailbox.
            if (service.ServerInfo.MajorVersion == 14)
            {
                Console.WriteLine("Major server version is Exchange Server 2010.");
                Console.WriteLine();

                // Using the search results, hard delete the first item.
                // In Exchange 2010, an item that is deleted by using DeleteMode.HardDelete is moved to the Recoverable Items\Purges folder.
                results.Items[0].Delete(DeleteMode.HardDelete);

                // Using the search results, move the second item to the Deleted Items folder.
                results.Items[1].Delete(DeleteMode.MoveToDeletedItems);

                // Using the search results, soft delete the third item.
                // In Exchange 2010, an item that is deleted by using DeleteMode.SoftDelete is moved to the Recoverable Items\Deletions folder.
                results.Items[2].Delete(DeleteMode.SoftDelete);

                Console.WriteLine("3 items that matched the search criteria (" + SearchCriteria + ") were deleted from the " + ParentFolder + " folder.");
            }
        }

        #region Appointments and Meetings

        public static bool CreateAppointment(ExchangeService service, string Subject, string Body, DateTime Start, DateTime End, string Location, List<string> RequiredAttendees, List<string> OptionalAttendees)
        {
            Appointment appointment = new Appointment(service)
            {
                Subject = Subject,
                Body = Body,
                Start = Start,
                End = End,
                Location = Location
            };

            if (RequiredAttendees == null && OptionalAttendees == null)
            {
                appointment.Save(SendInvitationsMode.SendToNone);
            }
            else
            {
                if (RequiredAttendees != null)
                {
                    foreach (string attendee in RequiredAttendees)
                    {
                        appointment.RequiredAttendees.Add(attendee);
                    }
                }

                if (OptionalAttendees != null)
                {
                    foreach (string attendee in OptionalAttendees)
                    {
                        appointment.OptionalAttendees.Add(attendee);
                    }
                }
            }
            return true;
        }

        public static bool DeleteAppointment(ExchangeService service, string ItemId)
        {
            Appointment appointment = Appointment.Bind(service, new ItemId(ItemId));
            appointment.Delete(DeleteMode.MoveToDeletedItems);
            return true;
        }

        public static bool RespondToInvitation(ExchangeService service, string ItemId, bool IsAttending)
        {
            Appointment appointment = Appointment.Bind(service, new Microsoft.Exchange.WebServices.Data.ItemId(ItemId));
            appointment.Accept(IsAttending);
            return true;
        }
        public static bool DeleteMeetingRequest(ExchangeService service, string ItemId)
        {
            MeetingRequest meetingRequest = MeetingRequest.Bind(service, new ItemId(ItemId));
            meetingRequest.Delete(DeleteMode.MoveToDeletedItems);
            return true;
        }

        #endregion

    }
}
