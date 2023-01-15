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
                    // �ʱ�ȭ ���� �ٸ� �ν��Ͻ��� ���� �� Ȯ��.
                    _instance = FindObjectOfType<T>();

                    // �ٸ� �ν��Ͻ��� ���� ��� ���ο� �ν��Ͻ� ����.
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

                // ���ο� ���� �ε� �Ǿ��� ��, ��� ������Ʈ�� �����Ǵ� ���� ����
                // ������Ʈ�� ���� �ν��Ͻ��� �� ����Ŭ ��ȯ�� ���� �����ǵ��� ����.
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
