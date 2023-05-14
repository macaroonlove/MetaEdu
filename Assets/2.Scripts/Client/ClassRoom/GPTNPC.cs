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


        private string title = "����";
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
                recieveMsg = message.Content.Split("��")[i];
                data.Add(new Question("0", recieveMsg.Split("��")[0], recieveMsg.Split("��")[1], recieveMsg.Split("��")[2], recieveMsg.Split("��")[3], recieveMsg.Split("��")[4]));
            }
        }

        private async void SendReply()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = title + "�� ������ ������ ������ 5�� �����, \"�˹����ȴ�1�ȴ�2�ȴ�3�ȴ�4\" ���·� ����ϼ���.\n�������, \"���¾�迡�� ���� ���� �༺�� �����ΰ���?�ȱݼ���������ȭ���ȼ���\" �����Դϴ�.\n��1 �� �����̰�, �������� ��� �����Դϴ�. ������ ����� �򰥸� ������ �����ؾ� �մϴ�. ����� ������ ���ڼ��� 10�ڸ� �ѱ�� �ȵ˴ϴ�.������ ��Ȯ�� �´��� Ȯ���ϼ���. ������ ���� �̿ܿ��� ��� �ؽ�Ʈ�� �������� �ʽ��ϴ�.\n���� ������ ��ġ�� �ȉϴϴ�."
            };

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + title + "�� ������ ������ ������ 5�� �����, \"�˹����ȴ�1�ȴ�2�ȴ�3�ȴ�4\" ���·� ����ϼ���.\n�������, \"���¾�迡�� ���� ���� �༺�� �����ΰ���?�ȱݼ���������ȭ���ȼ���\" �����Դϴ�.\n��1 �� �����̰�, �������� ��� �����Դϴ�. ������ ����� �򰥸� ������ �����ؾ� �մϴ�. ����� ������ ���ڼ��� 10�ڸ� �ѱ�� �ȵ˴ϴ�.������ ��Ȯ�� �´��� Ȯ���ϼ���. ������ ���� �̿ܿ��� ��� �ؽ�Ʈ�� �������� �ʽ��ϴ�.";

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

                _stateText.text = "��ȣ�ۿ� Ű�� ���� npc�� ��ȭ�� �� �ֽ��ϴ�.";
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
                _talk.text = "�����Ϸ� �Դ�?";
                _choiceObj[0].SetActive(false);
                _choiceObj[1].SetActive(false);
                _choices[2].text = "�⼮���� Ǯ���Ծ��.";
                _choices[3].text = "�ƹ� �͵� �ƴϿ���.";
            }
            else if (state.Equals(State.QUIZ))
            {
                if (data.Count.Equals(0))
                {
                    _choicesObj.SetActive(false);
                    _talk.text = "������ �غ���� �ʾƼ�..\n���� �ִٰ� �÷�?";
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
                    _stateText.text = "��ȣ�ۿ� Ű�� ���� npc�� ��ȭ�� �� �ֽ��ϴ�.";
                    _npc.rotation = Quaternion.Euler(Vector3.zero);
                    state = State.IDLE;
                }
            }
            else if (state.Equals(State.QUIZ))
            {
                _choicesObj.SetActive(false);
                if (ans.Equals(slt - 1))
                {
                    _talk.text = "������! �����̾�.";
                }
                else
                {
                    _talk.text = "��.. �����̾�.\n������" + ansTxt + "(��)��.";
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