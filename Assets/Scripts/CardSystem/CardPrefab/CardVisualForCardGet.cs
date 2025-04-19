using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class CardVisualForCardGet : MonoBehaviour {

    [Header("组件引用")]
    [SerializeField] private Image cardImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI ManaCostText;

    public void Initialize(CardDataBase data) {

        cardImage.sprite = data.cardIcon;
        nameText.text = data.displayName;
        descriptionText.text = data.description;
        ManaCostText.text = data.manaCost.ToString();

    }

}
