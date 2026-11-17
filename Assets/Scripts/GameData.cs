using System;
using System.Collections.Generic;
using UnityEngine;

// Static helper: remembers the current player's name and stores one high score per name.
// Saved with PlayerPrefs, so it survives scene changes AND closing the game.
// No GameObject needed - don't attach this to anything.
public static class GameData
{
    private const string ScoresKey = "HighScores";
    private const string LastNameKey = "LastPlayerName";

    [Serializable] private class Entry { public string name; public int score; }
    [Serializable] private class Table { public List<Entry> entries = new List<Entry>(); }

    public const string DefaultName = "Player";
    private static string s_PlayerName;

    // Current player. Set by the menu, read by the game scene.
    public static string PlayerName
    {
        get
        {
            if (s_PlayerName == null)
                s_PlayerName = PlayerPrefs.GetString(LastNameKey, "");
            return string.IsNullOrWhiteSpace(s_PlayerName) ? DefaultName : s_PlayerName;
        }
        set
        {
            s_PlayerName = (value ?? "").Trim();
            PlayerPrefs.SetString(LastNameKey, s_PlayerName);
            PlayerPrefs.Save();
        }
    }

    private static Table Load()
    {
        string json = PlayerPrefs.GetString(ScoresKey, "");
        if (string.IsNullOrEmpty(json)) return new Table();
        return JsonUtility.FromJson<Table>(json) ?? new Table();
    }

    private static Entry Find(Table table, string name)
    {
        name = (name ?? "").Trim();
        // same name = same player, ignoring upper/lower case
        return table.entries.Find(e => string.Equals(e.name, name, StringComparison.OrdinalIgnoreCase));
    }

    public static bool HasHighScore(string name)
    {
        return Find(Load(), name) != null;
    }

    // 0 for a name that has never played, so a new name starts with no high score
    public static int GetHighScore(string name)
    {
        var entry = Find(Load(), name);
        return entry != null ? entry.score : 0;
    }

    // Saves only if the score beats that name's current best
    public static void SaveHighScore(string name, int score)
    {
        name = (name ?? "").Trim();
        if (name.Length == 0) name = DefaultName;

        var table = Load();
        var entry = Find(table, name);
        if (entry == null)
            table.entries.Add(new Entry { name = name, score = score });
        else if (score > entry.score)
            entry.score = score;
        else
            return;

        PlayerPrefs.SetString(ScoresKey, JsonUtility.ToJson(table));
        PlayerPrefs.Save();

        Debug.Log($"Saved {score} for {name}");
    }
}
