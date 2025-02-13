using System;
using UnityEngine;
using UnityEngine.UI;

namespace _PROJECT._Scripts
{
    public class Quit : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(QuitGame);
        }

        private void QuitGame()
        {
            Application.Quit();
        }
    }
}