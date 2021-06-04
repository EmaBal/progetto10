using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public Slider hpSlider;
    public Image marker;

    public void SetHUD(Unit unit)
    {
        nameText.text = unit.unitName;
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;
        marker.enabled = false;
    }

    public void setHP(int hp)
    {
        hpSlider.value = hp;
    }

    public void setMarkerVisibility(bool vis)
    {
        if (vis)
            marker.enabled = true;
        else
            marker.enabled = false;
        }
}
