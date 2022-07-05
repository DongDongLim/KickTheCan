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
}
