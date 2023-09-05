using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] TextMeshPro scoreText;

    private int currentScore;

    private void OnTriggerEnter(Collider other)
    {
        currentScore++;
        scoreText.text = currentScore.ToString();
    }
}
