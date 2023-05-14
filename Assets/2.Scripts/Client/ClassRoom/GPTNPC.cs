using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

namespace OpenAI
{
    public class GPTNPC : MonoBehaviour
    {
        private enum State { IDLE, WHAT, QUIZ };
        private State state = State.IDLE;
        private List<Question> data = new List<Question>();
        public List<int> cho = new List<int>() { 2, 3, 4, 5 };
        private int ans = 2;
        private string ansTxt = "";
        [SerializeField] private Transform _npc;
        [SerializeField] private GameObject _talkPanel;
        [SerializeField] private TextMeshProUGUI _stateText;
        [SerializeField] private TextMeshProUGUI _talk;
        [SerializeField] private GameObject _choicesObj;
        [SerializeField] private TextMeshProUGUI[] _choices;
        [SerializeField] private GameObject[] _choiceObj;

        private PlayerController playerInput;
        private PlayerInputPress inputPress;


        private string title = "과학";
        private string recieveMsg = "";
        private OpenAIApi openai = new OpenAIApi();

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "You are a brilliant AI that creates new problems.\nMake sure your answer is exactly correct.\nAttempt to solve the following problem:";

        private void Start()
        {
            SendReply();
        }

        private void AppendMessage(ChatMessage message)
        {
            //Debug.Log(message.Content);
            for(int i = 1; i < 6; i++)
            {
                recieveMsg = message.Content.Split("▦")[i];
                data.Add(new Question("0", recieveMsg.Split("▥")[0], recieveMsg.Split("▥")[1], recieveMsg.Split("▥")[2], recieveMsg.Split("▥")[3], recieveMsg.Split("▥")[4]));
            }
        }

        private async void SendReply()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = title + "을 주제로 객관식 문제를 5개 만들고, \"▦문제▥답1▥답2▥답3▥답4\" 형태로 출력하세요.\n예를들면, \"▦태양계에서 가장 밝은 행성은 무엇인가요?▥금성▥지구▥화성▥수성\" 형태입니다.\n답1 이 정답이고, 나머지는 모두 오답입니다. 오답은 정답과 헷갈릴 정도로 정교해야 합니다. 정답과 오답의 글자수는 10자를 넘기면 안됩니다.정답이 정확히 맞는지 확인하세요. 생성한 문제 이외에는 어떠한 텍스트도 생성하지 않습니다.\n이전 문제와 겹치면 안됍니다."
            };

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + title + "을 주제로 객관식 문제를 5개 만들고, \"▦문제▥답1▥답2▥답3▥답4\" 형태로 출력하세요.\n예를들면, \"▦태양계에서 가장 밝은 행성은 무엇인가요?▥금성▥지구▥화성▥수성\" 형태입니다.\n답1 이 정답이고, 나머지는 모두 오답입니다. 오답은 정답과 헷갈릴 정도로 정교해야 합니다. 정답과 오답의 글자수는 10자를 넘기면 안됩니다.정답이 정확히 맞는지 확인하세요. 생성한 문제 이외에는 어떠한 텍스트도 생성하지 않습니다.";

            messages.Add(newMessage);

            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0301",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();

                messages.Add(message);
                AppendMessage(message);
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if(inputPress == null)
                {
                    other.TryGetComponent(out inputPress);
                    other.TryGetComponent(out playerInput);
                }

                _stateText.text = "상호작용 키를 눌러 npc와 대화할 수 있습니다.";
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (inputPress.interact && state.Equals(State.IDLE))
                {
                    playerInput.enabled = false;
                    _stateText.text = "";
                    state = State.WHAT;
                    TalkingToNPC();
                    _talkPanel.SetActive(true);
                    inputPress.interact = false;
                }
            }
        }

        private void TalkingToNPC()
        {
            if (state.Equals(State.WHAT))
            {
                _npc.LookAt(inputPress.transform);
                inputPress.transform.LookAt(_npc.transform);
                _choicesObj.SetActive(true);
                _talk.text = "무슨일로 왔니?";
                _choiceObj[0].SetActive(false);
                _choiceObj[1].SetActive(false);
                _choices[2].text = "출석퀴즈 풀러왔어요.";
                _choices[3].text = "아무 것도 아니에요.";
            }
            else if (state.Equals(State.QUIZ))
            {
                if (data.Count.Equals(0))
                {
                    _choicesObj.SetActive(false);
                    _talk.text = "문제가 준비되지 않아서..\n조금 있다가 올래?";
                    StartCoroutine(WaitForInput());
                    return;
                }
                _choiceObj[0].SetActive(true);
                _choiceObj[1].SetActive(true);
                int b = Random.Range(0, data.Count);
                string[] a = data[b].load();
                _talk.text = a[1];
                ansTxt = a[2];

                UtilClass.Shuffle(cho);
                for (int i = 0; i < 4; i++)
                {
                    if (cho[i].Equals(2))
                    {
                        ans = i;
                    }
                        
                    _choices[i].text = a[cho[i]];
                }
                data.RemoveAt(b);
            }
        }

        public void SelectChoice(int slt)
        {
            if (state.Equals(State.WHAT))
            {
                if (slt.Equals(3))
                {
                    state = State.QUIZ;
                    TalkingToNPC();
                }
                else if (slt.Equals(4))
                {
                    _talkPanel.SetActive(false);
                    playerInput.enabled = true;
                    _stateText.text = "상호작용 키를 눌러 npc와 대화할 수 있습니다.";
                    _npc.rotation = Quaternion.Euler(Vector3.zero);
                    state = State.IDLE;
                }
            }
            else if (state.Equals(State.QUIZ))
            {
                _choicesObj.SetActive(false);
                if (ans.Equals(slt - 1))
                {
                    _talk.text = "축하해! 정답이야.";
                }
                else
                {
                    _talk.text = "땡.. 오답이야.\n정답은" + ansTxt + "(이)야.";
                }
                if(data.Count.Equals(3))
                    SendReply();
                StartCoroutine(WaitForInput());
            }
        }

        IEnumerator WaitForInput()
        {
            while (!inputPress.interact)
            {
                yield return null;
            }

            state = State.WHAT;
            TalkingToNPC();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInput.enabled = true;
                _stateText.text = "";
            }
        }
    }
}