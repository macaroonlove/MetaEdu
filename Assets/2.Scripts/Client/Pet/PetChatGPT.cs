using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenAI
{
    public class PetChatGPT : MonoBehaviour
    {
        public InputField inputField;
        public Button button;
        public TextMeshProUGUI textArea;
        public GameObject choice1;
        public GameObject choice2;

        private OpenAIApi openai = new OpenAIApi("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", "org-GfZ86Cd8GkAqZHYHn6X5JdHu");
        private string userInput;
        private string Instruction = "In the chat room, please act as a teacher, guide, helper, who helps people according to the context of the conversation.\nQ: ";
        private PlayerController Player;
        private PetController _petController;
        private void Start()
        {
            button.onClick.AddListener(SendReply);
        }


        private async void SendReply()
        {
            userInput = inputField.text;
            Instruction += $"{userInput}\nA: ";

            textArea.text = ".....";
            inputField.text = "";

            button.enabled = false;
            inputField.enabled = false;

            var completionResponse = await openai.CreateCompletion(new CreateCompletionRequest()
            {
                Prompt = Instruction,
                Model = "text-davinci-003",
                MaxTokens = 500
            });

            textArea.text = completionResponse.Choices[0].Text;
            Instruction += $"{completionResponse.Choices[0].Text}\nQ: ";

            button.enabled = true;
            inputField.enabled = true;
        }

        public void OffSpeechBubble()
        {
            if(ReferenceEquals(Player, null))
            {
                Player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
                _petController = GameObject.FindWithTag("Pet").GetComponent<PetController>();
            }
            _petController.SpeechBubble.SetActive(false);
            Player.GPTAnim.SetBool(Player.animGPT, false);
            Player.GPTAnim.SetBool(Player.animInputUI, false);
            choice1.SetActive(true);
            Player.GPTState = false;
            Player.vcamPetGPT.Priority = 5;
            textArea.text = "";
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
            choice1.SetActive(false);
        }
    }
}
