using System;
using Theseus.Elements.Enumerations;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class SimpleCondition : IElement, ITheseusCodeEmitter, IJavaScriptCodeEmitter
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

        public string EmitTheseusCode(int indent = 0)
        {
            var notS = Invert ? "not " : "";
            //var ntS = Invert ? "n't" : "";
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

        public string EmitJavaScriptCode(int indent = 0)
        {
            var not = Invert ? "!" : "";
            switch (Mode)
            {
                case ConditionMode.IsState:
                    return $"TODO";
                case ConditionMode.IsSet:
                    return $"{not}game.flags.isSet(Flag.{Object})";
                case ConditionMode.IsIn:
                    return $"TODO";
                case ConditionMode.IsHere:
                    return $"TODO";
                case ConditionMode.IsCarried:
                    return $"TODO";
                case ConditionMode.HasBeenState:
                    return $"TODO";
                case ConditionMode.HasBeenSet:
                    return $"TODO";
                default:
                    return $"TODO";
            }
        }
    }
}
