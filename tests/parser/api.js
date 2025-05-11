const { Jison, Lexer } = require("../setup");
const assert = require("assert");

describe("Parser API Tests", () => {
    const lexData = {
        rules: [
            ["x", "return 'x';"],
            ["y", "return 'y';"]
        ]
    };

    describe("Grammar Definition Tests", () => {
        it("should handle tokens as a string", () => {
            const grammar = {
                tokens: "x y",
                startSymbol: "A",
                bnf: {
                    "A": ['A x',
                          'A y',
                          '']
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new Lexer(lexData);
            assert.ok(parser.parse('xyx'), "Should parse xyx");
        });

        it("should handle generator creation", () => {
            const grammar = {
                bnf: {
                    "A": ['A x',
                          'A y',
                          '']
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new Lexer(lexData);
            assert.ok(parser.parse('xyx'), "Should parse xyx");
        });

        it("should handle extra spaces in productions", () => {
            const grammar = {
                tokens: "x y",
                startSymbol: "A",
                bnf: {
                    "A": ['A x ',
                          'A y',
                          '']
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new Lexer(lexData);
            assert.ok(parser.parse('xyx'), "Should parse xyx");
        });

        it("should handle | separated rules", () => {
            const grammar = {
                tokens: "x y",
                startSymbol: "A",
                bnf: {
                    "A": "A x | A y | "
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new Lexer(lexData);
            assert.ok(parser.parse('xyx'), "Should parse xyx");
        });
    });

    describe("Grammar Configuration Tests", () => {
        it("should handle optional start symbol", () => {
            const grammar = {
                tokens: "x y",
                bnf: {
                    "A": "A x | A y | "
                }
            };

            const parser = new Jison.Parser(grammar);
            assert.ok(true, "Should not throw error");
        });

        it("should require nonterminal start symbol", () => {
            const grammar = {
                tokens: "x y",
                startSymbol: "x",
                bnf: {
                    "A": "A x | A y | "
                }
            };

            assert.throws(
                () => new Jison.Generator(grammar),
                "Should throw error for terminal start symbol"
            );
        });

        it("should handle token list as string", () => {
            const grammar = {
                tokens: "x y",
                startSymbol: "A",
                bnf: {
                    "A": "A x | A y | "
                }
            };

            const gen = new Jison.Generator(grammar);
            assert.ok(gen.terminals.indexOf('x') >= 0, "Should include x in terminals");
        });
    });

    describe("Grammar Options Tests", () => {
        it("should handle grammar options", () => {
            const grammar = {
                options: {type: "slr"},
                tokens: "x y",
                startSymbol: "A",
                bnf: {
                    "A": ['A x',
                          'A y',
                          '']
                }
            };

            const gen = new Jison.Generator(grammar);
            assert.ok(gen, "Should create generator");
        });

        it("should allow overwriting grammar options", () => {
            const grammar = {
                options: {type: "slr"},
                tokens: "x y",
                startSymbol: "A",
                bnf: {
                    "A": ['A x',
                          'A y',
                          '']
                }
            };

            const gen = new Jison.Generator(grammar, {type: "lr0"});
            assert.equal(gen.constructor, Jison.LR0Generator, "Should use LR0 generator");
        });
    });

    describe("Shared Scope Tests", () => {
        it("should handle shared yy scope", () => {
            const lexData = {
                rules: [
                    ["x", "return 'x';"],
                    ["y", "return yy.xed ? 'yfoo' : 'ybar';"]
                ]
            };
            const grammar = {
                tokens: "x yfoo ybar",
                startSymbol: "A",
                bnf: {
                    "A": [['A x', "yy.xed = true;"],
                          ['A yfoo', "return 'foo';"],
                          ['A ybar', "return 'bar';"],
                          '']
                }
            };

            const parser = new Jison.Parser(grammar, {type: "lr0"});
            parser.lexer = new Lexer(lexData);
            assert.equal(parser.parse('y'), "bar", "Should return bar for single y");
            assert.equal(parser.parse('xxy'), "foo", "Should return foo after x's");
        });
    });

    describe("Error Handling Tests", () => {
        it("should handle custom parse error method", () => {
            const lexData = {
                rules: [
                    ["a", "return 'a';"],
                    ["b", "return 'b';"],
                    ["c", "return 'c';"],
                    ["d", "return 'd';"],
                    ["g", "return 'g';"]
                ]
            };
            const grammar = {
                tokens: "a b c d g",
                startSymbol: "S",
                bnf: {
                    "S": ["a g d",
                          "a A c",
                          "b A d",
                          "b g c"],
                    "A": ["B"],
                    "B": ["g"]
                }
            };

            const parser = new Jison.Parser(grammar, {type: "lalr"});
            parser.lexer = new Lexer(lexData);
            const result = {};
            parser.yy.parseError = function(str, hash) {
                Object.assign(result, hash);
                throw str;
            };

            assert.throws(() => parser.parse("aga"));
            assert.strictEqual(result.text, "a", "Parse error text should equal a");
            assert.strictEqual(typeof result.token, 'string', "Parse error token should be a string");
            assert.strictEqual(result.line, 0, "Hash should include line number");
        });

        it("should handle EOF in error message", () => {
            const grammar = {
                bnf: {
                    "A": ['x x y']
                }
            };

            const parser = new Jison.Parser(grammar);
            parser.lexer = new Lexer(lexData);
            parser.lexer.showPosition = null;
            parser.yy.parseError = function(str) {
                assert.ok(str.match("end of input"), "Should mention end of input");
            };

            assert.throws(() => parser.parse("xx"));
        });
    });

    describe("Location Tests", () => {
        it("should handle locations in actions", () => {
            const grammar = {
                tokens: ['x', 'y'],
                startSymbol: "A",
                bnf: {
                    "A": ['x A',
                          ['y', 'return @1'],
                          '']
                }
            };

            const lexData = {
                rules: [
                    ["\\s", "/*ignore*/"],
                    ["x", "return 'x';"],
                    ["y", "return 'y';"]
                ]
            };
            const gen = new Jison.Generator(grammar);
            const parser = gen.createParser();
            parser.lexer = new Lexer(lexData);
            const loc = parser.parse('xx\nxy');

            assert.equal(loc.first_line, 2, "First line should be 2");
            assert.equal(loc.last_line, 2, "Last line should be 2");
            assert.equal(loc.first_column, 1, "First column should be 1");
            assert.equal(loc.last_column, 2, "Last column should be 2");
        });

        it("should handle default location action", () => {
            const grammar = {
                tokens: ['x', 'y'],
                startSymbol: "A",
                bnf: {
                    "A": ['x A',
                          ['y', 'return @$'],
                          '']
                }
            };

            const lexData = {
                rules: [
                    ["\\s", "/*ignore*/"],
                    ["x", "return 'x';"],
                    ["y", "return 'y';"]
                ]
            };
            const gen = new Jison.Generator(grammar);
            const parser = gen.createParser();
            parser.lexer = new Lexer(lexData);
            const loc = parser.parse('xx\nxy');

            assert.equal(loc.first_line, 2, "First line should be 2");
            assert.equal(loc.last_line, 2, "Last line should be 2");
            assert.equal(loc.first_column, 1, "First column should be 1");
            assert.equal(loc.last_column, 2, "Last column should be 2");
        });
    });

    describe("Parser Instance Tests", () => {
        it("should handle instance creation", () => {
            const grammar = {
                tokens: ['x', 'y'],
                startSymbol: "A",
                bnf: {
                    "A": ['x A',
                          ['B', 'return @B'],
                          ''],
                    "B": ['y']
                }
            };

            const gen = new Jison.Generator(grammar);
            const parser = gen.createParser();
            parser.lexer = {
                toks: ['x', 'x', 'x', 'y'],
                lex: function() {
                    return this.toks.shift();
                },
                setInput: function() {}
            };
            const parser2 = new parser.Parser();
            parser2.lexer = parser.lexer;
            parser2.parse('xx\nxy');

            parser.blah = true;
            assert.notEqual(parser.blah, parser2.blah, "Should not inherit properties");
        });

        it("should handle reentrant parsing", () => {
            const grammar = {
                bnf: {
                    "S": ['A EOF'],
                    "A": ['x A',
                          'B',
                          'C'],
                    "B": [['y', 'return "foo";']],
                    "C": [['w', 'return yy.parser.parse("xxxy") + "bar";']]
                }
            };

            const lexData = {
                rules: [
                    ["\\s", "/*ignore*/"],
                    ["w", "return 'w';"],
                    ["x", "return 'x';"],
                    ["y", "return 'y';"],
                    ["$", "return 'EOF';"]
                ]
            };
            const gen = new Jison.Generator(grammar);
            const parser = gen.createParser();
            parser.lexer = new Lexer(lexData);
            const result = parser.parse('xxw');
            assert.equal(result, "foobar", "Should handle reentrant parsing");
        });
    });
});

