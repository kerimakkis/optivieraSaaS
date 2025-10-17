const { app, BrowserWindow, Menu, Tray, shell, ipcMain, dialog } = require('electron');
const path = require('path');
const { spawn } = require('child_process');
const fs = require('fs');
const os = require('os');

let mainWindow;
let splashWindow;
let tray;
let aspNetProcess;
let isQuitting = false;

// Configuration
const CONFIG = {
  appName: 'Optiviera ERP',
  port: 0, // Will be set dynamically
  minWidth: 1200,
  minHeight: 800,
  defaultWidth: 1400,
  defaultHeight: 900
};

function createSplashScreen() {
  splashWindow = new BrowserWindow({
    width: 500,
    height: 400,
    transparent: true,
    frame: false,
    alwaysOnTop: true,
    skipTaskbar: true,
    webPreferences: {
      nodeIntegration: false
    }
  });

  // Get the logo path and convert to base64
  const logoPath = path.join(__dirname, 'icon.png');
  let logoBase64 = '';
  
  try {
    const logoBuffer = fs.readFileSync(logoPath);
    logoBase64 = `data:image/png;base64,${logoBuffer.toString('base64')}`;
  } catch (error) {
    console.error('Could not read logo:', error);
  }

  // Create splash screen with rotating Optiviera logo (same as in app)
  const splashHTML = `
    <!DOCTYPE html>
    <html>
    <head>
      <style>
        body {
          margin: 0;
          padding: 0;
          display: flex;
          justify-content: center;
          align-items: center;
          height: 100vh;
          background: linear-gradient(135deg, #2c3e50 0%, #34495e 100%);
          font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
          overflow: hidden;
        }
        .splash-content {
          text-align: center;
          color: white;
          display: flex;
          flex-direction: column;
          align-items: center;
        }
        .optiviera-spinner {
          width: 150px;
          height: 150px;
          background-image: url('${logoBase64}');
          background-size: contain;
          background-repeat: no-repeat;
          background-position: center;
          animation: optiviera-spin 2s linear infinite;
          border-radius: 50%;
          box-shadow: 0 0 40px rgba(26, 188, 156, 0.8), 0 0 80px rgba(26, 188, 156, 0.5);
          margin-bottom: 30px;
        }
        @keyframes optiviera-spin {
          0% { transform: rotate(0deg); }
          100% { transform: rotate(360deg); }
        }
        h1 {
          font-size: 36px;
          margin: 0 0 10px 0;
          font-weight: 600;
          letter-spacing: 1px;
          text-shadow: 0 2px 10px rgba(0, 0, 0, 0.5);
        }
        .subtitle {
          font-size: 16px;
          opacity: 0.9;
          margin-bottom: 30px;
          font-weight: 300;
        }
        .loading-dots {
          display: flex;
          gap: 10px;
        }
        .loading-dot {
          width: 12px;
          height: 12px;
          border-radius: 50%;
          background-color: #1abc9c;
          animation: loading-pulse 1.4s infinite ease-in-out both;
          box-shadow: 0 0 10px rgba(26, 188, 156, 0.8);
        }
        .loading-dot:nth-child(1) { animation-delay: -0.32s; }
        .loading-dot:nth-child(2) { animation-delay: -0.16s; }
        .loading-dot:nth-child(3) { animation-delay: 0s; }
        @keyframes loading-pulse {
          0%, 80%, 100% { 
            transform: scale(0.6); 
            opacity: 0.5; 
          }
          40% { 
            transform: scale(1); 
            opacity: 1; 
          }
        }
      </style>
    </head>
    <body>
      <div class="splash-content">
        <div class="optiviera-spinner"></div>
        <h1>Optiviera ERP</h1>
        <p class="subtitle">Öko-Level ERP für kleine Unternehmen</p>
        <div class="loading-dots">
          <div class="loading-dot"></div>
          <div class="loading-dot"></div>
          <div class="loading-dot"></div>
        </div>
      </div>
    </body>
    </html>
  `;

  splashWindow.loadURL(`data:text/html;charset=utf-8,${encodeURIComponent(splashHTML)}`);
}

