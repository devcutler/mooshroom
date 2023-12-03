# mooshroom

A command-line tool to download JAR files from Mojang.

### Usage
```
$ mooshroom <game version> <type> [options]

mooshroom 1.0.0
Copyright (C) 2023 mooshroom

  -r, --raw                  (Default: false) Only output data. Good for scripts.

  -l, --list                 (Default: false) List available versions.

  -f, --filter               The type of version entries you want to filter for.

  -d, --download             (Default: false) Download selected version.

  -o, --output               (Default: minecraft_<type>.<game version>.jar) Output jar to a specific file.

  --help                     Display this help screen.

  --version                  Display version information.

  <game version> (pos. 0)    The version you want to list matching or download.

  <type> (pos. 1)            (Default: server) The kind of jar you want to download.

```

- `<game version>` defaults to `latest`
- `<type>` defaults to `server`, as I mostly expect to use this myself for downloading server jars.

#### Available filter types:
- `release`
- `snapshot`
- `release_candidate` (or `rc`) (internally, this doesn't exist, I just check the release id for the string `"rc"`)
- `old_alpha`
- `old_beta`