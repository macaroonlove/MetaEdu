using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

[System.Serializable]
public class Question
{
    string[] arr = new string[11];
    #region 생성자
    public Question(string type, string desc, string ans1, string ans2, string ans3, string ans4)
    {
        arr[0] = type;
        arr[1] = desc;
        arr[2] = ans1;
        arr[3] = ans2;
        arr[4] = ans3;
        arr[5] = ans4;
    }

    public Question(string type, string desc, string ans1, string ans2, string ans3, string ans4, string ans5, string ans6, string ans7, string ans8, string ans9)
    {
        arr[0] = type;
        arr[1] = desc;
        arr[2] = ans1;
        arr[3] = ans2;
        arr[4] = ans3;
        arr[5] = ans4;
        arr[6] = ans5;
        arr[7] = ans6;
        arr[8] = ans7;
        arr[9] = ans8;
        arr[10] = ans9;
    }

    public Question(string type, string desc, string keyword, string num)
    {
        arr[0] = type;
        arr[1] = desc;
        arr[2] = keyword;
        arr[3] = num;
    }
    #endregion
    public string[] load()
    {
        return arr;
    }

    public string arrtostr()
    {
        string str = "▤" + arr[0] + "▥" + arr[1] + "▥" + arr[2] + "▥" + arr[3] + "▥" + arr[4] + "▥" + arr[5] + "▥" + arr[6] + "▥" + arr[7] + "▥" + arr[8] + "▥" + arr[9] + "▥" + arr[10];
        return str;
    }
}

public class QuestionManager : MonoBehaviour
{
    public List<Question> data = new List<Question>();

    public TMP_InputField title;
    public TMP_Dropdown type;
    public TMP_InputField problem;

    public TMP_InputField[] multiAnswer;
    public TMP_InputField[] shortAnswer;
    public TMP_InputField essayAnswer;
    public TMP_InputField keyword;

    public GameObject[] ansPanel;
    public string result = "";
    
    public Transform quizContent;
    public GameObject delButton;
    public TextMeshProUGUI exitPopup;
    public TextMeshProUGUI errorToast;
    public Button[] pnpButton;
    public int currentPage = 1;
    public int maxPage = 1;

    private bool _isCreate = true;

    #region 문제 생성 & 수정 & 삭제
    public void CreateQuiz()
    {
        data.Clear();
        currentPage = 1;
        maxPage = 1;
        gameObject.SetActive(true);
        TempSave();
        pnpButton[0].interactable = false;
        pnpButton[2].gameObject.SetActive(true);
        title.text = "";
        ResetQuiz();
        _isCreate = true;
    }

    public void UpdateQuiz()
    {
        data.Clear();
        currentPage = 1;
        maxPage = 0;
        gameObject.SetActive(true);
        LoadQuiz(Singleton.Inst.question[Singleton.Inst.currSelect]);
        _isCreate = false;
    }

    void LoadQuiz(string quiz)
    {
        title.text = quiz.Split("▤")[0];
        for(int i = 1; i < quiz.Split("▤").Length; i++)
        {
            string[] q = quiz.Split("▤")[i].Split("▥");
            data.Add(new Question(q[0], q[1], q[2], q[3], q[4], q[5], q[6], q[7], q[8], q[9], q[10]));
            maxPage++;
        }
        QuizRenewal(0);
    }
    #endregion

    public void PNP(int num)
    {
        TempSave();
        if (num == -2) --currentPage; // 이전
        else if (num == -1) ++currentPage; // 다음
        else // 새로만들기
        {
            ++currentPage;
            ++maxPage;
        }
        if (maxPage > 1) delButton.SetActive(true);
        QuizRenewal(num);
    }

    void QuizRenewal(int num)
    {
        pnpButton[0].interactable = (currentPage <= 1) ? false : true;
        pnpButton[2].gameObject.SetActive((currentPage >= maxPage) ? true : false);

        try
        {
            QuizInit(data[currentPage - 1].load());
        }
        catch
        {
            if(num == 0)
            {
                ResetQuiz();
            }
        }
    }

    void ResetQuiz()
    {
        problem.text = "";
        for (int i = 0; i < 4; i++)
            multiAnswer[i].text = "";
        for (int i = 0; i < 9; i++)
            shortAnswer[i].text = "";
        essayAnswer.text = "";
        keyword.text = "";
    }