function createWindow() {
  // Create the browser window
  mainWindow = new BrowserWindow({
    width: CONFIG.defaultWidth,
    height: CONFIG.defaultHeight,
    minWidth: CONFIG.minWidth,
    minHeight: CONFIG.minHeight,
    icon: getIconPath(),
    webPreferences: {
      nodeIntegration: false,
      contextIsolation: true,
      preload: path.join(__dirname, 'preload.js'),
      webSecurity: true
    },
    show: false, // Don't show until ready
    titleBarStyle: process.platform === 'darwin' ? 'hiddenInset' : 'default'
  });

  // Set window title
  mainWindow.setTitle(CONFIG.appName);

  // Create system tray
  createTray();

  // Start ASP.NET Core process
  startAspNetProcess();

  // Show window when ready
  mainWindow.once('ready-to-show', () => {
    mainWindow.show();
    
    // Focus on the window
    if (mainWindow) {
      mainWindow.focus();
    }
    
    // Close splash screen
    if (splashWindow && !splashWindow.isDestroyed()) {
      splashWindow.close();
      splashWindow = null;
    }
  });

  // Handle window closed
  mainWindow.on('close', (event) => {
    if (!isQuitting) {
      event.preventDefault();
      mainWindow.hide();
      
      // Show notification on first minimize
      if (process.platform === 'win32') {
        mainWindow.setSkipTaskbar(false);
      }
    }
  });

  // Handle window closed
  mainWindow.on('closed', () => {
    mainWindow = null;
  });

  // Handle external links
  mainWindow.webContents.setWindowOpenHandler(({ url }) => {
    shell.openExternal(url);
    return { action: 'deny' };
  });
}

function createTray() {
  const iconPath = getIconPath();
  
  tray = new Tray(iconPath);
  
  const contextMenu = Menu.buildFromTemplate([
    {
      label: 'Show Optiviera',
      click: () => {
        if (mainWindow) {
          mainWindow.show();
          mainWindow.focus();
        }
      }
    },
    {
      label: 'Hide Optiviera',
      click: () => {
        if (mainWindow) {
          mainWindow.hide();
        }
      }
    },
    { type: 'separator' },
    {
      label: 'Quit',
      click: () => {
        isQuitting = true;
        app.quit();
      }
    }
  ]);

  tray.setContextMenu(contextMenu);
  tray.setToolTip(CONFIG.appName);
  
  // Double click to show window
  tray.on('double-click', () => {
    if (mainWindow) {
      mainWindow.show();
      mainWindow.focus();
    }
  });
}

function startAspNetProcess() {
  // In production, files are in app.asar, so we need to extract them first
  const isPackaged = app.isPackaged;
  let appPath;
  
  if (isPackaged) {
    // Extract files from app.asar to a temporary directory
    const tempDir = path.join(os.tmpdir(), 'optiviera-temp');
    if (!fs.existsSync(tempDir)) {
      fs.mkdirSync(tempDir, { recursive: true });
    }
    appPath = tempDir;
    
    // Copy files from app.asar to temp directory
    const asarPath = path.join(process.resourcesPath, 'app.asar');
    if (fs.existsSync(asarPath)) {
      // Extract all files from asar
      const asar = require('asar');
      asar.extractAll(asarPath, appPath);
      
      // Move files from resources/app to root of temp directory
      const resourcesAppPath = path.join(appPath, 'resources', 'app');
      if (fs.existsSync(resourcesAppPath)) {
        // Copy all files from resources/app to temp directory root
        const files = fs.readdirSync(resourcesAppPath);
        files.forEach(file => {
          const srcPath = path.join(resourcesAppPath, file);
          const destPath = path.join(appPath, file);
          if (fs.statSync(srcPath).isDirectory()) {
            // Copy directory recursively
            fs.cpSync(srcPath, destPath, { recursive: true });
          } else {
            // Copy file
            fs.copyFileSync(srcPath, destPath);
          }
        });
      }
    }
  } else {
    appPath = path.join(__dirname, 'resources', 'app');
  }
  
  const exeName = process.platform === 'win32' ? 'Optiviera.exe' : 'Optiviera';
  const exePath = path.join(appPath, exeName);
  
  console.log('Checking exe path:', exePath);
  
  // Check if the executable exists
  if (!fs.existsSync(exePath)) {
    dialog.showErrorBox('Error', `Optiviera executable not found at: ${exePath}`);
    app.quit();
    return;
  }

  // Kill any existing Optiviera processes on Windows
  if (process.platform === 'win32') {
    try {
      spawn('taskkill', ['/IM', 'Optiviera.exe', '/F'], { stdio: 'ignore' });
      // Wait a bit for processes to be killed
      setTimeout(() => startAspNetServer(), 500);
      return;
    } catch (error) {
      console.error('Error killing existing processes:', error);
    }
  }
  
  startAspNetServer();
}

