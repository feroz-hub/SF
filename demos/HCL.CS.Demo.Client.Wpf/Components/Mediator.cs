using System;
using System.Collections.Generic;

namespace HCL.CS.DemoClientWpfApp.Components
{
    public static class Mediator
    {
        private static IDictionary<string, List<Action<object>>> actionList =
           new Dictionary<string, List<Action<object>>>();

        public static void Subscribe(string token, Action<object> callback)
        {
            if (!actionList.ContainsKey(token))
            {
                var list = new List<Action<object>>();
                list.Add(callback);
                actionList.Add(token, list);
            }
            else
            {
                bool found = false;
                foreach (var item in actionList[token])
                {
                    if (item.Method.ToString() == callback.Method.ToString())
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    actionList[token].Add(callback);
                }
            }
        }

        public static void Unsubscribe(string token, Action<object> callback)
        {
            if (actionList.ContainsKey(token))
            {
                actionList[token].Remove(callback);
            }
        }

        public static void Notify(string token, object args = null)
        {
            if (actionList.ContainsKey(token))
            {
                foreach (var callback in actionList[token])
                {
                    callback(args);
                }
            }
        }
    }
}


