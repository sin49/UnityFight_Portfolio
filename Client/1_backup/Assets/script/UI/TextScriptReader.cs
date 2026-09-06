using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextScriptReader 
{
    
    public static string TextFileReader(string textfilename,string TextfileException)
    {
        string filepath = Directory.GetCurrentDirectory() + $"\\Assets\\Text\\{textfilename}.{TextfileException}";
        return File.ReadAllText(filepath) ;
    }
    public static void TextFileWriter(string textfilename, string TextfileException,string Text)
    {
        string filepath = Directory.GetCurrentDirectory() + $"\\Assets\\Text\\{textfilename}.{TextfileException}";
        File.WriteAllText(filepath, Text);
    }
    public static string ReplaceTextScript(string text,string slicetext, string inserttext)
    {
        int tmp = text.IndexOf(slicetext + ")");
        int tmpcount = (slicetext + ")").Length;
        int tmp2 = text.IndexOf("<" + slicetext+">");
        string tmptext= text.Remove(tmp + tmpcount, tmp2 - tmp + tmpcount);
        return tmptext.Insert(tmp + tmpcount, inserttext);
    }
    public static string SliceTextScript(string text,string slicetext)
    {
        int tmp = text.IndexOf(slicetext + ")");

        int tmpcount = (slicetext + ")").Length;
        int tmp2 = text.IndexOf("<" + slicetext + ">");
    
        Debug.Log("substring:"+text.Substring(tmp + tmpcount, tmp2-tmpcount-tmp));
        return text.Substring(tmp + tmpcount, tmp2-tmpcount-tmp);
    }
}
