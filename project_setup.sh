#!/usr/bin/env bash
# project_setup.sh - MonoGame DesktopGL project (x64, Contentless; adds root VS Code debug config)
set -euo pipefail

# Ensure x86_64 (Rosetta)
if [[ "$(uname -m)" != "x86_64" ]]; then
  exec arch -x86_64 /bin/bash "$0" "$@"
fi

DOTNET_X64="/usr/local/share/dotnet/x64/dotnet"
export DOTNET_ROOT="/usr/local/share/dotnet/x64"
export DOTNET_HOST_PATH="$DOTNET_X64"
export PATH="$DOTNET_ROOT:$PATH"
hash -r || true

[[ -x "$DOTNET_X64" ]] || { echo "x64 dotnet not found at $DOTNET_X64"; exit 1; }

usage(){ echo "Usage: $0 <ProjectName> [-s Solution.sln]"; exit 1; }
[[ $# -lt 1 ]] && usage

PROJECT_NAME="$1"; shift
export PROJECT_NAME
SOLUTION=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    -s|--sln|--solution)
      [[ $# -lt 2 ]] && { echo "Missing value after $1"; exit 1; }
      SOLUTION="$2"; shift 2;;
    *) echo "Unknown argument: $1"; usage;;
  esac
done

if [[ -z "$SOLUTION" ]]; then
  SLN_FOUND="$(find . -maxdepth 1 -type f -name '*.sln' -print -quit || true)"
  if [[ -n "$SLN_FOUND" ]]; then
    SOLUTION="${SLN_FOUND#./}"
  else
    SOLUTION="$(basename "$PWD").sln"
  fi
fi

[[ -f "$SOLUTION" ]] || "$DOTNET_X64" new sln -n "${SOLUTION%.sln}"

[[ -d "$PROJECT_NAME" ]] && { echo "Folder exists: $PROJECT_NAME"; exit 1; }
"$DOTNET_X64" new mgdesktopgl -n "$PROJECT_NAME" -o "./$PROJECT_NAME"
"$DOTNET_X64" sln "$SOLUTION" add "./$PROJECT_NAME/${PROJECT_NAME}.csproj"

pushd "$PROJECT_NAME" >/dev/null

CSPROJ="${PROJECT_NAME}.csproj"

###############################################
# 1) Contentless package setup                 #
###############################################
"$DOTNET_X64" add "$CSPROJ" package Contentless --version "4.0.0" >/dev/null
"$DOTNET_X64" remove "$CSPROJ" package MonoGame.Content.Builder.Task >/dev/null 2>&1 || true

sed -i '' -E \
  's#<PackageReference[[:space:]]+Include="Contentless"([^>/]*)/?>#<PackageReference Include="Contentless"\1>\n      <ExcludeAssets>build;buildTransitive</ExcludeAssets>\n    </PackageReference>#g' \
  "$CSPROJ"

if grep -q '<PackageReference[[:space:]]\+Include="Contentless"' "$CSPROJ" && \
   ! grep -q '<ExcludeAssets>build;buildTransitive</ExcludeAssets>' "$CSPROJ"; then
  sed -i '' -E \
    '/<PackageReference[[:space:]]+Include="Contentless"([^>]*)>/,/<\/PackageReference>/ s#</PackageReference>#      <ExcludeAssets>build;buildTransitive</ExcludeAssets>\n    </PackageReference>#' \
    "$CSPROJ"
fi

awk '
  BEGIN{seen=0; inblk=0}
  /<PackageReference[[:space:]]+Include="Contentless"/{
    if(seen==1){skip=1} else {seen=1; inblk=1}
  }
  { if(!skip) print }
  /<\/PackageReference>/{
    if(inblk==1){inblk=0; skip=0}
  }
' "$CSPROJ" > "$CSPROJ.tmp" && mv "$CSPROJ.tmp" "$CSPROJ"

###############################################
# 2) Add SpriteFontPlus                        #
###############################################
echo "Adding SpriteFontPlus (0.9.2)..."
"$DOTNET_X64" add "$CSPROJ" package SpriteFontPlus --version "0.9.2" >/dev/null

# Remove duplicate SpriteFontPlus references (keep only the first)
awk '
  BEGIN{seen=0; inblk=0}
  /<PackageReference[[:space:]]+Include="SpriteFontPlus"/{
    if(seen==1){skip=1} else {seen=1; inblk=1}
  }
  { if(!skip) print }
  /<\/PackageReference>/{
    if(inblk==1){inblk=0; skip=0}
  }
' "$CSPROJ" > "$CSPROJ.tmp" && mv "$CSPROJ.tmp" "$CSPROJ"

###############################################
# 3) Ensure Content/** copies to output        #
###############################################
grep -q 'Include="Content/\*\*"' "$CSPROJ" || \
sed -i '' -E 's#</Project>#  <ItemGroup>\n    <None Include="Content/**" CopyToOutputDirectory="PreserveNewest" />\n  </ItemGroup>\n</Project>#' "$CSPROJ"

###############################################
# 4) Ensure Debug builds emit symbols (PDB)    #
###############################################
if ! grep -q '<DebugType>portable</DebugType>' "$CSPROJ"; then
  sed -i '' -E 's#</Project>#  <PropertyGroup Condition="'"'"'$(Configuration)'"'"'=='"'"'Debug'"'"'">\n    <DebugType>portable</DebugType>\n    <DebugSymbols>true</DebugSymbols>\n    <Optimize>false</Optimize>\n  </PropertyGroup>\n</Project>#' "$CSPROJ"
fi

# Capture TargetFramework for root launch.json (defaults to net8.0 if not found)
TARGET_FRAMEWORK="$(grep -oE '<TargetFramework>[^<]+' "$CSPROJ" | sed 's/<TargetFramework>//' | head -n 1 || true)"
if [[ -z "$TARGET_FRAMEWORK" ]]; then
  TARGET_FRAMEWORK="net8.0"
fi

popd >/dev/null

"$DOTNET_X64" restore "./$PROJECT_NAME/${PROJECT_NAME}.csproj"

############################################
# Root VS Code .vscode/launch.json update   #
# (in higher_level_folder/.vscode)          #
# Adds a config that debugs the built DLL   #
############################################
export TARGET_FRAMEWORK
python3 <<'EOF'
import json, os

root = os.getcwd()  # higher_level_folder
proj = os.environ["PROJECT_NAME"]
tfm = os.environ.get("TARGET_FRAMEWORK", "net8.0")

vscode_dir = os.path.join(root, ".vscode")
os.makedirs(vscode_dir, exist_ok=True)
launch_path = os.path.join(vscode_dir, "launch.json")

if os.path.exists(launch_path):
    with open(launch_path, "r", encoding="utf-8") as f:
        try:
            data = json.load(f)
        except json.JSONDecodeError:
            data = {"version": "0.2.0", "configurations": []}
else:
    data = {"version": "0.2.0", "configurations": []}

configs = data.setdefault("configurations", [])

new_name = f"Debug {proj} DLL (x64)"
dll_path = f"${{workspaceFolder}}/{proj}/bin/Debug/{tfm}/{proj}.dll"

cfg = {
    "name": new_name,
    "type": "coreclr",
    "request": "launch",
    "program": "/usr/local/share/dotnet/x64/dotnet",
    "args": [dll_path],
    "cwd": f"${{workspaceFolder}}/{proj}",
    "console": "integratedTerminal",
    "stopAtEntry": false,
    "env": {
        "DOTNET_ROOT": "/usr/local/share/dotnet/x64",
        "DOTNET_HOST_PATH": "/usr/local/share/dotnet/x64/dotnet",
        "DOTNET_MULTILEVEL_LOOKUP": "0",
        "PATH": "/usr/local/share/dotnet/x64:${env:PATH}"
    }
}

# Replace existing config with same name or same dll path; otherwise append
def matches(existing):
    if existing.get("name") == new_name:
        return True
    args = existing.get("args") or []
    return any(dll_path == str(a) for a in args)

replaced = False
for i, existing in enumerate(configs):
    if matches(existing):
        configs[i] = cfg
        replaced = True
        break

if not replaced:
    configs.append(cfg)

with open(launch_path, "w", encoding="utf-8") as f:
    json.dump(data, f, indent=2)
    f.write("\n")
EOF

echo "✅ Created ${PROJECT_NAME} and added to ${SOLUTION}."
echo "   - Contentless configured"
echo "   - SpriteFontPlus (0.9.2) added"
echo "   - MGCB removed"
echo "   - Content/** auto-copies"
echo "   - Debug symbols ensured (portable PDB)"
echo "   - Root VS Code debug entry added: Debug ${PROJECT_NAME} DLL (x64)"