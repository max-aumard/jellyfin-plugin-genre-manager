# Jellyfin Genre Manager Plugin

A Jellyfin plugin that displays movie and TV show genres as Netflix-style sections on your home page via integration with the Home Screen Sections plugin.

## Features

- 🎬 **Genre-based home sections** - Display your media organized by genre
- 🎨 **Native Jellyfin look** - Perfectly integrated with Home Screen Sections plugin
- ⚙️ **Fully configurable** - Choose which genres to display
- 📺 **Movies & TV Series** - Support for both content types
- 🔢 **Customizable** - Set the number of items per section

## Requirements

**This plugin requires the [Home Screen Sections](https://github.com/IAmParadox27/jellyfin-plugin-home-sections) plugin to be installed and enabled.**

- Jellyfin 10.8.0 or newer
- Home Screen Sections plugin v2.3.7 or newer

## Installation

1. Install the **Home Screen Sections** plugin from the Jellyfin plugin catalog
2. Install **Genre Manager** from your plugin repository or manually:
   - Download the latest release from [GitHub Releases](https://github.com/max-aumard/jellyfin-plugin-genre-manager/releases)
   - Extract the zip file
   - Place the `.dll` file in your Jellyfin plugins folder
3. Restart Jellyfin
4. Go to **Dashboard → Plugins → Genre Manager** to configure

## Configuration

1. **Select genres**: Choose which genres you want to display on the home page
   - Available genres: Action, Adventure, Animation, Comedy, Crime, Documentary, Drama, Family, Fantasy, Horror, Mystery, Romance, Science Fiction, Thriller, Western, Biography, History, Music, War, Sport

2. **Items per section**: Set how many items to show in each genre section (default: 20)

3. **Content type**: Choose to show only movies or include TV series

4. **Save and restart**: After saving your configuration, restart Jellyfin for changes to take effect

5. **Enable sections in Home Screen Sections**:
   - Go to **Dashboard → Plugins → Home Screen Sections → Section Settings**
   - Enable the genre sections you want to appear (e.g., "Genre_Action", "Genre_Comedy")
   - Configure their display order and view mode (Landscape/Portrait)

## How It Works

Genre Manager registers genre-based sections with the Home Screen Sections plugin at startup. Each selected genre becomes available as a section that you can enable and configure in the Home Screen Sections settings.

The sections are fully native to Jellyfin - they use the same rendering and styling as built-in sections like "Continue Watching" and "Recently Added".

## Troubleshooting

### Sections not appearing

1. Make sure **Home Screen Sections** plugin is installed and enabled
2. Restart Jellyfin after changing Genre Manager configuration
3. Check that you've enabled the genre sections in **Home Screen Sections → Section Settings**
4. Verify that you have media items tagged with the selected genres

### No items in genre sections

- Ensure your media is properly tagged with genre metadata
- Genre names must match exactly (e.g., "Science Fiction" not "Sci-Fi")
- Run a library scan to update metadata

## License

MIT License - see LICENSE file for details

## Credits

Built with inspiration from the [Home Screen Sections](https://github.com/IAmParadox27/jellyfin-plugin-home-sections) plugin architecture.
