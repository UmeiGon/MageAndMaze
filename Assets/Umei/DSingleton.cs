using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//シーンを跨がない&&1つしか存在してはいけない&&MonoBehaviourを継承させたい、時に継承させる。
namespace UMI
{
    public class DSingleton<T> : MonoBehaviour
where T : DSingleton<T>
    {

        static T instance;
        //debug用
        static T Instance
        {
            set { instance = value; Debug.Log("新しくSingletonの" + value + "が作成されました。"); }
            get { return instance; }
        }
        public static T GetInstance()
        {
            if (Instance == null)
            {
                //無かった場合新しくscene上に作る
                var obj = new GameObject("SingletonEmpty");
                Instance = obj.AddComponent<T>();
            }
            return Instance;
        }
        //上書きしてほしくないのでprivateに
        private void Awake()
        {
            //staticのinstanceがnullの場合自分を入れる。
            if (Instance == null)
            {
                Instance = (T)this;
            }

            //staticのinstanceが自分じゃない場合自殺
            if (Instance != this)
            {
                Destroy(gameObject);
            }
            SAwake();
        }
        //継承したclassがAwake使う時、これを使う。
        protected virtual void SAwake() { }
    }
}
