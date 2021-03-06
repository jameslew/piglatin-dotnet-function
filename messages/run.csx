#r "Newtonsoft.Json"
#load "PostDialogExtensions.csx"
#load "EchoDialog.csx"
#load "Utils.csx"

using System;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Text.RegularExpressions;

public class pigLatinBotUserData
{
    public bool isNewUser = true;
    public DateTime lastReadLegalese = DateTime.MinValue;
}

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!"); 

    string jsonContent = await req.Content.ReadAsStringAsync();

    log.Info(jsonContent);

    var message = JsonConvert.DeserializeObject<Activity>(jsonContent);
    MicrosoftAppCredentials.TrustServiceUrl(message.ServiceUrl);
    var connector = new ConnectorClient(new Uri(message.ServiceUrl), new MicrosoftAppCredentials());
    //var sc = new StateClient(new Uri(message.ChannelId == "emulator" ? message.ServiceUrl : "https://intercom-api-scratch.azurewebsites.net"), new MicrosoftAppCredentials());
    var sc = message.GetStateClient();
    BotState botState = new BotState(sc);
    DateTime lastModifiedPolicies = DateTime.Parse("2015-10-01");
    botTests tests = new botTests();

    if (message != null)
    {
        log.Info(string.Format("ActivityType {0}, Activity.Text {1}", message.GetActivityType(), message.Text));
        // one of these will have an interface and process it again
        switch (message.GetActivityType())
        {
            case ActivityTypes.Message:
                
                if (message.Text.ToLower().Contains("message types test"))
                { 
                    var mtResult = await tests.messageTypesTest((Activity) message, connector, sc); 
                    await connector.Conversations.ReplyToActivityAsync(mtResult);
                }
                else if (message.Text.ToLower().Contains("data types test"))
                {
                    var dtResult = await tests.dataTypesTest((Activity) message, connector, sc);
                    await connector.Conversations.ReplyToActivityAsync(dtResult);
                }
                else if (message.Text.ToLower().Contains("card types test"))
                {
                    var ctResult = await tests.cardTypesTest((Activity) message, connector);
                    await connector.Conversations.ReplyToActivityAsync(ctResult);
                }
                else if (message.Text.ToLower().Contains("slack blocks test"))
                {
                    log.Info("Slack Blocks Test Begin");
                    var msgReply = message.CreateReply(translateToPigLatin('Populating channelData'));
                    msgReply.channelData = '{"blocks":[{"text":{"type":"section","text":{"type":"mrkdwn","text":"Are you using a Mac or PC?"}}},{"attachments":{"type":"actions","elements":[{"type":"button","text":{"type":"plain_text","emoji":true,"text":"I'm using a Mac"},"value":"I'm using a Mac"},{"type":"button","text":{"type":"plain_text","emoji":true,"text":"I'm using Windows"},"value":"I'm using Windows"}]}}]}';
                    await connector.Conversations.SendToConversationAsync(msgReply);
                    log.Info("Slack Blocks Test End);
                }
                else
                {
                    log.Info("Processing Simple Message");
                    var msgReply = message.CreateReply(translateToPigLatin(message.Text));
                    await connector.Conversations.SendToConversationAsync(msgReply);
                    log.Info("Completed sending Simple Message");
                }
                break;

            case ActivityTypes.ConversationUpdate:
                foreach(ChannelAccount added in message.MembersAdded)
                {
                    Activity addedMessage = message.CreateReply();

                    bool needToSendWelcomeText = false;
                    pigLatinBotUserData addedUserData = new pigLatinBotUserData();
                    BotData botData = new BotData();

                    // is the added member me?
                    if (added.Id == message.Recipient.Id)
                    {
                        addedMessage.Text = string.Format(translateToPigLatin("Hey there, I'm PigLatinBot. I make intelligible text unintelligible.  Ask me how by typing 'Help', and for terms and info, click ") + "[erehay](http://www.piglatinbot.com)", added.Name);
                        var reply = await connector.Conversations.ReplyToActivityAsync(addedMessage);
                        continue;
                    }
                    
                    // okay, check for real users
                    try
                    {
                        botData = (BotData) await botState.GetUserDataAsync(message.ChannelId, added.Id);
                    }
                    catch (Exception e)
                    {
                        if (e.Message == "Resource not found")
                        { }
                        else
                            throw e;
                    }

                    if(botData == null)
                        botData = new BotData(eTag: "*");

                    addedUserData = botData.GetProperty<pigLatinBotUserData>("v1") ?? new pigLatinBotUserData();

                    if (addedUserData.isNewUser == true)
                    {
                        addedUserData.isNewUser = false;
                        needToSendWelcomeText = true;
                    }

                    if (addedUserData.lastReadLegalese < lastModifiedPolicies)
                    {
                        addedUserData.lastReadLegalese = DateTime.UtcNow;
                        needToSendWelcomeText = true;
                    }
                    if (needToSendWelcomeText)
                    {
                        addedMessage.Text = string.Format(translateToPigLatin("Welcome to the chat") + " {0}, " + translateToPigLatin("I'm PigLatinBot. I make intelligible text unintelligible.  Ask me how by typing 'Help', and for terms and info, click ") + "[erehay](http://www.piglatinbot.com)", added.Name);
                        addedMessage.Recipient = added;
                        addedMessage.Conversation = null;

                        try
                        {
                            botData.SetProperty("v1", addedUserData);
                            var response = await botState.SetUserDataAsync(message.ChannelId, added.Id, botData);
                        }
                        catch (Exception e)
                        {
                            log.Info(e.Message);
                        }
                        
                        var ConversationId = await connector.Conversations.CreateDirectConversationAsync(message.Recipient, message.From);
                        addedMessage.Conversation = new ConversationAccount(id: ConversationId.Id);
                        var reply = await connector.Conversations.SendToConversationAsync(addedMessage);
                    }
                }

                //maybe someone got removed
                if (message.MembersRemoved != null)
                {
                    foreach (ChannelAccount removed in message.MembersRemoved)
                    {
                        Activity removedMessage = message.CreateReply();
                        removedMessage.Locale = "en";

                        removedMessage.Text = string.Format("{0}", removed.Name) + translateToPigLatin(" has Left the building");
                        var reply = await connector.Conversations.ReplyToActivityAsync(removedMessage);
                    }
                }
                break;

            case ActivityTypes.ContactRelationUpdate:
            case ActivityTypes.Typing:
            case ActivityTypes.DeleteUserData:
                //In this case the DeleteUserData message comes from the user so we can clear the data and set it back directly
                BotData currentBotData = (BotData) await botState.GetUserDataAsync(message.ChannelId, message.From.Id);
                pigLatinBotUserData deleteUserData = new pigLatinBotUserData();
                currentBotData.SetProperty("v1", deleteUserData);
                await botState.SetUserDataAsync(message.ChannelId, message.From.Id, currentBotData);
                
                Activity replyMessage = message.CreateReply();
                replyMessage.Text = translateToPigLatin("I have deleted your data oh masterful one");
                log.Info("Clearing user's BotUserData");
                return replyMessage;

            case ActivityTypes.Ping:
            default:
                log.Error($"Unknown activity type ignored: {message.GetActivityType()}");
                break;
        }
    }
    return req.CreateResponse(HttpStatusCode.Accepted);    
}
