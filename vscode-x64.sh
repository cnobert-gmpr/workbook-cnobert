#!/bin/bash
# Rosetta_DotNet_x64.command
# Launch VS Code under Rosetta with x64 dotnet, and write a log so we can see failures.

set -euo pipefail

LOG_FILE="$HOME/Desktop/vscode_rosetta_dotnet_log.txt"
: > "$LOG_FILE"

log()
{
  echo "$1" | tee -a "$LOG_FILE"
}

DOTNET_ROOT_X64="/usr/local/share/dotnet/x64"
DOTNET_X64="$DOTNET_ROOT_X64/dotnet"

# Try common VS Code app paths (Stable and Insiders)
VSCODE_APP_STABLE="/Applications/Visual Studio Code.app"
VSCODE_APP_INSIDERS="/Applications/Visual Studio Code - Insiders.app"

VSCODE_CODE_STABLE="$VSCODE_APP_STABLE/Contents/Resources/app/bin/code"
VSCODE_CODE_INSIDERS="$VSCODE_APP_INSIDERS/Contents/Resources/app/bin/code"

SCRIPT_DIR="$(cd -- "$(dirname -- "$0")" && pwd)"
cd "$SCRIPT_DIR" || exit 1

log "---- $(date) ----"
log "Script dir: $SCRIPT_DIR"
log "uname -m before rosetta: $(uname -m)"

# Relaunch under Rosetta if not already x86_64
if [[ "$(uname -m)" != "x86_64" ]]; then
  log "Relaunching under Rosetta..."
  exec arch -x86_64 /bin/bash "$0" "$@"
fi

log "uname -m after rosetta: $(uname -m)"

# Confirm x64 dotnet exists
if [[ ! -x "$DOTNET_X64" ]]; then
  log "ERROR: x64 dotnet not found at: $DOTNET_X64"
  log "Does this directory exist? ls -la /usr/local/share/dotnet"
  exit 1
fi

# Force x64 .NET for anything launched from this process
export DOTNET_ROOT="$DOTNET_ROOT_X64"
export PATH="$DOTNET_ROOT:$PATH"
export DOTNET_HOST_PATH="$DOTNET_X64"
unset DOTNET_ROOT_ARM64 || true
unset DOTNET_MULTILEVEL_LOOKUP || true
hash -r

log "dotnet resolved to: $(command -v dotnet)"
log "dotnet --info (filtered):"
(dotnet --info | grep -E "Architecture|Base Path|RID|Version" || true) | tee -a "$LOG_FILE"

# Quit VS Code fully
log "Quitting VS Code..."
osascript -e 'tell application "Visual Studio Code" to quit' >/dev/null 2>&1 || true
osascript -e 'tell application "Visual Studio Code - Insiders" to quit' >/dev/null 2>&1 || true
pkill -f "Visual Studio Code" >/dev/null 2>&1 || true

# Pick a VS Code "code" launcher if present
VSCODE_CODE=""
if [[ -x "$VSCODE_CODE_STABLE" ]]; then
  VSCODE_CODE="$VSCODE_CODE_STABLE"
  log "Found VS Code CLI (Stable): $VSCODE_CODE"
elif [[ -x "$VSCODE_CODE_INSIDERS" ]]; then
  VSCODE_CODE="$VSCODE_CODE_INSIDERS"
  log "Found VS Code CLI (Insiders): $VSCODE_CODE"
else
  log "No executable 'code' launcher found inside VS Code app bundles."
fi

# Launch method A: use code CLI (best, env inherited)
if [[ -n "$VSCODE_CODE" ]]; then
  log "Launching with code CLI under Rosetta..."
  arch -x86_64 "$VSCODE_CODE" -n "$SCRIPT_DIR" 2>&1 | tee -a "$LOG_FILE" || true
fi

# If VS Code didn't appear, launch method B: open the app under Rosetta (fallback)
# This may not preserve env vars, but at least verifies Rosetta launch works.
if ! pgrep -f "Visual Studio Code" >/dev/null 2>&1; then
  log "Fallback launch: arch -x86_64 open -n -a 'Visual Studio Code' ..."
  arch -x86_64 open -n -a "Visual Studio Code" "$SCRIPT_DIR" 2>&1 | tee -a "$LOG_FILE" || true
fi

log "Done. Log written to: $LOG_FILE"
exec bash -i