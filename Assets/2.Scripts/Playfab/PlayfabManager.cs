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

    [Header("�α���&ȸ������")]
    public TMP_InputField lID_Input;
    public TMP_InputField lPWInput, sUsernameInput, sPWInput, srPWInput;
    public TextMeshProUGUI loginError, signupError;
    private string _lUserName, _lPassword, _sUserName, _sPassword, _srPassword;

    [Header("ĳ���� ����")]
    public TMP_InputField nickNameText;
    public ToggleGroup toggleGroup;
    public TextMeshProUGUI createError;
    public GameObject[] models;
    private string _sex = "Q";

    #region �α���&ȸ������
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

    #region �α���
    public void LoginButton()
    {
        var request = new LoginWithPlayFabRequest { Username = _lUserName, Password = _lPassword };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
    }
    // �α��ο� �����ϸ�
    private void OnLoginSuccess(LoginResult result)
    {
        Singleton.Inst.Playfab_ID = result.PlayFabId;

        var request = new GetAccountInfoRequest { Username = _lUserName };
        PlayFabClientAPI.GetAccountInfo(request, GetAccountSuccess, (PlayFabError error) => { Debug.Log("����� ���� �������� ����"); });
    }

    private void OnLoginFailure(PlayFabError error) => loginError.text = "�����̸��̳� ��й�ȣ��\n��ġ���� �ʽ��ϴ�.";
    #endregion

    #region ȸ������
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
            signupError.text = "�̹� �����ϴ� �����Դϴ�.";
        else if (errorLog.Contains("between"))
            signupError.text = "��й�ȣ�� 6�� �̻� 100�� �����̾�� �մϴ�.";
        else if (errorLog.Contains("Password, EncryptedRequest"))
            signupError.text = ">�����̸��̳� ��й�ȣ�� �Է����ּ���.";
        else if (errorLog.Contains("Invalid username or password"))
            signupError.text = "�����̸��̳� �н����尡 �߸��Ǿ����ϴ�.";
        else
            Debug.Log(errorLog);
    }
    #endregion

    #region ������ Ÿ��Ʋ ������ �������µ� �����ϸ�
    private void GetAccountSuccess(GetAccountInfoResult result)
    {
        string nickname = result.AccountInfo.TitleInfo.DisplayName;
        if (ReferenceEquals(nickname, null))
        {
            var request = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "NewCRT", "N" } } };
            PlayFabClientAPI.UpdateUserData(request, (result) => print("���� ���� ����"), (Error) => print("���� ���� ����"));
            uiMgr.CRPanel();
        }
        else
        {
            var request = new GetUserDataRequest() { PlayFabId = Singleton.Inst.Playfab_ID };
            PlayFabClientAPI.GetUserData(request, GetDataSuccess, (error) => print("����"));
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

    #region ĳ���� ����
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
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, DisplayNameUpdateSuccess, (error) => createError.text = "����� �� ���� �г����Դϴ�.");
    }

    void DisplayNameUpdateSuccess(UpdateUserTitleDisplayNameResult result)
    {
        var request = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "Sex", _sex }, { "NickName", result.DisplayName }, { "Question", "��������" }, { "Ex", "0" }, { "Level", "1" } } };
        PlayFabClientAPI.UpdateUserData(request, NewCRTUpdateSuccess, (Error) => createError.text = "ĳ���͸� ������ �����߽��ϴ�.");
    }

    void NewCRTUpdateSuccess(UpdateUserDataResult result)
    {
        var request = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "NewCRT", "Y" } } };
        PlayFabClientAPI.UpdateUserData(request, (result) => photonMgr.Connect(), (Error) => createError.text = "���� ���忡 �����߽��ϴ�.");
    }
    #endregion
    #endregion
}