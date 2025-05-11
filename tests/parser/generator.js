const { Jison, Lexer } = require("../setup");
const assert = require("assert");
const fs = require('fs');
const path = require('path');

describe("Parser Generator Tests", () => {
    const baseGrammar = {
        tokens: "x y",
        startSymbol: "A",
        bnf: {
            "A": ['A x',
                  'A y',
                  '']
        }
    };

    const baseLexData = {
        rules: [
            ["x", "return 'x';"],
            ["y", "return 'y';"]
        ]
    };

    describe("Module Generation Tests", () => {
        it("should generate valid AMD module", () => {
            const input = "xyxxxy";
            const gen = new Jison.Generator(baseGrammar);
            gen.lexer = new Lexer(baseLexData);

            const parserSource = gen.generateAMDModule();
            let parser = null;
            const define = (callback) => {
                parser = callback();
            };
            eval(parserSource);

            assert.ok(parser.parse(input), "AMD module should parse input correctly");
        });

        it("should generate valid CommonJS module", () => {
            const input = "xyxxxy";
            const gen = new Jison.Generator(baseGrammar);
            gen.lexer = new Lexer(baseLexData);

            const parserSource = gen.generateCommonJSModule();
            const exports = {};
            eval(parserSource);

            assert.ok(exports.parse(input), "CommonJS module should parse input correctly");
        });

        it("should generate valid default module", () => {
            const input = "xyxxxy";
            const gen = new Jison.Generator(baseGrammar);
            gen.lexer = new Lexer(baseLexData);

            const parserSource = gen.generateModule();
            eval(parserSource);

            assert.ok(parser.parse(input), "Default module should parse input correctly");
        });

        it("should generate module with custom name", () => {
            const input = "xyxxxy";
            const gen = new Jison.Generator(baseGrammar);
            gen.lexer = new Lexer(baseLexData);

            const parserSource = gen.generate({moduleType: "js", moduleName: "parsey"});
            eval(parserSource);

            assert.ok(parsey.parse(input), "Custom named module should parse input correctly");
        });

        it("should generate namespaced module", () => {
            const input = "xyxxxy";
            const compiler = {};
            const gen = new Jison.Generator(baseGrammar);
            gen.lexer = new Lexer(baseLexData);

            const parserSource = gen.generateModule({moduleName: "compiler.parser"});
            eval(parserSource);

            assert.ok(compiler.parser.parse(input), "Namespaced module should parse input correctly");
        });
    });

    describe("JSON Grammar Tests", () => {
        it("should parse JSON grammar", () => {
            const jsonGrammar = {
                "comment": "ECMA-262 5th Edition, 15.12.1 The JSON Grammar. (Incomplete implementation)",
                "author": "Zach Carter",
                "lex": {
                    "macros": {
                        "digit": "[0-9]",
                        "exp": "([eE][-+]?{digit}+)"
                    },
                    "rules": [
                        ["\\s+", "/* skip whitespace */"],
                        ["-?{digit}+(\\.{digit}+)?{exp}?", "return 'NUMBER';"],
                        ["\"[^\"]*\"", "yytext = yytext.substr(1,yyleng-2); return 'STRING';"],
                        ["\\{", "return '{'"],
                        ["\\}", "return '}'"],
                        ["\\[", "return '['"],
                        ["\\]", "return ']'"],
                        [",", "return ','"],
                        [":", "return ':'"],
                        ["true\\b", "return 'TRUE'"],
                        ["false\\b", "return 'FALSE'"],
                        ["null\\b", "return 'NULL'"]
                    ]
                },
                "tokens": "STRING NUMBER { } [ ] , : TRUE FALSE NULL",
                "start": "JSONText",
                "bnf": {
                    "JSONString": ["STRING"],
                    "JSONNumber": ["NUMBER"],
                    "JSONBooleanLiteral": ["TRUE", "FALSE"],
                    "JSONText": ["JSONValue"],
                    "JSONValue": [
                        "JSONNullLiteral",
                        "JSONBooleanLiteral",
                        "JSONString",
                        "JSONNumber",
                        "JSONObject",
                        "JSONArray"
                    ],
                    "JSONObject": [
                        "{ }",
                        "{ JSONMemberList }"
                    ],
                    "JSONMember": ["JSONString : JSONValue"],
                    "JSONMemberList": [
                        "JSONMember",
                        "JSONMemberList , JSONMember"
                    ],
                    "JSONArray": [
                        "[ ]",
                        "[ JSONElementList ]"
                    ],
                    "JSONElementList": [
                        "JSONValue",
                        "JSONElementList , JSONValue"
                    ]
                }
            };

            const gen = new Jison.Generator(jsonGrammar);
            const parserSource = gen.generateModule();
            eval(parserSource);

            assert.ok(parser.parse(JSON.stringify(jsonGrammar.bnf)), "JSON grammar should parse its own BNF");
        });
    });

    describe("Module Include Tests", () => {
        it("should include custom code in generated module", () => {
            const lexData = {
                rules: [
                    ["y", "return 'y';"]
                ]
            };
            const grammar = {
                bnf: {
                    "E": [["E y", "return test();"],
                          ""]
                },
                moduleInclude: "function test(val) { return 1; }"
            };

            const gen = new Jison.Generator(grammar);
            gen.lexer = new Lexer(lexData);

            const parserSource = gen.generateCommonJSModule();
            const exports = {};
            eval(parserSource);

            assert.ok(exports.parse("y"), "Module should include and use custom code");
        });
    });
});
