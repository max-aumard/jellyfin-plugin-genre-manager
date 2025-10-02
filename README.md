# Jellyfin Genre Manager Plugin

A Jellyfin plugin that automatically displays movie genres on the home page as Netflix-style horizontal rows.

## Features

- ✅ Automatic genre detection from your Jellyfin library
- ✅ Netflix-style horizontal scrolling rows
- ✅ Configurable genre selection and priority
- ✅ Support for movies and/or TV series
- ✅ Responsive design with hover effects
- ✅ Easy navigation to genre pages

## Requirements

- Jellyfin 10.8.0 or higher
- .NET 6.0 SDK (for compilation)

## Installation

### 1. Install the Plugin

Download the latest `Jellyfin.Plugin.GenreManager.dll` from the [Releases](https://github.com/yourusername/jellyfin-plugin-genremanager/releases) page or compile from source.

**Windows:**
```powershell
mkdir "%ProgramData%\Jellyfin\Server\plugins\GenreManager"
copy Jellyfin.Plugin.GenreManager.dll "%ProgramData%\Jellyfin\Server\plugins\GenreManager\"
net stop JellyfinServer && net start JellyfinServer
```

**Linux:**
```bash
sudo mkdir -p /var/lib/jellyfin/plugins/GenreManager
sudo cp Jellyfin.Plugin.GenreManager.dll /var/lib/jellyfin/plugins/GenreManager/
sudo systemctl restart jellyfin
```

**Docker:**
```bash
docker cp Jellyfin.Plugin.GenreManager.dll jellyfin:/config/plugins/GenreManager/
docker restart jellyfin
```

### 2. Inject the JavaScript

Copy the JavaScript file to your Jellyfin web directory:

**Windows:**
```powershell
copy ClientScripts\genreDisplay.js "C:\Program Files\Jellyfin\Server\jellyfin-web\genreDisplay.js"
```

**Linux:**
```bash
sudo cp ClientScripts/genreDisplay.js /usr/share/jellyfin/web/genreDisplay.js
```

Then add the following line to your Jellyfin `index.html` (before `</body>`):
```html
<script src="/genreDisplay.js"></script>
```

**Note:** The `index.html` file locations:
- Windows: `C:\Program Files\Jellyfin\Server\jellyfin-web\index.html`
- Linux: `/usr/share/jellyfin/web/index.html`
- Docker: `/jellyfin/jellyfin-web/index.html`

### 3. Configure the Plugin

1. Go to **Dashboard** → **Plugins** → **Genre Manager**
2. Select the genres you want to display
3. Set the priority order (1 = top of page)
4. Choose the number of items per section
5. Select movies only or movies + series
6. Save and refresh your home page

## Building from Source

```bash
cd Jellyfin.Plugin.GenreManager
dotnet build --configuration Release
```

The compiled DLL will be in `bin/Release/net6.0/Jellyfin.Plugin.GenreManager.dll`

## API Endpoints

The plugin exposes the following REST endpoints:

- `GET /api/genremanager/genres?userId={userId}` - List all available genres
- `GET /api/genremanager/section/{genreName}?userId={userId}&limit=20` - Get items for a specific genre
- `GET /api/genremanager/configuration?userId={userId}` - Get active configuration

## Configuration

The plugin stores configuration in XML format at:
- Windows: `%ProgramData%\Jellyfin\Server\plugins\configurations\Jellyfin.Plugin.GenreManager.xml`
- Linux: `/var/lib/jellyfin/plugins/configurations/Jellyfin.Plugin.GenreManager.xml`

Configuration options:
- `SelectedGenres`: List of genres to display
- `GenreOrdering`: Priority order for each genre
- `ItemsPerSection`: Number of items per genre row (default: 20)
- `ShowOnlyMovies`: Display only movies or include TV series (default: true)

## Troubleshooting

### Plugin doesn't appear in the plugin list
- Verify the DLL is in the correct folder
- Check file permissions (Linux)
- Restart Jellyfin completely
- Check logs: `%ProgramData%\Jellyfin\Server\log\` (Windows) or `/var/log/jellyfin/` (Linux)

### Genres don't appear on the home page
- Verify JavaScript is injected in `index.html`
- Check browser console (F12) for errors
- Clear browser cache
- Ensure genres are configured in plugin settings
- Verify you have items in your library

### JavaScript not loading
- Check the file path in `index.html`
- Verify `genreDisplay.js` exists in the web directory
- Check file permissions

## Screenshots

After installation, your home page will display genre rows like this:

```
Action                                                           ›
[Movie1] [Movie2] [Movie3] [Movie4] [Movie5] [Movie6] [Movie7] →

Comedy                                                           ›
[Movie1] [Movie2] [Movie3] [Movie4] [Movie5] [Movie6] [Movie7] →

Drama                                                            ›
[Movie1] [Movie2] [Movie3] [Movie4] [Movie5] [Movie6] [Movie7] →
```

With horizontal scrolling, hover effects, and clickable genre titles.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built for [Jellyfin](https://jellyfin.org/)
- Uses Jellyfin API 10.8.x
- Inspired by Netflix's UI design

## Support

If you encounter issues:
1. Check the [Troubleshooting](#troubleshooting) section
2. Review Jellyfin logs
3. Open an issue on [GitHub](https://github.com/yourusername/jellyfin-plugin-genremanager/issues)

---

**Version:** 1.0.0
**Compatible with:** Jellyfin 10.8.0+
**Framework:** .NET 6.0
