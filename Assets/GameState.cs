using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*Yes this is bad*/
public class GameState : MonoBehaviour {

    //GameObjects
    public GameObject PauseMenu;
    public GameObject Game;

    public GameObject Asleep;
    public GameObject ReallyAngry;
    public GameObject Angry;
    public GameObject Neutral;
    public GameObject Happy;
    public GameObject ReallyHappy;

    //time
    public Text timeText;
    private float time;
    private float countdownTime;

    //score
    public Text score;
    public Text highScore;
    public Text highScorePaused;
    private float scoreAdjusted;
    private float scoreBonus;
    private float scorePenalty;

    //music

    public Slider musicSlider;
    private int musicValue;
    public Text musicValueText;

    public Slider volumeSlider;
    private float volumeValue;
    public Text volumeValueText;

    public AudioSource musicSource1;
    public AudioSource musicSource2;

    //public Toggle toggle;
    //public Toggle toggle1;

    //cloud
    public Text preferredVolumeText;
    public Text preferredBeatText;
    private int rangeVol;
    private int idealLowVol;
    private int idealHighVol;
    private int lowerBoundVol;
    private int upperBoundVol;
    private int beatChoice;
    private bool isCloudy;
    private float cloudTime;


    void Start () {
        Time.timeScale = 1;
        cloudTime = 3;
        countdownTime = 60;
        musicSlider.value = 1;
        isCloudy = false;

        time = 0;
        scoreAdjusted = 0;
        scoreBonus = 0;
        scorePenalty = 0;


        PauseMenu.SetActive(false);
        Game.SetActive(true);
        musicSource1.Stop();
        musicSource2.Stop();
        musicSource1.Play();
        musicSource2.Play();
        

        highScore.text = PlayerPrefs.GetFloat("HighScore", 0).ToString();
        highScorePaused.text = highScore.text;
    }
    

    void Update()
    {
        MusicControl();

        CloudSetup();

        ScoreSetup();

        time += Time.deltaTime;        
        scoreAdjusted = time*10 + scoreBonus + scorePenalty;
        score.text = scoreAdjusted.ToString("f0");

        countdownTime -= Time.deltaTime;
        timeText.text = countdownTime.ToString("f0");

        //gameover
        if (countdownTime <= 0)
        {
            FinalScore();
            Pause();
        }                
    }


    private void MusicControl()
    {
        musicValue = (int)musicSlider.value;
        musicValueText.text = musicValue.ToString();

        if (musicValue == 1)
        {
            musicSource1.volume = volumeSlider.value;
            musicSource2.volume = 0;
        }
        else if (musicValue == 2)
        {
            musicSource1.volume = volumeSlider.value;
            musicSource2.volume = volumeSlider.value;
        }
        else if (musicValue == 3)
        {
            musicSource1.volume = 0;
            musicSource2.volume = volumeSlider.value;
        }

        volumeValue = volumeSlider.value;
        volumeValueText.text = volumeValue.ToString("f0");
    }

    private void CloudSetup()
    {
        cloudTime -= Time.deltaTime;
        Debug.Log("cloud time " + cloudTime);

        if (cloudTime <= 0)
        {
            idealLowVol = Random.Range(15, 90);
            idealHighVol = idealLowVol + 2;
            lowerBoundVol = idealLowVol - 3;
            upperBoundVol = idealHighVol + 3;
            cloudTime = Random.Range(7, 10);
            beatChoice = Random.Range(1, 4);

            preferredVolumeText.text = "Volume " + idealLowVol.ToString();
            preferredBeatText.text = "Beat # " + beatChoice.ToString();
            isCloudy = true;
        }
        if (cloudTime <= 2 && cloudTime > 0)
        {
            isCloudy = false;
            preferredVolumeText.text = "Volume ";
            preferredBeatText.text = "Beat # ";
            CloudCycle(3);
        }
    }

