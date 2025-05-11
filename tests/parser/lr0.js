const { Jison, Lexer } = require("../setup");
const assert = require("assert");

describe("LR(0) Parser Tests", () => {
    const lexData = {
        rules: [
            ["x", "return 'x';"],
            ["y", "return 'y';"]
        ]
    };

    describe("Nullable Grammar Tests", () => {
        it("should handle left-recursive nullable grammar", () => {
            const grammar = {
                tokens: ['x'],
                startSymbol: "A",
                bnf: {
                    "A": ['A x',
                          '']
                }
            };

            const parser = new Jison.Parser(grammar, {type: "lr0"});
            parser.lexer = new Lexer(lexData);

            assert.ok(parser.parse('xxx'), "Should parse three x's");
            assert.ok(parser.parse("x"), "Should parse single x");
            assert.throws(
                () => parser.parse("y"),
                "Should throw parse error on invalid token"
            );
        });

        it("should handle right-recursive nullable grammar", () => {
            const grammar = {
                tokens: ['x'],
                startSymbol: "A",
                bnf: {
                    "A": ['x A',
                          '']
                }
            };

            const gen = new Jison.Generator(grammar, {type: "lr0"});

            assert.equal(gen.table.length, 4, "Parser table should have 4 states");
            assert.equal(gen.conflicts, 2, "Grammar should have 2 conflicts");
        });
    });

    describe("Basic Grammar Tests", () => {
        it("should parse 0+0 grammar", () => {
            const lexData = {
                rules: [
                    ["0", "return 'ZERO';"],
                    ["\\+", "return 'PLUS';"]
                ]
            };
            const grammar = {
                tokens: ["ZERO", "PLUS"],
                startSymbol: "E",
                bnf: {
                    "E": ["E PLUS T",
                          "T"],
                    "T": ["ZERO"]
                }
            };

            const parser = new Jison.Parser(grammar, {type: "lr0"});
            parser.lexer = new Lexer(lexData);

            assert.ok(parser.parse("0+0+0"), "Should parse multiple additions");
            assert.ok(parser.parse("0"), "Should parse single zero");
            assert.throws(
                () => parser.parse("+"),
                "Should throw parse error on invalid input"
            );
        });
    });
});
