#load "Utils.csx"

using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System.Diagnostics;
using Autofac;
using System.Text;
using Microsoft.Rest;

[Serializable]
public class EchoDialog : IDialog<object>
{
    protected int count = 1;

    public Task StartAsync(IDialogContext context)
    {
        try
        {
            context.Wait(MessageReceivedAsync);
        }
        catch (OperationCanceledException error)
        {
            return Task.FromCanceled(error.CancellationToken);
        }
        catch (Exception error)
        {
            return Task.FromException(error);
        }

        return Task.CompletedTask;
    }

    public virtual async Task MessageReceivedAsync(IDialogContext dlgCtxt, IAwaitable<IMessageActivity> argument)
    {
        var message = await argument;

        //var connector = new ConnectorClient(new Uri("https://intercomScratch.azure-api.net"), new Microsoft.Bot.Connector.MicrosoftAppCredentials());
        //var sc = new StateClient(new Uri(message.ChannelId == "emulator" ? message.ServiceUrl : "https://intercom-api-scratch.azurewebsites.net"), new MicrosoftAppCredentials());

    }

    private async Task<Activity> messageTypesTest(Activity message, ConnectorClient connector, StateClient sc)
    {

        StringBuilder sb = new StringBuilder();
        var botState = new BotState(sc);
        // DM a user 
        var newDirectToUser = new Activity()
        {
            Text = "Should go directly to user",
            Type = "message",
            From = message.Recipient,
            Recipient = message.From,
            ChannelId = message.ChannelId
        };

        var ConversationId = await connector.Conversations.CreateDirectConversationAsync(message.Recipient, message.From);
        newDirectToUser.Conversation = new ConversationAccount(id: ConversationId.Id);
        var reply = await connector.Conversations.SendToConversationAsync(newDirectToUser);
        if (reply != null)
            sb.AppendLine(reply.Message);


        // message to conversation not directed to user using CreateReply
        Activity replyToConversation = message.CreateReply("Should go to conversation, but does not address the user that generated it");
        var bcReply = await connector.Conversations.SendToConversationAsync(replyToConversation);
        if(bcReply != null)
            sb.AppendLine(bcReply.Message);

        // reply to to user using CreateReply
        Activity replyToConversation = message.CreateReply("Should go to conversation, but addressing the user that generated it");
        replyToConversation.Recipient = message.From;
        var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
        if (reply != null)
            sb.AppendLine(reply.Message);

        return message.CreateReply(translateToPigLatin("Completed MessageTypesTest"));
    }

