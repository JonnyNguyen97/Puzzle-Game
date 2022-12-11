using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mananger : MonoBehaviour
{
    Board board;
    Transform boardtrans;
    bool isPause = false;

    void Awake()
    {
        board = FindObjectOfType<Board>();
        boardtrans = board.GetComponent<Transform>();
        PauseGame();
    }

    public void DisableChooseGems()
    {
        foreach(Transform child in boardtrans)
        {
            BoxCollider2D boxCollider2D = child.GetComponent<BoxCollider2D>();
            if (boxCollider2D != null)
            {
                //Debug.Log($"This child -> {child} with status {boxCollider2D.enabled}");
                boxCollider2D.enabled = false;
            }
        }
    }

    public void EnableChooseGems()
    {
        foreach(Transform child in boardtrans)
        {
            BoxCollider2D boxCollider2D = child.GetComponent<BoxCollider2D>();
            if (boxCollider2D != null)
            {
                boxCollider2D.enabled = true;
            }
        }
    }

    public void PauseGame()
    {
        DisableChooseGems();
        isPause = true;
        Time.timeScale = 0;
    }

    public void PlayContinue()
    {
        EnableChooseGems();
        isPause = false;
        Time.timeScale = 1;
    }

    public bool GetIsPause()
    {
        return isPause;
    }

    public void SceneMainGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainGame");
    }

    public void SceneMainMenu()
    {

    }
}
