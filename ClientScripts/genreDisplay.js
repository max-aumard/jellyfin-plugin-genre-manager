(function() {
    'use strict';

    console.log('[Genre Manager] Script loaded');

    let config = null;
    let apiClient = null;

    // Charger la configuration du plugin
    async function loadConfig() {
        try {
            const response = await fetch('/GenreManager/config');
            if (!response.ok) {
                console.error('[Genre Manager] Failed to load config:', response.status);
                return false;
            }
            config = await response.json();
            console.log('[Genre Manager] Config loaded:', config);
            return true;
        } catch (error) {
            console.error('[Genre Manager] Error loading config:', error);
            return false;
        }
    }

    // Attendre que l'API Jellyfin soit disponible
    function waitForApiClient() {
        return new Promise((resolve) => {
            const checkInterval = setInterval(() => {
                if (window.ApiClient) {
                    clearInterval(checkInterval);
                    apiClient = window.ApiClient;
                    console.log('[Genre Manager] ApiClient found');
                    resolve();
                }
            }, 100);
        });
    }

    // Créer une section de genre
    async function createGenreSection(container, genreName) {
        try {
            const userId = apiClient.getCurrentUserId();

            // Construire les paramètres de requête
            const params = {
                Genres: genreName,
                Limit: config.itemsPerSection,
                Recursive: true,
                SortBy: 'SortName',
                SortOrder: 'Ascending',
                Fields: 'PrimaryImageAspectRatio',
                ImageTypeLimit: 1,
                EnableImageTypes: 'Primary,Backdrop,Thumb'
            };

            // Filtrer par type si demandé
            if (config.showOnlyMovies) {
                params.IncludeItemTypes = 'Movie';
            } else {
                params.IncludeItemTypes = 'Movie,Series';
            }

            // Requête vers l'API Jellyfin
            const result = await apiClient.getItems(userId, params);

            if (!result.Items || result.Items.length === 0) {
                console.log(`[Genre Manager] No items found for genre: ${genreName}`);
                return;
            }

            console.log(`[Genre Manager] Found ${result.Items.length} items for genre: ${genreName}`);

            // Créer la section HTML
            const section = document.createElement('div');
            section.className = 'verticalSection genreManagerSection';
            section.setAttribute('data-genre', genreName);

            // En-tête
            const header = document.createElement('div');
            header.className = 'sectionTitleContainer';

            const titleLink = document.createElement('a');
            titleLink.className = 'button-link emby-button';
            titleLink.style.textDecoration = 'none';
            titleLink.href = `#!/movies.html?genreIds=${encodeURIComponent(genreName)}`;

            const title = document.createElement('h2');
            title.className = 'sectionTitle sectionTitle-cards';
            title.textContent = genreName;

            titleLink.appendChild(title);
            header.appendChild(titleLink);
            section.appendChild(header);

            // Conteneur horizontal scrollable
            const itemsContainer = document.createElement('div');
            itemsContainer.className = 'itemsContainer horizontal-scroll padded-top-focusscale padded-bottom-focusscale';
            itemsContainer.style.display = 'flex';
            itemsContainer.style.overflowX = 'auto';
            itemsContainer.style.overflowY = 'hidden';
            itemsContainer.style.scrollBehavior = 'smooth';
            itemsContainer.style.gap = '0.5em';

            // Créer les cartes
            result.Items.forEach(item => {
                const cardElement = createCard(item);
                itemsContainer.appendChild(cardElement);
            });

            section.appendChild(itemsContainer);
            container.appendChild(section);

        } catch (error) {
            console.error(`[Genre Manager] Error creating section for ${genreName}:`, error);
        }
    }

    // Créer une carte d'élément
    function createCard(item) {
        const card = document.createElement('div');
        card.className = 'card backdropCard backdropCard-scalable';
        card.style.minWidth = '230px';
        card.style.flex = '0 0 auto';

        const cardBox = document.createElement('div');
        cardBox.className = 'cardBox visualCardBox';

        const cardScalable = document.createElement('div');
        cardScalable.className = 'cardScalable visualCardBox-cardScalable';

        const cardPadder = document.createElement('div');
        cardPadder.className = 'cardPadder-backdrop';

        const cardContent = document.createElement('a');
        cardContent.className = 'cardContent cardContent-button cardImageContainer';
        cardContent.href = `#!/details?id=${item.Id}&serverId=${item.ServerId}`;

        // Image de la carte
        const cardImageContainer = document.createElement('div');
        cardImageContainer.className = 'cardImageContainer coveredImage coveredImage-noScale';

        const cardImage = document.createElement('div');
        cardImage.className = 'cardImage';

        let imageUrl = '';
        if (item.ImageTags && item.ImageTags.Primary) {
            imageUrl = apiClient.getImageUrl(item.Id, {
                type: 'Primary',
                tag: item.ImageTags.Primary,
                maxWidth: 400
            });
        }

        if (imageUrl) {
            cardImage.style.backgroundImage = `url('${imageUrl}')`;
            cardImage.style.backgroundSize = 'cover';
            cardImage.style.backgroundPosition = 'center';
        }

        cardImageContainer.appendChild(cardImage);
        cardContent.appendChild(cardImageContainer);

        cardPadder.appendChild(cardContent);
        cardScalable.appendChild(cardPadder);
        cardBox.appendChild(cardScalable);

        // Texte de la carte
        const cardFooter = document.createElement('div');
        cardFooter.className = 'cardFooter visualCardBox-cardFooter';

        const cardText = document.createElement('div');
        cardText.className = 'cardText';
        cardText.textContent = item.Name;

        cardFooter.appendChild(cardText);
        cardBox.appendChild(cardFooter);

        card.appendChild(cardBox);

        return card;
    }

    // Injecter les styles CSS
    function injectStyles() {
        const style = document.createElement('style');
        style.textContent = `
            /* Sections de genres */
            .genreManagerSection {
                margin-bottom: 2em;
            }

            /* Conteneur horizontal scrollable */
            .genreManagerSection .horizontal-scroll {
                -webkit-overflow-scrolling: touch;
                scrollbar-width: thin;
            }

            .genreManagerSection .horizontal-scroll::-webkit-scrollbar {
                height: 8px;
            }

            .genreManagerSection .horizontal-scroll::-webkit-scrollbar-track {
                background: rgba(255, 255, 255, 0.1);
            }

            .genreManagerSection .horizontal-scroll::-webkit-scrollbar-thumb {
                background: rgba(255, 255, 255, 0.3);
                border-radius: 4px;
            }

            .genreManagerSection .horizontal-scroll::-webkit-scrollbar-thumb:hover {
                background: rgba(255, 255, 255, 0.5);
            }

            /* Effet hover sur les cartes */
            .genreManagerSection .card:hover {
                transform: scale(1.05);
                transition: transform 0.2s ease;
                z-index: 10;
            }

            /* Titre cliquable */
            .genreManagerSection .sectionTitle:hover {
                color: #00a4dc;
            }
        `;
        document.head.appendChild(style);
        console.log('[Genre Manager] Styles injected');
    }

    // Attendre que le conteneur home apparaisse dans le DOM
    function waitForHomeContainer() {
        return new Promise((resolve) => {
            console.log('[Genre Manager] Waiting for home container...');

            // Liste des sélecteurs à essayer
            const selectors = [
                '.homeSectionsContainer',
                '.view[data-type="home"]',
                '#homeTab',
                '.page[data-type="home"]',
                '[data-role="page"]',
                '.view',
                'div[data-url*="home"]'
            ];

            // Fonction pour vérifier si un conteneur existe
            const checkContainer = () => {
                for (const selector of selectors) {
                    const element = document.querySelector(selector);
                    if (element) {
                        console.log('[Genre Manager] Found home container with selector:', selector);
                        console.log('[Genre Manager] Container:', element);
                        resolve(element);
                        return true;
                    }
                }
                return false;
            };

            // Vérifier immédiatement
            if (checkContainer()) {
                return;
            }

            // Si pas trouvé, utiliser MutationObserver
            console.log('[Genre Manager] Container not found, setting up MutationObserver...');
            const observer = new MutationObserver(() => {
                if (checkContainer()) {
                    observer.disconnect();
                }
            });

            observer.observe(document.body, {
                childList: true,
                subtree: true
            });

            // Timeout après 10 secondes
            setTimeout(() => {
                observer.disconnect();
                console.error('[Genre Manager] Timeout waiting for home container');
                resolve(null);
            }, 10000);
        });
    }

    // Initialiser les sections de genres
    async function initializeGenreSections() {
        console.log('[Genre Manager] Initializing genre sections');

        // Attendre que le conteneur home apparaisse
        const homeContainer = await waitForHomeContainer();

        if (!homeContainer) {
            console.error('[Genre Manager] Home container not found after waiting');
            return;
        }

        console.log('[Genre Manager] Home container ready, creating sections');

        // Créer un conteneur pour nos sections de genres
        let genreContainer = document.getElementById('genreManagerSections');
        if (!genreContainer) {
            genreContainer = document.createElement('div');
            genreContainer.id = 'genreManagerSections';
            genreContainer.className = 'sections';

            // Insérer au début du conteneur home
            if (homeContainer.firstChild) {
                homeContainer.insertBefore(genreContainer, homeContainer.firstChild);
            } else {
                homeContainer.appendChild(genreContainer);
            }
            console.log('[Genre Manager] Created genre container');
        }

        // Créer une section pour chaque genre sélectionné
        for (const genre of config.selectedGenres) {
            await createGenreSection(genreContainer, genre);
        }

        console.log('[Genre Manager] All sections created');
    }

    // Point d'entrée principal
    async function initialize() {
        console.log('[Genre Manager] Initializing...');

        // Attendre l'API Client
        await waitForApiClient();

        // Charger la configuration
        const configLoaded = await loadConfig();
        if (!configLoaded) {
            console.error('[Genre Manager] Failed to load configuration');
            return;
        }

        // Injecter les styles
        injectStyles();

        // Initialiser les sections
        await initializeGenreSections();
    }

    // Écouter l'événement viewshow pour la page d'accueil
    document.addEventListener('viewshow', function(e) {
        if (e.detail && e.detail.type === 'home') {
            console.log('[Genre Manager] Home page shown');
            initialize();
        }
    });

    // Initialiser si déjà sur la page d'accueil
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function() {
            if (window.location.hash.includes('home') || window.location.hash === '#!/' || window.location.hash === '') {
                console.log('[Genre Manager] DOMContentLoaded - initializing');
                initialize();
            }
        });
    } else {
        if (window.location.hash.includes('home') || window.location.hash === '#!/' || window.location.hash === '') {
            console.log('[Genre Manager] Document ready - initializing');
            initialize();
        }
    }

})();
