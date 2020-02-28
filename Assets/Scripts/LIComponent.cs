using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LIComponent : MonoBehaviour
{
    public TextMeshProUGUI title;
    public FileObj fileObj;
    public SelectUIController controller;
    
    public void OnClick()
    {
        controller.SetChoseMusicUI(fileObj);
    }
}
