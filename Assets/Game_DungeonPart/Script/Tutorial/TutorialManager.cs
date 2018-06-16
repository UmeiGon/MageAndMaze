using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutorialSequence
{
    Default = -1,
    Ryukku,
    SkillLearn,
    FlameShot,
    Learn,
    SkillSet
}

public class TutorialManager : MonoBehaviour
{

    GameObject parent;
    DungeonPartManager dMn;
    Player player;
    EventCanvasManager eventSceneManager;
    [SerializeField] GameObject cameraParent;
    [SerializeField] GameObject magicSelectWindow;
    CameraManager cameraMn;
    EventTriggerCameraRotater eventTriggerCameraRotater;
    UISwitch uiSwitch;
    PlayerSkillTree playerSkillTree;
    [SerializeField] GameObject itemDescriotionPanel;
    PlayerItem playerItem;

    private bool isTutorialON = false;
    public bool IsTutorialON
    {
        get
        {
            if (!dMn) Init();
            //return isTutorialON;
            return (dMn.floor == 1 && SaveData.GetInt("IsTutorialON", 1) == 1);
        }
    }

    [SerializeField] GameObject arrow;

    [System.Serializable]
    class ArrowTransformData
    {
        public RectTransform targetRect;
        public float angle;
    }
    [SerializeField]
    ArrowTransformData[] arrowTransformData;

    [SerializeField] GameObject narrationWindow;
    [SerializeField] GameObject playerAppearanceEffect;

    public int TutorialNumber { get; private set; }

    public void Init()
    {
        parent = GameObject.Find("GameObjectParent");
        dMn = parent.GetComponentInChildren<DungeonPartManager>();
        isTutorialON = (dMn.floor == 1 && SaveData.GetInt("IsTutorialON", 1) == 1);
        arrow.SetActive(false);
    }

    // Use this for initialization
    public void StartBehaviour()
    {

        if (!dMn) Init();

        // チュートリアルをする時以外、処理の必要なし
        if (dMn.floor == 1 && SaveData.GetInt("IsTutorialON", 1) == 1)
        {
            itemDescriotionPanel = ItemButtonManager.GetInstance().setumeiPanel;

            eventSceneManager = parent.GetComponentInChildren<EventCanvasManager>();
            eventTriggerCameraRotater = parent.GetComponentInChildren<EventTriggerCameraRotater>();
            uiSwitch = parent.GetComponentInChildren<UISwitch>();
            playerSkillTree = parent.GetComponentInChildren<PlayerSkillTree>();
            playerItem = parent.GetComponentInChildren<PlayerItem>();

            eventTriggerCameraRotater.RotateMoveButtonsAndMiniMap(180);
            TutorialNumber = 0;
            StartCoroutine(PlayerAppearanceCoroutine());
            StartCoroutine(TutorialCoroutine());
        }
        else
        {
            Debug.Log("チュートリアルに関してエラーが起きている。");
            TutorialNumber = 100;
        }
    }

    // プレイヤー登場エフェクト
    IEnumerator PlayerAppearanceCoroutine()
    {
        playerAppearanceEffect.SetActive(true);
        var se = playerAppearanceEffect.GetComponent<AudioSource>();
        // SEは鳴らさない
        se.enabled = false;
        yield return new WaitForSeconds(4);

        se.enabled = true;
        playerAppearanceEffect.SetActive(false);
    }

