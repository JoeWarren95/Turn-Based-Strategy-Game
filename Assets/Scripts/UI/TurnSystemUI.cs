using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnSystemUI : MonoBehaviour
{
    ///This script handles the turn system, meaning it controls the end turn button,
    ///and keeping track of what turn we are on

    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private GameObject enemyTurnVisualGameObject;

    private void Start()
    {
        endTurnButton.onClick.AddListener(() =>
        {
            //this calls our NextTurn function from our TurnSystem script
            TurnSystem.Instance.NextTurn();
        });

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        UpdateTurnText();

        UpdateEnemyTurnVisual();

        UpdateEndTurnButtonVisibility();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnText();

        UpdateEnemyTurnVisual();

        UpdateEndTurnButtonVisibility();
    }

    private void UpdateTurnText()
    {
        //This function updates our UI for Turn # whenever a full turn has ended
        turnNumberText.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
    }

    private void UpdateEnemyTurnVisual()
    {
        //this function makes it so the player can't perform any actions when it is the enemy's turn
        enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }

    private void UpdateEndTurnButtonVisibility()
    {
        //this function activates/deactivates our End Turn button, this is so the player can only end
        //a turn when it is their turn and not the enemy's turn
        endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }
}
