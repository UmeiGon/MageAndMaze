using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButtonManager : UMI.DSingleton<ItemButtonManager>
{
    private Dictionary<int, ItemData> items;
    public GameObject setumeiPanel;
    [SerializeField]
    private GameObject eleLvUpCheckPanel;
    [SerializeField]
    private GameObject blockkingPanel;
    [SerializeField]
    private GameObject AttributeTraningPanel;
    private Player player;
    private const int maxSyouhiItem = 3;
    [SerializeField]
    RectTransform seiseiButton;
    public RectTransform SeiseiButton { get { return seiseiButton; } }

    [SerializeField]
    private UseItemTextSetter[] useItemTexts;
    private List<GameObject> createItemsButton = new List<GameObject>();
    public List<GameObject> CreateItemsButton {
        get { return createItemsButton; }
    }
    private List<int> canCreateItemsId = new List<int>();
    private int traningStoneColor;
    //0~11のパネルの番号-1は未選択
    private int selectPanelNum = -1;
    int[] useItemSyuren = new int[3];
    bool inited = false;
    AtkAndDef playerAAD = null;

    private void Start()
    {
        items = GameObject.Find("GameObjectParent").GetComponentInChildren<PlayerItem>().items;
        player = GameObject.Find("GameObjectParent").GetComponentInChildren<Player>();
        playerAAD = player.GetComponent<AtkAndDef>();
        for (int i = 0; i < 12; i++)
        {
            createItemsButton.Add(transform.Find("CreateImage (" + i + ")").gameObject);
        }

        selectedPanel();
        setCreateItems();
        UpdateUseItemSyuren();
        //ソウルストーンのimageをset
        for (int i = 0; i < useItemTexts.Length; i++)
        {
            useItemTexts[i].ItemImage.sprite = items[i].itemImage;
        }

        lvUpCheckReload();
        useItemReload();
        eleLvUpCheckPanel.SetActive(false);
        inited = true;
    }

    void UpdateUseItemSyuren()
    {
        useItemSyuren[(int)PlayerItem.stone.RED_STONE] = 5 + ((int)((playerAAD.FlameMagicPower - 1) * 10) / 5) * 5;
        useItemSyuren[(int)PlayerItem.stone.YELLOW_STONE] = 5 + ((int)((playerAAD.LightMagicPower - 1) * 10) / 5) * 5;
        useItemSyuren[(int)PlayerItem.stone.BLUE_STONE] = 5 + ((int)((playerAAD.IceMagicPower - 1) * 10) / 5) * 5;
    }

    private void OnEnable()
    {
        if (inited)
        {
            selectedPanel();
            setCreateItems();
            useItemReload();
            lvUpCheckReload();
            eleLvUpCheckPanel.SetActive(false);
        }

    }
    //-1はセレクトしなかった場合の処理
    private void selectedPanel(int panelnum = -1)
    {
        selectPanelNum = panelnum;
        bool active = (panelnum != -1);
        setumeiPanel.SetActive(active);
        blockkingPanel.SetActive(active);
        if (active)
        {
            setumeiPanel.transform.Find("description").GetComponent<Text>().text = items[canCreateItemsId[panelnum]].setumei;
            setumeiPanel.transform.Find("name").GetComponent<Text>().text = "" + items[canCreateItemsId[panelnum]].name;
            setumeiPanel.transform.Find("haveNumbers").GetComponent<Text>().text = "所持数:" + items[canCreateItemsId[panelnum]].kosuu;

            setumeiPanel.transform.Find("itemImage").GetComponent<Image>().sprite = items[canCreateItemsId[panelnum]].itemImage;
        }
    }
    public void BlockkingOnClick()
    {
        selectedPanel();
        useItemReload();
        eleLvUpCheckPanel.SetActive(false);
    }

    public void ItemsOnClick(int panelnum)
    {
        eleLvUpCheckPanel.SetActive(false);
        selectedPanel(panelnum);
        useItemReload(items[canCreateItemsId[panelnum]].syouhiSozai);
        lvUpCheckReload();
    }
    public void CreateOnClick()
    {
        int createItemId = canCreateItemsId[selectPanelNum];
        ItemCreate();
        setCreateItems();
        //もし前選んだスキルが今回も作れるなら、引き続き同じアイテムの場所に作成パネルが呼ばれる。
        int panelnum = -1;
        for (int i = 0; i < canCreateItemsId.Count; i++)
        {
            if (createItemId == canCreateItemsId[i])
            {
                panelnum = i;
            }
        }
        selectedPanel(panelnum);
        useItemReload((panelnum == -1) ? null : items[canCreateItemsId[panelnum]].syouhiSozai);

    }


    public void eleLvUpOnClick(int stone_color)
    {
        switch (stone_color)
        {
            case 0:
                eleLvUpCheckPanel.transform.Find("Attribute").GetComponent<Text>().text = "火属性修練";
                eleLvUpCheckPanel.transform.Find("Attribute").GetComponent<Text>().color = Color.red;
                break;
            case 1:
                eleLvUpCheckPanel.transform.Find("Attribute").GetComponent<Text>().text = "雷属性修練";
                eleLvUpCheckPanel.transform.Find("Attribute").GetComponent<Text>().color = Color.yellow;
                break;
            case 2:
                eleLvUpCheckPanel.transform.Find("Attribute").GetComponent<Text>().text = "水属性修練";
                eleLvUpCheckPanel.transform.Find("Attribute").GetComponent<Text>().color = Color.blue;
                break;
        }
        Dictionary<int, int> useStones = new Dictionary<int, int>();
        traningStoneColor = stone_color;
        useStones[stone_color] = useItemSyuren[traningStoneColor];
        useItemReload(useStones);
        blockkingPanel.SetActive(true);
        setumeiPanel.SetActive(false);
        eleLvUpCheckPanel.SetActive(true);
    }
    public void yesOnClick()
    {
        if (items == null) return;

        items[traningStoneColor].kosuu -= useItemSyuren[traningStoneColor];
        player.ElementLevelUp(traningStoneColor + 1);
        Dictionary<int, int> useStones = new Dictionary<int, int>();
        UpdateUseItemSyuren();
        if (items[traningStoneColor].kosuu < useItemSyuren[traningStoneColor]) eleLvUpCheckPanel.SetActive(false);
        useStones[(int)traningStoneColor] = useItemSyuren[traningStoneColor];
        useItemReload((items[traningStoneColor].kosuu >= useItemSyuren[traningStoneColor]) ? useStones : null);
        setCreateItems();
        GameObject.Find("GameObjectParent").GetComponentInChildren<PlayerDetailInfo>().UpdateStatusText();
        lvUpCheckReload();
    }
    public void noOnClick()
    {
        useItemReload();
        eleLvUpCheckPanel.SetActive(false);
    }
    //アイテム個数を増やす
    private void ItemCreate()
    {
        foreach (var i in items[canCreateItemsId[selectPanelNum]].syouhiSozai)
        {
            items[i.Key].kosuu -= i.Value;
        }
        items[canCreateItemsId[selectPanelNum]].kosuu++;
        GameObject.Find("GameObjectParent").GetComponentInChildren<InventryInfo>().UpdateInventry();
        lvUpCheckReload();
    }

    private void lvUpCheckReload()
    {
        foreach (int s in Enum.GetValues(typeof(PlayerItem.stone)))
        {
            switch (s)
            {
                case 0:
                    AttributeTraningPanel.transform.Find("" + s).transform.Find("Rate").GetComponent<Text>().text = string.Format("{0:0.00}", player.atkAndDef.FlameMagicPower) + "倍";
                    break;
                case 1:
                    AttributeTraningPanel.transform.Find("" + s).transform.Find("Rate").GetComponent<Text>().text = string.Format("{0:0.00}", player.atkAndDef.LightMagicPower) + "倍";
                    break;
                case 2:
                    AttributeTraningPanel.transform.Find("" + s).transform.Find("Rate").GetComponent<Text>().text = string.Format("{0:0.00}", player.atkAndDef.IceMagicPower) + "倍";
                    break;
            }

            if (useItemSyuren[s] <= items[s].kosuu)
            {
                AttributeTraningPanel.transform.Find("" + s).GetComponent<Button>().interactable = true;

            }
            else
            {
                AttributeTraningPanel.transform.Find("" + s).GetComponent<Button>().interactable = false;

            }

        }
    }
    //消費するアイテム個数を更新
    private void useItemReload(Dictionary<int, int> use_items = null)
    {

        for (int i = 0; i < useItemTexts.Length; i++)
        {
            useItemTexts[i].ItemValueText.text = "x" + items[i].kosuu;
            if (use_items != null && use_items.ContainsKey(i) && use_items[i] != 0)
            {
                useItemTexts[i].UseItemValueText.text = "-" + use_items[i];
            }
            else
            {
                useItemTexts[i].UseItemValueText.text = "";
            }
        }
    }
    //作成できるアイテムを更新
    void setCreateItems()
    {
        canCreateItemsId.Clear();
        foreach (var i in items)
        {
            if (canCreate(i.Key))
            {
                canCreateItemsId.Add(i.Key);
            }
        }
        for (int n = 0; n < 12; n++)
        {
            if (n < canCreateItemsId.Count)
            {
                createItemsButton[n].transform.Find("ItemImage").GetComponent<Image>().enabled = true;
                createItemsButton[n].GetComponentInChildren<Button>().interactable = true;
                createItemsButton[n].transform.Find("ItemImage").GetComponent<Image>().sprite = items[canCreateItemsId[n]].itemImage;
            }
            else
            {
                createItemsButton[n].GetComponentInChildren<Button>().interactable = false;
                createItemsButton[n].transform.Find("ItemImage").GetComponent<Image>().enabled = false;
            }

        }
    }

    bool canCreate(int itemId)
    {
        if (items[itemId].syouhiSozai == null)
        {
            return false;
        }
        if (items[itemId].syouhiSozai.Count == 0)
        {
            return false;
        }
        foreach (var i in items[itemId].syouhiSozai)
        {
            if (!(items[i.Key].kosuu >= i.Value))
            {
                return false;
            }
        }
        return true;
    }
}
