# Destination instances and routes

ShareX 23 replaces the fixed image, text and file destinations with instances and routes. Requirements are tracked in [#8868](https://github.com/ShareX/ShareX/issues/8868).

## Model

The model lives in `ShareX.Destinations` (`net10.0`, no UI) so it can be tested on any OS.

* **Destination instance** (`DestinationInstance`): one configured uploader. It has a GUID `Id`, a `Name`, a `Category` (image, text or file), the `Uploader`, optional `Settings` (a JSON object of that uploader's settings), an optional `AccountIndex` for FTP accounts and custom uploaders, and optional `AcceptedFileTypes`. Any uploader, including custom uploaders, can be duplicated into more instances.
* **Default instance**: one per uploader. It has no settings of its own and uses the uploader settings as before.
* **File type** (`FileTypeDefinition`): premade Images, Videos, Audio, Text, Documents, Archives and Other files, plus custom types defined by a list of extensions.
* **Route** (`DestinationRoute`): a file type mapped to an instance. The default routes live in the default task settings. Workflows and hotkeys can override individual routes or tick Use default.

## Matching

`RouteResolver.Resolve` picks the route for a file:

1. A custom file type that lists the exact extension.
2. The premade type that contains the extension.
3. Other files.

Text uploads use `.txt` and screenshots use the extension of the configured image format. ShareX's own image and text extension settings still decide what counts as an image or text. A task override wins over the default route for the same file type. An override that points at a missing or incompatible instance falls back to the default route.

## User interface

* The Destinations menu shows one entry per route with its instance, and a radio list of compatible instances. Task menus add Use default.
* **Routes and instances** window: the Routes tab edits routes and custom file types. The Instances tab adds, renames, duplicates, removes and edits instances and their accepted file types.
* The after capture and before upload windows show the route that will be used as "File type → Instance".
* History stores `Route`, `DestinationInstance` and `DestinationInstanceId` tags.

## Migration

On first start `DestinationRouting.Migrate` creates:

* A default instance for every uploader, plus one instance per FTP account and custom uploader.
* Default routes from the existing destinations: Images to the image destination, Text to the text destination, everything else to the file destination.
* The same routes for every hotkey that did not use the default destinations.

The legacy `ImageDestination`, `TextDestination` and `FileDestination` fields are kept in step with the routes, so older ShareX versions and code paths that read them see the same choice. Nothing changes until the user edits a route.

## Tests

`ShareX.Destinations.Tests` covers matching, overrides, compatibility, editing and JSON round trips.
