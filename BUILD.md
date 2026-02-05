# Guide de Build - Genre Manager Plugin

## Prérequis

- .NET 8.0 SDK ou supérieur
- Git

## Build local

### 1. Cloner le repository

```bash
git clone https://github.com/max-aumard/jellyfin-plugin-genre-manager.git
cd jellyfin-plugin-genre-manager
```

### 2. Restaurer les dépendances

```bash
dotnet restore
```

### 3. Builder le projet

```bash
dotnet build --configuration Release
```

Le DLL sera généré dans : `bin/Release/net8.0/Jellyfin.Plugin.GenreManager.dll`

## Créer une release pour le catalogue Jellyfin

### 1. Builder et packager

```bash
# Build en mode Release
dotnet build --configuration Release

# Créer un dossier temporaire
mkdir -p release-temp

# Copier les fichiers nécessaires
cp bin/Release/net8.0/Jellyfin.Plugin.GenreManager.dll release-temp/
cp bin/Release/net8.0/Jellyfin.Plugin.GenreManager.xml release-temp/ 2>/dev/null || true

# Créer l'archive ZIP
cd release-temp
zip -r ../GenreManager-v2.0.0.0.zip .
cd ..

# Nettoyer
rm -rf release-temp
```

### 2. Calculer le checksum MD5

```bash
md5sum GenreManager-v2.0.0.0.zip
```

Copier le hash MD5 et le mettre dans `manifest-test.json` à la place de `PLACEHOLDER_MD5_CHECKSUM`

### 3. Créer la release GitHub

```bash
# Créer un tag
git tag v2.0.0.0
git push origin v2.0.0.0

# Créer la release via l'interface GitHub ou avec gh CLI
gh release create v2.0.0.0 \
  GenreManager-v2.0.0.0.zip \
  --title "v2.0.0.0 - Clean Rebuild for Jellyfin 10.11" \
  --notes "## Version 2.0.0.0 - Clean Rebuild

### Changes
- Updated for Jellyfin 10.11.x compatibility
- Migrated to .NET 8.0
- Fixed manifest compatibility issues
- Ready for testing and feature additions

### Requirements
- Jellyfin 10.11.0+
- Home Screen Sections plugin v2.3.7+

### Installation
1. Install via catalog using manifest-test.json
2. Or manually: Extract ZIP to Jellyfin plugins folder and restart"
```

### 4. Tester le manifest

Une fois la release créée, vous pouvez ajouter le manifest de test à Jellyfin :

URL du manifest : `https://raw.githubusercontent.com/max-aumard/jellyfin-plugin-genre-manager/main/manifest-test.json`

## Installation manuelle (pour développement)

Si vous voulez tester sans passer par le catalogue :

1. Builder le projet (voir ci-dessus)
2. Copier le DLL vers le dossier plugins de Jellyfin :
   - Linux : `/var/lib/jellyfin/plugins/GenreManager/`
   - Windows : `%AppData%\Jellyfin\Server\plugins\GenreManager\`
   - Docker : `/config/plugins/GenreManager/`
3. Redémarrer Jellyfin

## Versions

- **Version actuelle** : 2.0.0.0
- **Target ABI** : Jellyfin 10.11.0.0
- **Framework** : .NET 8.0

## Notes

- Les anciennes versions (1.0.0.0 à 1.0.0.9) ciblaient Jellyfin 10.8/10.10 et sont obsolètes
- Utilisez `manifest-test.json` pour les tests en développement
- Utilisez `manifest.json` pour les releases de production
