
<a id="readme-top"></a>

<br />
<div align="center">


  <h3 align="center">Swengine</h3>

  <p align="center">
    Simple tool that helps you quickly download and apply live wallpapers on your wayland desktops using SWWW
    <br />
  </p>
</div>


![Screenshot_24-Aug_17-13-43_12857](https://github.com/user-attachments/assets/ddb7c47d-8cf8-48f9-952e-8bf14b9c51a7)
Swengine scrapes online providers of live wallpapers, downloads them, converts them to your chosen resolution and fps and applies them using the swww(https://github.com/LGFae/swww/) wallpaper daemon for wayland. It allows you to spice up your desktop with ease.
<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- GETTING STARTED -->
## Getting Started

Swengine is written in c#. You will require the dotnet SDK to build it.

### Dependencies

Runtime dependencies include: 
```
vlc and/or libvlc-dev           -> (depends on your distro and package manager. Install both if both are available)
ffmpeg                          -> For conversions
swwww                            -> Wallpaper daemon for wayland that supports gif wallpapers 
  ```

Build dependencies include:
```
dotnet-sdk
```

### Installation
### Arch and arch based distros:

Swengine is available in the aur. You can install using your favourite aur helper:
```
yay -S swengine
```

### Build from source
You must install the dotnet-sdk to build from source. It is only required for the build. You can install it from your distro's package repositories:

<h3>Debian</h2>

```
sudo apt-get install dotnet-sdk-8.0
```

<h3>Arch and arch based distros</h3>


```
sudo pacman -S dotnet-sdk
```

After installing dotnet, you may install from source.

```
#clone project
git clone https://www.github.com/eugenenoble2005/swengine.git

#cd into project
cd swengine/swengine.desktop

#run install script
chmod +x ./install.sh && ./install.sh
```
You should see the desktop entry.

### Usage

## WLROOTS COMPOSITORS
Install swww for your distro. This tool does not manage the swww daemon. You must configure it appropriately before using this program. You should autostart the daemon for your wayland session.

```
yay -S swww-git
```
<h3>Example Hyprland Configuration</h3>

```
#start swww daemon on login
exec-once= swww-daemon --format xrgb &

#display last used wallpaper from cache
exec=swww restore
```

<h3>Example Sway Configuration</h3>

```
#start swww daemon on login
exec swww-daemon --format xrgb &

#diaplay last used wallpaper from cache
exec swww restore
```
Ensure the daemon is running before attempting to use this tool.

## KDE PLASMA AND GNOME
This tool has out of the box support for Plasma 6+ and GNOME. No additional packages are required including swww. The default wallpaper utility for these desktop environments are used. Simple change the backend drop down to KDE or GNOME and apply a wallpaper. 

<b>NOTE: GNOME DOES NOT HAVE SUPPORT FOR ANIMATED WALLPAPERS BY DEFAULT</b>


### Tips
<p>All downloaded wallpapers are saved in your Pictures/wallpapers folder should you need to share them or use them for other purposes.</p>

<p>Live wallpapers can take a very long time (2+ minutes sometimes) to apply. This is because they need to be downloaded, converted, compressed and cached. This tool gives you the option to select the resolution (360p to 4k), frame rate and duration of these wallpapers. Needless to say, the larger these values , the longer the initial time to apply and the larger the size of the wallpaper.</p>

<p>You can also chose to upload your own video or image files and set them as your wallpaper immediately. </p>
Available providers at the time of this commit:
<ul>
<li> https://www.motionbgs.com</li>
<li> https://www.moewalls.com</li>
<li> https://www.desktophut.com</li>
<li> https://www.wallhaven.cc</li>
<li> https://www.wallpaperscraft.com</li>
<li> https://www.wallpapers-clan.com</li>
</ul>
<h3>Scripts</h3>
<p>Somtimes, you might want to run a script or command when you apply a new wallpaper. Examples of such times may be when you want to generate a new color pallete for your desktop using wallust or pywal or when you want to copy your new wallpaper to a shared location so it can be used by your wayland screen locker. In any case , you can easily add custom commands that will run after swengine changes your wallpaper. 

</p>
<p>Clicking the scripts button on the toolbar will present you with a very simple text editor where you edit the shell script located at 
  
  ```$HOME/.swengine_after_run.sh```. 
  
  You can chose to edit this file anyway you want with any text editor. This shell script will be run when you've applied a new wallpaper. 
**IT GOES WITHOUT SAYING THAT YOUR COMPUTER WILL ATTEMPT TO RUN ANY COMMAND IT FINDS IN THIS FILE, FAULTY COMMANDS CAN DAMAGE YOUR COMPUTER , EXERCISE GREAT CAUTION HERE**. 
  Here is a simple example that runs wallust with the newely set wallpaper:
  
  ```
  #run wallust
  wallust run "$1"

  #copy wallpaper to shared location
  cp "$1" $HOME/.config/hypr/wallpaper_effects/.wallpaper_current
  ```

"$1" means the first paramter fed to the script which in this case will always be the full rooted path of the newely applied wallpaper.
</p>
