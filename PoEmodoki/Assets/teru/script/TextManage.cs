using TMPro;
using UnityEngine;

public class TextManage : MonoBehaviour
{
    public float blinkSpeed = 2.0f;

    // TextMeshProコンポーネントを格納する変数
    private TextMeshProUGUI textComponent;

    void Start()
    {
        // アタッチされているTextMeshProUGUIコンポーネントを取得
        textComponent = GetComponent<TextMeshProUGUI>();

        // コンポーネントが見つからない場合はエラーを出して処理を停止
        if (textComponent == null)
        {
            Debug.LogError("TextMeshProUGUI コンポーネントが見つかりません。");
            enabled = false; // スクリプトを無効にする
        }
    }

    void Update()
    {
        // 1. サイン波を使ってアルファ値（透明度）を計算します。
        // Time.time * blinkSpeed で、時間が経つにつれてサイン波を動かします。
        // Mathf.Sin() の結果は -1 から 1 の範囲です。

        // 2. Mathf.Abs() で絶対値を取り、結果を 0 から 1 の範囲にします。
        // これにより、滑らかに 0 → 1 → 0 → 1 ... と変化します。
        float alpha = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));

        // 3. 現在のテキストの色を取得します。
        Color color = textComponent.color;

        // 4. 取得した色のアルファ値 (A) を計算した値で上書きします。
        color.a = alpha;

        // 5. 新しい色をテキストに設定します。
        textComponent.color = color;
    }
}
