using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPT
{
    public class GPTManagerTester : MonoBehaviour
    {
        //�Ȃ񂩌��킹��
        private void Start()
        {
            GPTManager.Instance.GetText("�Ȃ񂩌����Ă�������", Log);
        }

        //Log�ɏo��
        private void Log(string result)
        {
            Debug.Log(result);
        }
    }
}
