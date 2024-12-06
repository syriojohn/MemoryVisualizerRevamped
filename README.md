# Memory Visualizer

A WPF-based application for visualizing hierarchical memory structures using Microsoft Automatic Graph Layout (MSAGL).

## Features

- Load and visualize hierarchical data from JSON files
- Load and visualize graph structures from DOT files
- Interactive graph visualization with zoom and pan capabilities
- Color-coded nodes based on type:
  - Company: Light Blue
  - Division: Light Green
  - Employee: Light Yellow
  - Product: Light Pink
  - Revenue: Light Gray
- Refresh functionality to re-layout graphs
- Comprehensive error logging

## Dependencies

### NuGet Packages
- Microsoft.Msagl (v1.1.6)
- Microsoft.Msagl.Drawing (v1.1.6)
- Microsoft.Msagl.GraphViewerGDI (v1.1.7)
- Newtonsoft.Json (v13.0.3)
- NLog (v5.2.7)

### Framework Requirements
- .NET 8.0
- Windows Forms Integration (for MSAGL viewer)

## Usage

1. Launch the application
2. Use the toolbar buttons to:
   - "Load JSON": Load hierarchical data from a JSON file
   - "Load DOT": Load graph data from a DOT file
   - "Refresh": Re-layout the current graph
3. Interact with the graph:
   - Zoom: Mouse wheel
   - Pan: Click and drag
   - Select nodes: Click on nodes
   - Move nodes: Click and drag nodes

## Sample Data

The application comes with sample files in the `SampleData` directory:
- `sample.json`: Example of hierarchical memory structure
- `sample.dot`: Example of graph structure in DOT format

## Known Issues

- The refresh operation may occasionally throw an exception, but the graph will still display correctly
- Window resizing may temporarily affect graph visibility

## Logging

Logs are stored in `%APPDATA%\MemoryVisualizer\logs` with detailed information about:
- File loading operations
- Graph creation and modification
- Error conditions and exceptions
- Layout operations
