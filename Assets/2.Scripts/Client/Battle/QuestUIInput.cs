using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestUIInput : MonoBehaviour, IPointerClickHandler
{
    public delegate void MonsterDie();
    public static event MonsterDie OnMonsterDie;

    private GameObject HighLevelQuestionUI;

    QuizManager quizManager;
    // Start is called before the first frame update
    void Awake()
    {
        quizManager = GameObject.Find("QuizManager").GetComponent<QuizManager>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnMonsterDie();
            quizManager.QuestionPanel.SetActive(false);
            quizManager.ChoiceAnswerPanel.SetActive(false);
            Camera.main.cullingMask = -1;
        }
    }
}
