const { Jison, RegExpLexer } = require("../setup");
const assert = require("assert");

describe("Parser Precedence Tests", () => {
    describe("Operator Associativity", () => {
        it("should handle left associative operators correctly", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"],
                    ["\\+", "return '+';"],
                    ["$", "return 'EOF';"]
                ]
            };
            const grammar = {
                tokens: ["x", "+", "EOF"],
                startSymbol: "S",
                operators: [
                    ["left", "+"]
                ],
                bnf: {
                    "S": [[ 'E EOF', "return $1;" ]],
                    "E": [[ "E + E", "$$ = ['+', $1, $3];" ],
                          [ "x", "$$ = ['x'];"]]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new RegExpLexer(lexData);

            const expectedAST = ["+", ["+", ["x"], ["x"]], ["x"]];
            const result = parser.parse("x+x+x");
            assert.deepEqual(result, expectedAST, "Left associative parsing should group operations from left to right");
        });

        it("should handle right associative operators correctly", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"],
                    ["\\+", "return '+';"],
                    ["$", "return 'EOF';"]
                ]
            };
            const grammar = {
                tokens: ["x", "+", "EOF"],
                startSymbol: "S",
                operators: [
                    ["right", "+"]
                ],
                bnf: {
                    "S": [[ "E EOF", "return $1;" ]],
                    "E": [[ "E + E", "$$ = ['+', $1, $3];" ],
                          [ "x", "$$ = ['x'];" ]]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new RegExpLexer(lexData);

            const expectedAST = ["+", ["x"], ["+", ["x"], ["x"]]];
            const result = parser.parse("x+x+x");
            assert.deepEqual(result, expectedAST, "Right associative parsing should group operations from right to left");
        });
    });

    describe("Multiple Precedence Levels", () => {
        it("should respect operator precedence with multiple operators", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"],
                    ["\\+", "return '+';"],
                    ["\\*", "return '*';"],
                    ["$", "return 'EOF';"]
                ]
            };
            const grammar = {
                tokens: ["x", "+", "*", "EOF"],
                startSymbol: "S",
                operators: [
                    ["left", "+"],
                    ["left", "*"]
                ],
                bnf: {
                    "S": [[ "E EOF", "return $1;" ]],
                    "E": [[ "E + E", "$$ = ['+', $1, $3];" ],
                          [ "E * E", "$$ = ['*', $1, $3];" ],
                          [ "x", "$$ = ['x'];" ]]
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new RegExpLexer(lexData);

            const expectedAST = ["+", ["*", ["x"], ["x"]], ["x"]];
            const result = parser.parse("x*x+x");
            assert.deepEqual(result, expectedAST, "Multiplication should have higher precedence than addition");
        });
    });

    describe("Non-associative Operators", () => {
        it("should throw error for non-associative operator used multiple times", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"],
                    ["=", "return '=';"],
                    ["$", "return 'EOF';"]
                ]
            };
            const grammar = {
                tokens: ["x", "=", "EOF"],
                startSymbol: "S",
                operators: [
                    ["nonassoc", "="]
                ],
                bnf: {
                    "S": ["E EOF"],
                    "E": ["E = E",
                          "x"]
                }
            };

            const parser = new Jison.Parser(grammar, {type: "lalr"});
            parser.lexer = new RegExpLexer(lexData);

            assert.throws(
                () => parser.parse("x=x=x"),
                "Should throw parse error when non-associative operator is used multiple times"
            );
            assert.ok(parser.parse("x=x"), "Single use of non-associative operator should be valid");
        });
    });

    describe("Context-dependent Precedence", () => {
        it("should handle unary operators with correct precedence", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"],
                    ["-", "return '-';"],
                    ["\\+", "return '+';"],
                    ["\\*", "return '*';"],
                    ["$", "return 'EOF';"]
                ]
            };
            const grammar = {
                tokens: ["x", "-", "+", "*", "EOF"],
                startSymbol: "S",
                operators: [
                    ["left", "-", "+"],
                    ["left", "*"],
                    ["left", "UMINUS"]
                ],
                bnf: {
                    "S": [[ "E EOF", "return $1;" ]],
                    "E": [[ "E - E", "$$ = [$1,'-', $3];" ],
                          [ "E + E", "$$ = [$1,'+', $3];" ],
                          [ "E * E", "$$ = [$1,'*', $3];" ],
                          [ "- E", "$$ = ['#', $2];", {prec: "UMINUS"} ],
                          [ "x", "$$ = ['x'];" ]]
                }
            };

            const parser = new Jison.Parser(grammar, {type: "slr"});
            parser.lexer = new RegExpLexer(lexData);

            const expectedAST = [[[["#", ["x"]], "*", ["#", ["x"]]], "*", ["x"]], "-", ["x"]];
            const result = parser.parse("-x*-x*x-x");
            assert.deepEqual(result, expectedAST, "Unary operators should have correct precedence in complex expressions");
        });
    });

    describe("Multi-operator Rules", () => {
        it("should handle complex operator combinations without conflicts", () => {
            const lexData = {
                rules: [
                    ["x", "return 'ID';"],
                    ["\\.", "return 'DOT';"],
                    ["=", "return 'ASSIGN';"],
                    ["\\(", "return 'LPAREN';"],
                    ["\\)", "return 'RPAREN';"],
                    ["$", "return 'EOF';"]
                ]
            };
            const grammar = {
                tokens: "ID DOT ASSIGN LPAREN RPAREN EOF",
                startSymbol: "S",
                operators: [
                    ["right", "ASSIGN"],
                    ["left", "DOT"]
                ],
                bnf: {
                    "S": [[ "e EOF", "return $1;" ]],
                    "id": [[ "ID", "$$ = ['ID'];"]],
                    "e": [[ "e DOT id", "$$ = [$1,'-', $3];" ],
                          [ "e DOT id ASSIGN e", "$$ = [$1,'=', $3];" ],
                          [ "e DOT id LPAREN e RPAREN", "$$ = [$1,'+', $3];" ],
                          [ "id ASSIGN e", "$$ = [$1,'+', $3];" ],
                          [ "id LPAREN e RPAREN", "$$ = [$1,'+', $3];" ],
                          [ "id", "$$ = $1;" ]]
                }
            };

            const gen = new Jison.Generator(grammar, {type: 'slr'});
            assert.equal(gen.conflicts, 0, "Grammar should be conflict-free");
        });
    });
});
