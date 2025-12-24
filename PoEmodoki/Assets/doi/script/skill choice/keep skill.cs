using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.SceneManagement;
using UnityEngine;


public class keepskill : MonoBehaviour
{
    
     PlayerCon player; // allskill / mySkills を持つ

    [Header("Inventory UI (unique slots)")]
    [SerializeField] private SkillslotUI[] inventorySlots; // 例：10枠
    [SerializeField] private List<RectTransform> otherImages = new(); // 当たり判定用なら使う

    [Header("Equip UI (slot1-3)")]
    [SerializeField] private SkillslotUI[] equipSlots = new SkillslotUI[3]; // slot1-3のUI

    // 表示順を安定させるための「登場順リスト」
    // （火×3でも inventoryには火が1回だけ出る。順番も毎回変わらない）
    private readonly List<SkillStatus> inventoryOrder = new();

    void Start()
    {
        RefreshAllUI();
    }
    void Awake()
    {
        if (player == null)
            player = FindObjectOfType<PlayerCon>();
    }
    // ===== UI更新（方法B：まとめて×個表示） =====
    public void RefreshAllUI()
    {
        Debug.Log($"player null? {player == null}");
        if (player != null)
        {
            Debug.Log($"allskill null? {player.allskill == null}");
            Debug.Log($"mySkills null? {player.mySkills == null}");
        }

        if (player == null || player.allskill == null || player.mySkills == null)
        {
            Debug.LogError("player / allskill / mySkills が未設定");
            return;
        }


        // 1) 所持数を集計
        Dictionary<SkillStatus, int> counts = BuildCounts(player.allskill);

        // 2) 表示順 inventoryOrder を更新（所持してるものだけ残す）
        SyncInventoryOrder(counts);

        // 3) インベントリUI更新（1種類=1枠 + ×count）
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            var slotUI = inventorySlots[i];
            if (slotUI == null) continue;

            if (i < inventoryOrder.Count)
            {
                var skill = inventoryOrder[i];
                slotUI.gameObject.SetActive(true);
                slotUI.SetSkill(skill);
                slotUI.SetCount(counts.TryGetValue(skill, out int c) ? c : 0);
            }
            else
            {
                slotUI.Clear();
                slotUI.gameObject.SetActive(false);
            }
        }

        // 当たり判定用にRectTransform一覧が必要ならここで作る
        otherImages.Clear();
        foreach (var s in inventorySlots)
            if (s != null) otherImages.Add(s.GetComponent<RectTransform>());

        // 4) 装備UI更新（slot1-3）
        for (int i = 0; i < equipSlots.Length; i++)
        {
            var ui = equipSlots[i];
            if (ui == null) continue;

            SkillStatus equipped = (i < player.mySkills.Count) ? player.mySkills[i] : null;
            ui.SetSkill(equipped);

            // 装備枠にも残弾を出したいならここで表示
            if (equipped != null && counts.TryGetValue(equipped, out int c))
                ui.SetCount(c);
            else
                ui.SetCount(0);
        }
    }

    private Dictionary<SkillStatus, int> BuildCounts(List<SkillStatus> allskill)
    {
        var counts = new Dictionary<SkillStatus, int>();
        foreach (var s in allskill)
        {
            if (s == null) continue;
            counts.TryGetValue(s, out int c);
            counts[s] = c + 1;
        }
        return counts;
    }

    private void SyncInventoryOrder(Dictionary<SkillStatus, int> counts)
    {
        // 既にorderにあるもののうち、所持0になったものは消す
        for (int i = inventoryOrder.Count - 1; i >= 0; i--)
        {
            if (!counts.ContainsKey(inventoryOrder[i]))
                inventoryOrder.RemoveAt(i);
        }

        // 新しく入手したスキルを末尾に追加（登場順が安定）
        foreach (var kv in counts)
        {
            if (!inventoryOrder.Contains(kv.Key))
                inventoryOrder.Add(kv.Key);
        }
    }

    // ===== 入手（敵ドロップ → インベントリへ） =====
    public void AddSoul(SkillStatus skill)
    {
        if (skill == null) return;
        player.allskill.Add(skill);
        RefreshAllUI();
    }

    // ===== 装備（インベントリから選んで slotIndex に入れる） =====
    public void EquipToSlot(int slotIndex, SkillStatus skill)
    {
        if (skill == null) return;
        if (slotIndex < 0 || slotIndex > 2) return;

        // 所持がないなら装備できない
        if (!player.allskill.Contains(skill))
        {
            Debug.Log("所持してないスキルは装備できない");
            return;
        }

        // mySkills を 3枠分確保（null埋め）
        while (player.mySkills.Count < 3) player.mySkills.Add(null);

        player.mySkills[slotIndex] = skill;
        RefreshAllUI();
    }

    // ===== 使用（slot1-3のどれかを使う） =====
    public void UseEquipped(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex > 2) return;
        if (player.mySkills.Count <= slotIndex) return;

        SkillStatus skill = player.mySkills[slotIndex];
        if (skill == null) return;

        // 所持（弾数）がある？
        int idx = player.allskill.IndexOf(skill);
        if (idx == -1)
        {
            // 0発なら装備解除
            player.mySkills[slotIndex] = null;
            RefreshAllUI();
            return;
        }

        // 1発消費
        player.allskill.RemoveAt(idx);

        // 0になったら装備解除（任意：自動解除するのが仕様ならON）
        if (!player.allskill.Contains(skill))
            player.mySkills[slotIndex] = null;

        RefreshAllUI();
    }
}
