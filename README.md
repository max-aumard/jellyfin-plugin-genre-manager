# Jellyfin Genre Manager Plugin

Plugin Jellyfin qui affiche automatiquement les genres de films sur la page d'accueil sous forme de rangées horizontales style Netflix.

## Fonctionnalités

- ✅ Affichage automatique des genres de votre bibliothèque
- ✅ Rangées horizontales style Netflix avec défilement
- ✅ Configuration des genres à afficher
- ✅ Ordre de priorité personnalisable
- ✅ Support films et séries
- ✅ Navigation vers les pages de genres

## Prérequis

- Jellyfin 10.8.0 ou supérieur
- .NET 6.0 SDK (pour compiler)

## Installation

### Option 1 : Installation depuis un fichier DLL

1. Téléchargez le fichier `Jellyfin.Plugin.GenreManager.dll`
2. Copiez-le dans le dossier plugins de Jellyfin :
   - **Windows**: `%ProgramData%\Jellyfin\Server\plugins\GenreManager\`
   - **Linux**: `/var/lib/jellyfin/plugins/GenreManager/`
   - **Docker**: `/config/plugins/GenreManager/`
3. Redémarrez Jellyfin

### Option 2 : Compilation depuis les sources

```bash
# Cloner ou copier le projet
cd Jellyfin.Plugin.GenreManager

# Restaurer les dépendances
dotnet restore

# Compiler le plugin
dotnet build --configuration Release

# Le fichier DLL sera dans bin/Release/net6.0/
```

## Configuration

1. Allez dans **Tableau de bord** → **Plugins** → **Genre Manager**
2. Sélectionnez les genres à afficher
3. Définissez l'ordre de priorité (1 = en haut)
4. Configurez le nombre d'éléments par section
5. Choisissez d'afficher uniquement les films ou films + séries
6. Sauvegardez et rafraîchissez la page d'accueil

## Intégration du JavaScript

Le plugin génère automatiquement le JavaScript nécessaire. Vous devez l'injecter dans la page d'accueil de Jellyfin.

### Méthode 1 : Injection manuelle

Ajoutez cette ligne dans le fichier `index.html` de Jellyfin (avant `</body>`) :

```html
<script src="/ClientScripts/genreDisplay.js"></script>
```

### Méthode 2 : Utilisation du plugin File Transformation

Si vous avez installé le plugin **File Transformation**, il injectera automatiquement le script.

## Structure du projet

```
Jellyfin.Plugin.GenreManager/
├── Plugin.cs                        # Classe principale du plugin
├── PluginConfiguration.cs            # Modèle de configuration
├── Controllers/
│   └── GenreController.cs            # API REST
├── Configuration/
│   ├── PluginPageInfo.cs             # Page de config
│   └── configPage.html               # Interface admin
└── ClientScripts/
    └── genreDisplay.js               # Frontend
```

## API Endpoints

Le plugin expose les endpoints suivants :

- `GET /api/genremanager/genres?userId={userId}` - Liste tous les genres disponibles
- `GET /api/genremanager/section/{genreName}?userId={userId}&limit=20` - Films d'un genre spécifique
- `GET /api/genremanager/configuration?userId={userId}` - Configuration active

## Développement

### Ajouter des genres

Les genres sont détectés automatiquement depuis :
- TMDb (The Movie Database)
- OMDb (Open Movie Database)
- TheTVDB
- Fichiers NFO locaux

### Personnaliser le style

Les styles CSS sont définis dans `genreDisplay.js`. Vous pouvez les modifier pour personnaliser l'apparence :

```javascript
// Dans genreDisplay.js, fonction injectStyles()
.horizontal-scroll .itemCard {
    width: 200px;  // Largeur des cartes
    // ... autres styles
}
```

## Dépannage

### Le plugin n'apparaît pas dans la liste

- Vérifiez que le fichier DLL est dans le bon dossier
- Redémarrez complètement Jellyfin
- Vérifiez les logs : `/var/log/jellyfin/` ou `%ProgramData%\Jellyfin\Server\log\`

### Les genres ne s'affichent pas sur la page d'accueil

- Vérifiez que le JavaScript est bien injecté
- Ouvrez la console du navigateur (F12) et vérifiez les erreurs
- Assurez-vous que des genres sont configurés dans le plugin

### Erreurs de compilation

- Vérifiez que vous utilisez .NET 6.0 SDK minimum
- Exécutez `dotnet restore` avant de compiler
- Vérifiez que les packages Jellyfin sont bien téléchargés

## Contributions

Les contributions sont les bienvenues ! N'hésitez pas à :
- Signaler des bugs
- Proposer des améliorations
- Ajouter des fonctionnalités

## Licence

Ce plugin est fourni tel quel, sans garantie. Utilisez-le à vos propres risques.

## Support

Pour toute question ou problème, consultez :
- La documentation Jellyfin : https://jellyfin.org/docs/
- Le forum Jellyfin : https://forum.jellyfin.org/

## Changelog

### Version 1.0.0
- Version initiale
- Affichage automatique des genres
- Configuration des genres et priorités
- Interface style Netflix
