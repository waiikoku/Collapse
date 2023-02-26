using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class MissionUI : MonoBehaviour
{
    [SerializeField] private Button missionBtn;
    [SerializeField] private TextMeshProUGUI missionName;
    public MissionData missionData;

    public void AddListener(UnityAction callback)
    {
        missionBtn.onClick.AddListener(callback);
    }

    public void LoadData(MissionData data)
    {
        missionData = data;
        bool dataNull = data == null;
        missionName.text = dataNull? "Null": data.missionName;
        if(!dataNull)
        {
            if (data.locked == false)
            {
                missionBtn.interactable = true;
            }
            else
            {
                missionBtn.interactable = false;
            }
        }
    }
}