function startAspNetServer() {
  // In production, files are in app.asar, so we need to extract them first
  const isPackaged = app.isPackaged;
  let appPath;
  
  if (isPackaged) {
    // Extract files from app.asar to a temporary directory
    const tempDir = path.join(os.tmpdir(), 'optiviera-temp');
    if (!fs.existsSync(tempDir)) {
      fs.mkdirSync(tempDir, { recursive: true });
    }
    appPath = tempDir;
    
    // Copy files from app.asar to temp directory
    const asarPath = path.join(process.resourcesPath, 'app.asar');
    if (fs.existsSync(asarPath)) {
      // Extract all files from asar
      const asar = require('asar');
      asar.extractAll(asarPath, appPath);
      
      // Move files from resources/app to root of temp directory
      const resourcesAppPath = path.join(appPath, 'resources', 'app');
      if (fs.existsSync(resourcesAppPath)) {
        // Copy all files from resources/app to temp directory root
        const files = fs.readdirSync(resourcesAppPath);
        files.forEach(file => {
          const srcPath = path.join(resourcesAppPath, file);
          const destPath = path.join(appPath, file);
          if (fs.statSync(srcPath).isDirectory()) {
            // Copy directory recursively
            fs.cpSync(srcPath, destPath, { recursive: true });
          } else {
            // Copy file
            fs.copyFileSync(srcPath, destPath);
          }
        });
      }
    }
  } else {
    appPath = path.join(__dirname, 'resources', 'app');
  }
  
  const exeName = process.platform === 'win32' ? 'Optiviera.exe' : 'Optiviera';
  const exePath = path.join(appPath, exeName);
  
  console.log('App path:', appPath);
  console.log('Exe path:', exePath);
  console.log('Exe exists:', fs.existsSync(exePath));

  // Find an available port
  const net = require('net');
  const server = net.createServer();
  
  server.listen(0, () => {
    CONFIG.port = server.address().port;
    server.close();
    
    // Start ASP.NET Core process
    const env = { ...process.env };
    env.ASPNETCORE_URLS = `http://localhost:${CONFIG.port}`;
    env.ASPNETCORE_ENVIRONMENT = 'Production';
    env.ELECTRON_MODE = 'true';
    
    aspNetProcess = spawn(exePath, [], {
      cwd: appPath,
      env: env,
      stdio: ['ignore', 'pipe', 'pipe'],
      detached: false // Important: Keep process attached to parent
    });

    let startupLog = '';
    
    aspNetProcess.stdout.on('data', (data) => {
      const msg = data.toString();
      console.log(`ASP.NET: ${msg}`);
      startupLog += msg;
    });

    aspNetProcess.stderr.on('data', (data) => {
      const msg = data.toString();
      console.error(`ASP.NET Error: ${msg}`);
      startupLog += msg;
    });

    aspNetProcess.on('close', (code) => {
      console.log(`ASP.NET process exited with code ${code}`);
      console.log('Startup log:', startupLog);
      if (!isQuitting) {
        const errorMsg = `Optiviera application stopped with exit code ${code}.\n\nLog:\n${startupLog.substring(0, 500)}`;
        dialog.showErrorBox('Application Error', errorMsg);
        app.quit();
      }
    });

    aspNetProcess.on('error', (err) => {
      console.error('Failed to start ASP.NET process:', err);
      dialog.showErrorBox('Startup Error', `Failed to start Optiviera application.\n\nError: ${err.message}\n\nPath: ${exePath}`);
      app.quit();
    });

    // Wait for server to be ready, then load the page
    waitForServer(CONFIG.port);
  });
}

