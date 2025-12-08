using UnityEngine;
using UnityEngine.UI;
public class imagemove : MonoBehaviour
{
    // 依存するRectTransformをInspectorで設定（publicフィールドまたはSerializeFieldを使う）
    public RectTransform imageRect;
    public RectTransform slot0;
    public RectTransform slot1;
    public RectTransform slot3;

    // 許容誤差（floatの比較に使用）
 
    // 移動範囲とfloat比較の許容誤差
    private const float MAX_Y_POSITION = 150f;
    private const float MIN_Y_POSITION = -150f;
    private const float Y_TOLERANCE = 1.0f;
    private void Update()
    {
        Vector2 move = Vector2.zero;
        bool jumped = false; // 瞬間移動が発生したかを追跡

        // --- 1. 左右の移動 (GetKeyDown: 1回押すと1マス移動) ---
        // 左右は純粋な段階的移動と判断し、最初に処理
        if (Input.GetKeyDown(KeyCode.RightArrow) && imageRect.anchoredPosition.x < 350 && imageRect.anchoredPosition.x != slot3.anchoredPosition.x)
        {
            move.x = 150;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && imageRect.anchoredPosition.x > -400 && imageRect.anchoredPosition.x != slot1.anchoredPosition.x)
        {
            move.x = -150;
        }

        // --- 2. 瞬間移動 (ジャンプ) 処理 ---
        // 特定のY座標からのみジャンプし、それ以外は段階的移動を許可する

        // DownArrow Jump: Y座標が120に極めて近い位置から押された場合のみ、slot0へジャンプ
        // floatの厳密な比較を避けるため、Mathf.Absを使用
        if (Input.GetKey(KeyCode.DownArrow) && Mathf.Abs(imageRect.anchoredPosition.y - 120f) < Y_TOLERANCE)
        {
            imageRect.anchoredPosition = slot0.anchoredPosition;
            jumped = true;
        }

        // ※ UpArrowの瞬間移動ロジック（y > 0でslot1へジャンプ）は、
        // 段階的な上移動を阻害するため、削除しました。
        // もし特定のY座標からのみジャンプさせたい場合は、上記DownArrowと同様に厳密な条件を設定してください。

        // --- 3. 段階的上下移動 処理 ---

        // 瞬間移動が発生していない場合のみ、段階的移動を計算する
        if (!jumped)
        {
            // 上へ移動：最大Y座標よりも小さい場合、上に移動可能
            if (Input.GetKey(KeyCode.UpArrow) && imageRect.anchoredPosition.y < MAX_Y_POSITION)
            {
                move.y = 150;
            }
            // 下へ移動：最小Y座標よりも大きく、かつDownArrowキーが瞬間移動に使われていない場合
            else if (Input.GetKey(KeyCode.DownArrow) && imageRect.anchoredPosition.y > MIN_Y_POSITION)
            {
                move.y = -150;
            }
        }

        // --- 4. 最終的な位置の適用 ---
        imageRect.anchoredPosition += move;
    }
}