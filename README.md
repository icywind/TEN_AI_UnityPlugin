# TEN AI Agent Demo

  

![TENDemoIOS](https://github.com/user-attachments/assets/8ea3df82-61ba-4fa2-ba43-52b85040a27f)
  

This app is powered by the technology of Realtime Communication, Realtime Transcription, a Large Language Model (LLM), and Text to Speech extensions. The TEN Framework makes the workflow super easy! The Unity Demo resembles the web demo and acts as the mobile frontend to the AI Agent. You may ask the Agent any general question.

The Plugin is exported as a reusable package that can be imported on any Agora RTC projects. Download the package and import into your project.


## Prerequisites:

- Agora Developer account
- Agora Video SDK for Unity (v4.2.6 or up)
- [TEN Frameworks Agent](https://github.com/TEN-framework/TEN-Agent)
- Unity 2021 or up

  

### Implicit requirements by TEN Framework:

- Text to Speech Support (API Key from [Azure Speech Service](https://portal.azure.com/#view/Microsoft_Azure_ProjectOxford/CognitiveServicesHub/~/SpeechServices))

- LLM Support (e.g. [API key from OpenAI](https://platform.openai.com/api-keys))

## Setups

### TEN Agent Server

First you should have gotten the TEN Agent working in your environment. You will just need the Server (ten_agent_dev) running for this application.

![docker](https://github.com/user-attachments/assets/f292ad45-7be7-458a-b40a-46cce847809f)

Note, the last verified commit is `ac7fd7a7a76d09d018513989d32b37ba7685e652`.

  

## To run this Demo
0. Clone this project.
1. Download and import the [Agora Video SDK](https://docs.agora.io/en/sdks?platform=unity) for Unity.
2. Add both **TENEntryScreen** scene and **TENDemoScene** to the Build Settings. The TENEntryScreen scene should be on top.
3. Fill out the information for Config input asset.
![ten config](https://github.com/user-attachments/assets/4eab50db-a885-4d06-a349-64c3cadd188f)

4. Play the demo on Editor or build it to device platform of your choice.  

### Alternatively, Android apk
Go to the Release section, download the apk file and experience the AI Agent.

## To use the prefab for your own project
0. Download the TENClientFramework.unitypackage file from the **Releases** section; import it to your project.
1. Download and import the [Agora Video SDK](https://docs.agora.io/en/sdks?platform=unity) for Unity.
2. Drag the prefabs into your project and use them connect to your controller code.
	- ChatController
	- SphereVisual
	- TENManager
3. Follow the [Modification Steps](#modification-steps-to-an-existing-project) below.
4. Fill out the information for Config input asset.

5. Play the demo on Editor or build it to device platform of your choice. 

## Modification Steps to An Existing Project

1. Pass Scriptable object to AppConfig:

```csharp
using Agora.TEN.Client;
```
```csharp
[SerializeField]
TENConfigInput TENConfig; // input from Editor

// Call this function in your Init step
void  SetConfig()
{
	AppConfig.Shared.SetValue(TENConfig);
	// obtain channel name before this call
    AppConfig.Shared.Channel = _channelName;
}
```
Note you should provide a channel name in your app logic to use among the participanting users.  In this 1-to-1 chat project, we provide an util function to ganerate a string:
```csharp
    _channelName = AppConfig.Shared.Channel = UtilFunctions.GenRandomString("agora_", 5);
```

2. Hook the prefabs up in your main logic:

```csharp
[SerializeField]
internal  IChatTextDisplay  TextDisplay;  // ChatController
[SerializeField]
internal  TENSessionManager  TENSession;  // TENManager
[SerializeField]
internal  SphereVisualizer  Visualizer;   // SphereVisual
```

3. Use `TENSession.GetToken()` to get token before joining channel
```csharp
async void GetTokenAndJoin()
{
    AppConfig.Shared.RtcToken = await TENSession.GetToken();
    JoinChannel();  // your join channel call that uses the rtc token
}
```

4. Setup sound visualization, which will automatically configure the Agora RTC engine for audio data capture.

```csharp
	Visualizer?.Init(RtcEngine);
```

5. Start the TEN session on OnJoinChannelSuccess()
```csharp
// _app is the instance of your controller class
_app.TENSession.StartSession(connection.localUid);
```

6. Disable/Enable the sound visualizer, a component of a gameobject that represents the AI Agent:
- Disable it during initialization, e.g. SetupUI() or Start()
```csharp
	Visualizer?.gameObject.SetActive(false);
```
- Enable it when the AI Agent user joins the channel:
```csharp
public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
{
    if (uid == AppConfig.Shared.AgentUid) {
        _app.Visualizer?.gameObject.SetActive(true);
	}
}
```

7. Register handler OnStreamMessage:
- SDK ver 4.4.0
```csharp
public  override  void  OnStreamMessage(RtcConnection  connection, uint  remoteUid, int  streamId, byte[] data, ulong  length, ulong  sentTs)
{
	string  str = System.Text.Encoding.UTF8.GetString(data, 0, (int)length);
	_app.TextDisplay.ProcessTextData(remoteUid, str);
	_app.TextDisplay.DisplayChatMessages(_app.LogText.gameObject);
}

```
- SDK ver 4.2.6
```csharp
public override void OnStreamMessage(RtcConnection connection, uint remoteUid, int streamId, byte[] data, uint length, System.UInt64 sentTs)
{
    string str = System.Text.Encoding.UTF8.GetString(data, 0, (int)length);
    _app.TextDisplay.ProcessTextData(remoteUid, str);
    _app.TextDisplay.DisplayChatMessages(_app.LogText.gameObject);
}

```
8. OnDestroy() or logic to stop.

```csharp
TENSession.StopSession();
```

## References

For reference, it is worthwhile to check out the following resources:

* [TEN Framework docs](https://doc.theten.ai/)
* [TEN IOS Client](https://github.com/AgoraIO-Community/TEN-AI-Demo-IOS)
* [Agora SDK API references](https://api-ref.agora.io/en/voice-sdk/ios/4.x/documentation/agorartckit).


  

## License

[MIT License](https://github.com/icywind/TEN-AI-Demo-IOS/blob/main/LICENSE)