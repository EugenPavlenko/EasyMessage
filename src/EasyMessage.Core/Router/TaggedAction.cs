using System;

namespace EasyMessage.Core.Router
{
    public class TaggedAction<T>
    {
        public TaggedAction(string tag, Action<T> action)
        {
            Tag = tag;
            Action = action;
        }
        
        public string Tag { get; }
        public Action<T> Action { get; }
    }
}