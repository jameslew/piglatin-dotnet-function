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

class MessageHandler {

    private async Task<Activity> messageTypesTest(Activity message, ConnectorClient connector, StateClient sc)
    {

        StringBuilder sb = new StringBuilder();
        /*var botState = new BotState(sc);
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
        await context.PostAsync(conversation.Id);
        newDirectToUser.Conversation = new ConversationAccount(id: ConversationId.Id);
        var reply = await connector.Conversations.SendToConversationAsync(newDirectToUser);
        if (reply != null)
            sb.AppendLine(reply.Message);*/


        // message to conversation not directed to user using CreateReply
        Activity replyToConversation = message.CreateReply("Should go to conversation, but does not address the user that generated it");
        var bcReply = await connector.Conversations.SendToConversationAsync(replyToConversation);
        if(bcReply != null)
            sb.AppendLine(bcReply.Message);

        // reply to to user using CreateReply
        Activity newConversation = message.CreateReply("Should go to conversation, but addressing the user that generated it");
        newConversation.Recipient = message.From;
        var replyWNewConversation = await connector.Conversations.SendToConversationAsync(newConversation);
        if (replyWNewConversation != null)
            sb.AppendLine(replyWNewConversation.Message);

        return message.CreateReply(translateToPigLatin("Completed MessageTypesTest"));
    }


