# Bilan du Projet Jellyfin Genre Manager Plugin

**Date d'analyse :** 2026-02-05
**Version actuelle :** 1.0.0.9
**Branche actuelle :** claude/migrate-to-main-InSy3

---

## 1. Vue d'ensemble du projet

### 1.1 Description
Le **Jellyfin Genre Manager Plugin** est un plugin pour Jellyfin qui affiche automatiquement des sections de genres de films et séries TV sur la page d'accueil, dans un style Netflix (rangées horizontales scrollables). Il s'intègre avec le plugin **Home Screen Sections** pour fournir des sections natives à Jellyfin.

### 1.2 Objectif
Permettre aux utilisateurs de Jellyfin de :
- Organiser leur page d'accueil par genres (Action, Comédie, Drame, etc.)
- Configurer quels genres afficher et combien d'éléments par section
- Filtrer par type de contenu (films uniquement ou films + séries)
- Intégration native et seamless avec l'interface Jellyfin via Home Screen Sections

---

## 2. Architecture technique

### 2.1 Stack technologique
- **Framework :** .NET 8.0
- **Target ABI :** Jellyfin 10.10.0.0
- **Dépendances principales :**
  - `Jellyfin.Model` v10.10.*
  - `Jellyfin.Controller` v10.10.*
  - `Newtonsoft.Json` v13.0.1

### 2.2 Structure du projet

```
jellyfin-plugin-genre-manager/
├── Configuration/
│   └── configPage.html          # Page de configuration du plugin dans Jellyfin
├── HomeScreen/
│   └── Sections/
│       ├── GenreSectionBase.cs  # Classe de base abstraite pour toutes les sections de genre
│       ├── ActionGenreSection.cs
│       ├── ComedyGenreSection.cs
│       ├── DramaGenreSection.cs
│       └── ... (20 sections au total)
├── Library/
│   ├── IHomeScreenManager.cs    # Interface pour le gestionnaire HomeScreen
│   └── IHomeScreenSection.cs    # Interface pour les sections HomeScreen
├── Model/
│   └── Dto/
│       └── HomeScreenSectionPayload.cs
├── Services/
│   └── GenreRegistrationTask.cs # Tâche planifiée pour enregistrer les sections au démarrage
├── Plugin.cs                     # Classe principale du plugin
├── PluginConfiguration.cs        # Configuration du plugin
├── manifest.json                 # Manifeste pour le catalogue de plugins Jellyfin
├── build.yaml                    # Configuration de build
└── README.md                     # Documentation utilisateur
```

### 2.3 Genres supportés (20 au total)
- **Films d'action** : Action, Adventure, Thriller, Western, War
- **Films familiaux** : Animation, Comedy, Family, Romance
- **Films sérieux** : Drama, Crime, Mystery, Biography, History
- **Films fantastiques** : Fantasy, Science Fiction, Horror
- **Documentaires** : Documentary, Sport, Music

---

## 3. Fonctionnement du plugin

### 3.1 Processus d'initialisation

1. **Chargement du plugin** : Jellyfin charge le plugin au démarrage
2. **Configuration** : Le plugin lit `PluginConfiguration` pour savoir quels genres sont activés
3. **Enregistrement des sections** :
   - `GenreRegistrationTask` (tâche planifiée) s'exécute au démarrage de Jellyfin
   - Attend 5 secondes pour que HomeScreen soit initialisé
   - Découvre toutes les classes héritant de `GenreSectionBase` via réflexion
   - Enregistre chaque section auprès de `IHomeScreenManager` en utilisant `RegisterResultsDelegate<T>()`

### 3.2 Architecture des sections de genre

#### GenreSectionBase (classe abstraite)
```csharp
public abstract class GenreSectionBase : IHomeScreenSection
{
    protected abstract string GenreName { get; }  // Ex: "Action", "Comedy"

    // Implémentation de IHomeScreenSection
    public string Section => $"Genre_{GenreName.Replace(" ", "")}";  // Ex: "Genre_Action"
    public QueryResult<BaseItemDto> GetResults(...) { ... }
    public abstract IHomeScreenSection CreateInstance(...);
}
```

#### Sections concrètes (ex: ActionGenreSection)
Chaque genre a sa propre classe qui hérite de `GenreSectionBase` et définit simplement :
- Le nom du genre via `GenreName`
- L'implémentation de `CreateInstance()` pour l'injection de dépendances

