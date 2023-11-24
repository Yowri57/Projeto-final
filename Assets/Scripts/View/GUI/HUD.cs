using UnityEngine;
using System;
using System.Collections;
using System.IO;

namespace com.javierquevedo
{
    public class HUD : MonoBehaviour
    {

        public Game game;
        public static int pontos;
        public static float _timeOffset;
        public GUIStyle textStyle;

        void Start()
        {
            game = new Game();
            _timeOffset = Time.timeSinceLevelLoad;


            textStyle = new GUIStyle();
            textStyle.normal.textColor = Color.white;
            //textStyle.normal.textColor = new Color(1.0f, 0.0f, 1.0f); 
            textStyle.fontSize = 24;
            //textStyle.fontStyle = FontStyle.Bold; 
            //textStyle.alignment = TextAnchor.MiddleCenter; 
            textStyle.wordWrap = true;

            Font customFont = Resources.Load<Font>("PixelOperatorSC-Bold");
            textStyle.font = customFont;
        }

        void Update()
        {
        }

        void OnGUI()
        {
            pontos = game.score;
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            float rightPosition = 0.82f * screenWidth;
            float leftPosition = 0.02f * screenWidth;
            float yPosition = 0.2f * screenHeight;

            TimeSpan timeSpan = TimeSpan.FromSeconds(Time.timeSinceLevelLoad - _timeOffset);
            string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

            GUI.Label(new Rect(leftPosition, yPosition + 20, 200, 30), "Pontuação: " + pontos, textStyle);
            GUI.Label(new Rect(leftPosition, yPosition + 40, 200, 30), "Tempo de jogo: " + timeText, textStyle);
            GUI.Label(new Rect(rightPosition, yPosition + 60, 200, 30), "Alvo da vez", textStyle);
        }


        public static void gravartempo()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(Time.timeSinceLevelLoad - _timeOffset);
            string timeText2 = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            StreamWriter sw = new StreamWriter("Test.txt", true);
            //Write a line of text
            sw.WriteLine(timeText2);
            sw.WriteLine(pontos);
            //Write a second line of text
            sw.Close();

        }
    }
}