    private async Task<Activity> cardTypesTest(Activity message, ConnectorClient connector)
    {

        StringBuilder sb = new StringBuilder();

        // reply to to everyone with a PigLatin Hero Card
        Activity replyToConversation = message.CreateReply(translateToPigLatin("Should go to conversation, with a fancy schmancy hero card"));
        replyToConversation.Recipient = message.From;
        replyToConversation.Type = "message";
        replyToConversation.Attachments = new List<Attachment>();

        List<CardImage> cardImages = new List<CardImage>();
        cardImages.Add(new CardImage(url: "https://3.bp.blogspot.com/-7zDiZVD5kAk/T47LSvDM_jI/AAAAAAAAByM/AUhkdynaJ1Y/s200/i-speak-pig-latin.png"));
        cardImages.Add(new CardImage(url: "https://2.bp.blogspot.com/-Ab3oCVhOBjI/Ti23EzV3WCI/AAAAAAAAB1o/tiTeBslO6iU/s1600/bacon.jpg"));

        List<CardAction> cardButtons = new List<CardAction>();

        CardAction plButton = new CardAction()
        {
            Value = "https://en.wikipedia.org/wiki/Pig_Latin",
            Type = "openUrl",
            Title = "WikiPedia Page"
        };
        cardButtons.Add(plButton);

        HeroCard plCard = new HeroCard()
        {
            Title = translateToPigLatin("I'm a hero card, aren't I fancy?"),
            Subtitle = "Pig Latin Wikipedia Page",
            Images = cardImages,
            Buttons = cardButtons
        };

        Attachment plAttachment = plCard.ToAttachment();
        replyToConversation.Attachments.Add(plAttachment);

        var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
        if (reply != null)
            sb.AppendLine(reply.Message);


        // reply to to everyone with a PigLatin Thumbnail Card

        Activity replyToConversation = message.CreateReply(translateToPigLatin("Should go to conversation, with a smaller, but still fancy thumbnail card"));
        replyToConversation.Recipient = message.From;
        replyToConversation.Type = "message";
        replyToConversation.Attachments = new List<Attachment>();

        List<CardImage> cardImages = new List<CardImage>();
        cardImages.Add(new CardImage(url: "https://3.bp.blogspot.com/-7zDiZVD5kAk/T47LSvDM_jI/AAAAAAAAByM/AUhkdynaJ1Y/s200/i-speak-pig-latin.png"));

        List<CardAction> cardButtons = new List<CardAction>();

        CardAction plButton = new CardAction()
        {
            Value = "https://en.wikipedia.org/wiki/Pig_Latin",
            Type = "openUrl",
            Title = "WikiPedia Page"
        };
        cardButtons.Add(plButton);

        ThumbnailCard plCard = new ThumbnailCard()
        {
            Title = translateToPigLatin("I'm a hero card, aren't I fancy?"),
            Subtitle = "Pig Latin Wikipedia Page",
            Images = cardImages,
            Buttons = cardButtons
        };

        Attachment plAttachment = plCard.ToAttachment();
        replyToConversation.Attachments.Add(plAttachment);

        var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
        if (reply != null)
            sb.AppendLine(reply.Message);

        // reply to to everyone with a PigLatin Signin card
        Activity replyToConversation = message.CreateReply(translateToPigLatin("Should go to conversation, sign-in card"));
        replyToConversation.Recipient = message.From;
        replyToConversation.Type = "message";
        replyToConversation.Attachments = new List<Attachment>();

        List<CardAction> cardButtons = new List<CardAction>();

        CardAction plButton = new CardAction()
        {
            Value = "https://spott.cloudapp.net/setup?id=838303b66d9a4f4c7308fa465c5abf74",
            Type = "signin",
            Title = "Connect"
        };
        cardButtons.Add(plButton);

        SigninCard plCard = new SigninCard( text:"You need to authorize me to access Spotify", buttons: cardButtons);

        Attachment plAttachment = plCard.ToAttachment();
        replyToConversation.Attachments.Add(plAttachment);

        var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
        if (reply != null)
            sb.AppendLine(reply.Message);

        // reply to to everyone with a PigLatin Receipt Card
        Activity replyToConversation = message.CreateReply(translateToPigLatin("Should go to conversation, with a smaller, but still fancy thumbnail card"));
        replyToConversation.Recipient = message.From;
        replyToConversation.Type = "message";
        replyToConversation.Attachments = new List<Attachment>();

        List<CardImage> cardImages = new List<CardImage>();
        cardImages.Add(new CardImage(url: "https://3.bp.blogspot.com/-7zDiZVD5kAk/T47LSvDM_jI/AAAAAAAAByM/AUhkdynaJ1Y/s200/i-speak-pig-latin.png"));

        List<CardAction> cardButtons = new List<CardAction>();

        CardAction plButton = new CardAction()
        {
            Value = "https://en.wikipedia.org/wiki/Pig_Latin",
            Type = "openUrl",
            Title = "WikiPedia Page"
        };
        cardButtons.Add(plButton);

        ReceiptItem lineItem1 = new ReceiptItem()
        {
            Title = translateToPigLatin("Pork Shoulder"),
            Subtitle = translateToPigLatin("8 lbs"),
            Text = translateToPigLatin("You know you want it"),
            Image = new CardImage(url: "https://3.bp.blogspot.com/-_sl51G9E5io/TeFkYbJ2lDI/AAAAAAAAAL8/Ug_naHX6pAk/s400/porkshoulder.jpg"),
            Price = "16.25",
            Quantity = "1",
            Tap = null
        };

        ReceiptItem lineItem2 = new ReceiptItem()
        {
            Title=translateToPigLatin("Bacon"),
            Subtitle=translateToPigLatin("5 lbs"),
            Text = translateToPigLatin("There's nothing better."),
            Image=new CardImage(url: "http://www.drinkamara.com/wp-content/uploads/2015/03/bacon_blog_post.jpg"),
            Price="34.50",
            Quantity="2",
            Tap= null
        };

        List<ReceiptItem> receiptList = new List<ReceiptItem>();
        receiptList.Add(lineItem1);
        receiptList.Add(lineItem2);

        ReceiptCard plCard = new ReceiptCard()
        {
            Title = translateToPigLatin("I'm a receipt card, aren't I fancy?"),
            Buttons = cardButtons,
            Items = receiptList,
            Total = "275.25", 
            Tax = "27.52"
        };

        Attachment plAttachment = plCard.ToAttachment();
        replyToConversation.Attachments.Add(plAttachment);

        var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);
        if (reply != null)
            sb.AppendLine(reply.Message);

