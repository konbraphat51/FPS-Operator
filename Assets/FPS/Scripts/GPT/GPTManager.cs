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
        //�e�L�X�g�𓾂���AInvoke
        //�C���f�b�N�X��ID
        Dictionary<int, UnityEvent<string>> onTextGots = new Dictionary<int, UnityEvent<string>>();

        //���ɓo�^����ID
        private int nextID = 0;

        //�A�N�Z�X��
        const string API_END_POINT = "https://api.openai.com/v1/completions";
        const string API_KEY = "ENTER KEY HERE";

        /// <summary>
        /// �v�����v�g��ChatGPT�ɑ���A����ꂽ�e�L�X�g��textGetter�ɓn��
        /// </summary>
        /// <param name="prompt">ChatGPT�ɓ��͂���v�����v�g</param>
        /// <param name="textGetter">�擾�����ۂɌĂ΂�郁�\�b�h�B�����ɓ���ꂽ�e�L�X�g��n��</param>
        public void GetText(string prompt, UnityAction<string> textGetter)
        {
            //�Q�b�^�[��o�^
            int id = AddQueue(textGetter);

            //�񓯊������iAPI���M�E��M�j���J�n
            GetAPIResponse(prompt, id);
        }

        /// <summary>
        /// �񓯊���������
        /// API�𑗐M�E��M
        /// �Q�b�^�[���Ă�
        /// </summary>
        private async void GetAPIResponse(string prompt, int id)
        {
            //���N�G�X�g��JSON�I�u�W�F�N�g
            APIRequestData requestData = new()
            {
                Prompt = prompt,
                MaxTokens = 300 //���X�|���X�̃e�L�X�g���r�؂��ꍇ�A�������ύX����
            };

            //�V���A���C�Y
            string requestJson = JsonConvert.SerializeObject(requestData, Formatting.Indented);

            // POST����f�[�^
            byte[] data = System.Text.Encoding.UTF8.GetBytes(requestJson);

            string jsonString = null;
            // POST���N�G�X�g�𑗐M
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
                        //���N�G�X�g��
                        break;
                    case UnityWebRequest.Result.Success:
                        //���N�G�X�g����
                        jsonString = request.downloadHandler.text;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();

                }
            }

            // �f�V���A���C�Y
            APIResponseData jsonObject = JsonConvert.DeserializeObject<APIResponseData>(jsonString);

            //���X�|���X����e�L�X�g�擾
            string outputText = jsonObject.Choices.FirstOrDefault().Text;
            string resultText = outputText.TrimStart('\n');

            //����
            onTextGots[id].Invoke(resultText);

            //UnityEvent���s�v�ɂȂ����̂ŁA�폜
            DeleteQueue(id);
        }

        /// <summary>
        /// �����҂���ǉ�
        /// ID��Ԃ�
        /// </summary>
        private int AddQueue(UnityAction<string> unityAction)
        {
            //����UnityEvent��ID
            int currentID = nextID;

            //�o�^
            onTextGots[currentID] = new UnityEvent<string>();
            onTextGots[currentID].AddListener(unityAction);

            //ID�i�߂�
            nextID++;

            return currentID;
        }

        /// <summary>
        /// �s�v��UnityEvent���폜
        /// </summary>
        private void DeleteQueue(int id)
        {
            //Dictionary����폜
            onTextGots.Remove(id);
        }
    }
}
