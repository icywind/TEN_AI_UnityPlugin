# TEN AI Agent Demo

  

![TENDemoIOS](https://github.com/user-attachments/assets/8ea3df82-61ba-4fa2-ba43-52b85040a27f)
  

This app is powered by the technology of Realtime Communication, Realtime Transcription, a Large Language Model (LLM), and Text to Speech extensions. The TEN Framework makes the workflow super easy! The Unity Demo resembles the web demo and acts as the mobile frontend to the AI Agent. You may ask the Agent any general question.

The Plugin is exported as a reusable package that can be imported on any Agora RTC projects. Download the package and import into your project.


## Prerequisites:

- Agora Developer account

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

## To use the prefab for your own project
0. Download the TENClientFramework.unitypackage file from the **Releases** section; import it to your project.
1. Download and import the [Agora Video SDK](https://docs.agora.io/en/sdks?platform=unity) for Unity.
2. Drag the prefabs into your project and use them connect to your controller code.
	- AppConfigInput
	- ChatController
	- SphereVisual
	- TENManager
3. Follow the [Modification Steps](#modification-steps-to-an-existing-project) below.
4. Fill out the information for Config input asset.

5. Play the demo on Editor or build it to device platform of your choice. 

## Modification Steps to An Existing Project

1. Pass Scriptable object to AppConfig:

```csharp
void  SetConfig()
{
	AppConfig.Shared.SetValue(TENConfig);
}
```

2. Hook them up in your main logic:

```csharp
[SerializeField]
internal  IChatTextDisplay  TextDisplay;
[SerializeField]
internal  TENSessionManager  TENSession;
[SerializeField]
internal  SphereVisualizer  Visualizer;
```

3. Use ```TENSession.GetToken()``` to get token before joining channel

4. Setup for Audio display before Mixing in InitEngine()

```csharp
int  CHANNEL = 1;
int  SAMPLE_RATE = 44100;
RtcEngine.SetPlaybackAudioFrameBeforeMixingParameters(SAMPLE_RATE, CHANNEL);
RtcEngine.RegisterAudioFrameObserver(new  AudioFrameObserver(this),
AUDIO_FRAME_POSITION.AUDIO_FRAME_POSITION_BEFORE_MIXING,OBSERVER_MODE.RAW_DATA);
```

5. Implement AudioFrameObserver:
```csharp
internal  class  AudioFrameObserver : IAudioFrameObserver
{
	TENDemoChat  _app;
	internal  AudioFrameObserver(TENDemoChat  client)
	{
		_app = client;
	}
}

public  override  bool  OnPlaybackAudioFrameBeforeMixing(string  channel_id, uint  uid,AudioFrame  audio_frame)
{
	var  floatArray = UtilFunctions.ConvertByteToFloat16(audio_frame.RawBuffer);
	_app.Visualizer?.UpdateVisualizer(floatArray);
	return  false;
}
```

  
  

6. OnJoinChannelSuccess()
```csharp
_app.TENSession.StartSession(connection.localUid);
```

7. Register handler OnStreamMessage:
```csharp
public  override  void  OnStreamMessage(RtcConnection  connection, uint  remoteUid, int  streamId, byte[] data, ulong  length, ulong  sentTs)
{
	string  str = System.Text.Encoding.UTF8.GetString(data, 0, (int)length);
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

* [Agora SDK API references](https://api-ref.agora.io/en/voice-sdk/ios/4.x/documentation/agorartckit).

  

## License

[MIT License](https://github.com/icywind/TEN-AI-Demo-IOS/blob/main/LICENSE)