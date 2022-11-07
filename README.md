# RomTools

## Commands

### PruneRoms

This command will reduce the number of ROM files within a specific folder, attempting to remove
all redundant and unwanted files by performing the following steps.

1. Listing all files (not recursively)
2. Md5 hashing all files
3. Run the following filters on the hashed files,
  a. Filtering by extension
  b. Filtering by filename (see 'Filtering by Filename' below for more details)
  c. Filtering binary identical duplicates (will have different filenames)

Limitations:

* 7z archives contents hashing not currently supported
  * Disable hashing to workaround
* Zip archives contents hashing containing more than 1 file not currently supported (e.g Amiga archives can contain multiple files)
  * Disable hashing to workaround

## Filtering by Filename

This process assumes that your files are named using the following scheme,

https://www.emuparadise.me/help/romnames.php

The filename is used to assure that we filter out all files that you don't want. For example,

* Foreign languages
* Prototypes
* Unverified
* Hacks

> Please note that this process assumes your files are named correctly in the first place.
> At current RomTools does not offer any way to fix filename issues.  This may come at a
> later date if it is possible to do so.