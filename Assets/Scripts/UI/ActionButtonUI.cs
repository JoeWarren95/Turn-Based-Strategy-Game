using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionButtonUI : MonoBehaviour
{
    /// <summary>
    /// This script automatically sets up our buttons with their given name/function
    /// </summary>

    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button button;
    [SerializeField] private GameObject selectedGameObject;

    //reference to our BaseAction script
    private BaseAction baseAction;

    public void SetBaseAction(BaseAction baseAction)
    {
        this.baseAction = baseAction;

        //the '.ToUpper()' makes it so everything is upper case
        //this line will change the button name based on the action it is supposed to perform
        textMeshPro.text = baseAction.GetActionName().ToUpper();

        //this is called an anonymous function
        button.onClick.AddListener(() => {
            //this is an alternative way of writing a separate function and calling it here
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
        });
    }

    public void UpdateSelectedVisual()
    {
        //this script allows the buttons to update themselves rather than having us update them manually
        //here we grab whatever action is selected - 
        BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();

        // - and we either set the gameobject active or not depending on if it's selected
        selectedGameObject.SetActive(selectedBaseAction == baseAction);
    }

}
