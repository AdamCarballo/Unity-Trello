Unity Trello
=================
☑️️ Generate Trello cards directly from Unity

Overview
----
When a player experiences a bug on a game, very rarely will waste their time contacting the developer by email or using a forum, and even if they document the bug, it will probably have incomplete information.<br><br>
Developing a game where your players / testers are random people interested in your game, there is no easy way to store all the bugs in one place without needing to rewrite every one of them to a safe place where you can start debugging the issue.

This repository allows you to use a Trello board as a bug and issue storage, creating Trello cards directly from Unity on runtime using the Trello API. You can execute automatic collection of errors when you [catch exceptions](https://msdn.microsoft.com/en-us/library/0yd65esw.aspx) or let the players report the bug to you, directly from the game, using the same UI. You can then pull system information using [SystemInfo](https://docs.unity3d.com/ScriptReference/SystemInfo.html) and other functions provided by the Unity API.

License
---
This repository is based on [Trello-Cards-Unity](https://github.com/bfollington/Trello-Cards-Unity) by [bfollington](https://github.com/bfollington).<br>
Even though the code has been improved and has added features over the original source, all the files that come from the original repository (*stated on the header comment on each file*) remain under the original author license.<br><br>
This repository also uses `MiniJSON.cs` whose license is stated on the header comment inside the file.

Any other files remain under the MIT License.

Install
----
The source code is available directly from the folders, or if you want you can download only the [Unitypackage](https://github.com/AdamEC/Unity-Trello/releases) and import from there.

Before continuing, read the **Key and token Security** section, down below, it's very important to understand the security risks.<br>
To use this repository, you will need a Trello account (*remember, not your personal account!*) to obtain a Trello API key and a token.

#### Obtain your Application Key:
Go to https://trello.com/app-key (*Logged on the bot / bug report Trello account!*) and save the key.
This application key will become the *Trello API key*, or just *key* for short.

#### Create a new Application (Token):
Now, go to https://trello.com/1/connect?key=[yourkeygoeshere]&name=Unity%20Trello&response_type=token&scope=read,write&expiration=never (*Change `[yourkeygoeshere]` for your key*).<br>
This will create a new App that will not expirate (`expiration=never`). If you want to remove this app on the future, go to Trello > Settings and revoke access to the App (*Unity Trello*).<br>
If everything works, you will be redirected to https://trello.com/1/token/approve where you will get your token.

Key and token Security
----
Common sense should be enough, but some tips never hurt.

Before using your Trello account, or any other account with access to private or personal boards, remember that the public Trello API key and token will be stored on the client binaries. They will remain in private variables, without external access, and not in plain text (as long as they are not hardcoded).

The public key (as the name implies) can be public, think of it as the account identifier. That's also why is recommended to create a new account for every app / integration. You don't want the same public API key for two unrelated apps, or your personal account.

The application token is another story though. Anyone with your token (and public API) will have access to the entire account. 

This is why you should **not use your personal account, or any account with access to other boards**. If someone obtains your key and token, they can **create, move and delete any card, on any board you have access**. I recommend creating a bot account that will only have access to the bug report board, this way if something goes wrong, only that board would get affected.

This repository doesn't include any encryption for the key or token, but I don't recommend wasting your time trying to hide the token. Obfuscation or encryption will never protect your token from someone who really wants it (for some reason). If you really want to keep your credentials truly save, you should move the request to upload Trello cards to an online server (*that you own*), where your key and token would be safe.

On the good side, the Trello API is always contacted over HTTPS (TLS), so sniffing the key or token from a HTTP packet won't be possible.
If they try to decompile the key and token from the libraries of the game, as long as you use inspector variables, obtaining either the token or the key is a very hard and tedious task to do (but not impossible).

I don't recommend this approach for released projects, or projects with huge player bases, because at that point a forum is more secure and probably, useful.

Usage
----
Unity Trello consists of four scripts, `TrelloAPI.cs`, `TrelloCard.cs`, `TrelloException.cs` and `TrelloSend.cs` to work.<br>
All scripts include the namespace `Trello` to avoid issues with existing scripts. Remember to use it.

#### TrelloAPI.cs
Interacts directly with the Trello API using MiniJSON and uploads cards.<br>
Used by other components, this script talks directly with the Trello API. It shouldn't be used directly.
___

#### TrelloCard.cs
Base class for a Trello card.<br>
This script will hold all the data a Trello card can store and send to the API.
___

#### TrelloException.cs
Custom Exception for Trello scripts.
___

#### TrelloSend.cs
Script that holds keys and allows to send Trello cards.<br>
This script will hold the Trello API key, token and default board and list where cards will be uploaded.<br>
It also contains `SendNewCard()` that will handle the selection of board, list and will upload the card all by itself.
___

### Creating and sending Trello cards:
As stated before, `SendNewCard()` from `TrelloSend.cs` will do all of this automatically.<br>
If you just want to know how to populate a Trello card, skip to the next code block.

```csharp
// Create a new Trello card
var card = new TrelloCard();

// Generate a new TrelloAPI using the given authorization
var api = new TrelloAPI(string _key, string _token);

// Pull all the Trello boards where the user has permissions
yield return api.PopulateBoards();
// Select the given board from the pulled list
api.SetCurrentBoard(string board);

// Pull all the lists inside the selected board
yield return api.PopulateLists();
// Select the given list from the pulled lists
api.SetCurrentList(string list);

// Write the ID of the selected list on the TrelloCard.idList variable
card.idList = api.GetCurrentListId();

// Call TrelloAPI.UploadCard() to upload the card to the Trello servers
yield return api.UploadCard(card);
```

### Populating a Trello card:
A Trello card can store the following data:

```csharp
string name // Name of the card. Will appear in the Title of the card
string desc // Description of the card. Will appear in the description of the card
string due // Date of the card. Will appear as "Due Date" in the card. Must use System.DateTime
string urlSource // A URL. If not null a link will be attached to the card
byte[] fileSource // A file / photo. If not null will try to attach a file to the card
string fileName // Name of the file. If this or fileSource is null, the file will be ignored

// Public, but will get overwritten on runtime:
string idList // ID of the list where the card will get uploaded
```

To see an example of how to let the player fill this data, check `TrelloDemoCard.cs` inside the Demo folder.<br>
It also contains a function `UploadJPG()` that takes screenshots in JPG (includes the UI) and converts them back to a byte array, ready to be sent.

### Trello Syntax:
Descriptions in Trello use the Markdown syntax to change the style of the text.<br>
See this example extracted from `TrelloDemoCard.cs` that uses Markdown syntax:
```csharp
card.desc =
"#" + "Demo " + Application.version + "\n" +
"___\n" +
"###System Information\n" +
"- " + SystemInfo.operatingSystem + "\n" +
"- " + SystemInfo.processorType + "\n" +
"- " + SystemInfo.systemMemorySize + " MB\n" +
"- " + SystemInfo.graphicsDeviceName + " (" + SystemInfo.graphicsDeviceType + ")\n" +
"\n" +
"___\n" +
"###User Description\n" +
"```\n" +
desc.text + "\n" +
"```\n" +
"___\n" +
"###Other Information\n" +
"Playtime: " + String.Format("{0:0}:{1:00}", Mathf.Floor(Time.time / 60), Time.time % 60) + "h" + "\n" +
"Tracked object position: " + trackedObject.position;
```
Once uploaded to Trello, the card description looks like this:<br>
![Trello Description](https://i.imgur.com/Zt7UD9N.png)<br>

For more information on how to use Markdown, or what syntax Trello supports, read the Trello [documentation](http://help.trello.com/article/821-using-markdown-in-trello).

Demo
----
The demo folder from the source code (included in the [Unitypackage](https://github.com/AdamEC/Unity-Trello/releases)) includes a demo scene with custom UI to understand how everything works.<br>
Remember to fill the variables `key` and `token` on the inspector with the ones obtained from Trello, and set the default board and list where the card should be uploaded.

History
----
Created by Àdam Carballo (AdamEC)<br>
Check other works on *[Engyne Creations](https://engynecreations.com)*.<br>

[![ko-fi](https://www.ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/X8X4XHCE)

Huge thanks to [bfollington](https://github.com/bfollington) for the original code that I've been using for a long time.
