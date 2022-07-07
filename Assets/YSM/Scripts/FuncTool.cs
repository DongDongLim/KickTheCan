using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuncTool
{

    static public IDictionary<string, object> ConvertToIDictionary(string key, string stringData)
    {
        IDictionary<string, object> tmp = new Dictionary<string, object>();
        tmp.Add(key, stringData);
        return tmp;
    }

    static public IDictionary<string, object> ConvertToIDictionary(string key, bool booleanData)
    {
        IDictionary<string, object> tmp = new Dictionary<string, object>();
        tmp.Add(key, booleanData);
        return tmp;
    }

    static public IDictionary<string, object> ConvertToIDictionary(string key, int integerData)
    {
        IDictionary<string, object> tmp = new Dictionary<string, object>();
        tmp.Add(key, integerData);
        return tmp;
    }

    static public string CompareStrings(string str1 ,string str2)
    {
        switch (string.Compare(str1, str2))
        {
            case -1:
                return str1 + str2;
            case 1:
                return str2 + str1;
            default: return "Error";
        }
    }
}
