using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public KeyCode[] KeyCodes4K, KeyCodes5K, KeyCodes6K, KeyCodes8K;
    public GameManager gameM;
    SheetManager sheetM;

    [Header("Game Option")]
    public KeyCode[] ScrollSpeedKeyCodes;

    public KeyCode[] KeyCodes;

    private void Start() 
    {
        sheetM = gameM.sheetM;

        switch (sheetM.modeLine)
        {
            case 4: KeyCodes = KeyCodes4K; break;
            case 5: KeyCodes = KeyCodes5K; break;
            case 6: KeyCodes = KeyCodes6K; break;
            case 8: KeyCodes = KeyCodes8K; break;
        }
    }
}
