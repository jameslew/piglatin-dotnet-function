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


    }


}