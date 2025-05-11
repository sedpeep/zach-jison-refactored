const { Jison, Lexer } = require("../setup");
const assert = require("assert");

describe("SLR Parser Tests", () => {
    const lexData = {
        rules: [
            ["x", "return 'x';"],
            ["y", "return 'y';"]
        ]
    };

    describe("Nullable Grammar Tests", () => {
        it("should handle left-recursive nullable grammar correctly", () => {
            const grammar = {
                tokens: ['x'],
                startSymbol: "A",
                bnf: {
                    "A": ['A x',
                          '']
                }
            };

            const gen = new Jison.Generator(grammar, {type: "slr"});
            const parser = gen.createParser();
            parser.lexer = new Lexer(lexData);

            assert.ok(parser.parse('xxx'), "Should parse three x's");
            assert.ok(parser.parse("x"), "Should parse single x");
            assert.throws(
                () => parser.parse("y"),
                "Should throw parse error on invalid token"
            );
            assert.equal(gen.conflicts, 0, "Grammar should be conflict-free");
        });

        it("should handle right-recursive nullable grammar correctly", () => {
            const grammar = {
                tokens: ['x'],
                startSymbol: "A",
                bnf: {
                    "A": ['x A',
                          '']
                }
            };

            const gen = new Jison.Generator(grammar, {type: "slr"});
            const parser = gen.createParser();
            parser.lexer = new Lexer(lexData);

            assert.ok(parser.parse('xxx'), "Should parse three x's");
            assert.equal(gen.table.length, 4, "Parser table should have 4 states");
            assert.equal(gen.conflicts, 0, "Grammar should be conflict-free");
            assert.equal(gen.nullable('A'), true, "Nonterminal A should be nullable");
        });
    });
});