### 3.3 Récupération des résultats

Quand l'utilisateur charge la page d'accueil :
1. HomeScreen appelle `GetResults()` pour chaque section active
2. Le plugin :
   - Vérifie que le genre est activé dans la configuration
   - Construit une requête `InternalItemsQuery` avec :
     - Filtre de genre (`Genres = ["Action"]`)
     - Limite d'items (`config.ItemsPerSection`, défaut: 20)
     - Tri aléatoire (`OrderBy = Random`)
     - Type de contenu (films uniquement ou films + séries)
   - Retourne les résultats sous forme de `BaseItemDto[]`

---

## 4. Configuration

### 4.1 Paramètres configurables

**PluginConfiguration.cs** expose :
```csharp
public class PluginConfiguration
{
    public List<string> SelectedGenres { get; set; }  // Genres actifs
    public int ItemsPerSection { get; set; }          // Nombre d'items par section (défaut: 20)
    public bool ShowOnlyMovies { get; set; }          // Films seulement ou films + séries
}
```

### 4.2 Interface de configuration

**configPage.html** fournit une interface web dans Jellyfin permettant de :
- Cocher/décocher les genres à afficher (20 genres disponibles)
- Définir le nombre d'items par section (5-50)
- Activer/désactiver le filtre "Films uniquement"
- Sauvegarder avec redémarrage obligatoire de Jellyfin

---

## 5. Dépendances externes

### 5.1 Plugin requis : Home Screen Sections

**Critique** : Ce plugin ne peut pas fonctionner seul. Il nécessite absolument le plugin **Home Screen Sections** (v2.3.7+) qui :
- Fournit `IHomeScreenManager` pour enregistrer les sections
- Fournit `IHomeScreenSection` interface que nos sections implémentent
- Gère l'affichage réel des sections sur la page d'accueil
- Permet à l'utilisateur d'activer/désactiver/réorganiser les sections

### 5.2 Mécanisme d'intégration

Le plugin utilise **Dependency Injection** pour obtenir `IHomeScreenManager` :
```csharp
var homeScreenManager = _serviceProvider.GetService(typeof(IHomeScreenManager)) as IHomeScreenManager;
```

Si HomeScreen n'est pas installé ou si l'interface n'est pas trouvée, le plugin log un warning et ne fait rien.

---

## 6. Historique et évolution du plugin

### 6.1 Versions majeures (manifest.json)

Le projet a connu une évolution tumultueuse avec **10 versions** publiées entre v1.0.0.0 et v1.0.0.9 :

#### v1.0.0.0 (2025-10-02 19:15)
- Version initiale : intégration native avec Home Screen Sections

#### v1.0.0.1 à v1.0.0.4 (2025-10-02)
- **Problèmes de débogage** : ajout de logs extensifs, correction de payloads, augmentation des délais de démarrage (5s → 10s)
- Issues : sections ne s'affichaient pas, problèmes de communication avec HomeScreen

#### v1.0.0.5 (2025-10-02 22:47)
- **Réécriture complète** : implémentation native de `IHomeScreenSection`

#### v1.0.0.6 (2025-10-02 23:00)
- Tentative d'auto-découverte via scan de DLL dans le dossier config de HomeScreen

#### v1.0.0.7 (2025-10-02 23:05)
- Enregistrement automatique via `IPluginServiceRegistrator` et `IHostedService`

#### v1.0.0.8 (2025-10-03 00:30)
- Migration vers Jellyfin 10.10.x + .NET 8.0
- Utilisation de `IScheduledTask` pour l'enregistrement automatique

#### v1.0.0.9 (2025-10-03 00:53) - **VERSION ACTUELLE**
- Fix : matching de namespace correct pour auto-découverte par HomeScreen

### 6.2 Observations sur l'historique

**Problèmes récurrents identifiés** :
1. **Difficultés d'intégration avec HomeScreen** : 5 réécritures en une journée
2. **Problèmes de timing** : besoin de délais au démarrage (5 secondes)
3. **Problèmes de découverte** : tentatives de scan DLL, injection de dépendances, etc.
4. **Changements d'API Jellyfin** : migration de .NET 6.0 → .NET 8.0, Jellyfin 10.8 → 10.10

