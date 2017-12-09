using System;
using Theseus.Elements.Enumerations;
using Theseus.Elements.JavaScriptUtils;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class SimpleCondition : IElement, ISemanticsValidator, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
        public ConditionMode Mode { get; }
        public string Object { get; }
        public bool Invert { get; }
        public string Host { get; }
        public ItemState State { get; }

        public SimpleCondition(ConditionMode mode, string obj, ItemState state, bool invert)
        {
            if (mode == ConditionMode.IsState || mode == ConditionMode.HasBeenState)
            {
                Mode = mode;
                Object = obj;
                Invert = invert;
                Host = "";
                State = state;
            }
            else
            {
                throw new Exception("Incorrect parameters for Condition");
            }
        }

        public SimpleCondition(ConditionMode mode, string obj, bool invert)
        {
            if (mode == ConditionMode.IsSet || mode == ConditionMode.HasBeenSet || mode == ConditionMode.IsHere ||
                mode == ConditionMode.IsCarried || mode == ConditionMode.HasBeenCarried)
            {
                Mode = mode;
                Object = obj;
                Invert = invert;
                Host = "";
                State = ItemState.None;
            }
            else
            {
                throw new Exception("Incorrect parameters for Condition");
            }
        }

        public SimpleCondition(ConditionMode mode, string obj, string host, bool invert)
        {
            if (mode == ConditionMode.IsIn)
            {
                Mode = mode;
                Object = obj;
                Invert = invert;
                Host = host;
                State = ItemState.None;
            }
            else
            {
                throw new Exception("Incorrect parameters for Condition");
            }
        }

        public void BuildSemantics(ISemantics semantics)
        {
        }

        public void CheckSemantics(ISemantics semantics)
        {
            if (Mode == ConditionMode.IsSet ||
                Mode == ConditionMode.HasBeenSet)
            {
                if (!semantics.HasFlagByName(Object))
                {
                    throw new Exception($"Unknown flag \"{Object}\"");
                }
            }
        }

        public string EmitTheseusCode(int indent = 0)
        {
            var notS = Invert ? "not " : "";
            var dontS = Invert ? "do not " : "";
            switch (Mode)
            {
                case ConditionMode.IsState:
                    return $"{Object} is {notS}{State.ToString().ToLower()}";
                case ConditionMode.IsSet:
                    return $"{Object} is {notS}set";
                case ConditionMode.IsIn:
                    return $"{Object} is {notS}in {Host}";
                case ConditionMode.IsHere:
                    return $"{Object} is {notS}here";
                case ConditionMode.IsCarried:
                    return $"I {dontS}carry {Object}";
                case ConditionMode.HasBeenState:
                    return $"{Object} has {notS}been {State.ToString().ToLower()}";
                case ConditionMode.HasBeenSet:
                    return $"{Object} has {notS}been set";
                default:
                    return $"I have {notS}carried {Object}";
            }
        }

        public void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb)
        {
            var gName = $"THESEUS.{GameUtils.GameName.ToUpper()}";
            var not = Invert ? "!" : "";
            switch (Mode)
            {
                case ConditionMode.IsState:
                    if (State == ItemState.Hidden)
                    {
                        var s = Invert ? $"{gName}.{Object}.isVisible()" : $"!{gName}.{Object}.isVisible()";
                        cb.Append(s);
                    }
                    else
                    {
                        cb.Append($"{not}{gName}.{Object}.is{State}()");
                    }
                    break;
                case ConditionMode.IsSet:
                    cb.Append($"{not}context.flags().has({gName}.Flag.{Object})");
                    break;
                case ConditionMode.IsIn:
                    cb.Append($"{not}{gName}.{Host}.containedItems.has({gName}.{Object})");
                    break;
                case ConditionMode.IsHere:
                    if (semantics.HasCharacterByName(Object))
                    {
                        cb.Append($"{not}context.location().characters.has({gName}.{Object})");
                    }
                    else
                    {
                        cb.Append($"{not}context.location.items.has({gName}.{Object})");
                    }
                    break;
                case ConditionMode.IsCarried:
                    cb.Append($"{not}context.inventory().has({gName}.{Object})");
                    break;
                case ConditionMode.HasBeenState:
                    cb.Append($"{not}{gName}.{Object}.hasBeen{State}()");
                    break;
                case ConditionMode.HasBeenSet:
                    cb.Append($"{not}context.historicFlags().has({gName}.Flag.{Object})");
                    break;
                default:
                    cb.Append($"{not}context.historicInventory().has({gName}.{Object})");
                    break;
            }
        }
    }
}
