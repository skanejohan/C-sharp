using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class CharacterOption : IElement, ITheseusCodeEmitter
    {
        public Enumerations.CharacterOption Option { get; }
        public string Ident { get; }
        public int StepsBehind { get; }

        public CharacterOption(Enumerations.CharacterOption option)
        {
            Option = option;
        }

        public CharacterOption(Enumerations.CharacterOption option, string ident)
        {
            Option = option;
            Ident = ident;
        }

        public CharacterOption(Enumerations.CharacterOption option, int stepsBehind)
        {
            Option = option;
            StepsBehind = stepsBehind;
        }

        public string EmitTheseusCode(int indent = 0)
        {
            switch (Option)
            {
                case Enumerations.CharacterOption.ConversationIs:
                    return $"conversation is {Ident}";
                case Enumerations.CharacterOption.FollowsPlayerBehind:
                    var step = StepsBehind == 1 ? "step" : "steps";
                    return $"follows player {StepsBehind} {step} behind";
                case Enumerations.CharacterOption.Hidden:
                    return "hidden";
                default:
                    return $"starts at {Ident}";
            }
        }

        public string EmitJavaScriptCode(ISemantics semantics, int indent = 0)
        {
            return ""; // Not used - code is emitted from Character
        }
    }
}
