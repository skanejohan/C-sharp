using Theseus.Elements.Enumerations;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    // TODO We will probably need to access the semantics from e.g. EmitJavaScriptCode. How? 
    public class Effect : IElement, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
        public EffectType Type { get; }
        public string Object { get; }
        public string Host { get; }

        public Effect(EffectType type, string obj, string host = "")
        {
            Type = type;
            Object = obj;
            Host = host;
        }

        public string EmitTheseusCode(int indent = 0)
        {
            switch (Type)
            {
                case EffectType.Add:
                    return $"add {Object} to {Host}";
                case EffectType.Set:
                    return $"set {Object}";
                case EffectType.Clear:
                    return $"clear {Object}";
                case EffectType.Show:
                    return $"show {Object}";
                case EffectType.Hide:
                    return $"hide {Object}";
                case EffectType.MoveTo:
                    return $"move to {Object}";
                case EffectType.Lock:
                    return $"lock {Object}";
                case EffectType.Close:
                    return $"close {Object}";
                case EffectType.Open:
                    return $"open {Object}";
                case EffectType.Unlock:
                    return $"unlock {Object}";
            }
            return "";
        }

        public string EmitJavaScriptCode(int indent = 0)
        {
            var s = "TODO";
            switch (Type)
            {
                //case EffectType.Add:
                //    return $"add {Object} to {Host}";
                case EffectType.Set:
                    s = $"game.flags.set(Flag.{Object});";
                    break;
                case EffectType.Clear:
                    s = $"game.flags.clear(Flag.{Object});";
                    break;
                case EffectType.Show:
                    s = $"game.verbs.add({Object});"; // TODO or item
                    break;
                case EffectType.Hide:
                    s = $"game.verbs.remove({Object});"; // TODO or item
                    break;
                    //case EffectType.Hide:
                    //    return $"hide {Object}";
                    //case EffectType.MoveTo:
                    //    return $"move to {Object}";
                    //case EffectType.Lock:
                    //    return $"lock {Object}";
                    //case EffectType.Close:
                    //    return $"close {Object}";
                    //case EffectType.Open:
                    //    return $"open {Object}";
                    //case EffectType.Unlock:
                    //    return $"unlock {Object}";
            }
            return s.Indent(indent);
        }

    }
}
