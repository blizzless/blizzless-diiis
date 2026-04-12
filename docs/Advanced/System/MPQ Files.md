# MPQ Files: A Detailed Technical Article

The MPQ file format (short for **Mo'PaQ**, or sometimes Multi-Purpose Quick), developed by Blizzard Entertainment, is a sophisticated archive format used in many of their classic games, including Diablo, StarCraft, Warcraft III, and World of Warcraft. MPQ archives are highly regarded for their flexibility, supporting compression, encryption, extensible metadata, file segmenting, and revision tracking. Here's a detailed technical overview:

## Core Structure and Technical Design

### 1. **Physical Layout**

- **Data Preceding Archive (Optional)**: MPQ files can be appended to EXE installers or other payloads and don't necessarily start at offset 0. The archive should always be aligned at a 512-byte boundary[[1]](https://www.zezula.net/en/mpq/mpqformat.html).
- **User Data Header (Optional)**: Some archives have a user data header that points to the MPQ start, often used in custom maps for newer Blizzard titles[[1]](https://www.zezula.net/en/mpq/mpqformat.html).
- **MPQ Header (Required)**: The MPQ header indicates version, offsets, and sizes of core tables and is mandatory. Its size varies by version (32 bytes for v1, 44 bytes for v2, 208 bytes for v4)[[1]](https://www.zezula.net/en/mpq/mpqformat.html).
- **Files/Resources**: Assets like sounds, graphics, maps, and more are stored—sometimes compressed, segmented, or encrypted[[2]](https://docs.aspose.net/file-formats/mpq/).
- **Special Files**: Listfile, attributes, digital signatures, and special tables can be present for additional metadata and validation[[1]](https://www.zezula.net/en/mpq/mpqformat.html).

### 2. **File Indexing and Hash Tables**

- MPQ uses a hash table for fast file lookup. When searching for a file, its lowercased filename is hashed, modulo the table size. If multiple files hash to the same index, they're chained in a "colliding hash cluster" with further hash values and locale codes used for disambiguation[[5]](http://www.zezula.net/en/mpq/techinfo.html)[[6]](https://encyclopedia.pub/entry/37738).
- The hash and block tables are commonly encrypted, using the file name as part of the key, making random decryption difficult without dictionaries or brute-force attacks[[6]](https://encyclopedia.pub/entry/37738).

### 3. **Compression and Encryption**

- MPQ archives can employ several compression algorithms: PKZIP, zlib, bzip2, LZMA, and even Huffman for some assets. Encryption can be applied both to index tables (hash/block) and file contents[[2]](https://docs.aspose.net/file-formats/mpq/)[[6]](https://encyclopedia.pub/entry/37738).
- Files may be stored sector-based (split into chunks typically 4KB, each possibly compressed and/or encrypted individually) or as a single compressed/encrypted unit if the file is small[[7]](https://deepwiki.com/ladislav-zezula/StormLib/3.1-reading-files).

### 4. **Format Revisions**

- **Version 1**: Used in early games, limited to 2GB archives.
- **Version 2**: Adds extended headers and hi-block tables for larger archives.
- **Version 3/4**: Add optional HET/BET tables, improve hash/block table capacities, and bring stronger metadata and signatures[[1]](https://www.zezula.net/en/mpq/mpqformat.html).

## Working with MPQ Files

- **Extraction & Editing**: Tools like MPQ Editor, StormLib (for coding solutions), and 7-Zip can open and manipulate MPQ files. Many modders use these tools to extract, replace, or repack assets, though modifying MPQs is unsupported by Blizzard and may cause game instability[[8]](https://pilotglossary.com/blog/mpq-editor-your-ultimate-guide)[[7]](https://deepwiki.com/ladislav-zezula/StormLib/3.1-reading-files).
- **StormLib**: A popular open-source library, StormLib provides APIs to open archives, read file contents (handling decompression/encryption automatically), add files to archives, and manage sector/single-unit files efficiently[[7]](https://deepwiki.com/ladislav-zezula/StormLib/3.1-reading-files)[[9]](https://deepwiki.com/ladislav-zezula/StormLib/1.1-mpq-archive-structure).

## Advanced Features

- **Internationalization & Patching**: MPQs can store multiple versions of files within the same archive to handle localization, platform differences, and game patches. This facilitates updates without needing full replacements of large archives[[6]](https://encyclopedia.pub/entry/37738).
- **Digital Signatures**: Later versions support strong cryptographic signatures to guarantee archive integrity.

## Feature Comparison Table

| Feature              | Description                                                                                      |
|----------------------|-----------------------------------------------------------------------------------------------------|
| Compression          | PKZIP, zlib, LZMA, bzip2, Huffman; file-by-file, sector-by-sector                                |
| Encryption           | Hash/block table plus file contents, key derived from filename                                   |
| File Indexing        | Encrypted hash table, multi-hash disambiguation, fast search via custom algorithm                |
| Patch Support        | Multiple file versions/resources, for updates/localization/platform differences                   |
| Header Structure     | Versioned headers, extensible tables, optional metadata                                          |
| Editing Tools        | MPQ Editor, StormLib, MPQ Extractor, manual extraction via 7-Zip                                 |
