# XNoCAD
XNoCAD is a PCB CAD designer software, similar to KiCad, DipTrace, etc

[![Build and test on push or pull request](https://github.com/mihai-ene-public/cadide/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/mihai-ene-public/cadide/actions/workflows/build-and-test.yml)

## How to install
XNoCAD is currently available for **Windows** only.

To see what's new, see the [Releases Page](https://github.com/mihai-ene-public/cadide/releases)

- On the releases page, for the release of your choice, download the .zip file that starts with XNoCAD.
- Extract this .zip file in a folder of your choice.
- In this folder double click on IDE.exe.
- Optionally, create a Desktop shortcut.

## How to build
- Use Visual Studio 2022 Community or higher from [here](https://www.visualstudio.com/)
- Make sure you have **.NET desktop development** workload installed
- Right click on the project with the name **IDE** and choose *Build*
- Or, make **IDE** as startup project and run (F5)

## Features
- Exports Gerber and NC Drill files
- Exports BOM, Pick and place files and assembly drawings
- All export is done in one single operation which is called ***build***
- All ERC and DRC is done in a single operation called ***compile***
- You have a solution with different projects
- These projects are various types: Gerber (for manufacturing outputs) and Library
- These projects can depend on one another or can depend on other external libraries
- A library can depend on another library
- All symbols, footprints, components, etc have their own separate file.
- All files are source control friendly, they are XML text.
- External libraries are built and deployed in a similar way is done in software development
    - All files mentioned above are like source files
    - What gets built is like a binary file ( .dll, .exe, .bin etc)
- Manufacturing outputs work on the same concept of build
- You could have multiple schematics and multiple boards in the same project
- Boards can refer to the same schematic (if you want to have different board layers of the same circuit)
- Schematics
    - have multiple sheets
    - supports filter selection
    - can define net classes
    - can define electrical rules for ERC
    - can inspect the BOM when the circuit is designed
- Boards
    - can mask layers
    - can show single layer
    - can create layer groups, to show a specific set of layers at a time
    - can change a layer color
    - can toggle layer visibility
    - have 3D preview
        - can choose to show/hide parts
        - change solder mask color, etc
    - can define board rules for DRC
        - they work in a similar way as CSS: a rule will be overriden by the one below it
        - you can have generic rules and then refine to more specific; you can have very complex rules defined this way
        - rules are applied as filtered to items (e.g. trace width) or item pairs (electrical clearance)
    - can inspect BOM
    - can define the manufacturing outputs
        - Gerber and NC Drills
        - BOM, can specify the columns and group them
        - Assembly drawings and pick and place files
        - most of the outputs have a preview
- Footprints
    - can be defined manually or using a generator
    - have 3D preview
- What's novel to this software is that there is a model (3D) file type
- Models
    - can import other file types (.obj, .stl) like in other software
    - can define models using simple primitives (cubes, cylinders, etc)
    - can define models using generators

## Milestones for version v1.0
- Panelization
- Spice sim
- Advanced routing: diff pairs
- Length matching
- Import from Eagle and KiCad
- Localization
