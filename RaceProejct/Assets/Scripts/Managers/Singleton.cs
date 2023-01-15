using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace JH.Singleton
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        
        public static T Instance
        {
            get
            {
                if( _instance ==  null)
                {
                    // 초기화 전에 다른 인스턴스가 없는 지 확인.
                    _instance = FindObjectOfType<T>();

                    // 다른 인스턴스가 없는 경우 새로운 인스턴스 생성.
                    if(_instance == null)
                    {
                        GameManager obj = new GameManager();
                        obj.name = typeof(T).Name;
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        public virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;

                // 새로운 씬이 로드 되었을 때, 대상 오브젝트가 삭제되는 것을 막고
                // 오브젝트의 현재 인스턴스가 씬 사이클 전환할 떄도 유지되도록 보장.
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
