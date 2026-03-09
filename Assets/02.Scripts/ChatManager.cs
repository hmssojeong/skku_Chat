using ExitGames.Client.Photon;
using Photon.Chat;
using UnityEngine;

// 채팅 이벤트를 수신하는 옵저버 인터페이스
public class ChatManager : MonoBehaviour, IChatClientListener
{
    public static ChatManager Instance { get; private set; }

    private const string CHANNEL_SKKU = "skku2";
    private const string CHANNEL_NOTICE = "notice";

    // 채팅 클라이언트
    private ChatClient _chatClient;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _chatClient = new ChatClient(this);
        _chatClient.DebugOut = DebugLevel.ALL;
        _chatClient.ChatRegion = "ASIA";
        var auth = new AuthenticationValues("몽일");
        _chatClient.Connect("8952ee8f-6b2e-45c9-9cdb-b60a2f906585", "1.0", auth);
    }

    private void Update()
    {
        // ChatClient는 MonoBehaviour가ㅣ 아니므로, 매 프레임 서비스 펌프를 호출해줘야
        // 네트워크 메시지가 처리되고, 아래 콜백 메서드들이 실행된다.
        _chatClient.Service();
    }

    // IChatClientListener는 11개의 콜백으로 채팅 이벤트를 처리 (이게 포톤챗의 끝)

    // 1. 연결 상태 변화
    public void OnConnected()
    {
        Debug.Log("[Photon Chat] 서버에 연결됐습니다.");

        _chatClient.Subscribe(new string[] { CHANNEL_SKKU, CHANNEL_NOTICE });
    }

    public void OnChatStateChange(ChatState state)
    {
        // 스피너 같은 로딩창을 띄워줘도되고... 알아서 처리
        Debug.Log($"[Photon Chat] 상태 변경 ▶ {state}");
    }

    public void OnDisconnected()
    {
        Debug.Log("[Photon Chat] 서버에 연결 해제됐습니다.");
    }


    // 2. 채널 입장/퇴장
    public void OnSubscribed(string[] channels, bool[] results)
    {
        for (int i = 0; i < results.Length; i++)
        {
            Debug.Log($"[Photon Chat] 채널 {channels[i]} 구독 ({results[i]})");
        }

        foreach (var channel in _chatClient.PublicChannels)
        {
            // 여기서 내가 구동중인 채널 목록을 알 수 있다.
        }

        SendChatMessage("안녕하세요.");
    }

    public void OnUnsubscribed(string[] channels)
    {
        foreach (string channel in channels)
        {
            Debug.Log($"[Photon Chat] 채널 {channel} 구독 해지");
        }
    }


    // 3. 다른 유저의 온라인 상태
    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log($"[Photon Chat] 채널 {channel}에 {user} 입장!");
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log($"[Photon Chat] 채널 {channel}에 {user} 퇴장!");
    }

    // 친구/팔로우 리스트 중 특정 유저가 상태 변경 시
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        // 온라인/자리비움/오프라인/상태메시지 등..
        throw new System.NotImplementedException();
    }

    // 4. 메시지 수신
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        // 포톤챗이 네트워크 최적화를 위해 같은 프레인에 여러 개의 메시지를 받으면
        // 매 번 함수를 호출하는게 아니라 배열을 묶어서 한번에 전달하기도 한다.
        for (int i = 0; i < messages.Length; i++)
        {
            Debug.Log($"[Photon Chat] [{channelName}] {senders[i]}: {messages[i]}");
        }
    }

    // 1:1 private chat(귓속말) 메시지가 오면 호출되는 함수
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new System.NotImplementedException();
    }


    // 포톤챗 내부에서 디버그 로그가 발생할 때 호출된다.
    // level에서 지정한 심각도 이상만 들어오며, 개발 단계에서 로그 확인용이다.
    public void DebugReturn(DebugLevel level, string message)
    {
        switch (level)
        {
            case DebugLevel.ERROR:
                Debug.LogError("[Photon Error] " + message);
                break;

            case DebugLevel.WARNING:
                Debug.LogWarning("[Photon Warning] " + message);
                break;

            default:
                Debug.LogError("[Photon Info] " + message);
                break;
        }
    }


    public void SendChatMessage(string message)
    {
        if (_chatClient == null) return;
        if (_chatClient.CanChat == false) ;

        _chatClient.PublishMessage(CHANNEL_SKKU, message);
    }
}