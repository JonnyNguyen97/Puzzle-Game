using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    [SerializeField] int totalTimeRemain = 30;
    [SerializeField] TextMeshProUGUI txtTimerRemain;
    [SerializeField] TextMeshProUGUI txtpTotalScore;
    [SerializeField] GameObject panelOverGame;

    Mananger mananger;
    ScoreKeeper scoreKeeper;

    void Awake()
    {
        mananger = FindObjectOfType<Mananger>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();

    }

    // Start is called before the first frame update
    void Start()
    {
        TimerCountDown();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TimerCountDown()
    {
        StartCoroutine(CountdownCoroutine());
    }

    IEnumerator CountdownCoroutine()
    {
        while(totalTimeRemain > 0)
        {
            if (totalTimeRemain <= 5)
            {
                txtTimerRemain.color = new Color32(195, 39, 39, 255);
            }
            txtTimerRemain.text = totalTimeRemain.ToString();
            if (!mananger.GetIsPause())
                totalTimeRemain -= 1;

            yield return new WaitForSeconds(1f);
        }
        if (totalTimeRemain == 0)
        {
            //SceneManager.LoadScene("MainMenu");
            txtpTotalScore.text = scoreKeeper.GetTotalScore().ToString("000000");
            panelOverGame.SetActive(true);
            mananger.PauseGame();

        }
    }


}
