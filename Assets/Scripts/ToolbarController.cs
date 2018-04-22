using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarController : MonoBehaviour {

    public Button ButtonSelect;
    public Button ButtonBulldoze;
    public Button ButtonRoad;
    public Button ButtonBuilding;
    public Button ButtonNature;
    public Button ButtonDefense;

    public GameObject PanelBuilding;
    public GameObject PanelNature;
    public GameObject PanelDefense;

    public Action<ConstructionMode> OnChangeMode;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ToggleButtons(int index)
    {
        ButtonSelect.interactable = true;
        ButtonBulldoze.interactable = true;
        ButtonRoad.interactable = true;
        ButtonBuilding.interactable = true;
        ButtonNature.interactable = true;
        ButtonDefense.interactable = true;

        PanelBuilding.SetActive(false);
        PanelNature.SetActive(false);
        PanelDefense.SetActive(false);

        switch (index)
        {
            case 0:
                ButtonSelect.interactable = false;
                break;
            case 1:
                ButtonBulldoze.interactable = false;
                break;
            case 2:
                ButtonRoad.interactable = false;
                break;
            case 3:
                ButtonBuilding.interactable = false;
                PanelBuilding.SetActive(true);

                break;
            case 4:
                ButtonNature.interactable = false;
                PanelNature.SetActive(true);

                break;
            case 5:
                ButtonDefense.interactable = false;
                PanelDefense.SetActive(true);

                break;
        }
    }

    public void OnSelectSelect()
    {
        ToggleButtons(0);

        if (OnChangeMode != null)
            OnChangeMode(ConstructionMode.None);
    }

    public void OnSelectBulldoze()
    {
        ToggleButtons(1);

        if (OnChangeMode != null)
            OnChangeMode(ConstructionMode.Bulldozer);
    }

    public void OnSelectRoad()
    {
        ToggleButtons(2);

        if (OnChangeMode != null)
            OnChangeMode(ConstructionMode.Road);
    }

    public void OnSelectBuilding()
    {
        ToggleButtons(3);

        if (OnChangeMode != null)
            OnChangeMode(ConstructionMode.Building);
    }

    public void OnSelectNature()
    {
        ToggleButtons(4);

        if (OnChangeMode != null)
            OnChangeMode(ConstructionMode.Nature);
    }

    public void OnSelectDefense()
    {
        ToggleButtons(5);

        if (OnChangeMode != null)
            OnChangeMode(ConstructionMode.Defense);
    }
}
