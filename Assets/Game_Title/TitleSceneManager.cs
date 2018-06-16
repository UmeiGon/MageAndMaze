using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour {

    GameObject parent;
    SceneTransitionManager sceneTransitionManager;

    [SerializeField]
    GameObject menu;
    [SerializeField]
    Button continueButton;
    [SerializeField]
    Image[] fadeInImgs;
    [SerializeField]
    Text fadeOutText;
    float timeFade = 0;
    bool menuOn = false;

    [SerializeField] GameObject titleBack;
    [SerializeField] GameObject titleChara;
    [SerializeField] Image titleCharaWhite;
    [SerializeField] GameObject titleText;

    [SerializeField] float titleImageMoveDistance = 200;
    [SerializeField] float animationTime = 1.5f;
    

    // Use this for initialization
    void Start () {
        // 中断フラグONなら中断データからコンティニュー（再開）できる
        int isInterrupt = SaveData.GetInt("IsInterrupt", 0);
        continueButton.gameObject.SetActive((isInterrupt == 1) ? true : false);
        parent = GameObject.Find("GameObjectParent");
        sceneTransitionManager = parent.GetComponentInChildren<SceneTransitionManager>();
        sceneTransitionManager.fadeInImage.color = new Color(0, 0, 0, 0);
        StartCoroutine(TitleAnimation());
    }

    IEnumerator TitleAnimation()
    {
        StartCoroutine(sceneTransitionManager.FadeIn());
        // キャラとタイトルロゴが右からスライド
        for ( float t = animationTime; t >= 0; t -= Time.deltaTime )
        {
            titleChara.transform.localPosition = new Vector3(t * t * titleImageMoveDistance, 0, 0);
            titleText.transform.localPosition = new Vector3(t * t * titleImageMoveDistance * 3, 0, 0);
            yield return null;
        }
        titleChara.transform.localPosition = Vector3.zero;
        titleText.transform.localPosition = Vector3.zero;
        // キャラ部分が白く光る
        for ( float t = 0; t < 1; t += Time.deltaTime )
        {
            titleCharaWhite.color = new Color(1, 1, 1, 1 - t);
            yield return null;
        }
        titleCharaWhite.color = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update () {
        if (menuOn)
        {
            timeFade += Time.deltaTime;
            foreach(Image img in fadeInImgs)
            {
                Color color = img.color;
                img.color = new Color(color.r, color.g, color.b, timeFade);
            }
            Color colr = fadeOutText.color;
            fadeOutText.color = new Color(colr.r, colr.g, colr.b, ( 1 - timeFade ));
        }
	}
    public void ToMenu()
    {
        menuOn = true;
        menu.SetActive(true);
    }

    public void ToTitleScene()
    {
        SceneManager.LoadScene("Title");
    }
    
    // 洞窟１の最初から
    public void ToDungeon1Scene()
    {
        StartCoroutine(ToDungeon1SceneCoroutine(false));
    }

    public IEnumerator ToDungeon1SceneCoroutine(bool isContinue)
    {
        yield return StartCoroutine(sceneTransitionManager.FadeOut());

        if (!isContinue)
        {
            // 中断フラグOFF（最初から）
            DungeonPartManager.SaveDataReset();
            //SaveData.Save();
            SceneManager.LoadScene("Opening");
        }
        else
        {
            SceneManager.LoadScene("Dungeon1", LoadSceneMode.Single);
            SceneManager.LoadScene("CanvasScene",LoadSceneMode.Additive);
        }
    }

    //public void ToDungeon2Scene()
    //{
    //    SceneManager.LoadScene("Dungeon1");
    //}
    // 中断データから
    public void ToContinue()
    {
        StartCoroutine(ToDungeon1SceneCoroutine(true));
    }
    public void ToVillageScene()
    {
        SceneManager.LoadScene("Village1");
    }

    
}
