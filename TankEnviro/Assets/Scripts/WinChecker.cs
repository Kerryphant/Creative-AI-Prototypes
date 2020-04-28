using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinChecker : MonoBehaviour
{

    private TankAgent[] tanks;
    public GameObject RLWinText;
    public GameObject BTWinText;
    public float resetDelay;

    public int maxDeathsBeforeWin = 5;
    public int RLDeathCount = 0;
    public int BTDeathCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (tanks == null)
        {
            tanks = FindObjectsOfType<TankAgent>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        RLDeathCount = 0;
        BTDeathCount = 0;

        foreach (TankAgent currentTank in tanks)
        {
            if (currentTank.teamID == 0)
			{
                RLDeathCount += currentTank.deathCount;
			}
            else if(currentTank.teamID == 1)
			{
                BTDeathCount += currentTank.deathCount;
            } 
        }

        //check incase both teams have got a high death count
        if (RLDeathCount > maxDeathsBeforeWin && BTDeathCount > maxDeathsBeforeWin)
		{
            //whichever has the highest death count wins, if a draw, behaviour tree default wins
            Win(BTDeathCount > RLDeathCount);
		}
       
        if(RLDeathCount > maxDeathsBeforeWin)
		{
            //BT wins
            Win(false);
		}
        else if(BTDeathCount > maxDeathsBeforeWin)
		{
            //RL wins
            Win(true);
		}

    }

    //Pass true if RL wins, False if BT wins
    void Win(bool RLWin)
	{
        if(RLWin)
		{
            RLWinText.SetActive(true);
		}
        else
		{
            BTWinText.SetActive(true);
        }

        Time.timeScale = 0.5f;

        Invoke("Reset", resetDelay);
	}

    void Reset()
	{
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