    public void ScoreSetup()
    {
        if (idealLowVol <= volumeValue && volumeValue <= idealHighVol && beatChoice == musicSlider.value && isCloudy)
        {
            scoreBonus += 1f;
            CloudCycle(5);
            //happy cloud
            Debug.Log("full bonus");
        }
        else if (lowerBoundVol <= volumeValue && volumeValue <= upperBoundVol && beatChoice == musicSlider.value && isCloudy)
        {
            scoreBonus += .5f;
            CloudCycle(4);
            //neutral cloud
            Debug.Log("partial bonus");
        }
        else
        {
            Debug.Log("no bonus");
        }
        if ((lowerBoundVol > volumeValue || volumeValue > upperBoundVol) && beatChoice != musicSlider.value && isCloudy)
        {
            scorePenalty -= 1f;
            CloudCycle(1);
            //very angry cloud
            Debug.Log("big penalty");
        }
        else if ((lowerBoundVol > volumeValue || volumeValue > upperBoundVol || beatChoice != musicSlider.value) && isCloudy)
        {
            scorePenalty -= .5f;
            CloudCycle(2);
            //angry cloud
            Debug.Log("small penalty");
        }
        else
        {
            Debug.Log("no penalty");
        }

        Debug.Log("vol val" + volumeValue);
        Debug.Log("ideal low" + idealLowVol);
        Debug.Log("ideal high" + idealHighVol);
        Debug.Log("beat choice" + beatChoice);
        Debug.Log("music slider" + musicSlider.value);
        Debug.Log("iscloudy" + isCloudy);
    }
    
    public void FinalScore()
    {
        if(scoreAdjusted > PlayerPrefs.GetFloat("HighScore"))
        {
            PlayerPrefs.SetFloat("HighScore", scoreAdjusted);
        }
    }


    public void Pause()
    {
        isCloudy = false;
        musicSource1.Pause();
        musicSource2.Pause();
        Time.timeScale = 0;
        PauseMenu.SetActive(true);
        Game.SetActive(false);
    }

    public void Resume()
    {
        PauseMenu.SetActive(false);
        Game.SetActive(true);
        Time.timeScale = 1;
        isCloudy = true;
        musicSource1.Play();
        musicSource2.Play();
    }

    public void CloudCycle(int num)
    {
        if(num == 5)
        {
            ReallyHappy.SetActive(true);
            Happy.SetActive(true);
            Neutral.SetActive(true);
            Angry.SetActive(true);
            ReallyAngry.SetActive(true);
            Asleep.SetActive(true);
        }
        else if(num == 4)
        {
            ReallyHappy.SetActive(false);
            Happy.SetActive(true);
            Neutral.SetActive(true);
            Angry.SetActive(true);
            ReallyAngry.SetActive(true);
            Asleep.SetActive(true);
        }
        else if (num == 3)
        {
            ReallyHappy.SetActive(false);
            Happy.SetActive(false);
            Neutral.SetActive(true);
            Angry.SetActive(true);
            ReallyAngry.SetActive(true);
            Asleep.SetActive(true);
        }
        else if (num == 2)
        {
            ReallyHappy.SetActive(false);
            Happy.SetActive(false);
            Neutral.SetActive(false);
            Angry.SetActive(true);
            ReallyAngry.SetActive(true);
            Asleep.SetActive(true);
        }
        else if (num == 1)
        {
            ReallyHappy.SetActive(false);
            Happy.SetActive(false);
            Neutral.SetActive(false);
            Angry.SetActive(false);
            ReallyAngry.SetActive(true);
            Asleep.SetActive(true);
        }
        else if (num == 0)
        {
            ReallyHappy.SetActive(false);
            Happy.SetActive(false);
            Neutral.SetActive(false);
            Angry.SetActive(false);
            ReallyAngry.SetActive(false);
            Asleep.SetActive(true);
        }

    }


    public void ResetScore()
    {
        PlayerPrefs.DeleteKey("HighScore");
        highScore.text = "0";
    }
}
