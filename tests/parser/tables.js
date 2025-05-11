const { Jison } = require("../setup");
const assert = require("assert");

describe("Parser Table Tests", () => {
    describe("Nullable Grammar Tests", () => {
        it("should generate correct tables for right-recursive nullable grammar", () => {
            const grammar = {
                tokens: ['x'],
                startSymbol: "A",
                bnf: {
                    "A": ['x A',
                          '']
                }
            };

            const gen = new Jison.Generator(grammar, {type: "slr"});
            const gen2 = new Jison.Generator(grammar, {type: "lalr"});

            assert.equal(gen.table.length, 4, "Parser table should have 4 states");
            assert.equal(gen.nullable('A'), true, "Nonterminal A should be nullable");
            assert.equal(gen.conflicts, 0, "Grammar should be conflict-free");
            assert.deepEqual(gen.table, gen2.table, "SLR and LALR tables should be identical");
        });
    });

    describe("Table Comparison Tests", () => {
        it("should generate identical tables for SLR, LALR, and LR parsers", () => {
            const grammar = {
                tokens: ["ZERO", "PLUS"],
                startSymbol: "E",
                bnf: {
                    "E": ["E PLUS T",
                          "T"],
                    "T": ["ZERO"]
                }
            };

            const gen = new Jison.Generator(grammar, {type: "slr"});
            const gen2 = new Jison.Generator(grammar, {type: "lalr"});
            const gen3 = new Jison.Generator(grammar, {type: "lr"});

            assert.deepEqual(gen.table, gen2.table, "SLR and LALR tables should be identical");
            assert.deepEqual(gen2.table, gen3.table, "LALR and LR tables should be identical");
        });
    });

    describe("LL Parser Tests", () => {
        it("should generate correct LL parse table", () => {
            const grammar = {
                tokens: ['x'],
                startSymbol: "A",
                bnf: {
                    "A": ['x A',
                          '']
                }
            };

            const gen = new Jison.Generator(grammar, {type: "ll"});
            const expectedTable = {
                $accept: {x: [0], $end: [0]},
                A: {x: [1], $end: [2]}
            };

            assert.deepEqual(gen.table, expectedTable, "LL table should have correct structure");
        });

        it("should detect conflicts in LL grammar", () => {
            const grammar = {
                tokens: ['x'],
                startSymbol: "L",
                bnf: {
                    "L": ['T L T',
                          ''],
                    "T": ["x"]
                }
            };

            const gen = new Jison.Generator(grammar, {type: "ll"});
            assert.equal(gen.conflicts, 1, "Grammar should have one conflict");
        });
    });

    describe("Ambiguous Grammar Tests", () => {
        it("should detect conflicts in ambiguous grammar", () => {
            const grammar = {
                tokens: ['x', 'y'],
                startSymbol: "A",
                bnf: {
                    "A": ['A B A',
                          'x'],
                    "B": ['',
                          'y']
                }
            };

            const gen = new Jison.Generator(grammar, {type: "lr"});
            assert.equal(gen.conflicts, 2, "Ambiguous grammar should have two conflicts");
        });
    });
});
