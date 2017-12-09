using System;
using Theseus.Elements.Enumerations;
using Theseus.Elements.JavaScriptUtils;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Effect : IElement, ISemanticsValidator, ITheseusCodeEmitter, IJavaScriptCodeEmitter
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

        public virtual void BuildSemantics(ISemantics semantics)
        {
        }

        public virtual void CheckSemantics(ISemantics semantics)
        {
            if (Type == EffectType.Set ||
                Type == EffectType.Clear)
            {
                if (!semantics.HasFlagByName(Object))
                {
                    throw new Exception($"Unknown flag \"{Object}\"");
                }
            }
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

        public void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb)
        {
            var gName = $"THESEUS.{GameUtils.GameName.ToUpper()}";
            string obj = "";
            string fun = "";
            if (Object.Contains("."))
            {
                obj = Object.Split('.')[0];
                fun = Object.Split('.')[1];
            }
            else
            {
                obj = Object;
            }

            switch (Type)
            {
                // TODO additional eeffect types
                case EffectType.Set:
                    cb.Add($"context.flags().add({gName}.Flag.{Object});");
                    break;
                case EffectType.Clear:
                    cb.Add($"context.flags().delete({gName}.Flag.{Object});");
                    break;
                case EffectType.Show:
                    if (semantics.HasFunctionByName(fun))
                    {
                        cb.Add($"{gName}.{obj}.setFunctionVisible(\"{fun}\", true);");
                    }
                    else
                    {
                        cb.Add($"{gName}.{Object}.setVisible(true);");
                    }
                    break;
                case EffectType.Hide:
                    if (semantics.HasFunctionByName(fun))
                    {
                        cb.Add($"{gName}.{obj}.setFunctionVisible(\"{fun}\", false);");
                    }
                    else
                    {
                        cb.Add($"{gName}.{Object}.setVisible(false);");
                    }
                    break;
                case EffectType.MoveTo:
                    cb.Add($"context.setLocation({gName}.{Object})");
                    break;
            }
        }

    }
}
