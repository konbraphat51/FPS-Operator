using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPT
{
    public class GPTManagerTester : MonoBehaviour
    {
        //‚È‚ñ‚©Œ¾‚í‚¹‚é
        private void Start()
        {
            GPTManager.Instance.GetText("‚È‚ñ‚©Œ¾‚Á‚Ä‚­‚¾‚³‚¢", Log);
        }

        //Log‚Éo—Í
        private void Log(string result)
        {
            Debug.Log(result);
        }
    }
}