    private async Task<Activity> cardTypesTest(Activity message, ConnectorClient connector)
    {

        StringBuilder sb = new StringBuilder();

        // reply to to everyone with a PigLatin Hero Card
        Activity heroReply = message.CreateReply(translateToPigLatin("Should go to conversation, with a fancy schmancy hero card"));
        heroReply.Recipient = message.From;
        heroReply.Type = "message";
        heroReply.Attachments = new List<Attachment>();

        List<CardImage> heroCardImages = new List<CardImage>();
        heroCardImages.Add(new CardImage(url: "https://3.bp.blogspot.com/-7zDiZVD5kAk/T47LSvDM_jI/AAAAAAAAByM/AUhkdynaJ1Y/s200/i-speak-pig-latin.png"));
        heroCardImages.Add(new CardImage(url: "https://2.bp.blogspot.com/-Ab3oCVhOBjI/Ti23EzV3WCI/AAAAAAAAB1o/tiTeBslO6iU/s1600/bacon.jpg"));

        List<CardAction> heroCardButtons = new List<CardAction>();

        CardAction heroPlButton = new CardAction()
        {
            Value = "https://en.wikipedia.org/wiki/Pig_Latin",
            Type = "openUrl",
            Title = "WikiPedia Page"
        };
        heroCardButtons.Add(heroPlButton);

        HeroCard heroPlCard = new HeroCard()
        {
            Title = translateToPigLatin("I'm a hero card, aren't I fancy?"),
            Subtitle = "Pig Latin Wikipedia Page",
            Images = heroCardImages,
            Buttons = heroCardButtons
        };

        Attachment heroPlAttachment = heroPlCard.ToAttachment();
        heroReply.Attachments.Add(heroPlAttachment);

        var heroResult = await connector.Conversations.SendToConversationAsync(heroReply);
        if (heroResult != null)
            sb.AppendLine(heroResult.Message);


        // reply to to everyone with a PigLatin Thumbnail Card

        Activity thumbnailCardReply = message.CreateReply(translateToPigLatin("Should go to conversation, with a smaller, but still fancy thumbnail card"));
        thumbnailCardReply.Recipient = message.From;
        thumbnailCardReply.Type = "message";
        thumbnailCardReply.Attachments = new List<Attachment>();

        List<CardImage> tnCardImages = new List<CardImage>();
        tnCardImages.Add(new CardImage(url: "https://3.bp.blogspot.com/-7zDiZVD5kAk/T47LSvDM_jI/AAAAAAAAByM/AUhkdynaJ1Y/s200/i-speak-pig-latin.png"));

        List<CardAction> tnCardButtons = new List<CardAction>();

        CardAction tnPlButton = new CardAction()
        {
            Value = "https://en.wikipedia.org/wiki/Pig_Latin",
            Type = "openUrl",
            Title = "WikiPedia Page"
        };
        tnCardButtons.Add(tnPlButton);

        ThumbnailCard tnPlCard = new ThumbnailCard()
        {
            Title = translateToPigLatin("I'm a hero card, aren't I fancy?"),
            Subtitle = "Pig Latin Wikipedia Page",
            Images = tnCardImages,
            Buttons = tnCardButtons
        };

        Attachment tnPlAttachment = tnPlCard.ToAttachment();
        thumbnailCardReply.Attachments.Add(tnPlAttachment);

        var tnCardResult = await connector.Conversations.SendToConversationAsync(thumbnailCardReply);
        if (tnCardResult != null)
            sb.AppendLine(tnCardResult.Message);

        // reply to to everyone with a PigLatin Signin card
        Activity signinCardReply = message.CreateReply(translateToPigLatin("Should go to conversation, sign-in card"));
        signinCardReply.Recipient = message.From;
        signinCardReply.Type = "message";
        signinCardReply.Attachments = new List<Attachment>();

        List<CardAction> siCardButtons = new List<CardAction>();

        CardAction siPlButton = new CardAction()
        {
            Value = "https://spott.cloudapp.net/setup?id=838303b66d9a4f4c7308fa465c5abf74",
            Type = "signin",
            Title = "Connect"
        };
        siCardButtons.Add(siPlButton);

        SigninCard siPlCard = new SigninCard( text:"You need to authorize me to access Spotify", buttons: siCardButtons);

        Attachment siPlAttachment = siPlCard.ToAttachment();
        signinCardReply.Attachments.Add(siPlAttachment);

        var siCardResult = await connector.Conversations.SendToConversationAsync(signinCardReply);
        if (siCardResult != null)
            sb.AppendLine(siCardResult.Message);

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
        Activity carouselCardReply = message.CreateReply(translateToPigLatin("Should go to conversation, with a fancy schmancy hero card"));
        carouselCardReply.Recipient = message.From;
        carouselCardReply.Type = "message";
        carouselCardReply.Attachments = new List<Attachment>();

        Dictionary<string, string> cardContentList = new Dictionary<string, string>();
        cardContentList.Add("PigLatin", "https://3.bp.blogspot.com/-7zDiZVD5kAk/T47LSvDM_jI/AAAAAAAAByM/AUhkdynaJ1Y/s200/i-speak-pig-latin.png");
        cardContentList.Add("Pork Shoulder", "https://3.bp.blogspot.com/-_sl51G9E5io/TeFkYbJ2lDI/AAAAAAAAAL8/Ug_naHX6pAk/s400/porkshoulder.jpg");
        cardContentList.Add("Bacon", "http://www.drinkamara.com/wp-content/uploads/2015/03/bacon_blog_post.jpg");

        foreach(KeyValuePair<string, string> cardContent in cardContentList)
        {
            List<CardImage> carouselCardImages = new List<CardImage>();
            carouselCardImages.Add(new CardImage(url:cardContent.Value ));

            List<CardAction> carouselCardButtons = new List<CardAction>();

            CardAction carouselPlButton = new CardAction()
            {
                Value = $"https://en.wikipedia.org/wiki/{cardContent.Key}",
                Type = "openUrl",
                Title = "WikiPedia Page"
            };
            carouselCardButtons.Add(carouselPlButton);

            HeroCard carouselPlCard = new HeroCard()
            {
                Title = translateToPigLatin($"I'm a hero card about {cardContent.Key}"),
                Subtitle = $"{cardContent.Key} Wikipedia Page",
                Images = carouselCardImages,
                Buttons = carouselCardButtons
            };

            Attachment carouselPlAttachment = carouselPlCard.ToAttachment();
            carouselCardReply.Attachments.Add(carouselPlAttachment);
        }

        carouselCardReply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

        var carouselCardResult = await connector.Conversations.SendToConversationAsync(carouselCardReply);

        if (carouselCardResult != null)
            sb.AppendLine(carouselCardResult.Message);

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