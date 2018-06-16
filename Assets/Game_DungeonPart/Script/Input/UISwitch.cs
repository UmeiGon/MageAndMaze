using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SubUIType
{
    CHECK_NEXT_FLOOR,
    INTERACT,
    SubUITypeMax
}

public class UISwitch : MonoBehaviour {

    GameObject parent;
    TutorialManager tutorialMn;
    SESet seSet;

    [SerializeField]
    Canvas[] uis;
    [SerializeField]
    GameObject[] subUIs = new GameObject[(int)SubUIType.SubUITypeMax];

    [SerializeField]
    GameObject subCamera;

    [SerializeField]
    GameObject uiMenuButtons;

    public bool Interactable = true;
    public DungUIType UIType;

   
    // Use this for initialization
    void Start () {
        uis[3] = SkillTreeButtonManager.GetInstance().transform.root.GetComponent<Canvas>();
        uis[2] = ItemButtonManager.GetInstance().transform.root.GetComponent<Canvas>();
        for ( int t = 0; t < uis.Length; t++ )
        {
            if ( uis[t] )
            {
                // 可視化
                uis[t].GetComponent<Canvas>().enabled = true;
            }
        }
        uiMenuButtons.GetComponent<Canvas>().enabled = true;

        parent = GameObject.Find("GameObjectParent");
        tutorialMn = parent.GetComponentInChildren<TutorialManager>();
    }
	
    public void SwitchUI(int type)
    {
        if ( !parent ) Start();

        // チュートリアル中の操作制限
        if ( tutorialMn.IsTutorialON )
        {
            if ( ( 1 <= tutorialMn.TutorialNumber
                && tutorialMn.TutorialNumber <= 6 )
                //|| ( 5 <= tutorialMn.TutorialNumber
                //&& tutorialMn.TutorialNumber <= 6 )
                )
            {
                // ステータス画面への切り替え不可
                if ( type == (int)DungUIType.INVENTRY )
                    return;
            }
            if ( 8 <= tutorialMn.TutorialNumber
            && tutorialMn.TutorialNumber <= 11 )
            {
                // スキルツリー以外への切り替え不可
                if ( type != (int)DungUIType.EVENT
                    && type != (int)DungUIType.SKILLTREE )
                    return;
            }
            if ( 12 <= tutorialMn.TutorialNumber
            && tutorialMn.TutorialNumber <= 13 )
            {
                // 会話UIと修練＆精製UI以外への切り替え不可
                if ( type != (int)DungUIType.EVENT
                    && type != (int)DungUIType.PRACTICE_AND_ITEMCRAFT )
                    return;
            }
        }

        if ( !Interactable ) return;
        
        // type に合致するものだけアクティブ化
        for (int t = 0; t < (int)DungUIType.DungUITypeMax; t++)
        {
            if ( uis[t] )
            {
                if ((int)DungUIType.INVENTRY <= type && type <= (int)DungUIType.SKILLTREE
                    && (int)DungUIType.INVENTRY <= t && t <= (int)DungUIType.SKILLTREE )
                {
                    uis[t].gameObject.SetActive(true);
                    uis[t].GetComponent<Canvas>().enabled = ( t == type );
                }
                else uis[t].gameObject.SetActive( t == type );
            }
        }

        // サブカメラON/OFF
        subCamera.SetActive((type == (int)DungUIType.INVENTRY) ? true : false);

        // 「修練＆精製」「ステータス」などのウィンドウ選択ボタン
        uiMenuButtons.SetActive(( (int)DungUIType.INVENTRY <= type && type <= (int)DungUIType.SKILLTREE ) ? 
            true : false);

        UIType = (DungUIType) type;
    }

    public void SwitchSubUI(int type, bool on)
    {
        subUIs[type].SetActive(on);
    }


}
