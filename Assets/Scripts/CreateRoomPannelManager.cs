using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomPannelManager : MonoBehaviour
{
    public InputField roomNameInputField;
    public Button okButton;
    public Button cancelButton;
    public Text errorText;

    public delegate void CreateRoomDelegate(string roomName); //선언해서 받는 거랑 addlistner를 통해서 사용하는 거랑 차이가 있나??
    public CreateRoomDelegate createRoomDelegate;             //선언을 통해 받으면 함수의 리턴타입과 인수타입을 지정해줄 수 있다!
                                                              //함수의 핵심: 리턴값 and 인수타입 

    /*델리게이트 - 인터페이스(이벤트)*/
    /*객체지향언어의 특징
     C언어: 절차지향 1차원적으로 쭉 나열하여 프로그램을 제작하는 방식
     
     */
	// Use this for initialization
	void Start ()
    {
        errorText.text = ""; //에러메세지 초기화
        okButton.interactable = false;

        //콜백함수의 사용방식
        //특정 이벤트가 발생했을 때 그에 대응하는 함수를 호출
        //AddListener함수: 델리게이트를 통해 콜백함수 호출!! delegate은 미리 선언하지 않아도 사용 가능???
       
        roomNameInputField.onValueChanged.AddListener(delegate { CheckRoomName();});
	}

    void CheckRoomName()
    {
        if (!string.IsNullOrEmpty(roomNameInputField.text))
        {
            okButton.interactable = true;
        }
        else
        {
            okButton.interactable = false;
        }
    }
	
    public void OnClickOKButton()
    {
        errorText.text = ""; //에러 텍스트 초기화

        if (createRoomDelegate == null)
        {
            return;
        }

        //중복 입력 방지를 위해 모두 비활성화 시킴
        okButton.interactable = false;
        cancelButton.interactable = false;
        roomNameInputField.interactable = false;

        createRoomDelegate(roomNameInputField.text);
    }

    public void OnClickCancelButton()
    {
        Destroy(this.gameObject); //해당 패널 삭제
    }

    public void CreateRoomFailed()
    {
        errorText.text = "방 생성 실패";
        cancelButton.interactable = true;
        roomNameInputField.interactable = true;
        roomNameInputField.text = "";
    }

    public void CreateRoomSuccess()
    {
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
