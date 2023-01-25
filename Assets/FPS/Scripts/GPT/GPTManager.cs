using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;

namespace GPT
{
    public class GPTManager : SingletonMonoBehaviour<GPTManager>
    {
        //テキストを得たら、Invoke
        //インデックスはID
        Dictionary<int, UnityEvent<string>> onTextGots = new Dictionary<int, UnityEvent<string>>();

        //次に登録するID
        private int nextID = 0;

        //アクセス先
        const string API_END_POINT = "https://api.openai.com/v1/completions";
        const string API_KEY = "ENTER KEY HERE";

        /// <summary>
        /// プロンプトをChatGPTに送り、得られたテキストをtextGetterに渡す
        /// </summary>
        /// <param name="prompt">ChatGPTに入力するプロンプト</param>
        /// <param name="textGetter">取得した際に呼ばれるメソッド。引数に得られたテキストを渡す</param>
        public void GetText(string prompt, UnityAction<string> textGetter)
        {
            //ゲッターを登録
            int id = AddQueue(textGetter);

            //非同期処理（API送信・受信）を開始
            GetAPIResponse(prompt, id);
        }

        /// <summary>
        /// 非同期処理部分
        /// APIを送信・受信
        /// ゲッターを呼ぶ
        /// </summary>
        private async void GetAPIResponse(string prompt, int id)
        {
            //リクエストのJSONオブジェクト
            APIRequestData requestData = new()
            {
                Prompt = prompt,
                MaxTokens = 300 //レスポンスのテキストが途切れる場合、こちらを変更する
            };

            //シリアライズ
            string requestJson = JsonConvert.SerializeObject(requestData, Formatting.Indented);

            // POSTするデータ
            byte[] data = System.Text.Encoding.UTF8.GetBytes(requestJson);

            string jsonString = null;
            // POSTリクエストを送信
            using (UnityWebRequest request = UnityWebRequest.Post(API_END_POINT, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(data);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + API_KEY);
                await request.SendWebRequest();

                switch (request.result)
                {
                    case UnityWebRequest.Result.InProgress:
                        //リクエスト中
                        break;
                    case UnityWebRequest.Result.Success:
                        //リクエスト成功
                        jsonString = request.downloadHandler.text;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();

                }
            }

            // デシリアライズ
            APIResponseData jsonObject = JsonConvert.DeserializeObject<APIResponseData>(jsonString);

            //レスポンスからテキスト取得
            string outputText = jsonObject.Choices.FirstOrDefault().Text;
            string resultText = outputText.TrimStart('\n');

            //発動
            onTextGots[id].Invoke(resultText);

            //UnityEventが不要になったので、削除
            DeleteQueue(id);
        }

        /// <summary>
        /// 発動待ちを追加
        /// IDを返す
        /// </summary>
        private int AddQueue(UnityAction<string> unityAction)
        {
            //このUnityEventのID
            int currentID = nextID;

            //登録
            onTextGots[currentID] = new UnityEvent<string>();
            onTextGots[currentID].AddListener(unityAction);

            //ID進める
            nextID++;

            return currentID;
        }

        /// <summary>
        /// 不要なUnityEventを削除
        /// </summary>
        private void DeleteQueue(int id)
        {
            //Dictionaryから削除
            onTextGots.Remove(id);
        }
    }
}
