(function() {
    'use strict';

    // Configuration
    const PLUGIN_CONFIG = {
        name: 'GenreManager',
        checkInterval: 100,
        maxChecks: 50
    };

    // Attendre que la page d'accueil soit chargée
    function waitForHomeTab() {
        let checks = 0;
        const interval = setInterval(function() {
            checks++;
            const homeTab = document.querySelector('.homeSectionsContainer') ||
                           document.querySelector('#homeTab');

            if (homeTab) {
                clearInterval(interval);
                initializeGenreSections(homeTab);
            } else if (checks >= PLUGIN_CONFIG.maxChecks) {
                clearInterval(interval);
                console.error('GenreManager: Conteneur home introuvable');
            }
        }, PLUGIN_CONFIG.checkInterval);
    }

    // Initialiser les sections de genres
    async function initializeGenreSections(container) {
        try {
            const apiClient = ApiClient;
            const userId = apiClient.getCurrentUserId();

            // Récupérer la configuration des genres
            const response = await apiClient.fetch({
                url: apiClient.getUrl(`/api/genremanager/configuration?userId=${userId}`),
                type: 'GET'
            });

            const config = await response.json();

            // Créer une section pour chaque genre
            for (const genre of config.genres) {
                await createGenreSection(container, genre, userId);
            }

        } catch (error) {
            console.error('GenreManager: Erreur lors de l\'initialisation', error);
        }
    }

    // Créer une section de genre
    async function createGenreSection(container, genre, userId) {
        try {
            const apiClient = ApiClient;

            // Récupérer les éléments du genre
            const response = await apiClient.fetch({
                url: apiClient.getUrl(genre.apiEndpoint),
                type: 'GET'
            });

            const result = await response.json();

            if (!result.Items || result.Items.length === 0) {
                return; // Pas d'éléments, ne pas afficher la section
            }

            // Créer la section HTML
            const section = document.createElement('div');
            section.className = 'verticalSection genreSection';
            section.setAttribute('data-genre', genre.genreName);

            // En-tête cliquable
            const header = createClickableHeader(genre.genreName);
            section.appendChild(header);

            // Conteneur des cartes avec défilement horizontal
            const itemsContainer = document.createElement('div');
            itemsContainer.className = 'itemsContainer horizontal-scroll';

            // Créer les cartes pour chaque élément
            result.Items.forEach(item => {
                const card = createItemCard(item, apiClient);
                itemsContainer.appendChild(card);
            });

            section.appendChild(itemsContainer);
            container.appendChild(section);

        } catch (error) {
            console.error(`GenreManager: Erreur pour le genre ${genre.genreName}`, error);
        }
    }

    // Créer l'en-tête cliquable d'une section
    function createClickableHeader(genreName) {
        const header = document.createElement('div');
        header.className = 'sectionTitleContainer';

        const link = document.createElement('a');
        link.className = 'button-link emby-button';
        link.href = '#';
        link.style.textDecoration = 'none';
        link.onclick = function(e) {
            e.preventDefault();
            navigateToGenrePage(genreName);
        };

        const title = document.createElement('h2');
        title.className = 'sectionTitle';
        title.textContent = genreName;

        const arrow = document.createElement('span');
        arrow.textContent = ' ›';
        arrow.style.marginLeft = '10px';
        arrow.style.fontSize = '1.2em';

        title.appendChild(arrow);
        link.appendChild(title);
        header.appendChild(link);

        return header;
    }

    // Créer une carte d'élément
    function createItemCard(item, apiClient) {
        const card = document.createElement('div');
        card.className = 'card itemCard portraitCard';
        card.setAttribute('data-id', item.Id);

        const cardContent = document.createElement('a');
        cardContent.className = 'cardContent';
        cardContent.href = `#!/details?id=${item.Id}`;

        // Conteneur d'image
        const imageContainer = document.createElement('div');
        imageContainer.className = 'cardImageContainer';

        const img = document.createElement('img');
        img.className = 'cardImage';
        img.alt = item.Name;

        // Obtenir l'URL de l'image
        const imageUrl = apiClient.getImageUrl(item.Id, {
            type: 'Primary',
            maxWidth: 300,
            quality: 90
        });
        img.src = imageUrl;

        imageContainer.appendChild(img);
        cardContent.appendChild(imageContainer);

        // Texte de la carte
        const cardText = document.createElement('div');
        cardText.className = 'cardText';

        const cardTextCentered = document.createElement('div');
        cardTextCentered.className = 'cardTextCentered';
        cardTextCentered.textContent = item.Name;

        cardText.appendChild(cardTextCentered);
        cardContent.appendChild(cardText);

        card.appendChild(cardContent);

        return card;
    }

    // Naviguer vers la page de genre
    function navigateToGenrePage(genreName) {
        const apiClient = ApiClient;
        const userId = apiClient.getCurrentUserId();

        // Construire l'URL de navigation
        Dashboard.navigate(`#!/movies.html?genreIds=${encodeURIComponent(genreName)}`);
    }

    // Injecter les styles CSS
    function injectStyles() {
        const style = document.createElement('style');
        style.textContent = `
            /* Conteneur de défilement horizontal */
            .horizontal-scroll {
                display: flex;
                overflow-x: auto;
                overflow-y: hidden;
                scroll-behavior: smooth;
                -webkit-overflow-scrolling: touch;
                gap: 12px;
                padding: 0 3.5% 20px;
                scroll-snap-type: x mandatory;
            }

            /* Barre de défilement stylisée */
            .horizontal-scroll::-webkit-scrollbar {
                height: 8px;
            }

            .horizontal-scroll::-webkit-scrollbar-track {
                background: rgba(255, 255, 255, 0.1);
            }

            .horizontal-scroll::-webkit-scrollbar-thumb {
                background: rgba(255, 255, 255, 0.3);
                border-radius: 4px;
            }

            .horizontal-scroll::-webkit-scrollbar-thumb:hover {
                background: rgba(255, 255, 255, 0.5);
            }

            /* Cartes d'éléments */
            .horizontal-scroll .itemCard {
                flex: 0 0 auto;
                width: 200px;
                scroll-snap-align: start;
                transition: transform 0.3s ease, box-shadow 0.3s ease;
            }

            .horizontal-scroll .itemCard:hover {
                transform: scale(1.05);
                z-index: 100;
                box-shadow: 0 8px 16px rgba(0, 0, 0, 0.5);
            }

            /* En-tête de section */
            .genreSection .sectionTitle {
                cursor: pointer;
                transition: color 0.2s;
            }

            .genreSection .sectionTitle:hover {
                color: #00a4dc;
            }

            /* Responsive */
            @media (max-width: 768px) {
                .horizontal-scroll .itemCard {
                    width: 150px;
                }
            }

            @media (min-width: 1920px) {
                .horizontal-scroll .itemCard {
                    width: 250px;
                }
            }
        `;
        document.head.appendChild(style);
    }

    // Point d'entrée
    document.addEventListener('DOMContentLoaded', function() {
        injectStyles();
    });

    // Écouter l'événement de changement de vue
    document.addEventListener('viewshow', function(e) {
        if (e.detail && e.detail.type === 'home') {
            waitForHomeTab();
        }
    });

    // Initialiser au chargement si déjà sur la page d'accueil
    if (window.location.hash.includes('home') || window.location.hash === '#!/' || window.location.hash === '') {
        waitForHomeTab();
    }
})();
