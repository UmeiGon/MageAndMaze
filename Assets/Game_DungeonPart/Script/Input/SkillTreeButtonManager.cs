using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeButtonManager : UMI.DSingleton<SkillTreeButtonManager>
{
    GameObject parent;
    private const int TREE_WID = 3, TREE_HEI = 8, NONE = -1;
    public const int RED = 0, YELLOW = 1, BLUE = 2, TREE_SUU = 3, SET_SUU = 6;
    public int selectSkill { get; private set; }
    //今選択している登録パネルの番号達
    //private bool[] selectedRegList = new bool[6];
    //UIのgameobject達
    [SerializeField]
    private GameObject SkillButtonPre;
    [SerializeField]
    private GameObject BranchPre;
    [SerializeField]
    private GameObject ScrollPanel;
    [SerializeField]
    private GameObject SyutokuButton;
    [SerializeField]
    private GameObject registPanel;
    [SerializeField]
    private GameObject selectImage;
    [SerializeField]
    private GameObject SkillTreeCanvas;

    private bool deleteMode = false;
    public GameObject SetumeiPanel;

    private GameObject[] SyadanPanel;

    private Dictionary<int, GameObject> skillButtons = new Dictionary<int, GameObject>();
    public GameObject[] SoulStoneImage = new GameObject[TREE_SUU];
    private GameObject[] regSkillButton = new GameObject[SET_SUU];

    private PlayerSkillTree pst;
    private bool stated = false;
    // Use this for initialization
    void Start()
    {
        parent = GameObject.Find("GameObjectParent");
        //プレイヤーからスキルツリーデータを持ってくる
        pst = parent.GetComponentInChildren<PlayerSkillTree>();
        //ツリー背景インスタンス化
        GameObject[] TreeBacks = new GameObject[TREE_SUU];
        SyadanPanel = new GameObject[TREE_SUU];

        for (int i = 0; i < TREE_SUU; i++)
        {
            TreeBacks[i] = ScrollPanel.transform.Find("" + i).gameObject;
            SyadanPanel[i] = TreeBacks[i].transform.Find("Syadan").gameObject;
        }
        //ツリーの配置データを作成
        int[][,] TreeHaiti = new int[TREE_SUU][,]
        {
            //redのツリー配置
        new int[TREE_HEI, TREE_WID]
        {
            {  0,  0,108},
            {  0,105,  0},
            {103,  0,107},
            {  0,109,  0},
            {102,  0,106},
            {  0,104,  0},
            {101,  0,  0},
            {  1,  0,  0}
        },
         //yellowのツリー配置
         new int[TREE_HEI, TREE_WID]
        {
            {  0,  0,208},
            {203,  0,  0},
            {  0,205,207},
            {202,  0,  0},
            {  0,204,  0},
            {  0,  0,206},
            {201,  0,  0},
            {  0,  0,  0}
        },
          //blueのツリー配置
          new int[TREE_HEI, TREE_WID]
        {
            {  0,308,  0},
            {303,  0,306},
            {  0,  0,  0},
            {302,  0,307},
            {  0,305,  0},
            {301,  0,  0},
            {304,  0,  0},
            {  0,  0,  0}
        }
        };
        //スクロールパネルにツリーを配置
        for (int i = 0; i < TREE_SUU; i++)
        {
            SkillTreeUICreate(TreeBacks[i], TreeHaiti[i]);
            TreeBacks[i].transform.localPosition = new Vector3(i * (TreeBacks[i].GetComponent<RectTransform>().sizeDelta.x + 60) + 30, 0);
        }
        //TreeBacks[RED].GetComponent<Image>().color = Color.red;
        //TreeBacks[YELLOW].GetComponent<Image>().color = Color.yellow;
        //TreeBacks[BLUE].GetComponent<Image>().color = Color.blue;
        for (int i = 0; i < SET_SUU; i++)
        {
            regSkillButton[i] = registPanel.transform.Find("" + i).gameObject;
        }
        Debug.Log(pst.Skills[1].Syutoku);
        Init();
        syutokuKousin();
        RegiButKousin();
        //セレクトimageを前に出す
        //selectImage.transform.SetAsLastSibling();
        //SetumeiPanel.transform.SetAsLastSibling();
        //setImage.transform.SetAsLastSibling();
        //transform.SetAsLastSibling();
        stated = true;
    }
    private void Init()
    {
        selectSkill = NONE;
        SetumeiPanel.SetActive(false);
        selectImage.SetActive(false);
        for (int i = 0; i < SET_SUU; i++)
        {
            regSkillButton[i].transform.Find("SelectImage").gameObject.SetActive(false);
        }
    }
    private void OnEnable()
    {
        if (stated)
        {
            Init();
            syutokuKousin();
            RegiButKousin();
        }
    }
    void SkillTreeUICreate(GameObject treeBack, int[,] haitidata)
    {
        float width = treeBack.GetComponent<RectTransform>().sizeDelta.x;
        float height = treeBack.GetComponent<RectTransform>().sizeDelta.y;
        float offset = SkillButtonPre.GetComponent<RectTransform>().sizeDelta.x / 2 + 10;
        float masukanW = (width - offset * 2) / (TREE_WID - 1);
        float masukanH = (height - offset * 2) / (TREE_HEI - 1);
        Dictionary<int, GameObject> Buttons = new Dictionary<int, GameObject>();
        for (int h = 0; h < TREE_HEI; h++)
        {
            for (int w = 0; w < TREE_WID; w++)
            {
                int num = haitidata[h, w];
                if (num > 0)
                {
                    skillButtons[num] = Instantiate(SkillButtonPre, treeBack.transform);
                    Buttons[num] = skillButtons[num];
                    Buttons[num].transform.localPosition = new Vector3(offset + (w) * masukanW, -offset + (h) * -masukanH);
                    Buttons[num].GetComponent<SkillButton>().init(num);
                    Buttons[num].GetComponentInChildren<Text>().text += num;
                    if (pst.Skills[num].skillImage != null) Buttons[num].transform.Find("skillImage").GetComponent<Image>().sprite = pst.Skills[num].skillImage;
                    else Buttons[num].transform.Find("skillImage").GetComponent<Image>().color = new Color(1, 1, 1, 0);

                    Buttons[num].GetComponent<Button>().onClick.AddListener(() => skillButtonClick(num));
                }
            }
        }
        //前提スキルボタンに対して枝を伸ばす。
        foreach (GameObject i in Buttons.Values)
        {
            if (null == pst.Skills[i.GetComponent<SkillButton>().skillnum].ZenteiSkillId) break;
            foreach (int k in pst.Skills[i.GetComponent<SkillButton>().skillnum].ZenteiSkillId)
            {
                var dis = Vector3.Distance(Buttons[k].transform.localPosition, i.transform.localPosition);
                var diff = (Buttons[k].transform.localPosition - i.transform.localPosition).normalized;
                var br = Instantiate(BranchPre, treeBack.transform);
                br.transform.rotation = Quaternion.FromToRotation(Vector3.up, diff);
                br.transform.SetParent(treeBack.transform);
                br.transform.localPosition = i.transform.localPosition;
                br.GetComponent<RectTransform>().sizeDelta = new Vector2(20, dis);
                br.transform.SetAsFirstSibling();
                Buttons[k].GetComponent<SkillButton>().Branch.Add(br);
            }
        }
    }


    //登録しているスキルのアイコンを、setskillsを参照して一括更新
    void RegiButKousin()
    {
        for (int i = 0; i < SET_SUU; i++)
        {
            //セットされていたら
            if (pst.SetSkills[i] != NONE)
            {
                var iconimg = regSkillButton[i].transform.Find("skillImage").GetComponent<Image>();
                iconimg.enabled = true;
                iconimg.sprite = pst.Skills[pst.SetSkills[i]].skillImage;
            }
            else
            {
                regSkillButton[i].transform.Find("skillImage").GetComponent<Image>().enabled = false;
            }
        }

    }

    //全てのスキルボタンの状態を更新,スキル選択した時に更新。
    void syutokuKousin()
    {
        //ボタンの状態を更新
        foreach (var i in skillButtons.Values)
        {
            i.GetComponent<SkillButton>().jotaiKousin();
        }
        //ソウルストーン個数を更新

        for (int i = 0; i < TREE_SUU; i++)
        {
            SoulStoneImage[i].transform.Find("Text").GetComponent<Text>().text = "x" + pst.GetComponent<PlayerItem>().items[i].kosuu;
        }
        //ソウルストーン消費量を更新
        if (selectSkill != NONE)
        {
            int red;
            int yellow;
            int blue;
            red = pst.Skills[selectSkill].UseSoul[SkillTreeButtonManager.RED];
            yellow = pst.Skills[selectSkill].UseSoul[SkillTreeButtonManager.YELLOW];
            blue = pst.Skills[selectSkill].UseSoul[SkillTreeButtonManager.BLUE];
            //Debug.Log("selectSkill = " + selectSkill);
            switch (skillButtons[selectSkill].GetComponent<SkillButton>().Jotai)
            {
                case SkillButton.jotai.GOT:
                    SyutokuButton.GetComponent<Button>().interactable = false;
                    SyutokuButton.GetComponentInChildren<Text>().text = "習得済み";
                    SoulStoneImage[RED].transform.Find("usesoultext").GetComponent<Text>().text = "";
                    SoulStoneImage[YELLOW].transform.Find("usesoultext").GetComponent<Text>().text = "";
                    SoulStoneImage[BLUE].transform.Find("usesoultext").GetComponent<Text>().text = "";
                    break;
                case SkillButton.jotai.UN_GOT:
                    if (pst.GetComponent<PlayerItem>().stoneEnoughCheck(pst.Skills[selectSkill].UseSoul[RED], pst.Skills[selectSkill].UseSoul[YELLOW], pst.Skills[selectSkill].UseSoul[BLUE]))
                    {
                        SyutokuButton.GetComponent<Button>().interactable = true;
                        SyutokuButton.GetComponentInChildren<Text>().text = "習得可能";
                    }
                    else
                    {
                        SyutokuButton.GetComponent<Button>().interactable = false;
                        SyutokuButton.GetComponentInChildren<Text>().text = "ソウルストーン\nが足りません";
                    }

                    SoulStoneImage[RED].transform.Find("usesoultext").GetComponent<Text>().text = (red == 0) ? "" : "-" + red;
                    SoulStoneImage[YELLOW].transform.Find("usesoultext").GetComponent<Text>().text = (yellow == 0) ? "" : "-" + yellow;
                    SoulStoneImage[BLUE].transform.Find("usesoultext").GetComponent<Text>().text = (blue == 0) ? "" : "-" + blue;
                    break;
                case SkillButton.jotai.CANT_GET:
                    SyutokuButton.GetComponent<Button>().interactable = false;
                    SyutokuButton.GetComponentInChildren<Text>().text = "前提スキル\n未習得";

                    SoulStoneImage[RED].transform.Find("usesoultext").GetComponent<Text>().text = (red == 0) ? "" : "-" + red;
                    SoulStoneImage[YELLOW].transform.Find("usesoultext").GetComponent<Text>().text = (yellow == 0) ? "" : "-" + yellow;
                    SoulStoneImage[BLUE].transform.Find("usesoultext").GetComponent<Text>().text = (blue == 0) ? "" : "-" + blue;
                    break;
            }
        }
        else
        {
            foreach (var i in SoulStoneImage)
            {
                i.transform.Find("usesoultext").GetComponent<Text>().text = "";
            }
        }
    }
    //右下のボタンを押したときの処理
    public void RegClick(int num)
    {
        if (deleteMode)
        {
            regSkillButton[num].transform.Find("SelectImage").gameObject.SetActive(false);
            pst.SetSkills[num] = NONE;
        }
        else
        {
            if (selectSkill == NONE) return;
            pst.SetSkills[num] = selectSkill;
        }
        RegiButKousin();
    }
    //syadaneventが押されたらsyadanpanelを非アクティブに
    public void Syadan()
    {
        foreach (var i in SyadanPanel)
        {
            i.SetActive(false);
        }
        foreach (var i in regSkillButton)
        {
            i.transform.Find("SelectImage").gameObject.SetActive(false);
        }
        SetumeiPanel.SetActive(false);
        selectSkill = NONE;
    }
    //スキルボタンを押したら
    public void skillButtonClick(int num)
    {
        selectSkill = num;
        selectImage.SetActive(true);
        SetumeiPanel.SetActive(true);
        var but_rect = skillButtons[num].GetComponent<RectTransform>();

        // var this_rect = GetComponent<RectTransform>();
        var view_rect = transform.Find("kariPanel").GetComponent<RectTransform>();
        Vector3 tp = view_rect.InverseTransformPoint(but_rect.position);
        //Debug.Log(but_rect.position);
        //Debug.Log(tp);
        var wide_size = view_rect.rect.width;
        var height_size = view_rect.rect.height;
        var pop_rect = SetumeiPanel.GetComponent<RectTransform>();
        Vector3 popPos = but_rect.position;
        //横計算
        if (tp.x < wide_size * 0.0f)
        {
            popPos.x += pop_rect.sizeDelta.x * pop_rect.lossyScale.x * 0.6f * 1+but_rect.sizeDelta.x;
        }
        else if (tp.x < wide_size * 0.3f)
        {
            popPos.x += pop_rect.sizeDelta.x * pop_rect.lossyScale.x * 0.6f * 1;
        }
        else if (tp.x < wide_size * 0.6f)
        {
            //デフォルト位置
        }
        else if (tp.x < wide_size * 1.0f)
        {
            popPos.x += pop_rect.sizeDelta.x * pop_rect.lossyScale.x * 0.6f * -1;
        }
        else
        {
            popPos.x += pop_rect.sizeDelta.x * pop_rect.lossyScale.x * 0.6f * -1- but_rect.sizeDelta.x;
        }

        //縦計算
        if (tp.y<height_size*0.0f)
        {
            popPos.y += pop_rect.sizeDelta.y * pop_rect.lossyScale.y * 1.0f * 1 + but_rect.sizeDelta.y;
        }
        else if (tp.y < height_size * 0.3f)
        {
            popPos.y += pop_rect.sizeDelta.y * pop_rect.lossyScale.y * 0.5f * 1;
        }
        else if (tp.y < height_size * 0.7f)
        {
            //デフォルト位置
        }
        else if (tp.y < height_size * 1.0f)
        {
            popPos.y += pop_rect.sizeDelta.y * pop_rect.lossyScale.y * 0.5f * -1;
        }
        else
        {
            popPos.y += pop_rect.sizeDelta.y * pop_rect.lossyScale.y * 0.5f * -1-but_rect.sizeDelta.y;
        }
        Debug.Log(but_rect.position);
        Debug.Log(tp);
        Debug.Log("wide=" + wide_size);
        Debug.Log("height=" + height_size);
        //trans_x = (GetComponent<RectTransform>().position.x > but_rect.position.x) ? 1 : -1;
        //trans_y = (GetComponent<RectTransform>().position.y > but_rect.position.y) ? 1 : -1;

        pop_rect.position = popPos;
        SetumeiPanel.transform.Find("SkillNameText").GetComponent<Text>().text = pst.Skills[num].SkillName;
        SetumeiPanel.transform.Find("SkillSetumei").GetComponent<Text>().text = pst.Skills[num].skillDescription;
        SetumeiPanel.transform.Find("SkillImage").GetComponent<Image>().sprite = pst.Skills[num].skillImage;
        foreach (var i in SyadanPanel)
        {
            i.SetActive(true);
        }
        selectImage.transform.position = skillButtons[num].transform.position;
        //習得してたらスキルセットのボタンを全てアクティブに。してなかったら非アクティブに
        bool syutokuFlag = pst.Skills[num].Syutoku;
        SkillSetSelectActive(syutokuFlag);
        syutokuKousin();
    }

    void SkillSetSelectActive(bool syutokuFlag)
    {
        deleteMode = false;
        foreach (var i in regSkillButton)
        {
            var s = i.transform.Find("SelectImage").gameObject;
            s.GetComponent<Image>().color = Color.blue;
            s.SetActive(syutokuFlag);
        }
    }

    public void SyutokuClick()
    {

        bool canSyutoku = true;
        //習得できるか判定
        for (int i = 0; i < TREE_SUU; i++)
        {
            if (pst.GetComponent<PlayerItem>().items[i].kosuu < pst.Skills[selectSkill].UseSoul[i])
            {
                canSyutoku = false;
            }
        }
        if (canSyutoku)
        {
            for (int i = 0; i < TREE_SUU; i++)
            {
                pst.GetComponent<PlayerItem>().items[i].kosuu -= pst.Skills[selectSkill].UseSoul[i];
            }
            pst.Skills[selectSkill].Syutoku = true;
            SkillSetSelectActive(true);
            syutokuKousin();
        }
    }

    public void KaijoClick()
    {
        deleteMode = true;
        for (int i = 0; i < SET_SUU; i++)
        {
            var r = regSkillButton[i].transform.Find("SelectImage").gameObject;
            r.SetActive(pst.SetSkills[i] != NONE);
            r.GetComponent<Image>().color = Color.red;
        }
        RegiButKousin();
    }
}
