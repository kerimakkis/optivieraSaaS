const crypto = require('crypto');
const fs = require('fs');
const path = require('path');

const distDir = path.join(__dirname, '..', 'dist');

function generateChecksum(filePath) {
  if (!fs.existsSync(filePath)) {
    return null;
  }
  const fileBuffer = fs.readFileSync(filePath);
  const hashSum = crypto.createHash('sha256');
  hashSum.update(fileBuffer);
  return hashSum.digest('hex');
}

function formatFileSize(bytes) {
  return (bytes / (1024 * 1024)).toFixed(2) + ' MB';
}

console.log('\n=== Generating Checksums ===\n');

const files = [
  'OptivieraERP.exe',
  'win-unpacked/OptivieraERP.exe'
];

const checksums = [];

files.forEach(file => {
  const filePath = path.join(distDir, file);
  const checksum = generateChecksum(filePath);

  if (checksum) {
    const stats = fs.statSync(filePath);
    const info = {
      file: file,
      sha256: checksum,
      size: formatFileSize(stats.size),
      bytes: stats.size
    };
    checksums.push(info);

    console.log(`File: ${file}`);
    console.log(`SHA256: ${checksum}`);
    console.log(`Size: ${info.size} (${info.bytes} bytes)`);
    console.log('');
  }
});

// Save to file
const checksumFile = path.join(distDir, 'checksums.json');
fs.writeFileSync(checksumFile, JSON.stringify(checksums, null, 2));
console.log(`Checksums saved to: ${checksumFile}`);
console.log('\n=== Done ===\n');
