using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.Json;
using PlayFab.ClientModels;
using System.Threading.Tasks;
using System;
using System.IO;
using Photon.Pun;
using System.Runtime.Serialization.Formatters.Binary;

public class QuestionManager : MonoBehaviour
{
    #region �̱���
    private static QuestionManager _instance;

    public static QuestionManager Inst
    {
        get
        {
            if (ReferenceEquals(_instance, null))
            {
                _instance = new QuestionManager();
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (ReferenceEquals(_instance, null))
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion

    public List<QuestionData> questionDatas = new List<QuestionData>();
    public List<QuestionData> titleDatas = new List<QuestionData>();
    public int selectQuestion = 0;

    private Transform quizContent;

    void Start()
    {
        GetUserData();
        GetTitleData();
    }

    #region ���� Ÿ��Ʋ ������
    void GetUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
        }, result => {
            string data = result.Data["Question"].Value;
            if(data is not null)
            {
                questionDatas = PlayFabSimpleJson.DeserializeObject<List<QuestionData>>(data);
            }
            QuestionInit();
        }, error => {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public async Task<bool> SetUserData()
    {
        var tcs = new TaskCompletionSource<bool>();

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>() { { "Question", PlayFabSimpleJson.SerializeObject(questionDatas) } }
        }, result =>
        {
            tcs.SetResult(true);
        }, error =>
        {
            tcs.SetResult(false);
        });

        return await tcs.Task;
    }

    public void QuestionInit()
    {
        if (quizContent == null)
        {
            quizContent = GameObject.Find("Quiz_Content").transform;
            quizContent.parent.parent.parent.gameObject.SetActive(false);
        }
        for (int i = 0; i < questionDatas.Count; i++)
        {
            Transform quizSet = quizContent.GetChild(i);
            quizSet.gameObject.SetActive(true);
            quizSet.GetChild(1).GetComponent<TextMeshProUGUI>().text = questionDatas[i].title;
        }
    }
    #endregion

    #region Ÿ��Ʋ ������
    public async Task<bool> GetTitleData()
    {
        var tcs = new TaskCompletionSource<bool>();

        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest()
        {
        }, result => {
            string data = result.Data["Question"];
            if (data is not null)
            {
                titleDatas = PlayFabSimpleJson.DeserializeObject<List<QuestionData>>(data);
            }
            tcs.SetResult(true);
        }, error => {
            Debug.Log(error.GenerateErrorReport());
            tcs.SetResult(false);
        });

        return await tcs.Task;
    }

    public async Task<bool> AddQuestion(string title, string desc)
    {
        var tcs = new TaskCompletionSource<bool>();

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "AddTitleData",
            FunctionParameter = new { questionTitle = title, questionAnswer = questionDatas[selectQuestion].answer, questionDesc = desc, questionDown = 0, questionOwner = Singleton.Inst.displayId, questionDownloader = "" },
            GeneratePlayStreamEvent = true // PlayStream �̺�Ʈ ���� ����
        };

        PlayFabClientAPI.ExecuteCloudScript(request, 
        result => {
            tcs.SetResult(true);
        }, error => {
            tcs.SetResult(false);
        });

        return await tcs.Task;
    }

    public async Task<bool> DownloadPlus(int idx)
    {
        var tcs = new TaskCompletionSource<bool>();

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "DownloadQuestion",
            FunctionParameter = new { questionIndex = idx, questionDownloader = Singleton.Inst.displayId },
            GeneratePlayStreamEvent = true // PlayStream �̺�Ʈ ���� ����
        };

        PlayFabClientAPI.ExecuteCloudScript(request,
        result => {
            tcs.SetResult(true);
        }, error => {
            tcs.SetResult(false);
        });

        return await tcs.Task;
    }

    public async Task<bool> DeleteQuestion(int idx)
    {
        var tcs = new TaskCompletionSource<bool>();

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "DeleteTitleData",
            FunctionParameter = new { questionIndex = idx },
            GeneratePlayStreamEvent = true // PlayStream �̺�Ʈ ���� ����
        };

        PlayFabClientAPI.ExecuteCloudScript(request,
        result => {
            tcs.SetResult(true);
        }, error => {
            tcs.SetResult(false);
        });

        return await tcs.Task;
    }
    #endregion
}

[System.Serializable]
public class QuestionData
{
    public string title;
    public List<string> answer = new List<string>();
    public string desc;
    public int download;
    public string owner;
    public string downloader;
}

[System.Serializable]
public class Question
{
    string[] tqa = new string[11];
    #region ������
    public Question(string type, string quest, string ans1, string ans2, string ans3, string ans4)
    {
        tqa[0] = type;
        tqa[1] = quest;
        tqa[2] = ans1;
        tqa[3] = ans2;
        tqa[4] = ans3;
        tqa[5] = ans4;
    }

    public Question(string type, string quest, string ans1, string ans2, string ans3, string ans4, string ans5, string ans6, string ans7, string ans8, string ans9)
    {
        tqa[0] = type;
        tqa[1] = quest;
        tqa[2] = ans1;
        tqa[3] = ans2;
        tqa[4] = ans3;
        tqa[5] = ans4;
        tqa[6] = ans5;
        tqa[7] = ans6;
        tqa[8] = ans7;
        tqa[9] = ans8;
        tqa[10] = ans9;
    }

    public Question(string type, string quest, string keyword, string num)
    {
        tqa[0] = type;
        tqa[1] = quest;
        tqa[2] = keyword;
        tqa[3] = num;
    }
    #endregion
    public string[] load()
    {
        return tqa;
    }

    public string arrtostr()
    {
        string str = tqa[0] + "��" + tqa[1] + "��" + tqa[2] + "��" + tqa[3] + "��" + tqa[4] + "��" + tqa[5] + "��" + tqa[6] + "��" + tqa[7] + "��" + tqa[8] + "��" + tqa[9] + "��" + tqa[10];
        return str;
    }
}