using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiInventoryTextBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshTop1 = null;
    [SerializeField] private TextMeshProUGUI textMeshTop2 = null;
    [SerializeField] private TextMeshProUGUI textMeshTop3 = null;
    [SerializeField] private TextMeshProUGUI textMeshBottom1 = null;
    [SerializeField] private TextMeshProUGUI textMeshBottom2 = null;
    [SerializeField] private TextMeshProUGUI textMeshBottom3 = null;

    // set text values

    public void SetTextboxText(string textTop1, string textTop2, string textTop3, string textTBottom1, string textTBottom2, string textTBottom3)
    {
        textMeshTop1.text = textTop1;
        textMeshTop2.text = textTop2;
        textMeshTop3.text = textTop3;
        textMeshBottom1.text = textTBottom1;
        textMeshBottom2.text = textTBottom2;
        textMeshBottom3.text = textTBottom3;
    }
}
