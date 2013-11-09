using System.Collections.Generic;

namespace JsonPrettyPrinterPlus.JsonPrettyPrinterInternals
{

    /// <summary>
    /// 
    /// </summary>
    public class PPScopeState
    {

        /// <summary>
        /// 
        /// </summary>
        public enum JsonScope
        {
            Object,
            Array
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly Stack<JsonScope> _jsonScopeStack = new Stack<JsonScope>();

        /// <summary>
        /// 
        /// </summary>
        public bool IsTopTypeArray
        {
            get
            {
                return _jsonScopeStack.Count > 0 && _jsonScopeStack.Peek() == JsonScope.Array;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ScopeDepth
        {
            get {
                return _jsonScopeStack.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void PushObjectContextOntoStack()
        {
            _jsonScopeStack.Push(JsonScope.Object);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonScope PopJsonType()
        {
            return _jsonScopeStack.Pop();
        }

        /// <summary>
        /// 
        /// </summary>
        public void PushJsonArrayType()
        {
            _jsonScopeStack.Push(JsonScope.Array);
        }
    }
}