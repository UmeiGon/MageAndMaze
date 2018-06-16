using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPartManager : MonoBehaviour
{

    [SerializeField] GameObject parent;
    Player player;
    public static DungeonPartManager Instance;
    public MapManager mapManager;
    UIMiniMap miniMap;
    public DungeonInitializer dungeonInitializer;
    public EnemyManager enemyManager;
    public MoveButtonManager moveButtonManager;
    public int dungeonType = 0;
    public int floor = 1;
    UISwitch ui;
    SceneTransitionManager sceneTransitionManager;
    EventCanvasManager eventSceneManager;
    SESet seSet;
    TutorialManager tutorialMn;
    PlayerItem playerItem;
    TurnManager turnMn;

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 30;
        // デバッグ用
        //SaveDataReset();
    }
    // Use this for initialization
    void Start()
    {
        if (!parent) parent = GameObject.Find("GameObjectParent");
        player = parent.GetComponentInChildren<Player>();

        floor = SaveData.GetInt("Floor", 1);
        if (floor % 8 >= 5 && floor % 8 <= 7 && floor != 30)
        {
            dungeonType = 1;
        }
        mapManager = parent.GetComponentInChildren<MapManager>();
        miniMap = parent.GetComponentInChildren<UIMiniMap>();
        dungeonInitializer = parent.GetComponentInChildren<DungeonInitializer>();
        enemyManager = parent.GetComponentInChildren<EnemyManager>();
        moveButtonManager = parent.GetComponentInChildren<MoveButtonManager>();
        sceneTransitionManager = parent.GetComponentInChildren<SceneTransitionManager>();
        eventSceneManager = parent.GetComponentInChildren<EventCanvasManager>();
        seSet = parent.GetComponentInChildren<SESet>();
        tutorialMn = parent.GetComponentInChildren<TutorialManager>();
        playerItem = parent.GetComponentInChildren<PlayerItem>();
        turnMn = parent.GetComponentInChildren<TurnManager>();

        mapManager.d_initializer = dungeonInitializer;
        // ダンジョンの生成
        mapManager.DungeonGenerate();
        int _bgmNum = dungeonType;
        if (floor % 8 == 0 || floor == 30) _bgmNum = 2;
        parent.GetComponentInChildren<BGMSet>().SetBGM(_bgmNum);
        player.LoadPlayerInfo();
        player.Init();
        miniMap.MiniMapInit();

        ui = parent.GetComponentInChildren<UISwitch>();
        ui.SwitchUI((int)DungUIType.BATTLE);

        StartCoroutine(sceneTransitionManager.FadeIn());

        // フロア開始時イベント
        if (tutorialMn.IsTutorialON)
        {
            // チュートリアルの開始
            tutorialMn.StartBehaviour();
        }
        else if (0 == SaveData.GetInt("IsInterrupt", 0) && floor != 30)
        {
            Debug.Log("IsInterrupt = " + SaveData.GetInt("IsInterrupt", 0));
            //// 階の最初にイベント発生する際
            //int random = Random.Range(0, 100);
            //// 確率10％ずつ
            //if ( random < 50 )
            //{
            //    eventSceneManager.EventStart("Event_1");
            //}
            //else if ( random < 100 )
            //{
            //    eventSceneManager.EventStart("Event_3");
            //}
        }

        // アイテム個数の初期化
        if (tutorialMn.IsTutorialON)
        {
            playerItem.items[0].kosuu = 9;
        }
        else if (floor == 1 && 0 == SaveData.GetInt("IsInterrupt", 0))
        {
            // チュートリアルOFFの1階スタート時
            playerItem.items[0].kosuu = 3;
            playerItem.items[1].kosuu = 3;
            playerItem.items[2].kosuu = 3;
        }

        turnMn.DungeonSave();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void NextFloor()
    {
        StartCoroutine(NextFloorCoroutine());
    }

    IEnumerator NextFloorCoroutine()
    {
        seSet.PlaySE(SESet.Type.STAIRS_DOWN);
        yield return StartCoroutine(sceneTransitionManager.FadeOut());

        if (floor == 30)
        {
            SaveGameClear();
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameClear");
            yield break;
        }
        floor++;
        SaveData.SetInt("Floor", floor);
        // 中断フラグOFF
        turnMn.DungeonSave();
        SaveData.SetInt("IsInterrupt", 0);
        SaveData.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dungeon1", UnityEngine.SceneManagement.LoadSceneMode.Single);
        UnityEngine.SceneManagement.SceneManager.LoadScene("CanvasScene", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    public void SaveGameClear()
    {
        SaveDataReset();
    }

    public void DownFloor(int i)
    {
        floor = (i - 1);
        NextFloor();
    }

    public static void SaveDataReset()
    {
        Debug.Log("セーブデータリセット");

        // リセットで消してはならないデータを残してリセット
        int _isTutorialON = SaveData.GetInt("IsTutorialON", 1);
        SaveData.Clear();
        SaveData.SetInt("IsTutorialON", _isTutorialON);

        SaveData.Save();
    }

    public void SaveData_Reset()
    {
        SaveDataReset();
    }

    bool isDoubleSpeed = false;

    public void DebugChangeSpeed()
    {
        isDoubleSpeed = !isDoubleSpeed;
        Time.timeScale = (isDoubleSpeed) ? 2 : 1;
    }
}
