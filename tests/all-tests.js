// Import all test files
require("./parser/parser-tests");

// If running directly, use Mocha
if (require.main === module) {
    require("mocha/bin/mocha");
}
