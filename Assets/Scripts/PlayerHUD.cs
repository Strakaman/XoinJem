using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHUD : MonoBehaviour {

    //[SerializeField]
    //int whichPlayerYouFor = 0;

    public Text nameText;
    public Text scoreText;
    public Image background;

    bool gameOver = false;

    public void UpdateBGColor(Color c)
    {
        background.color = c;
    }
    public void UpdateYourScore(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void UpdateYourName(string name)
    {
        nameText.text = name;
    }

    public IEnumerator VictoryFlicker()
    {
        Color prevColor = background.color;
        gameOver = true;
        while (gameOver)
        {
            yield return new WaitForSeconds(.75f);
            background.color = Color.white;
            yield return new WaitForSeconds(.25f);
            background.color = prevColor;

        }

    }

    public void StopVictoryFlicker()
    {
        gameOver = false;
    }
}
