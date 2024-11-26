using System;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ManualUIController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI[] manualTxts;

    [SerializeField]
    private Button nextBt;

    [SerializeField]
    private Button prevBt;

    [SerializeField]
    private Button closeBt;

    private int index;

    private void Awake()
    {
        index = 0;
    }

    private void Start()
    {
        nextBt.onClick.AddListener(NextPage);
        prevBt.onClick.AddListener(PrevPage);
        closeBt.onClick.AddListener(CloseManual);

        for(int i = 0; i < manualTxts.Length; i++)
        {
            if (i == index)
                manualTxts[i].enabled = true;
            else
                manualTxts[i].enabled = false;
        }
    }

    private void CloseManual()
    {
        gameObject.SetActive(false);
    }

    private void NextIndex()
    {
        index++;
        if (index >= 2)
            index = 2;
    }

    private void PrevIndex()
    {
        index--;

        if (index <= 0)
            index = 0;
    }

    private void NextPage()
    {
        manualTxts[index].enabled = false;
        NextIndex();
        manualTxts[index].enabled = true;
    }

    private void PrevPage()
    {
        manualTxts[index].enabled = false;
        PrevIndex();
        manualTxts[index].enabled = true;
    }


}