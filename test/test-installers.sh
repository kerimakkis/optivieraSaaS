#!/bin/bash

# Optiviera ERP v1.0.0 - Installer Test Suite
# Tests all installer packages EXCEPT OptivieraERP.exe
# Date: 24 Ekim 2025

# Don't exit on error - we want to run all tests
# set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Test results
TESTS_PASSED=0
TESTS_FAILED=0
TESTS_TOTAL=0

# Directories
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"
DOWNLOADS_DIR="$PROJECT_DIR/hosting/downloads"
TEST_MOUNT_DIR="/tmp/optiviera-test-mount"
TEST_RESULTS_DIR="$SCRIPT_DIR/results"
TEST_REPORTS_DIR="$SCRIPT_DIR/reports"
TEST_REPORT_FILE="$TEST_RESULTS_DIR/test-results-$(date +%Y%m%d-%H%M%S).txt"

# Create directories if they don't exist
mkdir -p "$TEST_RESULTS_DIR" "$TEST_REPORTS_DIR"

# Expected file sizes (in MB, approximate)
EXPECTED_WIN_EXE_SIZE=130
EXPECTED_MAC_INTEL_SIZE=188
EXPECTED_MAC_ARM_SIZE=180
EXPECTED_LINUX_APPIMAGE_SIZE=172
EXPECTED_LINUX_DEB_SIZE=113

# Tolerance (±10%)
SIZE_TOLERANCE=10

# Initialize test report
init_report() {
    echo "╔══════════════════════════════════════════════════════════════╗" > "$TEST_REPORT_FILE"
    echo "║   OPTIVIERA ERP v1.0.0 - INSTALLER TEST REPORT             ║" >> "$TEST_REPORT_FILE"
    echo "╚══════════════════════════════════════════════════════════════╝" >> "$TEST_REPORT_FILE"
    echo "" >> "$TEST_REPORT_FILE"
    echo "Test Date: $(date '+%Y-%m-%d %H:%M:%S')" >> "$TEST_REPORT_FILE"
    echo "Test Directory: $DOWNLOADS_DIR" >> "$TEST_REPORT_FILE"
    echo "" >> "$TEST_REPORT_FILE"
    echo "═══════════════════════════════════════════════════════════════" >> "$TEST_REPORT_FILE"
    echo "" >> "$TEST_REPORT_FILE"
}

# Log functions
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
    echo "[INFO] $1" >> "$TEST_REPORT_FILE"
}

log_success() {
    echo -e "${GREEN}[PASS]${NC} $1"
    echo "[PASS] $1" >> "$TEST_REPORT_FILE"
    TESTS_PASSED=$((TESTS_PASSED + 1))
    TESTS_TOTAL=$((TESTS_TOTAL + 1))
}

log_error() {
    echo -e "${RED}[FAIL]${NC} $1"
    echo "[FAIL] $1" >> "$TEST_REPORT_FILE"
    TESTS_FAILED=$((TESTS_FAILED + 1))
    TESTS_TOTAL=$((TESTS_TOTAL + 1))
}

log_warning() {
    echo -e "${YELLOW}[WARN]${NC} $1"
    echo "[WARN] $1" >> "$TEST_REPORT_FILE"
}

log_section() {
    echo ""
    echo -e "${BLUE}═══ $1 ═══${NC}"
    echo "" >> "$TEST_REPORT_FILE"
    echo "═══ $1 ═══" >> "$TEST_REPORT_FILE"
}

# Test file existence
test_file_exists() {
    local file="$1"
    local name="$2"

    log_info "Testing: $name"

    if [ -f "$file" ]; then
        log_success "File exists: $name"
        return 0
    else
        log_error "File NOT found: $name"
        return 1
    fi
}

# Test file size
test_file_size() {
    local file="$1"
    local expected_mb="$2"
    local name="$3"

    if [ ! -f "$file" ]; then
        log_error "Cannot test size: File not found - $name"
        return 1
    fi

    # Get actual size in MB
    local actual_bytes=$(stat -f%z "$file")
    local actual_mb=$((actual_bytes / 1024 / 1024))

    # Calculate tolerance range
    local min_mb=$((expected_mb * (100 - SIZE_TOLERANCE) / 100))
    local max_mb=$((expected_mb * (100 + SIZE_TOLERANCE) / 100))

    log_info "Size check: $name"
    log_info "  Expected: ~${expected_mb} MB (±${SIZE_TOLERANCE}%)"
    log_info "  Actual: ${actual_mb} MB"
    log_info "  Accepted range: ${min_mb}-${max_mb} MB"

    if [ "$actual_mb" -ge "$min_mb" ] && [ "$actual_mb" -le "$max_mb" ]; then
        log_success "Size OK: ${actual_mb} MB (within acceptable range)"
        return 0
    else
        log_warning "Size outside expected range: ${actual_mb} MB"
        return 0  # Warning, not failure
    fi
}

