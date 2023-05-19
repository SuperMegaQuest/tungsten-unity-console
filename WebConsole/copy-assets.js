const fs = require('fs-extra');

fs.copySync('source/index.html', 'dist/index.html');
fs.copySync('source/style.css', 'dist/style.css');
