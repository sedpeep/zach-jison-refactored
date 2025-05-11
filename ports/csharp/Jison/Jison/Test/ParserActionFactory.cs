using System;

namespace jQuerySheet
{
    public static class ParserActionFactory
    {
        public static IParserAction CreateShiftAction(int nextState)
        {
            return new ParserAction((int)ParserActionType.Shift, nextState);
        }

        public static IParserAction CreateReduceAction(int production)
        {
            return new ParserAction((int)ParserActionType.Reduce, production);
        }

        public static IParserAction CreateAcceptAction()
        {
            return new ParserAction((int)ParserActionType.Accept, 0);
        }

        public static IParserAction CreateAction(ParserActionType type, int nextState)
        {
            return new ParserAction((int)type, nextState);
        }
    }
} 