# Test file readability
test_file_readable() {
    local file="$1"
    local name="$2"

    if [ ! -f "$file" ]; then
        log_error "Cannot test readability: File not found - $name"
        return 1
    fi

    if [ -r "$file" ]; then
        log_success "File is readable: $name"
        return 0
    else
        log_error "File is NOT readable: $name"
        return 1
    fi
}

# Test DMG structure
test_dmg_structure() {
    local dmg_file="$1"
    local name="$2"

    log_info "Testing DMG structure: $name"

    if [ ! -f "$dmg_file" ]; then
        log_error "DMG file not found: $name"
        return 1
    fi

    # Test if DMG can be inspected
    if hdiutil imageinfo "$dmg_file" > /dev/null 2>&1; then
        log_success "DMG structure is valid: $name"
    else
        log_error "DMG structure is INVALID: $name"
        return 1
    fi

    # Get DMG info
    local dmg_info=$(hdiutil imageinfo "$dmg_file" 2>/dev/null | grep -E "(Format|Size|Checksum)")
    log_info "DMG Info:"
    echo "$dmg_info" | while IFS= read -r line; do
        log_info "  $line"
    done

    return 0
}

# Test DMG mount/unmount
test_dmg_mount() {
    local dmg_file="$1"
    local name="$2"

    log_info "Testing DMG mount/unmount: $name"

    if [ ! -f "$dmg_file" ]; then
        log_error "DMG file not found: $name"
        return 1
    fi

    # Create mount point
    mkdir -p "$TEST_MOUNT_DIR"

    # Try to mount
    if hdiutil attach "$dmg_file" -mountpoint "$TEST_MOUNT_DIR" -readonly -nobrowse > /dev/null 2>&1; then
        log_success "DMG mounted successfully: $name"

        # Check for .app bundle
        local app_count=$(find "$TEST_MOUNT_DIR" -name "*.app" -maxdepth 1 | wc -l)
        if [ "$app_count" -gt 0 ]; then
            log_success "Found .app bundle in DMG"

            # List app contents
            local app_path=$(find "$TEST_MOUNT_DIR" -name "*.app" -maxdepth 1 | head -1)
            log_info "App bundle: $(basename "$app_path")"

            # Check for required files
            if [ -f "$app_path/Contents/Info.plist" ]; then
                log_success "Info.plist found"
            else
                log_error "Info.plist NOT found"
            fi

            if [ -f "$app_path/Contents/MacOS/OptivieraERP" ]; then
                log_success "Main executable found"
            else
                log_error "Main executable NOT found"
            fi

            # Check for backend files (in app.asar or directory)
            if [ -f "$app_path/Contents/Resources/app.asar" ]; then
                log_success "Backend package found (app.asar)"

                # Check if asar/npx is available to verify contents
                if command -v npx >/dev/null 2>&1; then
                    # Check for Optiviera executable in asar
                    if npx --yes asar list "$app_path/Contents/Resources/app.asar" 2>/dev/null | grep -q "app-files/Optiviera"; then
                        log_success "Backend Optiviera executable found in asar"
                    else
                        log_warning "Could not verify backend in asar (may still be present)"
                    fi
                else
                    log_info "asar tool not available - skipping detailed backend check"
                fi
            elif [ -d "$app_path/Contents/Resources/app-files" ] || [ -d "$app_path/Contents/Resources/app" ]; then
                log_success "Backend files directory found"

                # Check for Optiviera executable (backend)
                if find "$app_path/Contents/Resources" -name "Optiviera" -type f 2>/dev/null | grep -q .; then
                    log_success "Backend Optiviera executable found"
                else
                    log_error "Backend Optiviera executable NOT found"
                fi
            else
                log_error "Backend files NOT found (neither asar nor directory)"
            fi
        else
            log_error "No .app bundle found in DMG"
        fi

        # Unmount
        if hdiutil detach "$TEST_MOUNT_DIR" > /dev/null 2>&1; then
            log_success "DMG unmounted successfully: $name"
        else
            log_error "Failed to unmount DMG: $name"
            return 1
        fi
    else
        log_error "Failed to mount DMG: $name"
        return 1
    fi

    # Cleanup
    rm -rf "$TEST_MOUNT_DIR"

    return 0
}

