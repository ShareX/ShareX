### Website: [getsharex.com](https://getsharex.com)

[![](https://getsharex.com/img/ShareX_Screenshot.png)](https://getsharex.com)

# Features

## Capturing

ShareX incorporates the following methods to allow screen capture.

* Fullscreen: Creates a screenshot of the entire screen area.
* Active window: Captures the currently active window.
* Active monitor: Captures the monitor area where the mouse cursor currently resides.
* Window menu: Has list of active windows so you can select which window to take screenshot of.
* Monitor menu: Has a list of monitors so the you can select which monitor to take screenshot from.
* Rectangle: Allows you to take screenshot from a single rectangle or multiple rectangular areas drawn by the mouse by dragging it from one corner of a rectangle to the diagonally opposite other corner of the rectangle.
* Rectangle (Objects): Allows you to take screenshot of a rectangle area or when you hovers window or an object it will automatically select rectangular area so you does not need to drag the area using the mouse.
* Rectangle (Annotate): This rectangle capture similar to Light version but also allows to perform drawing in the capture area.
* Rectangle (Light): Basic version of Rectangle capture designed for slow computers.
* Rounded Rectangle, Ellipse, Triangle, Diamond: Works similar to rectangle capture with the only difference being the shape.
* Polygon: Allows you to click points on screen to make polygon shape to capture areas inside it.
* Freehand: Allows you to draw areas similar to drawing with pencil and the inside area will be captured.
* Last Region: Will repeat the screen capture which was done last.
* Screen recording (FFmpeg): You can record a selected area on your screen or the entire screen. [FFmpeg](https://en.wikipedia.org/wiki/FFmpeg) allows you to record screen including sound and compress in real time using [x264](https://en.wikipedia.org/wiki/x264), [VP8](https://en.wikipedia.org/wiki/VP8), [Xvid](https://en.wikipedia.org/wiki/Xvid) etc.
* Screen recording (GIF): You can record a selected area on your screen or the entire screen in animated GIF.
* Auto capture: Allows you to automatically capture a screen area with specific time interval.

### After capture tasks

You can select any or all of these tasks to be automatically run after each screen capture.

* Add image effects / watermark: You can choose from over 37 image effects including watermark and apply them to an image.
* Open in image editor: Using [Greenshot](https://en.wikipedia.org/wiki/Greenshot) image editor to annotate image.
* Copy image to clipboard: Copies image to clipboard.
* Print image: Be able to print images with printer device.
* Save image to file: Saves image as file with your preferred image format.
* Save image to file as: Shows file dialog before saving so you can select where to write file to.
* Save thumbnail image to file: Saves resized image as file.
* Copy file to clipboard: Copies image file to clipboard.
* Copy file path to clipboard: Copies image file path to clipboard.
* Perform actions: You can automatically run other applications with image file path as the parameter so this way you can use Command-line interface applications to accomplish tasks which would have not been possible before. For example, you could open a screenshot in [Paint.NET](https://en.wikipedia.org/wiki/Paint.NET) before uploading it to a remote host.
* Upload image to host: Allows you to automatically upload image file to a host that you selected. For example, you could upload images to [Imgur](https://en.wikipedia.org/wiki/Imgur), [ImageShack](https://en.wikipedia.org/wiki/ImageShack), [Flickr](https://en.wikipedia.org/wiki/Flickr) or upload as a file to [Dropbox](https://en.wikipedia.org/wiki/Dropbox_(service)), [Amazon S3](https://en.wikipedia.org/wiki/Amazon_S3) etc.
* Delete file locally: Deletes local file.

## Uploading

ShareX has multiple ways to upload files.

* Upload file: Uploads file to selected host according to file data type.
* Upload folder: Uploads files inside folder.
* Upload from clipboard: ShareX will automatically detect clipboard format and select tasks accordingly. It will first check if clipboard data format is an image, text or file. If the data format is text then it can check whether it is a URL or plaintext. If it is a URL then it can automatically shorten the URL or upload URL contents by downloading the file from the URL and uploading the content. It can also check whether it is a folder so it can index the contents of the folder. These settings are customizable through “Task settings” and are disabled by default due to privacy reasons.
* Upload from URL: Downloads file from URL and uploads it to a selected host.
* Drag and drop upload (drop area or main window): You can drag and drop files to ShareX main window or to the drag & drop box in order to upload them.
* Shell context menu: In Windows you can right click file and select “Upload with ShareX” to upload that file.
* Send to (via rom Windows Explorer): Also when you right click file ShareX will be in “Send to” submenu.
* Watch folder: You can configure to watch specific folders so if new file appear in these folders that file will be automatically uploaded.

### After upload tasks

These tasks will automatically run after successful upload to any host.

* Shorten URL: Automatically shortens URL with selected URL shortener service.
* Share URL: To be able to share URL to URL sharing service.
* Copy URL to clipboard: Automatically copies URL to clipboard so it will be ready to share.
* Open URL: URL will be automatically opened in default browser.
* Show QR code window: URL will be shown as QR code in window. You can open URL in their mobile phone etc.

## Destinations

ShareX supports the following destinations.

### Image uploaders

* [Imgur](https://en.wikipedia.org/wiki/Imgur)
* [ImageShack](https://en.wikipedia.org/wiki/ImageShack)
* [TinyPic](https://en.wikipedia.org/wiki/TinyPic)
* [Flickr](https://en.wikipedia.org/wiki/Flickr)
* [Photobucket](https://en.wikipedia.org/wiki/Photobucket)
* [Picasa](https://en.wikipedia.org/wiki/Picasa)
* [Twitter](https://en.wikipedia.org/wiki/Twitter)
* [Chevereto](https://en.wikipedia.org/wiki/Chevereto)
* Hızlı Resim
* Custom image uploader
* File uploader

### Text uploaders

* [Pastebin](https://en.wikipedia.org/wiki/Pastebin)
* Paste2
* Slexy
* Pastee.org
* Paste.ee
* [GitHub Gist](https://en.wikipedia.org/wiki/GitHub#Gist)
* uPaste
* Hastebin
* Custom text uploader
* File uploader

### File uploaders

* [Dropbox](https://en.wikipedia.org/wiki/Dropbox_(service))
* FTP
* [OneDrive](https://en.wikipedia.org/wiki/OneDrive)
* [Google Drive](https://en.wikipedia.org/wiki/Google Drive)
* Copy
* [Box](https://en.wikipedia.org/wiki/Box_(company))
* [MEGA](https://en.wikipedia.org/wiki/Mega_(service))
* [Amazon S3](https://en.wikipedia.org/wiki/Amazon_S3)
* [ownCloud](https://en.wikipedia.org/wiki/ownCloud)
* [MediaFire](https://en.wikipedia.org/wiki/MediaFire)
* Pomf
* Gfycat
* Pushbullet
* MediaCrush
* [RapidShare](https://en.wikipedia.org/wiki/RapidShare)
* SendSpace
* Minus
* Ge.tt
* Hostr
* [JIRA](https://en.wikipedia.org/wiki/JIRA)
* Shared folder
* Email
* Custom file uploader

### URL shorteners

* [bitly](https://en.wikipedia.org/wiki/bitly)
* goo.gl
* is.gd
* [TinyURL](https://en.wikipedia.org/wiki/TinyURL)
* turl.ca
* yourls.org
* nl.cm
* adf.ly
* Custom URL shortener

### URL sharing services

* Email
* [Twitter](https://en.wikipedia.org/wiki/Twitter)
* [Facebook](https://en.wikipedia.org/wiki/Facebook)
* [Google+](https://en.wikipedia.org/wiki/Google+)
* [Reddit](https://en.wikipedia.org/wiki/Reddit)
* [Pinterest](https://en.wikipedia.org/wiki/Pinterest)
* [Tumblr](https://en.wikipedia.org/wiki/Tumblr)
* [LinkedIn](https://en.wikipedia.org/wiki/LinkedIn)
* [StumbleUpon](https://en.wikipedia.org/wiki/StumbleUpon)
* [Delicious](https://en.wikipedia.org/wiki/Delicious_(website))
* [VK](https://en.wikipedia.org/wiki/VK_(social network))
* Pushbullet

## Tools

Additional tools to make certain tasks more efficient.

* Screen color picker: As the name suggests, allows you to retrieve the color from anywhere on the screen and provide values of Hue, Saturation, Brightness, and RGB.
* Image editor: Based on [Greenshot](https://en.wikipedia.org/wiki/Greenshot) image editor. It offers functions such as ability to add annotations, highlighting or obfuscations to the screenshot. It allows to draw basic shapes (rectangles, ellipses, lines, arrows and freehand) and add text to a screenshot.
* Image effects: Allows to apply over 37 different image effects with their own settings to image. Edited images can be saved in PNG or other formats.
* Hash check: Allows you to check/compare file hash values.
* DNS changer: Allows you to quickly change computer DNS settings with popular DNS servers such as [Google DNS](https://en.wikipedia.org/wiki/Google_DNS).
* QR code: Open QR code window which you can enter text to get QR code of it. You can copy QR code image to your clipboard or save as file.
* Index folder: Allows you to share the index of a folder contents by uploading the index of the selected folder as text, html or xml.
* Ruler: Allows you to get X, Y, width, height, distance and angle information on screen.
* FTP client: Opens a basic FTP client you interface for the currently configured FTP account.
* Tweet message: Allows you to post message to Twitter.
* Monitor test: Allows you to render different colors on the screen which provides you the opportunity to test for bleeding and dead pixels on LCD monitors.