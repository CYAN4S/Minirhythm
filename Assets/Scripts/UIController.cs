using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    #region UNITYEDITOR SETUPS
    public List<GameObject> PressingEffects4K, PressingEffects5K, PressingEffects6K, PressingEffects8K;
    public TextMeshProUGUI judgementText, comboText, scoreText, detailText, speedText;
    public InputManager inputManager;
    public Image grooveMeter;
    #endregion

    public List<GameObject> PressingEffects;

    public void SetPressingEffects(int modeLine)
    {
        switch (modeLine)
        {
            case 4: PressingEffects = PressingEffects4K; break;
            case 5: PressingEffects = PressingEffects5K; break;
            case 6: PressingEffects = PressingEffects6K; break;
            case 8: PressingEffects = PressingEffects8K; break;
        }
    }

    private void Update() 
    {
        for (int i = 0; i < 4; i++)
        {
            /// 4K ONLY
            if (Input.GetKey(inputManager.KeyCodes4K[i]))
            {
                PressingEffects4K[i].SetActive(true);
            }
            else
            {
                PressingEffects4K[i].SetActive(false);
            }
        }
    }

    public void ComboEffect(int combo)
    {
        comboText.text = combo.ToString();
    }

    public void JudgeEffect(string judgeString, float timeGap)
    {
        judgementText.text = judgeString;
        detailText.text = (timeGap * 100).ToString("F0");
    }

    public void ScoreEffect(double score)
    {
        scoreText.text = score.ToString("000000");
    }


    public void HealthEffect(float health)
    {
        grooveMeter.fillAmount = health;
    }


}