# Test AppImage structure
test_appimage_structure() {
    local appimage_file="$1"
    local name="$2"

    log_info "Testing AppImage structure: $name"

    if [ ! -f "$appimage_file" ]; then
        log_error "AppImage file not found: $name"
        return 1
    fi

    # Check if executable
    if [ -x "$appimage_file" ]; then
        log_success "AppImage is executable: $name"
    else
        log_error "AppImage is NOT executable: $name"
        return 1
    fi

    # Check file type
    local file_type=$(file "$appimage_file")
    if echo "$file_type" | grep -q "executable"; then
        log_success "AppImage has correct file type"
    else
        log_warning "AppImage file type unexpected: $file_type"
    fi

    # Try to extract metadata (if available)
    if "$appimage_file" --appimage-help > /dev/null 2>&1; then
        log_success "AppImage responds to commands"
    else
        log_info "AppImage --appimage-help not available (normal on macOS)"
    fi

    return 0
}

# Test Debian package structure
test_deb_structure() {
    local deb_file="$1"
    local name="$2"

    log_info "Testing Debian package structure: $name"

    if [ ! -f "$deb_file" ]; then
        log_error "Debian package not found: $name"
        return 1
    fi

    # Check if dpkg-deb is available (it is on macOS)
    if ! command -v dpkg-deb &> /dev/null; then
        log_warning "dpkg-deb not available - skipping detailed tests"
        log_info "Install: brew install dpkg"
        return 0
    fi

    # Test package info
    if dpkg-deb --info "$deb_file" > /dev/null 2>&1; then
        log_success "Debian package structure is valid: $name"

        # Get package info
        local pkg_info=$(dpkg-deb --info "$deb_file" 2>/dev/null)
        log_info "Package Info:"
        echo "$pkg_info" | head -10 | while IFS= read -r line; do
            log_info "  $line"
        done
    else
        log_error "Debian package structure is INVALID: $name"
        return 1
    fi

    # List contents
    log_info "Package contents (first 20 files):"
    dpkg-deb --contents "$deb_file" 2>/dev/null | head -20 | while IFS= read -r line; do
        log_info "  $line"
    done

    return 0
}

# Test Windows EXE structure
test_windows_exe_structure() {
    local exe_file="$1"
    local name="$2"

    log_info "Testing Windows EXE structure: $name"

    if [ ! -f "$exe_file" ]; then
        log_error "EXE file not found: $name"
        return 1
    fi

    # Check file type
    local file_type=$(file "$exe_file")
    if echo "$file_type" | grep -qi "PE32\|executable\|Windows"; then
        log_success "EXE has valid Windows executable format"
        log_info "  File type: $(echo "$file_type" | cut -d: -f2- | xargs)"
    else
        log_error "EXE does NOT have valid Windows format"
        log_info "  File type: $file_type"
        return 1
    fi

    # Check if it's a PE (Portable Executable) file
    if head -c 2 "$exe_file" | grep -q "MZ"; then
        log_success "EXE has valid PE (MZ) header"
    else
        log_error "EXE does NOT have valid PE header"
        return 1
    fi

    return 0
}

