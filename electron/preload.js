const { contextBridge, ipcRenderer } = require('electron');

// Expose protected methods that allow the renderer process to use
// the ipcRenderer without exposing the entire object
contextBridge.exposeInMainWorld('electronAPI', {
  // App info
  getAppVersion: () => ipcRenderer.invoke('get-app-version'),
  
  // Dialogs
  showMessageBox: (options) => ipcRenderer.invoke('show-message-box', options),
  showSaveDialog: (options) => ipcRenderer.invoke('show-save-dialog', options),
  
  // Updates
  checkForUpdates: () => ipcRenderer.invoke('check-for-updates'),
  
  // Platform info
  platform: process.platform,
  
  // App events
  onAppReady: (callback) => {
    ipcRenderer.on('app-ready', callback);
  },
  
  // Window events
  onWindowFocus: (callback) => {
    ipcRenderer.on('window-focus', callback);
  },
  
  onWindowBlur: (callback) => {
    ipcRenderer.on('window-blur', callback);
  }
});

// Add some global utilities
window.addEventListener('DOMContentLoaded', () => {
  // Add desktop-specific CSS classes
  document.body.classList.add('desktop-app');
  document.body.classList.add(`platform-${process.platform}`);
  
  // Add desktop-specific behaviors
  if (process.platform === 'darwin') {
    document.body.classList.add('macos');
  } else if (process.platform === 'win32') {
    document.body.classList.add('windows');
  } else {
    document.body.classList.add('linux');
  }
});

// Handle external links
document.addEventListener('click', (event) => {
  const link = event.target.closest('a');
  if (link && link.href && !link.href.startsWith('http://localhost')) {
    event.preventDefault();
    // Let the main process handle external links
    window.open(link.href, '_blank');
  }
});
