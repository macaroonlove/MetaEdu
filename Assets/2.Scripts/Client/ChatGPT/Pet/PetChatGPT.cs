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

        private OpenAIApi openai;

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "선생님역할을해줘";

        private PlayerController Player;
        private PetController _petController;
        private void Start()
        {
            openai =  new OpenAIApi(APIKeyManager.Inst.GetApiKey(), APIKeyManager.Inst.GetOrganizeKey());
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

