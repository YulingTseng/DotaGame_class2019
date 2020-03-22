using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    bool ControlSound;      //判斷聲音開關

    [Header("聲音按鈕")]
    public Button SoundButton;

    [Header("開聲音圖片")]
    public Sprite OpenSoundTexure;

    [Header("關聲音圖片")]
    public Sprite CloseSoundTexure;

    /*string OpenSoundTexurePath;
    string CloseSoundTexurePath;*/

    // Start is called before the first frame update
    void Start()
    {
        /*OpenSoundTexurePath = Application.StreamingAssetsPath + "/sound_open.png";
        CloseSoundTexurePath = Application.StreamingAssetsPath + "/sound_close.png";*/
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartButton()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void Control()
    {
        ControlSound = !ControlSound;       //!反義
        //AudioListener.pause=true 整體遊戲無聲
        //AudioListener.pause=false 整體遊戲有聲
        AudioListener.pause = ControlSound;

        /*WWW wwwOpenSound = new WWW(OpenSoundTexurePath);
        WWW wwwCloseSound = new WWW(CloseSoundTexurePath);*/

        if (ControlSound)
        {
            SoundButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("sound_open");
            //SoundButton.GetComponent<Image>().sprite = Sprite.Creat(wwwOpenSound.texture, new Rect(0, 0, wwwOpenSound.texture.width, wwwOpenSound.texture.height), new Vector2(0, 0));
        }
        else
        {
            SoundButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("sound_close");
            //SoundButton.GetComponent<Image>().sprite = Sprite.Creat(wwwCloseSound.texture, new Rect(0, 0, wwwCloseSound.texture.width, wwwCloseSound.texture.height), new Vector2(0, 0));
        }
    }
}
