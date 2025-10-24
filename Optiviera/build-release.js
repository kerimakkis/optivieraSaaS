const { execSync } = require('child_process');
const fs = require('fs');
const path = require('path');

const config = {
  projectPath: './Optiviera',
  electronPath: './electron',
  outputPath: './dist',
  platforms: {
    win: { runtime: 'win-x64', name: 'Windows x64' },
    winArm: { runtime: 'win-arm64', name: 'Windows ARM64' },
    mac: { runtime: 'osx-arm64', name: 'macOS ARM64' },
    macIntel: { runtime: 'osx-x64', name: 'macOS Intel' },
    linux: { runtime: 'linux-x64', name: 'Linux x64' },
    linuxArm: { runtime: 'linux-arm64', name: 'Linux ARM64' }
  }
};

function log(message) {
  console.log(`[BUILD] ${message}`);
}

function runCommand(command, cwd = process.cwd()) {
  log(`Running: ${command}`);
  try {
    execSync(command, { 
      cwd, 
      stdio: 'inherit',
      encoding: 'utf8'
    });
  } catch (error) {
    log(`Error running command: ${command}`);
    log(`Error: ${error.message}`);
    process.exit(1);
  }
}

function cleanDirectory(dir) {
  if (fs.existsSync(dir)) {
    log(`Cleaning directory: ${dir}`);
    fs.rmSync(dir, { recursive: true, force: true });
  }
  fs.mkdirSync(dir, { recursive: true });
}

function copyFile(src, dest) {
  const destDir = path.dirname(dest);
  if (!fs.existsSync(destDir)) {
    fs.mkdirSync(destDir, { recursive: true });
  }
  fs.copyFileSync(src, dest);
}

function copyDirectory(src, dest) {
  if (!fs.existsSync(src)) return;
  
  const entries = fs.readdirSync(src, { withFileTypes: true });
  
  for (const entry of entries) {
    const srcPath = path.join(src, entry.name);
    const destPath = path.join(dest, entry.name);
    
    if (entry.isDirectory()) {
      copyDirectory(srcPath, destPath);
    } else {
      copyFile(srcPath, destPath);
    }
  }
}

function buildAspNetCore(runtime) {
  log(`Building ASP.NET Core for ${runtime}...`);
  
  // Use absolute path to avoid nested dist folders
  const publishPath = path.resolve(config.outputPath, 'publish', runtime);
  cleanDirectory(publishPath);
  
  runCommand(
    `dotnet publish -c Release -r ${runtime} --self-contained true -o "${publishPath}"`,
    config.projectPath
  );
  
  return publishPath;
}

function buildElectron(platform) {
  log(`Building Electron for ${platform}...`);
  
  if (platform === 'all') {
    // Build for all platforms
    runCommand('npm run build:all', config.electronPath);
  } else {
    // Copy published ASP.NET Core app to electron resources for specific platform
    const platformConfig = config.platforms[platform];
    if (platformConfig) {
      const aspNetPath = path.resolve(config.outputPath, 'publish', platformConfig.runtime);
      const electronResourcesPath = path.resolve(config.electronPath, 'resources', 'app');
      
      log(`Copying ASP.NET app from: ${aspNetPath}`);
      log(`Copying ASP.NET app to: ${electronResourcesPath}`);
      
      cleanDirectory(electronResourcesPath);
      copyDirectory(aspNetPath, electronResourcesPath);
      
      // Verify copy
      const exeName = platformConfig.runtime.startsWith('win') ? 'Optiviera.exe' : 'Optiviera';
      const exePath = path.join(electronResourcesPath, exeName);
      if (fs.existsSync(exePath)) {
        log(`✓ ${exeName} copied successfully`);
      } else {
        log(`✗ ERROR: ${exeName} not found after copy!`);
        process.exit(1);
      }
    }
    
    // Build Electron app
    const buildCommand = `npm run build:${platform}`;
    runCommand(buildCommand, config.electronPath);
  }
}

function createInitialDatabase() {
  log('Creating initial database...');
  
  // Create a pre-seeded SQLite database with admin user
  const dbScriptPath = path.join(config.projectPath, 'Scripts', 'CreateInitialDatabase.cs');
  
  if (fs.existsSync(dbScriptPath)) {
    try {
      // Create database using the script
      runCommand(`dotnet run --project ${config.projectPath} -- --create-db`, config.projectPath);
      log('Database created successfully with admin user.');
    } catch (error) {
      log('Warning: Database creation failed. Manual setup may be required.');
      log(`Error: ${error.message}`);
    }
  } else {
    log('Warning: Database creation script not found. Manual database setup required.');
  }
}

function generateChecksums() {
  log('Generating checksums...');
  
  const distPath = path.join(config.electronPath, 'dist');
  if (!fs.existsSync(distPath)) return;
  
  const crypto = require('crypto');
  const checksums = {};
  
  function processDirectory(dir, relativePath = '') {
    const entries = fs.readdirSync(dir, { withFileTypes: true });
    
    for (const entry of entries) {
      const fullPath = path.join(dir, entry.name);
      const relativeFilePath = path.join(relativePath, entry.name);
      
      if (entry.isDirectory()) {
        processDirectory(fullPath, relativeFilePath);
      } else {
        const fileBuffer = fs.readFileSync(fullPath);
        const hash = crypto.createHash('sha256').update(fileBuffer).digest('hex');
        checksums[relativeFilePath] = hash;
      }
    }
  }
  
  processDirectory(distPath);
  
  const checksumPath = path.join(distPath, 'checksums.json');
  fs.writeFileSync(checksumPath, JSON.stringify(checksums, null, 2));
  
  log(`Checksums saved to: ${checksumPath}`);
}

function main() {
  const args = process.argv.slice(2);
  const platform = args[0] || 'all';
  
  log(`Starting build process for platform: ${platform}`);
  log(`Project path: ${config.projectPath}`);
  log(`Electron path: ${config.electronPath}`);
  log(`Output path: ${config.outputPath}`);
  
  // Clean output directory
  cleanDirectory(config.outputPath);
  
  if (platform === 'all') {
    // Build for all platforms
    for (const [platformKey, platformConfig] of Object.entries(config.platforms)) {
      log(`\n=== Building for ${platformConfig.name} ===`);
      buildAspNetCore(platformConfig.runtime);
    }
    
    // Build Electron for all platforms
    buildElectron('all');
  } else if (config.platforms[platform]) {
    // Build for specific platform
    const platformConfig = config.platforms[platform];
    log(`\n=== Building for ${platformConfig.name} ===`);
    
    buildAspNetCore(platformConfig.runtime);
    buildElectron(platform);
  } else {
    log(`Error: Unknown platform: ${platform}`);
    log(`Available platforms: ${Object.keys(config.platforms).join(', ')}, all`);
    process.exit(1);
  }
  
  // Create initial database
  createInitialDatabase();
  
  // Generate checksums
  generateChecksums();
  
  log('\n=== Build completed successfully! ===');
  log(`Output directory: ${path.join(config.electronPath, 'dist')}`);
}

// Handle command line arguments
if (require.main === module) {
  main();
}

module.exports = {
  buildAspNetCore,
  buildElectron,
  createInitialDatabase,
  generateChecksums,
  config
};
