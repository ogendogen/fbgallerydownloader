# Facebook Gallery Downloader
Download all images from private Facebook conversation. Unfortunately, app is not user friendly and I guess it's not that easy to create one.
How to use it?

1. Extract our cookies from Facebook
* Open CookiesDownloader
* Wait for Chromium to download
* Login to Facebook
* Press any key to console app to save cookies
* Cookies are saved to cookies.dat
2. Move cookies.dat to the same directory with FBGalleryDownloader executable
3. Create config.json file in the same directory with FBGalleryDownloader executable. DOM elements aren't the same for every user and every conversation. Furthermore, Facebook DOM is compilated. You need to specify all for concrete conversation. You can generate query selectors with browser's dev tools. Config keys:
* OutputPath - path where images will be downloaded
* Condition - query selector of element to check if exists (it's best to choose element's property), processing stops if it doesn't exist
* ImageUrlSelector - query selector for image (src property)
* MovieUrlSelector - query selector for movie (src property)
* NextImageButtonClick - query selector with click action for "next image" button
* StartImageUrl - start image url (opened as gallery)
* CounterStart - counter for output images

You can start with example config.json attached in release packages.
All required dependencies are attached with executable.
