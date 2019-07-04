using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CoorChange : MonoBehaviour
{
    public Color color1, color2;
    public MeshRenderer obj;
    public Color mCol1, mCol2;
    public Text text;

    public void ChangeColor(int index)
    {
        text.color = ((index == 1) ? color1 : color2);
        obj.material.color = (index == 1) ? mCol1 : mCol2;
    }
}