function waitForServer(port, attempts = 0) {
  const maxAttempts = 30; // 30 seconds max
  const http = require('http');
  
  http.get(`http://localhost:${port}`, (res) => {
    console.log(`Server is ready! Status: ${res.statusCode}`);
    if (mainWindow) {
      mainWindow.loadURL(`http://localhost:${port}`);
    }
  }).on('error', (err) => {
    if (attempts < maxAttempts) {
      console.log(`Waiting for server... attempt ${attempts + 1}/${maxAttempts}`);
      setTimeout(() => waitForServer(port, attempts + 1), 1000);
    } else {
      console.error('Server failed to start in time');
      dialog.showErrorBox('Server Timeout', 'The application server took too long to start. Please try again.');
      app.quit();
    }
  });
}

function getIconPath() {
  const iconName = process.platform === 'win32' ? 'icon.ico' : 
                   process.platform === 'darwin' ? 'icon.icns' : 'icon.png';
  return path.join(__dirname, iconName);
}

// App event handlers
app.whenReady().then(() => {
  // Show splash screen first
  createSplashScreen();
  
  // Then create main window (hidden)
  createWindow();

  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) {
      createSplashScreen();
      createWindow();
    } else if (mainWindow) {
      mainWindow.show();
    }
  });
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    isQuitting = true;
    app.quit();
  }
});

app.on('before-quit', () => {
  isQuitting = true;
  
  // Kill ASP.NET process properly
  if (aspNetProcess && !aspNetProcess.killed) {
    try {
      if (process.platform === 'win32') {
        // Windows: Force kill the process tree
        spawn('taskkill', ['/pid', aspNetProcess.pid, '/f', '/t']);
      } else {
        // Unix: Use SIGTERM first, then SIGKILL
        aspNetProcess.kill('SIGTERM');
        setTimeout(() => {
          if (aspNetProcess && !aspNetProcess.killed) {
            aspNetProcess.kill('SIGKILL');
          }
        }, 1000);
      }
    } catch (error) {
      console.error('Error killing ASP.NET process:', error);
    }
  }
});

app.on('will-quit', (event) => {
  // Ensure ASP.NET process is killed
  if (aspNetProcess && !aspNetProcess.killed) {
    event.preventDefault();
    
    if (process.platform === 'win32') {
      spawn('taskkill', ['/pid', aspNetProcess.pid, '/f', '/t']);
    } else {
      aspNetProcess.kill('SIGKILL');
    }
    
    setTimeout(() => {
      app.exit(0);
    }, 500);
  }
});

// Handle app activation (macOS)
app.on('activate', () => {
  if (mainWindow) {
    mainWindow.show();
  }
});

// IPC handlers
ipcMain.handle('get-app-version', () => {
  return app.getVersion();
});

ipcMain.handle('show-message-box', async (event, options) => {
  const result = await dialog.showMessageBox(mainWindow, options);
  return result;
});

ipcMain.handle('show-save-dialog', async (event, options) => {
  const result = await dialog.showSaveDialog(mainWindow, options);
  return result;
});

// Auto-updater (placeholder for future implementation)
ipcMain.handle('check-for-updates', async () => {
  // TODO: Implement update checking
  return { available: false };
});

// Prevent navigation to external URLs
app.on('web-contents-created', (event, contents) => {
  contents.on('will-navigate', (event, navigationUrl) => {
    const parsedUrl = new URL(navigationUrl);
    
    if (parsedUrl.origin !== `http://localhost:${CONFIG.port}`) {
      event.preventDefault();
      shell.openExternal(navigationUrl);
    }
  });
});
