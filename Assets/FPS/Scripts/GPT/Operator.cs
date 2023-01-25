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

        [Tooltip("メッセージ間隔（秒）")]
        [SerializeField] private float messageInterval = 30f;

        private float messageTimer = 0f;

        private float wholeTimer = 0f;

        //作戦開始
        void Start()
        {
            GPTManager.Instance.GetText(
                "You are an operator of a game player as a soldier. The game just started now. Say greetings to the player.",
                UpdateMessage);
        }

        private void Update()
        {
            //タイマー進める
            messageTimer += Time.deltaTime;
            wholeTimer += Time.deltaTime;

            //時間が来たら、メッセージ更新
            if(messageTimer >= messageInterval)
            {
                FetchNextMessage();

                //タイマーリセット
                messageTimer = 0f;
            }
        }

        /// <summary>
        /// 状況をインプットし、オペレーターを話させる
        /// </summary>
        private void FetchNextMessage()
        {
            //HPをパーセンテージで
            float playerHealthRatio = playerHealth.GetRatio() * 100f;
            float enemyHealthRatio = enemyHealth.GetRatio() * 100f;

            //経過時間
            int passedTime = (int)wholeTimer;

            //プロンプト
            string prompt = "You are an operator of a game player as a soldier. " +
                $"{passedTime} seconds has passed from the game started. " +
                $"Player's HP is now {playerHealthRatio}%. " +
                $"Enemy's HP is now {enemyHealthRatio}%. " +
                "Say something to the player within 30 words.";

            //ChatGPTに送信
            GPTManager.Instance.GetText(prompt, UpdateMessage);
        }

        /// <summary>
        /// テキストを取得したら、UIを更新
        /// </summary>
        /// <param name="text">GPTから得られたテキスト</param>
        private void UpdateMessage(string text)
        {
            messageUI.text = text;
        }
    }
}