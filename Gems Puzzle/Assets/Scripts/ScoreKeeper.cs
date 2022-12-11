using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreKeeper : MonoBehaviour
{
    int score = 0;
    [SerializeField] int addingScoreNormal = 5;
    [SerializeField] TextMeshProUGUI txtScores;


    // Start is called before the first frame update
    void Start()
    {
        RunUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RunUI()
    {
        txtScores.text = score.ToString("000000");
    }

    public int GetAddingNormal()
    {
        return addingScoreNormal;
    }

    public void IncrScore(int value)
    {
        score += value;
        txtScores.text = score.ToString("000000");
    }

    public int GetTotalScore()
    {
        return score;
    }
}
