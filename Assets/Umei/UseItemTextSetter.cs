using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UseItemTextSetter : MonoBehaviour {
    [SerializeField]
    Text useItemValueText;
    public Text UseItemValueText
    {
        get { return useItemValueText; }
    }

    [SerializeField]
    Text itemValueText;
    public Text ItemValueText
    {
        get { return itemValueText; }
    }

    [SerializeField]
    Image itemImage;
    public Image ItemImage
    {
        get { return itemImage;}
    }
}