    void QuizInit(string[] init)
    {
        problem.text = init[1];
        if (init[0] == "0")
        {
            type.value = 0;
            ansPanel[0].SetActive(true);
            ansPanel[1].SetActive(false);
            ansPanel[2].SetActive(false);
            for (int i = 0; i < 4; i++)
            {
                if (init[i + 2] != "")
                {
                    multiAnswer[i].gameObject.SetActive(true);
                    multiAnswer[i].text = init[i + 2];
                }
                else
                {
                    if(i > 1)
                        multiAnswer[i].gameObject.SetActive(false);
                    multiAnswer[i].text = "";
                }
            }
                
        }
        else if (init[0] == "1")
        {
            type.value = 1;
            ansPanel[0].SetActive(false);
            ansPanel[1].SetActive(true);
            ansPanel[2].SetActive(false);
            for (int i = 0; i < 9; i++)
            {
                if (init[i + 2] != "")
                {
                    shortAnswer[i].gameObject.SetActive(true);
                    shortAnswer[i].text = init[i + 2];
                }
                else
                {
                    shortAnswer[i].gameObject.SetActive(false);
                    shortAnswer[i].text = "";
                }
            }
            
        }
        else
        {
            type.value = 2;
            ansPanel[0].SetActive(false);
            ansPanel[1].SetActive(false);
            ansPanel[2].SetActive(true);
            essayAnswer.text = init[2];
            keyword.text = init[3];
        }
    }

    public void TempSave()
    {
        if(data.Count < currentPage) // 새로운 페이지가 추가되면 리스트 추가
        {
            if (type.value == 0)
                data.Add(new Question(type.value + "", problem.text, multiAnswer[0].text, multiAnswer[1].text, multiAnswer[2].text, multiAnswer[3].text));
            else if (type.value == 1)
                data.Add(new Question(type.value + "", problem.text, shortAnswer[0].text, shortAnswer[1].text, shortAnswer[2].text, shortAnswer[3].text, shortAnswer[4].text, shortAnswer[5].text, shortAnswer[6].text, shortAnswer[7].text, shortAnswer[8].text));
            else
                data.Add(new Question(type.value + "", problem.text, essayAnswer.text, keyword.text));
        }
        else
        {
            if (type.value == 0)
                data[currentPage - 1] = new Question(type.value + "", problem.text, multiAnswer[0].text, multiAnswer[1].text, multiAnswer[2].text, multiAnswer[3].text);
            else if (type.value == 1)
                data[currentPage - 1] = new Question(type.value + "", problem.text, shortAnswer[0].text, shortAnswer[1].text, shortAnswer[2].text, shortAnswer[3].text, shortAnswer[4].text, shortAnswer[5].text, shortAnswer[6].text, shortAnswer[7].text, shortAnswer[8].text);
            else
                data[currentPage - 1] = new Question(type.value + "", problem.text, essayAnswer.text, keyword.text);
        }
    }

    public void Delete()
    {
        TempSave();
        data.RemoveAt(currentPage - 1);
        maxPage--;
        if (maxPage == 1) delButton.SetActive(false);
        if (currentPage > maxPage) currentPage--;
        QuizRenewal(0);
    }

    public void PopUp()
    {
        string tl = title.text == "" ? "이름없음" : title.text;
        exitPopup.text = $"\'{tl}\'의\n변경내용 을 저장하시겠습니까?";
    }

    public void Save()
    {
        TempSave();
        result = title.text;
        for(int i = 0; i < currentPage; i++)
            result += data[i].arrtostr();

        if (!_isCreate)
        {
            quizContent.GetChild(Singleton.Inst.currSelect).GetChild(1).GetComponent<TextMeshProUGUI>().text = title.text;
            Singleton.Inst.questions = "";
            Singleton.Inst.question[Singleton.Inst.currSelect] = result;
            for(int i = 0; i < Singleton.Inst.question.Count; i++)
                Singleton.Inst.questions += "▦" + Singleton.Inst.question[i];
        }
        else
        {
            Singleton.Inst.questions += "▦" + result;
        }
        var request = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "Question", Singleton.Inst.questions } } };
        PlayFabClientAPI.UpdateUserData(request, (result) => { gameObject.SetActive(false); Singleton.Inst.QuestionInit(); }, (Error) => { errorToast.text = "저장에 실패하셨습니다."; errorToast.transform.parent.gameObject.SetActive(true); });
    }

    public void Remove()
    {
        Singleton.Inst.questions = "없음";
        for (int i = 0; i < quizContent.childCount; i++)
        {
            quizContent.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < Singleton.Inst.question.Count; i++)
        {
            if(i == Singleton.Inst.currSelect)
                Singleton.Inst.questions += "";
            else
                Singleton.Inst.questions += "▦" + Singleton.Inst.question[i];
        }
        var request = new UpdateUserDataRequest() { Data = new Dictionary<string, string>() { { "Question", Singleton.Inst.questions } } };
        PlayFabClientAPI.UpdateUserData(request, (result) => { Singleton.Inst.QuestionInit(); }, (Error) => { Debug.Log("삭제실패"); });
    }
}
