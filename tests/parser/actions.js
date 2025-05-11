const { Jison, RegExpLexer } = require("../setup");
const assert = require("assert");

describe("Parser Action Tests", () => {
    describe("Basic Semantic Action Tests", () => {
        it("should handle basic return actions", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"],
                    ["y", "return 'y';"]
                ]
            };
            const grammar = {
                bnf: {
                    "E": [["E x", "return 0"],
                          ["E y", "return 1"],
                          ""]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new RegExpLexer(lexData);

            assert.equal(parser.parse('x'), 0, "Should return 0 for x");
            assert.equal(parser.parse('y'), 1, "Should return 1 for y");
        });

        it("should handle null returns", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"]
                ]
            };
            const grammar = {
                bnf: {
                    "E": [["E x", "return null;"],
                          ""]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new RegExpLexer(lexData);

            assert.equal(parser.parse('x'), null, "Should return null");
        });

        it("should handle terminal semantic values", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"],
                    ["y", "return 'y';"]
                ]
            };
            const grammar = {
                bnf: {
                    "E": [["E x", "return [$2 === 'x']"],
                          ["E y", "return [$2]"],
                          ""]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new RegExpLexer(lexData);

            assert.deepEqual(parser.parse('x'), [true], "Should return [true] for x");
            assert.deepEqual(parser.parse('y'), ['y'], "Should return ['y'] for y");
        });
    });

    describe("Stack Lookup Tests", () => {
        it("should handle semantic action stack lookup", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"],
                    ["y", "return 'y';"]
                ]
            };
            const grammar = {
                bnf: {
                    "pgm": [["E", "return $1"]],
                    "E": [["B E", "return $1+$2"],
                          ["x", "$$ = 'EX'"]],
                    "B": [["y", "$$ = 'BY'"]]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new RegExpLexer(lexData);

            assert.equal(parser.parse('x'), "EX", "Should return EX for x");
            assert.equal(parser.parse('yx'), "BYEX", "Should return BYEX for yx");
        });

        it("should handle semantic actions on nullable grammar", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"]
                ]
            };
            const grammar = {
                bnf: {
                    "S": [["A", "return $1"]],
                    "A": [['x A', "$$ = $2+'x'"],
                          ['', "$$ = '->'"]]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new RegExpLexer(lexData);

            assert.equal(parser.parse('xx'), "->xx", "Should handle nullable grammar");
        });
    });

    describe("Named Semantic Value Tests", () => {
        it("should handle named semantic values", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"]
                ]
            };
            const grammar = {
                bnf: {
                    "S": [["A", "return $A"]],
                    "A": [['x A', "$$ = $A+'x'"],
                          ['', "$$ = '->'"]]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new RegExpLexer(lexData);

            assert.equal(parser.parse('xx'), "->xx", "Should handle named semantic values");
        });

        it("should handle ambiguous named semantic values", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"],
                    ["y", "return 'y';"]
                ]
            };
            const grammar = {
                operators: [["left", "y"]],
                bnf: {
                    "S": [["A", "return $A"]],
                    "A": [['A y A', "$$ = $A2+'y'+$A1"],
                          ['x', "$$ = 'x'"]]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new RegExpLexer(lexData);

            assert.equal(parser.parse('xyx'), "xyx", "Should handle ambiguous named values");
        });
    });

    describe("Special Value Tests", () => {
        it("should handle previous semantic value lookup", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"],
                    ["y", "return 'y';"]
                ]
            };
            const grammar = {
                bnf: {
                    "S": [["A B", "return $A + $B"]],
                    "A": [['A x', "$$ = $A+'x'"], ['x', "$$ = $1"]],
                    "B": [["y", "$$ = $0"]]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new RegExpLexer(lexData);

            assert.equal(parser.parse('xxy'), "xxxx", "Should handle previous value lookup");
        });

        it("should handle negative semantic value lookup", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"],
                    ["y", "return 'y';"],
                    ["z", "return 'z';"]
                ]
            };
            const grammar = {
                bnf: {
                    "S": [["G A B", "return $G + $A + $B"]],
                    "G": [['z', "$$ = $1"]],
                    "A": [['A x', "$$ = $A+'x'"], ['x', "$$ = $1"]],
                    "B": [["y", "$$ = $-1"]]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new RegExpLexer(lexData);

            assert.equal(parser.parse('zxy'), "zxz", "Should handle negative value lookup");
        });
    });

    describe("AST Building Tests", () => {
        it("should build AST correctly", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"]
                ]
            };
            const grammar = {
                bnf: {
                    "S": [['A', "return $1;"]],
                    "A": [['x A', "$2.push(['ID',{value:'x'}]); $$ = $2;"],
                          ['', "$$ = ['A',{}];"]]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new RegExpLexer(lexData);

            const expectedAST = ['A', {},
                ['ID', {value: 'x'}],
                ['ID', {value: 'x'}],
                ['ID', {value: 'x'}]];

            const result = parser.parse("xxx");
            assert.deepEqual(result, expectedAST, "Should build correct AST");
        });

        it("should handle 0+0 grammar", () => {
            const lexData = {
                rules: [
                    ["0", "return 'ZERO';"],
                    ["\\+", "return 'PLUS';"],
                    ["$", "return 'EOF';"]
                ]
            };
            const grammar = {
                bnf: {
                    "S": [["E EOF", "return $1"]],
                    "E": [["E PLUS T", "$$ = ['+',$1,$3]"],
                          ["T", "$$ = $1"]],
                    "T": [["ZERO", "$$ = [0]"]]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new RegExpLexer(lexData);

            const expectedAST = ["+", ["+", [0], [0]], [0]];
            assert.deepEqual(parser.parse("0+0+0"), expectedAST, "Should handle 0+0 grammar");
        });
    });

    describe("Special Action Tests", () => {
        it("should handle YYACCEPT", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"],
                    ["y", "return 'y';"]
                ]
            };
            const grammar = {
                bnf: {
                    "pgm": [["E", "return $1"]],
                    "E": [["B E", "return $1+$2"],
                          ["x", "$$ = 'EX'"]],
                    "B": [["y", "YYACCEPT"]]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new RegExpLexer(lexData);

            assert.equal(parser.parse('x'), "EX", "Should handle normal parsing");
            assert.equal(parser.parse('yx'), true, "Should handle YYACCEPT");
        });

        it("should handle YYABORT", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"],
                    ["y", "return 'y';"]
                ]
            };
            const grammar = {
                bnf: {
                    "pgm": [["E", "return $1"]],
                    "E": [["B E", "return $1+$2"],
                          ["x", "$$ = 'EX'"]],
                    "B": [["y", "YYABORT"]]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new RegExpLexer(lexData);

            assert.equal(parser.parse('x'), "EX", "Should handle normal parsing");
            assert.equal(parser.parse('yx'), false, "Should handle YYABORT");
        });
    });

    describe("Parser Parameter Tests", () => {
        it("should handle parse parameters", () => {
            const lexData = {
                rules: [
                    ["y", "return 'y';"]
                ]
            };
            const grammar = {
                bnf: {
                    "E": [["E y", "return first + second;"],
                          ""]
                },
                parseParams: ["first", "second"]
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new RegExpLexer(lexData);

            assert.equal(parser.parse('y', "foo", "bar"), "foobar", "Should handle parse parameters");
        });
    });

    describe("Symbol Alias Tests", () => {
        it("should handle symbol aliases", () => {
            const lexData = {
                rules: [
                    ["a", "return 'a';"],
                    ["b", "return 'b';"],
                    ["c", "return 'c';"]
                ]
            };
            const grammar = {
                bnf: {
                    "pgm": [["expr[alice] expr[bob] expr[carol]", "return $alice+$bob+$carol;"]],
                    "expr": [["a", "$$ = 'a';"],
                            ["b", "$$ = 'b';"],
                            ["c", "$$ = 'c';"]]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new RegExpLexer(lexData);
            assert.equal(parser.parse('abc'), "abc", "Should handle symbol aliases");
        });

        it("should handle symbol aliases in EBNF", () => {
            const lexData = {
                rules: [
                    ["a", "return 'a';"],
                    ["b", "return 'b';"],
                    ["c", "return 'c';"]
                ]
            };
            const grammar = {
                ebnf: {
                    "pgm": [["expr[alice] (expr[bob] expr[carol])+", "return $alice+$2;"]],
                    "expr": [["a", "$$ = 'a';"],
                            ["b", "$$ = 'b';"],
                            ["c", "$$ = 'c';"]]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new RegExpLexer(lexData);
            assert.equal(parser.parse('abc'), "ab", "Should handle aliases in EBNF");
        });
    });
});
