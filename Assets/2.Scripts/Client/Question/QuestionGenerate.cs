using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class QuestionGenerate : MonoBehaviour
{
    public List<Question> questions = new List<Question>();

    public TMP_InputField title;
    public TMP_Dropdown type;
    public TMP_InputField problem;

    public TMP_InputField[] multiAnswer;
    public TMP_InputField[] shortAnswer;
    public TMP_InputField essayAnswer;
    public TMP_InputField keyword;

    public GameObject[] ansPanel;
    private List<string> result = new List<string>();
    
    public Transform quizContent;
    public GameObject delButton;
    public TextMeshProUGUI exitPopup;
    public TextMeshProUGUI errorToast;
    public Button[] pnpButton;
    public int currentPage = 1;
    public int maxPage = 1;

    private bool _isCreate = true;

    private PlayerInput _playerInput;

    public void InputLimit(bool tf)
    {
        if(ReferenceEquals(_playerInput, null))
        {
            GameObject.FindGameObjectWithTag("Player").TryGetComponent(out _playerInput);
        }
        gameObject.SetActive(tf);
        _playerInput.enabled = !tf;
    }

    #region 문제 생성 & 수정
    public void CreateQuiz()
    {
        questions.Clear();
        currentPage = 1;
        maxPage = 1;
        title.text = "";
        TempSave();
        pnpButton[0].interactable = false;
        pnpButton[2].gameObject.SetActive(true);
        ResetQuiz();
        InputLimit(true);
        _isCreate = true;
    }

    public void UpdateQuiz()
    {
        questions.Clear();
        currentPage = 1;
        maxPage = 0;

        string data = "";
        for (int i = 0; i < QuestionManager.Inst.questionDatas[QuestionManager.Inst.selectQuestion].answer.Count; i++)
            data += QuestionManager.Inst.questionDatas[QuestionManager.Inst.selectQuestion].answer[i];

        LoadQuiz(QuestionManager.Inst.questionDatas[QuestionManager.Inst.selectQuestion]);
        InputLimit(true);
        _isCreate = false;
    }

    void LoadQuiz(QuestionData quiz)
    {
        title.text = quiz.title;
        for (int i = 0; i < quiz.answer.Count; i++)
        {
            string[] q = quiz.answer[i].Split("▥");
            questions.Add(new Question(q[0], q[1], q[2], q[3], q[4], q[5], q[6], q[7], q[8], q[9], q[10]));
            maxPage++;
        }
        QuizRenewal(0);
    }
    #endregion

    public void PNP(int num)
    {
        TempSave();
        if (num.Equals(-2)) --currentPage; // 이전
        else if (num.Equals(-1)) ++currentPage; // 다음
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
            QuizInit(questions[currentPage - 1].load());
        }
        catch
        {
            if(num.Equals(0))
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
        if (init[0].Equals("0"))
        {
            type.value = 0;
            ansPanel[0].SetActive(true);
            ansPanel[1].SetActive(false);
            ansPanel[2].SetActive(false);
            for (int i = 0; i < 4; i++)
            {
                if (!init[i + 2].Equals(""))
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
        else if (init[0].Equals("1"))
        {
            type.value = 1;
            ansPanel[0].SetActive(false);
            ansPanel[1].SetActive(true);
            ansPanel[2].SetActive(false);
            for (int i = 0; i < 9; i++)
            {
                if (!init[i + 2].Equals(""))
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
        if(questions.Count < currentPage) // 새로운 페이지가 추가되면 리스트 추가
        {
            if (type.value.Equals(0))
                questions.Add(new Question(type.value + "", problem.text, multiAnswer[0].text, multiAnswer[1].text, multiAnswer[2].text, multiAnswer[3].text));
            else if (type.value.Equals(1))
                questions.Add(new Question(type.value + "", problem.text, shortAnswer[0].text, shortAnswer[1].text, shortAnswer[2].text, shortAnswer[3].text, shortAnswer[4].text, shortAnswer[5].text, shortAnswer[6].text, shortAnswer[7].text, shortAnswer[8].text));
            else
                questions.Add(new Question(type.value + "", problem.text, essayAnswer.text, keyword.text));
        }
        else
        {
            if (type.value.Equals(0))
                questions[currentPage - 1] = new Question(type.value + "", problem.text, multiAnswer[0].text, multiAnswer[1].text, multiAnswer[2].text, multiAnswer[3].text);
            else if (type.value.Equals(1))
                questions[currentPage - 1] = new Question(type.value + "", problem.text, shortAnswer[0].text, shortAnswer[1].text, shortAnswer[2].text, shortAnswer[3].text, shortAnswer[4].text, shortAnswer[5].text, shortAnswer[6].text, shortAnswer[7].text, shortAnswer[8].text);
            else
                questions[currentPage - 1] = new Question(type.value + "", problem.text, essayAnswer.text, keyword.text);
        }
    }

    public void Delete()
    {
        TempSave();
        questions.RemoveAt(currentPage - 1);
        maxPage--;
        if (maxPage.Equals(1)) delButton.SetActive(false);
        if (currentPage > maxPage) currentPage--;
        QuizRenewal(0);
    }

    public void PopUp()
    {
        string tl = title.text.Equals("") ? "이름없음" : title.text;
        exitPopup.text = $"\'{tl}\'의\n변경내용 을 저장하시겠습니까?";
    }

    public async void Save()
    {
        TempSave();
        result.Clear();
        for(int i = 0; i < questions.Count; i++)
            result.Add(questions[i].arrtostr());

        QuestionData data = new QuestionData();
        data.title = title.text;
        data.answer.AddRange(result);
        data.download = 0;
        data.owner = Singleton.Inst.displayId;

        if (!_isCreate) // update
        {
            quizContent.GetChild(QuestionManager.Inst.selectQuestion).GetChild(1).GetComponent<TextMeshProUGUI>().text = title.text;
            QuestionManager.Inst.questionDatas[QuestionManager.Inst.selectQuestion] = data;
        }
        else // create
        {   
            QuestionManager.Inst.questionDatas.Add(data);
        }

        if (await QuestionManager.Inst.SetUserData())
        {
            QuestionManager.Inst.QuestionInit();
            InputLimit(false);
        }
        else
        {
            errorToast.text = "저장에 실패하셨습니다."; 
            errorToast.transform.parent.gameObject.SetActive(true);
        }
    }

    public async void Remove()
    {
        for (int i = 0; i < quizContent.childCount; i++)
        {
            quizContent.GetChild(i).gameObject.SetActive(false);
        }
        QuestionManager.Inst.questionDatas.RemoveAt(QuestionManager.Inst.selectQuestion);

        if (await QuestionManager.Inst.SetUserData())
        {
            QuestionManager.Inst.QuestionInit();
            InputLimit(false);
        }
        else
        {
            errorToast.text = "삭제에 실패하셨습니다.";
            errorToast.transform.parent.gameObject.SetActive(true);
        }
    }
}