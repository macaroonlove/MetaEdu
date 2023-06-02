using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PlayfabManager : MonoBehaviour
{
    public UI_PublicManager uiMgr;
    public LoginPhotonManager photonMgr;

    [Header("로그인&회원가입")]
    public TMP_InputField lID_Input;
    public TMP_InputField lPWInput, sUsernameInput, sPWInput, srPWInput;
    public TextMeshProUGUI loginError, signupError;
    private string _lUserName, _lPassword, _sUserName, _sPassword, _srPassword;

    [Header("캐릭터 생성")]
    public TMP_InputField nickNameText;
    public ToggleGroup toggleGroup;
    public TextMeshProUGUI createError;
    public GameObject[] models;
    private string _sex = "Q";

    #region 로그인&회원가입
    public void LId_Changed()
    {
        _lUserName = lID_Input.text;
        loginError.text = "";
    }
    public void LPw_Changed()
    {
        _lPassword = lPWInput.text;
        loginError.text = "";
    }
    public void SId_Changed() => _sUserName = sUsernameInput.text;
    public void SPw_Changed() => _sPassword = sPWInput.text;
    public void SRPw_Changed() => _srPassword = srPWInput.text;

    #region 로그인
    public void LoginButton()
    {
        var request = new LoginWithPlayFabRequest { Username = _lUserName, Password = _lPassword };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
    }
    // 로그인에 성공하면
    private void OnLoginSuccess(LoginResult result)
    {
        Singleton.Inst.Playfab_ID = result.PlayFabId;

        var request = new GetAccountInfoRequest { Username = _lUserName };
        PlayFabClientAPI.GetAccountInfo(request, GetAccountSuccess, (PlayFabError error) => { Debug.Log("사용자 정보 가져오기 실패"); });
    }

    private void OnLoginFailure(PlayFabError error) => loginError.text = "계정이름이나 비밀번호가\n일치하지 않습니다.";
    #endregion

    #region 회원가입
    public void SignupButton()
    {
        if (_sPassword.Equals(_srPassword))
        {
            var request = new RegisterPlayFabUserRequest { Username = _sUserName, Password = _sPassword, RequireBothUsernameAndEmail = false };
            PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
        }
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        uiMgr.SignupPannelOC(1);
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        string errorLog = error.GenerateErrorReport();
        if (errorLog.Contains("Username not available"))
            signupError.text = "이미 존재하는 계정입니다.";
        else if (errorLog.Contains("between"))
            signupError.text = "비밀번호가 6자 이상 100자 이하이어야 합니다.";
        else if (errorLog.Contains("Password, EncryptedRequest"))
            signupError.text = ">계정이름이나 비밀번호를 입력해주세요.";
        else if (errorLog.Contains("Invalid username or password"))
            signupError.text = "계정이름이나 패스워드가 잘못되었습니다.";
        else
            Debug.Log(errorLog);
    }
    #endregion

    #region 계정의 타이틀 정보를 가져오는데 성공하면
    private void GetAccountSuccess(GetAccountInfoResult result)
    {
        string nickname = result.AccountInfo.TitleInfo.DisplayName;
        if (ReferenceEquals(nickname, null))
        {
            var request = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "NewCRT", "N" } } };
            PlayFabClientAPI.UpdateUserData(request, (result) => print("정보 저장 성공"), (Error) => print("정보 저장 실패"));
            uiMgr.CRPanel();
        }
        else
        {
            var request = new GetUserDataRequest() { PlayFabId = Singleton.Inst.Playfab_ID };
            PlayFabClientAPI.GetUserData(request, GetDataSuccess, (error) => print("실패"));
        }
    }

    void GetDataSuccess(GetUserDataResult result)
    {
        if (result.Data["NewCRT"].Value.Equals("N"))
        {
            uiMgr.CRPanel();
        }
        else if (result.Data["NewCRT"].Value.Equals("Y"))
        {
            photonMgr.Connect();
        }
    }
    #endregion

    #region 캐릭터 생성
    public void SButton(bool isOn)
    {
        (_sex.Equals("Q") ? models[0] : _sex.Equals("W") ? models[1] : _sex.Equals("E") ? models[2] : _sex.Equals("R") ? models[3] : _sex.Equals("Y") ? models[4] : models[5]).SetActive(false);
        if (isOn)
            _sex = toggleGroup.ActiveToggles().FirstOrDefault().name;
        (_sex.Equals("Q") ? models[0] : _sex.Equals("W") ? models[1] : _sex.Equals("E") ? models[2] : _sex.Equals("R") ? models[3] : _sex.Equals("Y") ? models[4] : models[5]).SetActive(true);
    }

    public void CharacterNickNameOk()
    {
        createError.text = "";
        var request = new UpdateUserTitleDisplayNameRequest { DisplayName = nickNameText.text };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, DisplayNameUpdateSuccess, (error) => createError.text = "사용할 수 없는 닉네임입니다.");
    }

    void DisplayNameUpdateSuccess(UpdateUserTitleDisplayNameResult result)
    {
        var request = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "Sex", _sex }, { "NickName", result.DisplayName }, { "Question", "문제모음" }, { "Ex", "0" }, { "Level", "1" } } };
        PlayFabClientAPI.UpdateUserData(request, NewCRTUpdateSuccess, (Error) => createError.text = "캐릭터를 생성에 실패했습니다.");
    }

    void NewCRTUpdateSuccess(UpdateUserDataResult result)
    {
        var request = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "NewCRT", "Y" } } };
        PlayFabClientAPI.UpdateUserData(request, (result) => photonMgr.Connect(), (Error) => createError.text = "유저 저장에 실패했습니다.");
    }
    #endregion
    #endregion
}