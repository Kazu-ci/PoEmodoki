using NUnit.Framework;
using UnityEngine;
using UnityEngine.Audio;

public class BGMManager : MonoBehaviour
{
    [SerializeField]AudioSource AudioSource;
    [SerializeField]AudioClip[] clipList;
    private bool isBossBGMPlaying = false;
    // Update is called once per frame
    void Update()
    {
        // ボスエリアに入った ＆ まだボス曲を流していない場合
        if (GameCon.Instance.BossArea() && !isBossBGMPlaying)
        {
            PlayBGM(1); // clipList[1] をボス曲とする
            isBossBGMPlaying = true;
        }
        
    }

    private void PlayBGM(int index)
    {
        if (index < 0 || index >= clipList.Length) return;

        AudioSource.Stop();
        AudioSource.clip = clipList[index];
        AudioSource.Play();
    }
}
