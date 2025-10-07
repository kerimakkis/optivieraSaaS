const { app, BrowserWindow, Menu, Tray, shell, ipcMain, dialog } = require('electron');
const path = require('path');
const { spawn } = require('child_process');
const fs = require('fs');
const os = require('os');

let mainWindow;
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
  const appPath = path.join(__dirname, 'resources', 'app');
  const exeName = process.platform === 'win32' ? 'Optiviera.exe' : 'Optiviera';
  const exePath = path.join(appPath, exeName);
  
  // Check if the executable exists
  if (!fs.existsSync(exePath)) {
    dialog.showErrorBox('Error', `Optiviera executable not found at: ${exePath}`);
    app.quit();
    return;
  }

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
      stdio: ['ignore', 'pipe', 'pipe']
    });

    aspNetProcess.stdout.on('data', (data) => {
      console.log(`ASP.NET: ${data}`);
    });

    aspNetProcess.stderr.on('data', (data) => {
      console.error(`ASP.NET Error: ${data}`);
    });

    aspNetProcess.on('close', (code) => {
      console.log(`ASP.NET process exited with code ${code}`);
      if (!isQuitting) {
        dialog.showErrorBox('Error', 'Optiviera application has stopped unexpectedly.');
        app.quit();
      }
    });

    aspNetProcess.on('error', (err) => {
      console.error('Failed to start ASP.NET process:', err);
      dialog.showErrorBox('Error', 'Failed to start Optiviera application.');
      app.quit();
    });

    // Wait a moment for the server to start, then load the page
    setTimeout(() => {
      if (mainWindow) {
        mainWindow.loadURL(`http://localhost:${CONFIG.port}`);
      }
    }, 3000);
  });
}

function getIconPath() {
  const iconName = process.platform === 'win32' ? 'icon.ico' : 
                   process.platform === 'darwin' ? 'icon.icns' : 'icon.png';
  return path.join(__dirname, iconName);
}

// App event handlers
app.whenReady().then(() => {
  createWindow();

  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) {
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
  
  // Kill ASP.NET process
  if (aspNetProcess) {
    aspNetProcess.kill();
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
