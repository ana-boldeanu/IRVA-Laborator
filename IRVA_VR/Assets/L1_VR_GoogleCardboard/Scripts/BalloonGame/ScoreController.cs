using L1_VR_GoogleCardboard.Scripts.Helpers;
using TMPro;
using UnityEngine;

namespace L1_VR_GoogleCardboard.Scripts.BalloonGame
{
    /// <summary>
    /// Used to keep track of the game score & update a world space UI canvas.
    /// </summary>
    public class ScoreController : Singleton<ScoreController>
    {
       [SerializeField] private TextMeshProUGUI scoreText;
       int score = 0;
       
       public void IncrementScore()
       {
           // TODO 6.2 : Keep track of the score & set the UI text.
           score++;
           scoreText.text = score.ToString();
       }

       public void DecrementScore()
       {
            // TODO 6.3 : Keep track of the score & set the UI text.
            if (score <= 0)
                return;

            score--;
            scoreText.text = score.ToString();
       }
    }
}