    IEnumerator TutorialCoroutine()
    {
        player = parent.GetComponentInChildren<Player>();
        TurnManager turnMn = parent.GetComponentInChildren<TurnManager>();

        arrow.SetActive(false);
        eventSceneManager.EventStart("tutorial1");
        // セリフ
        while (uiSwitch.UIType != DungUIType.BATTLE)
        {
            // バトルUIに戻ってくるまで待つ
            yield return null;
        }
        arrow.SetActive(true);

        // 移動
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (player.action != ActionType.MOVE)
        {
            yield return null;
        }

        // キャラの方向転換
        SetNextArrow();
        Vector3 charaDir = player.charaDir;
        yield return new WaitForSeconds(0.2f);
        while (player.charaDir == charaDir)
        {
            yield return null;
        }


        // カメラ回転
        SetNextArrow();
        float eulerY = cameraParent.transform.eulerAngles.y;
        yield return new WaitForSeconds(0.2f);
        while (Mathf.Abs(cameraParent.transform.eulerAngles.y - eulerY) < 80)
        {
            yield return null;
        }

        // プレイヤーが特定位置に到達するまで移動許可
        TutorialNumber++;
        arrow.SetActive(false);
        while (player.pos.x < 10)
        {
            yield return null;
        }

        eventSceneManager.EventStart("tutorial2");
        // セリフ
        while (uiSwitch.UIType != DungUIType.BATTLE)
        {
            // バトルUIに戻ってくるまで待つ
            yield return null;
        }
        arrow.SetActive(true);


        // 魔法選択ボタン
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (!magicSelectWindow.activeSelf)
        {
            yield return null;
        }

        // マジックショット
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (player.skillNum != 1)
        {
            // 魔法選択ウィンドウ閉じさせない
            if (!magicSelectWindow.activeSelf)
            {
                magicSelectWindow.SetActive(true);
            }
            yield return null;
        }

        int exp = player.Exp;
        while (player.Exp == exp)
        {
            yield return null;
        }

        arrow.SetActive(false);
        narrationWindow.SetActive(false);

        eventSceneManager.EventStart("tutorial3");
        // セリフ
        while (uiSwitch.UIType != DungUIType.BATTLE)
        {
            // バトルUIに戻ってくるまで待つ
            yield return null;
        }
        arrow.SetActive(true);

        // バッグボタン
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (uiSwitch.UIType != DungUIType.INVENTRY)
        {
            yield return null;
        }

        // 「スキル習得」ボタン
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (uiSwitch.UIType != DungUIType.SKILLTREE)
        {
            yield return null;
        }

        SkillTreeButtonManager skillTreeButtonManager = SkillTreeButtonManager.GetInstance();

        // 「フレイムショット」を選択
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (skillTreeButtonManager.selectSkill != 101)
        {
            yield return null;
        }

        // 「習得する」ボタン
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (!playerSkillTree.Skills[101].Syutoku)
        {
            yield return null;
        }

        // スキルセットの2番
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        bool isSetSkill = false;
        while (!isSetSkill)
        {
            foreach (int skill in playerSkillTree.SetSkills)
            {
                if (skill == 101)
                {
                    isSetSkill = true;
                }
            }
            yield return null;
        }

        arrow.SetActive(false);
        eventSceneManager.EventStart("tutorial4");
        // セリフ
        while (uiSwitch.UIType != DungUIType.SKILLTREE)
        {
            // スキルツリーに戻ってくるまで待つ
            yield return null;
        }
        arrow.SetActive(true);

        // 「修練＆精製」ボタン
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (uiSwitch.UIType != DungUIType.PRACTICE_AND_ITEMCRAFT)
        {
            yield return null;
        }

        // 「攻撃力アップのオーブ」ボタン
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (!itemDescriotionPanel.activeSelf)
        {
            yield return null;
        }

        // 「精製」ボタン
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (playerItem.items[100].kosuu == 0)
        {
            yield return null;
        }

        // これ以降TutoriaNumberは動かない
        arrow.SetActive(false);
        eventSceneManager.EventStart("tutorial5");
        // セリフ
        while (uiSwitch.UIType != DungUIType.PRACTICE_AND_ITEMCRAFT)
        {
            // 精製UIに戻ってくるまで待つ
            yield return null;
        }

        // 回復パネルの説明位置
        while (player.pos.x < 17 || turnMn.PlayerActionSelected)
        {
            yield return null;
        }
        //Debug.Log("healPanel");
        TutorialNumber++;
        eventSceneManager.EventStart("tutorial_heal");

        // 爆弾の説明位置
        while (player.pos.z > 17 || turnMn.PlayerActionSelected)
        {
            yield return null;
        }
        //Debug.Log("bomb");
        eventSceneManager.EventStart("tutorial_bomb");

        // 岩・氷ブロックの説明位置
        while (player.pos.x > 15 || turnMn.PlayerActionSelected)
        {
            yield return null;
        }
        Debug.Log("rockAndIceBlock");
        //eventSceneManager.EventStart("rockAndIceBlock");

        // 水たまりの説明位置
        while (player.pos.x > 9 || turnMn.PlayerActionSelected)
        {
            yield return null;
        }
        //Debug.Log("water");
        eventSceneManager.EventStart("tutorial_water");

        // 階段の説明位置
        while (player.pos.z > 7 || turnMn.PlayerActionSelected)
        {
            yield return null;
        }
        //Debug.Log("stairs");
        eventSceneManager.EventStart("tutorial_stair");


        yield return null;
    }

    IEnumerator WaitUntilTap()
    {
        yield return new WaitForSeconds(0.2f);
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }
    }

    IEnumerator WaitUntilFingerUp()
    {
        yield return new WaitForSeconds(0.2f);
        while (Input.touchCount == 0 && !Input.GetMouseButton(0))
        {
            yield return null;
        }
        while (Input.touchCount != 0 && !Input.GetMouseButtonUp(0))
        {
            yield return null;
        }
    }

    void SetNextArrow()
    {
        Vector3 arrowpos = arrowTransformData[TutorialNumber].targetRect.position;
        arrow.transform.position = arrowpos;
        var dis = Vector3.Distance(Vector3.zero, new Vector3(arrowTransformData[TutorialNumber].targetRect.sizeDelta.x, arrowTransformData[TutorialNumber].targetRect.sizeDelta.y));
        arrow.transform.eulerAngles =new Vector3(0,0,Mathf.Rad2Deg*arrowTransformData[TutorialNumber].angle * Mathf.PI * 2.0f);
        TutorialNumber++;
        Debug.Log("TutorialNumber = " + TutorialNumber);
    }

}
