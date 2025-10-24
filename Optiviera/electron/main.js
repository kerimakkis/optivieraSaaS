const { app, BrowserWindow, Menu, Tray, shell, ipcMain, dialog } = require('electron');
const path = require('path');
const { spawn } = require('child_process');
const fs = require('fs');
const os = require('os');
const { autoUpdater } = require('electron-updater');

let mainWindow;
let splashWindow;
let tray;
let aspNetProcess;
let isQuitting = false;

// Configure auto-updater
autoUpdater.autoDownload = false;
autoUpdater.autoInstallOnAppQuit = true;

// Auto-updater event handlers
autoUpdater.on('checking-for-update', () => {
  console.log('Checking for updates...');
});

autoUpdater.on('update-available', (info) => {
  console.log('Update available:', info.version);
  dialog.showMessageBox(mainWindow, {
    type: 'info',
    title: 'Update Available',
    message: `A new version (${info.version}) is available. Would you like to download it now?`,
    buttons: ['Download', 'Later'],
    defaultId: 0
  }).then((result) => {
    if (result.response === 0) {
      autoUpdater.downloadUpdate();
    }
  });
});

autoUpdater.on('update-not-available', () => {
  console.log('Update not available');
});

autoUpdater.on('error', (err) => {
  console.error('Update error:', err);
});

autoUpdater.on('download-progress', (progressObj) => {
  console.log(`Download progress: ${progressObj.percent}%`);
  if (mainWindow) {
    mainWindow.setProgressBar(progressObj.percent / 100);
  }
});

