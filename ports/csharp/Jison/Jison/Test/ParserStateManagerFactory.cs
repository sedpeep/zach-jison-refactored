using System;

namespace jQuerySheet
{
    public static class ParserStateManagerFactory
    {
        public static ParserStateManager CreateStateManager(IParser parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException(nameof(parser));
            }

            return new ParserStateManager(parser);
        }

        public static ParserStateManager CreateStateManager(IParser parser, Dictionary<int, IParserState> states)
        {
            if (parser == null)
            {
                throw new ArgumentNullException(nameof(parser));
            }
            if (states == null)
            {
                throw new ArgumentNullException(nameof(states));
            }

            var manager = new ParserStateManager(parser);
            foreach (var state in states)
            {
                manager.AddState(state.Key, state.Value);
            }
            return manager;
        }

        public static ParserStateManager CreateDefaultStateManager(IParser parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException(nameof(parser));
            }

            var states = new Dictionary<int, IParserState>
            {
                { 1, new ShiftState() },
                { 2, new ReduceState(parser, 0) },
                { 3, new AcceptState() }
            };

            return CreateStateManager(parser, states);
        }
    }
} 