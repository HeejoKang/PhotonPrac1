using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

public class IntroManager : PunBehaviour //Monobehavior + photon 합친 클래스
{
    public Button createButton; //방 생성 버튼
    public GameObject createRoomPannelPrefab; //방 생성 팝업
    public GameObject gameRoomCellPrefab;
    public Canvas canvas;
    public GameObject scrollContent; //셀의 부모

    string serverVer = "0.1";
    CreateRoomPannelManager createRoomPannelManager; //룸 생성 성공을 사용자에게 알려줄 변수

    private void Awake()
    {
        createButton.interactable = false;

        //photon 초기화
        PhotonNetwork.logLevel = PhotonLogLevel.Full; //포톤의 로그를 모두 띄웁니다
        PhotonNetwork.autoJoinLobby = true; //로비에서 자동접속을 허용합니다
        PhotonNetwork.automaticallySyncScene = true; //포톤이 알아서 싱크를 맞춥니다
    }

    public void OnClickCreateButton()
    {
        if (createRoomPannelManager == null) //방 생성 팝업이 안 떴을 때
        {
            GameObject createRoomPannel = Instantiate(createRoomPannelPrefab);
            createRoomPannel.transform.SetParent(canvas.transform, false); ///캔버스를 부모로서 지정
            createRoomPannelManager = createRoomPannel.GetComponent<CreateRoomPannelManager>();
            createRoomPannelManager.createRoomDelegate = CreateRoom;
            // 델리게이트를 통해 함수를 미리 저장하여 이벤트나 사용시점에 대기할 수 있음
            // 시간 차를 두고 준비해 놓을 수 있음?
        }
    }

    void CreateRoom(string roomName)
    {
        if (string.IsNullOrEmpty(roomName))
        {
            return;
        }

        RoomOptions option = new RoomOptions(); //포톤 내 정의 된 클래스: 방의 속성을 정할 수 있는 변수
        option.IsOpen = true;
        option.IsVisible = true;
        option.MaxPlayers = 10;

        PhotonNetwork.CreateRoom(roomName, option, TypedLobby.Default);
    }

    void Connect()
    {
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.ConnectUsingSettings(serverVer); //서버버전과 접속의 관계 물어볼 것(?)
        }
    }

    public override void OnJoinedLobby()
    {
        //Todo: Create Room 버튼 활성화
      //  base.OnJoinedLobby(); //오버라이드 함수에 부모의 원 함수 내용을 안 적는 이유??
        createButton.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        //Todo: Game Scene으로 전환(게임실행)
        //방 만들기 위한 팝업 창 닫기
       // base.OnJoinedRoom();
        Debug.Log("## OnJoinedRoom()");

        if (createRoomPannelManager != null)
        {
            createRoomPannelManager.CreateRoomSuccess();
        }

        //메세지큐잉작업: 미전달 메세지 처리 과정을 모두 오프시킵니다.(버림)
        PhotonNetwork.isMessageQueueRunning = false;
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        //방을 선택할 수 있게 UI 갱신
        //base.OnPhotonJoinRoomFailed(codeAndMsg);

    }
    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        //유저에게 방 생성 실패 알리기
        //base.OnPhotonCreateRoomFailed(codeAndMsg);

        if (createRoomPannelManager != null)
        {
            createRoomPannelManager.CreateRoomFailed();
        }
    }

    void Interactable(bool interactable)
    {
        foreach (GameObject cell in GameObject.FindGameObjectsWithTag("GameRoomCell"))
        {
            cell.GetComponent<Button>().interactable = interactable; //함수 Interactable의 매겨변수 interactable
        }
        createButton.interactable = interactable;
    }

    //방 정보 변경 감지 + 방 목록 확인 가능 함수
    public override void OnReceivedRoomListUpdate()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("GameRoomCell"))
        {
            Destroy(obj);
        }

        scrollContent.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        int rowCount = 0;

        foreach (RoomInfo room in PhotonNetwork.GetRoomList())
        {
            GameObject gameRoomCellObject = Instantiate(gameRoomCellPrefab);
            gameRoomCellObject.transform.SetParent(scrollContent.transform, false);

            //게임정보를 담은 구조체로 객체 생성
            GameRoomInfo gameRoomInfo = new GameRoomInfo(room.Name, room.PlayerCount, room.MaxPlayers); 
            GameRoomCell gameRoomCell = gameRoomCellObject.GetComponent<GameRoomCell>();
            gameRoomCell.SetRoomInfo(gameRoomInfo);

            //셀 선택 동작
            //OnClick 이벤트가 실행되면(AddListener가 주시하다가) 델리게이트 무명함수를 실행
            gameRoomCellObject.GetComponent<Button>().onClick.AddListener(delegate { PhotonNetwork.JoinRoom(gameRoomInfo.roomName); });

            scrollContent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 80);
            
        }

        //base.OnReceivedRoomListUpdate();


    }


}
