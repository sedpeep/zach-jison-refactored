const { Jison, Lexer } = require("../setup");
const assert = require("assert");

describe("Error Handling Tests", () => {
    describe("Basic Error Tests", () => {
        it("should catch and handle errors", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"],
                    ["y", "return 'y';"],
                    [".", "return 'ERR';"]
                ]
            };
            const grammar = {
                bnf: {
                    "A": ['A x',
                          'A y',
                          ['A error', "return 'caught';"],
                          '']
                }
            };

            const parser = new Jison.Parser(grammar, {type: "lr0"});
            parser.lexer = new Lexer(lexData);
            assert.ok(parser.parse('xxy'), "Should parse valid input");
            assert.equal(parser.parse('xyg'), "caught", "Should return 'caught' on error");
        });

        it("should recover from errors", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"],
                    ["y", "return 'y';"],
                    [".", "return 'ERR';"]
                ]
            };
            const grammar = {
                bnf: {
                    "A": ['A x',
                          ['A y', "return 'recovery'"],
                          'A error',
                          '']
                }
            };

            const parser = new Jison.Parser(grammar, {type: "lr0"});
            parser.lexer = new Lexer(lexData);
            assert.equal(parser.parse('xxgy'), "recovery", "Should recover from error");
        });
    });

    describe("Nested Error Tests", () => {
        it("should handle deep error recovery", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"],
                    ["y", "return 'y';"],
                    ["g", "return 'g';"],
                    [";", "return ';';"],
                    [".", "return 'ERR';"]
                ]
            };
            const grammar = {
                bnf: {
                    "S": ['g A ;',
                          ['g error ;', 'return "nested"']],
                    "A": ['A x',
                          'x']
                }
            };

            const parser = new Jison.Parser(grammar, {type: "lr0"});
            parser.lexer = new Lexer(lexData);
            assert.ok(parser.parse('gxxx;'), "Should parse valid input");
            assert.equal(parser.parse('gxxg;'), "nested", "Should handle nested error");
        });

        it("should throw when no recovery is possible", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"],
                    ["y", "return 'y';"],
                    [".", "return 'ERR';"]
                ]
            };
            const grammar = {
                bnf: {
                    "A": ['A x',
                          ['A y', "return 'recovery'"],
                          '']
                }
            };

            const parser = new Jison.Parser(grammar, {type: "lr0"});
            parser.lexer = new Lexer(lexData);
            assert.throws(
                () => parser.parse('xxgy'),
                "Should throw when no recovery is possible"
            );
        });
    });

    describe("Complex Error Recovery Tests", () => {
        it("should handle error after error recovery", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"],
                    ["y", "return 'y';"],
                    ["g", "return 'g';"],
                    [".", "return 'ERR';"]
                ]
            };
            const grammar = {
                bnf: {
                    "S": ['g A y',
                          ['g error y', 'return "nested"']],
                    "A": ['A x',
                          'x']
                }
            };

            const parser = new Jison.Parser(grammar, {type: "lr0"});
            parser.lexer = new Lexer(lexData);
            assert.throws(
                () => parser.parse('gxxx;'),
                "Should throw on error after recovery"
            );
        });

        it("should throw error despite recovery rule", () => {
            const lexData = {
                rules: [
                    ["0", "return 'ZERO';"],
                    ["\\+", "return 'PLUS';"],
                    [";", "return ';';"],
                    [".", "return 'INVALID'"],
                    ["$", "return 'EOF';"]
                ]
            };
            const grammar = {
                bnf: {
                    "S": [["Exp EOF", "return $1"]],
                    "Exp": [["E ;", "$$ = $1;"],
                           ["E error", "$$ = $1;"]],
                    "E": [["E PLUS T", "$$ = ['+',$1,$3]"],
                          ["T", "$$ = $1"]],
                    "T": [["ZERO", "$$ = [0]"]]
                }
            };

            const parser = new Jison.Parser(grammar, {debug: true});
            parser.lexer = new Lexer(lexData);

            const expectedAST = ["+", ["+", [0], [0]], [0]];
            assert.throws(
                () => parser.parse("0+0+0>"),
                "Should throw despite recovery rule"
            );
        });
    });

    describe("AST Recovery Tests", () => {
        it("should maintain correct AST after error recovery", () => {
            const lexData = {
                rules: [
                    ["0", "return 'ZERO';"],
                    ["\\+", "return 'PLUS';"],
                    [";", "return ';';"],
                    ["$", "return 'EOF';"],
                    [".", "return 'INVALID';"]
                ]
            };
            const grammar = {
                bnf: {
                    "S": [["Exp EOF", "return $1"]],
                    "Exp": [["E ;", "$$ = $1;"],
                           ["E error", "$$ = $1;"]],
                    "E": [["E PLUS T", "$$ = ['+',$1,$3]"],
                          ["T", "$$ = $1"]],
                    "T": [["ZERO", "$$ = [0]"]]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new Lexer(lexData);

            const expectedAST = ["+", ["+", [0], [0]], [0]];
            assert.deepEqual(parser.parse("0+0+0"), expectedAST, "Should maintain correct AST");
        });

        it("should handle bison-style error recovery", () => {
            const lexData = {
                rules: [
                    ["0", "return 'ZERO';"],
                    ["\\+", "return 'PLUS';"],
                    [";", "return ';';"],
                    ["$", "return 'EOF';"],
                    [".", "return 'INVALID';"]
                ]
            };
            const grammar = {
                bnf: {
                    "S": [["stmt stmt EOF", "return $1"]],
                    "stmt": [["E ;", "$$ = $1;"],
                            ["error ;", "$$ = $1;"]],
                    "E": [["E PLUS T", "$$ = ['+',$1,$3]"],
                          ["T", "$$ = $1"]],
                    "T": [["ZERO", "$$ = [0]"]]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new Lexer(lexData);

            assert.ok(parser.parse("0+0++++>;0;"), "Should recover from multiple errors");
        });
    });
});
