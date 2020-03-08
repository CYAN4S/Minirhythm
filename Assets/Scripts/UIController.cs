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
    public GameManager gameM;
    public InputManager inputM;
    SheetManager sheetM;
    public Image grooveMeter;
    #endregion

    public List<GameObject> PressingEffects;

    private void Start() 
    {
        sheetM = gameM.sheetM;
        inputM = InputManager.instance;

        switch (sheetM.modeLine)
        {
            case 4: PressingEffects = PressingEffects4K; break;
            case 5: PressingEffects = PressingEffects5K; break;
            case 6: PressingEffects = PressingEffects6K; break;
            case 8: PressingEffects = PressingEffects8K; break;
        }
    }

    private void Update() 
    {
        for (int i = 0; i < inputM.KeyCodes.Length; i++)
        {
            if (Input.GetKey(inputM.KeyCodes[i]))
            {
                PressingEffects[i].SetActive(true);
            }
            else
            {
                PressingEffects[i].SetActive(false);
            }
        }
    }

    public void ComboEffect(int combo)
    {
        comboText.text = combo.ToString();
    }

    public void ComboResetEffect(int combo)
    {
        comboText.text = combo.ToString();
    }

    public void JudgeEffect(string judgeString, float timeGap)
    {
        judgementText.text = judgeString;
        detailText.text = (timeGap * 100).ToString("F0");
    }

    public void JudgeEffect(string judgeString)
    {
        judgementText.text = judgeString;
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
