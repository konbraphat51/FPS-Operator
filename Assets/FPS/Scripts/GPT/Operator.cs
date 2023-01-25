using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.FPS.Game;

namespace GPT
{
    public class Operator : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageUI;

        [SerializeField] private Health playerHealth;
        [SerializeField] private Health enemyHealth;

        [Tooltip("���b�Z�[�W�Ԋu�i�b�j")]
        [SerializeField] private float messageInterval = 30f;

        private float messageTimer = 0f;

        private float wholeTimer = 0f;

        //���J�n
        void Start()
        {
            GPTManager.Instance.GetText(
                "You are an operator of a game player as a soldier. The game just started now. Say greetings to the player.",
                UpdateMessage);
        }

        private void Update()
        {
            //�^�C�}�[�i�߂�
            messageTimer += Time.deltaTime;
            wholeTimer += Time.deltaTime;

            //���Ԃ�������A���b�Z�[�W�X�V
            if(messageTimer >= messageInterval)
            {
                FetchNextMessage();

                //�^�C�}�[���Z�b�g
                messageTimer = 0f;
            }
        }

        /// <summary>
        /// �󋵂��C���v�b�g���A�I�y���[�^�[��b������
        /// </summary>
        private void FetchNextMessage()
        {
            //HP���p�[�Z���e�[�W��
            float playerHealthRatio = playerHealth.GetRatio() * 100f;
            float enemyHealthRatio = enemyHealth.GetRatio() * 100f;

            //�o�ߎ���
            int passedTime = (int)wholeTimer;

            //�v�����v�g
            string prompt = "You are an operator of a game player as a soldier. " +
                $"{passedTime} seconds has passed from the game started. " +
                $"Player's HP is now {playerHealthRatio}%. " +
                $"Enemy's HP is now {enemyHealthRatio}%. " +
                "Say something to the player within 30 words.";

            //ChatGPT�ɑ��M
            GPTManager.Instance.GetText(prompt, UpdateMessage);
        }

        /// <summary>
        /// �e�L�X�g���擾������AUI���X�V
        /// </summary>
        /// <param name="text">GPT���瓾��ꂽ�e�L�X�g</param>
        private void UpdateMessage(string text)
        {
            messageUI.text = text;
        }
    }
}