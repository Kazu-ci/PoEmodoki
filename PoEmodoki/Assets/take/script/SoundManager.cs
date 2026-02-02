using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [Header("音源再生用")]
    [SerializeField] private AudioSource audioSource;

    //以下サウンドを登録
    public enum SE
    {
        Attack,
        Dead,
        GetSoul,
        Hit,
        AOE,
        EnemyDead,

    }

    [System.Serializable]
    public class SoundData
    {
        public SE type;
        public AudioClip clip;
    }

    [Header("SEリスト")]
    [SerializeField]private List<SoundData> soundList = new List<SoundData>();

    private void Awake()
    {
        if(Instance != null&&Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Update()
    {
        
    }

    public void PlaySE(SE type)
    {
        //リストから一致するSEを探す
        foreach(var data in soundList)
        {
            if(data.type == type)
            {
                audioSource.PlayOneShot(data.clip);
                return;
            }
        }
    }
}
