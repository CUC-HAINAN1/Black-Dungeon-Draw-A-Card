using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public class ChineseCharacterExtractor
{
    [MenuItem("Tools/提取中文字符用于 TMP")]
    static void ExtractChineseCharacters()
    {
        string[] fileExtensions = { ".cs", ".txt", ".json", ".csv", ".asset", ".prefab", ".xml", ".uxml" };
        string[] allFiles = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);

        HashSet<char> chineseCharacters = new HashSet<char>();
        Regex chineseRegex = new Regex("[\u4e00-\u9fff]", RegexOptions.Compiled);

        foreach (var file in allFiles)
        {
            if (!fileExtensions.Contains(Path.GetExtension(file))) continue;

            try
            {
                string content = File.ReadAllText(file, Encoding.UTF8);
                MatchCollection matches = chineseRegex.Matches(content);

                foreach (Match match in matches)
                {
                    chineseCharacters.Add(match.Value[0]);
                }
            }
            catch
            {
                CustomLogger.LogWarning("跳过无法读取的文件: " + file);
            }
        }

        StringBuilder sb = new StringBuilder();
        foreach (var ch in chineseCharacters)
        {
            sb.Append(ch);
        }

        string savePath = EditorUtility.SaveFilePanel("保存字符集", Application.dataPath, "TMP_Characters.txt", "txt");
        if (!string.IsNullOrEmpty(savePath))
        {
            File.WriteAllText(savePath, sb.ToString(), Encoding.UTF8);
            EditorUtility.RevealInFinder(savePath);
            CustomLogger.Log($"✅ 提取完毕，共 {chineseCharacters.Count} 个中文字符");
        }
    }
}