# Main test execution
main() {
    echo ""
    echo "╔══════════════════════════════════════════════════════════════╗"
    echo "║   OPTIVIERA ERP v1.0.0 - INSTALLER TEST SUITE              ║"
    echo "╚══════════════════════════════════════════════════════════════╝"
    echo ""

    init_report

    # Test Windows x64 EXE
    log_section "Windows x64 EXE Package"
    WIN_EXE="$DOWNLOADS_DIR/OptivieraERP.exe"
    test_file_exists "$WIN_EXE" "Windows x64 EXE"
    test_file_readable "$WIN_EXE" "Windows x64 EXE"
    test_file_size "$WIN_EXE" "$EXPECTED_WIN_EXE_SIZE" "Windows x64 EXE"
    test_windows_exe_structure "$WIN_EXE" "Windows x64 EXE"

    # Test macOS Intel DMG
    log_section "macOS Intel (x64) DMG Package"
    MAC_INTEL_DMG="$DOWNLOADS_DIR/Optiviera ERP-1.0.0.dmg"
    test_file_exists "$MAC_INTEL_DMG" "macOS Intel DMG"
    test_file_readable "$MAC_INTEL_DMG" "macOS Intel DMG"
    test_file_size "$MAC_INTEL_DMG" "$EXPECTED_MAC_INTEL_SIZE" "macOS Intel DMG"
    test_dmg_structure "$MAC_INTEL_DMG" "macOS Intel DMG"
    test_dmg_mount "$MAC_INTEL_DMG" "macOS Intel DMG"

    # Test macOS ARM64 DMG
    log_section "macOS ARM64 (Apple Silicon) DMG Package"
    MAC_ARM_DMG="$DOWNLOADS_DIR/Optiviera ERP-1.0.0-arm64.dmg"
    test_file_exists "$MAC_ARM_DMG" "macOS ARM64 DMG"
    test_file_readable "$MAC_ARM_DMG" "macOS ARM64 DMG"
    test_file_size "$MAC_ARM_DMG" "$EXPECTED_MAC_ARM_SIZE" "macOS ARM64 DMG"
    test_dmg_structure "$MAC_ARM_DMG" "macOS ARM64 DMG"
    test_dmg_mount "$MAC_ARM_DMG" "macOS ARM64 DMG"

    # Test Linux AppImage
    log_section "Linux ARM64 AppImage Package"
    LINUX_APPIMAGE="$DOWNLOADS_DIR/Optiviera ERP-1.0.0-arm64.AppImage"
    test_file_exists "$LINUX_APPIMAGE" "Linux AppImage"
    test_file_readable "$LINUX_APPIMAGE" "Linux AppImage"
    test_file_size "$LINUX_APPIMAGE" "$EXPECTED_LINUX_APPIMAGE_SIZE" "Linux AppImage"
    test_appimage_structure "$LINUX_APPIMAGE" "Linux AppImage"

    # Test Linux Debian package
    log_section "Linux ARM64 Debian Package"
    LINUX_DEB="$DOWNLOADS_DIR/optiviera-desktop_1.0.0_arm64.deb"
    test_file_exists "$LINUX_DEB" "Linux Debian Package"
    test_file_readable "$LINUX_DEB" "Linux Debian Package"
    test_file_size "$LINUX_DEB" "$EXPECTED_LINUX_DEB_SIZE" "Linux Debian Package"
    test_deb_structure "$LINUX_DEB" "Linux Debian Package"

    # Final summary
    log_section "TEST SUMMARY"
    echo ""
    echo "Total Tests: $TESTS_TOTAL"
    echo "Passed: $TESTS_PASSED"
    echo "Failed: $TESTS_FAILED"
    echo ""

    echo "" >> "$TEST_REPORT_FILE"
    echo "═══════════════════════════════════════════════════════════════" >> "$TEST_REPORT_FILE"
    echo "TEST SUMMARY" >> "$TEST_REPORT_FILE"
    echo "═══════════════════════════════════════════════════════════════" >> "$TEST_REPORT_FILE"
    echo "Total Tests: $TESTS_TOTAL" >> "$TEST_REPORT_FILE"
    echo "Passed: $TESTS_PASSED" >> "$TEST_REPORT_FILE"
    echo "Failed: $TESTS_FAILED" >> "$TEST_REPORT_FILE"
    echo "" >> "$TEST_REPORT_FILE"

    if [ $TESTS_FAILED -eq 0 ]; then
        echo -e "${GREEN}✅ ALL TESTS PASSED!${NC}"
        echo "✅ ALL TESTS PASSED!" >> "$TEST_REPORT_FILE"
        echo ""
        echo "Test report saved to: $TEST_REPORT_FILE"
        return 0
    else
        echo -e "${RED}❌ SOME TESTS FAILED!${NC}"
        echo "❌ SOME TESTS FAILED!" >> "$TEST_REPORT_FILE"
        echo ""
        echo "Test report saved to: $TEST_REPORT_FILE"
        return 1
    fi
}

# Run main function
main