---

## 7. Points forts du projet

1. **Architecture propre** :
   - Séparation claire des responsabilités (Sections, Services, Configuration)
   - Utilisation de classes de base abstraites pour éviter la duplication de code
   - Injection de dépendances bien utilisée

2. **Configuration flexible** :
   - Interface web intuitive
   - Large choix de genres (20)
   - Paramètres pertinents (nombre d'items, type de contenu)

3. **Documentation** :
   - README complet avec instructions d'installation
   - Commentaires XML dans le code
   - Messages d'erreur clairs avec logging

4. **Compatibilité** :
   - Supporte Jellyfin 10.10 (dernière version)
   - .NET 8.0 (moderne)

---

## 8. Problèmes identifiés

### 8.1 Problèmes critiques

#### 1. **Le plugin ne fonctionne actuellement pas**
Selon le contexte fourni, le plugin est non fonctionnel. Les causes probables :

**a) Problèmes d'enregistrement avec HomeScreen**
- Dépendance stricte sur le timing (délai de 5s)
- Utilisation de réflexion fragile pour appeler `RegisterResultsDelegate<T>()`
- Pas de mécanisme de retry si l'enregistrement échoue

**Code problématique dans GenreRegistrationTask.cs:85-96** :
```csharp
var registerMethod = homeScreenManager.GetType()
    .GetMethods()
    .FirstOrDefault(m =>
        m.Name == "RegisterResultsDelegate" &&
        m.IsGenericMethod == false &&
        m.GetParameters().Length == 1);
```
Cette recherche de méthode par réflexion est fragile et peut échouer si l'API de HomeScreen change.

**b) Problèmes de namespace/interface**
Le changelog de v1.0.0.9 mentionne "Proper namespace matching", ce qui suggère que le plugin doit implémenter l'interface exacte de HomeScreen et non une copie locale.

**c) Absence de gestion d'erreur robuste**
Si HomeScreen n'est pas trouvé, le plugin log simplement un warning et ne fait rien. Il n'y a pas de notification à l'utilisateur.

#### 2. **Dépendance non gérée**
Le plugin requiert Home Screen Sections mais :
- Aucune vérification automatique de la présence du plugin
- Aucune validation de version compatible
- L'utilisateur n'est informé qu'après installation via un message statique dans l'UI

#### 3. **Problèmes de découplage**
Le plugin définit ses propres interfaces (`IHomeScreenManager`, `IHomeScreenSection`) qui sont censées correspondre à celles de HomeScreen. Cela crée :
- Risque de désynchronisation si HomeScreen change son API
- Duplication de définitions
- Confusion sur la source de vérité

### 8.2 Problèmes de conception

#### 1. **Classe Plugin.cs incohérente**
```csharp
public override string Description =>
    "Affiche automatiquement les genres de films en rangées horizontales style Netflix sur la page d'accueil";
```
Description en français alors que le reste du code et README sont en anglais.

#### 2. **build.yaml obsolète**
```yaml
version: "1.0.0.0"
targetAbi: "10.8.0.0"
framework: "net6.0"
```
Le fichier `build.yaml` n'a pas été mis à jour depuis la v1.0.0.0 alors que :
- Version actuelle : 1.0.0.9
- Target ABI actuel : 10.10.0.0
- Framework actuel : net8.0

#### 3. **Duplication de code**
Les 20 classes de genre sont quasi-identiques (ex: ActionGenreSection.cs) :
- Seul le `GenreName` change
- Chaque classe duplique les constructeurs et CreateInstance()
- Pourrait être simplifié avec une factory ou génériques

#### 4. **Limite hardcodée**
```csharp
public int? Limit => 1;  // GenreSectionBase.cs:35
```
Cette propriété retourne toujours `1`, mais le nombre réel d'items est défini par `config.ItemsPerSection`. Confusion possible.

### 8.3 Problèmes de maintenance

#### 1. **Aucun système de tests**
- Pas de tests unitaires
- Pas de tests d'intégration
- Difficile de valider les corrections

#### 2. **Historique de commits chaotique**
10 versions en moins de 5 heures suggère :
- Développement sans environnement de test
- Commits directs en production
- Manque de planification

#### 3. **Documentation incomplète**
- Pas de documentation technique sur l'architecture
- Pas de guide de contribution
- Pas d'explication sur le mécanisme d'intégration avec HomeScreen

