# Memory Visualizer

A WPF application for visualizing and debugging memory structures of running applications.

## Project Log

### Session 1 (Initial Setup) - [Current Date]
- Created WPF application with MSAGL integration
- Implemented basic node visualization
- Added sample business hierarchy visualization
- Features implemented:
  - Color-coded nodes by type (Company, Division, Employee, Product)
  - Revenue information in tooltips
  - Multiple layout options
  - Interactive graph manipulation

### Current Features
- **Visualization**
  - Node Types:
    - Company (Blue)
    - Division (Green)
    - Employee (Yellow)
    - Product (Pink)
  - Hierarchical data display
  - Interactive graph manipulation
  - Multiple layout options

- **Data Structure**
  - Company hierarchy
  - Sales data
  - Revenue tracking
  - Employee information

### Planned Features
- [ ] Export functionality (DOT/JSON formats)
- [ ] Memory inspection of target application
- [ ] Real-time data updates
- [ ] Advanced filtering options
- [ ] Search functionality
- [ ] Custom layout saving
- [ ] Node grouping
- [ ] Detailed node information panel

## Project Structure
MemoryVisualizer/
├── Models/
│   └── MemoryNode.cs          # Core data structure
├── MainWindow.xaml            # Main UI
├── MainWindow.xaml.cs         # Main logic
└── App.xaml                   # Application configuration

## Dependencies
- Microsoft.Msagl
- Microsoft.Msagl.Drawing
- Microsoft.Msagl.GraphViewerGDI
- .NET 8.0

## Setup Instructions
1. Install Visual Studio 2022 or later
2. Install .NET 8.0 SDK
3. Open solution
4. Restore NuGet packages
5. Build and run

## Development Guidelines
1. Document all major changes in the Project Log section
2. Keep track of feature implementations and modifications
3. Note any breaking changes or important updates
4. Document any dependencies added or modified
