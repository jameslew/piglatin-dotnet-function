# Debugging a C# Azure Bot Service In Visual Studio 

## Setup 
There are a number of tools which are needed to set up the debug environment.

### Install tools
1. Install [DotNet CLI](https://github.com/dotnet/cli) 
2. Install [Node/npm](https://nodejs.org/en/) 
3. Run **npm install -g azure-functions-cli** to install the Azure Function host environment.
4. Install [Command Task Runner](https://visualstudiogallery.msdn.microsoft.com/e6bf6a3d-7411-4494-8a1e-28c1a8c4ce99) Visual Studio extension
5. Install [Bot Framework Emulator](https://docs.botframework.com/en-us/tools/bot-framework-emulator/#navtitle)
6. *(OPTIONAL)* run **npm install -g ngrok** to install [ngrok](http://ngrok.io) for debugging bots in the cloud

### Initialize Project
In the root of your project
1. Run **dotnet restore** to restore your .net assemblies from the project.json file.

## Debugging
### Load solution 

Load your solution file.  If you have the *Command Task Runner* installed then it should 
automatically start up the web hosting environment for your http://localhost:3978/api/messages.

If you open the *Task Runner Explorer* window you can see the console output of the debug host as it dynmicaly compiles your .csx files.

> You open the *Task Runner Explorer* window via  *View>Other windows>Task Runner Explorer* Menu, or you can use **Ctrl+Alt+Backspace**.

### Start emulator 
Confiugure  http://localhost:3978/api/messages with your appid/password for your bot.  You should now be able to send
a message to your bot and see responses in the emulator

>  If you don't have an appid/password yet for your bot then just leave those properties empty
  
### Start Debugger by hitting F5
When you start the debugger, it will attach to the Azure Function host (func.exe).  

You should now be able to set breakpoints.  

> Whenever you change a file and save your breakpoints will go away, because the file doesn't match the running code.
> The next time a message comes in will cause the code to be recompiled and your breakpoints will come back.





