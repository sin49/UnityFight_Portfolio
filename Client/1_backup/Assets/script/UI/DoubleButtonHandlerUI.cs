using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DoubleButtonHandlerUI : MonoBehaviour
{
    public int count;
    public int MaxCount;
    public Button PlusButton;
    public Button MinusButton;
    public int CountWeightByButton = 1;
    public TextMeshProUGUI CountText;

    protected virtual void PlusButtonInput()
    {

        if (MaxCount ==( count+= CountWeightByButton))
        {
            PlusButton.interactable = false;
        }
        if (count > 0 && MinusButton.interactable == false)
        {
            MinusButton.interactable = true;
        }
      
    }
    protected virtual void MinusButtonInput()
    {
        if (MaxCount == count)
        {
            PlusButton.interactable = true;
        }
        if ((count -= CountWeightByButton) == 0)
        {
            MinusButton.interactable = false;
        }
       

    }
    protected virtual void Awake()
    {
        PlusButton.onClick.AddListener(PlusButtonInput);
        MinusButton.onClick.AddListener(MinusButtonInput);

    }
    protected virtual void OnEnable()
    {
        MinusButton.interactable = false;
    }
    protected virtual void Update()
    {
        CountText.text = count.ToString();
    }
}
