using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPT
{
    public class GPTManagerTester : MonoBehaviour
    {
        //なんか言わせる
        private void Start()
        {
            GPTManager.Instance.GetText("なんか言ってください", Log);
        }

        //Logに出力
        private void Log(string result)
        {
            Debug.Log(result);
        }
    }
}
