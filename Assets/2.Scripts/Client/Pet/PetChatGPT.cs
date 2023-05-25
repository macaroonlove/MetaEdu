using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;

namespace OpenAI
{
    public class PetChatGPT : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Button button;
        [SerializeField] private ScrollRect scroll;
        [SerializeField] private TextMeshProUGUI textArea;
        [SerializeField] private GameObject choice1;
        [SerializeField] private GameObject choice2;

        private OpenAIApi openai = new OpenAIApi("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", "org-GfZ86Cd8GkAqZHYHn6X5JdHu");

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "너의 역할은 'ChatGPT를 활용한 PBL 수업을 위해 개발된 교육용 메타버스 플랫폼에서 ChatGPT와 G-러닝을 활용한 자기주도적 학습을 위해서 너가 이제 플레이어들에 도움을 주는 선생님 및 친구' 이야";

        private PlayerController Player;
        private PetController _petController;
        private void Start()
        {
            button.onClick.AddListener(SendReply);
            textArea.text = "무엇을 도와드릴까요?";
        }

        private void AppendMessage(ChatMessage message)
        {
            textArea.text = message.Content;
        }

        private async void SendReply()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };

            textArea.text = ".......";

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text;

            messages.Add(newMessage);

            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;

            // Complete the instruction
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

            button.enabled = true;
            inputField.enabled = true;
        }
        public void OffSpeechBubble()
        {
            if (ReferenceEquals(Player, null))
            {
                Player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
                _petController = GameObject.FindWithTag("Pet").GetComponent<PetController>();
            }
            _petController.SpeechBubble.SetActive(false);
            Player.GPTAnim.SetBool(Player.animGPT, false);
            Player.GPTAnim.SetBool(Player.animInputUI, false);
            choice2.SetActive(true);
            Player.GPTState = false;
            Player.vcamPetGPT.Priority = 5;
            textArea.text = "무엇을 도와드릴까요?";
            inputField.text = "";
        }

        public void OnInputUI()
        {
            if (ReferenceEquals(Player, null))
            {
                Player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
                _petController = GameObject.FindWithTag("Pet").GetComponent<PetController>();
            }
            Player.GPTAnim.SetBool(Player.animInputUI, true);
            choice2.SetActive(false);
        }
    }
}