---

## 9. Migration master → main

### 9.1 État actuel
- Branche distante : `origin/master`
- Branche locale créée : `main` (basée sur master)
- Branche de travail : `claude/migrate-to-main-InSy3`

### 9.2 Actions nécessaires pour finaliser la migration

**Pour terminer la migration, il faut :**

1. **Sur GitHub (interface web ou API)** :
   - Créer la branche `main` sur le remote
   - Définir `main` comme branche par défaut du repository
   - (Optionnel) Supprimer la branche `master` après vérification

2. **Mettre à jour les références** :
   - `manifest.json:9` : `"imageUrl": "https://raw.githubusercontent.com/max-aumard/jellyfin-plugin-genre-manager/master/logo.png"`
     → Changer `master` en `main`
   - README.md référence potentiellement des URLs avec `/master/`

**Note** : La restriction git actuelle ne permet pas de pousser directement vers `main` depuis la CLI. La migration doit être faite via l'interface GitHub.

---

## 10. Recommandations pour les prochaines étapes

### 10.1 Corrections critiques (priorité haute)

1. **Débugger l'intégration avec HomeScreen** :
   - Ajouter des logs détaillés pour comprendre pourquoi l'enregistrement échoue
   - Vérifier que HomeScreen est bien chargé avant d'enregistrer
   - Implémenter un mécanisme de retry
   - Valider que les interfaces correspondent exactement à celles de HomeScreen

2. **Supprimer les interfaces locales** :
   - Référencer directement les interfaces de HomeScreen via NuGet (si disponible)
   - Ou documenter clairement que ce sont des copies et ajouter des tests de validation

3. **Améliorer la gestion des dépendances** :
   - Vérifier au démarrage que HomeScreen est installé et actif
   - Logger une erreur claire si la dépendance manque
   - Potentiellement afficher une notification dans l'UI Jellyfin

### 10.2 Améliorations de code (priorité moyenne)

1. **Simplifier les sections de genre** :
   - Créer une factory pour générer les sections dynamiquement
   - Éliminer les 20 classes quasi-identiques
   - Exemple : `GenreSectionFactory.Create("Action", dependencies)`

2. **Mettre à jour build.yaml** :
   - Version, targetAbi, framework
   - Automatiser la synchronisation avec le .csproj

3. **Unifier la langue** :
   - Tout en anglais (code, comments, description, README)
   - Ou tout en français si c'est un projet personnel

4. **Ajouter des tests** :
   - Tests unitaires pour GenreSectionBase.GetResults()
   - Tests d'intégration avec un mock de IHomeScreenManager
   - Tests de configuration

### 10.3 Améliorations d'infrastructure (priorité basse)

1. **CI/CD** :
   - GitHub Actions pour build automatique
   - Tests automatiques sur PR
   - Publication automatique des releases

2. **Versioning sémantique** :
   - Adopter un processus de release plus structuré
   - Changelog automatique
   - Tags git synchronisés avec manifest.json

3. **Documentation** :
   - Ajouter un fichier ARCHITECTURE.md
   - Documenter le processus de développement local
   - Créer un CONTRIBUTING.md

---

## 11. Conclusion

### État actuel
Le **Jellyfin Genre Manager Plugin** est un projet bien intentionné avec une architecture propre, mais qui souffre de :
- **Problèmes d'intégration** avec le plugin HomeScreen (non fonctionnel actuellement)
- **Historique de développement chaotique** (10 versions en quelques heures)
- **Manque de tests** et de validation
- **Dépendance fragile** sur un plugin externe sans vérification robuste

### Potentiel
Le projet a un **bon potentiel** car :
- L'idée est pertinente (organisation par genre style Netflix)
- L'architecture de base est solide
- La configuration est flexible
- Compatible avec la dernière version de Jellyfin

### Prochaines étapes
1. **Débugger et corriger** l'intégration avec HomeScreen
2. **Finaliser la migration** master → main
3. **Refactoriser** pour simplifier et rendre le code plus maintenable
4. **Ajouter des tests** pour éviter les régressions
5. **Implémenter des fonctionnalités** supplémentaires une fois la base stable

---

**Fin du bilan - Prêt pour l'implémentation de nouvelles fonctionnalités**