autoUpdater.on('update-downloaded', (info) => {
  console.log('Update downloaded:', info.version);
  if (mainWindow) {
    mainWindow.setProgressBar(-1); // Remove progress bar
  }

  dialog.showMessageBox(mainWindow, {
    type: 'info',
    title: 'Update Ready',
    message: `Version ${info.version} has been downloaded. The application will restart to install the update.`,
    buttons: ['Restart Now', 'Later'],
    defaultId: 0
  }).then((result) => {
    if (result.response === 0) {
      isQuitting = true;
      autoUpdater.quitAndInstall();
    }
  });
});

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

  // Handle window close button (X)
  mainWindow.on('close', (event) => {
    // Always quit when close button is clicked
    // This ensures ASP.NET process is properly terminated and database is unlocked
    isQuitting = true;
    // Don't prevent default - let the window close normally
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

async function startAspNetProcess() {
  // In production, files are in app.asar, so we need to extract them first
  const isPackaged = app.isPackaged;
  let appPath;

  if (isPackaged) {
    // Use AppData for persistent cache instead of temp directory
    const cacheDir = path.join(app.getPath('userData'), 'app-cache');
    const versionFile = path.join(cacheDir, '.version');
    const asarPath = path.join(process.resourcesPath, 'app.asar');

    // Get current asar file hash/timestamp for cache validation
    let needsExtraction = true;
    if (fs.existsSync(cacheDir) && fs.existsSync(versionFile)) {
      try {
        const cachedVersion = fs.readFileSync(versionFile, 'utf8');
        const currentStats = fs.statSync(asarPath);
        const currentVersion = `${currentStats.size}-${currentStats.mtimeMs}`;

        // Check if cache is still valid
        if (cachedVersion === currentVersion) {
          needsExtraction = false;
          console.log('Using cached application files');
        }
      } catch (error) {
        console.log('Cache validation failed, will re-extract:', error);
      }
    }

    if (needsExtraction) {
      console.log('Extracting application files to cache...');

      try {
        // Clean old cache
        if (fs.existsSync(cacheDir)) {
          fs.rmSync(cacheDir, { recursive: true, force: true });
        }
        fs.mkdirSync(cacheDir, { recursive: true });

        // Extract files from app.asar
        if (fs.existsSync(asarPath)) {
          const asar = require('asar');
          asar.extractAll(asarPath, cacheDir);

          // Move files from app-files to root of cache directory
          const appFilesPath = path.join(cacheDir, 'app-files');
          if (fs.existsSync(appFilesPath)) {
            const files = fs.readdirSync(appFilesPath);
            files.forEach(file => {
              const srcPath = path.join(appFilesPath, file);
              const destPath = path.join(cacheDir, file);
              if (fs.statSync(srcPath).isDirectory()) {
                fs.cpSync(srcPath, destPath, { recursive: true });
              } else {
                fs.copyFileSync(srcPath, destPath);
              }
            });

            // Clean up the app-files directory after moving files
            fs.rmSync(appFilesPath, { recursive: true, force: true });
          }

          // Save version info for cache validation
          const currentStats = fs.statSync(asarPath);
          const currentVersion = `${currentStats.size}-${currentStats.mtimeMs}`;
          fs.writeFileSync(versionFile, currentVersion, 'utf8');

          console.log('Application files extracted and cached successfully');

          // CRITICAL FIX: Wait for file system to flush all writes
          // This prevents "exit code 1" error on first run after installation
          // The delay ensures all extracted files are fully written to disk
          // before attempting to start the ASP.NET process
          const exeName = process.platform === 'win32' ? 'Optiviera.exe' : 'Optiviera';
          const exePath = path.join(cacheDir, exeName);

          // Wait and verify the main executable is accessible
          let retries = 0;
          const maxRetries = 10;
          while (retries < maxRetries) {
            try {
              // Try to access the file to ensure it's fully written and unlocked
              fs.accessSync(exePath, fs.constants.R_OK);
              console.log('Executable verified and accessible');
              break;
            } catch (err) {
              console.log(`Waiting for executable to be ready (attempt ${retries + 1}/${maxRetries})...`);
              // Async sleep - allows event loop to continue
              await new Promise(resolve => setTimeout(resolve, 500));
              retries++;
            }
          }
        }
      } catch (extractError) {
        console.error('FATAL: Extraction failed:', extractError);
        // Don't quit here - let the exe check below handle it
        // This way we'll get a clearer error message
      }
    }

    appPath = cacheDir;
  } else {
    appPath = path.join(__dirname, 'app-files');
  }

  const exeName = process.platform === 'win32' ? 'Optiviera.exe' : 'Optiviera';
  const exePath = path.join(appPath, exeName);

  console.log('Checking exe path:', exePath);
  console.log('App path contents:', fs.existsSync(appPath) ? fs.readdirSync(appPath).slice(0, 20) : 'NOT FOUND');

  // Check if the executable exists
  if (!fs.existsSync(exePath)) {
    const errorDetails = `Optiviera executable not found at: ${exePath}\n\nApp path: ${appPath}\nApp path exists: ${fs.existsSync(appPath)}\nCache dir files: ${fs.existsSync(appPath) ? fs.readdirSync(appPath).length : 0}`;
    console.error(errorDetails);
    dialog.showErrorBox('Error', errorDetails);
    app.quit();
    return;
  }

  console.log('Exe found! Size:', fs.statSync(exePath).size, 'bytes');

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
  // Use the same caching logic as startAspNetProcess
  const isPackaged = app.isPackaged;
  let appPath;

  if (isPackaged) {
    // Use the cached directory (already extracted by startAspNetProcess)
    appPath = path.join(app.getPath('userData'), 'app-cache');
  } else {
    appPath = path.join(__dirname, 'app-files');
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
    console.log(`Starting ASP.NET on port ${CONFIG.port}`);
    console.log(`Working directory: ${appPath}`);
    console.log(`Executable: ${exePath}`);

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

    console.log(`ASP.NET process spawned with PID: ${aspNetProcess.pid}`);

    let startupLog = '';
    let logStartTime = Date.now();

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
      const runTime = Date.now() - logStartTime;
      console.log(`ASP.NET process exited with code ${code} after ${runTime}ms`);
      console.log('Full startup log:', startupLog);
      if (!isQuitting) {
        // Show last 3000 chars of log to see the actual error
        const logToShow = startupLog.length > 3000 ? '...' + startupLog.substring(startupLog.length - 3000) : startupLog;
        const errorMsg = `Optiviera application stopped with exit code ${code}.\n\nRuntime: ${runTime}ms\n\nLog (last 3000 chars):\n${logToShow}`;
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

function waitForServer(port, attempts = 0, startTime = Date.now()) {
  const maxTimeout = 30000; // 30 seconds max
  const http = require('http');

  // Adaptive polling: fast at start, slower later
  // First 5 seconds: 100ms intervals (fast detection)
  // Next 10 seconds: 500ms intervals
  // Last 15 seconds: 1000ms intervals
  const elapsed = Date.now() - startTime;
  let pollInterval;
  if (elapsed < 5000) {
    pollInterval = 100;
  } else if (elapsed < 15000) {
    pollInterval = 500;
  } else {
    pollInterval = 1000;
  }

  http.get(`http://localhost:${port}`, (res) => {
    const totalTime = Date.now() - startTime;
    console.log(`Server is ready! Status: ${res.statusCode}, Time: ${totalTime}ms`);
    if (mainWindow) {
      mainWindow.loadURL(`http://localhost:${port}`);
    }
  }).on('error', (err) => {
    if (elapsed < maxTimeout) {
      console.log(`Waiting for server... attempt ${attempts + 1} (${elapsed}ms elapsed)`);
      setTimeout(() => waitForServer(port, attempts + 1, startTime), pollInterval);
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

  // Check for updates after 3 seconds (after app is fully loaded)
  setTimeout(() => {
    if (app.isPackaged) {
      autoUpdater.checkForUpdates().catch(err => {
        console.log('Auto-update check failed (this is normal if update server is not configured):', err.message);
      });
    }
  }, 3000);

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

app.on('before-quit', (event) => {
  isQuitting = true;

  // Kill ASP.NET process properly and WAIT for it
  if (aspNetProcess && !aspNetProcess.killed) {
    try {
      console.log('Killing ASP.NET process:', aspNetProcess.pid);

      if (process.platform === 'win32') {
        // Windows: Force kill the process tree synchronously
        const { execSync } = require('child_process');
        try {
          execSync(`taskkill /pid ${aspNetProcess.pid} /f /t`, { stdio: 'ignore', timeout: 3000 });
          console.log('ASP.NET process killed successfully');
        } catch (killError) {
          console.log('Taskkill error (process may already be dead):', killError.message);
        }

        // Also kill any orphaned Optiviera.exe processes
        try {
          execSync('taskkill /IM Optiviera.exe /f', { stdio: 'ignore', timeout: 3000 });
          console.log('All Optiviera.exe processes killed');
        } catch (cleanupError) {
          console.log('Cleanup error (no processes to kill):', cleanupError.message);
        }
      } else {
        // Unix: Use SIGKILL immediately
        aspNetProcess.kill('SIGKILL');
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

// Auto-updater
ipcMain.handle('check-for-updates', async () => {
  try {
    if (!app.isPackaged) {
      return { available: false, message: 'Updates only available in production' };
    }

    const result = await autoUpdater.checkForUpdates();
    return {
      available: result.updateInfo.version !== app.getVersion(),
      currentVersion: app.getVersion(),
      latestVersion: result.updateInfo.version
    };
  } catch (error) {
    console.error('Update check failed:', error);
    return { available: false, error: error.message };
  }
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