        // reply to to everyone with a Carousel of three hero cards
        Activity replyToConversation = message.CreateReply(translateToPigLatin("Should go to conversation, with a fancy schmancy hero card"));
        replyToConversation.Recipient = message.From;
        replyToConversation.Type = "message";
        replyToConversation.Attachments = new List<Attachment>();

        Dictionary<string, string> cardContentList = new Dictionary<string, string>();
        cardContentList.Add("PigLatin", "https://3.bp.blogspot.com/-7zDiZVD5kAk/T47LSvDM_jI/AAAAAAAAByM/AUhkdynaJ1Y/s200/i-speak-pig-latin.png");
        cardContentList.Add("Pork Shoulder", "https://3.bp.blogspot.com/-_sl51G9E5io/TeFkYbJ2lDI/AAAAAAAAAL8/Ug_naHX6pAk/s400/porkshoulder.jpg");
        cardContentList.Add("Bacon", "http://www.drinkamara.com/wp-content/uploads/2015/03/bacon_blog_post.jpg");

        foreach(KeyValuePair<string, string> cardContent in cardContentList)
        {
            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage(url:cardContent.Value ));

            List<CardAction> cardButtons = new List<CardAction>();

            CardAction plButton = new CardAction()
            {
                Value = $"https://en.wikipedia.org/wiki/{cardContent.Key}",
                Type = "openUrl",
                Title = "WikiPedia Page"
            };
            cardButtons.Add(plButton);

            HeroCard plCard = new HeroCard()
            {
                Title = translateToPigLatin($"I'm a hero card about {cardContent.Key}"),
                Subtitle = $"{cardContent.Key} Wikipedia Page",
                Images = cardImages,
                Buttons = cardButtons
            };

            Attachment plAttachment = plCard.ToAttachment();
            replyToConversation.Attachments.Add(plAttachment);
        }

        replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;

        var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);

        if (reply != null)
            sb.AppendLine(reply.Message);

        return message.CreateReply(translateToPigLatin("Completed CardTypesTest"));
    }

    private async Task<Activity> dataTypesTest(Activity message, ConnectorClient connector, StateClient sc)
    {

        BotState botState = new BotState(sc);
        StringBuilder sb = new StringBuilder();
        // DM a user 
        DateTime timestamp = DateTime.UtcNow;

        pigLatinBotUserData addedUserData = new pigLatinBotUserData();
        BotData botData = new BotData();
        try
        {
            botData = (BotData)await botState.GetUserDataAsync(message.ChannelId, message.From.Id);
        }
        catch (Exception e)
        {
            if (e.Message == "Resource not found")
            { }
            else
                throw e;
        }

        if (botData == null)
            botData = new BotData(eTag: "*");

        addedUserData = botData.GetProperty<pigLatinBotUserData>("v1") ?? new pigLatinBotUserData();

        addedUserData.isNewUser = false;
        addedUserData.lastReadLegalese = timestamp;

        try
        {
            botData.SetProperty("v1", addedUserData);
            var response = await botState.SetUserDataAsync(message.ChannelId, message.From.Id, botData);
        }
        catch (Exception e)
        {
            Trace.WriteLine(e.Message);
        }

        try
        {
            botData = (BotData)await botState.GetUserDataAsync(message.ChannelId, message.From.Id);
        }
        catch (Exception e)
        {
            if (e.Message == "Resource not found")
            { }
            else
                throw e;
        }

        if (botData == null)
            botData = new BotData(eTag: "*");

        addedUserData = botData.GetProperty<pigLatinBotUserData>("v1") ?? new pigLatinBotUserData();

        if (addedUserData.isNewUser != false || addedUserData.lastReadLegalese != timestamp)
            sb.Append(translateToPigLatin("Bot data didn't match doofus."));
        else
            sb.Append(translateToPigLatin("Yo, that get/save/get thing worked."));

        return message.CreateReply(sb.ToString(),"en-Us");

    }
}