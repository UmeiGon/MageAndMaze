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

    [SerializeField] GameObject arrowParent;
    [SerializeField] RectTransform arrowRect;

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
        arrowParent.SetActive(false);
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

        arrowParent.SetActive(false);
        eventSceneManager.EventStart("tutorial1");
        // セリフ
        while (uiSwitch.UIType != DungUIType.BATTLE)
        {
            // バトルUIに戻ってくるまで待つ
            yield return null;
        }
        arrowParent.SetActive(true);

        // 移動
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (player.action != ActionType.MOVE)
        {
            HomingArrow();
            yield return null;
        }

        // キャラの方向転換
        SetNextArrow();
        Vector3 charaDir = player.charaDir;
        yield return new WaitForSeconds(0.2f);
        while (player.charaDir == charaDir)
        {
            HomingArrow();
            yield return null;
        }


        // カメラ回転
        SetNextArrow();
        float eulerY = cameraParent.transform.eulerAngles.y;
        yield return new WaitForSeconds(0.2f);
        while (Mathf.Abs(cameraParent.transform.eulerAngles.y - eulerY) < 80)
        {
            HomingArrow();
            yield return null;
        }

        // プレイヤーが特定位置に到達するまで移動許可
        TutorialNumber++;
        arrowParent.SetActive(false);
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
        arrowParent.SetActive(true);


        // 魔法選択ボタン
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (!magicSelectWindow.activeSelf)
        {
            HomingArrow();
            yield return null;
        }

        // マジックショット
        magicSelectWindow.SetActive(true);
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (player.skillNum != 1)
        {
            HomingArrow();
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

        arrowParent.SetActive(false);
        narrationWindow.SetActive(false);

        eventSceneManager.EventStart("tutorial3");
        // セリフ
        while (uiSwitch.UIType != DungUIType.BATTLE)
        {
            // バトルUIに戻ってくるまで待つ
            yield return null;
        }
        arrowParent.SetActive(true);

        // バッグボタン
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (uiSwitch.UIType != DungUIType.INVENTRY)
        {
            HomingArrow();
            yield return null;
        }

        // 「スキル習得」ボタン
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (uiSwitch.UIType != DungUIType.SKILLTREE)
        {
            HomingArrow();
            yield return null;
        }

        SkillTreeButtonManager skillTreeButtonManager = SkillTreeButtonManager.GetInstance();

        // 「フレイムショット」を選択
        //skilltreebuttonmanagerが他シーンにあり、インスペクターから参照できないのでここで情報を入力
        arrowTransformData[TutorialNumber] = new ArrowTransformData
        {
            angle = 0.25f,
            targetRect = skillTreeButtonManager.SkillButtons[101].GetComponent<RectTransform>(),
        };
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (skillTreeButtonManager.selectSkill != 101)
        {
            HomingArrow();
            yield return null;
        }

        // 「習得する」ボタン
        arrowTransformData[TutorialNumber] = new ArrowTransformData
        {
            angle = 0.25f,
            targetRect = skillTreeButtonManager.GetSyutokuButton.GetComponent<RectTransform>(),
        };
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (!playerSkillTree.Skills[101].Syutoku)
        {
            HomingArrow();
            yield return null;
        }

        // スキルセットの2番
        arrowTransformData[TutorialNumber] = new ArrowTransformData
        {
            angle = 0.75f,
            targetRect = skillTreeButtonManager.RegSkillButton[1].GetComponent<RectTransform>(),
        };
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        bool isSetSkill = false;
        while (!isSetSkill)
        {
            HomingArrow();
            foreach (int skill in playerSkillTree.SetSkills)
            {
                if (skill == 101)
                {
                    isSetSkill = true;
                }
            }
            yield return null;
        }

        arrowParent.SetActive(false);
        eventSceneManager.EventStart("tutorial4");
        // セリフ
        while (uiSwitch.UIType != DungUIType.SKILLTREE)
        {
            // スキルツリーに戻ってくるまで待つ
            yield return null;
        }
        arrowParent.SetActive(true);

        // 「修練＆精製」ボタン
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (uiSwitch.UIType != DungUIType.PRACTICE_AND_ITEMCRAFT)
        {
            HomingArrow();
            yield return null;
        }

        // 「攻撃力アップのオーブ」ボタン
        arrowTransformData[TutorialNumber] = new ArrowTransformData
        {
            angle = 0.75f,
            targetRect = ItemButtonManager.GetInstance().CreateItemsButton[0].GetComponent<RectTransform>(),
        };
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (!itemDescriotionPanel.activeSelf)
        {
            HomingArrow();
            yield return null;
        }

        // 「精製」ボタン
        arrowTransformData[TutorialNumber] = new ArrowTransformData
        {
            angle = 0.25f,
            targetRect = ItemButtonManager.GetInstance().SeiseiButton,
        };
        SetNextArrow();
        yield return new WaitForSeconds(0.2f);
        while (playerItem.items[100].kosuu == 0)
        {
            HomingArrow();
            yield return null;
        }

        // これ以降TutoriaNumberは動かない
        arrowParent.SetActive(false);
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
    //対象が動いても追従する
    void HomingArrow()
    {
        if (arrowTransformData[TutorialNumber - 1].targetRect != null)
        {
            arrowParent.transform.position = arrowTransformData[TutorialNumber - 1].targetRect.position;
        }
    }
    void SetNextArrow()
    {
        arrowParent.transform.position = arrowTransformData[TutorialNumber].targetRect.position;
        var tageRect = arrowTransformData[TutorialNumber].targetRect;
        var tageHalfX = tageRect.sizeDelta.x * 0.5f;
        var tageHalfY = tageRect.sizeDelta.y * 0.5f;
        var targetSize = new Vector3(tageHalfX, tageHalfY);
        var dis = Vector3.Distance(Vector3.zero, targetSize);
        arrowRect.localPosition = new Vector3(0, -dis, 0);
        arrowParent.transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * arrowTransformData[TutorialNumber].angle * Mathf.PI * 2.0f);
        Vector3.Distance(arrowRect.transform.position, arrowParent.transform.position);
        //fix
        var subx = arrowRect.transform.position.x - arrowParent.transform.position.x;
        var absSubX = Mathf.Abs(subx);
        if (absSubX > tageHalfX)
        {
            float fixX = 0;
            fixX = absSubX - tageHalfX;
            fixX *= (subx > 0) ? -1 : 1;
            arrowRect.transform.Translate( fixX,0, 0,Space.World);
        }
        var subY = arrowRect.transform.position.y - arrowParent.transform.position.y;
        var absSubY = Mathf.Abs(subY);
        if (absSubY > tageHalfY)
        {
            float fixY = 0;
            fixY = absSubY - tageHalfY;
            fixY *= (subY > 0) ? -1 : 1;
            arrowRect.transform.Translate(0, fixY, 0, Space.World);
        }

        TutorialNumber++;
        Debug.Log("TutorialNumber = " + TutorialNumber);
    }

}
